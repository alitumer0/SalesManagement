using BaSalesManagementApp.Core.Utilities.Results;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IProductRepository : IRepository, IAsyncRepository, IAsyncTransactionRepository, IAsyncInsertableRepository<Product>, IAsyncUpdateableRepository<Product>, IAsyncDeletableRepository<Product>, IAsyncQueryableRepository<Product>, IAsyncOrderableRepository<Product>, IAsyncFindableRepository<Product>, IDeletableRepository<Product>
    {
        Task<IResult> SetProductToPassiveAsync(Guid productId);
        Task<List<Product>> GetProductsByCompanyIdAsync(Guid companyId);
        Task<Product> GetByNameAsync(string productName);

      
    }
}