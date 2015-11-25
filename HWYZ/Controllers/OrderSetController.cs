using HWYZ.Context;
using HWYZ.Filters;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Utils;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class OrderSetController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                Expression<Func<Order, bool>> where = PredicateExtensions.True<Order>();

                if (!string.IsNullOrEmpty(key)) { where = where.And(q => q.OrderCode.Contains(key)); }

                Guser user = UserContext.user;

                if (user.Store == null) { where = where.And(q => q.Status > 0); }

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
                    int num = Convert.ToInt16(code.Substring(start.Length));

                    code = start + num;
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

                return RedirectToAction("Index", "OrderDetail", new { orderId = order.ID });
            }
        }
    }
}
