using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class CountryRepository : EFBaseRepository<Country>, ICountryRepository
    {
        public CountryRepository(BaSalesManagementAppDbContext context) : base(context)
        {
        }
    }
}
