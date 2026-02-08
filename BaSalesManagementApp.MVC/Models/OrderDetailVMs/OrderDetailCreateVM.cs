namespace BaSalesManagementApp.MVC.Models.OrderDetailVMs
{
    public class OrderDetailCreateVM
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}
