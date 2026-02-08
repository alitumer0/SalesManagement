using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entites.DbSets
{
    public class PaymentType:AuditableEntity
    {
        public string Name { get; set; }//Ödeme Tipi ismi
        public decimal Rate { get; set; }// % cinsinden komisyon miktarı 
        public Guid? CompanyId { get; set; }
    }
}
