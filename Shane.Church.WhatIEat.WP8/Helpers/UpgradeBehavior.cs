using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Shane.Church.WhatIEat.Strings;
using Shane.Church.Utility.Core.WP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Telerik.Windows.Controls;
using Wp7nl.Behaviors;

namespace Shane.Church.WhatIEat.WP.Helpers
{
	public class UpgradeBehavior : SafeBehavior<PhoneApplicationPage>
	{
		private ManifestAppInfo _appInfo;

		public UpgradeBehavior()
			: base()
		{
			_appInfo = new ManifestAppInfo();
		}

		protected override async void OnSetup()
		{
			await CheckForUpgrade();
			base.OnSetup();
		}

		protected override async void OnParentPageNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
			{
				await CheckForUpgrade();
			}

			base.OnParentPageNavigated(sender, e);
		}

		private async Task CheckForUpgrade()
		{
			if (await IsUpgradeAvailable())
			{
#if !WP8
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					RadMessageBox.Show(buttonsContent: new List<object> { Resources.Yes, Resources.No },
						title: Resources.UpgradeTitle,
						message: Resources.UpgradeText,
						closedHandler: (args) =>
						{
							if (args.Result == DialogResult.OK)
							{
								var marketplaceTask = new MarketplaceDetailTask();
								try
								{
									marketplaceTask.Show();
								}
								catch (InvalidOperationException)
								{
								}
							}
						});
				});
#else
				await Deployment.Current.Dispatcher.InvokeAsync(async () =>
					{
						var args = await RadMessageBox.ShowAsync(buttonsContent: new List<object> { Resources.Yes, Resources.No },
																title: Resources.UpgradeTitle,
																message: Resources.UpgradeText);
						if (args.Result == DialogResult.OK)
						{
							var marketplaceTask = new MarketplaceDetailTask();
							try
							{
								marketplaceTask.Show();
							}
							catch (InvalidOperationException)
							{
							}
						}
					});
#endif
			}
		}

		private async Task<bool> IsUpgradeAvailable()
		{
			var currentVersion = new Version(_appInfo.Version);
			var storeVersion = await GetLatestVersion();
			return storeVersion != null & currentVersion < storeVersion;
		}

		/// <summary>
		/// This method is almost 100% stolen from 
		/// http://www.pedrolamas.com/2013/07/24/checking-for-updates-from-inside-a-windows-phone-app/
		/// </summary>
		private async Task<Version> GetLatestVersion()
		{
			try
			{
				var cultureInfoName = CultureInfo.CurrentUICulture.Name;
				var url = string.Format(
					"http://marketplaceedgeservice.windowsphone.com/v8/catalog/apps/{0}?os={1}&cc={2}&oc=&lang={3}​",
					_appInfo.ProductId,
					Environment.OSVersion.Version,
					cultureInfoName.Substring(cultureInfoName.Length - 2).ToUpperInvariant(),
					cultureInfoName);

				HttpClient client = new HttpClient();
				var response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();

				using (var stream = await response.Content.ReadAsStreamAsync())
				{
					using (var reader = XmlReader.Create(stream))
					{
						reader.MoveToContent();
						var aNamespace = reader.LookupNamespace("a");
						reader.ReadToFollowing("entry", aNamespace);
						reader.ReadToDescendant("version");
						return new Version(reader.ReadElementContentAsString());
					}
				}
			}
			catch { }

			return null;
		}
	}
}
