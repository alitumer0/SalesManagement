using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BaSalesManagementApp.DataAccess.Context
{
    public class BaSalesManagementAppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BaSalesManagementAppDbContext(DbContextOptions<BaSalesManagementAppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public BaSalesManagementAppDbContext()
        {
        }

        public const string ConnectionName = "ProjectConnectionString";


        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Badge> Badges { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<StockType> StockTypes { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<Queue> Queue { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<StockTypeSize> StockTypeSizes { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<CurrentExchangeRate> CurrentExchangeRates { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(IEntityConfiguration).Assembly);


        }


        public override int SaveChanges()
        {

            SetBaseProperties();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetBaseProperties();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetBaseProperties()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            // HttpContext'in null olup olmadığını kontrol ediliyor.
            // Çünkü,Zamanlanmış görevlerde (BackgroundService, Hangfire gibi) HttpContext erişimi olmaz.
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Kullanıcı Bulunamadı";

            foreach (var entry in entries)
            {
                SetIfAdded(entry, userId);
                SetIfModified(entry, userId);
                SetIfDeleted(entry, userId);

            }
        }

        private void SetIfAdded(EntityEntry<BaseEntity> entry, string userId)
        {
            if (entry.State is not EntityState.Added)
            {
                return;
            }
            entry.Entity.CreatedDate = DateTime.Now;
            entry.Entity.CreatedBy = userId;
            entry.Entity.Status = Core.Enums.Status.Added;
        }

        private void SetIfDeleted(EntityEntry<BaseEntity> entry, string userId)
        {
            if (entry.State is not EntityState.Deleted)
            {
                return;
            }
            if (entry.Entity is not AuditableEntity entity)
            {
                return;
            }

            entry.State = EntityState.Modified;
            entity.DeletedDate = DateTime.Now;
            entity.DeletedBy = userId;
            entity.Status = Core.Enums.Status.Deleted;

        }

        private void SetIfModified(EntityEntry<BaseEntity> entry, string userId)
        {
            if (entry.State is not EntityState.Modified)
            {
                return;
            }

            // Eğer Status alanı Passive olarak ayarlanmışsa, ModifiedDate ve ModifiedBy güncellenmeli
            if (entry.Entity.Status is Core.Enums.Status.Passive or Core.Enums.Status.Actived)
            {
                entry.Entity.ModifiedDate = DateTime.Now;
                entry.Entity.ModifiedBy = userId;
                return;
            }

            // Diğer durumlarda normal olarak Modified işlemlerini yap
            entry.Entity.ModifiedDate = DateTime.Now;
            entry.Entity.ModifiedBy = userId;
            entry.Entity.Status = Core.Enums.Status.Modified;
        }
    }
}