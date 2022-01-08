namespace DUT.Application.ViewModels
{
    public class Result<T>
    {
        #region ctors
        public Result(bool success, bool notFound, string error, Exception exception, T data)
        {
            IsSuccess = success;
            IsNotFound = notFound;
            ErrorMessage = error;
            ExceptionType = exception;
            Data = data;
        }
        public Result()
        {

        }
        #endregion

        #region Methods

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

        #endregion

        #region Props

        public bool IsSuccess { get; set; }
        public bool IsNotFound { get; set; }
        public string ErrorMessage { get; set; }
        public Exception ExceptionType { get; set; }
        public T Data { get; set; }

        #endregion
    }
}