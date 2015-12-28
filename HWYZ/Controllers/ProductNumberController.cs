using HWYZ.Context;
using HWYZ.Filters;
using HWYZ.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Utils;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    [StoreAuthorize]
    public class ProductNumberController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.StoreProduct.Include("Product").AsQueryable();

                query = query.Where(q => q.StoreID.Contains(UserContext.user.StoreId));

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.Product.ProductName.Contains(key) || q.Product.ProductCode.Contains(key)); }

                PagedList<StoreProduct> cards = query.OrderBy(q => q.ProductNumber).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<StoreProduct>(new List<StoreProduct>(), 10, 0);

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
                    StoreProduct product = db.StoreProduct.Include("Product").Where(q => q.ID.Equals(id)).FirstOrDefault();

                    if (product == null) { return Json(new { code = -1, msg = "找不到指定商品" }); }

                    ViewBag.product = product;

                    return PartialView("Edit");
                }

                return PartialView("Add");
            }
        }

        [HttpPost]
        public JsonResult editNumber(StoreProduct product)
        {
            using (DBContext db = new DBContext())
            {
                Guser user = UserContext.user;

                //判断是否已存在相同商品
                StoreProduct sameProduct = db.StoreProduct.Where(q => q.ProductID.Equals(product.ProductID) && q.StoreID.Equals(user.StoreId) && !q.ID.Equals(product.ID)).FirstOrDefault();

                if (sameProduct != null) { return Json(new { code = -1, msg = "已存在相同商品" }); }

                StoreProduct oldOne = db.StoreProduct.Where(q => q.ID.Equals(product.ID)).FirstOrDefault();

                if (oldOne == null)
                {
                    product.ID = StringUtil.UniqueID();
                    product.StoreID = user.StoreId;

                    db.StoreProduct.Add(product);
                }
                else
                {
                    oldOne.ProductNumber = product.ProductNumber;
                    oldOne.OnlinePrice = product.OnlinePrice;
                    oldOne.OfflinePrice = product.OfflinePrice;

                    db.Entry(oldOne).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "保存成功" });
        }

        [HttpPost]
        public JsonResult deleteNumber(string id)
        {
            using (DBContext db = new DBContext())
            {
                StoreProduct product = db.StoreProduct.Where(q => q.ID.Equals(id)).FirstOrDefault();

                if (product == null) { return Json(new { code = -1, msg = "您要删除的用户不存在" }); }

                db.StoreProduct.Remove(product);

                db.SaveChanges();

                return Json(new { code = 1, msg = "删除成功" });
            }
        }
    }
}
