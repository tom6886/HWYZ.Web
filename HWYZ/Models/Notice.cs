using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HWYZ.Models
{
    [Table("Notice")]
    public class Notice : BaseObj
    {
        [Display(Name = "标题"), MaxLength(0x40), Required]
        public string Title { get; set; }

        [Display(Name = "内容"), MaxLength(int.MaxValue)]
        public string Content { get; set; }

        public NoticeStatus Status { get; set; }
    }
}
