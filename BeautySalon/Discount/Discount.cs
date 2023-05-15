using BeautySalon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Discount
{
   
    public class Discount : IDiscount
    {

        public void DefaultRecord(ClientService record)
        {
            decimal cost = record.Service.Cost;
            record.Cost = cost;
        }

        public void BirthDayDiscount(ClientService record)
        {
            decimal cost = record.Service.Cost;
            decimal percent = 0.8m;
            decimal newCost = (decimal) (cost * percent);
            record.Cost = newCost;
        }

        public void EverySixDiscount(ClientService record)
        {
            decimal cost = record.Service.Cost;
            decimal percent = 0.85m;
            decimal newCost = (decimal)(cost * percent);
            record.Cost = newCost;
        }
    }
}
