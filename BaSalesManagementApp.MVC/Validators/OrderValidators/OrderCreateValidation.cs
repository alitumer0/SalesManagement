using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.OrderVMs;
using BaSalesManagementApp.MVC.Validators.OrderDetailValidators;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.OrderValidators
{
    public class OrderCreateValidation : AbstractValidator<OrderCreateVM>
    {
        private readonly IStringLocalizer _localizer;

        public OrderCreateValidation(IStringLocalizer<Resource> localizer)
        {


            //RuleFor(s => s.Quantity)
            //    .NotEmpty().WithMessage(localizer[Messages.ORDER_EMPTY_VALIDATION])
            //    .GreaterThanOrEqualTo(1).WithMessage(localizer[Messages.ORDER_QUANTITY_VALIDATION]);

            //RuleFor(s => s.TotalPrice)
            //    .NotEmpty().WithMessage(localizer[Messages.ORDER_EMPTY_VALIDATION])
            //    .GreaterThan(0).WithMessage(localizer[Messages.ORDER_TOTALPRICE_VALIDATION])
            //    .ScalePrecision(2, 18).WithMessage(localizer[Messages.ORDER_PRECISION_VALIDATION]);

            RuleFor(s => s.OrderDate)
                .NotEmpty().WithMessage(localizer[Messages.ORDER_EMPTY_VALIDATION])
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage(localizer[Messages.ORDER_DATE_VALIDATION]);

            //RuleFor(s => s.DiscountRate)
            //.NotEmpty().WithMessage(localizer[Messages.ORDER_EMPTY_VALIDATION])
            //.InclusiveBetween(0, 100).WithMessage(localizer[Messages.ORDER_DISCOUNT_VALIDATION]);

            // OrderDetails listesindeki her bir OrderDetailCreateDTO'yu kontrol etmek için
            RuleForEach(s => s.OrderDetails).SetValidator(new OrderDetailValidation(localizer));
        }
    }

}

