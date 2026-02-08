
namespace BaSalesManagementApp.Entites.DbSets
{
    public class Company: AuditableEntity
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? CountryCode { get; set; }

        public byte[]? CompanyPhoto { get; set; }

        public Guid? CityID { get; set; }

        public virtual City? City { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }

        public virtual List<Badge> Badges { get; set; } = new();
        public virtual List<Branch> Branches { get; set; } = new();
        public virtual List<Promotion> Promotions { get; set; } = new();
        public virtual List<Product> Products { get; set; } = new();
        public virtual List<Customer> Customers { get; set; } = new();
        public virtual List<Order> Orders { get; set; } = new();

    }
}
