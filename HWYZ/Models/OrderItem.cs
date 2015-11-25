using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HWYZ.Models
{
    [Table("OrderItem")]
    public class OrderItem
    {
        [Key, Column(Order = 1), Required, MaxLength(0x40)]
        public string ID { get; set; }

        [Display(Name = "订单ID"), MaxLength(0x40), Required]
        public string OrderId { get; set; }

        [Display(Name = "商品ID"), MaxLength(0x40), Required]
        public string ProductId { get; set; }

        [Display(Name = "商品编码"), MaxLength(0x40), Required]
        public string ProductCode { get; set; }

        [Display(Name = "商品名称"), MaxLength(0x40), Required]
        public string ProductName { get; set; }

        [Display(Name = "订单数量")]
        public int OrderNumber { get; set; }

        [Display(Name = "配送数量")]
        public int RealNumber { get; set; }

        [Display(Name = "折扣系数")]
        public decimal Discount { get; set; }

        [Display(Name = "单价")]
        public decimal Price { get; set; }
    }
}
