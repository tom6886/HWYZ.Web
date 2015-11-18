using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Utils;

namespace HWYZ.Models
{
    [Table("Guser")]
    public class Guser : BaseObj
    {
        [Display(Name = "用户编号"), MaxLength(0x40), Required]
        public string Account { get; set; }

        [Display(Name = "口令"), MaxLength(0x40), Required]
        public string PassWord { get; set; }

        [Display(Name = "身份证号"), MaxLength(100), Required]
        public string CardNumber { get; set; }

        [NotMapped, Display(Name = "用户姓名"), MaxLength(100)]
        public string Name
        {
            set
            {
                this.DisplayName = value;
                this.PinYin = Pinyin.GetPinyin(value);
                this.PinYin1 = Pinyin.GetInitials(value);
            }
        }

        [Display(Name = "用户姓名"), MaxLength(100)]
        public string DisplayName { get; set; }

        [Display(Name = "用户姓名全拼"), MaxLength(100)]
        public string PinYin { get; set; }

        [Display(Name = "用户姓名首字母"), MaxLength(100)]
        public string PinYin1 { get; set; }

        [ForeignKey("RoleId")]
        public virtual GuserRole Role { get; set; }

        [MaxLength(200)]
        public string RoleId { get; set; }

        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }

        [MaxLength(200)]
        public string StoreId { get; set; }

        public Sex Sex { get; set; }

        public Status Status { get; set; }

        public string Tel { get; set; }

        public string Remark { get; set; }
    }
}
