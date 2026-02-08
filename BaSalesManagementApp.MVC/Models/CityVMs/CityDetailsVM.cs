namespace BaSalesManagementApp.MVC.Models.CityVMs
{
    public class CityDetailsVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public string CountryName { get; set; }
    }
}
