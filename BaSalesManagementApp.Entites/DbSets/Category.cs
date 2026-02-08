
namespace BaSalesManagementApp.Entites.DbSets
{
    public class Category : AuditableEntity
    {
        public string Name { get; set; }
        public Guid? CompanyId { get; set; }

    }
}
