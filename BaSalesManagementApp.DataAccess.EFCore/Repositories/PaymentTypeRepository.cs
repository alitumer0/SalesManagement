using BaSalesManagementApp.Core.Utilities.Results;
using Microsoft.EntityFrameworkCore;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class PaymentTypeRepository : EFBaseRepository<PaymentType>, IPaymentTypeRepository
    {
        private readonly BaSalesManagementAppDbContext _dbContext;
        public PaymentTypeRepository(BaSalesManagementAppDbContext context):base(context)
        {
            _dbContext = context;
        }

        public async Task<List<PaymentType>> GetPaymentTypesByCompanyIdAsync(Guid companyId)
        {
            var paymentTypes = await _dbContext.PaymentTypes
              .Where(p => p.CompanyId == companyId)
              .ToListAsync();
            return paymentTypes;
        }
    }
}
