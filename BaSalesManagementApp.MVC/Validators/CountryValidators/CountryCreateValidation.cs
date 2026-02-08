
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.CountryVMs;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CountryValidators
{
    public class CountryCreateValidation : AbstractValidator<CountryCreateVM>
    {

        private readonly IStringLocalizer _localizer;
        private readonly ICountryService _countryService;

        public CountryCreateValidation(IStringLocalizer<Resource> localizer, ICountryService countryService)
        {
            _countryService = countryService;

            //Türkçe isim validasyonu
            RuleFor(s => s.NameTr)
              .NotEmpty()
              .WithMessage(localizer[Messages.COUNTRY_NAME_NOT_EMPTY])
              .Matches(@"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s]+$")
              .WithMessage(localizer[Messages.COUNTRY_NAME_SHOULD_BE_STRING])
              .MaximumLength(30)
              .WithMessage(localizer[Messages.COUNTRY_NAME_MAXIMUM_LENGTH])
              .MinimumLength(2)
              .WithMessage(localizer[Messages.COUNTRY_NAME_MINIMUM_LENGTH])
              .Must(BeUniqueCountryNameTr)
              .WithMessage(localizer[Messages.COUNTRY_NAME_MUST_BE_UNIQUE]);

            // İngilizce İsim Validasyonu
            RuleFor(s => s.NameEn)
                .NotEmpty()
                .WithMessage(localizer[Messages.COUNTRY_NAME_NOT_EMPTY])
                .Matches(@"^[a-zA-Z\s]+$") // İngilizce karakterler
                .WithMessage(localizer[Messages.COUNTRY_NAME_SHOULD_BE_STRING])
                .MaximumLength(30)
                .WithMessage(localizer[Messages.COUNTRY_NAME_MAXIMUM_LENGTH])
                .MinimumLength(2)
                .WithMessage(localizer[Messages.COUNTRY_NAME_MINIMUM_LENGTH])
                .Must(BeUniqueCountryNameEn)
                .WithMessage(localizer[Messages.COUNTRY_NAME_MUST_BE_UNIQUE]);


        }

        // Türkçe kontrol
        private bool BeUniqueCountryNameTr(string nameTr)
        {
            return !_countryService.CountryExist(nameTr);
        }

        //İngilizce kontrol
        private bool BeUniqueCountryNameEn(string nameEn)
        {
            return !_countryService.CountryExist(nameEn);
        }

    }
}
