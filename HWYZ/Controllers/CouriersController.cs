using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class CouriersController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Courier.Include("Store").AsQueryable();

                string storeId = UserContext.user.StoreId;

                if (!string.IsNullOrEmpty(storeId)) { query = query.Where(q => q.StoreId.Equals(storeId)); }

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.CourierName.Contains(key) || q.CourierTel.Contains(key)); }

                PagedList<Courier> cards = query.OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Courier>(new List<Courier>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
            }
        }

        [HttpPost]
        public object queryDialog(string courierId)
        {
            using (DBContext db = new DBContext())
            {
                if (!string.IsNullOrEmpty(courierId))
                {
                    Courier courier = db.Courier.Include("Store").Where(q => q.ID.Equals(courierId)).FirstOrDefault();

                    if (courier == null) { return Json(new { code = -1, msg = "找不到指定送餐员" }); }

                    ViewBag.courier = courier;

                    return PartialView("Edit");
                }

                return PartialView("Add");
            }
        }

        [HttpPost]
        public JsonResult editCourier(Courier courier)
        {
            using (DBContext db = new DBContext())
            {
                Courier oldCourier = db.Courier.Where(q => q.ID.Equals(courier.ID)).FirstOrDefault();

                if (oldCourier == null)
                {
                    Guser user = UserContext.user;

                    courier.CreatorID = user.ID;
                    courier.Creator = user.DisplayName;
                    courier.StoreId = user.StoreId;
                    courier.Status = Status.enable;

                    db.Courier.Add(courier);
                }
                else
                {
                    oldCourier.ModifyTime = DateTime.Now;
                    oldCourier.CourierTel = courier.CourierTel;
                    oldCourier.Status = courier.Status;

                    db.Entry(oldCourier).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "保存成功" });
        }

        [HttpPost]
        public JsonResult deleteCourier(string courierId)
        {
            using (DBContext db = new DBContext())
            {
                Courier courier = db.Courier.Where(q => q.ID.Equals(courierId)).FirstOrDefault();

                if (courier == null) { return Json(new { code = -1, msg = "您要删除的送餐员不存在" }); }

                db.Courier.Remove(courier);

                db.SaveChanges();

                return Json(new { code = 1, msg = "删除成功" });
            }
        }
    }
}
