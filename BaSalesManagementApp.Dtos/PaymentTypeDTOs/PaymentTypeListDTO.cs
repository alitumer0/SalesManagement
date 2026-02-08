using BaSalesManagementApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Dtos.PaymentTypeDTOs
{
    public class PaymentTypeListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }//Ödeme Tipi ismi
        public decimal Rate { get; set; }// % cinsinden komisyon miktarı 
        public Status Status { get; set; }//Ödeme tipinin aktif olup olmadığını belirten property

        public DateTime CreatedDate { get; set; }
    }
}
