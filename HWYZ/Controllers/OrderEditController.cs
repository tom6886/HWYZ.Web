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
    [StoreAuthorize]
    public class OrderEditController : BaseController
    {
        public ActionResult Index(string orderId, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                Order order = db.Order.Where(q => q.ID.Equals(orderId)).FirstOrDefault();

                if (order == null || order.Status != OrderStatus.BeforeSubmit) { return null; }

                PagedList<OrderItem> cards = db.OrderItem.Where(q => q.OrderId.Equals(orderId)).OrderByDescending(q => q.ProductName).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<OrderItem>(new List<OrderItem>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                ViewBag.order = order;

                return View(cards);
            }
        }

        [HttpPost]
        public object queryDialog(string orderId, string itemId)
        {
            using (DBContext db = new DBContext())
            {
                Order order = db.Order.Where(q => q.ID.Equals(orderId)).FirstOrDefault();

                if (order == null) { return Json(new { code = -1, msg = "找不到对应订单" }); }

                ViewBag.orderId = orderId;

                if (!string.IsNullOrEmpty(itemId))
                {
                    OrderItem item = db.OrderItem.Where(q => q.ID.Equals(itemId)).FirstOrDefault();

                    if (item == null) { return Json(new { code = -2, msg = "找不到指定商品" }); }

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
                //判断订单是否存在
                Order order = db.Order.Where(q => q.ID.Equals(item.OrderId)).FirstOrDefault();

                if (order == null) { return Json(new { code = -1, msg = "找不到对应订单" }); }

                if (order.Status > OrderStatus.BeforeSubmit && UserContext.user.Store != null) { return Json(new { code = -2, msg = "您没有权限修改已提交过的订单" }); }

                if (order.Status == OrderStatus.Sended) { return Json(new { code = -3, msg = "订单已发货，无法修改" }); }

                if (order.Status == OrderStatus.Reject) { return Json(new { code = -4, msg = "订单已被驳回，无法修改" }); }

                //判断订单中是否已有此样商品
                OrderItem sameItem = db.OrderItem.Where(q => q.OrderId.Equals(item.OrderId) && q.ProductId.Equals(item.ProductId) && !q.ID.Equals(item.ID)).FirstOrDefault();

                if (sameItem != null) { return Json(new { code = -5, msg = "订单中已有相同商品" }); }

                OrderItem oldItem = db.OrderItem.Where(q => q.ID.Equals(item.ID)).FirstOrDefault();

                if (oldItem == null)
                {
                    Product product = db.Product.Where(q => q.ID.Equals(item.ProductId)).FirstOrDefault();

                    if (product == null) { return Json(new { code = -6, msg = "抱歉，找不到对应商品" }); }

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

            Order.RefreshPayable(item.OrderId);

            return Json(new { code = 1, msg = "保存成功" });
        }

        [HttpPost]
        public JsonResult delItem(string itemId)
        {
            string orderId = string.Empty;

            using (DBContext db = new DBContext())
            {
                OrderItem item = db.OrderItem.Where(q => q.ID.Equals(itemId)).FirstOrDefault();

                if (item == null) { return Json(new { code = -1, msg = "找不到对应商品" }); }

                orderId = item.OrderId;

                db.OrderItem.Remove(item);

                db.SaveChanges();
            }

            Order.RefreshPayable(orderId);

            return Json(new { code = 1, msg = "删除成功" });
        }

        [HttpPost]
        public JsonResult submitOrder(string orderId, string remark)
        {
            using (DBContext db = new DBContext())
            {
                Order order = db.Order.Where(q => q.ID.Equals(orderId)).FirstOrDefault();

                if (order == null) { return Json(new { code = -1, msg = "找不到对应订单" }); }

                if (order.Status != OrderStatus.BeforeSubmit) { return Json(new { code = -2, msg = "订单已被提交过，无法重复提交" }); }

                decimal pay = db.OrderItem.Where(q => q.OrderId.Equals(orderId)).Sum(q => q.Price * q.OrderNumber * q.Discount);

                if (pay == 0) { return Json(new { code = -2, msg = "请不要提交没有额度的订单" }); }

                order.Payable = pay;
                order.Paid = pay;
                order.Remark = remark;
                order.SubmitTime = DateTime.Now;
                order.Status = OrderStatus.BeforeSend;

                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "提交订单成功" });
        }
    }
}
