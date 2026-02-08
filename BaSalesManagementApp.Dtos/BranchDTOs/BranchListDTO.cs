using BaSalesManagementApp.Entites.DbSets;
using System.Text.Json.Serialization;

namespace BaSalesManagementApp.Dtos.BranchDTOs
{
    public class BranchListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;        
        public Guid CompanyId { get; set; }        
        public string CompanyName { get; set; } = null!;

        public byte[]? CompanyPhoto { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
