namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IStockTypeRepository 
        : 
        IAsyncRepository,
        IAsyncTransactionRepository,
        IAsyncInsertableRepository<StockType>, IAsyncDeletableRepository<StockType>, IAsyncUpdateableRepository<StockType>, 
        IAsyncFindableRepository<StockType>, IAsyncQueryableRepository<StockType>,
        IAsyncOrderableRepository<StockType>,
        IDeletableRepository<StockType>,
        IRepository
    {
    }
}