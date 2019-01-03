using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Data;
using StatisticalCenter.Common;
using StatisticalCenter.DataAccess.TableDataCollection;
using StatisticalCenter.Model;
using Newtonsoft.Json;

namespace StatisticalCenter.TwoCodeCount
{
    /// <summary>
    /// ajax 的摘要说明
    /// </summary>
    public class ajax : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            BasePage.BasePage bp = new BasePage.BasePage();
            if (bp.userId <= 0)
            {
                bp.SetUser();
                if (bp.userId <= 0)
                {
                    context.Response.Write("登录失效，请重新登录");
                    return;
                }
            }
            string action = context.Request["action"];
            if (string.IsNullOrEmpty(action))
            {
                context.Response.Write("操作类型有误，请确认");
                return;
            }
            switch (action.Trim())
            {
                case "GetBookList": GetBookList(context); break;
                case "GetCountData": GetCountData(context); break;
                case "GetExportData": GetExportData(context); break;
            }
        }


        /// <summary>
        /// 获取图书列表（仅含制作成二维码的图书）
        /// </summary>
        /// <returns></returns>
        private void GetBookList(HttpContext context)
        {
            T_DownLoaded_Book_Data dataAccess = new T_DownLoaded_Book_Data();
            DataTable dtAccess = dataAccess.GetBookList();
            string strJson = JsonConvert.SerializeObject(dtAccess);
            context.Response.Write(strJson);
        }

        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="bookName">图书名称</param>
        /// <param name="dateStart">统计开始时间</param>
        /// <param name="dateEnd">统计结束时间</param>
        /// <returns></returns>
        public void GetCountData(HttpContext context)
        {
            int pageIndex = ConvertHelper.ToInt32(context.Request["pageIndex"]);
            int pageSize = ConvertHelper.ToInt32(context.Request["pageSize"]);
            string bookName = context.Request["bookName"];
            string dateStart = context.Request["dateStart"];
            string dateEnd = context.Request["dateEnd"];
            int recordCount = 0;
            T_DownLoaded_Book_Data dataAccess = new T_DownLoaded_Book_Data();
            DataTable dtResult = dataAccess.GetCountData(pageIndex, pageSize, bookName, dateStart, dateEnd, ref recordCount);
            BaseResponse result = new BaseResponse();
            result.recordCount = recordCount;
            result.list = dtResult;
            string strJson = JsonConvert.SerializeObject(dtResult);
            context.Response.Write(strJson);
        }

        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <param name="cusId">二维码ID</param>
        /// <param name="bookName">图书名</param>
        /// <param name="dateStart">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns></returns>
        public void GetExportData(HttpContext context)
        {
            int cusId = ConvertHelper.ToInt32(context.Request["cusId"]);
            string bookName = context.Request["bookName"];
            string dateStart = context.Request["dateStart"];
            string dateEnd = context.Request["dateEnd"];
            T_DownLoaded_Book_Data dataAccess = new T_DownLoaded_Book_Data();
            DataTable dt = dataAccess.GetExportData(cusId, dateStart, dateEnd);

            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "二维码转化率统计" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}