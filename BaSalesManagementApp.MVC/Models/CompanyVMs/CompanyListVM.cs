using BaSalesManagementApp.Core.Enums;

namespace BaSalesManagementApp.MVC.Models.CompanyVMs
{
    public class CompanyListVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? CountryCode { get; set; } = null!;
        public Status Status { get; set; }
        public Guid CountryId { get; set; } 
        public string CountryName { get; set; } = null!;
        public byte[]? CompanyPhoto { get; set; }     //Company logosu için gerekli property
        public string? CompanyPhotoBase64 { get; set; }  // Base64 fotoğraf verisi
    }
}
