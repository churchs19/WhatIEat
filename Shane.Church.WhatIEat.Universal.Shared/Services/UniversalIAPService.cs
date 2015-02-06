using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace Shane.Church.WhatIEat.Universal.Shared.Services
{
    public class UniversalIAPService : IIAPService
    {
        private string[] _productCodes = new string[] {"RemoveAds"};
        private ListingInformation _listings;
        private ILoggingService _log;

        public UniversalIAPService(ILoggingService log)
        {
            if (log == null) { throw new ArgumentNullException("log"); }
            _log = log;
        }

        public async Task<string[]> GetProductIds()
        {
            await GetListings();
            return await Task.FromResult(_productCodes);
        }
        
        public async Task PurchaseProduct(string productId)
        {
            try
            {
                // Kick off purchase; don't ask for a receipt when it returns
                var purchaseResult = await CurrentApp.RequestProductPurchaseAsync(productId);

                await GetListings();

                var productInfo = _listings.ProductListings[productId];
                double price = 0;
                if (!double.TryParse(productInfo.FormattedPrice, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.CurrentUICulture, out price))
                {
                    price = 0;
                }
                var purchaseInfo = new ProductPurchaseInfo()
                {
                    ProductId = productId,
                    ProductName = productInfo.Name,
                    CommerceEngine = "Windows Phone Store",
                    CurrentMarket = _listings.CurrentMarket,
                    Price = price,
                    Currency = System.Globalization.CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol
                };

                _log.LogMessage(productId + " Purchased");
                _log.LogPurchaseComplete(purchaseInfo);

                // Now that purchase is done, give the user the goods they paid for
                // (DoFulfillment is defined later)
                await DoFulfillment();
            }
            catch (Exception ex)
            {
                // When the user does not complete the purchase (e.g. cancels or navigates back from the Purchase Page), an exception with an HRESULT of E_FAIL is expected.
                _log.LogException(ex, productId + " Purchase Aborted");
            }
        }

#pragma warning disable 1998
        public async Task DoFulfillment()
        {
            
        }
#pragma warning restore 1998

        public bool AreAdsVisible()
        {
#if !PERSONAL
            var license = CurrentApp.LicenseInformation.ProductLicenses[_productCodes[0]];
            return !license.IsActive;
#else
            return false;
#endif
        }

        private async Task<ListingInformation> GetListings()
        {
            if (_listings == null)
            {
                _listings = await CurrentApp.LoadListingInformationAsync();
            }
            return _listings;
        }
    }
}
