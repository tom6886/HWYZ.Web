using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HWYZ.Controllers
{
    public class ReportOfSellController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult ListOfNumber(string ProductName, string StartDate, string EndDate, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var itemQuery = db.OrderItem.AsQueryable();

                var orderQuery = db.Order.AsQueryable();

                orderQuery = orderQuery.Where(q => q.Status == OrderStatus.Sended);

                if (!string.IsNullOrEmpty(ProductName)) { itemQuery = itemQuery.Where(q => q.ProductName.Equals(ProductName)); }

                if (!string.IsNullOrEmpty(StartDate)) { DateTime start = DateTime.Parse(StartDate); orderQuery = orderQuery.Where(q => q.ModifyTime >= start); }

                if (!string.IsNullOrEmpty(EndDate)) { DateTime end = DateTime.Parse(EndDate); orderQuery = orderQuery.Where(q => q.ModifyTime <= end); }

                ViewBag.list = (from q in itemQuery
                                join o in orderQuery on q.OrderId equals o.ID
                                group q by new { q.ProductName, q.ProductCode } into s
                                select new CXSL() { ProductName = s.Key.ProductName, ProductCode = s.Key.ProductCode, ProductNumber = s.Sum(p => p.RealNumber) }).OrderByDescending(q => q.ProductNumber).Skip((pi - 1) * 10).Take(10).ToList();

                double totalCount = (from q in itemQuery
                                     join o in orderQuery on q.OrderId equals o.ID
                                     group q by new { q.ProductName } into s
                                     select new { s.Key.ProductName }).Count();

                ViewBag.pager = new Pager()
                {
                    _TotalCount = totalCount,
                    _PageIndex = pi
                };

                return PartialView();
            }
        }

        public FileResult ExportNumber(string ProductName, string StartDate, string EndDate)
        {
            using (DBContext db = new DBContext())
            {
                var itemQuery = db.OrderItem.AsQueryable();

                var orderQuery = db.Order.AsQueryable();

                orderQuery = orderQuery.Where(q => q.Status == OrderStatus.Sended);

                if (!string.IsNullOrEmpty(ProductName)) { itemQuery = itemQuery.Where(q => q.ProductName.Equals(ProductName)); }

                if (!string.IsNullOrEmpty(StartDate)) { DateTime start = DateTime.Parse(StartDate); orderQuery = orderQuery.Where(q => q.ModifyTime >= start); }

                if (!string.IsNullOrEmpty(EndDate)) { DateTime end = DateTime.Parse(EndDate); orderQuery = orderQuery.Where(q => q.ModifyTime <= end); }

                var list = (from q in itemQuery
                            join o in orderQuery on q.OrderId equals o.ID
                            group q by new { q.ProductName, q.ProductCode } into s
                            select new CXSL() { ProductName = s.Key.ProductName, ProductCode = s.Key.ProductCode, ProductNumber = s.Sum(p => p.RealNumber) }).OrderByDescending(q => q.ProductNumber).ToList();

                //创建Excel文件的对象
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

                //给sheet1添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("商品名称");
                row1.CreateCell(1).SetCellValue("商品编号");
                row1.CreateCell(2).SetCellValue("数量");

                string status = string.Empty;
                //将数据逐步写入sheet1各个行
                for (int i = 0; i < list.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].ProductName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].ProductCode);
                    rowtemp.CreateCell(2).SetCellValue(list[i].ProductNumber);
                }

                DateTime now = DateTime.Now;
                // 写入到客户端 
                return ExportExcel(book, now.ToString("yyMMddHHmmssfff"));
            }
        }

        public PartialViewResult ListOfPay(string ProductName, string StartDate, string EndDate, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var itemQuery = db.OrderItem.AsQueryable();

                var orderQuery = db.Order.AsQueryable();

                orderQuery = orderQuery.Where(q => q.Status == OrderStatus.Sended);

                if (!string.IsNullOrEmpty(ProductName)) { itemQuery = itemQuery.Where(q => q.ProductName.Equals(ProductName)); }

                if (!string.IsNullOrEmpty(StartDate)) { DateTime start = DateTime.Parse(StartDate); orderQuery = orderQuery.Where(q => q.ModifyTime >= start); }

                if (!string.IsNullOrEmpty(EndDate)) { DateTime end = DateTime.Parse(EndDate); orderQuery = orderQuery.Where(q => q.ModifyTime <= end); }

                ViewBag.list = (from q in itemQuery
                                join o in orderQuery on q.OrderId equals o.ID
                                group q by new { q.ProductName, q.ProductCode } into s
                                select new CXJE() { ProductName = s.Key.ProductName, ProductCode = s.Key.ProductCode, ProductPay = s.Sum(p => p.RealNumber * p.Price * p.Discount) }).OrderByDescending(q => q.ProductPay).Skip((pi - 1) * 10).Take(10).ToList();

                double totalCount = (from q in itemQuery
                                     join o in orderQuery on q.OrderId equals o.ID
                                     group q by new { q.ProductName } into s
                                     select new { s.Key.ProductName }).Count();

                ViewBag.pager = new Pager()
                {
                    _TotalCount = totalCount,
                    _PageIndex = pi
                };

                return PartialView();
            }
        }

        public FileResult ExportPay(string ProductName, string StartDate, string EndDate)
        {
            using (DBContext db = new DBContext())
            {
                var itemQuery = db.OrderItem.AsQueryable();

                var orderQuery = db.Order.AsQueryable();

                orderQuery = orderQuery.Where(q => q.Status == OrderStatus.Sended);

                if (!string.IsNullOrEmpty(ProductName)) { itemQuery = itemQuery.Where(q => q.ProductName.Equals(ProductName)); }

                if (!string.IsNullOrEmpty(StartDate)) { DateTime start = DateTime.Parse(StartDate); orderQuery = orderQuery.Where(q => q.ModifyTime >= start); }

                if (!string.IsNullOrEmpty(EndDate)) { DateTime end = DateTime.Parse(EndDate); orderQuery = orderQuery.Where(q => q.ModifyTime <= end); }

                var list = (from q in itemQuery
                            join o in orderQuery on q.OrderId equals o.ID
                            group q by new { q.ProductName, q.ProductCode } into s
                            select new CXJE() { ProductName = s.Key.ProductName, ProductCode = s.Key.ProductCode, ProductPay = s.Sum(p => p.RealNumber * p.Price * p.Discount) }).OrderByDescending(q => q.ProductPay).ToList();

                //创建Excel文件的对象
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

                //给sheet1添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("商品名称");
                row1.CreateCell(1).SetCellValue("金额");

                string status = string.Empty;
                //将数据逐步写入sheet1各个行
                for (int i = 0; i < list.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].ProductName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].ProductPay.ToString());
                }

                DateTime now = DateTime.Now;
                // 写入到客户端 
                return ExportExcel(book, now.ToString("yyMMddHHmmssfff"));
            }
        }
    }
}
