using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace HWYZ.Filters
{
    public sealed class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //如果UserContext.user为null  或者 MenuContext.menus为 null
            if (UserContext.user == null || MenuContext.menus == null || UserContext.store != null)
            {
                //页面跳转到 登录页面
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            //通过验证
            return;
        }
    }
}
