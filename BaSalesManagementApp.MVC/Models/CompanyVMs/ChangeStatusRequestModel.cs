namespace BaSalesManagementApp.MVC.Models.CompanyVMs;

public class ChangeStatusRequestModel
{
    public Guid CompanyId { get; set; }
    public string Status { get; set; }
}
