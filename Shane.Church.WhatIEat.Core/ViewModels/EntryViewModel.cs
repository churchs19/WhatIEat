using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.Utility.Core.Command;
using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Shane.Church.WhatIEat.Core.ViewModels
{
	public class EntryViewModel : ObservableObject
	{
		private IRepository<IEntry> _repository;

		public event ActionCompleteEventHandler SaveActionCompleted;
		public event ActionCompleteEventHandler RemoveActionCompleted;

		[Inject]
		public EntryViewModel(IRepository<IEntry> repository)
		{
			if (repository == null)
				throw new ArgumentNullException("repository");
			_repository = repository;

			_removeCommand = new RelayCommand(RemoveEntry);
			_saveCommand = new RelayCommand(SaveEntry);
		}

		private long _entryId;
		public long EntryId
		{
			get { return _entryId; }
			set
			{
				Set(() => EntryId, ref _entryId, value);
			}
		}

		private Guid _entryGuid;
		public Guid EntryGuid
		{
			get { return _entryGuid; }
			set
			{
				Set(() => EntryGuid, ref _entryGuid, value);
			}
		}

		private DateTime _entryDate;
		public DateTime EntryDate
		{
			get { return _entryDate; }
			set
			{
				Set(() => EntryDate, ref _entryDate, value);
			}
		}

		private string _entry;
		public string Entry
		{
			get { return _entry; }
			set
			{
				Set(() => Entry, ref _entry, value);
			}
		}

		private ICommand _removeCommand;
		public ICommand RemoveCommand
		{
			get
			{
				return _removeCommand;
			}
		}

		private ICommand _saveCommand;
		public ICommand SaveCommand
		{
			get {
				return _saveCommand;
			}
		}

		public void LoadEntry(Guid entryGuid)
		{
			var entry = _repository.GetFilteredEntries(it => it.EntryGuid == entryGuid).FirstOrDefault();
			if (entry != null)
			{
				EntryId = entry.EntryId;
				EntryDate = entry.EntryDate;
				Entry = entry.EntryText;
				EntryGuid = entry.EntryGuid;
			}
		}

		public void LoadEntry(IEntry entry)
		{
			EntryId = entry.EntryId;
			EntryDate = entry.EntryDate;
			Entry = entry.EntryText;
			EntryGuid = entry.EntryGuid;
		}

		public void RemoveEntry()
		{
			var entry = KernelService.Kernel.Get<IEntry>();
			entry.EntryDate = EntryDate.Date;
			entry.EntryGuid = EntryGuid;
			entry.EntryId = EntryId;
			entry.EntryText = Entry;
			_repository.DeleteEntry(entry);
			if (RemoveActionCompleted != null)
				RemoveActionCompleted(this, new EventArgs());
		}

		public void SaveEntry()
		{
			var entry = KernelService.Kernel.Get<IEntry>();
			entry.EntryDate = EntryDate.Date;
			entry.EntryGuid = EntryGuid;
			entry.EntryId = EntryId;
			entry.EntryText = Entry;
			_repository.AddOrUpdateEntry(entry);
			if (SaveActionCompleted != null)
				SaveActionCompleted(this, new EventArgs());
		}
	}
}
