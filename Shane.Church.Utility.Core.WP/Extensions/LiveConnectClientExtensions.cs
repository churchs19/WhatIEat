using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.Utility.Core.WP.Extensions
{
	public static class LiveConnectClientExtensions
	{
		public static Task<LiveOperationResult> GetAsyncTask(this LiveConnectClient client, string path, object userState = null)
		{
			TaskCompletionSource<LiveOperationResult> tcs =
			  new TaskCompletionSource<LiveOperationResult>();
			client.GetCompleted += (sender, args) =>
			{
				if (args.Error != null) tcs.SetException(args.Error);
				else if (args.Cancelled) tcs.SetCanceled();
				else tcs.SetResult(new LiveOperationResult(args.Result, args.RawResult));
			};
			if (userState == null)
				client.GetAsync(path);
			else
				client.GetAsync(path, userState);
			return tcs.Task;
		}
	}
}
