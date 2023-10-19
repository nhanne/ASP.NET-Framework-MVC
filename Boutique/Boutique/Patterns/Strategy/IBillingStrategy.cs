using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boutique.Patterns.Strategy
{
    internal interface IBillingStrategy
    {
        double GetActPrice(double rawPrice);
    }
    class NormalStrategy : IBillingStrategy
    {
        public double GetActPrice(double rawPrice) => rawPrice;
    }
    class HappyMonthStrategy : IBillingStrategy
    {
        public double GetActPrice(double rawPrice) => rawPrice * 0.95;
    }
    class HappyHourStrategy : IBillingStrategy{
        public double GetActPrice(double rawPrice) => rawPrice - 100000;
    }

}
