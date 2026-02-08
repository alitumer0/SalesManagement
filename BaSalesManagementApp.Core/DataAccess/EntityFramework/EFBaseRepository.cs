namespace BaSalesManagementApp.Core.DataAccess.EntityFramework
{
    public class EFBaseRepository<TEntity> : IAsyncRepository, IRepository, IAsyncTransactionRepository, IAsyncUpdateableRepository<TEntity>, IAsyncDeletableRepository<TEntity>, IAsyncFindableRepository<TEntity>, IAsyncInsertableRepository<TEntity>, IAsyncOrderableRepository<TEntity>, IAsyncQueryableRepository<TEntity>, IDeletableRepository<TEntity> where TEntity : BaseEntity

    {
        private readonly IdentityDbContext<IdentityUser, IdentityRole, string> _context;

        protected readonly DbSet<TEntity> _table;
        public EFBaseRepository(IdentityDbContext<IdentityUser, IdentityRole, string> context)
        {
            _context = context;
            _table = _context.Set<TEntity>();
        }



        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var entry = await _table.AddAsync(entity);
            return entry.Entity;
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            return _table.AddRangeAsync(entities);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression = null)
        {
            return expression is null ? GetAllActives().AnyAsync() : GetAllActives().AnyAsync(expression);
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<IExecutionStrategy> CreateExecutionStrategy()
        {
            return await Task.FromResult(_context.Database.CreateExecutionStrategy());
        }

        public void Delete(TEntity entity)
        {
            _table.Remove(entity);
        }

        public Task DeleteAsync(TEntity entity)
        {
            return Task.FromResult(_table.Remove(entity));
        }



        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = true)
        {
            return await GetAllActives(tracking).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true)
        {
            return await GetAllActives(tracking).Where(expression).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, TKey>> orderby, bool orderDesc = false, bool tracking = true)
        {
            var values = GetAllActives(tracking);
            return orderDesc ? await values.OrderByDescending(orderby).ToListAsync() : await values.OrderBy(orderby).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TKey>> orderby, bool orderDesc = false, bool tracking = true)
        {
            var values = GetAllActives(tracking).Where(expression);
            return orderDesc ? await values.OrderByDescending(orderby).ToListAsync() : await values.OrderBy(orderby).ToListAsync();
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true)
        {
            return await GetAllActives(tracking).FirstOrDefaultAsync(expression);
        }

        public async Task<TEntity> GetByIdAsync(Guid id, bool tracking = true)
        {
            return await GetAllActives(tracking).FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var entry = await Task.FromResult(_table.Update(entity));
            return entry.Entity;
        }
        protected IQueryable<TEntity> GetAllActives(bool tracking = true)
        {
            var values = _table.Where(x => x.Status != Enums.Status.Deleted);
            return tracking ? values : values.AsNoTracking();
        }
    }
}
