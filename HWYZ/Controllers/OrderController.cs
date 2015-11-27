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
    public class OrderController : BaseController
    {
        public ActionResult Index(string OrderCode, string StoreId, string Tel, string StartDate, string EndDate, string Status, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                Expression<Func<Order, bool>> where = PredicateExtensions.True<Order>();

                if (!string.IsNullOrEmpty(OrderCode)) { where = where.And(q => q.OrderCode.Contains(OrderCode)); }

                if (!string.IsNullOrEmpty(Tel)) { where = where.And(q => q.Tel.Contains(Tel)); }

                if (!string.IsNullOrEmpty(Status)) { where = where.And(q => q.Status.ToString().Equals(Status)); }

                DateTime now = DateTime.Now;
                //不选择开始日期默认为本月1号
                DateTime start = string.IsNullOrEmpty(StartDate) ? DateTime.Parse(string.Format("{0}/{1}/{2}", now.Year.ToString(), now.Month.ToString(), "01")) : DateTime.Parse(StartDate);
                DateTime end = string.IsNullOrEmpty(EndDate) ? now : DateTime.Parse(EndDate);

                if (start > end)
                {
                    DateTime temp = DateTime.MinValue;
                    temp = end;
                    end = start;
                    start = temp;
                }

                where = where.And(q => q.SubmitTime.CompareTo(start) > 0 && q.SubmitTime.CompareTo(end) < 0);

                Guser user = UserContext.user;

                if (user.Store == null)
                {
                    where = where.And(q => q.Status > 0);

                    if (!string.IsNullOrEmpty(StoreId)) { where = where.And(q => q.StoreId.Contains(StoreId)); }
                }
                else
                {
                    where = where.And(q => q.StoreId.Equals(user.StoreId));
                }

                PagedList<Order> cards = db.Order.Where(where.Compile()).OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Order>(new List<Order>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
            }
        }

        public ActionResult addOrder()
        {
            Guser user = UserContext.user;

            if (user.Store == null) { return null; }

            using (DBContext db = new DBContext())
            {
                DateTime now = DateTime.Now;

                string code = db.Order.Where(q => q.StoreId.Equals(user.StoreId)).Max(q => q.OrderCode);

                string start = string.Format("{0}{1}", user.Store.StoreCode, now.ToString("yyMMdd"));

                if (code == null || !code.StartsWith(start))
                {
                    code = string.Format("{0}{1}00", user.Store.StoreCode, now.ToString("yyMMdd"));
                }
                else
                {
                    int num = Convert.ToInt16(code.Substring(start.Length)) + 1;

                    if (num > 99) { return null; }

                    string numStr = num > 9 ? num.ToString() : "0" + num;

                    code = start + numStr;
                }

                Order order = new Order()
                {
                    Creator = user.DisplayName,
                    CreatorID = user.ID,
                    OrderCode = code,
                    StoreId = user.StoreId,
                    StoreName = user.Store.StoreName,
                    Tel = user.Tel,
                    Status = OrderStatus.BeforeSubmit,
                    SubmitTime = now
                };

                db.Order.Add(order);
                db.SaveChanges();

                return RedirectToAction("Index", "OrderEdit", new { orderId = order.ID });
            }
        }

        [HttpPost]
        public JsonResult delOrder(string orderId)
        {
            using (DBContext db = new DBContext())
            {
                Order order = db.Order.Where(q => q.ID.Equals(orderId)).FirstOrDefault();

                if (order == null) { return Json(new { code = -1, msg = "找不到对应订单" }); }

                db.OrderItem.RemoveRange(db.OrderItem.Where(q => q.OrderId.Equals(orderId)));

                db.Order.Remove(order);

                db.SaveChanges();

                return Json(new { code = 1, msg = "删除成功" });
            }
        }
    }
}
