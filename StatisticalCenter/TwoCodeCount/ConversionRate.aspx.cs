using StatisticalCenter.Common;
using StatisticalCenter.DataAccess.TableDataCollection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalCenter.CountTool
{
    public partial class ConversionRate : BasePage.BasePage
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
            string bookName = txtBooklink.Text;
            if (bookName == "全部")
            {
                bookName = "";
            }
            string timestart = textTimeStart.Text;
            string timeend = textTimeEnd.Text;
            if (string.IsNullOrEmpty(timestart))
            {
                timestart = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                textTimeStart.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(timeend))
            {
                timeend = DateTime.Now.ToString("yyyy-MM-dd");
                textTimeEnd.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            int recordCount = 0;
            T_DownLoaded_Book_Data dataAccess = new T_DownLoaded_Book_Data();
            DataTable dtResult = dataAccess.GetCountData(pageIndex, 10, bookName, timestart, timeend, ref recordCount);
            num.Value = recordCount.ToString();
            AspNetPager1.CurrentPageIndex = pageIndex;
            AspNetPager1.RecordCount = recordCount;
            AspNetPager1.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            pageIndex.Value= AspNetPager1.CurrentPageIndex.ToString();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            pageIndex.Value = "1";
            BindData(1);
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtBooklink.Text = "全部";
            textTimeStart.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            textTimeEnd.Text = DateTime.Now.ToString("yyyy-MM-dd");
            BindData(1);
        }
    }
}