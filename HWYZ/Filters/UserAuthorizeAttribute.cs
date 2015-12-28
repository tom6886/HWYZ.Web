using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace HWYZ.Filters
{
    public sealed class UserAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //如果UserContext.user为null  或者 MenuContext.menus为 null
            if (UserContext.user == null || MenuContext.menus == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest()) { return; }
                //页面跳转到 登录页面
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            Menu menu = MenuContext.menus.Where(q => q.ID.Equals(controllerName)).FirstOrDefault();

            if (menu == null)
            {
                menu = (from q in MenuContext.menus
                        from p in q.SubMenu
                        where p.ID.Equals(controllerName)
                        select p).FirstOrDefault();
            }

            if (menu != null && (menu.AuthVal & Convert.ToInt32(UserContext.user.Role.RoleVal)) == 0)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest()) { return; }
                //页面跳转到 登录页面
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            //通过验证
            return;
        }
    }
}
