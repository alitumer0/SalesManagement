namespace BaSalesManagementApp.Dtos.WarehouseDTOs
{
    public class WarehouseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Guid BranchId { get; set; }
        public string BranchName { get; set; }
    }
}
