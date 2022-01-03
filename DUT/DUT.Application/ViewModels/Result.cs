namespace DUT.Application.ViewModels
{
    public abstract class Result
    {
        protected Result(bool success, bool notFound, string error, Exception exception)
        {
            Success = success;
            NotFound = notFound;
            Error = error;
            Exception = exception;
        }
        public Result()
        {

        }

        public bool Success { get; set; }
        public bool NotFound { get; set; }
        public string Error { get; set; }
        public Exception Exception { get; set; }
    }

    public class Result<T> : Result
    {
        public Result(bool success, bool notFound, string error, Exception exception, T data)
            : base(success, notFound, error, exception)
        {
            Data = data;
        }

        public T Data { get; set; }

        public static Result<T> Success()
        {
            return new Result<T>(true, false, null, null, default);
        }

        public static Result<T> SuccessWithData(T data)
        {
            return new Result<T>(true, false, null, null, data);
        }

        public static Result<T> NotFound(string error = "Resource not found")
        {
            return new Result<T>(false, true, error, null, default);
        }

        public static Result<T> Error(string error = "Resource not found")
        {
            return new Result<T>(false, false, error, null, default);
        }

        public static Result<T> Exception(Exception exception)
        {
            return new Result<T>(false, false, null, exception, default);
        }
    }
}