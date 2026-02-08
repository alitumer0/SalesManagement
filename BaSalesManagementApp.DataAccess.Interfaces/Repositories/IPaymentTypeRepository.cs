using BaSalesManagementApp.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.Interfaces.Repositories
{
    public interface IPaymentTypeRepository: IRepository, IAsyncRepository, IAsyncTransactionRepository, IAsyncInsertableRepository<PaymentType>, IAsyncUpdateableRepository<PaymentType>, IAsyncDeletableRepository<PaymentType>, IAsyncQueryableRepository<PaymentType>, IAsyncOrderableRepository<PaymentType>, IAsyncFindableRepository<PaymentType>, IDeletableRepository<PaymentType>
    {
        Task<List<PaymentType>> GetPaymentTypesByCompanyIdAsync(Guid companyId);
    }
}
