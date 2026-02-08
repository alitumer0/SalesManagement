namespace BaSalesManagementApp.Dtos.StockDTOs
{
    public class StockDTO
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public Guid ProductId { get; set; }
        public string? WarehouseName { get; set; }
        public Guid WarehouseId { get; set; }
    }
}
