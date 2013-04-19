using Shane.Church.WhatIEat.Core.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace Shane.Church.WhatIEat.WP.ViewModels
{
    public class EntriesAppointmentSource : AppointmentSource
    {
		private MainViewModel _model;

        public EntriesAppointmentSource(MainViewModel model)
        {
			if (model == null)
				throw new ArgumentNullException("model");
			_model = model;
        }

        public override void FetchData(DateTime startDate, DateTime endDate)
        {
            this.AllAppointments.Clear();

			_model.LoadData(startDate, endDate);
			var appointments = _model.DateEntries.Select(it => new EntryAppointment() { StartDate = it.ItemDate.Date, EndDate = it.ItemDate.Date.AddDays(1).Date, Subject = it.ItemText } as IAppointment);
			this.AllAppointments.AddRange(appointments);

            this.OnDataLoaded();
        }
    }

    public class EntryAppointment : IAppointment
    {
        public DateTime EndDate
        {
            get;
            set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }
    }
}
