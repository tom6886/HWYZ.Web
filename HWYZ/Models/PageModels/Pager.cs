using System;
using System.Collections.Generic;

namespace HWYZ.Models
{
    public class Pager
    {
        public double _TotalCount
        {
            set
            {
                this.TotalCount = value;
                this.PageCount = Convert.ToInt32(Math.Ceiling(value / 10));
            }
        }

        public double TotalCount { get; set; }

        public int PageCount { get; set; }

        public int _PageIndex
        {
            set
            {
                this.PageIndex = value;
                this.PrevIndex = value == 1 ? 1 : value - 1;
                this.NextIndex = value == PageCount ? PageCount : value + 1;
            }
        }

        public int PageIndex { get; set; }

        public int PrevIndex { get; set; }

        public int NextIndex { get; set; }
    }
}
