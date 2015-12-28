using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utils;

namespace HWYZ.Models
{
    [Table("StoreProduct")]
    public class StoreProduct
    {
        [Key, Column(Order = 1), Required, MaxLength(0x40)]
        public string ID { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }

        [Display(Name = "商品ID"), MaxLength(0x40), Required]
        public string ProductID { get; set; }

        [Display(Name = "分店ID"), MaxLength(0x40), Required]
        public string StoreID { get; set; }

        [Display(Name = "商品数量")]
        public int ProductNumber { get; set; }

        [Display(Name = "线上价格")]
        public decimal OnlinePrice { get; set; }

        [Display(Name = "线下价格")]
        public decimal OfflinePrice { get; set; }
    }
}
