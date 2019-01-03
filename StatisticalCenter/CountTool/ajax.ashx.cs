using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using StatisticalCenter.Model;
using StatisticalCenter.Common;
using StatisticalCenter.DataAccess.TableMagicBook;
using Newtonsoft.Json;
using System.Web.SessionState;
using System.IO;
namespace StatisticalCenter.CountTool
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
                case "GetResourceData": GetResourceData(context); break;
                case "GetResourceDetail": GetResourceDetail(context); break;
                case "GetValidUrl": GetValidUrl(context); break;
                case "AddResource": AddResource(context); break;
                case "GetAreaCoutTable": GetAreaCoutTable(context); break;
                case "GetExportAreaData": GetExportAreaData(context); break;
                case "GetWayCountData": GetWayCountData(context); break;
                case "GetWayCountExport": GetWayCountExport(context); break;
                case "GetResourceValuePair": GetResourceValuePair(context); break;
                //case "CreateResourceCode": CreateResourceCode(context); break;
                case "GetValidResult": GetValidResult(context);break;
                case "UpdateNewName": UpdateNewName(context);break;
                case "ValidResource": ValidResource(context);break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void UpdateNewName(HttpContext context)
        {
            string newName = context.Request["Name"].ToString();
            int ID = ConvertHelper.ToInt32(context.Request["ID"]);
            T_CountToolResource DataAccess = new T_CountToolResource();
            DataAccess.UpdateNewName(newName, ID);
        }
        /// <summary>
        /// 获取二维码列表（统计工具资源）
        /// </summary>
        /// <returns></returns>
        public void GetResourceData(HttpContext context)
        {
            int pageIndex = ConvertHelper.ToInt32(context.Request["pageIndex"]);
            int pageSize = ConvertHelper.ToInt32(context.Request["pageSize"]);
            int status = ConvertHelper.ToInt32(context.Request["status"]);
            string resourceName = context.Request["resourceName"];
            int recordCount = 0;
            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetResourceData(pageIndex, pageSize, status, resourceName, ref recordCount);
            BaseResponse result = new BaseResponse();
            result.recordCount = recordCount;
            result.list = dt;
            string strResult = JsonConvert.SerializeObject(result);

            context.Response.Write(strResult);
        }

        /// <summary>
        /// 获取所有的资源，用于制作下拉列表
        /// </summary>
        /// <returns></returns>
        public void GetResourceValuePair(HttpContext context)
        {
            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetResourceValuePair();
            string strResult = JsonConvert.SerializeObject(dt);
            context.Response.Write(strResult);
        }

        /// <summary>
        /// 获取单条二维码信息
        /// </summary>
        /// <param name="context"></param>
        public void GetResourceDetail(HttpContext context)
        {
            int resourceId = ConvertHelper.ToInt32(context.Request["resourceId"]);
            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetResourceDetail(resourceId);
            BaseResponse result = new BaseResponse();
            result.recordCount = dt == null ? 0 : 1;
            result.list = dt;
            string strResult = JsonConvert.SerializeObject(result);
            context.Response.Write(strResult);
        }

        ///// <summary>
        ///// 创建新码
        ///// </summary>
        ///// <param name="context"></param>
        //public void CreateResourceCode(HttpContext context)
        //{
        //    string newCode = "<script>window.onload = function() { var hm = document.createElement(\"script\"); hm.src = \"JavaScript.js?q=" + encryId + "&b=testButton\"; hm.id = \"tj\"; var s = document.getElementsByTagName(\"script\")[0]; s.parentNode.insertBefore(hm, s);}</script>";
        //    context.Response.Write(newCode);
        //}

        /// <summary>
        /// 校验统计工具资源
        /// </summary>
        /// <returns></returns>
        public void GetValidUrl(HttpContext context)
        {
            int id = ConvertHelper.ToInt32(context.Request["resourceId"]);
            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetResourceDetail(id);
            if(dt==null)
            {
                return;
            }
            string strResult = $"<html><head><title>校验{dt.Rows[0]["CTRName"].ToString()}</title>{dt.Rows[0]["CTRCode"].ToString()} <script>setTimeout('window.close();', 10000);</script></head><body></body></html>";
            string name = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".html";
            string url = "/datafiles/countToolPage/" + name;
            string location = context.Server.MapPath("/datafiles/countToolPage/" + name);
            File.AppendAllText(strResult, location);
            context.Response.Write(url);
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="context"></param>
        public void GetValidResult(HttpContext context)
        {
            int resourceId = ConvertHelper.ToInt32(context.Request["resourceId"]);
            T_CountToolResource dataAccess = new T_CountToolResource();
            if (dataAccess.GetTestHistory(resourceId))
            {
                context.Response.Write("1");
            }
            else
            {
                context.Response.Write("0");
            }
        }

        /// <summary>
        /// 新增资源(二维码)
        /// </summary>
        /// <returns></returns>
        public void AddResource(HttpContext context)
        {
            string name = context.Request["name"];
            string weburl = context.Request["weburl"];
            string code = context.Request["code"];
            BasePage.BasePage bp = new BasePage.BasePage();
            int userId = bp.userId;
            string userName = bp.userName;
            string errorMsg = string.Empty;
            T_CountToolResource dataAccess = new T_CountToolResource();
            string strNewCode = string.Empty;//生成后的新代码
            if (dataAccess.AddResource(name, weburl, code, userId, userName,ref strNewCode, ref errorMsg))
            {
                context.Response.Write(strNewCode);
            }
            else
            {
                context.Response.Write("生成失败："+ errorMsg);
            }
        }

        /// <summary>
        /// 获取区域分布统计
        /// </summary>
        /// <returns></returns>
        public void GetAreaCoutTable(HttpContext context)
        {
            int resourceId = ConvertHelper.ToInt32(context.Request["resourceId"]);
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetAreaCoutTable(resourceId,startDate,endDate);

            BaseResponse result = new BaseResponse();
            result.recordCount = dt == null ? 0 : dt.Rows.Count;
            result.list = dt;
            string strResult = JsonConvert.SerializeObject(result);
            context.Response.Write(strResult);
        }

        /// <summary>
        ///  导出区域统计数据
        /// </summary>
        /// <returns></returns>
        public void GetExportAreaData(HttpContext context)
        {
            int resourceId =ConvertHelper.ToInt32( context.Request["resourceId"]);
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];

            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetExportAreaData(resourceId, startDate, endDate);
            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "区域统计数据" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        /// <summary>
        /// 渠道统计分页查询
        /// </summary>
        /// <returns></returns>
        public void GetWayCountData(HttpContext context)
        {
            int pageIndex = ConvertHelper.ToInt32(context.Request["pageIndex"]);
            int pageSize = ConvertHelper.ToInt32(context.Request["pageSize"]);
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            int resourceId = ConvertHelper.ToInt32(context.Request["resourceId"]);
            int recordCount = 0;

            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetWayCountData(pageIndex,pageSize,startDate,endDate, resourceId,ref recordCount);
            BaseResponse result = new BaseResponse();
            result.recordCount = dt == null ? 0 : dt.Rows.Count;
            result.list = dt;
            string strResult = JsonConvert.SerializeObject(result);
            context.Response.Write(strResult);         
        }

        /// <summary>
        /// 渠道统计数据导出
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public void GetWayCountExport(HttpContext context)
        {
            string startDate = context.Request["startDate"];
            string endDate = context.Request["endDate"];
            int resourceId = ConvertHelper.ToInt32(context.Request["resourceId"]);

            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetWayCountExport(startDate, endDate, resourceId);
            BaseResponse result = new BaseResponse();
            result.recordCount = dt == null ? 0 : dt.Rows.Count;
            result.list = dt;
            string strResult = JsonConvert.SerializeObject(result);
            context.Response.Write(strResult);

            BasePage.ExcelHelper helper = new BasePage.ExcelHelper();
            helper.ExportDataGridToCSV(dt, "渠道统计数据"+DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="context"></param>
        public void ValidResource(HttpContext context)
        {
            HttpHelper hh = new HttpHelper();
            T_CountToolResource dataAccess = new T_CountToolResource();
            DataTable dt = dataAccess.GetResourceDetail(ConvertHelper.ToInt32(context.Request["resourceId"]));
            if (dt == null || dt.Rows.Count == 0)
            {
                context.Response.Write("资源不存在");
                return;
            }
            string url = dt.Rows[0]["CTRUrl"].ToString();
            string strResult = hh.HttpGet(url);
            int index = strResult.IndexOf("Scripts/DoStatistical.js?q=");
            if(index<0)
            {
                context.Response.Write("校验失败，请确认代码是否已经加入");
                return;
            }
            index+= ("Scripts/DoStatistical.js?q=").Length;
            strResult = strResult.Substring(index, 20);
            strResult = strResult.Split('&')[0];
            strResult = MXR.Utilities.Security.Cryptography.Decryption.MXR(strResult);
            if(ConvertHelper.ToInt32(strResult)==ConvertHelper.ToInt32(context.Request["resourceId"]))
            {
                dataAccess.SetValid(ConvertHelper.ToInt32(context.Request["resourceId"]), 1);
                context.Response.Write("1");
            }
            else
            {
                context.Response.Write("校验失败，代码加入有误，请重新尝试加入");
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