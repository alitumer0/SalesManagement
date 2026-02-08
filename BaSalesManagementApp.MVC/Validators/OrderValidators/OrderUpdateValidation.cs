using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.OrderVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.OrderValidators
{
    public class OrderUpdateValidation : AbstractValidator<OrderUpdateVM>
    {

        public OrderUpdateValidation()
        {


         

            RuleFor(s => s.TotalPrice)
                .NotEmpty().WithMessage(Messages.ORDER_EMPTY_VALIDATION)
                .GreaterThan(0).WithMessage(Messages.ORDER_TOTALPRICE_VALIDATION)
                .ScalePrecision(2, 18).WithMessage(Messages.ORDER_PRECISION_VALIDATION);

            RuleFor(s => s.OrderDate)
                .NotEmpty().WithMessage(Messages.ORDER_EMPTY_VALIDATION)
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage(Messages.ORDER_DATE_VALIDATION);

            
        }
    }
}
