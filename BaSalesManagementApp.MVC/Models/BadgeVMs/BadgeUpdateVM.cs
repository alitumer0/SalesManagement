using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.MVC.Models.BadgeVMs
{
    public class BadgeUpdateVM
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = null!;
        public Guid? CompanyId { get; set; }
        public IEnumerable<Company>? Companies { get; set; } = new List<Company>();
    }
}
