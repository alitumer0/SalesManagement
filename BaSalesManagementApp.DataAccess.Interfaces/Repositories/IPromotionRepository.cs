namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IPromotionRepository:
        IAsyncRepository, IRepository,
        IAsyncTransactionRepository,
        IAsyncUpdateableRepository<Promotion>,
        IAsyncDeletableRepository<Promotion>,
        IAsyncFindableRepository<Promotion>,
        IAsyncInsertableRepository<Promotion>,
        IAsyncOrderableRepository<Promotion>,
        IAsyncQueryableRepository<Promotion>,
        IDeletableRepository<Promotion>
    {
    }
}
