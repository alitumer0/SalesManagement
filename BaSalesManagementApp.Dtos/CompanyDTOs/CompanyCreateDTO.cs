
using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.Dtos.CompanyDTOs
{
    public class CompanyCreateDTO
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public string? CountryCode { get; set; } = null!;

        public byte[]? CompanyPhoto { get; set; }


        public Guid? CityID { get; set; }
        //public Guid CountryId { get; set; }

        public Status Status { get; set; } = Status.Actived;     }
}
