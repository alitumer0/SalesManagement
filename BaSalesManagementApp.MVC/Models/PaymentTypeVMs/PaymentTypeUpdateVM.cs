namespace BaSalesManagementApp.MVC.Models.PaymentTypeVMs
{
    public class PaymentTypeUpdateVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
    }
}
