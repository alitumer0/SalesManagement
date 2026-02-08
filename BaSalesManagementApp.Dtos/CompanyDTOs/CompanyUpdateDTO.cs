using BaSalesManagementApp.Core.Enums;

namespace BaSalesManagementApp.Dtos.CompanyDTOs
{
    public class CompanyUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public byte[]? CompanyPhoto { get; set; }
        public bool RemovePhoto { get; set; }







        public Guid CityId { get; set; }
        public Guid CountryId { get; set; }
        public Status Status { get; set; }

        public string? CountryCode { get; set; }
    }
}
