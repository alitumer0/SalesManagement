namespace BaSalesManagementApp.Core.Utilities.Results
{
    public class Result : IResult
    {
        public bool IsSuccess { get; }

        public string Message { get; }
        public Result()
        {
            IsSuccess = false;
            Message = string.Empty;
        }
        public Result(bool issuccess) : this()
        {
            IsSuccess = issuccess;
        }
        public Result(bool issuccess, string messages) : this(issuccess) 
        {
                Message = messages;
        }

    }
}
