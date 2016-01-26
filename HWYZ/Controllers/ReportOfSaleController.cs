using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HWYZ.Controllers
{
    public class ReportOfSaleController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult List(string Province, string City, string Country, string StoreId, string StartDate, string EndDate, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var storeQuery = db.Store.AsQueryable();

                if (!string.IsNullOrEmpty(Province)) { storeQuery = storeQuery.Where(q => q.Province.Equals(Province)); }

                if (!string.IsNullOrEmpty(City)) { storeQuery = storeQuery.Where(q => q.City.Equals(City)); }

                if (!string.IsNullOrEmpty(Country)) { storeQuery = storeQuery.Where(q => q.Country.Equals(Country)); }

                if (!string.IsNullOrEmpty(StoreId)) { storeQuery = storeQuery.Where(q => q.ID.Equals(StoreId)); }

                var offQuery = db.OfflineSell.AsQueryable();

                var olQuery = db.AppOrder.AsQueryable();

                DateTime now = DateTime.Now;
                //不选择开始日期默认为本月1号
                DateTime start = string.IsNullOrEmpty(StartDate) ? DateTime.Parse(string.Format("{0}/{1}/{2}", now.Year.ToString(), now.Month.ToString(), "01")) : DateTime.Parse(StartDate);
                DateTime end = string.IsNullOrEmpty(EndDate) ? now : DateTime.Parse(EndDate).AddDays(1);

                if (start > end)
                {
                    DateTime temp = DateTime.MinValue;
                    temp = end;
                    end = start;
                    start = temp;
                }

                offQuery = offQuery.Where(q => q.XFRQ.CompareTo(start) > 0 && q.XFRQ.CompareTo(end) < 0 && q.bFinish == Finish.YJS);

                olQuery = olQuery.Where(q => q.CreateTime.CompareTo(start) > 0 && q.CreateTime.CompareTo(end) < 0 && q.Status == 5);

                ViewBag.list = (from q in storeQuery
                                join off in offQuery on q.ID equals off.StationID into off_join
                                from o in off_join.DefaultIfEmpty()
                                join ol in olQuery on q.ID equals ol.StoreId into ol_join
                                from l in ol_join.DefaultIfEmpty()
                                group new { q.StoreName, o.JE, l.Payable } by new { q.StoreName } into s
                                select new PSJE() { StoreName = s.Key.StoreName, Pay = s.Sum(p => (p.JE == null ? 0 : p.JE) + (p.Payable == null ? 0 : p.Payable)) }).OrderByDescending(q => q.Pay).Skip((pi - 1) * 10).Take(10).ToList();

                ViewBag.pager = new Pager()
                {
                    _TotalCount = storeQuery.Count(),
                    _PageIndex = pi
                };
            }

            return PartialView();
        }

        public FileResult Export(string Province, string City, string Country, string StoreId, string StartDate, string EndDate, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var storeQuery = db.Store.AsQueryable();

                if (!string.IsNullOrEmpty(Province)) { storeQuery = storeQuery.Where(q => q.Province.Equals(Province)); }

                if (!string.IsNullOrEmpty(City)) { storeQuery = storeQuery.Where(q => q.City.Equals(City)); }

                if (!string.IsNullOrEmpty(Country)) { storeQuery = storeQuery.Where(q => q.Country.Equals(Country)); }

                if (!string.IsNullOrEmpty(StoreId)) { storeQuery = storeQuery.Where(q => q.ID.Equals(StoreId)); }

                var offQuery = db.OfflineSell.AsQueryable();

                var olQuery = db.AppOrder.AsQueryable();

                DateTime now = DateTime.Now;
                //不选择开始日期默认为本月1号
                DateTime start = string.IsNullOrEmpty(StartDate) ? DateTime.Parse(string.Format("{0}/{1}/{2}", now.Year.ToString(), now.Month.ToString(), "01")) : DateTime.Parse(StartDate);
                DateTime end = string.IsNullOrEmpty(EndDate) ? now : DateTime.Parse(EndDate).AddDays(1);

                if (start > end)
                {
                    DateTime temp = DateTime.MinValue;
                    temp = end;
                    end = start;
                    start = temp;
                }

                offQuery = offQuery.Where(q => q.XFRQ.CompareTo(start) > 0 && q.XFRQ.CompareTo(end) < 0 && q.bFinish == Finish.YJS);

                olQuery = olQuery.Where(q => q.CreateTime.CompareTo(start) > 0 && q.CreateTime.CompareTo(end) < 0 && q.Status == 5);

                var list = (from q in storeQuery
                            join off in offQuery on q.ID equals off.StationID into off_join
                            from o in off_join.DefaultIfEmpty()
                            join ol in olQuery on q.ID equals ol.StoreId into ol_join
                            from l in ol_join.DefaultIfEmpty()
                            group new { q.StoreName, o.JE, l.Payable } by new { q.StoreName } into s
                            select new PSJE() { StoreName = s.Key.StoreName, Pay = s.Sum(p => (p.JE == null ? 0 : p.JE) + (p.Payable == null ? 0 : p.Payable)) }).OrderByDescending(q => q.Pay).ToList();

                //创建Excel文件的对象
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

                //给sheet1添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("经营单位");
                row1.CreateCell(1).SetCellValue("营业额");

                string status = string.Empty;
                //将数据逐步写入sheet1各个行
                for (int i = 0; i < list.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].StoreName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].Pay.ToString());
                }

                // 写入到客户端 
                return ExportExcel(book, now.ToString("yyMMddHHmmssfff"));
            }
        }
    }
}
