using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace Shane.Church.WhatIEat.Core.WP8.Services
{
    public class WP8IAPService : IIAPService
    {
        private string[] _productCodes = new string[] {"RemoveAds"};
        private ListingInformation _listings;

        public async Task<string[]> GetProductIds()
        {
            if (_listings == null)
            {
                _listings = await CurrentApp.LoadListingInformationAsync();
            }
            return await Task.FromResult(_productCodes);
        }
        
        public async Task PurchaseProduct(string productId)
        {
            try
            {
                // Kick off purchase; don't ask for a receipt when it returns
                await CurrentApp.RequestProductPurchaseAsync(productId, false);

                FlurryWP8SDK.Api.LogEvent(productId + " Purchased");

                // Now that purchase is done, give the user the goods they paid for
                // (DoFulfillment is defined later)
                await DoFulfillment();
            }
            catch (Exception ex)
            {
                // When the user does not complete the purchase (e.g. cancels or navigates back from the Purchase Page), an exception with an HRESULT of E_FAIL is expected.
                FlurryWP8SDK.Api.LogError(productId + " Purchase Aborted", ex);
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
    }
}
