namespace BaSalesManagementApp.Core.DataAccess.Interfaces
{
    public interface IDeletableRepository<TEntity> : IRepository where TEntity : BaseEntity
    {
        void Delete(TEntity entity);
    }
}
