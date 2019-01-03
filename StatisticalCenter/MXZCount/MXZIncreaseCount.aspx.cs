using StatisticalCenter.Common;
using StatisticalCenter.DataAccess.TableMagicBook;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalCenter.MXZCount
{
    public partial class MXZIncreaseCount : BasePage.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData(1);
            }
        }

        private void BindData(int pageIndex)
        {
            int type = ConvertHelper.ToInt32(CountType.Text);
            string startDate = textTimeStart.Text;
            string endDate = textTimeEnd.Text;
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                textTimeStart.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToString("yyyy-MM-dd");
                textTimeEnd.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
            int recordCount = 0;
            T_User_MXZ_History dataAccess = new T_User_MXZ_History();
            int totalPersonCount = 0;
            int totalCoinCount = 0;
            DataTable dt = dataAccess.GetMXZOrderCountTable(pageIndex, 10, type, startDate, endDate, ref totalPersonCount,ref totalCoinCount, ref recordCount);

            AspNetPager1.CurrentPageIndex = pageIndex;
            AspNetPager1.RecordCount = recordCount;
            AspNetPager1.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            pageIndex.Value = AspNetPager1.CurrentPageIndex.ToString();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            iswrong.Value = "0";
            DateTime start = Convert.ToDateTime(textTimeStart.Text);
            DateTime end = Convert.ToDateTime(textTimeEnd.Text);
            if (start > end)
            {
                iswrong.Value = "1";
                //Response.Write("<script>alert('开始时间必须小于结束时间！');</script>");
                textTimeStart.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                textTimeEnd.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
            BindData(1);
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            CountType.Text = "-1";
            textTimeStart.Text= DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            textTimeEnd.Text= DateTime.Now.ToString("yyyy-MM-dd");
            BindData(1);
        }
    }
}