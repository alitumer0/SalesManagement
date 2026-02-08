namespace BaSalesManagementApp.Dtos.WarehouseDTOs
{
    public class WarehouseCreateDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public Guid BranchId { get; set; }
    }
}
