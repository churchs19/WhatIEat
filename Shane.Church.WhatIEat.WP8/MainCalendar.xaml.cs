using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.ViewModels;
using Shane.Church.WhatIEat.Core.WP.Commands;
using Shane.Church.WhatIEat.Strings;
using Shane.Church.WhatIEat.WP.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace Shane.Church.WhatIEat.WP
{
	public partial class MainCalendar : PhoneApplicationPage
	{
		private MainViewModel _model;
		private INavigationService _navService;
		private ISettingsService _settingsService;
		private ILoggingService _log;

		public MainCalendar()
		{
			_navService = KernelService.Kernel.Get<INavigationService>();
			_settingsService = KernelService.Kernel.Get<ISettingsService>();
			_log = KernelService.Kernel.Get<ILoggingService>();

			InitializeComponent();

			InitializeAdControl();

			//Shows the rate reminder message, according to the settings of the RateReminder.
			(App.Current as App).rateReminder.Notify();

			_model = KernelService.Kernel.Get<MainViewModel>();

			_model.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "SyncRunning" && _model.SyncRunning == false)
				{
					this.MainRadCalendar.AppointmentSource = new EntriesAppointmentSource(_model);
				}
			};

			GenericGroupDescriptor<CalendarItemViewModel, DateTime> groupByMonth = new GenericGroupDescriptor<CalendarItemViewModel, DateTime>(it =>
			{
				return new DateTime(it.ItemDate.Year, it.ItemDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
			});
			groupByMonth.GroupFormatString = "{0:Y}";
			groupByMonth.SortMode = ListSortMode.Descending;
			summaryJumpList.GroupDescriptors.Add(groupByMonth);
			//GenericGroupDescriptor<CalendarItemViewModel, DateTime> groupByDay = new GenericGroupDescriptor<CalendarItemViewModel, DateTime>(it =>
			//{
			//	return DateTime.SpecifyKind(it.ItemDate.Date, DateTimeKind.Utc);
			//});
			//groupByDay.GroupFormatString = "{0:M}";
			//groupByDay.SortMode = ListSortMode.Descending;
			//summaryJumpList.GroupDescriptors.Add(groupByDay);
			summaryJumpList.DataRequested += this.summaryJumpList_DataRequested;
			summaryJumpList.GroupPickerItemTap += summaryJumpList_GroupPickerItemTap;

			this.DataContext = _model;
		}

		void summaryJumpList_GroupPickerItemTap(object sender, GroupPickerItemTapEventArgs e)
		{
			while (!summaryJumpList.Groups.Select(it => it.Key).Contains(e.DataItem))
			{
				_model.LoadNextSummaryItems();
				summaryJumpList.RefreshData();
			}
			foreach (DataGroup group in summaryJumpList.Groups)
			{
				if (object.Equals(e.DataItem, group.Key))
				{
					e.ClosePicker = true;
					e.ScrollToItem = true;
					e.DataItemToNavigate = group;
					return;
				}
			}
		}

		private void summaryJumpList_DataRequested(object sender, EventArgs e)
		{
			_model.LoadNextSummaryItems();
		}

		#region Ad Control
		private void InitializeAdControl()
		{
			AdControl.AdReceived += new Inneractive.Ad.InneractiveAd.IaAdReceived(AdControl_AdReceived);
			AdControl.AdFailed += new Inneractive.Ad.InneractiveAd.IaAdFailed(AdControl_AdFailed);
			AdControl.DefaultAdReceived += new Inneractive.Ad.InneractiveAd.IaDefaultAdReceived(AdControl_DefaultAdReceived);

#if PERSONAL
			AdPanel.Children.Remove(AdControl);
			AdControl = null;
#endif
		}

		void AdControl_DefaultAdReceived(object sender)
		{
			FlurryWP8SDK.Api.LogEvent("Unpaid Ad Received");
			AdControl.Visibility = System.Windows.Visibility.Visible;
		}

		private void AdControl_AdReceived(object sender)
		{
			FlurryWP8SDK.Api.LogEvent("Paid Ad Received");
			AdControl.Visibility = System.Windows.Visibility.Visible;
		}

		private void AdControl_AdFailed(object sender)
		{
			FlurryWP8SDK.Api.LogEvent("No Ad Received");
			AdControl.Visibility = System.Windows.Visibility.Collapsed;
		}
		#endregion

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			if (e.NavigationMode == NavigationMode.New)
			{
				NavigationService.RemoveBackEntry();
			}
			_model.Initialize();
			this.MainRadCalendar.AppointmentSource = new EntriesAppointmentSource(_model);
			this.summaryJumpList.GroupPickerItemsSource = null;
			this.summaryJumpList.GroupPickerItemsSource = _model.SummaryGroups;
			this.summaryJumpList.ItemsSource = null;
			this.summaryJumpList.ItemsSource = _model.SummaryEntries;
			InitializeApplicationBar();
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
			appBarButtonReview.Text = Shane.Church.WhatIEat.Strings.Resources.ReviewLabel;
			appBarButtonReview.Click += appBarReview_Click;
			ApplicationBar.Buttons.Add(appBarButtonReview);

			ApplicationBarMenuItem appBarMenuItemAbout = new ApplicationBarMenuItem();
			appBarMenuItemAbout.Text = Shane.Church.WhatIEat.Strings.Resources.AboutLabel;
			appBarMenuItemAbout.Click += appBarAbout_Click;
			ApplicationBar.MenuItems.Add(appBarMenuItemAbout);

			ApplicationBarIconButton appBarButtonSettings = new ApplicationBarIconButton(new Uri("/Images/Settings.png", UriKind.Relative));
			appBarButtonSettings.Text = Shane.Church.WhatIEat.Strings.Resources.SettingsLabel;
			appBarButtonSettings.Click += appBarButtonSettings_Click;
			ApplicationBar.Buttons.Add(appBarButtonSettings);

			if (_settingsService.LoadSetting<bool>("SyncEnabled"))
			{
				ApplicationBarIconButton appBarButtonSync = new ApplicationBarIconButton(new Uri("/Images/Synchronize.png", UriKind.Relative));
				appBarButtonSync.Text = Shane.Church.WhatIEat.Strings.Resources.SyncLabel;
				appBarButtonSync.Click += appBarButtonSync_Click;
				ApplicationBar.Buttons.Add(appBarButtonSync);
			}

			ApplicationBarIconButton appBarButtonExport = new ApplicationBarIconButton(new Uri("/Images/Excel-Export.png", UriKind.Relative));
			appBarButtonExport.Text = Shane.Church.WhatIEat.Strings.Resources.CsvExportLabel;
			appBarButtonExport.Click += appBarButtonExport_Click;
			ApplicationBar.Buttons.Add(appBarButtonExport);
		}

		private void appBarButtonExport_Click(object sender, EventArgs e)
		{
			_navService.NavigateTo(new Uri("/ExcelExport.xaml", UriKind.Relative));
		}

		private void appBarButtonSettings_Click(object sender, EventArgs e)
		{
			_navService.NavigateTo(new Uri("/Settings.xaml", UriKind.Relative));
		}

		private void appBarButtonSync_Click(object sender, EventArgs e)
		{
			_model.SyncCommand.Execute(null);
		}

		private void appBarAbout_Click(object sender, EventArgs e)
		{
			_navService.NavigateTo(new Uri("/About.xaml", UriKind.Relative));
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
				PhoneApplicationService.Current.State["SelectedDate"] = DateTime.SpecifyKind(e.Item.Date.Value.Date, DateTimeKind.Utc);
				KernelService.Kernel.Get<INavigationService>().NavigateTo(new Uri("/DateEdit.xaml", UriKind.Relative));
			}
		}

		private void Pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			PivotItem pi = e.AddedItems[0] as PivotItem;
			if (pi != null)
			{
				string header = pi.Header.ToString();
				if (header == Shane.Church.WhatIEat.Strings.Resources.CalendarLabel)
					_log.LogMessage("Viewed Calendar");
				else if (header == Shane.Church.WhatIEat.Strings.Resources.SummaryLabel)
					_log.LogMessage("Viewed Summary");
			}
		}
	}
}