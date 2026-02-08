using BaSalesManagementApp.Core.Enums;
using BaSalesManagementApp.Core.Utilities.Results;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : EFBaseRepository<Product>, IProductRepository
{
    private readonly BaSalesManagementAppDbContext _dbContext;

    public ProductRepository(BaSalesManagementAppDbContext context) : base(context)
    {
        _dbContext = context;
    }
    public async Task<IResult> SetProductToPassiveAsync(Guid productId)
    {
        var product = await GetByIdAsync(productId);
        if (product == null)
        {
            return new ErrorResult("Product not found");
        }

        // Sadece Status alanını güncelle
        product.Status = Status.Passive;

        // Ürünü güncellerken sadece Status alanının güncellenmesini sağlamak için:
        var entry = _dbContext.Entry(product);
        entry.Property(p => p.Status).IsModified = true;

        // Diğer tüm alanları değiştirilmemiş (Unchanged) olarak işaretle
        foreach (var property in entry.Properties)
        {
            if (property.Metadata.Name != nameof(product.Status))
            {
                property.IsModified = false;
            }
        }

        await _dbContext.SaveChangesAsync();
        return new SuccessResult("Product status set to passive successfully");
    }
    public async Task<List<Product>> GetProductsByCompanyIdAsync(Guid companyId)
    {
        var products = await _dbContext.Products
            .Where(p => p.CompanyId == companyId)
            .ToListAsync();

        return products;
    }

    public async Task<Product> GetByNameAsync(string productName)
    {
        return await _dbContext.Products
                               .FirstOrDefaultAsync(p => p.Name == productName);
    }

   

}
