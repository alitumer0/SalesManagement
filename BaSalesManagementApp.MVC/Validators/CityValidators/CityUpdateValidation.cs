
using BaSalesManagementApp.MVC.Models.CityVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CityValidators
{
    public class CityUpdateValidation : AbstractValidator<CityUpdateVM>
    {

        private readonly IStringLocalizer _localizer;
        public CityUpdateValidation(IStringLocalizer<Resource> localizer)
        {
            _localizer = localizer;

            RuleFor(s => s.Name)
              .NotEmpty()
              .WithMessage(localizer[Messages.CITY_NAME_CANNOT_BE_EMPTY])
              .Matches(@"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s]+$")
              .WithMessage(localizer[Messages.CITY_NAME_CANNOT_CONTAIN_NUMBER]);
        }
    }
}
