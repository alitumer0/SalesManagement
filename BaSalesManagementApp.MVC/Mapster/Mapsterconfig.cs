using BaSalesManagementApp.Dtos.CityDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.OrderDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Extensions;
using BaSalesManagementApp.MVC.Models.CityVMs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.OrderVMs;

namespace BaSalesManagementApp.MVC.Mapster
{
    public static class Mapsterconfig
    {

        /// <summary>
        /// CompanyPhoto dönşümü için gerekli olan mapster configürasyonu
        /// </summary>
        public static void Mapping()
        {

            TypeAdapterConfig<CompanyCreateVM, CompanyCreateDTO>.NewConfig()
                .Map(dest => dest.CompanyPhoto, src => FormFileExtensions.ConvertFormFileToByteArray(src.CompanyPhoto));

            TypeAdapterConfig<OrderCreateVM, OrderCreateDTO>.NewConfig()
               .Map(dest => dest.TotalPrice, src => src.TotalPrice / 100);
            TypeAdapterConfig<OrderDetailCreateDTO, OrderDetailCreateDTO>.NewConfig()
                .Map(dest => dest.TotalPrice, src => src.TotalPrice / 100)
                .Map(dest => dest.UnitPrice, src => src.UnitPrice / 100);

            TypeAdapterConfig<Order, OrderListDTO>.NewConfig()
                .Map(dest => dest.PaymentTypeId, src => src.PaymentTypeId);

            TypeAdapterConfig<City, CityListDTO>.NewConfig()
            .Map(dest => dest.CountryName,
            src => CultureInfo.CurrentCulture.Name == "tr-TR" ? src.Country.NameTr : src.Country.NameEn);
            TypeAdapterConfig<City, CityDTO>.NewConfig()
            .Map(dest => dest.CountryName,
            src => CultureInfo.CurrentCulture.Name == "tr-TR" ? src.Country.NameTr : src.Country.NameEn);

        }
    }
}
