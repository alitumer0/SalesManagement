namespace BaSalesManagementApp.Core.Utilities.Results
{
    public class ErrorDataResult<T>: DataResult<T>
    {
        public ErrorDataResult(): base(default, false){}
        public ErrorDataResult(string messages): base(default,false,messages){}

        public ErrorDataResult(T data, string messages): base(data,false,messages){}
    }
}
