using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUT.Constants.APIResponse
{
    public class APIResponse
    {
        public APIResponse(bool ok, string message, object data)
        {
            Ok = ok;
            Message = message;
            Data = data;
        }

        public bool Ok { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public static APIResponse OkResponse(object data = null)
        {
            return new APIResponse(true, null, data);
        }

        public static APIResponse BadRequestResponse(string error, object data = null)
        {
            return new APIResponse(false, error, data);
        }

        public static APIResponse NotFoundResponse(string error = "Resource not found")
        {
            return new APIResponse(false, error, null);
        }

        public static APIResponse InternalServerError(string requestId)
        {
            return new APIResponse(false, "Internal server error", requestId);
        }
    }
}
