using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.SessionState;
using StatisticalCenter.Common;
using Newtonsoft.Json;
using StatisticalCenter.DataAccess.TableMagicBook;
using StatisticalCenter.DataAccess.MultiTable;
using StatisticalCenter.Model;
namespace StatisticalCenter.BookCount
{
    /// <summary>
    /// ajax1 的摘要说明
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
                    context.Response.Write("登录失效，请重新登录4");
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
                case "GetBookTable": GetBookTable(context); break;//图书列表
                case "GetUserPressTable": GetUserPressTable(context); break;//出版商列表
                case "GetBuyBookCountData": GetBuyBookCountData(context); break;//图书购买统计
                case "GetBuyBookExportData": GetBuyBookExportData(context); break;//图书购买统计导出
                case "GetBuyBookExportDetailData": GetBuyBookExportDetailData(context); break;//图书购买统计导出明细
                case "GetBookDownloadCountData": GetBookDownloadCountData(context); break;//图书下载统计
                case "GetBookDownloadExportData": GetBookDownloadExportData(context); break;//图书下载统计导出
                case "GetBookContentCountData": GetBookContentCountData(context); break;//图书内容统计
                case "GetBookContentExportData": GetBookContentExportData(context); break;//图书内容统计导出
                case "GetBookContentExportDetailData": GetBookContentExportDetailData(context); break;//图书内容统计明细导出
                case "GetBookReadCountData": GetBookReadCountData(context); break;//获取图书阅读统计
                case "GetBookReadExportData": GetBookReadExportData(context); break;//获取图书阅读分析统计的导出数据
                case "GetBookReadExportDetailData": GetBookReadExportDetailData(context); break; //获取图书阅读分析统计的明细导出数据
                case "GetBookReadCount_Detail": GetBookReadCount_Detail(context); break;
                case "GetBookReadExportCount":GetBookReadExportCount(context);break;
            }
        }

        /// <summary>
        /// 获取图书列表
        /// </summary>
        /// <param name="context"></param>
        private void GetBookTable(HttpContext context)
        {
            int PressID = ConvertHelper.ToInt32(context.Request["PressID"]);
            dream_multimedia_book data = new dream_multimedia_book();
            DataTable dt = data.GetBookTable(PressID, new BasePage.BasePage().userId);
            string strJson = JsonConvert.SerializeObject(dt);
            context.Response.Write(strJson);
        }

        /// <summary>
        /// 获取出版社列表
        /// </summary>
        /// <param name="context"></param>
        private void GetUserPressTable(HttpContext context)
        {

            //dt.Rows.InsertAt(drr, h);
            T_AdminPress data = new T_AdminPress();
            DataTable dt = data.GetPublishTable(new BasePage.BasePage().userId);
            DataRow drr = dt.NewRow();
            drr["PressID"] = 0;
            drr["PressName"] = "全部";
            dt.Rows.InsertAt(drr, 0);
            string strJson = JsonConvert.SerializeObject(dt);
            context.Response.Write(strJson);
        }

        /// <summary>
        /// 获取图书购买统计
        /// </summary>
        /// <returns></returns>
        public void GetBuyBookCountData(HttpContext context)
        {
            int pageIndex = ConvertHelper.ToInt32(context.Request["pageIndex"]);
            int pageSize = ConvertHelper.ToInt32(context.Request["pageSize"]);
            int pressId = ConvertHelper.ToInt32(context.Request["pressId"]);
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }

            int recordCount = 0;
            BookCountAccess data = new BookCountAccess();
            int totalMxzCount = 0;//总计梦想钻数量
            int totalMxbCount = 0;//总计梦想币数量
            DataTable dt = data.GetBuyBookCountData(pageIndex, pageSize, pressId, bookGuid, startDate, endDate, new BasePage.BasePage().userId, ref recordCount, ref totalMxzCount, ref totalMxbCount);

            ResponseBookBuyCountModel result = new ResponseBookBuyCountModel();
            result.list = dt;
            result.recordCount = recordCount;
            result.totalMxbCount = totalMxbCount;
            result.totalMxzCount = totalMxzCount;
            string strJson = JsonConvert.SerializeObject(result);
            context.Response.Write(strJson);
        }

        /// <summary>
        /// 获取图书购买统计的导出数据
        /// </summary>
        /// <returns></returns>
        public void GetBuyBookExportData(HttpContext context)
        {
            int pressId = ConvertHelper.ToInt32(context.Request["pressId"]);
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }
            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBuyBookExportData(pressId, bookGuid, startDate, endDate, new BasePage.BasePage().userId);

            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "图书购买记录" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        /// <summary>
        /// 获取图书购买统计的明细导出数据
        /// </summary>
        /// <returns></returns>
        public void GetBuyBookExportDetailData(HttpContext context)
        {
            int pressId = ConvertHelper.ToInt32(context.Request["pressId"]);
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }
            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBuyBookExportDetailData(pressId, bookGuid, startDate, endDate, new BasePage.BasePage().userId);

            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "图书购买记录明细" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        /// <summary>
        /// 获取图书下载统计
        /// </summary>
        /// <returns></returns>
        public void GetBookDownloadCountData(HttpContext context)
        {
            int pageIndex = ConvertHelper.ToInt32(context.Request["pageIndex"]);
            int pageSize = ConvertHelper.ToInt32(context.Request["pageSize"]);
            int pressId = ConvertHelper.ToInt32(context.Request["pressId"]);
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }
            int recordCount = 0;
            int totalDownLoadCount = 0;
            int totalPersonCount = 0;

            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBookDownloadCountData(pageIndex, pageSize, pressId, bookGuid, startDate, endDate, new BasePage.BasePage().userId, ref recordCount, ref totalDownLoadCount, ref totalPersonCount);

            ResponseBookDownloadModel result = new ResponseBookDownloadModel();
            result.list = dt;
            result.recordCount = recordCount;
            result.totalDownLoadCount = totalDownLoadCount;
            result.totalPersonCount = totalPersonCount;

            string strResult = JsonConvert.SerializeObject(result);
            context.Response.Write(strResult);
        }

        /// <summary>
        /// 获取图书下载统计的导出数据
        /// </summary>
        /// <returns></returns>
        public void GetBookDownloadExportData(HttpContext context)
        {
            int pressId = ConvertHelper.ToInt32(context.Request["pressId"]);
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }
            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBookDownloadExportData(pressId, bookGuid, startDate, endDate, new BasePage.BasePage().userId);
            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "图书下载统计明细" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }
        /// <summary>
        /// 获取图书内容统计
        /// </summary>
        /// <returns></returns>
        public void GetBookContentCountData(HttpContext context)
        {
            int pageIndex = ConvertHelper.ToInt32(context.Request["pageIndex"]);
            int pageSize = ConvertHelper.ToInt32(context.Request["pageSize"]);
            int pressId = ConvertHelper.ToInt32(context.Request["pressId"]);
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];

            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }
            int recordCount = 0;

            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBookContentCountData(pageIndex, pageSize, pressId, bookGuid, startDate, endDate, new BasePage.BasePage().userId, ref recordCount);
            BaseResponse result = new BaseResponse();
            result.list = dt;
            result.recordCount = recordCount;
            string strJson = JsonConvert.SerializeObject(result);
            context.Response.Write(strJson);
        }

        /// <summary>
        /// 获取图书内容统计的导出
        /// </summary>
        /// <returns></returns>
        public void GetBookContentExportData(HttpContext context)
        {
            int pressId = ConvertHelper.ToInt32(context.Request["pressId"]);
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }

            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBookContentExportData(pressId, bookGuid, startDate, endDate, new BasePage.BasePage().userId);
            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "图书内容统计" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        /// <summary>
        /// 获取图书内容统计的明细导出数据
        /// </summary>
        /// <returns></returns>
        public void GetBookContentExportDetailData(HttpContext context)
        {
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            if (string.IsNullOrEmpty(bookGuid))
            {
                return;
            }
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }
            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBookContentExportDetailData(bookGuid, startDate, endDate, new BasePage.BasePage().userId);
            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "图书内容明细统计" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        /// <summary>
        /// 获取图书阅读分析统计
        /// </summary>
        /// <returns></returns>
        public void GetBookReadCountData(HttpContext context)
        {
            int pageIndex = ConvertHelper.ToInt32(context.Request["pageIndex"]);
            int pageSize = ConvertHelper.ToInt32(context.Request["pageSize"]);
            int pressId = ConvertHelper.ToInt32(context.Request["pressId"]);
            int type = ConvertHelper.ToInt32(context.Request["type"]);
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }

            int recordCount = 0;
            int totalPersonCount = 0;
            long totalTimeLength = 0;
            int totalReadCount = 0;
            BookCountAccess data = new BookCountAccess();

            DataTable dt = data.GetBookReadCountData(pageIndex, pageSize, type, pressId, bookGuid, startDate, endDate, new BasePage.BasePage().userId, ref recordCount, ref totalPersonCount, ref totalTimeLength, ref totalReadCount);

            ResponseBookReadCountModel result = new ResponseBookReadCountModel();
            result.list = dt;
            result.recordCount = recordCount;
            result.avgTimeLength = totalReadCount == 0 ? 0 : (int)(totalTimeLength / totalReadCount);
            result.totalPersonCount = totalPersonCount;
            result.totalReadCount = totalReadCount;
            string strResult = JsonConvert.SerializeObject(result);
            context.Response.Write(strResult);
        }

        /// <summary>
        /// 获取图书阅读分析统计的导出数据
        /// </summary>
        /// <returns></returns>
        public void GetBookReadExportData(HttpContext context)
        {
            int pressId = ConvertHelper.ToInt32(context.Request["pressId"]);
            int type = ConvertHelper.ToInt32(context.Request["type"]);
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }

            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBookReadExportData(type, pressId, bookGuid, startDate, endDate, new BasePage.BasePage().userId);
            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            if (type == 0)
            {
                helper.ExportDataGridToCSV(dt, "图书阅读统计" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            }
            else
            {
                helper.ExportDataGridToCSV(dt, "系列阅读统计" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            }
        }

        /// <summary>
        /// 获取图书阅读分析统计的明细导出数据
        /// </summary>
        /// <returns></returns>
        public void GetBookReadExportDetailData(HttpContext context)
        {
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }

            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBookReadExportDetailData(bookGuid, startDate, endDate, new BasePage.BasePage().userId);
            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "图书阅读明细统计" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        /// <summary>
        /// 明细导出
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public void GetBookReadCount_Detail(HttpContext context)
        {
            string bookGuid = context.Request["bookGuid"];
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToShortDateString();
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToShortDateString();
            }

            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBookReadCount_Detail(bookGuid, startDate, endDate);
            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "图书热点明细统计" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        /// <summary>
        /// 获取图书阅读统计导出信息
        /// </summary>
        /// <param name="context"></param>
        public void GetBookReadExportCount(HttpContext context)
        {
            int pressId = ConvertHelper.ToInt32(context.Request["pressId"]);//出版社ID
            int bookType = ConvertHelper.ToInt32(context.Request["bookType"]);//0图书，1系列
            string bookGuid =context.Request["bookGuid"];//图书GUID
            DateTime timeStart = ConvertHelper.ToDateTime(context.Request["timeStart"]);//开始时间
            DateTime timeEnd = ConvertHelper.ToDateTime(context.Request["timeEnd"]);//结束时间
            BookCountAccess data = new BookCountAccess();
            DataTable dt = data.GetBookReadCountData_Export(bookType, pressId, bookGuid, timeStart.ToShortDateString(), timeEnd.ToShortDateString(), new BasePage.BasePage().userId);
            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            if (bookType == 0)
            {
                helper.ExportDataGridToCSV(dt, "图书阅读统计" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            }
            else
            {
                helper.ExportDataGridToCSV(dt, "系列阅读统计" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            }
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