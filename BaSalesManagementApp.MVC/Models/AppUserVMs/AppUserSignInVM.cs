namespace BaSalesManagementApp.MVC.Models.AppUserVMs
{
    public class AppUserSignInVM
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsPersistant { get; set; }
    }
}