using Shane.Church.WhatIEat.Core.Data;
using System;

namespace Shane.Church.WhatIEat.Core.Services
{
	public interface ILoggingService
	{
		void LogMessage(string message);
		void LogException(Exception ex, string message = null);
		void LogPurchaseComplete(ProductPurchaseInfo purchaseInfo);
		void LogPageView(string page);
	}
}
