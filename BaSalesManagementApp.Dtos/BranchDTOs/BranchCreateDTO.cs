using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.Dtos.BranchDTOs
{
    public class BranchCreateDTO
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public Guid CompanyId { get; set; }        
    }
}
