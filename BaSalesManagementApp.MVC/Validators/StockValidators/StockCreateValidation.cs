using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.StockVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.StockValidators
{
    public class StockCreateValidation : AbstractValidator<StockCreateVM>
    {

        public StockCreateValidation(IStringLocalizer<Resource> localizer)
        {
            RuleFor(x => x.Count)
                .NotEmpty().WithMessage(localizer[Messages.STOCK_EMPTY_VALIDATION])
                .GreaterThanOrEqualTo(0).WithMessage(localizer[Messages.STOCK_POSITIVE_VALIDATION])
                .Must(x => x % 1 == 0).WithMessage(localizer[Messages.STOCK_WHOLENUMBER_VALIDATION]);

            RuleFor(x => x.ProductId)
               .NotEmpty()
               .WithMessage(localizer[Messages.STOCK_PRODUCT_RELATION]);
        }
    }
}
