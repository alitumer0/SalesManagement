
using BaSalesManagementApp.MVC.Models.CustomerVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CustomerValidators
{
    public class CustomerCreateValidation : AbstractValidator<CustomerCreateVM>
    {
        private readonly IStringLocalizer _localizer;
        public CustomerCreateValidation(IStringLocalizer<Resource> localizer)
        {
            _localizer = localizer;

            RuleFor(s => s.Name)
                .NotEmpty()
                .WithMessage(localizer[Messages.CUSTOMER_NAME_NOT_EMPTY])
                .MaximumLength(128)
                .WithMessage(localizer[Messages.CUSTOMER_NAME_MAXIMUM_LENGTH])
                .MinimumLength(2)
                .WithMessage(localizer[Messages.CUSTOMER_NAME_MINIMUM_LENGTH]);

            RuleFor(s => s.Address)
                .NotEmpty()
                .WithMessage(localizer[Messages.CUSTOMER_ADDRESS_NOT_EMPTY])
                .MaximumLength(128)
                .WithMessage(localizer[Messages.CUSTOMER_ADDRESS_MAXIMUM_LENGTH])
                .MinimumLength(2)
                .WithMessage(localizer[Messages.CUSTOMER_ADDRESS_MINIMUM_LENGTH]);

            RuleFor(s => s.Email)
                .NotEmpty()
                .WithMessage(localizer[Messages.CUSTOMER_EMAIL_NOT_EMPTY])
                .EmailAddress().WithMessage(localizer[Messages.CUSTOMER_EMAIL_INVALID]);

			RuleFor(s => s.Phone)
                .NotEmpty()
                .WithMessage(localizer[Messages.CUSTOMER_PHONE_NOT_EMPTY])
                .MinimumLength(6)
                .WithMessage(localizer[Messages.CUSTOMER_PHONE_MINIMUM_LENGTH])
                .MaximumLength(40)
                .WithMessage(localizer[Messages.CUSTOMER_PHONE_MAXIMUM_LENGTH])
                .Matches(@"^\+?[1-9]\d{0,2}(\s?\d{1,4}){1,5}$")
                .WithMessage(localizer[Messages.CUSTOMER_MATCHES]);

            RuleFor(s => s.CompanyId)
                .NotEmpty()
                .WithMessage(localizer[Messages.CUSTOMER_COMPANY_RELATION]);

            RuleFor(s => s.CityId)
                .NotEmpty()
                .WithMessage(localizer[Messages.CUSTOMER_CITY_RELATION]);

            RuleFor(s => s.CountryId)
                .NotEmpty()
                .WithMessage(localizer[Messages.CUSTOMER_COUNTRY_RELATION]);
        }
    }
}
