using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HWYZ.Models
{
    [Table("AppOrder")]
    public class AppOrder
    {
        [Key]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; }

        [Display(Name = "采购单位ID"), MaxLength(100)]
        public string StoreId { get; set; }

        [Display(Name = "应付金额")]
        public decimal Payable { get; set; }

        //1-未支付 2-已支付 3-配餐中 4-配送中 5-已收货 6-用户取消订单
        public int Status { get; set; }
    }
}
