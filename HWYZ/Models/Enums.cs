using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Utils;

namespace HWYZ.Models
{
    public enum Sex
    {
        male = 1,
        female = 0
    }

    public enum Status
    {
        enable = 1,
        disable = 0
    }

    public enum StoreType
    {
        PSZ = 1,
        JMD = 0
    }

    public enum OrderStatus
    {
        Reject = -1,
        BeforeSubmit = 0,
        BeforeSend = 1,
        Sended = 2,
        Checked = 3
    }

    public enum NoticeStatus
    {
        Draft = 0,
        Published = 1
    }

    public enum Flag
    {
        YRK = 1,
        WRK = 0
    }

    public enum Finish
    {
        YJS = 1,
        WJS = 0
    }
}
