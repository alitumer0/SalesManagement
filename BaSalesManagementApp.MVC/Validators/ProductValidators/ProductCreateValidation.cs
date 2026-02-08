using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.ProductVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.ProductValidators
{
    public class ProductCreateValidation : AbstractValidator<ProductCreateVM>
    {


        public ProductCreateValidation(IStringLocalizer<Resource> localizer)
        {


            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizer[Messages.PRODUCT_NAME_CANNOT_BE_EMPTY]);
            // .Matches(@"^[a-zA-Z\s]*$")
            //  .WithMessage(_localizer[Messages.PRODUCT_NAME_CANNOT_CONTAIN_NUMBER]);

            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage(localizer[Messages.PRODUCT_PRICE_CANNOT_BE_EMPTY])
                .GreaterThan(0)
                .WithMessage(localizer[Messages.PRODUCT_PRICE_MUST_BE_GREATER_THAN_ZERO])
                .ScalePrecision(2, 18)
                .WithMessage(localizer[Messages.PRODUCT_PRICE_MUST_HAVE_NO_MORE_THAN_TWO_DECIMAL_PLACES]);

            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage(localizer[Messages.PRODUCT_COMPANY_RELATION]);

            RuleFor(x => x.CategoryId)
               .NotEmpty()
               .WithMessage(localizer[Messages.PRODUCT_CATEGORY_RELATION]);
        }
    }
}
