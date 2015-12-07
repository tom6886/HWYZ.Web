using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                var query = db.Product.AsQueryable();

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.ProductName.Contains(key) || q.ProductCode.Contains(key)); }

                string storeId = UserContext.user.StoreId;

                if (string.IsNullOrEmpty(storeId))
                {
                    //管理员只维护并查看总店的商品
                    query = query.Where(q => q.StoreId == null);
                }
                else
                {
                    //分店可以查看总店在市商品并维护自己添加的商品
                    query = query.Where(q => ((q.StoreId == null && q.Status == Status.enable) || q.StoreId.Equals(storeId)));
                }

                PagedList<Product> cards = query.OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Product>(new List<Product>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
            }
        }

        [HttpPost]
        public object queryViewDialog(string productId)
        {
            using (DBContext db = new DBContext())
            {
                Product product = db.Product.Where(q => q.ID.Equals(productId)).FirstOrDefault();

                if (product == null) { return Json(new { code = -1, msg = "找不到指定商品" }); }

                ViewBag.product = product;

                return PartialView("View");
            }
        }

        [HttpPost]
        public object queryDialog(string productId)
        {
            using (DBContext db = new DBContext())
            {
                ViewBag.kinds = db.Dictionary.Where(q => q.ParentCode.Equals("kinds")).OrderBy(q => q.SortOrder).ToList();

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

        [HttpPost]
        public JsonResult editProduct(Product product)
        {
            using (DBContext db = new DBContext())
            {
                //判断编号是否重复
                Product sameCode = db.Product.Where(q => q.ProductCode.Equals(product.ProductCode) && !q.ID.Equals(product.ID)).FirstOrDefault();

                if (sameCode != null) { return Json(new { code = -1, msg = "商品编号已被注册" }); }

                Product oldProduct = db.Product.Where(q => q.ID.Equals(product.ID)).FirstOrDefault();

                if (oldProduct == null)
                {
                    product.CreatorID = UserContext.user.ID;
                    product.Creator = UserContext.user.DisplayName;
                    product.Name = product.ProductName;
                    product.Status = Status.enable;
                    product.StoreId = UserContext.user.StoreId;

                    db.Product.Add(product);
                }
                else
                {
                    if (UserContext.user.StoreId != oldProduct.StoreId) { return Json(new { code = -2, msg = "抱歉，您没有权限修改本商品" }); }

                    if (!string.IsNullOrEmpty(oldProduct.DocId) && !oldProduct.DocId.Equals(product.DocId))
                    {
                        Doc.delete(oldProduct.DocId);
                    }

                    oldProduct.ModifyTime = DateTime.Now;
                    oldProduct.Name = product.ProductName;
                    oldProduct.ProductType = product.ProductType;
                    oldProduct.Price = product.Price;
                    oldProduct.AllowReturn = product.AllowReturn;
                    oldProduct.Remark = product.Remark;
                    oldProduct.DocId = product.DocId;
                    oldProduct.Status = product.Status;

                    db.Entry(oldProduct).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "保存成功" });
        }

        [HttpPost]
        public JsonResult deleteProduct(string productId)
        {
            using (DBContext db = new DBContext())
            {
                Product product = db.Product.Where(q => q.ID.Equals(productId)).FirstOrDefault();

                if (product == null) { return Json(new { code = -1, msg = "您要删除的用户不存在" }); }

                if (!string.IsNullOrEmpty(product.DocId))
                {
                    Doc.delete(product.DocId);
                }

                db.Product.Remove(product);

                db.SaveChanges();

                return Json(new { code = 1, msg = "删除成功" });
            }
        }
    }
}
