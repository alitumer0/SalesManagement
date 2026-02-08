using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.EmployeeVMs;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Validators.EmployeeValidators
{
    public class EmployeeCreateValidation : AbstractValidator<EmployeeCreateVM>
    {
        private const long MaxFileSize = 500 * 1024;
        private readonly IStringLocalizer _localizer;

        public EmployeeCreateValidation(IStringLocalizer<Resource> localizer)
        {
			_localizer = localizer;

			RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(localizer[Messages.EMPLOYEE_NAMESPACE_IS_REQUIRED])
                .MaximumLength(50).WithMessage(localizer[Messages.EMPLOYEE_NAME_MAX_LENGTH_ERROR])
                .MinimumLength(2).WithMessage(localizer[Messages.EMPLOYEE_NAME_MIN_LENGTH_ERROR])
                .Matches(@"^[^\d]+$").WithMessage(localizer[Messages.EMPLOYEE_NAME_NUMERIC_FORBIDDEN]);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(localizer[Messages.EMPLOYEE_SURNAME_FIELD_IS_REQUIRED])
                .MaximumLength(50).WithMessage(localizer[Messages.EMPLOYEE_SURNAME_MAX_LENGTH_ERROR])
                .MinimumLength(2).WithMessage(localizer[Messages.EMPLOYEE_SURNAME_MIN_LENGTH_ERROR])
                .Matches(@"^[^\d]+$").WithMessage(localizer[Messages.EMPLOYEE_SURNAME_NUMERIC_FORBIDDEN]);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[Messages.EMPLOYEE_E_MAIL_FIELD_IS_REQUIRED])
                .EmailAddress().WithMessage(localizer[Messages.EMPLOYEE_VALID_EMAIL_REQUIRED])
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage(localizer[Messages.EMPLOYEE_VALID_EMAIL_REQUIRED])
                .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage(localizer[Messages.EMPLOYEE_TURKISH_CHAR])
                .Must(BeValidEmailDomain).WithMessage(localizer[localizer[Messages.EMPLOYEE_INVALID_EMAIL_DOMAIN]]);

            RuleFor(x => x.Photo)
                .Must(photo => photo == null || photo.Length <= MaxFileSize)
                .WithMessage(localizer[Messages.EMPLOYEE_PHOTO_SIZE_EXCEEDS_LIMIT])
                .Must(photo =>
                {
                    if (photo == null)
                    return true; 

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(photo.FileName)?.ToLowerInvariant(); 
                    return allowedExtensions.Contains(extension);
                })
                .WithMessage(localizer[Messages.EMPLOYEE_PHOTO_INVALID_FILE_TYPE]);


            //RuleFor(x => x.CompanyOptions)
            //    .NotEmpty().WithMessage(_localizer[Messages.EMPLOYEE_TITLE_REQUIRED]);


            //RuleFor(x => x.Role)
            //    .NotEmpty().WithMessage(_localizer[Messages.EMPLOYEE_TITLE_REQUIRED]);
        }
        private bool BeValidEmailDomain(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var domainParts = email.Split('@');
            if (domainParts.Length != 2)
                return false;

            var domain = domainParts[1];
            var domainPartsArray = domain.Split('.');

            // Domain kısmında en az iki parça olmalı (örneğin, gmail.com)
            if (domainPartsArray.Length < 2)
                return false;

            // Her domain parçasının geçerli uzunlukta olduğunu kontrol et
            foreach (var part in domainPartsArray)
            {
                //Bir domain parçasının uzunluğu en fazla 63 karakter olmalıdır. Bu, DNS yapılandırmalarında kullanılan standart bir sınırdır. 
                if (part.Length < 1 || part.Length > 63)
                    return false;
            }
            //Örneğin, gmail.com domaininde, .com kısmı TLD'dir.
            // Top-level domain (TLD) en az iki karakter uzunluğunda olmalı
            if (domainPartsArray.Last().Length < 2)
                return false;

            // Geçersiz domain adlarını reddetmek için geçerli alan adı kontrolü!
            var validDomains = new HashSet<string> { "gmail.com", "yahoo.com", "outlook.com", "bilgeadam.com", "bilgeadamboost.com", "hotmail.com" }; // Geçerli domainleri buraya ekle
            if (!validDomains.Contains(domain))
                return false;

            return true;
        }
    }
}
