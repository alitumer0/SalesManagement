namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IWarehouseRepository : IAsyncRepository,IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<Warehouse>, IAsyncDeletableRepository<Warehouse>, IAsyncFindableRepository<Warehouse>, IAsyncInsertableRepository<Warehouse>, IAsyncOrderableRepository<Warehouse>, IAsyncQueryableRepository<Warehouse>, IDeletableRepository<Warehouse>
    {
    }
}
