using BaSalesManagementApp.MVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaSalesManagementApp.MVC.Views.Components
{
    public class MultiselectViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<SelectListItem> options, string elementId = "js-example-basic-multiple")
        {
            var model = new MultiselectViewModel
            {
                Options = options,
                ElementId = elementId
            };

            return View(model);
        }
    }
}
