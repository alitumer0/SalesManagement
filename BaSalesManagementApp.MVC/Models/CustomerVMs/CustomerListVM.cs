using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.CustomerVMs
{
    public class CustomerListVM
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

        public byte[]? CompanyPhoto { get; set; } = null!;

        [Display(Name = "City Name")]
        public string? CityName { get; set; }

        [Display(Name = "Country Name")]
        public string? CountryName { get; set; }

        public Guid? CityId { get; set; }
        public Guid? CountryId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
