using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utils;

namespace HWYZ.Models
{
    [Table("SPXF")]
    public class OfflineSell
    {
        [Key]
        public string ID { get; set; }

        [Display(Name = "商品编码")]
        public string SPBM { get; set; }

        [Display(Name = "商品名称")]
        public string SPMC { get; set; }

        [Display(Name = "数量")]
        public int SL { get; set; }

        [Display(Name = "入库单价")]
        public decimal DJ { get; set; }

        [Display(Name = "总价")]
        public decimal JE { get; set; }

        [Display(Name = "分店ID")]
        public string StationID { get; set; }

        [Display(Name = "结算状态")]
        public Finish bFinish { get; set; }

        [Display(Name = "消费日期")]
        public DateTime XFRQ { get; set; }
    }
}
