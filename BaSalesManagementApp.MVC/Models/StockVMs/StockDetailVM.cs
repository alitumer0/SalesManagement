namespace BaSalesManagementApp.MVC.Models.StockVMs
{
    public class StockDetailVM
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public string? WarehouseName { get; set; }
    }
}
