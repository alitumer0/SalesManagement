using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CurrencyDTOs;
using BaSalesManagementApp.Dtos.MyProfileDTO;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using System.Globalization;

namespace BaSalesManagementApp.Business.Services.Adapt
{
    public static class MapingConfig
    {
        public static void RegisterMappings()
        {
           
            TypeAdapterConfig<MyProfileDTO, Admin>.NewConfig()
                .Ignore(dest => dest.Id)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhotoData, src => src.PhotoData);

            TypeAdapterConfig<MyProfileDTO, Employee>.NewConfig()
                .Ignore(dest => dest.Id)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhotoData, src => src.PhotoData);

            
            TypeAdapterConfig<OrderDetail, OrderDetailListDTO>.NewConfig()
                .Map(dest => dest.ProductName, src => src.Product.Name)
                .Map(dest => dest.CompanyName, src => src.Product.Company.Name)
                .Map(dest => dest.IsCompanyActive, src => src.Product.Company.Status != Status.Passive);

           
            TypeAdapterConfig<Company, CompanyDTO>.NewConfig()
                .Map(dest => dest.LogoUrl, src => src.CompanyPhoto != null
                    ? $"data:image/png;base64,{Convert.ToBase64String(src.CompanyPhoto)}"
                    : "/img/CompanyDefault.png");

            // Company → CompanyListDTO (Ülke adı ve ID'si dahil)
            TypeAdapterConfig<Company, CompanyListDTO>.NewConfig()
                .Map(dest => dest.CountryId,
                     src => src.City != null && src.City.Country != null ? src.City.Country.Id : Guid.Empty)
                .Map(dest => dest.CountryName,
                     src => src.City != null && src.City.Country != null
                        ? (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "tr"
                            ? src.City.Country.NameTr
                            : src.City.Country.NameEn)
                        : "Bilinmiyor");

           
            TypeAdapterConfig<CurrentExchangeRate, CurrentExchangeRateListDTO>
               .NewConfig()
               .Map(dest => dest.CreatedDate, src => src.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
