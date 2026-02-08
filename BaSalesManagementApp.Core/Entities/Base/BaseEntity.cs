namespace BaSalesManagementApp.Core.Entities.Base
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Status Status{ get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
