namespace DUT.Constants.APIResponse
{
    public class APIResponse
    {
        public APIResponse(bool ok, string message, string description, object data)
        {
            Ok = ok;
            Message = message;
            Description = description;
            Data = data;
        }

        public bool Ok { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }

        public static APIResponse OkResponse(object data = null)
        {
            return new APIResponse(true, null, null, data);
        }

        public static APIResponse BadRequestResponse(string error, object data = null)
        {
            return new APIResponse(false, error, null, data);
        }

        public static APIResponse ForbiddenResposne()
        {
            return new APIResponse(false, "Forbidden", "You are missing access rights", null);
        }

        public static APIResponse NotFoundResponse(string error = "Resource not found")
        {
            return new APIResponse(false, error, null, null);
        }

        public static APIResponse InternalServerError(string requestId)
        {
            return new APIResponse(false, "Internal server error", null, requestId);
        }
    }
}
