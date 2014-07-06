using GalaSoft.MvvmLight;
using Shane.Church.Utility.Core.Command;
using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Shane.Church.WhatIEat.Core.ViewModels
{
	public class MainViewModel : ObservableObject
	{
		protected IRepository<IEntry> _repository;
		protected ISettingsService _settings;
		protected SyncService _syncService;
		protected ILoggingService _log;

		public MainViewModel(IRepository<IEntry> repository, ISettingsService settings, SyncService sync, ILoggingService log)
		{
			if (repository == null)
				throw new ArgumentNullException("repository");
			_repository = repository;
			if (settings == null)
				throw new ArgumentNullException("settings");
			_settings = settings;
			if (sync == null)
				throw new ArgumentNullException("sync");
			_syncService = sync;
			if (log == null)
				throw new ArgumentNullException("log");
			_log = log;

			_dateEntries = new ObservableCollection<CalendarItemViewModel>();
			_dateEntries.CollectionChanged += _dateEntries_CollectionChanged;
			_summaryEntries = new ObservableCollection<CalendarItemViewModel>();
			_summaryEntries.CollectionChanged += _summaryEntries_CollectionChanged;
			_summaryGroups = new ObservableCollection<DateTime>();
			_summaryGroups.CollectionChanged += _summaryGroups_CollectionChanged;

#pragma warning disable 1998
			SyncCommand = new AsyncRelayCommand(async (o) =>
			{
				SyncRunning = true;
				await _syncService.Sync();
			}, null,
			async () =>
			{
				SyncRunning = false;
			},
			(ex) =>
			{
				//TODO: Handle Error
				SyncRunning = false;
				throw ex;
			});
		}
#pragma warning restore 1998

		void _summaryGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged(() => SummaryGroups);
		}

		void _summaryEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged(() => SummaryEntries);
		}

		void _dateEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged(() => DateEntries);
		}

		private ObservableCollection<CalendarItemViewModel> _dateEntries;
		public ObservableCollection<CalendarItemViewModel> DateEntries
		{
			get { return _dateEntries; }
		}

		private ObservableCollection<CalendarItemViewModel> _summaryEntries;
		public ObservableCollection<CalendarItemViewModel> SummaryEntries
		{
			get { return _summaryEntries; }
		}

		private ObservableCollection<DateTime> _summaryGroups;
		public ObservableCollection<DateTime> SummaryGroups
		{
			get { return _summaryGroups; }
		}

		private long _totalEntryCount;
		public long TotalEntryCount
		{
			get { return _totalEntryCount; }
		}

		public bool SyncEnabled
		{
			get { return _settings.LoadSetting<bool>("SyncEnabled"); }
		}

		private ICommand _syncCommand;
		public ICommand SyncCommand
		{
			get { return _syncCommand; }
			set
			{
				Set(() => SyncCommand, ref _syncCommand, value);
			}
		}

		private bool _syncRunning;
		public bool SyncRunning
		{
			get { return _syncRunning; }
			set
			{
				Set(() => SyncRunning, ref _syncRunning, value);
			}
		}

		public void Initialize()
		{
			_summaryEntries.Clear();
			LoadSummaryGroups();
			_dateEntries.Clear();
		}

		public void LoadSummaryGroups()
		{
			_summaryGroups.Clear();
			var keys = _repository.GetAllEntries()
								  .Select(it => new DateTime(it.EntryDate.Year, it.EntryDate.Month, 1, 0, 0, 0, DateTimeKind.Utc))
								  .Distinct()
								  .OrderByDescending(it => it);
			foreach (var key in keys)
				_summaryGroups.Add(key);
		}

		public void LoadNextSummaryItems()
		{
			if (SummaryEntries.Count == 0)
				_totalEntryCount = _repository.GetAllEntries().LongCount();
			if (SummaryEntries.Count < _totalEntryCount)
			{
				var entries = _repository.GetAllEntries()
										 .OrderByDescending(it => it.EntryDate)
										 .ThenByDescending(it => it.CreateDateTime)
										 .Skip(SummaryEntries.Count)
										 .Take(10);
				foreach (var e in entries)
				{
					var model = new CalendarItemViewModel() { ItemDate = DateTime.SpecifyKind(e.EntryDate, DateTimeKind.Utc), ItemText = e.EntryText };
					try
					{
						SummaryEntries.Add(model);
					}
					catch (Exception ex)
					{
						_log.LogException(ex);
					}
				}
			}
		}

		public void LoadData(DateTime startDate, DateTime endDate)
		{
			var specificStartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
			var specificEndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
			var entries = _repository.GetFilteredEntries(it => it.EntryDate >= specificStartDate && it.EntryDate <= specificEndDate);
			_dateEntries.Clear();
			foreach (var e in entries)
			{
				try
				{
					DateEntries.Add(new CalendarItemViewModel() { ItemDate = DateTime.SpecifyKind(e.EntryDate, DateTimeKind.Utc), ItemText = e.EntryText });
				}
				catch (Exception ex)
				{
					_log.LogException(ex);
				}
			}
		}
	}
}
