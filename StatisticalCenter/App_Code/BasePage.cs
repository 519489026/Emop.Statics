using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using StatisticalCenter.Common;
using System.Data;
using StatisticalCenter.DataAccess.TableArShop;
namespace StatisticalCenter.App_Code
{
    public class BasePage : Page
    {
        public BasePage()
        {
            if (userId == 0)
            {
                if (Request.Cookies[CommonStr.USERID] != null && !string.IsNullOrEmpty(Request.Cookies[CommonStr.USERID].ToString()))
                {
                    string strDesKey = Cache["UserPwdKey"] == null ? string.Empty : Cache["UserPwdKey"].ToString();
                    if (string.IsNullOrEmpty(strDesKey))
                    {
                        strDesKey = new MAS_SYSTEMCONFIG().GetValueByKey("UserPwdKey");
                    }
                    if (string.IsNullOrEmpty(strDesKey))
                    {
                        strDesKey = "mxr2016@" + DateTime.Now.Year;
                    }
                    string strUserId = Encryption.DecryptDES(Request.Cookies[CommonStr.Password].ToString(), strDesKey);
                    userId = ConvertHelper.ToInt32(strUserId);
                    if (userId <= 0)
                    {
                        CommonWeb.Alert(this, "登录失效，请重新登录");
                        return;
                    }
                    DataTable dtUser= new MAS_ADMIN_ACCOUNT().getUserLogin(userId);
                    if(dtUser==null||dtUser.Rows.Count<=0)
                    {
                        CommonWeb.Alert(this, "登录失效，请重新登录");
                        return;
                    }

                    Session.Add(Common.CommonStr.USERID, dtUser.Rows[0]["MAS_ADMIN_ID"]);
                    Session.Add(Common.CommonStr.USERNAME, dtUser.Rows[0]["MAS_ADMIN_NAME"]);
                    Session.Add(Common.CommonStr.NICKNAME, dtUser.Rows[0]["MAS_ADMIN_NICKNAME"]);
                    Session.Add(Common.CommonStr.Password, Encryption.EncryptDES(dtUser.Rows[0]["MAS_ADMIN_ID"].ToString(), strDesKey));

                    HttpCookie cookie = new HttpCookie(Common.CommonStr.COOKIE);
                    cookie.Values.Add(Common.CommonStr.USERID, dtUser.Rows[0]["MAS_ADMIN_ID"].ToString());
                    cookie.Values.Add(Common.CommonStr.USERNAME, dtUser.Rows[0]["MAS_ADMIN_NAME"].ToString());
                    cookie.Values.Add(CommonStr.Password, Encryption.EncryptDES(dtUser.Rows[0]["MAS_ADMIN_ID"].ToString(), strDesKey));
                    Response.AppendCookie(cookie);
                }
                else
                {
                    CommonWeb.Alert(this, "登录失效，请重新登录");
                }


                //context.Session.Add(Common.CommonStr.USERID, dt.Rows[0]["MAS_ADMIN_ID"]);
                //context.Session.Add(Common.CommonStr.USERNAME, dt.Rows[0]["MAS_ADMIN_NAME"]);
                //context.Session.Add(Common.CommonStr.NICKNAME, dt.Rows[0]["MAS_ADMIN_NICKNAME"]);
                //HttpCookie cookie = new HttpCookie(Common.CommonStr.COOKIE);
                //cookie.Values.Add(Common.CommonStr.USERID, dt.Rows[0]["MAS_ADMIN_ID"].ToString());
                //cookie.Values.Add(Common.CommonStr.USERNAME, dt.Rows[0]["MAS_ADMIN_NAME"].ToString());
                ////cookie.Values.Add(Common.CommonStr.NICKNAME, context.Server.HtmlEncode(dt.Rows[0]["MAS_ADMIN_NICKNAME"].ToString()));

                //context.Response.AppendCookie(cookie);
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