namespace BaSalesManagementApp.MVC.Models.CityVMs
{
    public class CityListVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public string CountryName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
