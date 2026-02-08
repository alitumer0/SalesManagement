using BaSalesManagementApp.Dtos.CityDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;
using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.MVC.Models.CustomerVMs
{
    public class CustomerUpdateVM
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = null!;
        public string? Address { get; set; } = null!;
        public string? Phone { get; set; } = null!;
		public string? Email { get; set; }


		public string? FullPhoneNumber { get; set; }  // Ülke Kodunu İçeren Telefon Numarası
		public string? CountryCode { get; set; }

        public Guid? CompanyId { get; set; }
        public Guid? CityId { get; set; }
        public Guid? CountryId { get; set; }
        public List<CompanyDTO>? Companies { get; set; } = new List<CompanyDTO>();
        public List<CityDTO> Cities { get; set; } = new List<CityDTO>();
        public List<CountryDTO> Countries { get; set; } = new List<CountryDTO>();
    }
}
