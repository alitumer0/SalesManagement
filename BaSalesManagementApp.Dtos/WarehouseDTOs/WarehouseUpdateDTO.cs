namespace BaSalesManagementApp.Dtos.WarehouseDTOs
{
    public class WarehouseUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        
        public Guid BranchId { get; set; }
    }
}
