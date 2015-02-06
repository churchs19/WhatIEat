using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;

namespace Shane.Church.WhatIEat.Universal.Shared.Services
{
	public class UniversalLoggingService : ILoggingService
	{
		private static Microsoft.ApplicationInsights.TelemetryClient client = new TelemetryClient();

		public void LogMessage(string message)
		{
			client.TrackEvent(message);
		}

		public void LogException(Exception ex, string message = null)
		{
			var telemetryItem = new ExceptionTelemetry(ex);
			if (message != null)
			{
				telemetryItem.Properties.Add("ApplicationMessage", message);
			}
			client.TrackException(telemetryItem);
		}

		public void LogPurchaseComplete(ProductPurchaseInfo purchaseInfo)
		{
			var telemetryItem = new EventTelemetry("In App Purchase Complete");
			telemetryItem.Properties.Add("ProductId", purchaseInfo.ProductId);
			telemetryItem.Properties.Add("ProductName", purchaseInfo.ProductName);
			telemetryItem.Properties.Add("CommerceEngine", purchaseInfo.CommerceEngine);
			telemetryItem.Properties.Add("CurrentMarket", purchaseInfo.CurrentMarket);
			telemetryItem.Properties.Add("Currency", purchaseInfo.Currency);
			telemetryItem.Properties.Add("Price", purchaseInfo.Price.ToString());

			client.TrackEvent(telemetryItem);
		}


		public void LogPageView(string page)
		{
			client.TrackPageView(page);
		}
	}
}
