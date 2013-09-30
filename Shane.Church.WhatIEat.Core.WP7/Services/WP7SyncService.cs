using Microsoft.Live;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Shane.Church.Utility.Core.WP.Extensions;
using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Exceptions;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Shane.Church.WhatIEat.Core.WP7.Services
{
	public class WP7SyncService : SyncService
	{
		LiveAuthClient _liveIdClient = new LiveAuthClient("00000000440EEC7F");

		public WP7SyncService(IMobileServiceClient client, ISettingsService settings, IRepository<IEntry> entries, ILoggingService log) :
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
				if (_session == null || _session.Expires.CompareTo(DateTimeOffset.Now) < 0)
				{
					result = await _liveIdClient.InitializeAsyncTask(new[] { "wl.basic", "wl.offline_access" });
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
				throw new SyncException(ex.Message, ex);
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
			if (_session == null || _session.Expires.CompareTo(DateTimeOffset.Now) < 0)
			{
				LiveLoginResult result;
				try
				{
					if (!(await LiveLoginSilent()))
					{
						result = await _liveIdClient.LoginAsyncTask(new[] { "wl.basic", "wl.offline_access" });
						if (result.Status == LiveConnectSessionStatus.Connected)
						{
							_session = result.Session;
						}
						else
						{
							_session = null;
							MessageBox.Show("You must be logged in to sync.");
							throw new SyncException("You must be logged in to sync");
						}
					}
				}
				catch (Exception ex)
				{
					FlurryWP8SDK.Api.LogError("Sync Exception", ex);
					throw new SyncException(ex.Message, ex);
				}
			}
			try
			{
				LiveConnectClient client = new LiveConnectClient(_session);
				LiveOperationResult meResult = await client.GetAsyncTask("me");
				JObject token = JObject.Parse("{\"authenticationToken\": \"" + _session.AuthenticationToken + "\"}");
				return await this.Client.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount, token);
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "Sync Exception");
				throw new SyncException(ex.Message, ex);
			}
		}

		public override Task<bool> IsNetworkConnected()
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Task.Factory.StartNew(() =>
			{
				try
				{
					var currentNetworkType = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;

					tcs.SetResult(currentNetworkType != Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None);
				}
				catch (Exception ex)
				{
					_log.LogException(ex, "IsNetworkConnected Exception");
					tcs.SetException(ex);
				}
			});
			return tcs.Task;
		}
	}
}
