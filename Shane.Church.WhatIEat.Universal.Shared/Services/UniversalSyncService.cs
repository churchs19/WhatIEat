using Microsoft.Live;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Universal.Shared;
using Shane.Church.WhatIEat.Strings;
using System;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;
using Windows.Networking.Connectivity;

namespace Shane.Church.WhatIEat.Universal.Shared.Services
{
	public class WP8SyncService : SyncService
	{
		LiveAuthClient _liveIdClient = new LiveAuthClient(LiveConfig.ClientId);

		public WP8SyncService(IMobileServiceClient client, ISettingsService settings, IRepository<IEntry> entries, ILoggingService log) :
			base(client, settings, entries, log)
		{
		}

		private LiveConnectSession _session;
		public LiveConnectSession Session
		{
			get { return _session; }
			set
			{
				_session = value;
			}
		}

		public async Task<bool> LiveLoginSilent()
		{
			LiveLoginResult result;
			try
			{
				if (_session == null)
				{
					result = await _liveIdClient.InitializeAsync(LiveConfig.Scopes);
					if (result.Status == LiveConnectSessionStatus.Connected)
					{
						_session = result.Session;
						return true;
					}
					else
					{
						_session = null;
						return false;
					}
				}
				else
					return true;
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "Sync Exception");
				return false;
			}
		}

		public override void Disconnect()
		{
			_session = null;
			User = null;
			_liveIdClient.Logout();
		}

		public override async Task<MobileServiceUser> AuthenticateUser()
		{
			if (_session == null)
			{
				LiveLoginResult result;
				try
				{
					if (!(await LiveLoginSilent()))
					{
						result = await _liveIdClient.LoginAsync(LiveConfig.Scopes);
						if (result.Status == LiveConnectSessionStatus.Connected)
						{
							_session = result.Session;
						}
						else
						{
							_session = null;
							await CoreApplication.GetCurrentView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
							{
								//Windows.UI.Xaml.Controls.MessageBox.Show(Resources.SyncLoginError);
								MessageDialog dlg = new MessageDialog(Resources.SyncLoginError);
								await dlg.ShowAsync();
							});
						}
					}
				}
				catch (Exception ex)
				{
					_log.LogException(ex, "Sync Exception");
				}
			}
			try
			{
				if (_session != null)
				{
					LiveConnectClient client = new LiveConnectClient(_session);
					LiveOperationResult meResult = await client.GetAsync("me");
					JObject token = JObject.Parse("{\"authenticationToken\": \"" + _session.AuthenticationToken + "\"}");
					return await this.Client.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount, token);
				}
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "Sync Exception");
				CoreApplication.GetCurrentView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
				{
					//Windows.UI.Xaml.Controls.MessageBox.Show(Resources.SyncLoginError);
					MessageDialog dlg = new MessageDialog(Resources.SyncUnknownError);
					await dlg.ShowAsync();
				});
			}
			return null;
		}

		public override Task<bool> IsNetworkConnected()
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Task.Factory.StartNew(() =>
			{
				try
				{
					var isConnected = NetworkInformation.GetInternetConnectionProfile() != null;
					tcs.SetResult(isConnected);
				}
				catch (Exception ex)
				{
					_log.LogException(ex, "IsNetworkConnected Exception");
					tcs.SetResult(false);
				}
			});
			return tcs.Task;
		}
	}
}
