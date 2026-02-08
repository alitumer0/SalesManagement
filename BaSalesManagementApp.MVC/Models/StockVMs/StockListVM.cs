namespace BaSalesManagementApp.MVC.Models.StockVMs
{
    public class StockListVM
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? WarehouseName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
