using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace DAO_WebPortal.Providers
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public AuthorizeUserAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                bool control = true;


                if (context.HttpContext.Session.GetInt32("UserID") == null)
                {
                    control = false;
                }

                if (!control)
                {
                    context.Result = new RedirectResult("../");
                }
            }
            catch
            {
                context.Result = new RedirectResult("../");
            }

        }
    }
}
