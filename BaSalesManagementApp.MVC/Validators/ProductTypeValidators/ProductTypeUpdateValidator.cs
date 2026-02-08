using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.ProductTypeVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.ProductTypeValidators
{
    public class ProductTypeUpdateValidator : AbstractValidator<ProductTypeUpdateVM>
    {

        public ProductTypeUpdateValidator()
        {


            RuleFor(productType => productType.Name)
                .NotEmpty().WithMessage(Messages.PRODUCTTYPE_NAME_NOT_EMPTY)
                .MaximumLength(128).WithMessage(Messages.PRODUCTTYPE_NAME_MAXIMUM_LENGTH)
                .MinimumLength(2).WithMessage(Messages.PRODUCTTYPE_NAME_MINIMUM_LENGTH);

            RuleFor(productType => productType.Description)
                .NotEmpty().WithMessage(Messages.PRODUCTTYPE_DESCRIPTION_NOT_EMPTY)
                .MaximumLength(128).WithMessage(Messages.PRODUCTTYPE_DESCRIPTION_MAXIMUM_LENGTH)
                .MinimumLength(2).WithMessage(Messages.PRODUCTTYPE_DESCRIPTION_MINIMUM_LENGTH);
        }
    }
}