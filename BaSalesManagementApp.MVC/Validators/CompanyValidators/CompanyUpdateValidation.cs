
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CompanyValidators
{
    public class CompanyUpdateValidation : AbstractValidator<CompanyUpdateVM>
    {
        private readonly IStringLocalizer _localizer;
        public CompanyUpdateValidation(IStringLocalizer<Resource> localizer)
        {


            RuleFor(s => s.Name)
                .NotEmpty()
                .WithMessage(Messages.COMPANY_NAME_NOT_EMPTY)
                .MaximumLength(128)
                .WithMessage(Messages.COMPANY_NAME_MAXIMUM_LENGTH)
                .MinimumLength(2)
                .WithMessage(Messages.COMPANY_NAME_MINIMUM_LENGTH);

            RuleFor(s => s.Address)
                .NotEmpty()
                .WithMessage(Messages.COMPANY_ADDRESS_NOT_EMPTY)
                .MaximumLength(128)
                .WithMessage(Messages.COMPANY_ADDRESS_MAXIMUM_LENGTH)
                .MinimumLength(2)
                .WithMessage(Messages.COMPANY_ADDRESS_MINIMUM_LENGTH);

            RuleFor(s => s.Phone)
                .NotEmpty()
                .WithMessage(Messages.COMPANY_PHONE_NOT_EMPTY)
                .MinimumLength(10)
                .WithMessage(Messages.COMPANY_PHONE_MINIMUM_LENGTH)
                .MaximumLength(40)
                .WithMessage(Messages.COMPANY_PHONE_MAXIMUM_LENGTH)
                .Matches(@"^\+?[0-9\s()-]+$") 
                .WithMessage(Messages.COMPANY_MATCHES);

            RuleFor(s => s.CityId)
               .NotEmpty()
               .WithMessage(localizer[Messages.CUSTOMER_CITY_RELATION]);

            RuleFor(s => s.CountryId)
                .NotEmpty()
                .WithMessage(localizer[Messages.CUSTOMER_COUNTRY_RELATION]);

        }
        private bool IsPhoneValidForCountry(string? countryCode, string? phone)
        {
            if (string.IsNullOrEmpty(countryCode) || string.IsNullOrEmpty(phone))
                return false;

            // Farklı ülke kodları için telefon numarası doğrulama
            switch (countryCode.ToUpper())
            {
                case "TR": // Türkiye için
                    return phone.Length == 10 && phone.StartsWith("5");
                case "US": // ABD için
                    return phone.Length == 10; // ABD'de de 10 haneli telefon numarası
                case "GB": // Birleşik Krallık için
                    return phone.Length == 11 && phone.StartsWith("7");
                default:
                    return false; // Diğer ülkeler için varsayılan validasyon
            }
        }
    }
}
