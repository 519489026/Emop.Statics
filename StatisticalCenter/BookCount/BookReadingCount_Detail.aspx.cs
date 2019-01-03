using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using StatisticalCenter.DataAccess.MultiTable;
namespace StatisticalCenter.BookCount
{
    public partial class BookReadingCount_Detail : BasePage.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string bookGuid = Request["bookGuid"];
            string startDate = Request["startDate"];
            string endDate = Request["endDate"];
            BookCountAccess data = new BookCountAccess();
            DataTable dt= data.GetBookReadCount_Detail(bookGuid, startDate, endDate);
            repData.DataSource = dt;
            repData.DataBind();
        }
    }
}