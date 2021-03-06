﻿using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class StoreGrantController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Store.Include("User").AsQueryable();

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.StoreName.Contains(key) || q.StoreCode.Contains(key)); }

                PagedList<Store> cards = query.OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 10);

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
                ViewBag.discount = db.Dictionary.Where(q => q.ParentCode.Equals("discount")).OrderBy(q => q.SortOrder).ToList();

                if (!string.IsNullOrEmpty(storeId))
                {
                    Store store = db.Store.Include("User").Where(q => q.ID.Equals(storeId)).FirstOrDefault();

                    if (store == null) { return Json(new { code = -1, msg = "找不到指定门店" }); }

                    ViewBag.store = store;

                    ViewBag.userName = store == null ? null : store.User == null ? null : store.User.DisplayName;

                    return PartialView("Edit");
                }

                return PartialView("Add");
            }
        }

        [HttpPost]
        public string getMap(string city)
        {
            using (DBContext db = new DBContext())
            {
                var list = db.Store.Where(q => q.City.Equals(city)).ToList();

                return new JavaScriptSerializer().Serialize(list);
            }
        }

        [HttpPost]
        public JsonResult editStore(Store store)
        {
            using (DBContext db = new DBContext())
            {
                //判断店名是否重复
                Store sameName = db.Store.Where(q => q.StoreName.Equals(store.StoreName) && !q.ID.Equals(store.ID)).FirstOrDefault();

                if (sameName != null) { return Json(new { code = -1, msg = "门店名称已被注册" }); }

                Store oldStore = db.Store.Where(q => q.ID.Equals(store.ID)).FirstOrDefault();

                if (oldStore == null)
                {
                    //生成流水号
                    int code = db.Store.Count() == 0 ? 80000 : Convert.ToInt32(db.Store.Max(q => q.StoreCode)) + 1;

                    store.StoreCode = code.ToString();
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
                    oldStore.UserId = store.UserId;
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
                    oldStore.BankName = store.BankName;
                    oldStore.BankAccount = store.BankAccount;
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
