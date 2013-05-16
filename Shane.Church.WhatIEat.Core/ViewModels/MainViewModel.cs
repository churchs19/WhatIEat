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

		void _dateEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged(() => DateEntries);
		}

		private ObservableCollection<CalendarItemViewModel> _dateEntries;
		public ObservableCollection<CalendarItemViewModel> DateEntries
		{
			get { return _dateEntries; }
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

		public void LoadData(DateTime startDate, DateTime endDate)
		{
			var specificStartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
			var specificEndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
			var entries = _repository.GetFilteredEntries(it => it.EntryDate >= specificStartDate && it.EntryDate <= specificEndDate);
			_dateEntries.Clear();
			foreach (var e in entries)
			{
				DateEntries.Add(new CalendarItemViewModel() { ItemDate = DateTime.SpecifyKind(e.EntryDate, DateTimeKind.Utc), ItemText = e.EntryText });
			}
		}
	}
}
