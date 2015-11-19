using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class ProductSetController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                Expression<Func<Product, bool>> where = PredicateExtensions.True<Product>();

                if (!string.IsNullOrEmpty(key)) { where = where.And(q => q.ProductName.Contains(key) || q.ProductCode.Contains(key)); }

                PagedList<Product> cards = db.Product.Where(where.Compile()).OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Product>(new List<Product>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
            }
        }

        [HttpPost]
        public object queryDialog(string productId)
        {
            using (DBContext db = new DBContext())
            {
                if (!string.IsNullOrEmpty(productId))
                {
                    Product product = db.Product.Where(q => q.ID.Equals(productId)).FirstOrDefault();

                    if (product == null) { return Json(new { code = -1, msg = "找不到指定商品" }); }

                    ViewBag.product = product;

                    return PartialView("Edit");
                }

                return PartialView("Add");
            }
        }
    }
}
