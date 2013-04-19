using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;
using Shane.Church.WhatIEat.WP.ViewModels;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.ViewModels;
using Shane.Church.WhatIEat.WP.Resources;
using Shane.Church.WhatIEat.WP.Commands;
using System.Windows.Navigation;

namespace Shane.Church.WhatIEat.WP
{
	public partial class MainCalendar : PhoneApplicationPage
	{
		private MainViewModel _model;

		public MainCalendar()
		{
			InitializeComponent();

			InitializeApplicationBar();

			_model = KernelService.Kernel.Get<MainViewModel>();
			var appointmentSource = new EntriesAppointmentSource(_model);
			this.MainRadCalendar.AppointmentSource = appointmentSource;
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			if (e.NavigationMode == NavigationMode.New)
			{
				NavigationService.RemoveBackEntry();
			}
			if (e.NavigationMode != System.Windows.Navigation.NavigationMode.New)
				this.MainRadCalendar.AppointmentSource = new EntriesAppointmentSource(_model);
			base.OnNavigatedTo(e);
		}

		private void InitializeApplicationBar()
		{
			ApplicationBar = new ApplicationBar();
			ApplicationBar.IsMenuEnabled = true;
			ApplicationBar.Mode = ApplicationBarMode.Minimized;
			ApplicationBar.Opacity = 0.5;

			//ApplicationBarMenuItem appBarMenuItemSettings = new ApplicationBarMenuItem(AppResources.SettingsMenuItem);
			//appBarMenuItemSettings.Click += new EventHandler(appBarMenuItemSettings_Click);
			//ApplicationBar.MenuItems.Add(appBarMenuItemSettings);

			ApplicationBarMenuItem appBarMenuItemReview = new ApplicationBarMenuItem(AppResources.ReviewLabel);
			appBarMenuItemReview.Click += new EventHandler(appBarMenuItemReview_Click);
			ApplicationBar.MenuItems.Add(appBarMenuItemReview);

			ApplicationBarMenuItem appBarMenuItemAbout = new ApplicationBarMenuItem(AppResources.AboutLabel);
			appBarMenuItemAbout.Click += new EventHandler(appBarMenuItemAbout_Click);
			ApplicationBar.MenuItems.Add(appBarMenuItemAbout);
		}

		private void appBarMenuItemAbout_Click(object sender, EventArgs e)
		{
			KernelService.Kernel.Get<INavigationService>().NavigateTo(new Uri("/About.xaml", UriKind.Relative));
		}

		private void appBarMenuItemReview_Click(object sender, EventArgs e)
		{
			RateThisAppCommand cmd = new RateThisAppCommand();
			cmd.Execute(null);
		}

		private void appBarMenuItemSettings_Click(object sender, EventArgs e)
		{

		}

		private void RadCalendar_ItemTap(object sender, CalendarItemTapEventArgs e)
		{
			if (e.Item.Date.HasValue)
			{
				PhoneApplicationService.Current.State["SelectedDate"] = e.Item.Date.Value.Date;
				KernelService.Kernel.Get<INavigationService>().NavigateTo(new Uri("/DateEdit.xaml", UriKind.Relative));
			}
		}
	}
}
