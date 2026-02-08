using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.WarehouseValidators
{
    public class WarehouseUpdateValidation : AbstractValidator<WarehouseUpdateVM>
    {

        public WarehouseUpdateValidation()
        {


            RuleFor(w => w.Name)
                 .NotEmpty()
                 .WithMessage(Messages.Warehouse_NOT_EMPTY)
                 .MaximumLength(128)
                 .WithMessage(Messages.Warehouse_MAX_LENGTH)
                 .MinimumLength(2)
                 .WithMessage(Messages.Warehouse_MIN_LENGTH);

            RuleFor(w => w.Address)
                .NotEmpty()
                .WithMessage(Messages.Warehouse_NOT_EMPTY)
                .MinimumLength(2)
                .WithMessage(Messages.Warehouse_MIN_LENGTH);
        }
    }
}
