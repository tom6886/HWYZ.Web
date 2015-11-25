using HWYZ.Context;
using HWYZ.Filters;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Utils;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    [StoreAuthorize]
    public class OrderDetailController : BaseController
    {
        public ActionResult Index(string orderId, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                PagedList<OrderItem> cards = db.OrderItem.Where(q => q.OrderId.Equals(orderId)).OrderByDescending(q => q.ProductName).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<OrderItem>(new List<OrderItem>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                Order order = db.Order.Where(q => q.ID.Equals(orderId)).FirstOrDefault();

                if (order == null) { return null; }

                ViewBag.order = order;

                return View(cards);
            }
        }

        [HttpPost]
        public object queryDialog(string orderItemId)
        {
            using (DBContext db = new DBContext())
            {
                if (!string.IsNullOrEmpty(orderItemId))
                {
                    OrderItem item = db.OrderItem.Where(q => q.ID.Equals(orderItemId)).FirstOrDefault();

                    if (item == null) { return Json(new { code = -1, msg = "找不到指定商品" }); }

                    ViewBag.item = item;

                    return PartialView("Edit");
                }

                return PartialView("Add");
            }
        }

        [HttpPost]
        public JsonResult editItem(OrderItem item)
        {
            using (DBContext db = new DBContext())
            {
                //判断订单中是否已有此样商品
                OrderItem sameItem = db.OrderItem.Where(q => q.OrderId.Equals(item.OrderId) && q.ProductId.Equals(item.ProductId) && !q.ID.Equals(item.ID)).FirstOrDefault();

                if (sameItem != null) { return Json(new { code = -1, msg = "订单中已有相同商品" }); }

                OrderItem oldItem = db.OrderItem.Where(q => q.ID.Equals(item.ID)).FirstOrDefault();

                if (oldItem == null)
                {
                    Product product = db.Product.Where(q => q.ID.Equals(item.ProductId)).FirstOrDefault();

                    if (product == null) { return Json(new { code = -2, msg = "抱歉，找不到对应商品" }); }

                    item.ID = StringUtil.UniqueID();
                    item.ProductCode = product.ProductCode;
                    item.ProductName = product.ProductName;
                    item.Price = product.Price;

                    db.OrderItem.Add(item);
                }
                else
                {
                    oldItem.OrderNumber = item.OrderNumber;

                    db.Entry(oldItem).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "保存成功" });
        }
    }
}
