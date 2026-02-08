namespace BaSalesManagementApp.Dtos.ProductTypeDtos
{
    public class StockTypeUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid CategoryId { get; set; }
        public string Description { get; set; }
    }
}