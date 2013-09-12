using Microsoft.Phone.Controls;
using Ninject;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.ViewModels;

namespace Shane.Church.WhatIEat.WP
{
	public partial class Settings : PhoneApplicationPage
	{
		public Settings()
		{
			InitializeComponent();

			InitializeAdControl();

			this.DataContext = KernelService.Kernel.Get<SettingsViewModel>();
		}

		#region Ad Control
		private void InitializeAdControl()
		{
			AdControl.AdReceived += new Inneractive.Ad.InneractiveAd.IaAdReceived(AdControl_AdReceived);
			AdControl.AdFailed += new Inneractive.Ad.InneractiveAd.IaAdFailed(AdControl_AdFailed);
			AdControl.DefaultAdReceived += new Inneractive.Ad.InneractiveAd.IaDefaultAdReceived(AdControl_DefaultAdReceived);

#if PERSONAL
			AdControl.IsEnabled = false;
			AdControl.Height = 0;
#endif
		}

		void AdControl_DefaultAdReceived(object sender)
		{
			AdControl.Visibility = System.Windows.Visibility.Visible;
		}

		private void AdControl_AdReceived(object sender)
		{
			AdControl.Visibility = System.Windows.Visibility.Visible;
		}

		private void AdControl_AdFailed(object sender)
		{
			AdControl.Visibility = System.Windows.Visibility.Collapsed;
		}
		#endregion

	}
}