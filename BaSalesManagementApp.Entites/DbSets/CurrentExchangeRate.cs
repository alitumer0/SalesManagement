using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entites.DbSets
{
    public class CurrentExchangeRate:AuditableEntity
    {
        public decimal DollarRate { get; set; } // Dolar kuru
        public decimal EuroRate { get; set; } // Euro kuru
    }

}
