using BaSalesManagementApp.Entites.DbSets;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.BranchVMs
{
    public class BranchDetailsVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;

        [Display(Name = "Company")]
        public string CompanyName { get; set; } = null!;
    }
}
