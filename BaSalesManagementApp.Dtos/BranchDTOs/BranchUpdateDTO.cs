using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.Dtos.BranchDTOs
{
    public class BranchUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public Guid CompanyId { get; set; }        
    }
}
