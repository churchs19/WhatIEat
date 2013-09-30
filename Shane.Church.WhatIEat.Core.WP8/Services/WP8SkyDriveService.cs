using Microsoft.Live;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.WP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.WhatIEat.Core.WP8.Services
{
	public class WP8SkyDriveService : ISkyDriveService
	{
		LiveAuthClient _authClient;
		string _whatIEatFolderId = null;
		ILoggingService _log;

		public WP8SkyDriveService(ILoggingService log)
		{
			if (log == null) throw new ArgumentNullException("log");
			_log = log;

			_authClient = new LiveAuthClient(LiveConfig.ClientId);
		}

		public async Task<bool> SaveToSkyDrive(SkyDriveSaveArgs args)
		{
			bool uploadSuccess = false;
			try
			{
				var loginResult = await _authClient.LoginAsync(LiveConfig.Scopes);
				if (loginResult.Status == LiveConnectSessionStatus.Connected)
				{
					LiveConnectClient client = new LiveConnectClient(loginResult.Session);
					var foldersResult = await client.GetAsync("me/skydrive/files?filter=folders");
					var folders = foldersResult.Result["data"] as List<object>;
					if (folders != null)
					{
						var folderQuery = (from f in folders
										   where f is IDictionary<string, object>
										   select (IDictionary<string, object>)f);
						_whatIEatFolderId = folderQuery.Where(it => it["name"].ToString().ToLowerInvariant().Equals("what i eat")).Select(it => it["id"].ToString()).FirstOrDefault();
					}
					if (string.IsNullOrWhiteSpace(_whatIEatFolderId))
					{
						Dictionary<string, object> folderData = new Dictionary<string, object>();
						folderData.Add("name", "What I Eat");
						var folderCreateResult = await client.PostAsync("me/skydrive", folderData);
						_whatIEatFolderId = folderCreateResult.Result["id"].ToString();
					}
					if (!string.IsNullOrWhiteSpace(_whatIEatFolderId))
					{
						using (var ms = new MemoryStream(args.Encoding.GetBytes(args.Content)))
						{
							var uploadResult = await client.UploadAsync(_whatIEatFolderId, args.Filename, ms, OverwriteOption.Overwrite);
							uploadSuccess = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				//TODO: Log Exception
				FlurryWP8SDK.Api.LogError("SkyDrive Upload Error", ex);
			}
			return uploadSuccess;
		}
	}
}
