﻿using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.Utility.Core.Command;
using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Input;

namespace Shane.Church.WhatIEat.Core.ViewModels
{
    public class DateEditViewModel : GalaSoft.MvvmLight.ObservableObject
    {
        private IRepository<IEntry> _repository;
        private IIAPService _iapService;

        public event ActionCompleteEventHandler AddActionCompleted;

        public DateEditViewModel()
            : this(KernelService.Kernel.Get<IRepository<IEntry>>(), KernelService.Kernel.Get<IIAPService>())
        {

        }

        [Inject]
        public DateEditViewModel(IRepository<IEntry> repository, IIAPService iapService)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            _repository = repository;
            if (iapService == null)
                throw new ArgumentNullException("iapService");
            _iapService = iapService;
            _entries = new ObservableCollection<EntryViewModel>();
            _entries.CollectionChanged += _entries_CollectionChanged;
            _addEntryCommand = new RelayCommand(AddEntry);
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (Set(() => SelectedDate, ref _selectedDate, value))
                {
                    LoadData(_selectedDate);
                }
            }
        }

        public MealTypeCollection MealTypes
        {
            get { return MealTypeCollection.GetCollection(); }
        }

        private ObservableCollection<EntryViewModel> _entries;
        public ObservableCollection<EntryViewModel> Entries
        {
            get { return _entries; }
        }

        void _entries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => Entries);
        }

        private string _newEntry;
        public string NewEntry
        {
            get { return _newEntry; }
            set
            {
                Set(() => NewEntry, ref _newEntry, value);
            }
        }

        private MealTypeViewModel _mealType = new MealTypeViewModel(Data.MealType.Undefined);
        public MealTypeViewModel MealType
        {
            get { return _mealType; }
            set
            {
                Set(() => MealType, ref _mealType, value);
            }
        }

        public bool AreAdsVisible
        {
            get
            {
                return _iapService.AreAdsVisible();
            }
        }

        private ICommand _addEntryCommand;
        public ICommand AddEntryCommand
        {
            get
            {
                return _addEntryCommand;
            }
        }

        public void LoadData(DateTime selectedDate)
        {
            _selectedDate = DateTime.SpecifyKind(selectedDate.Date, DateTimeKind.Utc);
            Entries.Clear();
            var entries = _repository.GetFilteredEntries(it => it.EntryDate.Date == selectedDate.Date).OrderBy(it=>it.MealType).ThenBy(it => it.CreateDateTime);
            foreach (var e in entries)
            {
                var evm = KernelService.Kernel.Get<EntryViewModel>();
                evm.LoadEntry(e);
                evm.RemoveActionCompleted += (sender, args) =>
                {
                    var item = sender as EntryViewModel;
                    if (item != null)
                    {
                        Entries.Remove(item);
                    }
                };
                Entries.Add(evm);
            }
        }

        public bool IsNewEntryValid()
        {
            return !string.IsNullOrEmpty(NewEntry);
        }

        public void AddEntry()
        {
            if (IsNewEntryValid())
            {
                var newEntry = KernelService.Kernel.Get<IEntry>();
                newEntry.EntryGuid = Guid.NewGuid();
                newEntry.EntryDate = DateTime.SpecifyKind(SelectedDate.Date, DateTimeKind.Utc);
                newEntry.EntryText = NewEntry;
                newEntry.MealType = MealType.MealType;
                newEntry = _repository.AddOrUpdateEntry(newEntry);
                var evm = KernelService.Kernel.Get<EntryViewModel>();
                evm.LoadEntry(newEntry);
                evm.RemoveActionCompleted += (sender, args) =>
                {
                    var item = sender as EntryViewModel;
                    if (item != null)
                    {
                        Entries.Remove(item);
                    }
                };
                Entries.Add(evm);
                NewEntry = "";
                if (AddActionCompleted != null)
                {
                    AddActionCompleted(this, new ValidationResultEventArgs());
                }
            }
            else
            {
                if (AddActionCompleted != null)
                {
                    AddActionCompleted(this, new ValidationResultEventArgs(false));
                }
            }
        }
    }
}
