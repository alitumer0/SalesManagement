using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.AppUserVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.AppUserValidators
{
    public class AppUserSignInValidator : AbstractValidator<AppUserSignInVM>
    {

        public AppUserSignInValidator()
        {


            RuleFor(appUser => appUser.Email).NotEmpty().WithMessage(Messages.PLEASE_ENTER_YOUR_EMAIL)
                .EmailAddress().WithMessage(Messages.PLEASE_ENTER_YOUR_EMAIL_WITH_CORRECT_FORMAT);

            RuleFor(appUser => appUser.Password).NotEmpty().WithMessage(Messages.PLEASE_ENTER_YOUR_PASSWORD);
        }
    }
}