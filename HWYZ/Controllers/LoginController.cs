using HWYZ.Context;
using HWYZ.Filters;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Utils;

namespace HWYZ.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpCookie cookie = request.Cookies["session-cookie-name"];

            if (cookie == null) { return View(); }

            string cookieAccountId = cookie["cookie-account-id-key"];

            if (string.IsNullOrEmpty(cookieAccountId))
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);

                return View();
            }

            using (var db = new DBContext())
            {
                Guser user = db.Guser.Include("Role").Where(q => q.ID.Equals(cookieAccountId)).FirstOrDefault();

                if (user == null || user.Status == Status.disable || Convert.ToInt32(user.Role.RoleVal) == 0)
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);

                    return View();
                }

                UserContext.user = user;

                List<Store> stores = db.Store.Where(q => q.UserId.Equals(user.ID)).OrderBy(q => q.StoreCode).ToList();

                if (stores.Count > 0)
                {
                    Store selectStore = stores[0];

                    UserContext.store = selectStore;

                    stores.RemoveAt(0);

                    UserContext.stores = stores;
                }

                List<Menu> menus = XmlHelper.XmlDeserializeFromFile<List<Menu>>(Server.MapPath("~/route.config"), Encoding.UTF8);

                MenuContext.menus = menus;

                string url = GetFirstMenu(menus, Convert.ToInt32(user.Role.RoleVal));

                return RedirectToAction("Index", url);
            }
        }

        [HttpPost]
        public JsonResult signIn(string account, string pwd, string remeberMe)
        {
            string returnUrl = string.Empty;

            using (var db = new DBContext())
            {
                string _pass = StringUtil.Md5Encrypt(pwd);

                Guser user = db.Guser.Include("Role").Where(q => q.Account.Equals(account) && q.PassWord.Equals(_pass)).FirstOrDefault();

                if (user == null) { return Json(new { code = -1, msg = "用户名或密码错误" }); }

                if (user.Status == Status.disable) { return Json(new { code = -2, msg = "此用户已禁用，请联系管理员" }); }

                int roleVal = Convert.ToInt32(user.Role.RoleVal);

                if (roleVal == 0) { return Json(new { code = -3, msg = "此用户角色未分配权限，请联系管理员" }); }

                UserContext.user = user;

                List<Store> stores = db.Store.Where(q => q.UserId.Equals(user.ID)).OrderBy(q => q.StoreCode).ToList();

                if (stores.Count > 0)
                {
                    Store selectStore = stores[0];

                    UserContext.store = selectStore;

                    stores.RemoveAt(0);

                    UserContext.stores = stores;
                }

                List<Menu> menus = XmlHelper.XmlDeserializeFromFile<List<Menu>>(Server.MapPath("~/route.config"), Encoding.UTF8);

                MenuContext.menus = menus;

                //暂时设置为自动保存自动登录
                remeberMe = "1";
                if (!string.IsNullOrEmpty(remeberMe))
                {
                    HttpCookie cookie = new HttpCookie("session-cookie-name");
                    cookie["cookie-account-id-key"] = UserContext.user.ID;
                    cookie.Expires = DateTime.Now.AddDays(7);
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                }

                returnUrl = GetFirstMenu(menus, roleVal);
            }

            return Json(new { code = 1, msg = "登录成功", url = returnUrl });
        }

        private string GetFirstMenu(List<Menu> menus, int roleVal)
        {
            string menuId = string.Empty;

            foreach (Menu item in menus)
            {
                if ((item.AuthVal & roleVal) == 0) { continue; }

                if (item.SubMenu.Count == 0) { menuId = item.ID; break; }

                foreach (Menu sub in item.SubMenu)
                {
                    if ((sub.AuthVal & roleVal) > 0)
                    {
                        menuId = sub.ID;
                        break;
                    }
                }

                break;
            }

            return menuId;
        }

        public ActionResult LogOff()
        {
            Session.Clear();
            HttpCookie cookie = new HttpCookie("session-cookie-name");
            cookie.Expires = DateTime.Now.AddDays(-1);
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "login");
        }

        public JsonResult LogChange(string storeId)
        {
            using (var db = new DBContext())
            {
                Guser user = UserContext.user;

                List<Store> stores = db.Store.Where(q => q.UserId.Equals(user.ID)).OrderBy(q => q.StoreCode).ToList();

                if (stores.Count > 0)
                {
                    Store selectStore = stores.Where(q => q.ID.Equals(storeId)).FirstOrDefault();

                    if (selectStore == null) { return Json(new { code = -1, msg = "找不到对应分店" }); }

                    UserContext.store = selectStore;

                    stores.Remove(selectStore);

                    UserContext.stores = stores;

                    string returnUrl = GetFirstMenu(MenuContext.menus, Convert.ToInt32(UserContext.user.Role.RoleVal));

                    return Json(new { code = 1, url = returnUrl });
                }

                return Json(new { code = -2, msg = "账户存在异常，请重新登录" });
            }
        }
    }
}
