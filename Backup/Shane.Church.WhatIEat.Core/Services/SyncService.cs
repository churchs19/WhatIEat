using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shane.Church.WhatIEat.Core.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.WhatIEat.Core.Services
{
	public abstract class SyncService
	{
		protected ISettingsService _settingsService;
		protected IRepository<IEntry> _entries;
		protected ILoggingService _log;

		public SyncService(IMobileServiceClient client, ISettingsService settings, IRepository<IEntry> entries, ILoggingService log)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");
			_settingsService = settings;
			if (entries == null)
				throw new ArgumentNullException("entries");
			_entries = entries;
			if (client == null)
				throw new ArgumentNullException("client");
			Client = client;
			if (log == null)
				throw new ArgumentNullException("log");
			_log = log;
		}

		private IMobileServiceClient _client;
		public IMobileServiceClient Client
		{
			get { return _client; }
			set { _client = value; }
		}

		private MobileServiceUser _user;
		public MobileServiceUser User
		{
			get { return _user; }
			set
			{
				_user = value;
			}
		}

		public bool IsConnected
		{
			get { return User != null; }
		}

		public abstract void Disconnect();

		public async Task Authenticate()
		{
			if (User == null)
				User = await AuthenticateUser();
		}

		public abstract Task<bool> IsNetworkConnected();

		public abstract Task<MobileServiceUser> AuthenticateUser();

		public async Task Sync()
		{
			try
			{
				if (await IsNetworkConnected())
				{
					await Authenticate();

					if (User != null)
					{
						DateTimeOffset lastSuccessfulSyncDate = _settingsService.LoadSetting<DateTimeOffset>("LastSuccessfulSync");

						var localEntries = _entries.GetFilteredEntries(it => it.EditDateTime > lastSuccessfulSyncDate, true).Select(it => new AzureEntry(it));

						JsonSerializer serializer = JsonSerializer.Create(Client.SerializerSettings);
						JArray arr = JArray.FromObject(localEntries, serializer);

						JObject body = new JObject();
						body.Add("entries", arr);
						body.Add("lastSyncDate", lastSuccessfulSyncDate);

						var results = await Client.GetTable("AzureSync").InsertAsync(body);

						foreach (var item in results["ServerChanges"])
						{
							try
							{
								var entry = item.ToObject<AzureEntry>(serializer).GetEntry();
								entry.EntryDate = DateTime.SpecifyKind(entry.EntryDate, DateTimeKind.Utc);
								_entries.AddOrUpdateEntry(entry);
							}
							catch
							{
								throw;
							}
						}

						_settingsService.SaveSetting<DateTimeOffset>(DateTimeOffset.Now, "LastSuccessfulSync");
					}
				}
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "Sync Error");
				//TODO: Do I need a message here?
			}
		}
	}
}
