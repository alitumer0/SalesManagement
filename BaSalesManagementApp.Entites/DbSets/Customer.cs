namespace BaSalesManagementApp.Entites.DbSets
{
    public class Customer : AuditableEntity
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public string? CountryCode { get; set; }
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        //Nav Prop

        public Guid CityId { get; set; }
        public virtual City City { get; set; }

        public Guid CountryId { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<Order> Orders { get; set; }      //Müşterinin siparişleri için eklendi
    }
}
