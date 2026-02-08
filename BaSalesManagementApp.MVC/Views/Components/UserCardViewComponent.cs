using BaSalesManagementApp.MVC.Models.UserInfoVMs;
using System.Security.Claims;

namespace BaSalesManagementApp.MVC.Views.Components
{
    public class UserCardViewComponent:ViewComponent
    {
        private readonly IUserCardService _userCardService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserCardViewComponent(IUserCardService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userCardService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userDto = await _userCardService.GetUserAsync(userId);

            if (userDto == null)
            {
                return Content(string.Empty);
            }

            var userInfo = new UserInfoVM
            {
                FullName = $"{userDto.FirstName} {userDto.LastName}",
                PhotoData = userDto.PhotoData,
                Role = userDto.Role
            };

            return View(userInfo);
        }
    }
}
