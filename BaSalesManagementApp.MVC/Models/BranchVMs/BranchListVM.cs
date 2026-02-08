using BaSalesManagementApp.Entites.DbSets;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BaSalesManagementApp.MVC.Models.BranchVMs
{
    public class BranchListVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;

        [Display(Name = "Branch")]
        public string BranchName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Company")]
        public string CompanyName { get; set; } = null!;

        public byte[]? CompanyPhoto { get; set; } = null!;
    }
}
