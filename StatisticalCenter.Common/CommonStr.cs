using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace StatisticalCenter.Common
{
    public class CommonStr
    {

        /// <summary>
        /// 登陆后保存的用户  Cookie
        /// </summary>
        public static string COOKIE = "UserCookie";

        /// <summary>
        /// 登陆后保存的用户id  INT
        /// </summary>
        public static string USERID = "UserId";

        /// <summary>
        /// 登陆后保存的用户名
        /// </summary>
        public static string USERNAME = "UserName";

        /// <summary>
        /// 登陆后保存的昵称
        /// </summary>
        public static string NICKNAME = "NickName";

        /// <summary>
        /// 此处密码为UserId加上Arshop.dbo.MAS_SYSTEMCONFIG表中Key值为UserPwdKey的项进行可逆加密
        /// </summary>
        public static string Password = "Password";

        /// <summary>
        /// 获取客户IP
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string ipAddress = "";
            IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in arrIPAddresses)
            {
                if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    ipAddress = ip.ToString();
                }
            }
            return ipAddress;
        }
    }
}