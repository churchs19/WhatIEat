using Microsoft.Phone.Controls;
using Ninject;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.ViewModels;
using Shane.Church.WhatIEat.Strings;
using System.Windows;
using System.Windows.Navigation;
using Telerik.Windows.Controls;

namespace Shane.Church.WhatIEat.WP
{
	public partial class ExcelExport : PhoneApplicationPage
	{
		protected ExcelExportViewModel _model;

		public ExcelExport()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			FlurryWP8SDK.Api.LogPageView();
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

            if (!_model.AreAdsVisible && AdControl != null)
            {
                AdPanel.Children.Remove(AdControl);
                AdControl = null;
            }

			base.OnNavigatedTo(e);
		}

		#region Ad Control
		private void InitializeAdControl()
		{
			AdControl.AdReceived += new Inneractive.Ad.InneractiveAd.IaAdReceived(AdControl_AdReceived);
			AdControl.AdFailed += new Inneractive.Ad.InneractiveAd.IaAdFailed(AdControl_AdFailed);
			AdControl.DefaultAdReceived += new Inneractive.Ad.InneractiveAd.IaDefaultAdReceived(AdControl_DefaultAdReceived);

#if PERSONAL
            //AdPanel.Children.Remove(AdControl);
            //AdControl = null;
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

	}
}