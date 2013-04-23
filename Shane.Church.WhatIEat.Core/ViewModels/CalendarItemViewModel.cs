using GalaSoft.MvvmLight;
using Shane.Church.WhatIEat.Core.Data;
using System;
using System.Collections.ObjectModel;

namespace Shane.Church.WhatIEat.Core.ViewModels
{
	public class CalendarItemViewModel : ObservableObject
	{
		public CalendarItemViewModel()
		{
			_calendarItemDate = DateTime.Now;
		}

		private DateTime _calendarItemDate;
		public DateTime ItemDate
		{
			get { return _calendarItemDate; }
			set
			{
				Set(() => ItemDate, ref _calendarItemDate, value);
			}
		}

		private string _itemText;
		public string ItemText
		{
			get { return _itemText; }
			set
			{
				Set(() => ItemText, ref _itemText, value);
			}
		}
	}
}