using BaSalesManagementApp.Business;
using BaSalesManagementApp.Business.Constants;
using BaSalesManagementApp.Business.Services;
using BaSalesManagementApp.Dtos.MyProfileDTO;
using BaSalesManagementApp.MVC.Models.AppUserVMs;
using BaSalesManagementApp.MVC.Models.ChangePasswordVMs;
using BaSalesManagementApp.MVC.Models.UserVMs;
using BaSalesManagementApp.Business.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ZXing.QrCode.Internal;
using Elfie.Serialization;
using BaSalesManagementApp.Entites.DbSets;


namespace BaSalesManagementApp.MVC.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService accountService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RecaptchaService _recaptchaService;
        private readonly RecaptchaSettings _recaptchaSettings;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public AccountController(IAccountService accountService, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RecaptchaService recaptchaService, IOptions<RecaptchaSettings> recaptchaSettings, IStringLocalizer<Resource> stringLocalizer)
        {
            this.accountService = accountService;
            _userManager = userManager;
            _signInManager = signInManager;
            _recaptchaService = recaptchaService;
            _recaptchaSettings = recaptchaSettings.Value;
            _stringLocalizer = stringLocalizer;
        }
        
        public IActionResult SignIn()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(AppUserSignInVM appUserSignInVM)
        {
            
            if (!ModelState.IsValid) return View(appUserSignInVM);

            
            var appUser = await accountService.FindByEmailAsync(appUserSignInVM.Email);
            if (appUser is null)
            {
                NotifyError(_stringLocalizer[Messages.ACCOUNT_NOT_FOUND]);
                return View(appUserSignInVM);
            }

            
            var checkPassword = await _signInManager.PasswordSignInAsync(appUser, appUserSignInVM.Password, appUserSignInVM.IsPersistant, false);
            if (!checkPassword.Succeeded)
            {
                NotifyError(_stringLocalizer[Messages.ACCOUNT_NOT_FOUND]);
                return View(appUserSignInVM);
            }

            
            var roles = await accountService.GetRolesAsync(appUser);
            if (roles == null || !roles.Any())
            {
                NotifyError(_stringLocalizer[Messages.ACCOUNT_NOT_FOUND]);
                return View(appUserSignInVM);
            }

            
            if (roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "AdminDashboard");
            }
            else if (roles.Contains("Employee"))
            {
                return RedirectToAction("Index", "EmployeeDashboard");
            }
            else
            {
                NotifyError(_stringLocalizer[Messages.ACCOUNT_NOT_FOUND]);
                return View(appUserSignInVM);
            }
        }



        public async Task<IActionResult> SignOut()
        {
            await accountService.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }

       
        public ActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> MyProfile()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileDTO = await accountService.GetProfileAsync(userId);
            if (profileDTO == null)
                return NotFound();

            var profileVM = profileDTO.Adapt<MyProfileVM>();
            return View(profileVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(MyProfileVM model)
        {
            if (!ModelState.IsValid)
                return View(model);


            model.FirstName = StringUtilities.CapitalizeEachWord(model.FirstName);
            model.LastName = StringUtilities.CapitalizeFirstLetter(model.LastName);

            // Fotoğraf kaldırılmışsa
            if (model.RemovePhoto)
            {
                model.Photo = null;
                model.PhotoData = null;
            }
            else if (model.Photo != null)
            {
                // Yeni fotoğraf varsa, byte dizisine dönüştür
                model.PhotoData = await ConvertFormFileToByteArrayAsync(model.Photo);
            }
            else if (model.PhotoData == null)
            {
                // Fotoğraf eklenmemişse, mevcut fotoğrafı kullan
                if (model.PhotoData == null && model.Photo == null)
                {
                    ModelState.AddModelError("", _stringLocalizer["No_photo_selected_and_no_existing_photo_found"]);
                    return View(model);
                }
            }

            var profileDTO = model.Adapt<MyProfileDTO>();
            var result = await accountService.UpdateProfileAsync(profileDTO);
            if (result == null)
            {
                ModelState.AddModelError("", _stringLocalizer["An_error_occurred_while_updating_the_profile"]);
                return View(model);
            }

            TempData["SuccessMessage"] = _stringLocalizer["Profile_updated_successfully"];
            return RedirectToAction("MyProfile");
        }

        public async Task<byte[]> ConvertFormFileToByteArrayAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Array.Empty<byte>(); // Boş bir byte dizisi döndür
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        //İlk Kullanıcı Parola değiştirme Action'ları.
        [HttpGet]
        public async Task<IActionResult> ChangePassword(string token)
        {
            var model = new ChangePasswordVM { Token = token };
            ViewData["RecaptchaSiteKey"] = _recaptchaSettings.SiteKey;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            ViewData["RecaptchaSiteKey"] = _recaptchaSettings.SiteKey;
            if (ModelState.IsValid)
            {
                var recaptchaResponse = Request.Form["g-recaptcha-response"];
                if (await _recaptchaService.Validate(recaptchaResponse))
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        NotifyError(_stringLocalizer[Messages.ACCOUNT_NOT_FOUND]);
                        return View(model);
                    }

                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

                    if (result.Succeeded)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                        await _signInManager.RefreshSignInAsync(user);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    NotifyError(_stringLocalizer[Messages.RECAPTCHA_FAILED_ERROR]);
                }
            }

            return View(model);
        }


        //MyProfil sayfasında şifre değiştirme Action'ı

        [HttpGet]
        public IActionResult ChangeMyPassword()
        {
            var model = new ChangeMyPasswordVM();
            ViewData["RecaptchaSiteKey"] = _recaptchaSettings.SiteKey;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeMyPassword(ChangeMyPasswordVM model)
        {
            ViewData["RecaptchaSiteKey"] = _recaptchaSettings.SiteKey;
            if (ModelState.IsValid)
            {
                var recaptchaResponse = Request.Form["g-recaptcha-response"];
                if (await _recaptchaService.Validate(recaptchaResponse))
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        NotifyError(_stringLocalizer[Messages.ACCOUNT_NOT_FOUND]);
                        return View(model);
                    }

                    var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.ConfirmNewPassword);

                    if (result.Succeeded)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                        await _signInManager.RefreshSignInAsync(user);
                        NotifySuccess(_stringLocalizer[Messages.CHANGE_PASSWORD_SUCCESS]);

                        return RedirectToAction("MyProfile");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        NotifyError(_stringLocalizer[Messages.CHANGE_PASSWORD_ERROR]);
                    }
                }
                else
                {
                    NotifyError(_stringLocalizer[Messages.RECAPTCHA_FAILED_ERROR]);
                }
            }

            return View(model);
        }

    }
}


//kontrol