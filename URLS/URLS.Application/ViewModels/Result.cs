namespace URLS.Application.ViewModels
{
    public class Result<T>
    {
        #region ctors
        public Result(bool success, bool notFound, bool forbid, string error, Exception exception, T data)
        {
            IsSuccess = success;
            IsNotFound = notFound;
            IsForbid = forbid;
            IsError = string.IsNullOrEmpty(error) ? false : true;
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
            return new Result<T>(true, false, false, null, null, default);
        }

        public static Result<T> SuccessWithData(T data)
        {
            if(data == null)
                return Success();
            return new Result<T>(true, false, false, null, null, data);
        }

        public static Result<T> NotFound(string error = "Resource not found")
        {
            return new Result<T>(false, true, false, error, null, default);
        }

        public static Result<T> Error(string error = "Resource not found")
        {
            return new Result<T>(false, false, false, error, null, default);
        }

        public static Result<T> Forbiden(string error = "Forbidden")
        {
            return new Result<T>(false, false, true, error, null, default);
        }

        public static Result<T> Exception(Exception exception)
        {
            return new Result<T>(false, false, false, null, exception, default);
        }

        #endregion

        #region Props

        public bool IsSuccess { get; set; }
        public bool IsNotFound { get; set; }
        public bool IsError { get; set; }
        public bool IsForbid { get; set; }
        public string ErrorMessage { get; set; }
        public Exception ExceptionType { get; set; }
        public T Data { get; set; }

        #endregion
    }
}