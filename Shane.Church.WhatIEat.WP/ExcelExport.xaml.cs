using Microsoft.Phone.Controls;
using Ninject;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.ViewModels;
using Shane.Church.WhatIEat.WP.Resources;
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
#if !WP8
				Deployment.Current.Dispatcher.BeginInvoke(() =>
#else
				Deployment.Current.Dispatcher.BeginInvoke(async () =>
#endif
				{
					this.busyIndicator.IsRunning = false;
					if (isSuccess)
					{
#if !WP8
						RadMessageBox.Show(new object[] { AppResources.Ok }, AppResources.UploadSuccessTitle, AppResources.UploadSuccessMessage);
#else
						await RadMessageBox.ShowAsync(new object[] { AppResources.Ok }, AppResources.UploadSuccessTitle, AppResources.UploadSuccessMessage);
#endif
					}
					else
					{
#if !WP8
						RadMessageBox.Show(new object[] { AppResources.Ok }, AppResources.UploadFailedTitle, AppResources.UploadFailedMessage);
#else
						await RadMessageBox.ShowAsync(new object[] { AppResources.Ok }, AppResources.UploadFailedTitle, AppResources.UploadFailedMessage);
#endif
					}
				});
			};
			this.DataContext = _model;

			base.OnNavigatedTo(e);
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

	}
}