using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.Dtos.BranchDTOs
{
    public class BranchDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
    }
}
