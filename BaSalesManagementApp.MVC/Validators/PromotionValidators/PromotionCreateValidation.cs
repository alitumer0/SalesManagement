using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.PromotionVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.PromotionValidators
{
    public class PromotionCreateValidation : AbstractValidator<PromotionCreateVM>
    {

        private readonly IStringLocalizer _localizer;
        public PromotionCreateValidation(IStringLocalizer<Resource> localizer)
        {


            RuleFor(x => x.Discount)
            .NotEmpty()
            .WithMessage(localizer[Messages.PROMOTION_DISCOUNT_REQUIRED])
            .GreaterThan(0)
            .WithMessage(localizer[Messages.PROMOTION_DISCOUNT_GREATER_THAN_ZERO])
            .LessThan(100)
            .WithMessage(localizer[Messages.PROMOTION_DISCOUNT_LESS_THAN_HUNDRED]);

            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage(localizer[Messages.PROMOTION_PRICE_REQUIRED])
                .GreaterThan(0)
                .WithMessage(localizer[Messages.PROMOTION_PRICE_GREATER_THAN_ZERO]);

            RuleFor(x => x.TotalPrice)
                .NotEmpty()
                .WithMessage(localizer[Messages.PROMOTION_TOTAL_PRICE_REQUIRED])
                .GreaterThan(0)
                .WithMessage(localizer[Messages.PROMOTION_TOTAL_PRICE_GREATER_THAN_ZERO])
                .GreaterThanOrEqualTo(x => x.Price)
                .WithMessage(localizer[Messages.PROMOTION_TOTAL_PRICE_GREATER_THAN_OR_EQUAL_PRICE]);

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage(localizer[Messages.PROMOTION_START_DATE_REQUIRED])
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage(localizer[Messages.PROMOTION_START_DATE_GREATER_THAN_TODAY])
                .LessThan(x => x.EndDate)
                .WithMessage(localizer[Messages.PROMOTION_START_DATE_LESS_THAN_END_DATE]);

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage(localizer[Messages.PROMOTION_END_DATE_REQUIRED])
                .GreaterThan(x => x.StartDate)
                .WithMessage(localizer[Messages.PROMOTION_END_DATE_GREATER_THAN_START_DATE]);

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage(localizer[Messages.PROMOTION_PRODUCT_RELATION]);

            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage(localizer[Messages.PROMOTION_COMPANY_RELATION]);

        }
    }
}
