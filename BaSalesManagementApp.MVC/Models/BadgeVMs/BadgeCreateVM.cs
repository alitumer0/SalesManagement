using BaSalesManagementApp.Entites.DbSets;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.BadgeVMs
{
    public class BadgeCreateVM
    {
        public string? Name { get; set; } = null!;
        public int? SalesQuantity { get; set; }
        public Guid? CompanyId { get; set; }
        public IEnumerable<Company>? Companies { get; set; } = new List<Company>();

    }
}
