using HWYZ.Models;
using System.Collections.Generic;
using System.Web;

namespace HWYZ.Context
{
    /// <summary>
    ///当前用户的运行环境，不管是登录的用户还是匿名用户，都的一份独立的运行环境，存放在session中
    /// </summary>
    public class UserContext
    {
        /// <summary>
        ///  用户信息的会话ID
        /// </summary>
        private const string ACCOUNT_KEY = "SESSION-ACCOUNT-KEY";

        /// <summary>
        /// 当前系统登录用户
        /// </summary>
        public static Guser user
        {
            get
            {
                if (HttpContext.Current.Session[ACCOUNT_KEY] == null)
                {
                    return null;
                }
                return HttpContext.Current.Session[ACCOUNT_KEY] as Guser;
            }
            set
            {
                HttpContext.Current.Session[ACCOUNT_KEY] = value;
            }
        }

        /// <summary>
        ///  门店信息的会话ID
        /// </summary>
        private const string STORE_KEY = "SESSION-STORE-KEY";

        public static Store store
        {
            get
            {
                if (HttpContext.Current.Session[STORE_KEY] == null)
                {
                    return null;
                }
                return HttpContext.Current.Session[STORE_KEY] as Store;
            }
            set
            {
                HttpContext.Current.Session[STORE_KEY] = value;
            }
        }

        /// <summary>
        ///  门店信息的会话ID
        /// </summary>
        private const string STORES_KEY = "SESSION-STORES-KEY";

        public static List<Store> stores
        {
            get
            {
                if (HttpContext.Current.Session[STORES_KEY] == null)
                {
                    return null;
                }
                return HttpContext.Current.Session[STORES_KEY] as List<Store>;
            }
            set
            {
                HttpContext.Current.Session[STORES_KEY] = value;
            }
        }
    }
}
