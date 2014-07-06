using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.WhatIEat.Core.Services
{
    public interface IIAPService
    {
        Task<string[]> GetProductIds();

        Task PurchaseProduct(string ProductId);

        Task DoFulfillment();

        bool AreAdsVisible();
    }
}
