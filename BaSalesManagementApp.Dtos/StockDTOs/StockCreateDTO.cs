namespace BaSalesManagementApp.Dtos.StockDTOs
{
    public class StockCreateDTO
    {
        public int Count { get; set; }
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
    }
}
