using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.BadgeVMs
{
    public class BadgeListVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        [Display(Name = "Badge")]
        public string BadgeName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Company")]
        public string CompanyName { get; set; } = null!;

        public byte[]? CompanyPhoto { get; set; } = null!;
    }
}
