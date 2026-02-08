
using BaSalesManagementApp.Core.Enums;

namespace BaSalesManagementApp.Entites.DbSets
{
    public class Order : AuditableEntity
    {
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsActive { get; set; }
        public string OrderNo { get; set; }
        public Guid AdminId { get; set; }
        public virtual Admin Admin { get; set; }
		public Guid? CustomerId { get; set; }       //müşterinin siparişi için gerekli olan property
        public virtual Customer? Customer { get; set; }
        public Guid? CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public Guid? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public Guid? PaymentTypeId { get; set; }
        public CurrencyType? CurrencyType { get; set; }
    }
}
