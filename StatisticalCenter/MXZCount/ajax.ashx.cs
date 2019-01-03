using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Data;
using StatisticalCenter.Common;
using Newtonsoft.Json;
using StatisticalCenter.DataAccess.TableMagicBook;
using StatisticalCenter.Model;
namespace StatisticalCenter.MXZCount
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
                case "GetMXZCountTable": GetMXZCountTable(context); break;
                case "ExportMXZCountTable":ExportMXZCountTable(context); break;
                case "ExportMXZDetail":ExportMXZDetail(context); break;
            }
        }

        /// <summary>
        /// 获取梦想钻统计分页数据
        /// </summary>
        /// <param name="context"></param>
        private void GetMXZCountTable(HttpContext context)
        {
            int type = ConvertHelper.ToInt32(context.Request["type"]);
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            int pageIndex = ConvertHelper.ToInt32(context.Request["pageIndex"]);
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            int pageSize = ConvertHelper.ToInt32(context.Request["pageSize"]);
            if (pageSize <= 0)
            {
                pageSize = 10;
            }
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            int recordCount = 0;
            T_User_MXZ_History dataAccess = new T_User_MXZ_History();
            int totalCoinCount = 0;
            int totalPersonCount = 0;
            DataTable dt = dataAccess.GetMXZOrderCountTable(pageIndex, pageSize, type, startDate, endDate, ref totalPersonCount, ref totalCoinCount, ref recordCount);
            ResponseMxzCountModel result = new ResponseMxzCountModel();
            result.totalPersonCount = totalPersonCount;
            result.totalCoinCount = totalCoinCount;
            result.list = dt;
            result.recordCount = recordCount;
            string strResult = JsonConvert.SerializeObject(result);
            context.Response.Write(strResult);
        }

        /// <summary>
        /// 导出梦想钻日统计数据
        /// </summary>
        /// <param name="context"></param>
        private void ExportMXZCountTable(HttpContext context)
        {
            int type = ConvertHelper.ToInt32(context.Request["type"]);
            string startDate = context.Request["startDate"];
            T_User_MXZ_History dataAccess = new T_User_MXZ_History();
            string endDate = context.Request["endDate"];
            DataTable dt = dataAccess.GetMXZOrderCountTable(type, startDate, endDate);
            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "梦想钻统计记录" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        /// <summary>
        /// 导出梦想钻明细记录
        /// </summary>
        /// <param name="context"></param>
        private void ExportMXZDetail(HttpContext context)
        {
            int type = ConvertHelper.ToInt32(context.Request["type"]);
            string startDate = context.Request["startDate"];
            T_User_MXZ_History dataAccess = new T_User_MXZ_History();
            string endDate = context.Request["endDate"];
            DataTable dt = dataAccess.GetMXZOrderDetails(type, startDate, endDate);
            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "梦想钻明细记录" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
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