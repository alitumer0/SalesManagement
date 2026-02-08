namespace BaSalesManagementApp.Dtos.ProductTypeDtos
{
    public class StockTypeAddDto
    {
        public string Name { get; set; }

        public Guid CategoryId { get; set; }

        public string Description { get; set; }
    }
}