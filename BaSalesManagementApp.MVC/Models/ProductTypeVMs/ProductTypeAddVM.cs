using BaSalesManagementApp.Dtos.CategoryDTOs;

namespace BaSalesManagementApp.MVC.Models.ProductTypeVMs
{
    public class ProductTypeAddVM
    {
        public string Name { get; set; }

        public Guid? CategoryId { get; set; }
        public List<CategoryDTO>? Categories { get; set; }

        public string Description { get; set; }
    }
}