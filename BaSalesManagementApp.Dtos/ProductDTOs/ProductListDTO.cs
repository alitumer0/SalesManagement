namespace BaSalesManagementApp.Dtos.ProductDTOs
{
    public class ProductListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal PurchasePrice { get; set; }

        //stringdi
        public byte[]? QRCode { get; set; }

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public byte[]? CompanyPhoto { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}