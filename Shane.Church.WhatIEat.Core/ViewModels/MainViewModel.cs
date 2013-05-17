using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.Utility.Core.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Shane.Church.WhatIEat.Core.Services;
using System.Threading.Tasks;
using Shane.Church.WhatIEat.Core.Exceptions;

namespace Shane.Church.WhatIEat.Core.ViewModels
{
	public class MainViewModel : ObservableObject
	{
		protected IRepository<IEntry> _repository;
		protected ISettingsService _settings;
		protected SyncService _syncService;

		public MainViewModel(IRepository<IEntry> repository, ISettingsService settings, SyncService sync)
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

			_dateEntries = new ObservableCollection<CalendarItemViewModel>();
			_dateEntries.CollectionChanged += _dateEntries_CollectionChanged;
			_summaryEntries = new ObservableCollection<CalendarItemViewModel>();
			_summaryEntries.CollectionChanged += _summaryEntries_CollectionChanged;
			_summaryGroups = new ObservableCollection<DateTime>();
			_summaryGroups.CollectionChanged += _summaryGroups_CollectionChanged;

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
					catch { }
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
				catch { }
			}
		}
	}
}
