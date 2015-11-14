using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Utils;

namespace HWYZ.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpCookie cookie = request.Cookies["session-cookie-name"];

            if (cookie != null)
            {
                string cookieAccountId = cookie["cookie-account-id-key"];
                if (cookieAccountId != null)
                {
                    using (var db = new DBContext())
                    {
                        Guser user = db.Guser.Where(q => q.ID.Equals(cookieAccountId)).FirstOrDefault();

                        UserContext.user = user;
                    }
                }
            }

            if (UserContext.user != null)
            {
                Response.Redirect("~/Home");
            }

            return View();
        }

        [HttpPost]
        public JsonResult signIn(string account, string pwd, string remeberMe)
        {
            using (var db = new DBContext())
            {
                string _pass = StringUtil.Md5Encrypt(pwd);

                Guser user = db.Guser.Where(q => q.Account.Equals(account) && q.PassWord.Equals(_pass)).FirstOrDefault();

                if (user == null) { return Json(new { code = -1, msg = "用户名或密码错误" }); }

                UserContext.user = user;

                MenuContext.menus = XmlHelper.XmlDeserializeFromFile<List<Menu>>(Server.MapPath("~/route.config"), Encoding.UTF8);

                if (!string.IsNullOrEmpty(remeberMe))
                {
                    HttpCookie cookie = new HttpCookie("session-cookie-name");
                    cookie["cookie-account-id-key"] = UserContext.user.ID;
                    cookie.Expires = DateTime.Now.AddDays(7);
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }


            return Json(new { code = 1, msg = "登录成功", url = "Home" });
        }

        public ActionResult LogOff()
        {
            Session.Clear();
            HttpCookie cookie = new HttpCookie("session-cookie-name");
            cookie.Expires = DateTime.Now.AddDays(-1);
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "login");
        }

        private void SerializeObject(string Xmlname)
        {
            List<Menu> list = new List<Menu>();

            List<Menu> list1 = new List<Menu>();

            Menu menu = new Menu() { ID = "home", Title = "首页", Url = "/Home" };

            Menu menu2 = new Menu() { ID = "users", Title = "用户管理", Url = "/Users" };

            list1.Add(menu2);

            Menu menu1 = new Menu() { Title = "系统管理", SubMenu = list1 };

            list.Add(menu);
            list.Add(menu1);

            XmlSerializer ser = new XmlSerializer(typeof(List<Menu>));

            TextWriter writer = new StreamWriter(Xmlname);
            ser.Serialize(writer, list);//要序列化的对象
            writer.Close();
        }
    }
}
