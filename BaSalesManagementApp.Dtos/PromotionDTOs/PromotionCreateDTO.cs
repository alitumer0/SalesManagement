namespace BaSalesManagementApp.Dtos.PromotionDTOs
{
    public class PromotionCreateDTO
    {
        public int Discount { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ProductId { get; set; }
        public Guid CompanyId { get; set; }
    }
}
