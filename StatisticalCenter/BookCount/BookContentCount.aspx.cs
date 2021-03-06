﻿using StatisticalCenter.Common;
using StatisticalCenter.DataAccess.MultiTable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalCenter.BookCount
{
    public partial class BookContentCount : BasePage.BasePage
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
            int pressID = ConvertHelper.ToInt32(hidPressID.Value);
            string bookGuID = txtBookGuidLink.Value;
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
            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBookContentCountData(pageIndex, 10, pressID, bookGuID, timestart, timeend, userId, ref recordCount);
            num.Value = recordCount.ToString();
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
            pageIndex.Value = "1";
            if (txtBooklink.Text == "")
            {
                txtBookGuidLink.Value = "";
            }
            if (txtBookGuidLink.Value == "")
            {
                txtBooklink.Text = "全部";
            }
            DateTime start = string.IsNullOrEmpty(textTimeStart.Text) ? DateTime.Now.AddDays(-7).Date : Convert.ToDateTime(textTimeStart.Text);
            DateTime end = string.IsNullOrEmpty(textTimeStart.Text) ? DateTime.Now.Date : Convert.ToDateTime(textTimeEnd.Text);
            if (start >= end)
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
            iswrong.Value = "0";
            pageIndex.Value = "1";
            pressChoose.Value = "0";
            hidPressIndex.Value = "0";
            hidPressID.Value = "0";
            hidPressText.Value = "全部";
            txtBooklink.Text = "图书";
            txtBookGuidLink.Value = "";
            textTimeStart.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            textTimeEnd.Text = DateTime.Now.ToString("yyyy-MM-dd");
            BindData(1);
        }
    }
}