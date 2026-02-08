using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entites.DbSets
{
    public class Country: AuditableEntity
    {
        public string NameTr { get; set; }
        public string? NameEn { get; set; }

        // Nav Prop
        public virtual IEnumerable<Customer> Customers { get; set; }
    }
}
