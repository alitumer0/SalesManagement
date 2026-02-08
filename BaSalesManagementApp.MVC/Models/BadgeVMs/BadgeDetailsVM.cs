using BaSalesManagementApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.BadgeVMs
{
    public class BadgeDetailsVM
    {

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        [Display(Name = "Company")]
        public string CompanyName { get; set; } = null!;

        public Status Status { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
