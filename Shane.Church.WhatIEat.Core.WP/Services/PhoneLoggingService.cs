using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;

namespace Shane.Church.WhatIEat.Core.WP.Services
{
    public class PhoneLoggingService : ILoggingService
    {
		private TelemetryClient _client;

		public PhoneLoggingService(TelemetryClient client)
		{
			if (client == null) throw new ArgumentNullException("client");
			_client = client;
		}

		public void LogMessage(string message)
		{
			_client.TrackEvent(message);
		}

		public void LogException(Exception ex, string message = null)
		{
			ExceptionTelemetry exData = new ExceptionTelemetry(ex);
			if (message != null)
			{
				exData.Properties.Add("message", message);
			}
			_client.TrackException(exData);
		}

		public void LogPurchaseComplete(ProductPurchaseInfo purchaseInfo)
		{
			var evtData = new EventTelemetry("In App Purchase Complete");
			evtData.Properties.Add("ProductId", purchaseInfo.ProductId);
			evtData.Properties.Add("ProductName", purchaseInfo.ProductName);
			evtData.Properties.Add("CommerceEngine", purchaseInfo.CommerceEngine);
			evtData.Properties.Add("CurrentMarket", purchaseInfo.CurrentMarket);
			evtData.Properties.Add("Currency", purchaseInfo.Currency);
			evtData.Properties.Add("Price", purchaseInfo.Price.ToString("N2"));

			_client.TrackEvent(evtData);
		}


		public void LogPageView(string page)
		{
			_client.TrackPageView(page);
		}
	}
}
