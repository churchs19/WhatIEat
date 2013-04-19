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
using System.Windows.Navigation;
using Shane.Church.WhatIEat.Core.WP.Commands;

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
			ApplicationBar.Mode = ApplicationBarMode.Minimized;
			ApplicationBar.Opacity = 0.5;
			ApplicationBar.BackgroundColor = (Color)Application.Current.Resources["AppBaseColor4"];
			ApplicationBar.ForegroundColor = (Color)Application.Current.Resources["AppBaseColor"];

			ApplicationBarIconButton appBarButtonReview = new ApplicationBarIconButton(new Uri("/Images/Rating.png", UriKind.Relative));
			appBarButtonReview.Text = AppResources.ReviewLabel;
			appBarButtonReview.Click += appBarReview_Click;
			ApplicationBar.Buttons.Add(appBarButtonReview);

			ApplicationBarIconButton appBarButtonAbout = new ApplicationBarIconButton(new Uri("/Images/About.png", UriKind.Relative));
			appBarButtonAbout.Text = AppResources.AboutLabel;
			appBarButtonAbout.Click += appBarAbout_Click;
			ApplicationBar.Buttons.Add(appBarButtonAbout);
		}

		private void appBarAbout_Click(object sender, EventArgs e)
		{
			KernelService.Kernel.Get<INavigationService>().NavigateTo(new Uri("/About.xaml", UriKind.Relative));
		}

		private void appBarReview_Click(object sender, EventArgs e)
		{
			RateThisAppCommand cmd = new RateThisAppCommand();
			cmd.Execute(null);
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
