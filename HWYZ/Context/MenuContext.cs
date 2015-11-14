using HWYZ.Models;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace HWYZ.Context
{
    public class MenuContext
    {
        /// <summary>
        ///  用户信息的会话ID
        /// </summary>
        private const string MENU_KEY = "SESSION-MENU-KEY";

        /// <summary>
        /// 当前系统登录用户
        /// </summary>
        public static List<Menu> menus
        {
            get
            {
                if (HttpContext.Current.Session[MENU_KEY] == null)
                {
                    return null;
                }
                return HttpContext.Current.Session[MENU_KEY] as List<Menu>;
            }

            set
            {
                HttpContext.Current.Session[MENU_KEY] = value;
            }
        }
    }
}
