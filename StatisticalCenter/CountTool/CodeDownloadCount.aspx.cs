﻿using StatisticalCenter.DataAccess.TableMagicBook;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalCenter.CountTool
{
    public partial class CodeDownloadCount : BasePage.BasePage
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
            int codeid = 0;
            if (txtBookGuidLink.Value == "")
            {
                codeid = 0;
            }
            else
            {
                codeid = Convert.ToInt32(txtBookGuidLink.Value);
            }
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
            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetWayCountData(pageIndex, 10, startDate, endDate, codeid, ref recordCount);
            num.Value = recordCount.ToString();
            repData.DataSource = dt;
            repData.DataBind();

            AspNetPager1.CurrentPageIndex = pageIndex;
            AspNetPager1.RecordCount = recordCount;
            AspNetPager1.DataBind();
        }

        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {
            BindData(AspNetPager1.CurrentPageIndex);
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
    }
}