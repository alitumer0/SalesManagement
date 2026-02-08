namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IStockRepository : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<Stock>, IAsyncDeletableRepository<Stock>, IAsyncFindableRepository<Stock>, IAsyncInsertableRepository<Stock>, IAsyncOrderableRepository<Stock>, IAsyncQueryableRepository<Stock>, IDeletableRepository<Stock>
    {
        Task<Stock?> GetStockByWarehouseAndProductAsync(Guid warehouseId, Guid productId);
    }
}
