﻿using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HWYZ.Controllers
{
    public class ReportOfSendController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult ListOfDetail(string storeId, string StartDate, string EndDate, string storeType, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var orderQuery = db.Order.AsQueryable();

                orderQuery = SetQuery(orderQuery, storeId, StartDate, EndDate);

                var storeQuery = db.Store.AsQueryable();

                if (!string.IsNullOrEmpty(storeType)) { StoreType type = (StoreType)Convert.ToInt16(storeType); storeQuery = storeQuery.Where(q => q.StoreType == type); }

                ViewBag.list = (from q in db.OrderItem
                                join o in orderQuery on q.OrderId equals o.ID
                                join t in storeQuery on o.StoreId equals t.ID
                                group q by new { o.StoreId, o.StoreName, q.ProductName, q.ProductCode } into s
                                orderby new { s.Key.StoreName, s.Key.ProductName }
                                select new PSMX() { StoreName = s.Key.StoreName, ProductName = s.Key.ProductName, ProductCode = s.Key.ProductCode, ProductNumber = s.Sum(p => p.RealNumber) }).Skip((pi - 1) * 10).Take(10).ToList();

                double totalCount = (from q in db.OrderItem
                                     join o in orderQuery on q.OrderId equals o.ID
                                     join t in storeQuery on o.StoreId equals t.ID
                                     group q by new { o.StoreId, o.StoreName, q.ProductName, q.ProductCode } into s
                                     select new { s.Key.StoreId, s.Key.ProductCode }).Count();

                ViewBag.pager = new Pager()
                {
                    _TotalCount = totalCount,
                    _PageIndex = pi
                };

                return PartialView();
            }
        }

        public FileResult ExportDetail(string storeId, string StartDate, string EndDate, string storeType)
        {
            using (DBContext db = new DBContext())
            {
                var orderQuery = db.Order.AsQueryable();

                orderQuery = SetQuery(orderQuery, storeId, StartDate, EndDate);

                var storeQuery = db.Store.AsQueryable();

                if (!string.IsNullOrEmpty(storeType)) { StoreType type = (StoreType)Convert.ToInt16(storeType); storeQuery = storeQuery.Where(q => q.StoreType == type); }

                var list = (from q in db.OrderItem
                            join o in orderQuery on q.OrderId equals o.ID
                            join t in storeQuery on o.StoreId equals t.ID
                            group q by new { o.StoreId, o.StoreName, q.ProductName, q.ProductCode } into s
                            orderby new { s.Key.StoreName, s.Key.ProductName }
                            select new PSMX() { StoreName = s.Key.StoreName, ProductName = s.Key.ProductName, ProductCode = s.Key.ProductCode, ProductNumber = s.Sum(p => p.RealNumber) }).ToList();

                //创建Excel文件的对象
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

                //给sheet1添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("分店");
                row1.CreateCell(1).SetCellValue("商品名称");
                row1.CreateCell(2).SetCellValue("商品编号");
                row1.CreateCell(3).SetCellValue("采购总数");

                string status = string.Empty;
                //将数据逐步写入sheet1各个行
                for (int i = 0; i < list.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].StoreName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].ProductName);
                    rowtemp.CreateCell(2).SetCellValue(list[i].ProductCode);
                    rowtemp.CreateCell(3).SetCellValue(list[i].ProductNumber);
                }

                DateTime now = DateTime.Now;
                // 写入到客户端 
                return ExportExcel(book, now.ToString("yyMMddHHmmssfff"));
            }
        }

        public PartialViewResult ListOfStore(string storeId, string StartDate, string EndDate, string storeType, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var orderQuery = db.Order.AsQueryable();

                orderQuery = SetQuery(orderQuery, storeId, StartDate, EndDate);

                var storeQuery = db.Store.AsQueryable();

                if (!string.IsNullOrEmpty(storeType)) { StoreType type = (StoreType)Convert.ToInt16(storeType); storeQuery = storeQuery.Where(q => q.StoreType == type); }

                ViewBag.list = (from q in orderQuery
                                join t in storeQuery on q.StoreId equals t.ID
                                group q by new { q.StoreId, q.StoreName } into s
                                select new PSJE() { StoreName = s.Key.StoreName, Pay = s.Sum(p => p.Paid) }).OrderByDescending(q => q.Pay).Skip((pi - 1) * 10).Take(10).ToList();

                double totalCount = (from q in orderQuery
                                     group q by new { q.StoreId } into s
                                     select new { s.Key.StoreId }).Count();

                ViewBag.pager = new Pager()
                {
                    _TotalCount = totalCount,
                    _PageIndex = pi
                };

                return PartialView();
            }
        }

        public FileResult ExportStore(string storeId, string StartDate, string EndDate, string storeType)
        {
            using (DBContext db = new DBContext())
            {
                var orderQuery = db.Order.AsQueryable();

                orderQuery = SetQuery(orderQuery, storeId, StartDate, EndDate);

                var storeQuery = db.Store.AsQueryable();

                if (!string.IsNullOrEmpty(storeType)) { StoreType type = (StoreType)Convert.ToInt16(storeType); storeQuery = storeQuery.Where(q => q.StoreType == type); }

                var list = (from q in orderQuery
                            join t in storeQuery on q.StoreId equals t.ID
                            group q by new { q.StoreId, q.StoreName } into s
                            select new PSJE() { StoreName = s.Key.StoreName, Pay = s.Sum(p => p.Paid) }).OrderByDescending(q => q.Pay).ToList();

                //创建Excel文件的对象
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

                //给sheet1添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("采购单位");
                row1.CreateCell(1).SetCellValue("金额");

                string status = string.Empty;
                //将数据逐步写入sheet1各个行
                for (int i = 0; i < list.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].StoreName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].Pay.ToString());
                }

                DateTime now = DateTime.Now;
                // 写入到客户端 
                return ExportExcel(book, now.ToString("yyMMddHHmmssfff"));
            }
        }

        private IQueryable<Order> SetQuery(IQueryable<Order> orderQuery, string storeId, string StartDate, string EndDate)
        {
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

            orderQuery = orderQuery.Where(q => q.Status == OrderStatus.Sended && q.ModifyTime.CompareTo(start) > 0 && q.ModifyTime.CompareTo(end) < 0);

            if (!string.IsNullOrEmpty(storeId)) { orderQuery = orderQuery.Where(q => q.StoreId.Equals(storeId)); }

            return orderQuery;
        }
    }
}
