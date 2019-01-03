using StatisticalCenter.Common;
using StatisticalCenter.DataAccess.TableMagicBook;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalCenter.CountTool
{
    public partial class CountTool : BasePage.BasePage
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
            int type = ConvertHelper.ToInt32(TypeChoose.Text);
            string name = Name.Text;
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            int recordCount = 0;
            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetResourceData(pageIndex, 10, type, name, ref recordCount);
            num.Value = recordCount.ToString();
            AspNetPager1.CurrentPageIndex = pageIndex;
            AspNetPager1.RecordCount = recordCount;
            AspNetPager1.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            pageIndex.Value = AspNetPager1.CurrentPageIndex.ToString();
        }

        protected void searchButton_Click(object sender, EventArgs e)
        {
            pageIndex.Value = "1";
            BindData(1);
        }

        protected void refresh_Click(object sender, EventArgs e)
        {
            TypeChoose.Text = "-1";
            Name.Text = "";
            pageIndex.Value = "1";
            BindData(1);
        }
    }
}