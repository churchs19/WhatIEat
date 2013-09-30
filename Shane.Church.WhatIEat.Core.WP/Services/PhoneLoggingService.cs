using Shane.Church.WhatIEat.Core.Services;
using System;

namespace Shane.Church.WhatIEat.Core.WP.Services
{
	public class PhoneLoggingService : ILoggingService
	{
		public void LogMessage(string message)
		{
			FlurryWP8SDK.Api.LogEvent(message);
		}

		public void LogException(Exception ex, string message = null)
		{
			var msg = message;
			if (string.IsNullOrWhiteSpace(msg)) msg = "What I Eat Exception";
			FlurryWP8SDK.Api.LogError(msg, ex);
		}
	}
}
