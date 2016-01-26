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
    public class ProductDeliveController : BaseController
    {
        public ActionResult Index(string OrderCode, string StoreId, string Tel, string StartDate, string EndDate, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Order.AsQueryable();

                if (!string.IsNullOrEmpty(OrderCode)) { query = query.Where(q => q.OrderCode.Contains(OrderCode)); }

                if (!string.IsNullOrEmpty(Tel)) { query = query.Where(q => q.Tel.Contains(Tel)); }

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

                //只显示代发货的订单
                query = query.Where(q => q.SubmitTime.CompareTo(start) > 0 && q.SubmitTime.CompareTo(end) < 0 && q.Status == OrderStatus.BeforeSend);

                Store store = UserContext.store;

                if (store == null)
                {
                    query = query.Where(q => q.Status != 0);

                    if (!string.IsNullOrEmpty(StoreId)) { query = query.Where(q => q.StoreId.Contains(StoreId)); }
                }
                else
                {
                    query = query.Where(q => q.StoreId.Equals(store.ID));
                }

                PagedList<Order> cards = query.OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Order>(new List<Order>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
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
