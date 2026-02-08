using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.PromotionVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.PromotionValidators
{
    public class PromotionUpdateValidation : AbstractValidator<PromotionUpdateVM>
    {


        public PromotionUpdateValidation()
        {


            RuleFor(x => x.Discount)
            .NotEmpty()
            .WithMessage(Messages.PROMOTION_DISCOUNT_REQUIRED)
            .GreaterThan(0)
            .WithMessage(Messages.PROMOTION_DISCOUNT_GREATER_THAN_ZERO)
            .LessThan(100)
            .WithMessage(Messages.PROMOTION_DISCOUNT_LESS_THAN_HUNDRED);

            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage(Messages.PROMOTION_PRICE_REQUIRED)
                .GreaterThan(0)
                .WithMessage(Messages.PROMOTION_PRICE_GREATER_THAN_ZERO);

            RuleFor(x => x.TotalPrice)
                .NotEmpty()
                .WithMessage(Messages.PROMOTION_TOTAL_PRICE_REQUIRED)
                .GreaterThan(0)
                .WithMessage(Messages.PROMOTION_TOTAL_PRICE_GREATER_THAN_ZERO)
                .GreaterThanOrEqualTo(x => x.Price)
                .WithMessage(Messages.PROMOTION_TOTAL_PRICE_GREATER_THAN_OR_EQUAL_PRICE);

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage(Messages.PROMOTION_START_DATE_REQUIRED)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage(Messages.PROMOTION_START_DATE_GREATER_THAN_TODAY)
                .LessThan(x => x.EndDate)
                .WithMessage(Messages.PROMOTION_START_DATE_LESS_THAN_END_DATE);

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage(Messages.PROMOTION_END_DATE_REQUIRED)
                .GreaterThan(x => x.StartDate)
                .WithMessage(Messages.PROMOTION_END_DATE_GREATER_THAN_START_DATE);

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage(Messages.PROMOTION_PRODUCT_RELATION);

            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage(Messages.PROMOTION_COMPANY_RELATION);
        }
    }

}
