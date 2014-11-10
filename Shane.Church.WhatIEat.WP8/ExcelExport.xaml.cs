using Microsoft.Phone.Controls;
using Ninject;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.ViewModels;
using Shane.Church.WhatIEat.Strings;
using System;
using System.Windows;
using System.Windows.Navigation;
using Telerik.Windows.Controls;

namespace Shane.Church.WhatIEat.WP
{
	public partial class ExcelExport : PhoneApplicationPage
	{
		protected ExcelExportViewModel _model;
		private ILoggingService _log;

		public ExcelExport()
		{
			InitializeComponent();

			_log = KernelService.Kernel.Get<ILoggingService>();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_model = KernelService.Kernel.Get<ExcelExportViewModel>();
			_model.ExportBeginning += () =>
			{
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					this.busyIndicator.IsRunning = true;
				});
			};
			_model.ExportCompleted += (isSuccess) =>
			{
				Deployment.Current.Dispatcher.BeginInvoke(async () =>
				{
					this.busyIndicator.IsRunning = false;
					if (isSuccess)
					{
						await RadMessageBox.ShowAsync(new object[] { Shane.Church.WhatIEat.Strings.Resources.Ok }, Shane.Church.WhatIEat.Strings.Resources.UploadSuccessTitle, Shane.Church.WhatIEat.Strings.Resources.UploadSuccessMessage);
					}
					else
					{
						await RadMessageBox.ShowAsync(new object[] { Shane.Church.WhatIEat.Strings.Resources.Ok }, Shane.Church.WhatIEat.Strings.Resources.UploadFailedTitle, Shane.Church.WhatIEat.Strings.Resources.UploadFailedMessage);
					}
				});
			};
			this.DataContext = _model;

            if (!_model.AreAdsVisible && AdMediator_3BB1FC != null)
			{
                AdPanel.Children.Remove(AdMediator_3BB1FC);
                AdMediator_3BB1FC = null;
			}

			base.OnNavigatedTo(e);
		}

		#region Ad Control
		private void InitializeAdControl()
		{
#if !PERSONAL
            AdMediator_3BB1FC.AdSdkEvent += AdMediator_AdSdkEvent;
            AdMediator_3BB1FC.AdMediatorError += AdMediator_AdMediatorError;
            AdMediator_3BB1FC.AdMediatorFilled += AdMediator_AdMediatorFilled;
            AdMediator_3BB1FC.AdSdkError += AdMediator_AdSdkError;
#else
			AdPanel.Children.Remove(AdMediator_3BB1FC);
			AdMediator_3BB1FC = null;
#endif
        }

		void AdMediator_AdSdkError(object sender, Microsoft.AdMediator.Core.Events.AdFailedEventArgs e)
		{
			_log.LogMessage(String.Format("Ad SDK Error by {0} ErrorCode: {1} ErrorDescription: {2} Error: {3}", e.Name, e.ErrorCode, e.ErrorDescription, e.Error));
		}

		void AdMediator_AdMediatorFilled(object sender, Microsoft.AdMediator.Core.Events.AdSdkEventArgs e)
		{
			_log.LogMessage(String.Format("Ad Filled:" + e.Name));
            if (AdMediator_3BB1FC != null)
			{
                AdMediator_3BB1FC.Visibility = System.Windows.Visibility.Visible;
			}
		}

		void AdMediator_AdMediatorError(object sender, Microsoft.AdMediator.Core.Events.AdMediatorFailedEventArgs e)
		{
			_log.LogMessage(String.Format("AdMediatorError:" + e.Error + " " + e.ErrorCode));
            if (e.ErrorCode == Microsoft.AdMediator.Core.Events.AdMediatorErrorCode.NoAdAvailable && AdMediator_3BB1FC != null)
			{
				// AdMediator will not show an ad for this mediation cycle
                AdMediator_3BB1FC.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void AdMediator_AdSdkEvent(object sender, Microsoft.AdMediator.Core.Events.AdSdkEventArgs e)
		{
			_log.LogMessage(String.Format("AdSdk event {0} by {1}", e.EventName, e.Name));
		}

		#endregion

	}
}