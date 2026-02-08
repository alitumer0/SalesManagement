using BaSalesManagementApp.DataAccess.EFCore.Repositories;
using BaSalesManagementApp.DataAccess.Interfaces.Repositories;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.ProductTypeDtos;
using System;

namespace BaSalesManagementApp.Business.Services
{
    /// <summary>
    /// ProductTypeService sınıfı ürün tipleri ile ilgili işlemleri gerçekleştirir.
    /// </summary>
    public class ProductTypeService : IStockTypeService
    {
        private readonly IStockTypeRepository productTypeRepository;
        public ProductTypeService(IStockTypeRepository productTypeRepository)
        {
            this.productTypeRepository = productTypeRepository;
        }

        /// <summary>
        /// Yeni bir ürün tipi ekler.
        /// </summary>
        /// <param name="productTypeAddDto">Eklenmek istenen ürün tipi ile ilgili verileri içeren veri transfer nesnesi.</param>
        /// <returns>İşlem sonucunda eklenen ürün tipi verilerini geri döndürür.</returns>
        public async Task<IDataResult<StockTypeDto>> AddAsync(StockTypeAddDto productTypeAddDto)
        {
            try
            {
                var productType = productTypeAddDto.Adapt<StockType>();
                await productTypeRepository.AddAsync(productType);
                await productTypeRepository.SaveChangeAsync();
                return new SuccessDataResult<StockTypeDto>(productType.Adapt<StockTypeDto>(), Messages.PRODUCTTYPE_ADD_SUCCESS);
            }
            catch (Exception exception)
            {
                return new ErrorDataResult<StockTypeDto>(productTypeAddDto.Adapt<StockTypeDto>(), Messages.PRODUCTTYPE_ADD_UNSUCCESS + " - " + exception.Message);
            }
        }

        /// <summary>
        /// Benzer kimliği ile bir ürün tipini siler.
        /// </summary>
        /// <param name="productTypeId">Silinmek istenen ürün tipinin benzersiz kimliği.</param>
        /// <returns>İşlem sonucunda silme işleminin başarılı olup olmadığı bilgisini döner.</returns>
        public async Task<IResult> DeleteAsync(Guid productTypeId)
        {
            try
            {
                var productType = await productTypeRepository.GetByIdAsync(productTypeId);
                if (productType is null) return new ErrorResult(Messages.PRODUCTTYPE_GETBYID_UNSUCCESS);

                await productTypeRepository.DeleteAsync(productType);
                await productTypeRepository.SaveChangeAsync();
                return new SuccessResult(Messages.PRODUCTTYPE_DELETE_SUCCESS);
            }
            catch (Exception exception)
            {
                return new ErrorResult(Messages.PRODUCTTYPE_DELETE_UNSUCCESS + " - " + exception.Message);
            }
        }

        /// <summary>
        /// Bir ürün tipini günceller.
        /// </summary>
        /// <param name="productTypeUpdateDto">Güncellenmiş ürün tipi ile ilgili verileri döner.</param>
        /// <returns>İşlem sonucunda güncellenen ürün tipi verilerini döndürür.</returns>
        public async Task<IDataResult<StockTypeDto>> UpdateAsync(StockTypeUpdateDto productTypeUpdateDto)
        {
            try
            {
                var existingProductType = await productTypeRepository.GetByIdAsync(productTypeUpdateDto.Id);
                if (existingProductType is null) return new ErrorDataResult<StockTypeDto>(Messages.PRODUCTTYPE_GETBYID_UNSUCCESS);

                var updatedProductType = productTypeUpdateDto.Adapt(existingProductType);
                await productTypeRepository.UpdateAsync(updatedProductType);
                await productTypeRepository.SaveChangeAsync();
                return new SuccessDataResult<StockTypeDto>(updatedProductType.Adapt<StockTypeDto>(), Messages.PRODUCTTYPE_UPDATE_SUCCESS);
            }
            catch (Exception exception)
            {
                return new ErrorDataResult<StockTypeDto>(Messages.PRODUCTTYPE_UPDATE_UNSUCCESS + " - " + exception.Message);
            }
        }

        /// <summary>
        /// Benzersiz kimliği ile bir ürün tipini getirir.
        /// </summary>
        /// <param name="productTypeId">Getirilmek istenen ürün tipinin benzersiz kimliği.</param>
        /// <returns>İşlem sonucunda varsa eğer ürün tipini döndürür yoksa null döner.</returns>
        public async Task<IDataResult<StockTypeDto>> GetByAsync(Guid productTypeId)
        {
            try
            {
                var producType = await productTypeRepository.GetByIdAsync(productTypeId);
                if (producType is null) return new ErrorDataResult<StockTypeDto>(Messages.PRODUCTTYPE_GETBYID_UNSUCCESS);

                return new SuccessDataResult<StockTypeDto>(producType.Adapt<StockTypeDto>(), Messages.PRODUCTTYPE_GETBYID_SUCCESS);
            }
            catch (Exception exception)
            {
                return new ErrorDataResult<StockTypeDto>(Messages.PRODUCTTYPE_GETBYID_UNSUCCESS + " - " + exception.Message);
            }
        }

        /// <summary>
        /// Tüm ürün tiplerini getirir.
        /// </summary>
        /// <returns>İşlem sonucunda varsa eğer tüm ürün tiplerini getirir yoksa null döner.</returns>
        public async Task<IDataResult<IEnumerable<StockTypeListDto>>> GetAllAsync()
        {
            try
            {
                var productTypes = await productTypeRepository.GetAllAsync();
                var productTypeList = productTypes.Adapt<IEnumerable<StockTypeListDto>>() ?? new List<StockTypeListDto>();
                if (productTypeList is null || productTypeList.Count() == 0) return new ErrorDataResult<IEnumerable<StockTypeListDto>>(productTypeList, Messages.PRODUCTTYPE_LISTED_NOTFOUND);

                return new SuccessDataResult<IEnumerable<StockTypeListDto>>(productTypes.Adapt<IEnumerable<StockTypeListDto>>(), Messages.PRODUCTTYPE_LISTED_SUCCESS);
            }
            catch (Exception exception)
            {
                return new ErrorDataResult<IEnumerable<StockTypeListDto>>(new List<StockTypeListDto>(), Messages.PRODUCTTYPE_LISTED_UNSUCCESS + " - " + exception.Message);
            }
        }

        public async Task<IDataResult<IEnumerable<StockTypeListDto>>> GetAllAsync(string sortOrder, string searchQuery = "")
        {
            try
            {
                var productTypes = await productTypeRepository.GetAllAsync();

                // Arama sorgusuna göre filtreleme
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    productTypes = productTypes.Where(pt => pt.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                var productTypeList = productTypes.Adapt<IEnumerable<StockTypeListDto>>() ?? new List<StockTypeListDto>();

                if (!productTypeList.Any())
                    return new ErrorDataResult<IEnumerable<StockTypeListDto>>(productTypeList, Messages.PRODUCTTYPE_LISTED_NOTFOUND);

                return new SuccessDataResult<IEnumerable<StockTypeListDto>>(productTypeList, Messages.PRODUCTTYPE_LISTED_SUCCESS);
            }
            catch (Exception exception)
            {
                return new ErrorDataResult<IEnumerable<StockTypeListDto>>(new List<StockTypeListDto>(), Messages.PRODUCTTYPE_LISTED_UNSUCCESS + " - " + exception.Message);
            }
        }

        public async Task<IDataResult<IEnumerable<StockTypeListDto>>> GetAllAsyncProductType(string sortOrder)
        {
            try
            {
                var products = await productTypeRepository.GetAllAsync();
                var productListDTOs = products.Adapt<List<StockTypeListDto>>();
                if (productListDTOs == null || productListDTOs.Count == 0)
                {
                    return new ErrorDataResult<IEnumerable<StockTypeListDto>>(productListDTOs, Messages.PRODUCTTYPE_LISTED_NOTFOUND);
                }
                switch (sortOrder.ToLower())
                {
                    case "date":
                        productListDTOs = productListDTOs.OrderByDescending(c => c.CreatedDate).ToList();
                        break;
                    case "datedesc":
                        productListDTOs = productListDTOs.OrderBy(c => c.CreatedDate).ToList();
                        break;
                    case "alphabetical":
                        productListDTOs = productListDTOs.OrderBy(c => c.Name).ToList();
                        break;
                    case "alphabeticaldesc":
                        productListDTOs = productListDTOs.OrderByDescending(c => c.Name).ToList();
                        break;
                }
                return new SuccessDataResult<IEnumerable<StockTypeListDto>>(productListDTOs, Messages.PRODUCTTYPE_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<IEnumerable<StockTypeListDto>>(new List<StockTypeListDto>(), Messages.PRODUCTTYPE_LISTED_UNSUCCESS);
            }
        }

        public async Task<IDataResult<IEnumerable<StockTypeListDto>>> GetAllAsync(string sortOrder)
        {
            try
            {
                var productTypes = await productTypeRepository.GetAllAsync();
                var productTypeList = productTypes.Adapt<IEnumerable<StockTypeListDto>>() ?? new List<StockTypeListDto>();
                if (productTypeList is null || !productTypeList.Any())
                {
                    return new ErrorDataResult<IEnumerable<StockTypeListDto>>(productTypeList, Messages.PRODUCTTYPE_LISTED_NOTFOUND);
                }



                switch (sortOrder.ToLower())
                {
                    case "date":
                        productTypeList = productTypeList.OrderByDescending(c => c.CreatedDate).ToList();
                        break;
                    case "datedesc":
                        productTypeList = productTypeList.OrderBy(c => c.CreatedDate).ToList();
                        break;
                    case "alphabetical":
                        productTypeList = productTypeList.OrderBy(c => c.Name).ToList();
                        break;
                    case "alphabeticaldesc":
                        productTypeList = productTypeList.OrderByDescending(c => c.Name).ToList();
                        break;

                }

                return new SuccessDataResult<IEnumerable<StockTypeListDto>>(productTypeList, Messages.PRODUCTTYPE_LISTED_SUCCESS);
            }
            catch (Exception exception)
            {
                return new ErrorDataResult<IEnumerable<StockTypeListDto>>(new List<StockTypeListDto>(), Messages.PRODUCTTYPE_LISTED_UNSUCCESS + " - " + exception.Message);
            }
        }

        /// <summary>
        /// Gelen kategori listesindeki kategorilerin ürün tiplerini verir.
        /// </summary>
        /// <param name="categories"></param>
        /// <returns> Asenkron işlemi temsil eden bir görev. Görev sonucunda ürün tipi listesini döndürür. </returns>
        public async Task<List<StockType>> GetProductTypeListByCategoryListAsync(List<Category> categories)
        {
            var productTypes = await productTypeRepository.GetAllAsync();

            var categoryIds = categories.Select(c => c.Id).ToList();

            return productTypes
                .Where(productType => categoryIds.Contains(productType.CategoryId))
                .ToList();
        }

        public async Task<IDataResult<List<StockTypeDto>>> GetProductTypeListByCategoryListAsyncs(List<Category> categories)
        {
            try
            {
                var productTypes = await productTypeRepository.GetAllAsync();
                var categoryIds = categories.Select(c => c.Id).ToList();
                var stockTypeDtos = productTypes
                    .Where(productType => categoryIds.Contains(productType.CategoryId))
                    .Select(productType => new StockTypeDto
                    {
                        Id = productType.Id,
                        Name = productType.Name,
                        CategoryId = productType.CategoryId,//Dtoya categoryId propertysi eklendi.
                    })
                    .ToList();
                if (stockTypeDtos.Count == 0)
                {
                    return new ErrorDataResult<List<StockTypeDto>>("Verilen kategoriler için ürün tipi bulunamadı.");
                }
                return new SuccessDataResult<List<StockTypeDto>>(stockTypeDtos, "Ürün tipleri başarıyla alındı.");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<StockTypeDto>>($"Bir hata oluştu: {ex.Message}");
            }
        }


        public async Task<bool> IsStockTypeExistsAsync(string name)
        {
            var stockType = await productTypeRepository.GetAsync(p => p.Name == name);
            return stockType != null;
        }
    }

}