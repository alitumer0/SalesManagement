namespace BaSalesManagementApp.MVC.Models.StockTypeSizeVMs
{
    public class StockTypeSizeListVM
    {
        public Guid Id { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? StockTypeName { get; set; }
        public Guid? CompanyId { get; set; }

    }
}
