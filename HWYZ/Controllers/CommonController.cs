using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace HWYZ.Controllers
{
    public class CommonController : Controller
    {
        public JsonResult getArea(string levelType, string pId, string key, int page = 1)
        {
            using (DBContext db = new DBContext())
            {
                Expression<Func<Area, bool>> where = PredicateExtensions.True<Area>();

                where = where.And(q => q.LevelType.Equals(levelType));

                if (!string.IsNullOrEmpty(pId)) { where = where.And(q => q.ParentId.Equals(pId)); }

                if (!string.IsNullOrEmpty(key)) { where = where.And(q => q.Name.Contains(key) || q.Pinyin.Contains(key)); }

                ArrayList results = new ArrayList();

                List<Area> list = db.Area.Where(where.Compile()).Skip((page - 1) * 10).Take(10).ToList();

                foreach (var item in list)
                {
                    results.Add(new { id = item.ID, name = item.Name });
                }

                int total = db.Area.Where(where.Compile()).Count();

                return Json(new { results = results, total = total, pageSize = 10 }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
