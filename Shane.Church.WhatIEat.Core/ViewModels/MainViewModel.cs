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

namespace Shane.Church.WhatIEat.Core.ViewModels
{
	public class MainViewModel : ObservableObject
	{
		private IRepository<IEntry> _repository;

		public MainViewModel(IRepository<IEntry> repository)
		{
			if (repository == null)
				throw new ArgumentNullException("repository");
			_repository = repository;

			_dateEntries = new ObservableCollection<CalendarItemViewModel>();
			_dateEntries.CollectionChanged += _dateEntries_CollectionChanged;

			//NavigateCommand = new RelayCommandGeneric<object>(o => Navigate(o));
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

		public void LoadData(DateTime startDate, DateTime endDate)
		{
			var entries = _repository.GetFilteredEntries(it => it.EntryDate >= startDate && it.EntryDate <= endDate);
			_dateEntries.Clear();
			foreach (var e in entries)
			{
				DateEntries.Add(new CalendarItemViewModel() { ItemDate = e.EntryDate, ItemText = e.EntryText });
			}
		}

		//private ICommand _navigateCommand;
		//public ICommand NavigateCommand
		//{
		//	get { return _navigateCommand; }
		//	set
		//	{
		//		Set(() => NavigateCommand, ref _navigateCommand, value);
		//	}
		//}

		//public bool Navigate(object parameter)
		//{
		//	var service = KernelService.Kernel.Get<INavigationService>();
		//	if (parameter is string)
		//	{
		//		service.NavigateTo(new Uri(parameter.ToString(), UriKind.Relative));
		//		return true;
		//	}
		//	else if (parameter is Uri)
		//	{
		//		service.NavigateTo((Uri)parameter);
		//		return true;
		//	}
		//	else
		//		return false;
		//}
	}
}
