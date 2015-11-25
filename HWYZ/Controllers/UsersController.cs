using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Utils;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class UsersController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Guser.AsQueryable();

                string storeId = UserContext.user.StoreId;

                if (!string.IsNullOrEmpty(storeId)) { query = query.Where(q => q.StoreId.Equals(storeId)); }

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.DisplayName.Contains(key) || q.CardNumber.Contains(key)); }

                PagedList<Guser> cards = query.OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Guser>(new List<Guser>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
            }
        }

        [HttpPost]
        public object queryDialog(string userId)
        {
            using (DBContext db = new DBContext())
            {
                ViewBag.roles = db.GuserRole.Select(q => new SelectListItem { Text = q.RoleName, Value = q.ID }).ToList();

                if (!string.IsNullOrEmpty(userId))
                {
                    Guser user = db.Guser.Include("Store").Where(q => q.ID.Equals(userId)).FirstOrDefault();

                    if (user == null) { return Json(new { code = -1, msg = "找不到指定用户" }); }

                    ViewBag.user = user;

                    ViewBag.storeName = user.Store == null ? null : user.Store.StoreName;

                    return PartialView("Edit");
                }

                return PartialView("Add");
            }
        }

        [HttpPost]
        public JsonResult editUser(Guser user)
        {
            using (DBContext db = new DBContext())
            {
                //判断编号是否重复
                Guser sameAccount = db.Guser.Where(q => q.Account.Equals(user.Account) && !q.ID.Equals(user.ID)).FirstOrDefault();

                if (sameAccount != null) { return Json(new { code = -1, msg = "用户编号已被注册" }); }

                Guser oldUser = db.Guser.Where(q => q.ID.Equals(user.ID)).FirstOrDefault();

                if (oldUser == null)
                {
                    user.CreatorID = UserContext.user.ID;
                    user.Creator = UserContext.user.DisplayName;
                    user.Name = user.DisplayName;
                    user.PassWord = StringUtil.Md5Encrypt("888");
                    user.Status = Status.enable;

                    db.Guser.Add(user);
                }
                else
                {
                    oldUser.ModifyTime = DateTime.Now;
                    oldUser.CardNumber = user.CardNumber;
                    oldUser.Name = user.DisplayName;
                    oldUser.RoleId = user.RoleId;
                    oldUser.StoreId = user.StoreId;
                    oldUser.Sex = user.Sex;
                    oldUser.Tel = user.Tel;
                    oldUser.Status = user.Status;

                    db.Entry(oldUser).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "保存成功" });
        }

        [HttpPost]
        public JsonResult deleteUser(string userId)
        {
            using (DBContext db = new DBContext())
            {
                Guser user = db.Guser.Where(q => q.ID.Equals(userId)).FirstOrDefault();

                if (user == null) { return Json(new { code = -1, msg = "您要删除的用户不存在" }); }

                db.Guser.Remove(user);

                db.SaveChanges();

                return Json(new { code = 1, msg = "删除成功" });
            }
        }
    }
}
