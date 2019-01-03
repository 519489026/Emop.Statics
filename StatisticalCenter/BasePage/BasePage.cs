using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using StatisticalCenter.DataAccess.TableArShop;
using StatisticalCenter.Common;

namespace StatisticalCenter.BasePage
{
    public class BasePage : Page
    {
        public BasePage()
        {


            //context.Session.Add(Common.CommonStr.USERID, dt.Rows[0]["MAS_ADMIN_ID"]);
            //context.Session.Add(Common.CommonStr.USERNAME, dt.Rows[0]["MAS_ADMIN_NAME"]);
            //context.Session.Add(Common.CommonStr.NICKNAME, dt.Rows[0]["MAS_ADMIN_NICKNAME"]);
            //HttpCookie cookie = new HttpCookie(Common.CommonStr.COOKIE);
            //cookie.Values.Add(Common.CommonStr.USERID, dt.Rows[0]["MAS_ADMIN_ID"].ToString());
            //cookie.Values.Add(Common.CommonStr.USERNAME, dt.Rows[0]["MAS_ADMIN_NAME"].ToString());
            ////cookie.Values.Add(Common.CommonStr.NICKNAME, context.Server.HtmlEncode(dt.Rows[0]["MAS_ADMIN_NICKNAME"].ToString()));

            //context.Response.AppendCookie(cookie);
            //}
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SetUser();
        }

        public void SetUser()
        {
            // 这里就可以访问Session对象了   
            if (userId == 0)
            {
                if ((HttpContext.Current.Request.Cookies[CommonStr.Password] == null || string.IsNullOrEmpty(HttpContext.Current.Request.Cookies[CommonStr.Password].ToString())) && string.IsNullOrEmpty(HttpContext.Current.Request["password"]))
                {
                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["LoginUrl"]);
                    return;
                }
                string strPass = string.Empty;
                if (!string.IsNullOrEmpty(Request["password"]))
                {
                    strPass = HttpContext.Current.Request["password"];
                }
                else if (HttpContext.Current.Request.Cookies[CommonStr.Password] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies[CommonStr.Password].ToString()))
                {
                    strPass = HttpContext.Current.Request.Cookies[CommonStr.Password].ToString();
                }
                string strUserId = MXR.Utilities.Security.Cryptography.Decryption.MXR(strPass);
                int intuserId = ConvertHelper.ToInt32(strUserId);
                if (intuserId <= 0)
                {
                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["LoginUrl"]);
                    return;
                }
                DataTable dtUser = new MAS_ADMIN_ACCOUNT().getUserLogin(intuserId);
                if (dtUser == null || dtUser.Rows.Count <= 0)
                {
                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["LoginUrl"]);
                    return;
                }
                Session.Add(CommonStr.USERID, dtUser.Rows[0]["MAS_ADMIN_ID"]);
                Session.Add(CommonStr.USERNAME, dtUser.Rows[0]["MAS_ADMIN_NAME"]);
                Session.Add(CommonStr.NICKNAME, dtUser.Rows[0]["MAS_ADMIN_NICKNAME"]);
                Session.Add(CommonStr.Password, MXR.Utilities.Security.Cryptography.Encryption.MXR(dtUser.Rows[0]["MAS_ADMIN_ID"].ToString(), true));
                userName = dtUser.Rows[0]["MAS_ADMIN_NAME"].ToString();
                userNickName = dtUser.Rows[0]["MAS_ADMIN_NICKNAME"].ToString();
                userId = intuserId;
                HttpCookie cookie = new HttpCookie(CommonStr.COOKIE);
                cookie.Values.Add(CommonStr.USERID, dtUser.Rows[0]["MAS_ADMIN_ID"].ToString());
                cookie.Values.Add(CommonStr.USERNAME, dtUser.Rows[0]["MAS_ADMIN_NAME"].ToString());
                cookie.Values.Add(CommonStr.Password, MXR.Utilities.Security.Cryptography.Encryption.MXR(dtUser.Rows[0]["MAS_ADMIN_ID"].ToString(), true));
                HttpContext.Current.Response.AppendCookie(cookie);
            }
            else
            {
                string strPass = string.Empty;
                if (!string.IsNullOrEmpty(Request["password"]))
                {
                    strPass = HttpContext.Current.Request["password"];
                }
                else if (HttpContext.Current.Request.Cookies[CommonStr.Password] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies[CommonStr.Password].ToString()))
                {
                    strPass = HttpContext.Current.Request.Cookies[CommonStr.Password].ToString();
                }
                int intUserId = ConvertHelper.ToInt32(MXR.Utilities.Security.Cryptography.Decryption.MXR(strPass));
                if (intUserId == 0)
                {
                    return;
                }
                if (intUserId == userId)
                {
                    return;
                }
                userId = intUserId;
                DataTable dtUser = new MAS_ADMIN_ACCOUNT().getUserLogin(userId);
                if (dtUser == null || dtUser.Rows.Count <= 0)
                {
                    CommonWeb.Alert(this, "登录失效，请重新登录");
                    return;
                }
                userName = dtUser.Rows[0]["MAS_ADMIN_NAME"].ToString();
                userNickName = dtUser.Rows[0]["MAS_ADMIN_NICKNAME"].ToString();
                Session.Add(CommonStr.USERID, dtUser.Rows[0]["MAS_ADMIN_ID"]);
                Session.Add(CommonStr.USERNAME, dtUser.Rows[0]["MAS_ADMIN_NAME"]);
                Session.Add(CommonStr.NICKNAME, dtUser.Rows[0]["MAS_ADMIN_NICKNAME"]);
                Session.Add(CommonStr.Password, MXR.Utilities.Security.Cryptography.Encryption.MXR(dtUser.Rows[0]["MAS_ADMIN_ID"].ToString(), true));

                HttpCookie cookie = new HttpCookie(CommonStr.COOKIE);
                cookie.Values.Add(CommonStr.USERID, dtUser.Rows[0]["MAS_ADMIN_ID"].ToString());
                cookie.Values.Add(CommonStr.USERNAME, dtUser.Rows[0]["MAS_ADMIN_NAME"].ToString());
                cookie.Values.Add(CommonStr.Password, MXR.Utilities.Security.Cryptography.Encryption.MXR(dtUser.Rows[0]["MAS_ADMIN_ID"].ToString(), true));
                HttpContext.Current.Response.AppendCookie(cookie);
            }
        }


        /// <summary>
        /// 当前登录用户的ID
        /// </summary>
        public int userId
        {
            get
            {
                return ConvertHelper.ToInt32(Session[CommonStr.USERID]);
            }
            set
            {
                Session[CommonStr.USERID] = value;
            }
        }

        /// <summary>
        /// 当前登录用户的用户名
        /// </summary>
        public string userName
        {
            get
            {
                return Session[CommonStr.USERNAME] == null ? string.Empty : Session[CommonStr.USERNAME].ToString();
            }
            set
            {
                Session[CommonStr.USERNAME] = value;
            }
        }

        /// <summary>
        /// 当前登录用户的昵称
        /// </summary>
        public string userNickName
        {
            get
            {
                return Session[CommonStr.NICKNAME] == null ? string.Empty : Session[CommonStr.NICKNAME].ToString();
            }
            set
            {
                Session[CommonStr.NICKNAME] = value;
            }
        }
    }
}