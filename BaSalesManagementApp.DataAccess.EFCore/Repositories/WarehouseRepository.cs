namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class WarehouseRepository : EFBaseRepository<Warehouse>,IWarehouseRepository
    {
        public WarehouseRepository(BaSalesManagementAppDbContext context) : base(context) 
        {
            
        }               
    }
}
