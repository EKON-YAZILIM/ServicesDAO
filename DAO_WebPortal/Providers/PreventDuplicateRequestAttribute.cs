using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class PreventDuplicateRequestAttribute : ActionFilterAttribute {
    public override void OnActionExecuting(ActionExecutingContext context) {
        if (context.HttpContext.Request.HasFormContentType && context.HttpContext.Request.Form.ContainsKey("__RequestVerificationToken")) {
            var currentToken = context.HttpContext.Request.Form["__RequestVerificationToken"].ToString();
            var lastToken = context.HttpContext.Session.GetString("LastProcessedToken");

            if (lastToken == currentToken) {
                context.ModelState.AddModelError(string.Empty, "Looks like you accidentally submitted the same form twice.");
            }
            else {
                context.HttpContext.Session.SetString("LastProcessedToken", currentToken);
            }
        }
    }
}