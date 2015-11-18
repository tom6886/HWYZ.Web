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
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class StoreGrantController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                Expression<Func<Store, bool>> where = PredicateExtensions.True<Store>();

                if (!string.IsNullOrEmpty(key)) { where = where.And(q => q.StoreName.Contains(key) || q.StoreCode.Contains(key)); }

                PagedList<Store> cards = db.Store.Where(where.Compile()).OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Store>(new List<Store>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
            }
        }

        [HttpPost]
        public object queryDialog(string storeId)
        {
            using (DBContext db = new DBContext())
            {
                if (!string.IsNullOrEmpty(storeId))
                {
                    Store store = db.Store.Where(q => q.ID.Equals(storeId)).FirstOrDefault();

                    if (store == null) { return Json(new { code = -1, msg = "找不到指定门店" }); }

                    ViewBag.store = store;

                    return PartialView("Edit");
                }

                return PartialView("Add");
            }
        }

        [HttpPost]
        public JsonResult editStore(Store store)
        {
            using (DBContext db = new DBContext())
            {
                //判断编号是否重复
                Store sameCode = db.Store.Where(q => q.StoreCode.Equals(store.StoreCode) && !q.ID.Equals(store.ID)).FirstOrDefault();

                if (sameCode != null) { return Json(new { code = -1, msg = "门店编号已被注册" }); }

                Store oldStore = db.Store.Where(q => q.ID.Equals(store.ID)).FirstOrDefault();

                if (oldStore == null)
                {
                    store.CreatorID = UserContext.user.ID;
                    store.Creator = UserContext.user.DisplayName;
                    store.Name = store.StoreName;
                    store.Status = Status.enable;

                    db.Store.Add(store);
                }
                else
                {
                    oldStore.ModifyTime = DateTime.Now;
                    oldStore.Name = store.StoreName;
                    oldStore.Province = store.Province;
                    oldStore.City = store.City;
                    oldStore.Country = store.Country;
                    oldStore.Address = store.Address;
                    oldStore.Lng = store.Lng;
                    oldStore.Lat = store.Lat;
                    oldStore.Presider = store.Presider;
                    oldStore.Tel = store.Tel;
                    oldStore.StoreType = store.StoreType;
                    oldStore.Discount = store.Discount;
                    oldStore.Alipay = store.Alipay;
                    oldStore.WeiXin = store.WeiXin;
                    oldStore.Bank = store.Bank;
                    oldStore.Status = store.Status;

                    db.Entry(oldStore).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "保存成功" });
        }

        [HttpPost]
        public JsonResult deleteStore(string storeId)
        {
            using (DBContext db = new DBContext())
            {
                Store store = db.Store.Where(q => q.ID.Equals(storeId)).FirstOrDefault();

                if (store == null) { return Json(new { code = -1, msg = "您要删除的门店不存在" }); }

                db.Store.Remove(store);

                db.SaveChanges();

                return Json(new { code = 1, msg = "删除成功" });
            }
        }
    }
}
