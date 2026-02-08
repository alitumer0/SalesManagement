using BaSalesManagementApp.Dtos.CountryDTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaSalesManagementApp.MVC.Models.CityVMs
{
    public class CityUpdateVM
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } 

        public Guid? CountryId { get; set; }

        public List<SelectListItem>? Countries { get; set; }

    }
}
