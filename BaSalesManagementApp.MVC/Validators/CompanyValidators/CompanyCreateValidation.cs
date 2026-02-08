using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;

namespace BaSalesManagementApp.MVC.Validators.CompanyValidators
{
    public class CompanyCreateValidation : AbstractValidator<CompanyCreateVM>
    {


        //public CompanyCreateValidation()
        //{


        //    RuleFor(s => s.Name)
        //        .NotEmpty()
        //        .WithMessage(Messages.COMPANY_NAME_NOT_EMPTY)
        //        .MaximumLength(128)
        //        .WithMessage(Messages.COMPANY_NAME_MAXIMUM_LENGTH)
        //        .MinimumLength(2)
        //        .WithMessage(Messages.COMPANY_NAME_MINIMUM_LENGTH);

        //    RuleFor(s => s.Address)
        //        .NotEmpty()
        //        .WithMessage(Messages.COMPANY_ADDRESS_NOT_EMPTY)
        //        .MaximumLength(128)
        //        .WithMessage(Messages.COMPANY_ADDRESS_MAXIMUM_LENGTH)
        //        .MinimumLength(2)
        //        .WithMessage(Messages.COMPANY_ADDRESS_MINIMUM_LENGTH);

        //    RuleFor(s => s.Phone)
        //        .NotEmpty()
        //        .WithMessage(Messages.COMPANY_PHONE_NOT_EMPTY)
        //        .MinimumLength(10)
        //        .WithMessage(Messages.COMPANY_PHONE_MINIMUM_LENGTH)
        //        .MaximumLength(40)
        //        .WithMessage(Messages.COMPANY_PHONE_MAXIMUM_LENGTH)
        //        .Matches(@"^\+?[0-9]+$")
        //        .WithMessage(Messages.COMPANY_MATCHES);
        //}



        public CompanyCreateValidation(IStringLocalizer<Resource> localizer)
        {
            // Şirket adı zorunlu ve maksimum 100 karakter
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer["Please enter the company name."])
                .Length(1, 100).WithMessage(localizer["The company name can be at most 100 characters long."]);

            // Adres zorunlu ve maksimum 200 karakter
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage(localizer["Please enter an address."])
                .MaximumLength(200).WithMessage(localizer["The address can be at most 200 characters long."]);

            // Ülke kodu boş olamaz (ISO 2 country code kullanımı)
            RuleFor(x => x.CountryCode)
                .NotEmpty().WithMessage(localizer["Please select a country."]);

            // Şehir ID'si zorunlu
            RuleFor(x => x.CityID)
                .NotNull().WithMessage(localizer["Please select a city."]);

            // Telefon numarası zorunlu ve ülkeye göre doğrulama yapılacak
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage(localizer["Please enter a phone number."])
                .Must((model, phone) => IsPhoneValidForCountry(model.CountryCode, phone))
                .WithMessage(localizer["The phone number format is not valid for the selected country."]);
        }

        // Ülke koduna göre telefon numarasının doğruluğunu kontrol eden fonksiyon
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
