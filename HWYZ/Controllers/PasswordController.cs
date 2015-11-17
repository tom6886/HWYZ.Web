using HWYZ.Context;
using HWYZ.Filters;
using HWYZ.Models;
using System.Data.Entity;
using System.Web.Mvc;
using Utils;

namespace HWYZ.Controllers
{
    public class PasswordController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult changePassword(string oldPass, string newPass)
        {
            using (DBContext db = new DBContext())
            {
                string _oldPass = StringUtil.Md5Encrypt(oldPass);

                Guser user = UserContext.user;

                if (!user.PassWord.Equals(_oldPass)) { return Json(new { code = -1, msg = "原密码错误" }); }

                user.PassWord = StringUtil.Md5Encrypt(newPass);

                db.Entry(user).State = EntityState.Modified;

                db.SaveChanges();

                return Json(new { code = 1, msg = "修改成功，3秒后跳转到登录页面", url = "Login/LogOff" });
            }
        }
    }
}
