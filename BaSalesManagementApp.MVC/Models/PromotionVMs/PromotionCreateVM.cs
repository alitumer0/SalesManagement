using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.MVC.Models.PromotionVMs
{
    public class PromotionCreateVM
    {
        public int? Discount { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid? ProductId { get; set; }
        public IEnumerable<ProductDTO>? Products { get; set; } = new List<ProductDTO>();

        public Guid? CompanyId { get; set; }
        public IEnumerable<CompanyDTO>? Companies { get; set; } = new List<CompanyDTO>();
        public Guid? CategoryId { get; set; }
        public IEnumerable<CategoryDTO>? Categories { get; set; } = new List<CategoryDTO>();
    }
}
