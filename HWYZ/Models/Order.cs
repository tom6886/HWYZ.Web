using HWYZ.Context;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace HWYZ.Models
{
    [Table("Order")]
    public class Order : BaseObj
    {
        [Display(Name = "订单号"), MaxLength(0x40), Required]
        public string OrderCode { get; set; }

        [Display(Name = "采购单位ID"), MaxLength(100)]
        public string StoreId { get; set; }

        [Display(Name = "采购单位名称"), MaxLength(100)]
        public string StoreName { get; set; }

        [Display(Name = "负责人联系电话"), MaxLength(100)]
        public string Tel { get; set; }

        [Display(Name = "发货人ID"), MaxLength(0x40)]
        public string DeliverId { get; set; }

        [Display(Name = "发货人姓名"), MaxLength(100)]
        public string DeliverName { get; set; }

        [Display(Name = "发货人电话"), MaxLength(0x40)]
        public string DeliverTel { get; set; }

        [Display(Name = "快递单号"), MaxLength(50)]
        public string ExpressCode { get; set; }

        [Display(Name = "快递查询地址")]
        public string ExpressUrl { get; set; }

        [Display(Name = "备注"), MaxLength(200)]
        public string Remark { get; set; }

        [Display(Name = "下单时间")]
        public DateTime SubmitTime { get; set; }

        [Display(Name = "应付金额")]
        public decimal Payable { get; set; }

        [Display(Name = "实付金额")]
        public decimal Paid { get; set; }

        [Display(Name = "驳回原因")]
        public string RejectReason { get; set; }

        public OrderStatus Status { get; set; }

        public static void RefreshPayable(string id)
        {
            using (DBContext db = new DBContext())
            {
                Order order = db.Order.Where(q => q.ID.Equals(id)).FirstOrDefault();

                order.Payable = db.OrderItem.Where(q => q.OrderId.Equals(id)).Sum(q => q.Price * q.OrderNumber * q.Discount);

                db.Entry(order).State = EntityState.Modified;

                db.SaveChanges();
            }
        }
    }
}
