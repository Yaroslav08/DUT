using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Filters
{
    public class DevelopmentOnlyAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var env = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>();
            if (!env.IsDevelopment())
            {
                context.Result = new NotFoundResult();
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
