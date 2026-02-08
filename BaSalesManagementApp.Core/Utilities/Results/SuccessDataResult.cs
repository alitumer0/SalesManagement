namespace BaSalesManagementApp.Core.Utilities.Results
{
    public class SuccessDataResult<T> : DataResult<T>
    {
        public SuccessDataResult() : base(default, true) { }
        public SuccessDataResult(string messages) : base(default, true, messages) { }
        public SuccessDataResult(T data, string messages) : base(data, true, messages) { }

       //public SuccessDataResult(T data) : base(data,true) { }
    }
}
