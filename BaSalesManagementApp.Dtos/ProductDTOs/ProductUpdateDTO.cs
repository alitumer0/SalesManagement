namespace BaSalesManagementApp.Dtos.ProductDTOs
{
    public class ProductUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }

        //public byte[]? QRCode { get; set; }
        public Guid CategoryId { get; set; }

        public Guid CompanyId { get; set; }

    }
}
