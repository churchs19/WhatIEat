using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Services;
using System;

namespace Shane.Church.WhatIEat.Core.WP.Services
{
    public class PhoneLoggingService : ILoggingService
    {
        public void LogMessage(string message)
        {
            MarkedUp.AnalyticClient.Info(message);
        }

        public void LogException(Exception ex, string message = null)
        {
            if (message == null)
            {
                MarkedUp.AnalyticClient.Error(ex.Message, ex);
            }
            else
            {
                MarkedUp.AnalyticClient.Error(message, ex);
            }
        }

        public void LogPurchaseComplete(ProductPurchaseInfo purchaseInfo)
        {
            var iap = new MarkedUp.InAppPurchase() {
                ProductId = purchaseInfo.ProductId,
                ProductName = purchaseInfo.ProductName,
                CommerceEngine = purchaseInfo.CommerceEngine,
                CurrentMarket = purchaseInfo.CurrentMarket,
                Currency = purchaseInfo.Currency,
                Price = purchaseInfo.Price
            };
            MarkedUp.AnalyticClient.InAppPurchaseComplete(iap);
        }
    }
}
