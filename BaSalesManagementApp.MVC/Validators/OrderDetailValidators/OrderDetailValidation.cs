using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.OrderDetailValidators
{
    public class OrderDetailValidation : AbstractValidator<OrderDetailCreateDTO>
    {
        public OrderDetailValidation(IStringLocalizer<Resource> localizer)
        {
            RuleFor(od => od.Quantity)
                .NotEmpty().WithMessage(localizer[Messages.ORDER_EMPTY_VALIDATION])
                .GreaterThanOrEqualTo(1).WithMessage(localizer[Messages.ORDER_QUANTITY_VALIDATION]);

            RuleFor(od => od.Discount)
                .InclusiveBetween(0, 100).WithMessage(localizer[Messages.ORDER_DISCOUNT_VALIDATION]);
        }
    }
}
