
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.CountryVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CountryValidators
{
    public class CountryUpdateValidation : AbstractValidator<CountryUpdateVM>
    {
        private readonly IStringLocalizer _localizer;


        public CountryUpdateValidation(IStringLocalizer<Resource> localizer)
        {
            _localizer = localizer;

            // Türkçe İsim Validasyonu
            RuleFor(s => s.NameTr)
              .NotEmpty()
              .WithMessage(@localizer[Messages.COUNTRY_NAME_NOT_EMPTY])
              .Matches(@"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s]+$") // Türkçe karakterler
              .WithMessage(@localizer[Messages.COUNTRY_NAME_SHOULD_BE_STRING])
              .MaximumLength(40)
              .WithMessage(@localizer[Messages.COUNTRY_NAME_MAXIMUM_LENGTH])
              .MinimumLength(2)
              .WithMessage(@localizer[Messages.COUNTRY_NAME_MINIMUM_LENGTH]);

            // İngilizce İsim Validasyonu
            RuleFor(s => s.NameEn)
              .NotEmpty()
              .WithMessage(@localizer[Messages.COUNTRY_NAME_NOT_EMPTY])
              .Matches(@"^[a-zA-Z\s]+$") // İngilizce karakterler
              .WithMessage(@localizer[Messages.COUNTRY_NAME_SHOULD_BE_STRING])
              .MaximumLength(40)
              .WithMessage(@localizer[Messages.COUNTRY_NAME_MAXIMUM_LENGTH])
              .MinimumLength(2)
              .WithMessage(@localizer[Messages.COUNTRY_NAME_MINIMUM_LENGTH]);
        }
    }
}
