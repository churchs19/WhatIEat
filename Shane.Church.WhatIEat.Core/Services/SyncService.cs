using Microsoft.WindowsAzure.MobileServices;
using Ninject;
using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shane.Church.WhatIEat.Core.Services
{
	public abstract class SyncService
	{
		private ISettingsService _settingsService;
		private IRepository<IEntry> _entries;

		public SyncService(IMobileServiceClient client, ISettingsService settings, IRepository<IEntry> entries)
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
			catch (Exception ex)
			{
				//TODO: Log Exception - Throw error
				throw ex;
			}
		}
	}
}
