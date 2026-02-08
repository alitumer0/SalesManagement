namespace BaSalesManagementApp.Core.Entities.Base
{
    public class AuditableEntity: BaseEntity
    {
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
