namespace BaSalesManagementApp.Dtos.WarehouseDTOs
{
    public class WarehouseListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string BranchName { get; set; }
        public Guid BranchId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
