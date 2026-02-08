namespace BaSalesManagementApp.Dtos.ProductDTOs
{
    public class ProductCreateDTO
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal PurchasePrice { get; set; }
        public byte[]? QRCode { get; set; }

        public Guid CategoryId { get; set; }

        public Guid CompanyId { get; set; }
    }
}
