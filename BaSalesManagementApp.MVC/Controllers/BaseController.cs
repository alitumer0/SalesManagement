using Microsoft.AspNetCore.Authorization;

namespace BaSalesManagementApp.MVC.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected INotyfService notyfService => HttpContext.RequestServices.GetService(typeof(INotyfService)) as INotyfService;

        public void NotifySuccess(string message)
        {
            notyfService.Success(message);
        }

        public void NotifyError(string message)
        {
            notyfService.Error(message);
        }

        public void NotifyInfo(string message)
        {
            notyfService.Information(message);
        }


    }
}
