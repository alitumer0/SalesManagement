namespace BaSalesManagementApp.Dtos.ProductTypeDtos
{
    public class StockTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public Guid CategoryId { get; set; }

    }
}