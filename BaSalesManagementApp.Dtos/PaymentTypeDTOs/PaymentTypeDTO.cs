using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.PaymentTypeDTOs
{
    public class PaymentTypeDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }//Ödeme Tipi ismi
        public decimal Rate { get; set; }// % cinsinden komisyon miktarı 
        public Guid? CompanyId { get; set; }

    }
}
