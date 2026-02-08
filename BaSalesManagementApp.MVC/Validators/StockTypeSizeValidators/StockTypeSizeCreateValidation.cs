
using BaSalesManagementApp.MVC.Models.StockTypeSizeVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.StockTypeSizeValidators
{
    public class StockTypeSizeCreateValidation : AbstractValidator<StockTypeSizeCreateVM>
    {


        public StockTypeSizeCreateValidation(IStringLocalizer<Resource> localizer)
        {
            //RuleFor(x => x.CategoryId)
            //    .NotEmpty()
            //    .WithMessage(localizer[Messages.STOCK_TYPE_SIZE_CATEGORY_RELATION]);

            //RuleFor(x => x.Size)
            //    .NotEmpty()
            //    .WithMessage(localizer[Messages.STOCK_TYPE_SIZE_NOT_FOUND])
            //    .MaximumLength(128)
            //    .WithMessage(localizer["STOCK_TYPE_SIZE_MAX_LENGTH"])
            //    .MinimumLength(1)
            //    .WithMessage(localizer[Messages.STOCK_TYPE_SIZE_MIN_LENGTH]);

            //RuleFor(x => x.Description)
            //    .NotEmpty()
            //    .WithMessage(localizer[Messages.STOCK_TYPE_SIZE_NOT_FOUND])
            //    .MaximumLength(128)
            //    .WithMessage(localizer[Messages.STOCK_TYPE_SIZE_MAX_LENGTH])
            //    .MinimumLength(1)
            //    .WithMessage(localizer[Messages.STOCK_TYPE_SIZE_MIN_LENGTH]);
        }

    }
}
