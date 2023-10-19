using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boutique.Patterns.Strategy
{
    internal class CustomerBill
    {
        public IBillingStrategy Strategy { get; set; }
        public CustomerBill(IBillingStrategy strategy)
        {
            this.Strategy = strategy;
        }
        public double LastPrice(double price, double percent)
        {
            return this.Strategy.GetActPrice(price * (percent/100)); 
        }
    }
}