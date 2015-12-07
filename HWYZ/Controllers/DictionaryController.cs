using HWYZ.Context;
using HWYZ.Filters;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Utils;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    [AdminAuthorize]
    public class DictionaryController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Dictionary.AsQueryable();

                query = query.Where(q => q.ParentCode == null);

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.Name.Contains(key) || q.Code.Contains(key)); }

                PagedList<Dictionary> cards = query.OrderBy(q => q.ModifyTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Dictionary>(new List<Dictionary>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
            }
        }

        [HttpPost]
        public object queryDialog(string id)
        {
            using (DBContext db = new DBContext())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    Dictionary dict = db.Dictionary.Where(q => q.ID.Equals(id)).FirstOrDefault();

                    if (dict == null) { return Json(new { code = -1, msg = "找不到指定参数" }); }

                    ViewBag.dict = dict;

                    return PartialView("Edit");
                }

                return PartialView("Add");
            }
        }

        [HttpPost]
        public JsonResult editDict(Dictionary dict)
        {
            using (DBContext db = new DBContext())
            {
                Dictionary oldOne = db.Dictionary.Where(q => q.ID.Equals(dict.ID)).FirstOrDefault();

                if (oldOne == null)
                {
                    dict.CreatorID = UserContext.user.ID;
                    dict.Creator = UserContext.user.DisplayName;

                    db.Dictionary.Add(dict);
                }
                else
                {
                    oldOne.SortOrder = dict.SortOrder;
                    oldOne.Name = dict.Name;

                    //只有子项的值可以修改
                    if (!string.IsNullOrEmpty(oldOne.ParentCode))
                    {
                        oldOne.Code = dict.Code;
                    }

                    oldOne.ModifyTime = DateTime.Now;

                    db.Entry(oldOne).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "保存成功" });
        }

        [HttpPost]
        public object queryNodes(string code)
        {
            ViewBag.code = code;

            return PartialView("Nodes");
        }

        [HttpPost]
        public ActionResult getNodes(string code)
        {
            using (DBContext db = new DBContext())
            {
                List<Dictionary> nodes = db.Dictionary.Where(q => q.ParentCode.Equals(code)).OrderBy(q => q.SortOrder).ToList();

                return PartialView("NodesList", nodes);
            }
        }

        [HttpPost]
        public object queryNodeEdit(string parentCode, string id)
        {
            ViewBag.parentCode = parentCode;

            using (DBContext db = new DBContext())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    Dictionary dict = db.Dictionary.Where(q => q.ID.Equals(id)).FirstOrDefault();

                    if (dict == null) { return Json(new { code = -1, msg = "找不到指定参数" }); }

                    ViewBag.dict = dict;

                    return PartialView("NodeEdit");
                }

                return PartialView("NodeAdd");
            }
        }
    }
}
