using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.PaymentTypeDTOs;
using BaSalesManagementApp.Entites.DbSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSalesManagementApp.Business.Services
{
    //PaymentService ödeme tipleri ile ilgili tüm CRUD işlemleirni gerçekleştirir.
    public class PaymentTypeService : IPaymentTypeService
    {
        private readonly IPaymentTypeRepository _paymentTypeRepository;

        public PaymentTypeService(IPaymentTypeRepository paymentTypeRepository)
        {
            _paymentTypeRepository = paymentTypeRepository;
        }

        public async Task<IDataResult<PaymentTypeDTO>> AddAsync(PaymentTypeCreateDTO paymentTypeCreateDTO)
        {
            try
            {
                var paymentType = paymentTypeCreateDTO.Adapt<PaymentType>();
                await _paymentTypeRepository.AddAsync(paymentType);
                await _paymentTypeRepository.SaveChangeAsync();
                return new SuccessDataResult<PaymentTypeDTO>(paymentType.Adapt<PaymentTypeDTO>(), Messages.PAYMENT_TYPE_CREATED_SUCCESS);
            }
            catch (Exception ex)
            {

                return new ErrorDataResult<PaymentTypeDTO>(Messages.PAYMENT_TYPE_CREATE_FAILED + ex.Message);
            }


        }

        public async Task<IResult> DeleteAsync(Guid paymentTypeId)
        {
            try
            {
                var paymentType = await _paymentTypeRepository.GetByIdAsync(paymentTypeId);
                if (paymentType == null)
                {
                    return new ErrorResult(Messages.PAYMENT_TYPE_NOT_FOUND);
                }
                await _paymentTypeRepository.DeleteAsync(paymentType);
                await _paymentTypeRepository.SaveChangeAsync();
                return new SuccessResult(Messages.PAYMENT_TYPE_DELETED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorResult(Messages.PAYMENT_TYPE_DELETE_FAILED + ex.Message);

            }
        }

        public async Task<IDataResult<List<PaymentTypeListDTO>>> GetAllAsync()
        {
            try
            {
                var paymentType = await _paymentTypeRepository.GetAllAsync();
                var paymentTypeListDTOs = paymentType.Adapt<List<PaymentTypeListDTO>>() ?? new List<PaymentTypeListDTO>();
                if (paymentType == null || paymentType.Count() == 0)
                {
                    return new ErrorDataResult<List<PaymentTypeListDTO>>(paymentTypeListDTOs, Messages.PAYMENT_TYPE_LIST_EMPTY);
                }
                return new SuccessDataResult<List<PaymentTypeListDTO>>(paymentTypeListDTOs, Messages.PAYMENT_TYPE_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {

                return new ErrorDataResult<List<PaymentTypeListDTO>>(Messages.PAYMENT_TYPE_LIST_FAILED + ex.Message);
            }
        }

        public async Task<IDataResult<List<PaymentTypeListDTO>>> GetAllWithStatusDefaultActivedAsync(Status status = Status.Actived)
        {
            try
            {
                var paymentType = await _paymentTypeRepository.GetAllAsync(expression: pt => pt.Status == status);
                var paymentTypeListDTOs = paymentType.Adapt<List<PaymentTypeListDTO>>() ?? new List<PaymentTypeListDTO>();
                if (paymentType == null || paymentType.Count() == 0)
                {
                    return new ErrorDataResult<List<PaymentTypeListDTO>>(paymentTypeListDTOs, Messages.PAYMENT_TYPE_LIST_EMPTY);
                }
                return new SuccessDataResult<List<PaymentTypeListDTO>>(paymentTypeListDTOs, Messages.PAYMENT_TYPE_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {

                return new ErrorDataResult<List<PaymentTypeListDTO>>(Messages.PAYMENT_TYPE_LIST_FAILED + ex.Message);
            }
        }

        public async Task<IDataResult<List<PaymentTypeListDTO>>> GetAllAsync(string sortOrder)
        {
            try
            {

                var paymentTypes = await _paymentTypeRepository.GetAllAsync();
                var paymentTypeList = paymentTypes.Adapt<List<PaymentTypeListDTO>>();

                if (paymentTypeList == null || paymentTypeList.Count == 0)
                {
                    return new ErrorDataResult<List<PaymentTypeListDTO>>(paymentTypeList, Messages.PAYMENT_TYPE_LIST_EMPTY);
                }
                // Sıralama işlemi
                switch (sortOrder.ToLower())
                {
                    case "date":
                        paymentTypeList = paymentTypeList.OrderByDescending(c => c.CreatedDate).ToList();
                        break;
                    case "datedesc":
                        paymentTypeList = paymentTypeList.OrderBy(c => c.CreatedDate).ToList();
                        break;
                    case "alphabetical":
                        paymentTypeList = paymentTypeList.OrderBy(c => c.Name).ToList();
                        break;
                    case "alphabeticaldesc":
                        paymentTypeList = paymentTypeList.OrderByDescending(c => c.Name).ToList();
                        break;
                        // default: sıralama varsayılan olarak alfabetik
                }
                return new SuccessDataResult<List<PaymentTypeListDTO>>(paymentTypeList, Messages.PAYMENT_TYPE_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<PaymentTypeListDTO>>(Messages.PAYMENT_TYPE_LIST_FAILED + ex.Message);

            }
        }
        public async Task<IDataResult<List<PaymentTypeListDTO>>> GetAllAsync(string sortOrder, string searchQuery)
        {
            try
            {
                var paymentTypes = await _paymentTypeRepository.GetAllAsync();
                var paymentTypeList = paymentTypes.Adapt<List<PaymentTypeListDTO>>();

                // Arama işlemi 
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    paymentTypeList = paymentTypeList.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                if (paymentTypeList == null || paymentTypeList.Count == 0)
                {
                    return new ErrorDataResult<List<PaymentTypeListDTO>>(paymentTypeList, Messages.PAYMENT_TYPE_LIST_EMPTY);
                }
                // Sıralama işlemi
                switch (sortOrder.ToLower())
                {
                    case "date":
                        paymentTypeList = paymentTypeList.OrderByDescending(c => c.CreatedDate).ToList();
                        break;
                    case "datedesc":
                        paymentTypeList = paymentTypeList.OrderBy(c => c.CreatedDate).ToList();
                        break;
                    case "alphabetical":
                        paymentTypeList = paymentTypeList.OrderBy(c => c.Name).ToList();
                        break;
                    case "alphabeticaldesc":
                        paymentTypeList = paymentTypeList.OrderByDescending(c => c.Name).ToList();
                        break;
                        // default: sıralama varsayılan olarak alfabetik
                }
                return new SuccessDataResult<List<PaymentTypeListDTO>>(paymentTypeList, Messages.PAYMENT_TYPE_LISTED_SUCCESS);

            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<PaymentTypeListDTO>>(Messages.PAYMENT_TYPE_LIST_FAILED + ex.Message);
            }
        }
        public async Task<IDataResult<PaymentTypeDTO>> GetByIdAsync(Guid paymentTypeId)
        {
            try
            {
                var paymentType = await _paymentTypeRepository.GetByIdAsync(paymentTypeId);
                if (paymentType == null)
                {
                    return new ErrorDataResult<PaymentTypeDTO>(Messages.PAYMENT_TYPE_NOT_FOUND);
                }
                return new SuccessDataResult<PaymentTypeDTO>(paymentType.Adapt<PaymentTypeDTO>(), Messages.PAYMENT_TYPE_FOUND_SUCCESS);
            }
            catch (Exception ex)
            {

                return new ErrorDataResult<PaymentTypeDTO>(Messages.PAYMENT_TYPE_GET_FAILED + ex.Message);
            }
        }
        public async Task<IDataResult<PaymentTypeDTO>> UpdateAsync(PaymentTypeUpdateDTO paymentTypeUpdateDTO)
        {
            try
            {
                var oldPaymentType = await _paymentTypeRepository.GetByIdAsync(paymentTypeUpdateDTO.Id);

                if (oldPaymentType == null)
                {
                    return new ErrorDataResult<PaymentTypeDTO>(Messages.PAYMENT_TYPE_NOT_FOUND);
                }

                var updatedPaymentType = paymentTypeUpdateDTO.Adapt(oldPaymentType);

                await _paymentTypeRepository.UpdateAsync(updatedPaymentType);

                await _paymentTypeRepository.SaveChangeAsync();

                return new SuccessDataResult<PaymentTypeDTO>(updatedPaymentType.Adapt<PaymentTypeDTO>(), Messages.PAYMENT_TYPE_UPDATED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<PaymentTypeDTO>(Messages.PAYMENT_TYPE_UPDATED_FAILED + ex.Message);
            }
        }

        public async Task<IDataResult<List<PaymentTypeListDTO>>> GetPaymentTypesByCompanyIdAsync(Guid companyId)
        {
            try
            {
                var paymentTypes = await _paymentTypeRepository.GetPaymentTypesByCompanyIdAsync(companyId);
                if (!paymentTypes.Any())
                {
                    return new ErrorDataResult<List<PaymentTypeListDTO>>(new List<PaymentTypeListDTO>(), "Bu şirkete ait kategori bulunamadı.");
                }
                var productListDTOs = paymentTypes.Adapt<List<PaymentTypeListDTO>>();
                return new SuccessDataResult<List<PaymentTypeListDTO>>(productListDTOs, "Kategoriler başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<PaymentTypeListDTO>>(null, $"Hata oluştu: {ex.Message}");
            }
        }

    }
}
