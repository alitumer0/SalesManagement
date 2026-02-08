
using BaSalesManagementApp.MVC.Models.CityVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CityValidators
{
    public class CityCreateValidation : AbstractValidator<CityCreateVM>
    {
        private readonly IStringLocalizer _localizer;
        private readonly ICityService _cityService;
        public CityCreateValidation(IStringLocalizer<Resource> localizer, ICityService cityService)
        {

            this._cityService = cityService;
            RuleFor(s => s.Name)
              .NotEmpty()
              .WithMessage(localizer[Messages.CITY_NAME_CANNOT_BE_EMPTY])
              .Matches(@"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s]+$")
              .WithMessage(localizer[Messages.CITY_NAME_CANNOT_CONTAIN_NUMBER])
              .Must(BeUniqueCityName)
              .WithMessage(localizer[Messages.CITY_NAME_MUST_BE_UNIQUE]);
            RuleFor(s => s.CountryId)
                .NotEmpty().WithMessage(localizer[Messages.COUNTRY_NAME_NOT_EMPTY]);
        }

        private bool BeUniqueCityName(string cityName)
        {
            return !_cityService.CityExist(cityName);
        }
    }



}
