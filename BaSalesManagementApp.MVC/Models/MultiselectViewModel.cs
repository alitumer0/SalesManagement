using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaSalesManagementApp.MVC.Models
{
    public class MultiselectViewModel
    {
        public List<SelectListItem> Options { get; set; }
        public string ElementId { get; set; }
    }
}
