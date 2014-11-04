using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.WhatIEat.Core.Data
{
    public class ProductPurchaseInfo
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string CommerceEngine { get; set; }
        public string CurrentMarket { get; set; }
        public string Currency { get; set; }
        public double Price { get; set; }
    }
}
