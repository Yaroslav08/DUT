using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Filters
{
    public class TurnOffEndpointAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            context.Result = new NotFoundResult();
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
