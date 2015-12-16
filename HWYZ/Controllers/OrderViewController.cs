using HWYZ.Context;
using HWYZ.Filters;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class OrderViewController : BaseController
    {
        public ActionResult Index(string orderId, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                Order order = db.Order.Where(q => q.ID.Equals(orderId)).FirstOrDefault();

                if (order == null) { return null; }

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
        public JsonResult rejectOrder(string orderId, string reason)
        {
            using (DBContext db = new DBContext())
            {
                Order order = db.Order.Where(q => q.ID.Equals(orderId)).FirstOrDefault();

                if (order == null) { return Json(new { code = -1, msg = "找不到对应订单" }); }

                if (order.Status != OrderStatus.BeforeSend) { return Json(new { code = -2, msg = "非待发货状态的订单不能驳回" }); }

                order.ModifyTime = DateTime.Now;
                order.RejectReason = reason;
                order.Status = OrderStatus.Reject;

                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "已驳回订单，请及时通知分店" });
        }
    }
}
