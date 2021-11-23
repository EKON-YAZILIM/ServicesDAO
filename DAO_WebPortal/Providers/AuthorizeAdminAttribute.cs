using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_WebPortal.Providers
{
    public class AuthorizeAdminAttribute : ActionFilterAttribute
    {
        public AuthorizeAdminAttribute()
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

                if (context.HttpContext.Session.GetString("UserType") != Helpers.Constants.Enums.UserIdentityType.Admin.ToString())
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
