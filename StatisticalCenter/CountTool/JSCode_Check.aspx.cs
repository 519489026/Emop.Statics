using StatisticalCenter.Common;
using StatisticalCenter.DataAccess.TableMagicBook;
using StatisticalCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalCenter.CountTool
{
    public partial class JSCode_Check : BasePage.BasePage
    {
        public string strCode = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            string url = HttpContext.Current.Request.Url.Query;
            string[] str = url.Split('=');
            int ID= ConvertHelper.ToInt32(str[1]);
            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetResourceDetail(ID);
            BaseResponse result = new BaseResponse();
            result.recordCount = dt == null ? 0 : 1;
            result.list = dt;
            strCode = dt.Rows[0]["CTRCode"].ToString();//获取CODE
        }
    }
}