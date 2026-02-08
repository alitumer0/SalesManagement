
using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Entites.DbSets;
using System.ComponentModel.DataAnnotations;


namespace BaSalesManagementApp.MVC.Models.CompanyVMs
{
    public class CompanyCreateVM
    {
        public string? Name { get; set; } = null!;
        public string? Address { get; set; } = null!;
        public string? Phone { get; set; } = null!;

        public string? CountryCode { get; set; } = null!;
        public Guid? CityID { get; set; }

        public Guid? CountryId { get; set; }

        //Kullanıcıdan yüklenen fotografi temsil eder
        public IFormFile CompanyPhoto { get; set; }
        public List<City>? Cities { get; set; } = new List<City>();
        public List<Country>? Countries { get; set; } = new List<Country>();
        public Status Status { get; set; } = Status.Actived;



    }


}

