namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class PromotionRepository:EFBaseRepository<Promotion>,IPromotionRepository
    {
        public PromotionRepository(BaSalesManagementAppDbContext context) : base(context)
        {
            
        }
    }
}
