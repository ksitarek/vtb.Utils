using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace vtb.Utils.Attributes
{
    public class ValidateRequestFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsDictionary = context
                    .ModelState
                    .ToDictionary(k => k.Key, v => v.Value.Errors.Select(e => e.ErrorMessage));

                context.Result = new BadRequestObjectResult(errorsDictionary);
            }
            else
            {
                base.OnActionExecuting(context);
            }
        }
    }
}