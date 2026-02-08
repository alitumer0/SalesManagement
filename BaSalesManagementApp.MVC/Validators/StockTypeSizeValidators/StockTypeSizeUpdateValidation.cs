
using BaSalesManagementApp.MVC.Models.StockTypeSizeVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.StockTypeSizeValidators
{
    public class StockTypeSizeUpdateValidation : AbstractValidator<StockTypeSizeCreateVM>
    {

        public StockTypeSizeUpdateValidation()
        {


            //RuleFor(w => w.Size)
            //    .NotEmpty()
            //    .WithMessage(Messages.STOCK_TYPE_SIZE_NOT_EMPTY)
            //    .MaximumLength(128)
            //    .WithMessage(Messages.STOCK_TYPE_SIZE_MAX_LENGTH)
            //    .MinimumLength(2)
            //    .WithMessage(Messages.STOCK_TYPE_SIZE_MIN_LENGTH);

            //RuleFor(w => w.Description)
            //    .NotEmpty()
            //    .WithMessage(Messages.STOCK_TYPE_SIZE_DESCRIPTION_NOT_EMPTY)
            //    .MaximumLength(128)
            //    .WithMessage(Messages.STOCK_TYPE_SIZE_DESCRIPTION_MAX_LENGTH)
            //    .MinimumLength(2)
            //    .WithMessage(Messages.STOCK_TYPE_SIZE_DESCRIPTION_MIN_LENGTH);
        }

    }
}
