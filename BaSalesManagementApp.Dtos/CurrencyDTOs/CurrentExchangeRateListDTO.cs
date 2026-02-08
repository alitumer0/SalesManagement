using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.CurrencyDTOs
{
    public class CurrentExchangeRateListDTO
    {
        public DateTime CreatedDate { get; set; } 
        public decimal DollarRate { get; set; }
        public decimal EuroRate { get; set; }
    }
}
