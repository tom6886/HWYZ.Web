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
    public class HomeController : BaseController
    {
        public ActionResult Index(int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                DateTime now = DateTime.Now;

                DateTime nowMonth = DateTime.Parse(string.Format("{0}/{1}/{2}", now.Year, now.Month, "01"));

                DateTime zero = DateTime.Parse(string.Format("{0} 00:00:00", now.ToString("yyyy-MM-dd")));

                DateTime yestday = zero.AddDays(-1);

                #region 获取通知公告
                var query = db.Notice.AsQueryable();

                if (UserContext.store != null) { query = query.Where(q => q.Status == NoticeStatus.Published); }

                PagedList<Notice> cards = query.OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Notice>(new List<Notice>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("Notice", cards);
                #endregion

                if (UserContext.store == null)
                {
                    ViewBag.storeNumber = db.Store.Where(q => q.Status == Status.enable).Count();

                    //统计线下营业额
                    decimal offlineSale = db.OfflineSell.Where(q => q.XFRQ > yestday && q.XFRQ < zero).Sum(q => (decimal?)q.JE).GetValueOrDefault();
                    //统计线上营业额
                    decimal onlineSale = db.AppOrder.Where(q => q.CreateTime > yestday && q.CreateTime < zero).Sum(q => (decimal?)q.Payable).GetValueOrDefault();

                    ViewBag.sale = offlineSale + onlineSale;

                    ViewBag.order = db.Order.Where(q => q.Status == OrderStatus.BeforeSend).Take(10).ToList();

                    ViewBag.income = db.Order.Where(q => q.Status == OrderStatus.Checked && q.SubmitTime > nowMonth).Sum(q => (decimal?)q.Paid).GetValueOrDefault();
                }
                else
                {
                    Store store = UserContext.store;

                    ViewBag.storeNumber = db.Store.Where(q => q.Status == Status.enable && q.UserId.Equals(UserContext.user.ID)).Count();

                    //统计线下营业额
                    decimal offlineSale = db.OfflineSell.Where(q => q.XFRQ > yestday && q.XFRQ < zero && q.StationID.Equals(store.ID)).Sum(q => (decimal?)q.JE).GetValueOrDefault();
                    //统计线上营业额
                    decimal onlineSale = db.AppOrder.Where(q => q.CreateTime > yestday && q.CreateTime < zero && q.StoreId.Equals(store.ID)).Sum(q => (decimal?)q.Payable).GetValueOrDefault();

                    ViewBag.sale = offlineSale + onlineSale;

                    ViewBag.stocks = (from q in db.Stock
                                      join off in db.OfflineSell on new { q.StationID, SPBM = q.ProdCode } equals new { off.StationID, off.SPBM } into off_join
                                      from o in off_join.DefaultIfEmpty()
                                      join ao in db.AppOrder on q.StationID equals ao.StoreId into ao_join
                                      from a in ao_join.DefaultIfEmpty()
                                      join aoi in db.AppOrderItem on new { OrderId = a.ID, ProductCode = q.ProdCode } equals new { aoi.OrderId, aoi.ProductCode } into aoi_join
                                      from ai in aoi_join.DefaultIfEmpty()
                                      where q.StationID == store.ID
                                      group new { q.NameCh, q.ProdCode, q.In, o.SL, ai.OrderNumber } by new { q.NameCh, q.ProdCode } into s
                                      select new CXSL() { ProductName = s.Key.NameCh, ProductCode = s.Key.ProdCode, ProductNumber = s.Sum(p => p.In) - s.Sum(p => p.SL == null ? 0 : p.SL) - s.Sum(p => p.OrderNumber == null ? 0 : p.OrderNumber) }).OrderBy(q => q.ProductNumber).Take(10).ToList();

                    ViewBag.order = db.Order.Where(q => q.Status == OrderStatus.BeforeSubmit || q.Status == OrderStatus.Sended).Take(10).ToList();
                }

                return View(cards);
            }
        }

        public ActionResult Edit(string noticeId)
        {
            using (DBContext db = new DBContext())
            {
                if (!string.IsNullOrEmpty(noticeId))
                {
                    Notice notice = db.Notice.Where(q => q.ID.Equals(noticeId)).FirstOrDefault();

                    if (notice == null) { return RedirectToAction("404"); }

                    ViewBag.notice = notice;

                    return View("EditNotice");
                }

                return View("AddNotice");
            }
        }

        public ActionResult ViewNotice(string noticeId)
        {
            using (DBContext db = new DBContext())
            {
                Notice notice = db.Notice.Where(q => q.ID.Equals(noticeId)).FirstOrDefault();

                if (notice == null) { return RedirectToAction("404"); }

                ViewBag.notice = notice;

                return View();
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult editNotice(Notice notice)
        {
            using (DBContext db = new DBContext())
            {
                //判断标题是否重复
                Notice sameTitle = db.Notice.Where(q => q.Title.Equals(notice.Title) && !q.ID.Equals(notice.ID)).FirstOrDefault();

                if (sameTitle != null) { return Json(new { code = -1, msg = "已存在相同标题的公告" }); }

                Notice oldNotice = db.Notice.Where(q => q.ID.Equals(notice.ID)).FirstOrDefault();

                if (oldNotice == null)
                {
                    notice.CreatorID = UserContext.user.ID;
                    notice.Creator = UserContext.user.DisplayName;
                    notice.Status = NoticeStatus.Draft;

                    db.Notice.Add(notice);
                }
                else
                {
                    if (oldNotice.Status == NoticeStatus.Published) { return Json(new { code = -2, msg = "已发布的公告不可以修改" }); }

                    oldNotice.ModifyTime = DateTime.Now;
                    oldNotice.Title = notice.Title;
                    oldNotice.Content = notice.Content;
                    oldNotice.Status = notice.Status;

                    db.Entry(oldNotice).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new { code = 1, msg = "保存成功" });
        }

        [HttpPost]
        public JsonResult deleteNotice(string noticeId)
        {
            using (DBContext db = new DBContext())
            {
                Notice notice = db.Notice.Where(q => q.ID.Equals(noticeId)).FirstOrDefault();

                if (notice == null) { return Json(new { code = -1, msg = "您要删除的公告不存在" }); }

                db.Notice.Remove(notice);

                db.SaveChanges();

                return Json(new { code = 1, msg = "删除成功" });
            }
        }
    }
}
