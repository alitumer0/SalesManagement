using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.WarehouseValidators
{
    public class WarehouseCreateValidation : AbstractValidator<WarehouseCreateVM>
    {


        public WarehouseCreateValidation()
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
