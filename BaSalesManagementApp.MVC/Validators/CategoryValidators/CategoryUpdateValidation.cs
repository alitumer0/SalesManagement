
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.CategoryValidators
{
    public class CategoryUpdateValidation : AbstractValidator<CategoryUpdateVM>
    {


        public CategoryUpdateValidation()
        {

            RuleFor(w => w.Name)
                .NotEmpty()
                .WithMessage(Messages.CATEGORY_NAMESPACE_IS_REQUIRED)
                .MaximumLength(128)
                .WithMessage(Messages.CATEGORY_MAX_LENGTH)
                .MinimumLength(2)
                .WithMessage(Messages.CATEGORY_MIN_LENGTH);
        }
    }
}
