using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using StatisticalCenter.Common;
namespace StatisticalCenter.DataAccess.TableMagicBook
{
    /// <summary>
    /// 统计工具资源类
    /// 备注：此处说的“统计资源”，实际上就是图书二维码
    /// </summary>
    public class T_CountToolResource
    {
        /// <summary>
        /// 更新二维码名称
        /// </summary>
        /// <param name="newName"></param>
        /// <param name="ID"></param>
        public void UpdateNewName(string newName, int ID)
        {
            string strSql = $@"
                Update MAGIC_BOOK.DBO.T_CountToolResource
                SET CTRName='{newName}'
                WHERE CTRId={ID}
                ";
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSql); 
        }
        /// <summary>
        /// 获取二维码列表（统计工具资源）
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="status">状态，1正常，0未校验（即待上线），2为校验失败</param>
        /// <param name="resourceName">资源名称（即二维码名称）</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetResourceData(int pageIndex, int pageSize, int status, string resourceName, ref int recordCount)
        {
            string strWhere = string.Empty;
            if (status >= 0)
            {
                strWhere += $"AND CTRStatus = {status}";
            }
            if (!string.IsNullOrEmpty(resourceName))
            {
                strWhere += $" AND CTRName LIKE '%{resourceName.Replace("'", "")}%'";
            }
            if (!string.IsNullOrEmpty(strWhere))
            {
                strWhere = strWhere.TrimStart().TrimStart('A').TrimStart('N').TrimStart('D');
                strWhere = "WHERE " + strWhere;
            }
            string strSql = $@"
                SELECT * FROM (
                    SELECT ROW_NUMBER() OVER(ORDER BY CTRId DESC) AS RowNum
                        ,[CTRId],[CTRName],[CTRUrl],[CTRCode]
                        ,[CTRStatus],[CTRCreateTime],[CTRCreatorId],[CTRCreatorName]
                    FROM {SystemDBConfig.T_CountToolResource} WITH(NOLOCK)
                    {strWhere}) AS A
                WHERE A.RowNum BETWEEN {(pageIndex - 1) * pageSize + 1} AND {pageIndex * pageSize};
                SELECT COUNT(1) AS AllCount
                FROM {SystemDBConfig.T_CountToolResource} WITH(NOLOCK)
                {strWhere}
                ";
            DataSet ds = SqlDataAccess.sda.ExecSqlQuery(strSql);
            if(ds==null||ds.Tables.Count<2)
            {
                return null;
            }
            if(ds.Tables[1]!=null&&ds.Tables[1].Rows.Count>0)
            {
                recordCount = ConvertHelper.ToInt32(ds.Tables[1].Rows[0]["AllCount"]);
            }
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取单条二维码信息
        /// </summary>
        /// <param name="resourceId">二维码ID</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="status">状态，1正常，0未校验（即待上线），2为校验失败</param>
        /// <param name="resourceName">资源名称（即二维码名称）</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetResourceDetail(int resourceId)
        {
            if(resourceId<=0)
            {
                return null;
            }
            string strSql = $@"
                SELECT [CTRId],[CTRName],[CTRUrl],[CTRCode],[CTRStatus]
                    ,[CTRCreateTime],[CTRCreatorId],[CTRCreatorName]
                FROM {SystemDBConfig.T_CountToolResource} WITH(NOLOCK) 
                WHERE CTRId={resourceId}";
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            return dt;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="status">1正常，默认，2校验失败</param>
        /// <returns></returns>
        public bool SetValid(int resourceId,int status)
        {
            string strSql = $"UPDATE {SystemDBConfig.T_CountToolResource} SET CTRStatus={status} WHERE CTRId={resourceId}";
            return SqlDataAccess.sda.ExecuteNonQuery(strSql) > 0;
        }

        /// <summary>
        /// 测试验证
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public bool GetTestHistory(int resourceId)
        {
            string strSql = $"SELECT TOP 1 CTHId FROM {SystemDBConfig.T_CountToolHistory} WITH(NOLOCK) WHERE CTH_CTRID = {resourceId} ";
            DataTable dtVisit = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dtVisit != null && dtVisit.Rows.Count > 0)
            {
                strSql = $"UPDATE {SystemDBConfig.T_CountToolResource} SET CTRStatus=1 WHERE CTRId={resourceId}";
                return SqlDataAccess.sda.ExecuteNonQuery(strSql) > 0;
            }
            return false;
        }

        /// <summary>
        /// 校验统计工具资源
        /// </summary>
        /// <returns></returns>
        public bool ValidResource()
        {
            //校验失败，即返回错误信息


            //校验成功，即将状态改为已校验（正常）

            return false;
        }

        /// <summary>
        /// 新增资源
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="weburl">URL</param>
        /// <param name="code">编码</param>
        /// <param name="userId">当前操作人ID</param>
        /// <param name="userName">当前操作人用户名</param>
        /// <returns></returns>
        public bool AddResource(string name, string weburl, string code, int userId, string userName,ref string newCode, ref string errorMsg)
        {
            if (string.IsNullOrEmpty(name))
            {
                errorMsg = "名称必须填写";
                return false;
            }
            if (string.IsNullOrEmpty(weburl))
            {
                errorMsg = "网址必须填写";
                return false;
            }
            if (string.IsNullOrEmpty(code))
            {
                errorMsg = "代码必须填写";
                return false;
            }

            if (!weburl.StartsWith("http"))
            {
                weburl = "http://" + weburl;
            }

            //生成统计工具（二维码）
            //接口中应有生成代码的功能
            //传入参数名称、网址、代码
            string strSql = $@"
                INSERT INTO {SystemDBConfig.T_CountToolResource}
                    ([CTRName],[CTRUrl]
                    ,[CTRCode],[CTRStatus]
                    ,[CTRCreateTime],[CTRCreatorId]
                    ,[CTRCreatorName])
                 VALUES
                       ('{name.Replace("'", "")}','{weburl.Replace("'", "")}'
                       ,'{code}',0
                       ,'{DateTime.Now}',{userId}
                       ,'{userName.Replace("'", "")}');
                SELECT @@IDENTITY;";
            try
            {
                DataTable dt= SqlDataAccess.sda.ExecSqlTableQuery(strSql);
                if(dt!=null&&dt.Rows.Count>0)
                {
                    int id = ConvertHelper.ToInt32(dt.Rows[0][0]);
                    if(id<=0)
                    {
                        errorMsg = "请稍后再试";
                        return false;
                    }

                    string encryId = MXREncryption.MXR(id.ToString(), true);
                    newCode = "<script>window.onload = function() { var hm = document.createElement(\"script\"); hm.src = \"" + System.Configuration.ConfigurationManager.AppSettings["ValidJsUrl"] + "?q=" + encryId + "&b=testButton\"; hm.id = \"tj\"; var s = document.getElementsByTagName(\"script\")[0]; s.parentNode.insertBefore(hm, s);}</script>";

                    strSql = $"UPDATE {SystemDBConfig.T_CountToolResource} SET CTRCode='{newCode}' WHERE CTRId={id}";
                    SqlDataAccess.sda.ExecuteNonQuery(strSql);
                    return true;
                }

                errorMsg = "请稍后再试";
                return false;
            }
            catch (Exception ex)
            {
                errorMsg = "出现异常，" + ex.Message;
            }
            return false;
        }


        /// <summary>
        /// 获取区域分布统计
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable GetAreaCoutTable(int resourceId, string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            string strWhere = $" WHERE CTHVisitDate BETWEEN '{startDate}' AND '{endDate}' ";
            if (resourceId > 0)
            {
                strWhere += $" AND CTH_CTRId={resourceId} ";
            }

            string strSql = $@"
                SELECT CTHProvince, CTHType, COUNT(1) AS AllCount,COUNT(DISTINCT CTHIp) AS PersonCount
                FROM {SystemDBConfig.T_CountToolHistory}
                {strWhere}
                GROUP BY CTHProvince, CTHType
                ORDER BY CTHProvince";
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            string curProvince = "-1";
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("CTHProvince", typeof(string));
            dtResult.Columns.Add("CTHBrowseCount", typeof(int));
            dtResult.Columns.Add("CTHPersonCount", typeof(int));
            dtResult.Columns.Add("CTHDownLoadCount", typeof(int));
            dtResult.Columns.Add("CTHPersonDownLoadCount", typeof(int));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["CTHProvince"].ToString() != curProvince)
                {
                    DataRow drNew = dtResult.NewRow();
                    drNew["CTHProvince"] = dt.Rows[i]["CTHProvince"].ToString();
                    drNew["CTHBrowseCount"] = 0;
                    drNew["CTHPersonCount"] = 0;
                    drNew["CTHDownLoadCount"] = 0;
                    drNew["CTHPersonDownLoadCount"] = 0;
                    dtResult.Rows.Add(drNew);
                    curProvince = dt.Rows[i]["CTHProvince"].ToString();
                }

                switch (ConvertHelper.ToInt32(dt.Rows[i]["CTHType"]))
                {
                    case 1:
                        dtResult.Rows[dtResult.Rows.Count - 1]["CTHBrowseCount"] = ConvertHelper.ToInt32(dt.Rows[i]["AllCount"]);
                        dtResult.Rows[dtResult.Rows.Count - 1]["CTHPersonCount"] = ConvertHelper.ToInt32(dt.Rows[i]["PersonCount"]);
                        break;
                    case 3:
                        dtResult.Rows[dtResult.Rows.Count - 1]["CTHDownLoadCount"] = ConvertHelper.ToInt32(dt.Rows[i]["AllCount"]);
                        dtResult.Rows[dtResult.Rows.Count - 1]["CTHPersonDownLoadCount"] = ConvertHelper.ToInt32(dt.Rows[i]["PersonCount"]);
                        break;
                }
            }
            DataView dv = dtResult.DefaultView;
            dv.Sort = "CTHPersonCount DESC";
            dtResult = dv.ToTable();
            return dtResult;
        }        
        /// <summary>
        /// 获取区域统计数据
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable GetExportAreaData(int resourceId, string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            string strWhere = $" WHERE CTHVisitDate BETWEEN '{startDate}' AND '{endDate}' ";
            if (resourceId > 0)
            {
                strWhere += $" AND CTH_CTRId={resourceId} ";
            }
            string strSql = $@"
                SELECT CTHProvince,CTHCity, CTHType, ct.CTRName,ct.CTRId,COUNT(1) AS AllCount,COUNT(DISTINCT hi.CTCookie) AS PersonCount
                FROM {SystemDBConfig.T_CountToolHistory} hi WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.T_CountToolResource} ct WITH(NOLOCK) ON ct.CTRId = hi.CTH_CTRId
                {strWhere}
                GROUP BY ct.CTRId,hi.CTHProvince,CTHCity,hi.CTHType,ct.CTRName
                ORDER BY ct.CTRId,CTHProvince,CTHCity";
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if(dt==null||dt.Rows.Count==0)
            {
                return null;
            }
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("二维码名称", typeof(string));
            dtResult.Columns.Add("省份", typeof(string));
            dtResult.Columns.Add("城市", typeof(string));
            dtResult.Columns.Add("扫码次数", typeof(int));
            dtResult.Columns.Add("扫码人数", typeof(int));
            dtResult.Columns.Add("下载次数", typeof(int));
            dtResult.Columns.Add("下载人数", typeof(int));
            dtResult.Columns.Add("CTRId", typeof(int));
            string curProvince = string.Empty;
            int curResourceId = 0;
            string curCity = string.Empty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["CTHProvince"].ToString() != curProvince||ConvertHelper.ToInt32(dt.Rows[i]["CTRId"])!=curResourceId||dt.Rows[i]["CTHCity"].ToString()!=curCity)
                {
                    DataRow drNew = dtResult.NewRow();
                    drNew["CTRId"] = ConvertHelper.ToInt32(dt.Rows[i]["CTRId"]);
                    drNew["二维码名称"] = dt.Rows[i]["CTRName"].ToString();
                    drNew["省份"] = dt.Rows[i]["CTHProvince"].ToString();
                    drNew["城市"] = dt.Rows[i]["CTHCity"].ToString();
                    drNew["扫码次数"] = 0;
                    drNew["扫码人数"] = 0;
                    drNew["下载次数"] = 0;
                    drNew["下载人数"] = 0;
                    dtResult.Rows.Add(drNew);
                    curProvince = dt.Rows[i]["CTHProvince"].ToString();
                    curResourceId = ConvertHelper.ToInt32(dt.Rows[i]["CTRId"]);
                }

                switch (ConvertHelper.ToInt32(dt.Rows[i]["CTHType"]))
                {
                    case 1:
                        dtResult.Rows[dtResult.Rows.Count - 1]["扫码次数"] = ConvertHelper.ToInt32(dt.Rows[i]["AllCount"]);
                        dtResult.Rows[dtResult.Rows.Count - 1]["扫码人数"] =  ConvertHelper.ToInt32(dt.Rows[i]["PersonCount"]);
                        break;
                    case 3:
                        dtResult.Rows[dtResult.Rows.Count - 1]["下载次数"] = ConvertHelper.ToInt32(dt.Rows[i]["AllCount"]);
                        dtResult.Rows[dtResult.Rows.Count - 1]["下载人数"] = ConvertHelper.ToInt32(dt.Rows[i]["PersonCount"]);
                        break;
                }
            }
            DataView dv = dtResult.DefaultView;
            dv.Sort = "CTRId ASC,省份 ASC,扫码人数 DESC";
            dtResult = dv.ToTable();
            dtResult.Columns.Remove("CTRId");
            return dtResult;
        }

        /// <summary>
        /// 获取所有的资源，用于制作下拉列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetResourceValuePair()
        {
            string strSql = $"SELECT CTRId, CTRName FROM {SystemDBConfig.T_CountToolResource} ORDER BY CTRId";
            return SqlDataAccess.sda.ExecSqlTableQuery(strSql);
        }

        /// <summary>
        /// 渠道统计分页查询
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="resourceId"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable GetWayCountData(int pageIndex,int pageSize,string startDate,string endDate,int resourceId,ref int recordCount)
        {
            //二维码下载渠道统计分页查询
            //传入二维码ID、startDate\endDate、pageIndex、pageSize
            //查询列：序号、二维码名称、扫码次数、扫码人数、下载次数
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            string strWhere = $" WHERE ctc.CTCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            string strHistoryWhere = $" WHERE his.CTHVisitDate BETWEEN '{startDate}' AND '{endDate}' ";
            if (resourceId > 0)
            {
                strWhere += $" AND ctc.CTC_CTRId={resourceId} ";
                strHistoryWhere += $" AND his.CTH_CTRId={resourceId} ";
            }

            string strSql = $@"
                SELECT * FROM (
                SELECT ROW_NUMBER() OVER(ORDER BY ctr.CTRId) RowNum
	                ,ctr.CTRId,ctr.CTRName
	                ,SUM(ctc.CTCBrowseCount) AS CTCBrowseCount
	                --,SUM(ctc.CTCBrowsePersonCount) AS CTCBrowsePersonCount
	                ,SUM(ctc.CTCDownloadCount) AS CTCDownloadCount
                 FROM {SystemDBConfig.T_CountToolCount} ctc WITH(NOLOCK)
	                INNER JOIN {SystemDBConfig.T_CountToolResource} ctr WITH(NOLOCK) ON ctr.CTRId=ctc.CTC_CTRId
                {strWhere}
                GROUP BY ctr.CTRId,ctr.CTRName) AS A
                WHERE A.RowNum BETWEEN {(pageIndex-1)*pageSize+1} AND {pageIndex*pageSize};
                SELECT COUNT(1) AS AllCount
                FROM(
                    SELECT COUNT(1) AS AllCount
                    FROM {SystemDBConfig.T_CountToolCount} ctc WITH(NOLOCK)
	                    INNER JOIN {SystemDBConfig.T_CountToolResource} ctr WITH(NOLOCK) ON ctr.CTRId=ctc.CTC_CTRId
                    {strWhere}
                    GROUP BY ctr.CTRId,ctr.CTRName) AS A";
            DataSet ds = SqlDataAccess.sda.ExecSqlQuery(strSql);
            if(ds==null||ds.Tables.Count<2||ds.Tables[0]==null||ds.Tables[0].Rows.Count==0)
            {
                return null;
            }
            ds.Tables[0].Columns.Add("CTCPersonBrowseCount", typeof(int));
            ds.Tables[0].Columns.Add("CTCPersonDownloadCount", typeof(int));
            string strSourceId = "";
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dr["CTCPersonBrowseCount"] = 0;
                dr["CTCPersonDownloadCount"] = 0;
                strSourceId += dr["CTRId"] + ",";
            }
            strSourceId = strSourceId.TrimEnd(',');

            if (resourceId > 0)//外界传入RESOURCEID
            {
                strSql = $@"
                    SELECT his.CTH_CTRId,his.CTHType,COUNT(DISTINCT his.[CTCookie]) AS AllCount
                    FROM {SystemDBConfig.T_CountToolHistory} his
                    {strHistoryWhere}
                    GROUP BY CTH_CTRId,his.CTHType";
                                }
            else
            {
                strSql = $@"
                    SELECT his.CTH_CTRId,his.CTHType,COUNT(DISTINCT his.[CTCookie]) AS AllCount
                    FROM {SystemDBConfig.T_CountToolHistory} his
                    {strHistoryWhere} AND his.CTH_CTRId IN ({strSourceId})
                    GROUP BY his.CTH_CTRId,his.CTHType";
            }
            DataTable dtPerson = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if(dtPerson!=null&&dtPerson.Rows.Count>0)
            {
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow[] drArrPerson = dtPerson.Select($"CTH_CTRId={dr["CTRId"]} ");
                    if(drArrPerson!=null&&drArrPerson.Length>0)
                    {
                        foreach(DataRow drPerson in drArrPerson)
                        {
                            if(ConvertHelper.ToInt32( drPerson["CTHType"])==1)
                            {
                                dr["CTCPersonBrowseCount"] = ConvertHelper.ToInt32(drPerson["AllCount"]);
                            }
                            else
                            {
                                dr["CTCPersonDownloadCount"] = ConvertHelper.ToInt32(drPerson["AllCount"]);
                            }
                        }
                    }
                }
            }
            if (ds.Tables[1]!=null&&ds.Tables[1].Rows.Count>0)
            {
                recordCount = ConvertHelper.ToInt32(ds.Tables[1].Rows[0]["AllCount"]);
            }
            return ds.Tables[0];
        }

        /// <summary>
        /// 渠道统计数据导出
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public DataTable GetWayCountExport(string startDate, string endDate, int resourceId)
        {
            //二维码下载渠道统计导出
            //传入二维码ID、startDate\endDate、pageIndex、pageSize
            //查询列：序号、二维码名称、扫码次数、扫码人数、下载次数
            //排序列：二维码ID、二维码名称
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            string strWhere = $" WHERE ctc.CTCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            string strHistoryWhere = $" WHERE his.CTHVisitDate BETWEEN '{startDate}' AND '{endDate}' ";
            if (resourceId > 0)
            {
                strWhere += $" AND ctc.CTC_CTRId={resourceId} ";
                strHistoryWhere += $" AND his.CTRId={resourceId} ";
            }

            string strSql = $@"
                 SELECT ctr.CTRId
                    ,ctr.CTRName AS 二维码名称
                    ,ctc.CTCCountDate AS 时间
	                ,ctc.CTCBrowseCount AS 扫码次数
	                ,ctc.CTCDownloadCount AS 下载次数
                 FROM {SystemDBConfig.T_CountToolCount} ctc WITH(NOLOCK)
	                INNER JOIN {SystemDBConfig.T_CountToolResource} ctr WITH(NOLOCK) ON ctr.CTRId=ctc.CTC_CTRId
                {strWhere}
                ORDER BY ctr.CTRId,ctc.CTCCountDate DESC";
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if(dt==null||dt.Rows.Count==0)
            {
                return null;
            }


            dt.Columns.Add("扫码人数", typeof(int));
            dt.Columns.Add("下载人数", typeof(int));
            string strSourceId = "";
            foreach (DataRow dr in dt.Rows)
            {
                dr["扫码人数"] = 0;
                dr["下载人数"] = 0;
                strSourceId += dr["CTRId"] + ",";
            }
            strSourceId = strSourceId.TrimEnd(',');

            if (resourceId > 0)//外界传入RESOURCEID
            {
                strSql = $@"
                    SELECT his.CTH_CTRId,his.CTHType,his.CTHVisitDate,COUNT(DISTINCT his.[CTCookie]) AS AllCount
                    FROM {SystemDBConfig.T_CountToolHistory} his
                    {strHistoryWhere}
                    GROUP BY CTH_CTRId,his.CTHType,his.CTHVisitDate";
            }
            else
            {
                strSql = $@"
                    SELECT his.CTH_CTRId,his.CTHType,his.CTHVisitDate,COUNT(DISTINCT his.[CTCookie]) AS AllCount
                    FROM {SystemDBConfig.T_CountToolHistory} his
                    {strHistoryWhere} AND his.CTH_CTRId IN ({strSourceId})
                    GROUP BY his.CTH_CTRId,his.CTHType,his.CTHVisitDate";
            }
            DataTable dtPerson = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dtPerson != null && dtPerson.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow[] drArrPerson = dtPerson.Select($"CTH_CTRId={dr["CTRId"]} AND CTHVisitDate='{dr["时间"]}' ");
                    if (drArrPerson != null && drArrPerson.Length > 0)
                    {
                        foreach (DataRow drPerson in drArrPerson)
                        {
                            if (ConvertHelper.ToInt32(drPerson["CTHType"]) == 1)
                            {
                                dr["扫码人数"] = ConvertHelper.ToInt32(drPerson["AllCount"]);
                            }
                            else
                            {
                                dr["下载人数"] = ConvertHelper.ToInt32(drPerson["AllCount"]);
                            }
                        }
                    }
                }
            }
            dt.Columns.Remove("CTRId");
            return dt;
        }
    }
}
