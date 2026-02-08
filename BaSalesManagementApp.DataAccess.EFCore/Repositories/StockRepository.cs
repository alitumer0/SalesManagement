namespace BaSalesManagementApp.DataAccess.EFCore.Repositories
{
    public class StockRepository : EFBaseRepository<Stock>, IStockRepository
    {
        public StockRepository(BaSalesManagementAppDbContext context) : base(context)
        {

        }
        /// <summary>
        /// Belirli bir depoda belirli bir üründen stok olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="warehouseId">Depo ID'si</param>
        /// <param name="productId">Ürün ID'si</param>
        /// <returns>Mevcut stok nesnesi (varsa)</returns>
        public async Task<Stock?> GetStockByWarehouseAndProductAsync(Guid warehouseId, Guid productId)
        {
            return await _table.FirstOrDefaultAsync(s => s.WarehouseId == warehouseId && s.ProductId == productId);
        }
    }
}
