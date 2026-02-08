
using BaSalesManagementApp.MVC.Models.CustomerVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CustomerValidators
{
    public class CustomerUpdateValidation : AbstractValidator<CustomerUpdateVM>
    {
        private readonly IStringLocalizer _localizer;

        public CustomerUpdateValidation(IStringLocalizer<Resource> localizer)
        {
            _localizer = localizer;

            RuleFor(s => s.Name)
                .NotEmpty()
                .WithMessage(_localizer[Messages.CUSTOMER_NAME_NOT_EMPTY])
                .MaximumLength(128)
                .WithMessage(_localizer[Messages.CUSTOMER_NAME_MAXIMUM_LENGTH])
                .MinimumLength(2)
                .WithMessage(_localizer[Messages.CUSTOMER_NAME_MINIMUM_LENGTH]);

            RuleFor(s => s.Address)
                .NotEmpty()
                .WithMessage(_localizer[Messages.CUSTOMER_ADDRESS_NOT_EMPTY])
                .MaximumLength(128)
                .WithMessage(_localizer[Messages.CUSTOMER_ADDRESS_MAXIMUM_LENGTH])
                .MinimumLength(2)
                .WithMessage(_localizer[Messages.CUSTOMER_ADDRESS_MINIMUM_LENGTH]);

            RuleFor(s => s.Phone)
                .NotEmpty()
                .WithMessage(_localizer[Messages.CUSTOMER_PHONE_NOT_EMPTY])
                .MinimumLength(6)
                .WithMessage(_localizer[Messages.CUSTOMER_PHONE_MINIMUM_LENGTH])
                .MaximumLength(40)
                .WithMessage(_localizer[Messages.CUSTOMER_PHONE_MAXIMUM_LENGTH])
                .Matches(@"^\+?[1-9]\d{0,2}(\s?\d{1,4}){1,5}$")
                .WithMessage(_localizer[Messages.CUSTOMER_MATCHES]);

            RuleFor(s => s.CompanyId)
               .NotEmpty()
               .WithMessage(_localizer[Messages.CUSTOMER_COMPANY_RELATION]);

            RuleFor(s => s.CityId)
                .NotEmpty()
                .WithMessage(_localizer[Messages.CUSTOMER_CITY_RELATION]);

            RuleFor(s => s.CountryId)
                .NotEmpty()
                .WithMessage(_localizer[Messages.CUSTOMER_COUNTRY_RELATION]);
        }
    }
}
