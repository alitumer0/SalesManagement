
using System.ComponentModel.DataAnnotations.Schema;

namespace BaSalesManagementApp.Entites.DbSets
{
    public class Employee : BaseUser
    {
        public byte[]? PhotoData { get; set; }

        public string? Title { get; set; }

        public Guid CompanyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
