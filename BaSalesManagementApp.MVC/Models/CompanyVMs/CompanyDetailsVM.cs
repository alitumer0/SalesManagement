using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.EmployeeDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.EmployeeVMs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BaSalesManagementApp.MVC.Models.CompanyVMs
{
    public class CompanyDetailsVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;

        //public string? FullAddress { get; set; } = null!;

        public string? CityName { get; set; } = null!;
        public string? CountryName { get; set; } = null!;

        public CompanyDTO Company { get; set; }

        public string Phone { get; set; } = null!;

        //public string? CountryCode { get; set; } = null!;
        public Status Status { get; set; }
        public string LogoUrl { get; set; }

        //şirket çalışanları listesi
        public List<SelectListItem> Employees { get; set; } = new List<SelectListItem>();

    }
}
