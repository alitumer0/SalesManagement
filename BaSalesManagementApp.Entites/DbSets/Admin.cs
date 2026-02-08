
namespace BaSalesManagementApp.Entites.DbSets
{
    public class Admin: BaseUser
    {
        public byte[]? PhotoData { get; set; }

        //NAV
        //Order ile One to many ilişkisi kuruldu.
        public virtual ICollection<Order>? Orders { get; set; } = new HashSet<Order>();
    }
}
