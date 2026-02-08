
namespace BaSalesManagementApp.Core.Utilities.Results
{
    public interface IResult
    {
        bool IsSuccess { get; }
        string Message { get; }
    }
}
