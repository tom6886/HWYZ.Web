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
    public class DeliveEditController : BaseController
    {
        public ActionResult Index(string orderId, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                Order order = db.Order.Where(q => q.ID.Equals(orderId)).FirstOrDefault();

                if (order == null || order.Status != OrderStatus.BeforeSend) { return null; }

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

                OrderItem item = db.OrderItem.Where(q => q.ID.Equals(itemId)).FirstOrDefault();

                if (item == null) { return Json(new { code = -2, msg = "找不到指定商品" }); }

                ViewBag.item = item;

                ViewBag.discount = db.Dictionary.Where(q => q.ParentCode.Equals("discount")).OrderBy(q => q.SortOrder).ToList();

                return PartialView("Edit");
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

                if (order.Status == OrderStatus.Sended) { return Json(new { code = -2, msg = "订单已发货，无法修改" }); }

                if (order.Status == OrderStatus.Reject) { return Json(new { code = -3, msg = "订单已被驳回，无法修改" }); }

                OrderItem oldItem = db.OrderItem.Where(q => q.ID.Equals(item.ID)).FirstOrDefault();

                if (oldItem == null)
                {
                    return Json(new { code = -4, msg = "找不到对应商品" });
                }

                oldItem.Discount = item.Discount;
                oldItem.OrderNumber = item.OrderNumber;

                db.Entry(oldItem).State = EntityState.Modified;

                db.SaveChanges();
            }

            decimal pay = Order.RefreshPay(item.OrderId);

            return Json(new { code = 1, msg = "保存成功", pay = pay });
        }

        [HttpPost]
        public JsonResult sendProduct(string orderId, decimal pay, string expressCode, string expressUrl)
        {
            if (pay <= 0) { return Json(new { code = -1, msg = "订单金额必须为正数" }); }

            using (DBContext db = new DBContext())
            {
                Order order = db.Order.Where(q => q.ID.Equals(orderId)).FirstOrDefault();

                if (order == null) { return Json(new { code = -2, msg = "找不到对应订单" }); }

                if (order.Status != OrderStatus.BeforeSend) { return Json(new { code = -3, msg = "订单已发货，无法重复发货" }); }

                Guser user = UserContext.user;

                order.Paid = pay;
                order.ExpressCode = expressCode;
                order.ExpressUrl = expressUrl;
                order.DeliverId = user.ID;
                order.DeliverName = user.DisplayName;
                order.DeliverTel = user.Tel;
                order.Status = OrderStatus.Sended;

                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "提交订单成功" });
        }
    }
}
