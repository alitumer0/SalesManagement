namespace BaSalesManagementApp.MVC.Models.PaymentTypeVMs
{
    // Test
    public class PaymentTypeCreateVM
    {
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
