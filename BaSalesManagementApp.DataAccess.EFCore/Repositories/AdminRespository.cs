using Microsoft.EntityFrameworkCore;
using System.Globalization;
using BaSalesManagementApp.Entites.DbSets;




namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class AdminRespository: EFBaseRepository<Admin>,IAdminRepository
    {
        private readonly BaSalesManagementAppDbContext _context;
        public AdminRespository(BaSalesManagementAppDbContext context):base(context) 
        {
            _context = context;

        }

        public Task<Admin?> GetByIdentityId(string identityId)
        {
            return _table.FirstOrDefaultAsync(x => x.IdentityId == identityId);
        }
        public async Task<(List<Admin> Items, int Total)> GetPagedAsync(
           string? search, string sort, int page, int pageSize)
        {
            var q = _context.Admins.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(a =>
                    (a.Email != null && a.Email.ToLower().Contains(s)) ||
                    (a.FirstName != null && a.FirstName.ToLower().Contains(s)) ||
                    (a.LastName != null && a.LastName.ToLower().Contains(s)));

                // opsiyonel: tarih parse (TR)
                if (DateTime.TryParse(search, new System.Globalization.CultureInfo("tr-TR"),
                    System.Globalization.DateTimeStyles.None, out var dt))
                {
                    var d = dt.Date;
                    q = q.Where(a => a.CreatedDate.Date == d);
                }
            }

            q = (sort ?? "name").ToLower() switch
            {
                "namedesc" => q.OrderByDescending(a => a.FirstName),
                "createddate" => q.OrderByDescending(a => a.CreatedDate),
                "createddatedesc" => q.OrderBy(a => a.CreatedDate),
                _ => q.OrderBy(a => a.FirstName)
            };

            var total = await q.CountAsync();

            var items = await q
                .Skip(Math.Max(0, (page - 1) * pageSize))
                .Take(pageSize)
                .Select(a => new Admin
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    CreatedDate = a.CreatedDate,
                    // 💡 Fotoğrafı buradan çıkardık, çünkü tablo görünümünde gerek yok.
                    PhotoData = null,
                })

                .AsNoTracking()
                .ToListAsync();

            return (items, total);
        }

        public Task<int> CountAsync()
            => _context.Admins.AsNoTracking().CountAsync();

        public Task<int> CountNewThisMonthAsync()
        {
            var now = DateTime.Now;
            return _context.Admins.AsNoTracking()
                .CountAsync(a => a.CreatedDate.Month == now.Month && a.CreatedDate.Year == now.Year);
        }
    }
}

