namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IStudentRepository : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<Student>, IAsyncDeletableRepository<Student>, IAsyncFindableRepository<Student>, IAsyncInsertableRepository<Student>, IAsyncOrderableRepository<Student>, IAsyncQueryableRepository<Student>, IDeletableRepository<Student>
    {

    }
}
