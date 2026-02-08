namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class StockTypeRepository : EFBaseRepository<StockType>, IStockTypeRepository
    {
        public StockTypeRepository(BaSalesManagementAppDbContext db) : base(db)
        {
        }
    }
}