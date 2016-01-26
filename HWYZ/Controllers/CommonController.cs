using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace HWYZ.Controllers
{
    public class CommonController : Controller
    {
        [HttpGet]
        public JsonResult getArea(string levelType, string pId, string key, int page = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Area.AsQueryable();

                query = query.Where(q => q.LevelType.Equals(levelType));

                if (!string.IsNullOrEmpty(pId)) { query = query.Where(q => q.ParentId.Equals(pId)); }

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.Name.Contains(key) || q.Pinyin.Contains(key)); }

                ArrayList results = new ArrayList();

                List<Area> list = query.Skip((page - 1) * 10).Take(10).ToList();

                foreach (var item in list)
                {
                    results.Add(new { id = item.ID, name = item.Name, code = item.CityCode });
                }

                int total = query.Count();

                return Json(new { results = results, total = total, pageSize = 10 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult getStore(string key, int page = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Store.AsQueryable();

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.StoreName.Contains(key) || q.StoreCode.Contains(key)); }

                ArrayList results = new ArrayList();

                List<Store> list = query.Skip((page - 1) * 10).Take(10).ToList();

                foreach (var item in list)
                {
                    results.Add(new { id = item.ID, name = item.StoreName });
                }

                int total = query.Count();

                return Json(new { results = results, total = total, pageSize = 10 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult getProduct(string key, int page = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Product.AsQueryable();

                query = query.Where(q => q.StoreId == null && q.Status == Status.enable);

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.ProductName.Contains(key) || q.ProductCode.Contains(key)); }

                ArrayList results = new ArrayList();

                List<Product> list = query.Skip((page - 1) * 10).Take(10).ToList();

                foreach (var item in list)
                {
                    results.Add(new { id = item.ID, name = item.ProductName, code = item.ProductCode, price = item.Price });
                }

                int total = query.Count();

                return Json(new { results = results, total = total, pageSize = 10 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult getStoreProduct(string key, int page = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Product.AsQueryable();

                query = query.Where(q => q.StoreId == null || q.StoreId.Equals(UserContext.store.ID));

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.ProductName.Contains(key) || q.ProductCode.Contains(key)); }

                ArrayList results = new ArrayList();

                List<Product> list = query.Skip((page - 1) * 10).Take(10).ToList();

                foreach (var item in list)
                {
                    results.Add(new { id = item.ID, name = item.ProductName, code = item.ProductCode, price = item.Price });
                }

                int total = query.Count();

                return Json(new { results = results, total = total, pageSize = 10 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult getUser(string key, int page = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Guser.AsQueryable();

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.DisplayName.Contains(key) || q.PinYin.Contains(key) || q.PinYin1.Contains(key)); }

                ArrayList results = new ArrayList();

                List<Guser> list = query.Skip((page - 1) * 10).Take(10).ToList();

                foreach (var item in list)
                {
                    results.Add(new { id = item.ID, name = item.DisplayName });
                }

                int total = query.Count();

                return Json(new { results = results, total = total, pageSize = 10 }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
