using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Entites.DbSets
{
    public class City : AuditableEntity
    {
        public string Name { get; set; }

        // Nav Prop
        public Guid CountryId { get; set; }

        public virtual Country Country { get; set; }

        public virtual IEnumerable<Customer> Customers { get; set; }

        public virtual ICollection<Company> Companies { get; set; }
    }
}
