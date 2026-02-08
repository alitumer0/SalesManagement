using BaSalesManagementApp.Dtos.OrderDetailDTOs;

namespace BaSalesManagementApp.Dtos.OrderDTOs
{
    public class OrderUpdateDTO
    {
        public Guid Id { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid AdminId { get; set; }
        public List<OrderDetailUpdateDTO> OrderDetails { get; set; }

        




       
    }
}
