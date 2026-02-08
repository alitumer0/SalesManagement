namespace BaSalesManagementApp.Core.Utilities.Results
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public T? Data { get; }
        public DataResult(T? data, bool issuccess):base(issuccess)
        {
            Data = data;
        }

        public DataResult(T? data, bool issuccess, string messages) : base(issuccess,messages)
        {
            Data=data;
        }

    }


}

