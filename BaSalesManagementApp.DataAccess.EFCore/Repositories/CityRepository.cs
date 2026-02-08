using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class CityRepository : EFBaseRepository<City>, ICityRepository
    {
        public CityRepository(BaSalesManagementAppDbContext context) : base(context)
        {
            
        }

       
    }
}
