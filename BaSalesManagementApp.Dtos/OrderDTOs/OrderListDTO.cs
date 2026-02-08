using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.EmployeeDTOs;

namespace BaSalesManagementApp.Dtos.OrderDTOs
{
    public class OrderListDTO
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsActive { get; set; }
        public Guid AdminId { get; set; }
        public string? AdminName { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeLastName { get; set; }
        public string Email { get; set; }
        public List<OrderDetailListDTO> OrderDetails { get; set; } = new List<OrderDetailListDTO>();
        public CurrencyType? CurrencyType { get; set; }
        public Guid? PaymentTypeId { get; set; }
    }
}