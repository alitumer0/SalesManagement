using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;

namespace BaSalesManagementApp.MVC.Models.WarehouseVMs
{
    public class WarehouseCreateVM
    {
        public string? Name { get; set; }
        public string? Address { get; set; }

        public Guid? SelectedCompanyId { get; set; }
        public List<CompanyDTO>? Companies { get; set; }

        public Guid? BranchId { get; set; }
        public List<BranchDTO>? Branches { get; set; }
    }
}
