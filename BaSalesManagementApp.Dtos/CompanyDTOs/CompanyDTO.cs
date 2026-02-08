using BaSalesManagementApp.Core.Enums;
using System.ComponentModel;

namespace BaSalesManagementApp.Dtos.CompanyDTOs
{
    public class CompanyDTO
    {
        public Guid Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; } = null!;
        [DisplayName("Address")]
        public string Address { get; set; } = null!;
        [DisplayName("Phone")]
        public string Phone { get; set; } = null!;

        public string? CityName { get; set; }
        public string? CountryName { get; set; }

        public byte[]? CompanyPhoto { get; set; } // Fotoğraf verisi

        public Status Status { get; set; }
        public string LogoUrl { get; set; }
    }
}
