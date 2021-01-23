using iSpeakWebApp.Controllers;
using System.Web.Mvc;
using System.Web.Routing;


namespace iSpeakWebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LoginValidationAttribute());
        }

        public class LoginValidationAttribute : FilterAttribute, IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context.ActionDescriptor.ActionName != nameof(UserAccountsController.Login)
                    && context.ActionDescriptor.ActionName != nameof(UserAccountsController.ChangePassword))
                {
                    if (!UserAccountsController.isLoggedIn(context.HttpContext.Session))
                    {
                        context.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new
                            {
                                action = UserAccountsController.ACTIONNAME_Login,
                                controller = UserAccountsController.CONTROLLERNAME,
                                returnUrl = context.HttpContext.Request.RawUrl
                            })
                        );
                    }
                }
            }

            public void OnActionExecuted(ActionExecutedContext context) { }
        }
    }
}
