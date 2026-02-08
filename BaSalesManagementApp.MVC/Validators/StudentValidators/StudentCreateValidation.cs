using BaSalesManagementApp.MVC.Models.StudentVMs;

namespace BaSalesManagementApp.MVC.Validators.StudentValidators
{
    public class StudentCreateValidation : AbstractValidator<StudentCreateVM>
    {
        public StudentCreateValidation()
        {
            RuleFor(s => s.Name).NotEmpty().WithMessage("Bu kısım boş bırakılamaz!")
                                .MaximumLength(50).WithMessage("50 karakterden fazla girilemez!")
                                .MinimumLength(3).WithMessage("En az 3 karakter girilmeli!");

            RuleFor(s => s.Age).NotEmpty().WithMessage("Bu kısım boş bırakılamaz.")
                               .NotNull().WithMessage("Sıfır girilemez!");

        }
    }
}
