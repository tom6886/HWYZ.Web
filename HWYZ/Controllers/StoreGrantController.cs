using HWYZ.Context;
using HWYZ.Filters;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class StoreGrantController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                Expression<Func<Store, bool>> where = PredicateExtensions.True<Store>();

                if (!string.IsNullOrEmpty(key)) { where = where.And(q => q.StoreName.Contains(key) || q.StoreCode.Contains(key)); }

                PagedList<Store> cards = db.Store.Where(where.Compile()).OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 1);

                if (null == cards)
                    cards = new PagedList<Store>(new List<Store>(), 1, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
            }
        }
    }
}
