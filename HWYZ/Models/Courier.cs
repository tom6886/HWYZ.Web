using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HWYZ.Models
{
    [Table("Courier")]
    public class Courier : BaseObj
    {
        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }

        [Display(Name = "分店ID"), MaxLength(0x40), Required]
        public string StoreId { get; set; }

        [Display(Name = "送餐员姓名"), MaxLength(100)]
        public string CourierName { get; set; }

        [Display(Name = "送餐员电话"), MaxLength(100)]
        public string CourierTel { get; set; }

        public Status Status { get; set; }
    }
}
