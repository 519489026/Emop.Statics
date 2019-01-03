using System;
using StatisticalCenter.DataAccess.UserLogin;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;

namespace StatisticalCenter
{
    public partial class UserLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        }

        protected void a_login_Click(object sender, EventArgs e)
        {
            string userName = txtAccount.Text;
            string passWord = txtPwd.Text;
            UserLoginDataAccess userLogin = new UserLoginDataAccess();
            DataTable dt = userLogin.getUserLogin(userName, passWord);
            if (dt.Rows.Count == 1)
            {
                Context.Session.Add(Common.CommonStr.USERID, dt.Rows[0]["MAS_ADMIN_ID"]);
                Context.Session.Add(Common.CommonStr.USERNAME, dt.Rows[0]["MAS_ADMIN_NAME"]);
                Context.Session.Add(Common.CommonStr.NICKNAME, dt.Rows[0]["MAS_ADMIN_NICKNAME"]);
                HttpCookie cookie = new HttpCookie(Common.CommonStr.COOKIE);
                cookie.Values.Add(Common.CommonStr.USERID, dt.Rows[0]["MAS_ADMIN_ID"].ToString());
                cookie.Values.Add(Common.CommonStr.USERNAME, dt.Rows[0]["MAS_ADMIN_NAME"].ToString());
                //cookie.Values.Add(Common.CommonStr.NICKNAME, context.Server.HtmlEncode(dt.Rows[0]["MAS_ADMIN_NICKNAME"].ToString()));
                Context.Response.AppendCookie(cookie);

                Response.Redirect("/DataUpdate.aspx");
            }
            else
            {
                Response.Write("<script>alert('用户名密码不正确！');</script>");
            }
        }
    }
}