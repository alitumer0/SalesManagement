namespace BaSalesManagementApp.Dtos.StockDTOs
{
    public class StockUpdateDTO
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }

    }
}
