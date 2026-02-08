using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;

namespace BaSalesManagementApp.MVC.Models.ProductVMs
{
    public class ProductUpdateVM
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public Guid? CategoryId { get; set; }
        public List<CategoryDTO> Categories { get; set; }

        public Guid? CompanyId { get; set; }
        public List<CompanyDTO> Companies { get; set; } = new List<CompanyDTO>();
    }
}
