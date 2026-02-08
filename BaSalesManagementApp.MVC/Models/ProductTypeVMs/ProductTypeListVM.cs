namespace BaSalesManagementApp.MVC.Models.ProductTypeVMs
{
    public class ProductTypeListVM
    {
        public Guid Id { get; set; }

        public string CategoryName { get; set; }
        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}