namespace BaSalesManagementApp.Dtos.ProductTypeDtos
{
    public class StockTypeListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string CategoryName { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}