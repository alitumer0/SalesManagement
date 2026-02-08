using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;

namespace BaSalesManagementApp.MVC.Models.ProductVMs
{
    public class ProductCreateVM
    {
        public string? Name { get; set; } = null!;
        public decimal? Price { get; set; } = null!;
        public decimal PurchasePrice { get; set; }

        public Guid? CategoryId { get; set; }
        public List<CategoryDTO>? Categories { get; set; } /*= new List<CategoryDTO>();*/

        public Guid? CompanyId { get; set; }
        public List<CompanyDTO>? Companies { get; set; } = new List<CompanyDTO>(); 
    }
}
