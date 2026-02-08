namespace BaSalesManagementApp.Core.DataAccess.Interfaces
{
    public interface IAsyncRepository
    {
        Task<int> SaveChangeAsync(CancellationToken cancellationToken = default);
    }
}
