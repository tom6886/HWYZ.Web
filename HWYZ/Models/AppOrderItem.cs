using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HWYZ.Models
{
    [Table("AppOrderItem")]
    public class AppOrderItem
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "订单ID"), Required]
        public int OrderId { get; set; }

        [Display(Name = "商品ID"), MaxLength(0x40), Required]
        public string ProductId { get; set; }

        [Display(Name = "商品编码"), MaxLength(0x40), Required]
        public string ProductCode { get; set; }

        [Display(Name = "商品名称"), MaxLength(0x40), Required]
        public string ProductName { get; set; }

        [Display(Name = "订单数量")]
        public int OrderNumber { get; set; }

        [Display(Name = "单价")]
        public decimal Price { get; set; }
    }
}
