using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utils;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class RolesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult getTable()
        {
            using (DBContext db = new DBContext())
            {
                List<GuserRole> roles = db.GuserRole.ToList();

                return PartialView("List", roles);
            }
        }

        [HttpPost]
        public object queryDialog(string roleId)
        {
            using (DBContext db = new DBContext())
            {
                if (!string.IsNullOrEmpty(roleId))
                {
                    GuserRole role = db.GuserRole.Where(q => q.ID.Equals(roleId)).FirstOrDefault();

                    if (role == null) { return Json(new { code = -1, msg = "找不到指定角色" }); }

                    ViewBag.role = role;

                    return PartialView("Edit");
                }

                return PartialView("Add");
            }
        }

        [HttpPost]
        public JsonResult queryTree(string roleId)
        {
            ArrayList zNodes = new ArrayList();

            GuserRole role = null;

            if (!string.IsNullOrEmpty(roleId))
            {
                DBContext db = new DBContext();

                role = db.GuserRole.Where(q => q.ID.Equals(roleId)).FirstOrDefault();
            }

            addNodes(zNodes, MenuContext.menus, "0", role);

            return Json(new { code = 1, tree = new JavaScriptSerializer().Serialize(zNodes) });
        }

        private void addNodes(ArrayList zNodes, List<Menu> list, string pId, GuserRole role)
        {
            foreach (var item in list)
            {
                zNodes.Add(new
                {
                    id = item.ID,
                    pId = pId,
                    name = item.Title,
                    authVal = item.AuthVal,
                    open = true,
                    @checked = (role != null && ((Convert.ToInt16(role.RoleVal) & Convert.ToInt16(item.AuthVal)) > 0))
                });

                if (item.SubMenu.Count > 0) { addNodes(zNodes, item.SubMenu, item.ID, role); }
            }
        }

        [HttpPost]
        public JsonResult editRole(GuserRole role)
        {
            using (DBContext db = new DBContext())
            {
                //判断名称是否重复
                GuserRole sameName = db.GuserRole.Where(q => q.RoleName.Equals(role.RoleName) && !q.ID.Equals(role.ID)).FirstOrDefault();

                if (sameName != null) { return Json(new { code = -1, msg = "已有同名角色" }); }

                //判断权限值是否重复
                GuserRole sameAuth = db.GuserRole.Where(q => q.RoleVal.Equals(role.RoleVal) && !q.ID.Equals(role.ID)).FirstOrDefault();

                if (sameAuth != null) { return Json(new { code = -2, msg = "已有相同权限的角色" }); }

                GuserRole oldRole = db.GuserRole.Where(q => q.ID.Equals(role.ID)).FirstOrDefault();

                if (oldRole == null)
                {
                    role.CreatorID = UserContext.user.ID;
                    role.Creator = UserContext.user.DisplayName;
                    role.Status = Status.enable;

                    db.GuserRole.Add(role);
                }
                else
                {
                    oldRole.ModifyTime = DateTime.Now;
                    oldRole.RoleName = role.RoleName;
                    oldRole.RoleVal = role.RoleVal;
                    oldRole.Status = role.Status;

                    db.Entry(oldRole).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "保存成功" });
        }

        [HttpPost]
        public JsonResult deleteRole(string roleId)
        {
            using (DBContext db = new DBContext())
            {
                GuserRole role = db.GuserRole.Where(q => q.ID.Equals(roleId)).FirstOrDefault();

                if (role == null) { return Json(new { code = -1, msg = "您要删除的用户不存在" }); }

                db.GuserRole.Remove(role);

                db.SaveChanges();

                return Json(new { code = 1, msg = "删除成功" });
            }
        }
    }
}
