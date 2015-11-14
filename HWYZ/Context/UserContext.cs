using HWYZ.Models;
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
    }
}
