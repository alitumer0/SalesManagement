using BaSalesManagementApp.Dtos.CountryDTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BaSalesManagementApp.MVC.Models.CityVMs
{
    public class CityCreateVM
    {
        public string? Name { get; set; } = null!;

        public Guid? CountryId { get; set; }

        public List<CountryDTO> Countries { get; set; } = new List<CountryDTO>();
    }
}
