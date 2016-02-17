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

                var offQuery = db.OfflineSell.AsQueryable();

                var olQuery = db.AppOrder.AsQueryable();

                offQuery = offQuery.Where(q => q.bFinish == Finish.YJS && q.XFRQ > yestday && q.XFRQ < zero);

                olQuery = olQuery.Where(q => q.Status == 5 && q.CreateTime > yestday && q.CreateTime < zero);

                if (UserContext.store == null)
                {
                    ViewBag.storeNumber = db.Store.Where(q => q.Status == Status.enable).Count();

                    //统计线下营业额
                    decimal offlineSale = offQuery.Sum(q => (decimal?)q.JE).GetValueOrDefault();
                    //统计线上营业额
                    decimal onlineSale = olQuery.Sum(q => (decimal?)q.Payable).GetValueOrDefault();

                    ViewBag.sale = offlineSale + onlineSale;

                    //未处理订单
                    ViewBag.order = db.Order.Where(q => q.Status == OrderStatus.BeforeSend).Take(10).ToList();

                    //销售金额
                    ViewBag.income = db.Order.Where(q => q.Status == OrderStatus.Checked && q.SubmitTime > nowMonth).Sum(q => (decimal?)q.Paid).GetValueOrDefault();

                    //营业额前三
                    ViewBag.saleRank = (from q in db.Store
                                        join off in offQuery on q.ID equals off.StationID into off_join
                                        from o in off_join.DefaultIfEmpty()
                                        join ol in olQuery on q.ID equals ol.StoreId into ol_join
                                        from l in ol_join.DefaultIfEmpty()
                                        group new { q.StoreName, o.JE, l.Payable } by new { q.StoreName } into s
                                        select new PSJE() { StoreName = s.Key.StoreName, Pay = s.Sum(p => (p.JE == null ? 0 : p.JE) + (p.Payable == null ? 0 : p.Payable)) }).OrderByDescending(q => q.Pay).Take(3).Where(q => q.Pay > 0).ToList();

                    var orderQuery = db.Order.AsQueryable();

                    orderQuery = orderQuery.Where(q => q.Status == OrderStatus.Checked && q.SubmitTime > nowMonth);

                    //销售额前三
                    ViewBag.incomeRank = (from q in db.Order
                                          where q.Status == OrderStatus.Checked && q.SubmitTime > nowMonth
                                          group q by new { q.StoreName } into s
                                          select new PSJE() { StoreName = s.Key.StoreName, Pay = s.Sum(p => p.Paid == null ? 0 : p.Paid) }).OrderByDescending(q => q.Pay).Take(3).Where(q => q.Pay > 0).ToList();
                }
                else
                {
                    Store store = UserContext.store;

                    ViewBag.storeNumber = db.Store.Where(q => q.Status == Status.enable && q.UserId.Equals(UserContext.user.ID)).Count();

                    //统计线下营业额
                    decimal offlineSale = offQuery.Where(q => q.StationID.Equals(store.ID)).Sum(q => (decimal?)q.JE).GetValueOrDefault();
                    //统计线上营业额
                    decimal onlineSale = olQuery.Where(q => q.StoreId.Equals(store.ID)).Sum(q => (decimal?)q.Payable).GetValueOrDefault();

                    ViewBag.sale = offlineSale + onlineSale;

                    ViewBag.stocks = (from q in db.Stock
                                      join off in offQuery on new { q.StationID, SPBM = q.ProdCode } equals new { off.StationID, off.SPBM } into off_join
                                      from o in off_join.DefaultIfEmpty()
                                      join ao in olQuery on q.StationID equals ao.StoreId into ao_join
                                      from a in ao_join.DefaultIfEmpty()
                                      join aoi in db.AppOrderItem on new { OrderId = a.ID, ProductCode = q.ProdCode } equals new { aoi.OrderId, aoi.ProductCode } into aoi_join
                                      from ai in aoi_join.DefaultIfEmpty()
                                      where q.StationID == store.ID
                                      group new { q.NameCh, q.ProdCode, q.In, o.SL, ai.OrderNumber } by new { q.NameCh, q.ProdCode } into s
                                      select new CXSL() { ProductName = s.Key.NameCh, ProductCode = s.Key.ProdCode, ProductNumber = s.Sum(p => p.In) - s.Sum(p => p.SL == null ? 0 : p.SL) - s.Sum(p => p.OrderNumber == null ? 0 : p.OrderNumber) }).OrderBy(q => q.ProductNumber).Take(10).ToList();

                    ViewBag.order = db.Order.Where(q => q.StoreId.Equals(store.ID) && (q.Status == OrderStatus.BeforeSubmit || q.Status == OrderStatus.Sended)).Take(10).ToList();
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
