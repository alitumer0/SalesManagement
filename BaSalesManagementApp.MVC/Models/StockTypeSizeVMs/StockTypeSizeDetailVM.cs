namespace BaSalesManagementApp.MVC.Models.StockTypeSizeVMs
{
    public class StockTypeSizeDetailVM
    {
        public Guid Id { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public string? StockTypeName { get; set; }

    }
}
