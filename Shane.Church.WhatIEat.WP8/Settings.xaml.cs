using Microsoft.Phone.Controls;
using Ninject;
using System;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.ViewModels;

namespace Shane.Church.WhatIEat.WP
{
	public partial class Settings : PhoneApplicationPage
	{
		ILoggingService _log;

		public Settings()
		{
			InitializeComponent();

			InitializeAdControl();

			this.DataContext = KernelService.Kernel.Get<SettingsViewModel>();

			_log = KernelService.Kernel.Get<ILoggingService>();
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			var model = this.DataContext as SettingsViewModel;
			if(model != null)
			{
                if (!model.AreAdsVisible && AdMediator_02B3FF != null)
				{
                    AdPanel.Children.Remove(AdMediator_02B3FF);
                    AdMediator_02B3FF = null;
				}
			}
			base.OnNavigatedTo(e);
		}

		#region Ad Control
		private void InitializeAdControl()
		{
#if !PERSONAL
            AdMediator_02B3FF.AdSdkEvent += AdMediator_AdSdkEvent;
            AdMediator_02B3FF.AdMediatorError += AdMediator_AdMediatorError;
            AdMediator_02B3FF.AdMediatorFilled += AdMediator_AdMediatorFilled;
            AdMediator_02B3FF.AdSdkError += AdMediator_AdSdkError;
#else
			AdPanel.Children.Remove(AdMediator_02B3FF);
			AdMediator_02B3FF = null;
#endif
        }

		void AdMediator_AdSdkError(object sender, Microsoft.AdMediator.Core.Events.AdFailedEventArgs e)
		{
			_log.LogMessage(String.Format("Ad SDK Error by {0} ErrorCode: {1} ErrorDescription: {2} Error: {3}", e.Name, e.ErrorCode, e.ErrorDescription, e.Error));
		}

		void AdMediator_AdMediatorFilled(object sender, Microsoft.AdMediator.Core.Events.AdSdkEventArgs e)
		{
			_log.LogMessage(String.Format("Ad Filled:" + e.Name));
            if (AdMediator_02B3FF != null)
			{
                AdMediator_02B3FF.Visibility = System.Windows.Visibility.Visible;
			}
		}

		void AdMediator_AdMediatorError(object sender, Microsoft.AdMediator.Core.Events.AdMediatorFailedEventArgs e)
		{
			_log.LogMessage(String.Format("AdMediatorError:" + e.Error + " " + e.ErrorCode));
            if (e.ErrorCode == Microsoft.AdMediator.Core.Events.AdMediatorErrorCode.NoAdAvailable && AdMediator_02B3FF != null)
			{
				// AdMediator will not show an ad for this mediation cycle
                AdMediator_02B3FF.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void AdMediator_AdSdkEvent(object sender, Microsoft.AdMediator.Core.Events.AdSdkEventArgs e)
		{
			_log.LogMessage(String.Format("AdSdk event {0} by {1}", e.EventName, e.Name));
		}

		#endregion
	}
}