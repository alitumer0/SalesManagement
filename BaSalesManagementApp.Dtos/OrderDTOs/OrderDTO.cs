using BaSalesManagementApp.Dtos.OrderDetailDTOs;

namespace BaSalesManagementApp.Dtos.OrderDTOs
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsActive { get; set; }
        public Guid AdminId { get; set; }
        public Guid CompanyId { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
