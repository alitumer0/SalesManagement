namespace BaSalesManagementApp.MVC.Models.CountryVMs
{
    public class CountryListVM
    {
        public Guid Id { get; set; }
        public string NameTr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        // Gösterimde kolaylık için: DisplayName
        public string Name { get; set; } = null!;

        public DateTime CreatedDate { get; set; }


    }
}
