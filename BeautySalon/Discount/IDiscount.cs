using BeautySalon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Discount
{
    public interface IDiscount 
    {
        void DefaultRecord(ClientService record);
        void BirthDayDiscount(ClientService record);
        void EverySixDiscount(ClientService record);
    }
}
