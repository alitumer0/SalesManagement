using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.MVC.Models.AdminVMs;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.CustomerVMs
{
    public class CustomerDetailsVM
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Display(Name = "Phone")]
        public string Phone { get; set; } = null!;

        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        [Display(Name = "City Name")]
        public string? CityName { get; set; }

        [Display(Name = "Country Name")]
        public string? CountryName { get; set; }
        public string Email { get; set; }

        public AdminDetailsVM Admin { get; set; }
        public CompanyDTO Company { get; set; }
    }
}
