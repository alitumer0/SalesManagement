namespace BaSalesManagementApp.Core.Utilities.Results
{
    public class ErrorResult: Result
    {
        public ErrorResult() : base(false) { }
        public ErrorResult(string messages) : base(false, messages) { }
    }
}
