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
    public class RolesController : Controller
    {
        public ActionResult Index()
        {
            using (DBContext db = new DBContext())
            {
                List<GuserRole> roles = db.GuserRole.ToList();

                return View(roles);
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
    }
}
