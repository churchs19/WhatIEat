using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.ViewModels;

namespace Shane.Church.WhatIEat.WP
{
	public partial class About : PhoneApplicationPage
	{
		IWebNavigationService _webNav;
		ILoggingService _log;

		public About()
		{
			InitializeComponent();

			_webNav = KernelService.Kernel.Get<IWebNavigationService>();
			_log = KernelService.Kernel.Get<ILoggingService>();

			this.DataContext = KernelService.Kernel.Get<AboutViewModel>();
		}

		private void PrivacyButton_Click(object sender, RoutedEventArgs e)
		{
			_webNav.NavigateTo(new Uri("http://bit.ly/whatieatprivacy"));
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_log.LogPageView("About");

			base.OnNavigatedTo(e);
		}
	}
}