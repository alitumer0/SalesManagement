namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class StudentRepository : EFBaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(BaSalesManagementAppDbContext context) : base(context)
        {

        }
    }
}
