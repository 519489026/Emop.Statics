using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using StatisticalCenter.Common;
namespace StatisticalCenter.DataAccess.MultiTable
{
    /// <summary>
    /// 图书统计相关逻辑
    /// </summary>
    public class BookCountAccess
    {
        #region 图书购买统计
        /// <summary>
        /// 获取图书购买统计
        /// </summary>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBuyBookCountData(int pageIndex,int pageSize, int pressId,string bookGuid
            ,string startDate,string endDate,int userId,ref int recordCount,ref int totalMxzCount ,ref int totalMxbCount)
        {
            string strWhere = $" WHERE dhc.DHCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            if(pressId>0)
            {
                strWhere += $" AND dmb.Press={pressId} ";
            }
            if(!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGUID='{bookGuid.Replace("'", "")}' ";
            }

            string strSql = $@"
                SELECT * FROM (
                    SELECT ROW_NUMBER() OVER(ORDER BY SUM(dhc.DHCRowCount+dhc.DHCMxzRowCount) DESC) AS RowNum
                        ,dmb.BookGUID,dmb.BookName,ISNULL(pr.PressName,'无') AS PressName
                        ,SUM(dhc.DHCCoinCount) CoinCount,SUM(dhc.DHCMxzCount) AS MxzCount
                        ,SUM(dhc.DHCRowCount+dhc.DHCMxzRowCount) AS BuyCount
                    FROM {SystemDBConfig.T_DevicePurchase_History_Count} dhc WITH(NOLOCK)
	                    INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID=dhc.DHCBookGuid COLLATE Chinese_PRC_CI_AS
                        INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}
	                    INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON pr.PressID=dmb.Press
                    {strWhere}
                    GROUP BY dmb.BookGUID,dmb.BookName,pr.PressName) AS A
                WHERE RowNum BETWEEN {(pageIndex - 1) * pageSize + 1} AND {pageIndex * pageSize};
                
                SELECT COUNT(1) AS AllCount FROM(
                SELECT dmb.BookGUID
                FROM {SystemDBConfig.T_DevicePurchase_History_Count} dhc WITH(NOLOCK)
	                INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID=dhc.DHCBookGuid COLLATE Chinese_PRC_CI_AS
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}
	                INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON pr.PressID=dmb.Press
                {strWhere}
                GROUP BY dmb.BookGUID) AS A 

                SELECT SUM(dhc.DHCCoinCount) CoinCount,SUM(dhc.DHCMxzCount) AS MxzCount
                FROM {SystemDBConfig.T_DevicePurchase_History_Count} dhc WITH(NOLOCK)
	                INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID=dhc.DHCBookGuid COLLATE Chinese_PRC_CI_AS
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}
	                INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON pr.PressID=dmb.Press 

                    {strWhere} ";
            DataSet ds = SqlDataAccess.sda.ExecSqlQuery(strSql);
            if(ds==null||ds.Tables.Count<3)
            {
                return null;
            }
            if(ds.Tables[1]!=null&&ds.Tables[1].Rows.Count>0)
            {
                recordCount = ConvertHelper.ToInt32(ds.Tables[1].Rows[0]["AllCount"]);
            }
            if(ds.Tables[2]!=null&&ds.Tables[2].Rows.Count>0)
            {
                totalMxbCount = ConvertHelper.ToInt32(ds.Tables[2].Rows[0]["CoinCount"]);
                totalMxzCount = ConvertHelper.ToInt32(ds.Tables[2].Rows[0]["MxzCount"]);
            }
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取图书购买统计的导出数据
        /// </summary>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBuyBookExportData(int pressId, string bookGuid, string startDate, string endDate, int userId)
        {
            string strWhere = $" WHERE dhc.DHCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            if (pressId > 0)
            {
                strWhere += $" AND dmb.Press={pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGUID='{bookGuid.Replace("'", "")}' ";
            }

            string strSql = $@"
                    SELECT dmb.BookName AS 图书名称
                    ,ISNULL(pr.PressName,'无') AS 出版社
                    ,SUM(dhc.DHCRowCount+dhc.DHCMxzRowCount) AS 购买量
                    ,SUM(dhc.DHCCoinCount) AS 梦想币
                    ,SUM(dhc.DHCMxzCount) AS 梦想钻     
                FROM {SystemDBConfig.T_DevicePurchase_History_Count} dhc WITH(NOLOCK)
	                INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID=dhc.DHCBookGuid COLLATE Chinese_PRC_CI_AS
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}
	                INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON pr.PressID=dmb.Press AND pr.PressID>0

                {strWhere}
                GROUP BY dmb.BookGUID,dmb.BookName,pr.PressName
                ORDER BY SUM(dhc.DHCRowCount+dhc.DHCMxzRowCount) DESC";
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            return dt;
        }

        /// <summary>
        /// 获取图书购买统计的明细导出数据
        /// </summary>
        /// <param name="publishId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBuyBookExportDetailData(int pressId, string bookGuid, string startDate, string endDate, int userId)
        {
            string strWhere = $" WHERE hi.F_CreateTime BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1)}' AND hi.F_PurchaseType IN(1,4) ";
            if (pressId > 0)
            {
                strWhere += $" AND dmb.Press={pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGUID='{bookGuid.Replace("'", "")}' ";
            }

            string strSql = $@"
            SELECT ISNULL(hi.F_DeviceId,'') AS 设备ID
                ,ISNULL(hi.F_CreateTime,'') AS 时间
	            ,ISNULL(dmb.BookName,'') AS 图书名称
	            ,ISNULL(pr.PressName,'') AS 出版社
	            ,ISNULL(ui.userID,0) AS 用户ID
	            ,ISNULL(ui.userNickName,'') AS 用户名称
	            ,(CASE hi.F_PurchaseType WHEN 1 THEN '梦想币' ELSE '梦想钻' END)  AS 货币类型
                , ISNULL(hi.F_UsedCoinNum,0) AS 金额
             FROM {SystemDBConfig.T_DevicePurchase_History} hi WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.T_User_Device_List} udl WITH(NOLOCK) ON udl.F_DeviceID = hi.F_DeviceID
                INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON hi.F_PurchaseContent = dmb.BookGUID COLLATE Chinese_PRC_90_CI_AS
                INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}
                LEFT JOIN {SystemDBConfig.UserInfo} ui WITH(NOLOCK) ON ui.userID = udl.F_UserID
	            INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON pr.PressID=dmb.Press

                {strWhere}
            ORDER BY  hi.F_CreateTime DESC";

            return SqlDataAccess.sda.ExecSqlTableQuery(strSql);
        }
        #endregion


        #region 图书下载统计
        /// <summary>
        /// 获取图书下载统计
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页长</param>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBookDownloadCountData(int pageIndex, int pageSize, int pressId, string bookGuid
            , string startDate, string endDate, int userId, ref int recordCount, ref int totalDownLoadCount, ref int totalPersonCount)
        {
            string strWhere = $" WHERE dbc.DBCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            string strRecordWhere = $" WHERE [time] BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1)}' ";
            if (pressId > 0)
            {
                strWhere += $" AND dmb.Press={pressId} ";
                strRecordWhere += $" AND dmb.Press={pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGUID='{bookGuid.Replace("'", "")}' ";
                strRecordWhere += $" AND dmb.BookGUID='{bookGuid.Replace("'", "")}' ";
            }
            string strSql = $@"
                SELECT* FROM (
                    SELECT ROW_NUMBER() OVER(ORDER BY SUM(dbc.DBCCount) DESC) AS RowNum
                        , dmb.BookName,dmb.BookGUID, ISNULL(pr.PressName, '无') AS PressName
                        , SUM(dbc.DBCCount) AS AllCount
                        ,0 AS PersonCount
                        ,0 AS RegistePersonCount

                        --, SUM(dbc.DBCNoRegisterPersonCount+dbc.DBCRegisterPersonCount) AS PersonCount
                        --, SUM(dbc.DBCRegisterPersonCount) AS RegistePersonCount
                    FROM {SystemDBConfig.download_book_count} dbc WITH(NOLOCK)            
                        INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID COLLATE Chinese_PRC_90_CI_AS = dbc.DBCBookGuid  
                        INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}          
	                    INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON pr.PressID=dmb.Press

                    {strWhere}         
                        GROUP BY dmb.BookName,dmb.BookGUID,pr.PressName) AS A
                    WHERE A.RowNum BETWEEN {(pageIndex - 1) * pageSize + 1} AND {pageIndex * pageSize};

                    SELECT COUNT(1) AS AllCount FROM(
                    SELECT dmb.BookGUID
                    FROM {SystemDBConfig.download_book_count} dbc WITH(NOLOCK)            
                        INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID COLLATE Chinese_PRC_90_CI_AS = dbc.DBCBookGuid    
                        INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}  
                    {strWhere}   
                    GROUP BY dmb.BookGuid) AS A;

                    SELECT COUNT(DISTINCT db.userId) AS RegisterPersonCount,COUNT(db.userId) RegisterDownloadCount
                    FROM {SystemDBConfig.download_book} db WITH(NOLOCK)        
                        INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID COLLATE Chinese_PRC_90_CI_AS = db.isbn
                        INNER JOIN {SystemDBConfig.T_AdminPress}  ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}
                    {strRecordWhere} AND db.userId>0;					

                    SELECT COUNT(DISTINCT db.phoneId) AS NoRegisterPersonCount,COUNT(db.phoneId) NoRegisterDownloadCount
                    FROM {SystemDBConfig.download_book} db WITH(NOLOCK)        
                        INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID COLLATE Chinese_PRC_90_CI_AS = db.isbn
                        INNER JOIN {SystemDBConfig.T_AdminPress}  ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}
                    {strRecordWhere} AND db.userId=0";

            DataSet ds = SqlDataAccess.sda.ExecSqlQuery(strSql);
            if (ds == null || ds.Tables.Count < 4)
            {
                return null;
            }
            string bookGuids = string.Empty;//需要获取注册用户、非注册用户下载数据的图书Guid信息
            if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
            {
                recordCount = ConvertHelper.ToInt32(ds.Tables[1].Rows[0]["AllCount"]);
            }
            if(ds.Tables[0]!=null&&ds.Tables[0].Rows.Count>0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    bookGuids += $"'{dr["bookGuid"]}',";
                }
                bookGuids = bookGuids.TrimEnd(',');
                string strSqlBookCount = $@"

                SELECT COUNT(DISTINCT db.userId) AS RegisterPersonCount,db.isbn
                FROM {SystemDBConfig.download_book} db WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID COLLATE Chinese_PRC_90_CI_AS = db.isbn
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press = ap.APPressId AND ap.APUserId = {userId}
                {strRecordWhere} AND db.userId > 0 AND db.isbn IN ({bookGuids})
                GROUP BY db.isbn

                SELECT COUNT(DISTINCT db.phoneId) AS NoRegisterPersonCount,db.isbn
                FROM {SystemDBConfig.download_book} db WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID COLLATE Chinese_PRC_90_CI_AS = db.isbn
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press = ap.APPressId AND ap.APUserId = {userId}
                {strRecordWhere} AND db.userId = 0 AND db.isbn IN ({bookGuids})
                GROUP BY db.isbn";
                DataSet dsBookCount = SqlDataAccess.sda.ExecSqlQuery(strSqlBookCount);
                if (dsBookCount != null && dsBookCount.Tables.Count == 2)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int personCount = 0;
                        if (dsBookCount.Tables[0] != null && dsBookCount.Tables[0].Rows.Count > 0)
                        {
                            DataRow[] drArrCount = dsBookCount.Tables[0].Select($"isbn='{dr["bookGuid"]}'");
                            if (drArrCount != null && drArrCount.Length > 0)
                            {
                                dr["RegistePersonCount"] = ConvertHelper.ToInt32(drArrCount[0]["RegisterPersonCount"]);
                                personCount += ConvertHelper.ToInt32(drArrCount[0]["RegisterPersonCount"]);
                            }
                        }

                        if (dsBookCount.Tables[1] != null && dsBookCount.Tables[1].Rows.Count > 0)
                        {
                            DataRow[] drArrCount = dsBookCount.Tables[1].Select($"isbn='{dr["bookGuid"]}'");
                            if (drArrCount != null && drArrCount.Length > 0)
                            {
                                personCount += ConvertHelper.ToInt32(drArrCount[0]["NoRegisterPersonCount"]);
                            }
                        }
                        dr["PersonCount"] = personCount;
                    }
                }
            }


            int totalRegisterPersonCount = 0;//注册用户总下载人数
            int totalRegisterDownLoadCount = 0;//注册用户总下载次数
            int totalNoRegisterPersonCount = 0;//未注册用户总下载人数
            int totalNoRegisterDownLoadCount = 0;//未册用户总下载次数
            if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
            {
                totalRegisterPersonCount = ConvertHelper.ToInt32(ds.Tables[2].Rows[0]["RegisterPersonCount"]);
                totalRegisterDownLoadCount = ConvertHelper.ToInt32(ds.Tables[2].Rows[0]["RegisterDownloadCount"]);
            }
            if(ds.Tables[3]!=null&&ds.Tables[3].Rows.Count>0)
            {
                totalNoRegisterPersonCount = ConvertHelper.ToInt32(ds.Tables[3].Rows[0]["NoRegisterPersonCount"]);
                totalNoRegisterDownLoadCount = ConvertHelper.ToInt32(ds.Tables[3].Rows[0]["NoRegisterDownloadCount"]);
            }

            totalDownLoadCount = totalNoRegisterDownLoadCount + totalRegisterDownLoadCount;
            totalPersonCount = totalNoRegisterPersonCount + totalRegisterPersonCount;
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取图书下载统计的导出数据
        /// </summary>
        /// <param name="publishId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBookDownloadExportData(int pressId, string bookGuid, string startDate, string endDate, int userId)
        {
            string strWhere = $" WHERE dbc.DBCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            string strRecordWhere = $"WHERE db.[time] BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1)}' ";
            if (pressId > 0)
            {
                strWhere += $" AND dmb.Press={pressId} ";
                strRecordWhere += $" AND dmb.Press={pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGUID='{bookGuid.Replace("'", "")}' ";
                strRecordWhere += $" AND dmb.BookGUID='{bookGuid.Replace("'", "")}' ";
            }
            string strSql = $@"
                SELECT dmb.bookGuid,dmb.BookName AS 图书名称, ISNULL(pr.PressName, '无') AS 出版社
                    , SUM(dbc.DBCCount)AS 下载次数
                    , 0 AS 下载人数
                    , 0 AS 注册用户下载人数
                FROM {SystemDBConfig.download_book_count} dbc WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID COLLATE Chinese_PRC_90_CI_AS = dbc.DBCBookGuid
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}
	                INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON pr.PressID=dmb.Press 
                {strWhere}
                GROUP BY dmb.BookName, dmb.BookGUID, pr.PressName
                ORDER BY SUM(dbc.DBCCount) DESC";
            DataTable dt= SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dt != null && dt.Rows.Count > 0)
            {
                string strSqlBookCount = $@"

                SELECT COUNT(DISTINCT db.userId) AS RegisterPersonCount,db.isbn
                FROM {SystemDBConfig.download_book} db WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID COLLATE Chinese_PRC_90_CI_AS = db.isbn
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press = ap.APPressId AND ap.APUserId = {userId}
                {strRecordWhere} AND db.userId > 0
                GROUP BY db.isbn

                SELECT COUNT(DISTINCT db.phoneId) AS NoRegisterPersonCount,db.isbn
                FROM {SystemDBConfig.download_book} db WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON dmb.BookGUID COLLATE Chinese_PRC_90_CI_AS = db.isbn
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press = ap.APPressId AND ap.APUserId = {userId}
                {strRecordWhere} AND db.userId = 0
                GROUP BY db.isbn";
                DataSet dsBookCount = SqlDataAccess.sda.ExecSqlQuery(strSqlBookCount);
                if (dsBookCount != null && dsBookCount.Tables.Count == 2)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        int personCount = 0;
                        if (dsBookCount.Tables[0] != null && dsBookCount.Tables[0].Rows.Count > 0)
                        {
                            DataRow[] drArrCount = dsBookCount.Tables[0].Select($"isbn='{dr["bookGuid"]}'");
                            if (drArrCount != null && drArrCount.Length > 0)
                            {
                                dr["注册用户下载人数"] = ConvertHelper.ToInt32(drArrCount[0]["RegisterPersonCount"]);
                                personCount += ConvertHelper.ToInt32(drArrCount[0]["RegisterPersonCount"]);
                            }
                        }

                        if (dsBookCount.Tables[1] != null && dsBookCount.Tables[1].Rows.Count > 0)
                        {
                            DataRow[] drArrCount = dsBookCount.Tables[1].Select($"isbn='{dr["bookGuid"]}'");
                            if (drArrCount != null && drArrCount.Length > 0)
                            {
                                personCount += ConvertHelper.ToInt32(drArrCount[0]["NoRegisterPersonCount"]);
                            }
                        }
                        dr["下载人数"] = personCount;
                    }
                }
                dt.Columns.Remove("bookGuid");
            }
            return dt;
        }
        #endregion



        #region 图书内容统计
        /// <summary>
        /// 获取图书内容统计
        /// </summary>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBookContentCountData(int pageIndex, int pageSize, int pressId, string bookGuid, string startDate, string endDate, int userId, ref int recordCount)
        {
            string strWhere = "";
            if (pressId > 0)
            {
                strWhere += $" AND dmb.Press={pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGuid='{bookGuid.Replace("'", "")}'";
            }

            string clickTimeJoin = $" AND chc.CHCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            string ugcTimeJoin = $" AND buc.BUCCountDate BETWEEN '{startDate}' AND '{endDate}' ";


            //1、提取符合条件的离线热点信息
            string strSql = $@"
                SELECT * FROM(
                    SELECT ROW_NUMBER() OVER(ORDER BY OffLineCount DESC) AS RowNum, * FROM (
                        SELECT dmb.BookGuid, dmb.BookName, ISNULL(pr.PressName, '无') AS PressName
                            ,SUM(ISNULL(CHCAudioCount, 0) + ISNULL(CHCCommentCount, 0) + ISNULL(CHCEmptyCount, 0)
                                + ISNULL(CHCImageCount, 0) + ISNULL(CHCModelCount, 0) + ISNULL(CHCUnknowCount, 0)
                                + ISNULL(CHCVideoCount, 0) + ISNULL(CHCWebCount, 0) + ISNULL(CHCWebsiteCount, 0)+ISNULL(CHCXmkhCount,0)) AS OffLineCount
                        FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                            INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}
	                        INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON pr.PressID=dmb.Press AND pr.PressID>0

                            LEFT JOIN {SystemDBConfig.click_hotspot_count} chc WITH(NOLOCK) ON chc.CHCBookGuid = dmb.BookGuid COLLATE Chinese_PRC_90_CI_AS AND chc.CHCIsOnLine = 0 {clickTimeJoin}
                        WHERE 1 = 1 {strWhere}
                        GROUP BY dmb.BookGUID, dmb.BookID, dmb.BookName, pr.PressName) AS R
                    ) AS A
                WHERE A.RowNum BETWEEN {(pageIndex - 1) * pageSize + 1} AND {pageIndex * pageSize};";
            DataTable dtResult = SqlDataAccess.sda.ExecSqlTableQuery(strSql);//最终结果
            if (dtResult == null || dtResult.Rows.Count == 0)
            {
                return null;
            }
            dtResult.Columns.Add("OnLineCount", typeof(int));//在线热点
            dtResult.Columns.Add("XmkhCount", typeof(int));//小梦辅导
            dtResult.Columns.Add("UGCAudioCount", typeof(int));//UGC音频
            dtResult.Columns.Add("UGCVideoCount", typeof(int));//UGC视频
            dtResult.Columns.Add("UGCImageCount", typeof(int));//UGC图片
            dtResult.Columns.Add("UGCWebCount", typeof(int));//UGC网址
            dtResult.Columns.Add("UGCFaceRecordCount", typeof(int));//UGC头像
            foreach (DataRow dr in dtResult.Rows)
            {
                dr["OnLineCount"] = 0;
                dr["XmkhCount"] = 0;
                dr["UGCAudioCount"] = 0;
                dr["UGCVideoCount"] = 0;
                dr["UGCImageCount"] = 0;
                dr["UGCWebCount"] = 0;
                dr["UGCFaceRecordCount"] = 0;
            }

            //2、提取所有记录条数
            strSql = $"SELECT COUNT(1) AS AllCount FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) WHERE 1 = 1 {strWhere}";
            DataTable dtCount = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dtCount != null && dtCount.Rows.Count > 0)
            {
                recordCount = ConvertHelper.ToInt32(dtCount.Rows[0]["AllCount"]);
            }

            //3、构建临时表
            string guidTable = Guid.NewGuid().ToString().Replace("-", "");
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append($@"
                CREATE TABLE #Temp{guidTable}(BookGuid VARCHAR(50));
                CREATE INDEX #Index_Temp{guidTable} ON #Temp{guidTable}(BOOKGUID);");
            foreach (DataRow dr in dtResult.Rows)
            {
                sbSql.Append($"INSERT INTO #Temp{guidTable} VALUES('{dr["BookGUID"].ToString()}');");
            }

            //4、构建UGC音频、视频、图片、网址、头像
            sbSql.Append($@"            
            SELECT tp.BookGuid
                , SUM(BUCAudioCount)AS UGCAudioCount
                , SUM(BUCVideoCount) AS UGCVideoCount
                , SUM(BUCImageCount) AS UGCImageCount
                , SUM(BUCWebCount) AS UGCWebCount
                , SUM(BUCFaceRecordCount) AS UGCFaceRecordCount
            FROM #Temp{guidTable} tp WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.BookUGC_Count} buc WITH(NOLOCK) ON buc.BUCBookGuid = tp.BookGuid {ugcTimeJoin} 
            GROUP BY tp.BookGuid;");

            //5、构建在线热点
            sbSql.Append($@"
                SELECT tp.BookGuid
                    , SUM(ISNULL(CHCAudioCount, 0) + ISNULL(CHCCommentCount, 0) + ISNULL(CHCEmptyCount, 0)
                        + ISNULL(CHCImageCount, 0) + ISNULL(CHCModelCount, 0)   + ISNULL(CHCUnknowCount, 0)
                        + ISNULL(CHCVideoCount, 0) + ISNULL(CHCWebCount, 0)     + ISNULL(CHCWebsiteCount, 0)+ISNULL(CHCXmkhCount,0)) AS OnLineCount
                FROM #Temp{guidTable} tp WITH(NOLOCK)
                    INNER JOIN { SystemDBConfig.click_hotspot_count} chc WITH(NOLOCK) ON chc.CHCBookGuid = tp.BookGuid
                        AND chc.CHCIsOnLine = 1 {clickTimeJoin}
                GROUP BY tp.BookGuid; ");

            //6、构建小梦辅导
            sbSql.Append($@"
            SELECT tp.BookGuid, SUM(ISNULL(chc.CHCXmkhCount, 0)) AS XmkhCount
            FROM #Temp{guidTable} tp WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.click_hotspot_count} chc on chc.CHCBookGuid = tp.BookGuid {clickTimeJoin}
            WHERE chc.CHCIsOnLine = 0
            group by tp.BookGuid;");
            sbSql.Append($"DROP INDEX #Index_Temp{guidTable} ON #Temp{guidTable};DROP TABLE #TEMP{guidTable};");

            DataSet ds = SqlDataAccess.sda.ExecSqlQuery(sbSql.ToString());
            if (ds == null || ds.Tables.Count < 3)
            {
                return dtResult;
            }
            foreach (DataRow drUgc in ds.Tables[0].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drUgc["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["UGCAudioCount"] = ConvertHelper.ToInt32(drUgc["UGCAudioCount"]);
                    drArr[0]["UGCVideoCount"] = ConvertHelper.ToInt32(drUgc["UGCVideoCount"]);
                    drArr[0]["UGCImageCount"] = ConvertHelper.ToInt32(drUgc["UGCImageCount"]);
                    drArr[0]["UGCWebCount"] = ConvertHelper.ToInt32(drUgc["UGCWebCount"]);
                    drArr[0]["UGCFaceRecordCount"] = ConvertHelper.ToInt32(drUgc["UGCFaceRecordCount"]);
                }
            }

            foreach (DataRow drOnline in ds.Tables[1].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drOnline["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["OnLineCount"] = ConvertHelper.ToInt32(drOnline["OnLineCount"]);
                }
            }

            foreach (DataRow drXmkf in ds.Tables[2].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drXmkf["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["XmkhCount"] = ConvertHelper.ToInt32(drXmkf["XmkhCount"]);
                }
            }

            return dtResult;
        }

        /// <summary>
        /// 获取图书内容统计的导出数据
        /// </summary>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBookContentExportData(int pressId, string bookGuid, string startDate, string endDate,int userId)
        {
            string strWhere = "";
            if (pressId > 0)
            {
                strWhere += $" AND dmb.Press={pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGuid='{bookGuid.Replace("'", "")}'";
            }

            string clickTimeJoin = $" AND chc.CHCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            string ugcTimeJoin = $" AND buc.BUCCountDate BETWEEN '{startDate}' AND '{endDate}' ";


            //1、提取符合条件的离线热点信息
            string strSql = $@"
                    SELECT * FROM (
                        SELECT dmb.BookGuid, dmb.BookName AS 图书名称, ISNULL(pr.PressName, '无') AS 出版社
                            ,0 AS 在线热点
                            ,SUM(ISNULL(CHCAudioCount, 0) + ISNULL(CHCCommentCount, 0) + ISNULL(CHCEmptyCount, 0)
                                + ISNULL(CHCImageCount, 0) + ISNULL(CHCModelCount, 0) + ISNULL(CHCUnknowCount, 0)
                                + ISNULL(CHCVideoCount, 0) + ISNULL(CHCWebCount, 0) + ISNULL(CHCWebsiteCount, 0)+ISNULL(CHCXmkhCount,0)) AS 离线热点
                        FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                            INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON dmb.Press=ap.APPressId AND ap.APUserId={userId}
	                        INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON pr.PressID=dmb.Press
                            LEFT JOIN {SystemDBConfig.click_hotspot_count} chc WITH(NOLOCK) ON chc.CHCBookGuid = dmb.BookGuid COLLATE Chinese_PRC_90_CI_AS AND chc.CHCIsOnLine = 0 {clickTimeJoin}
                        WHERE 1 = 1 {strWhere}
                        GROUP BY dmb.BookGUID, dmb.BookID, dmb.BookName, pr.PressName) AS R";
;
            DataTable dtResult = SqlDataAccess.sda.ExecSqlTableQuery(strSql);//最终结果
            if (dtResult == null || dtResult.Rows.Count == 0)
            {
                return null;
            }
            dtResult.Columns.Add("小梦辅导", typeof(int));//小梦辅导
            dtResult.Columns.Add("UGC音频", typeof(int));//UGC音频
            dtResult.Columns.Add("UGC视频", typeof(int));//UGC视频
            dtResult.Columns.Add("UGC图片", typeof(int));//UGC图片
            dtResult.Columns.Add("UGC网址", typeof(int));//UGC网址
            dtResult.Columns.Add("头像视频", typeof(int));//UGC头像
            foreach (DataRow dr in dtResult.Rows)
            {
                dr["小梦辅导"] = 0;
                dr["UGC音频"] = 0;
                dr["UGC视频"] = 0;
                dr["UGC图片"] = 0;
                dr["UGC网址"] = 0;
                dr["头像视频"] = 0;
            }

            //2、构建临时表
            string guidTable = Guid.NewGuid().ToString().Replace("-", "");
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append($@"
                CREATE TABLE #Temp{guidTable}(BookGuid VARCHAR(50));
                CREATE INDEX #Index_Temp{guidTable} ON #Temp{guidTable}(BOOKGUID);");
            foreach (DataRow dr in dtResult.Rows)
            {
                sbSql.Append($"INSERT INTO #Temp{guidTable} VALUES('{dr["BookGUID"].ToString()}');");
            }

            //3、构建UGC音频、视频、图片、网址、头像
            sbSql.Append($@"            
            SELECT tp.BookGuid
                , SUM(BUCAudioCount)AS UGCAudioCount
                , SUM(BUCVideoCount) AS UGCVideoCount
                , SUM(BUCImageCount) AS UGCImageCount
                , SUM(BUCWebCount) AS UGCWebCount
                , SUM(BUCFaceRecordCount) AS UGCFaceRecordCount
            FROM #Temp{guidTable} tp WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.BookUGC_Count} buc WITH(NOLOCK) ON buc.BUCBookGuid = tp.BookGuid {ugcTimeJoin} 
            GROUP BY tp.BookGuid;");

            //4、构建在线热点
            sbSql.Append($@"
                SELECT tp.BookGuid
                    , SUM(ISNULL(CHCAudioCount, 0) + ISNULL(CHCCommentCount, 0) + ISNULL(CHCEmptyCount, 0)
                        + ISNULL(CHCImageCount, 0) + ISNULL(CHCModelCount, 0)   + ISNULL(CHCUnknowCount, 0)
                        + ISNULL(CHCVideoCount, 0) + ISNULL(CHCWebCount, 0)     + ISNULL(CHCWebsiteCount, 0)+ISNULL(CHCXmkhCount,0)) AS OnLineCount
                FROM #Temp{guidTable} tp WITH(NOLOCK)
                    INNER JOIN { SystemDBConfig.click_hotspot_count} chc WITH(NOLOCK) ON chc.CHCBookGuid = tp.BookGuid
                        AND chc.CHCIsOnLine = 1 {clickTimeJoin}
                GROUP BY tp.BookGuid; ");

            //5、构建小梦辅导
            sbSql.Append($@"
            SELECT tp.BookGuid, SUM(ISNULL(chc.CHCXmkhCount, 0)) AS XmkhCount
            FROM #Temp{guidTable} tp WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.click_hotspot_count} chc on chc.CHCBookGuid = tp.BookGuid {clickTimeJoin}
            WHERE chc.CHCIsOnLine = 0
            group by tp.BookGuid;");
            sbSql.Append($"DROP INDEX #Index_Temp{guidTable} ON #Temp{guidTable};DROP TABLE #TEMP{guidTable};");

            DataSet ds = SqlDataAccess.sda.ExecSqlQuery(sbSql.ToString());
            if (ds == null || ds.Tables.Count < 3)
            {
                return dtResult;
            }
            foreach (DataRow drUgc in ds.Tables[0].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drUgc["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["UGC音频"] = ConvertHelper.ToInt32(drUgc["UGCAudioCount"]);
                    drArr[0]["UGC视频"] = ConvertHelper.ToInt32(drUgc["UGCVideoCount"]);
                    drArr[0]["UGC图片"] = ConvertHelper.ToInt32(drUgc["UGCImageCount"]);
                    drArr[0]["UGC网址"] = ConvertHelper.ToInt32(drUgc["UGCWebCount"]);
                    drArr[0]["头像视频"] = ConvertHelper.ToInt32(drUgc["UGCFaceRecordCount"]);
                }
            }

            foreach (DataRow drOnline in ds.Tables[1].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drOnline["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["在线热点"] = ConvertHelper.ToInt32(drOnline["OnLineCount"]);
                }
            }

            foreach (DataRow drXmkf in ds.Tables[2].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drXmkf["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["小梦辅导"] = ConvertHelper.ToInt32(drXmkf["XmkhCount"]);
                }
            }
            dtResult.Columns.Remove("BookGuid");
            DataView dv = dtResult.DefaultView;
            dv.Sort = "离线热点 DESC";
            dtResult = dv.ToTable();
            return dtResult;
        }

        /// <summary>
        /// 获取图书内容统计的明细导出数据
        /// </summary>
        /// <param name="publishId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBookContentExportDetailData(string bookGuid, string startDate, string endDate,int userId)
        {
            string strSql = $@"
                SELECT isOnLine, pageNo, COUNT(1) AS AllCount
                FROM {SystemDBConfig.click_hotspot} chs WITH(NOLOCK)
	                INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON chs.isbn=dmb.BookGUID
	                INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                WHERE chs.bookGuid = '{bookGuid}' AND chs.[time] BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1).ToShortDateString()}'
                GROUP BY isOnLine, pageNo
                ORDER BY pageNo;
                SELECT ugcType, pageNo, COUNT(1) AS AllCount
                FROM {SystemDBConfig.BookUGC} buc WITH(NOLOCK)
	                INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK) ON buc.bookGuid=dmb.BookGUID COLLATE Chinese_PRC_CI_AS
	                INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                WHERE bookguid = '{bookGuid}' AND createTime BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1).ToShortDateString()}'
                GROUP BY ugcType, pageNo";

            DataSet ds = SqlDataAccess.sda.ExecSqlQuery(strSql);
            if (ds == null || ds.Tables.Count < 2)
            {
                return null;
            }
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("页码", typeof(int));
            dtResult.Columns.Add("在线热点", typeof(int));
            dtResult.Columns.Add("离线热点", typeof(int));
            dtResult.Columns.Add("UGC音频", typeof(int));
            dtResult.Columns.Add("UGC视频", typeof(int));
            dtResult.Columns.Add("UGC图片", typeof(int));
            dtResult.Columns.Add("UGC网址", typeof(int));
            dtResult.Columns.Add("头像视频", typeof(int));
            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow[] drArrResult = dtResult.Select($"页码={dr["pageNo"]}");
                    if (drArrResult != null && drArrResult.Length > 0)
                    {
                        if(ConvertHelper.ToInt32( dr["isOnLine"])==0)
                        {
                            drArrResult[0]["离线热点"] = ConvertHelper.ToInt32(dr["AllCount"]);
                        }
                        else
                        {
                            drArrResult[0]["在线热点"] = ConvertHelper.ToInt32(dr["AllCount"]);
                        }
                        continue;
                    }
                    DataRow drResult = dtResult.NewRow();
                    drResult["页码"] = ConvertHelper.ToInt32(dr["pageNo"]);
                    if (ConvertHelper.ToInt32(dr["isOnLine"]) == 0)
                    {
                        drResult["离线热点"] = ConvertHelper.ToInt32(dr["AllCount"]);
                    }
                    else
                    {
                        drResult["在线热点"] = ConvertHelper.ToInt32(dr["AllCount"]);
                    }
                    dtResult.Rows.Add(drResult);
                }
            }

            if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    DataRow[] drArrResult = dtResult.Select($"页码={dr["pageNo"]}");
                    if (drArrResult != null && drArrResult.Length > 0)
                    {
                        switch(dr["ugcType"].ToString())
                        {
                            case "audio":drArrResult[0]["UGC音频"] = ConvertHelper.ToInt32(dr["AllCount"]);break;
                            case "image": drArrResult[0]["UGC图片"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                            case "video": drArrResult[0]["UGC视频"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                            case "faceRecord": drArrResult[0]["头像视频"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                            case "website": drArrResult[0]["UGC网址"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                        }
                        continue;
                    }
                    DataRow drResult = dtResult.NewRow();
                    drResult["页码"] = ConvertHelper.ToInt32(dr["pageNo"]);
                    switch (dr["ugcType"].ToString())
                    {
                        case "audio": drResult["UGC音频"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                        case "image": drResult["UGC图片"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                        case "video": drResult["UGC视频"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                        case "faceRecord": drResult["头像视频"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                        case "website": drResult["UGC网址"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                    }
                    dtResult.Rows.Add(drResult);
                }
            }
            foreach(DataRow dr in dtResult.Rows)
            {
                for(int i=0;i<dtResult.Columns.Count;i++)
                {
                    if(dr[i]==DBNull.Value)
                    {
                        dr[i] = 0;
                    }
                }
            }

            DataView dv = dtResult.DefaultView;
            dv.Sort = "页码 ASC";
            dtResult = dv.ToTable();
            return dtResult;
        }
        #endregion


        #region 图书阅读分析统计
        /// <summary>
        /// 获取图书阅读分析统计
        /// </summary>
        /// <param name="serialId">图书系列ID</param>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBookReadCountData(int pageIndex, int pageSize, int type, int pressId, string bookGuid
            , string startDate, string endDate, int userId, ref int recordCount, ref int totalPersonCount
            , ref long totalTimeLength, ref int totalReadCount)
        {
            if (type > 0)
            {
                return GetBookReadCountData_Serial(pageIndex, pageSize, pressId, bookGuid, startDate, endDate, userId
                    , ref recordCount, ref totalPersonCount, ref totalTimeLength, ref totalReadCount);
            }
            else
            {
                return GetBookReadCountData_Book(pageIndex, pageSize, pressId, bookGuid, startDate, endDate, userId
                    , ref recordCount, ref totalPersonCount, ref totalTimeLength, ref totalReadCount);
            }
        }

        /// <summary>
        /// 获取图书阅读分析统计
        /// </summary>
        /// <param name="serialId">图书系列D</param>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        private DataTable GetBookReadCountData_Book(int pageIndex, int pageSize, int pressId, string bookGuid
            , string startDate, string endDate,int userId, ref int recordCount, ref int totalPersonCount, ref long totalTimeLength
            , ref int totalReadCount)
        {
            string strWhere = string.Empty;
            if (pressId > 0)
            {
                strWhere += $"AND dmb.Press = {pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGUID = '{bookGuid}' ";
            }
            string clickTimeJoin = $" AND chc.CHCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            string ugcTimeJoin = $" AND buc.BUCCountDate BETWEEN '{startDate}' AND '{endDate}' ";

            string strSql = $@"
                SELECT* FROM (
                    SELECT ROW_NUMBER() OVER(ORDER BY SUM(brc.BRCPersonCount) DESC) AS RowNum, dmb.BookGUID, dmb.BookName, pr.PressName
                        , SUM(ISNULL(brc.BRCCount,0)) AS ReadCount
                        --, SUM(ISNULL(brc.BRCPersonCount,0)) AS ReadPesonCount
                        ,0 AS ReadPesonCount
                        , SUM(ISNULL(brc.BRCDuration,0.00)) AS ReadTimeLength
                    FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                        INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                        INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON dmb.Press = pr.PressId AND pr.PressId > 0
                        LEFT JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs_Count} brc WITH(NOLOCK) ON brc.BRCBookGuid = dmb.BookGuid COLLATE Chinese_PRC_CI_AS AND brc.BRCCountDate BETWEEN '{startDate}' AND '{endDate}'
                    WHERE  dmb.visible=1 {strWhere}
                    GROUP BY dmb.BookGUID, dmb.BookName, pr.PressName) AS A
                WHERE A.RowNum BETWEEN {(pageIndex - 1) * pageSize + 1} AND {pageIndex * pageSize}";
            DataTable dtResult = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                foreach (DataRow dr in dtResult.Rows)
                {
                    long timeLength = ConvertHelper.ToInt32(dr["ReadTimeLength"]);
                    int ReadCount = ConvertHelper.ToInt32(dr["ReadCount"]);
                    decimal avgLength = ReadCount == 0 ? 0 : (timeLength / ReadCount);
                    dr["ReadTimeLength"] = avgLength;
                }
            }
            else
            {
                return null;
            }
            dtResult.Columns.Add("OnLineCount", typeof(int));//在线热点
            dtResult.Columns.Add("OffLineCount", typeof(int));//离线热点
            dtResult.Columns.Add("XmkhCount", typeof(int));//小梦辅导
            dtResult.Columns.Add("UGCAudioCount", typeof(int));//UGC音频
            dtResult.Columns.Add("UGCVideoCount", typeof(int));//UGC视频
            dtResult.Columns.Add("UGCImageCount", typeof(int));//UGC图片
            dtResult.Columns.Add("UGCWebCount", typeof(int));//UGC网址
            dtResult.Columns.Add("UGCFaceRecordCount", typeof(int));//UGC头像
            foreach (DataRow dr in dtResult.Rows)
            {
                dr["OffLineCount"] = 0;
                dr["OnLineCount"] = 0;
                dr["XmkhCount"] = 0;
                dr["UGCAudioCount"] = 0;
                dr["UGCVideoCount"] = 0;
                dr["UGCImageCount"] = 0;
                dr["UGCWebCount"] = 0;
                dr["UGCFaceRecordCount"] = 0;
            }

            //构建临时表
            string guidTable = Guid.NewGuid().ToString().Replace("-", "");
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append($@"
                CREATE TABLE #Temp{guidTable}(BookGuid VARCHAR(50));
                CREATE INDEX #Index_Temp{guidTable} ON #Temp{guidTable}(BOOKGUID);");
            foreach (DataRow dr in dtResult.Rows)
            {
                sbSql.Append($"INSERT INTO #Temp{guidTable} VALUES('{dr["BookGUID"].ToString()}');");
            }

            //构建UGC音频、视频、图片、网址、头像
            sbSql.Append($@"            
            SELECT tp.BookGuid
                , SUM(BUCAudioCount)AS UGCAudioCount
                , SUM(BUCVideoCount) AS UGCVideoCount
                , SUM(BUCImageCount) AS UGCImageCount
                , SUM(BUCWebCount) AS UGCWebCount
                , SUM(BUCFaceRecordCount) AS UGCFaceRecordCount
            FROM #Temp{guidTable} tp WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.BookUGC_Count} buc WITH(NOLOCK) ON buc.BUCBookGuid = tp.BookGuid {ugcTimeJoin} 
            GROUP BY tp.BookGuid;");

            //构建在线热点
            sbSql.Append($@"
                SELECT tp.BookGuid
                    , SUM(ISNULL(CHCAudioCount, 0) + ISNULL(CHCCommentCount, 0) + ISNULL(CHCEmptyCount, 0)
                        + ISNULL(CHCImageCount, 0) + ISNULL(CHCModelCount, 0)   + ISNULL(CHCUnknowCount, 0)
                        + ISNULL(CHCVideoCount, 0) + ISNULL(CHCWebCount, 0)     + ISNULL(CHCWebsiteCount, 0)+ISNULL(CHCXmkhCount,0)) AS AllCount,chc.CHCIsOnLine
                FROM #Temp{guidTable} tp WITH(NOLOCK)
                    INNER JOIN { SystemDBConfig.click_hotspot_count} chc WITH(NOLOCK) ON chc.CHCBookGuid = tp.BookGuid {clickTimeJoin}
                GROUP BY tp.BookGuid,chc.CHCIsOnLine; ");

            //构建小梦辅导
            sbSql.Append($@"
            SELECT tp.BookGuid, SUM(ISNULL(chc.CHCXmkhCount, 0)) AS XmkhCount
            FROM #Temp{guidTable} tp WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.click_hotspot_count} chc on chc.CHCBookGuid = tp.BookGuid {clickTimeJoin}
            WHERE chc.CHCIsOnLine = 0
            group by tp.BookGuid;");

            //构建阅读人数信息
            sbSql.Append($@"
                SELECT top 100 COUNT(DISTINCT brl.F_DeviceId) AS AllCount,brl.F_BookGuid 
                FROM #Temp{guidTable} tp WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs} brl WITH(NOLOCK) ON brl.F_BookGuid=tp.BookGuid
                WHERE brl.F_ReadingDuration>0 AND brl.F_CreateTime BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1)}'
                GROUP BY F_BookGuid;");

            sbSql.Append($"DROP INDEX #Index_Temp{guidTable} ON #Temp{guidTable};DROP TABLE #TEMP{guidTable};");
            
            DataSet ds = SqlDataAccess.sda.ExecSqlQuery(sbSql.ToString());
            if (ds == null || ds.Tables.Count < 3)
            {
                return dtResult;
            }
            foreach (DataRow drUgc in ds.Tables[0].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drUgc["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["UGCAudioCount"] = ConvertHelper.ToInt32(drUgc["UGCAudioCount"]);
                    drArr[0]["UGCVideoCount"] = ConvertHelper.ToInt32(drUgc["UGCVideoCount"]);
                    drArr[0]["UGCImageCount"] = ConvertHelper.ToInt32(drUgc["UGCImageCount"]);
                    drArr[0]["UGCWebCount"] = ConvertHelper.ToInt32(drUgc["UGCWebCount"]);
                    drArr[0]["UGCFaceRecordCount"] = ConvertHelper.ToInt32(drUgc["UGCFaceRecordCount"]);
                }
            }

            foreach (DataRow drOnline in ds.Tables[1].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drOnline["BookGuid"].ToString()}'");

                if (drArr != null && drArr.Length > 0)
                {
                    foreach(DataRow dr in drArr)
                    {
                        if(drOnline["CHCIsOnLine"].ToString()=="1")
                        {
                            dr["OnLineCount"] = ConvertHelper.ToInt32(drOnline["AllCount"]);
                        }
                        else
                        {
                            dr["OffLineCount"] = ConvertHelper.ToInt32(drOnline["AllCount"]);
                        }
                    }
                }
            }

            foreach (DataRow drXmkf in ds.Tables[2].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drXmkf["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["XmkhCount"] = ConvertHelper.ToInt32(drXmkf["XmkhCount"]);
                }
            }
            foreach(DataRow drPerson in ds.Tables[3].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drPerson["F_BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["ReadPesonCount"] = ConvertHelper.ToInt32(drPerson["AllCount"]);
                }
            }


            strSql = $@"
                SELECT COUNT(1) AS AllCount
                FROM { SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                WHERE dmb.visible=1 { strWhere}";
            DataTable dtCount = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dtCount != null && dtCount.Rows.Count > 0)
            {
                recordCount = ConvertHelper.ToInt32(dtCount.Rows[0]["AllCount"]);
            }
            strSql = $@"
                SELECT COUNT(DISTINCT brl.F_DeviceId) AS ReadPesonCount
                    , SUM(brl.F_ReadingDuration / 10000.0000)AS ReadTimeLength
                    , COUNT(brl.F_DeviceId) AS ReadCount
                FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId = dmb.Press AND ap.APUserId = {userId}
                    INNER JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs} brl WITH(NOLOCK) ON brl.F_BookGuid = dmb.BookGuid COLLATE Chinese_PRC_CI_AS
                WHERE brl.F_ReadingDuration>0 AND brl.F_CreateTime BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1)}' {strWhere}";

            DataTable dtTotal = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dtTotal != null && dtTotal.Rows.Count > 0)
            {
                totalPersonCount = ConvertHelper.ToInt32(dtTotal.Rows[0]["ReadPesonCount"]);
                totalTimeLength = (int)(ConvertHelper.ToDecimal(dtTotal.Rows[0]["ReadTimeLength"])*10000);
                totalReadCount = ConvertHelper.ToInt32(dtTotal.Rows[0]["ReadCount"]);
            }
            return dtResult;
        }

        /// <summary>
        /// 获取图书阅读分析统计
        /// </summary>
        /// <param name="serialId">图书系列ID</param>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <param name="totalPersonCount">阅读人数</param>
        /// <param name="totalTimeLength">阅读时长（总）</param>
        /// <param name="totalCount">阅读次数</param>
        /// <returns></returns>
        private DataTable GetBookReadCountData_Serial(int pageIndex, int pageSize, int pressId, string bookGuid
            , string startDate, string endDate,int userId, ref int recordCount, ref int totalPersonCount, ref long totalTimeLength
            , ref int totalReadCount)
        {
            string strWhere = string.Empty;
            if (pressId > 0)
            {
                strWhere += $"AND dmb.Press = {pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGUID = '{bookGuid}' ";
            }
            string strSql = $@"
                SELECT* FROM (
                    SELECT ROW_NUMBER() OVER(ORDER BY SUM(brc.BRCCount) DESC) AS RowNum,bs.SeriesID,bs.SeriesName,pr.PressName
                        , SUM(ISNULL(brc.BRCCount,0)) AS ReadCount
                        --, SUM(ISNULL(brc.BRCPersonCount,0)) AS ReadPesonCount
                        , 0 AS ReadPesonCount
                        , SUM(ISNULL(brc.BRCDuration,0.00)) AS ReadTimeLength
                    FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                        INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                        INNER JOIN {SystemDBConfig.book_series} bs WITH(NOLOCK) ON bs.SeriesID=dmb.BookSeries
                        LEFT JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON dmb.Press = pr.PressId AND pr.PressId > 0
                        LEFT JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs_Count} brc WITH(NOLOCK) ON brc.BRCBookGuid = dmb.BookGuid COLLATE Chinese_PRC_CI_AS AND brc.BRCCountDate BETWEEN '{startDate}' AND '{endDate}' 
                    WHERE 1=1 {strWhere}
                    GROUP BY bs.SeriesID,bs.SeriesName,pr.PressName) AS A
                WHERE A.RowNum BETWEEN {(pageIndex - 1) * pageSize + 1} AND {pageIndex * pageSize}";
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if(dt==null||dt.Rows.Count==0)
            {
                return null;
            }

            string guidTable = Guid.NewGuid().ToString().Replace("-", "");
            StringBuilder sbSqlPersonCount = new StringBuilder();
            sbSqlPersonCount.Append($@"
                CREATE TABLE #Temp{guidTable}(SeriesID INT);
                CREATE INDEX #Index_Temp{guidTable} ON #Temp{guidTable}(SeriesID);");
            foreach(DataRow dr in dt.Rows)
            {
                sbSqlPersonCount.Append($"INSERT INTO #Temp{guidTable} VALUES({dr["SeriesID"]});");
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    long timeLength = ConvertHelper.ToInt32(dr["ReadTimeLength"]);
                    int ReadCount = ConvertHelper.ToInt32(dr["ReadCount"]);
                    decimal avgLength = ReadCount == 0 ? 0 : (timeLength/ ReadCount );
                    dr["ReadTimeLength"] = avgLength;
                }
            }

            sbSqlPersonCount.Append($@"
                SELECT dmb.BookSeries,COUNT(DISTINCT brl.F_DeviceId) AS AllCount
                FROM {SystemDBConfig.Dream_Multimedia_Book} dmb
                    INNER JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs} brl ON brl.F_BookGuid = dmb.BookGuid COLLATE Chinese_PRC_CI_AS
                    INNER JOIN #Temp{guidTable} tp ON tp.SeriesID=dmb.BookSeries
                WHERE brl.F_ReadingDuration>0 AND brl.F_CreateTime BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1)}'
                GROUP BY dmb.BookSeries;");
            sbSqlPersonCount.Append($"DROP INDEX #Index{guidTable} ON #Temp{guidTable};DROP TABLE #Temp{guidTable};");
            DataTable dtPerson = SqlDataAccess.sda.ExecSqlTableQuery(sbSqlPersonCount.ToString());
            if (dtPerson != null && dtPerson.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow[] drArrPerson = dtPerson.Select($"BookSeries={dr["SeriesID"]}");
                    if (drArrPerson != null && drArrPerson.Length > 0)
                    {
                        dr["ReadPesonCount"] = ConvertHelper.ToInt32(drArrPerson[0]["AllCount"]);
                    }
                }
            }
            strSql = $@"
                SELECT COUNT(1) AS AllCount FROM( 
                SELECT COUNT(1) AS AllCount
                FROM { SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                    INNER JOIN {SystemDBConfig.book_series} bs WITH(NOLOCK) ON bs.SeriesID=dmb.BookSeries
                    LEFT JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON dmb.Press = pr.PressId AND pr.PressId > 0
                WHERE 1=1 {strWhere}
                GROUP BY bs.SeriesID,bs.SeriesName,pr.PressName) AS A ";
            DataTable dtCount = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dtCount != null && dtCount.Rows.Count > 0)
            {
                recordCount = ConvertHelper.ToInt32(dtCount.Rows[0]["AllCount"]);
            }

            strSql = $@"
                SELECT SUM(DISTINCT brc.BRCPersonCount) AS ReadPesonCount
                    , SUM(brc.BRCDuration)AS ReadTimeLength
                    , SUM(brc.BRCCount) AS ReadCount
                FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                    INNER JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs_Count} brc WITH(NOLOCK) ON brc.BRCBookGuid = dmb.BookGuid COLLATE Chinese_PRC_CI_AS
                WHERE  brc.BRCCountDate BETWEEN '{startDate}' AND '{endDate}' {strWhere}";
            DataTable dtTotal = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dtTotal != null && dtTotal.Rows.Count > 0)
            {
                totalReadCount = ConvertHelper.ToInt32(dtTotal.Rows[0]["ReadCount"]);
                totalTimeLength = ConvertHelper.ToInt64(dtTotal.Rows[0]["ReadTimeLength"]);
                totalPersonCount = ConvertHelper.ToInt32(dtTotal.Rows[0]["ReadPesonCount"]);
            }
            return dt;
        }

        /// <summary>
        /// 获取图书阅读分析统计
        /// </summary>
        /// <param name="serialId">图书系列D</param>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBookReadCount_Detail(string bookGuid, string startDate, string endDate)
        {
            string strSql = $@"
                SELECT B.BookName,pageNo, isOnline, COUNT(1) AS AllCount
                FROM {SystemDBConfig.click_hotspot} AS A WITH(NOLOCK)
                inner join {SystemDBConfig.Dream_Multimedia_Book} AS B 
                on B.BookGUID=A.bookGuid
                WHERE A.bookGuid = '{bookGuid}' AND [time] BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1)}'
                GROUP BY pageNo, isOnline,B.BookName

                SELECT pageNo, ugcType, COUNT(1) AS AllCount
                FROM {SystemDBConfig.BookUGC} WITH(NOLOCK)
                WHERE BOOKGUID = '{bookGuid}' AND createTime BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1)}'
                GROUP BY pageNo, ugcType";
            DataSet ds = SqlDataAccess.sda.ExecSqlQuery(strSql);
            if (ds == null || ds.Tables.Count < 2)
            {
                return null;
            }

            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("图书名称", typeof(string));
            dtResult.Columns.Add("页码", typeof(int));
            dtResult.Columns.Add("在线热点", typeof(int));
            dtResult.Columns.Add("离线热点", typeof(int));
            dtResult.Columns.Add("UGC音频", typeof(int));
            dtResult.Columns.Add("UGC视频", typeof(int));
            dtResult.Columns.Add("UGC图片", typeof(int));
            dtResult.Columns.Add("UGC网址", typeof(int));
            dtResult.Columns.Add("头像视频", typeof(int));

            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow[] drArrResult = dtResult.Select("页码=" + dr["pageNo"].ToString());
                    if (drArrResult != null && drArrResult.Length > 0)
                    {
                        drArrResult[0]["图书名称"] = dr["BookName"];
                        if (dr["isOnline"].ToString() == "1")
                        {
                            drArrResult[0]["在线热点"] = ConvertHelper.ToInt32(dr["AllCount"]);
                        }
                        else
                        {
                            drArrResult[0]["离线热点"] = ConvertHelper.ToInt32(dr["AllCount"]);
                        }
                    }
                    else
                    {
                        DataRow drResult = dtResult.NewRow();
                        drResult["页码"] = ConvertHelper.ToInt32(dr["pageNo"]);
                        drResult["图书名称"] = dr["BookName"];
                        if (dr["isOnline"].ToString() == "1")
                        {
                            drResult["在线热点"] = ConvertHelper.ToInt32(dr["AllCount"]);
                        }
                        else
                        {
                            drResult["离线热点"] = ConvertHelper.ToInt32(dr["AllCount"]);
                        }
                        dtResult.Rows.Add(drResult);
                    }
                }
            }

            if(ds.Tables[1]!=null&&ds.Tables[1].Rows.Count>0)
            {
                foreach(DataRow dr in ds.Tables[1].Rows)
                {
                    DataRow[] drArrResult = dtResult.Select("页码=" + dr["pageNo"].ToString());
                    if (drArrResult != null && drArrResult.Length > 0)
                    {
                        switch(dr["ugcType"].ToString())
                        {
                            case "audio":drArrResult[0]["UGC音频"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                            case "image": drArrResult[0]["UGC图片"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                            case "video": drArrResult[0]["UGC视频"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                            case "faceRecord": drArrResult[0]["头像视频"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                            case "website": drArrResult[0]["UGC网址"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                        }
                    }
                    else
                    {
                        DataRow drResult = dtResult.NewRow();
                        drResult["页码"] = ConvertHelper.ToInt32(dr["pageNo"]);
                        switch (dr["ugcType"].ToString())
                        {
                            case "audio": drResult["UGC音频"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                            case "image": drResult["UGC图片"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                            case "video": drResult["UGC视频"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                            case "faceRecord": drResult["头像视频"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                            case "website": drResult["UGC网址"] = ConvertHelper.ToInt32(dr["AllCount"]); break;
                        }
                        dtResult.Rows.Add(drResult);
                    }
                }
            }
            foreach (DataRow drResult in dtResult.Rows)
            {
                for (int i = 0; i < dtResult.Columns.Count;i++)
                {
                    if (drResult[i] == DBNull.Value)
                    {
                        drResult[i] = 0;
                    }
                }
            }

            DataView dv = dtResult.DefaultView;
            dv.Sort = "页码 ASC";
            dtResult = dv.ToTable();
            return dtResult;
        }


        /// <summary>
        /// 获取图书阅读分析统计的导出数据
        /// </summary>
        /// <param name="publishId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBookReadExportData(int type, int pressId, string bookGuid, string startDate, string endDate,int userId)
        {
            if (type > 0)
            {
                return GetBookReadExportData_Serial(pressId, bookGuid, startDate, endDate,userId);
            }
            else
            {
                return GetBookReadExportData_Book(pressId, bookGuid, startDate, endDate,userId);
            }
        }


        /// <summary>
        /// 获取图书阅读分析统计的导出数据
        /// </summary>
        /// <param name="publishId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        private DataTable GetBookReadExportData_Serial(int pressId, string bookGuid, string startDate, string endDate, int userId)
        {
            string strWhere = $"  ";
            if (pressId > 0)
            {
                strWhere += $" AND dmb.Press = {pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGUID = '{bookGuid}' ";
            }

            string strSql = $@"
                    SELECT bs.SeriesName AS 系列名称
                        , ISNULL(pr.PressName,'无') AS 出版社
                        , SUM(ISNULL(brc.BRCPersonCount,0)) AS 阅读人数
                        , SUM(ISNULL(brc.BRCCount,0)) AS 阅读次数
                        , SUM(ISNULL(brc.BRCDuration,0)) AS 人均单次阅读时长秒
                    FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                        INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                        INNER JOIN {SystemDBConfig.book_series} bs WITH(NOLOCK) ON bs.SeriesID=dmb.BookSeries
                        LEFT JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON dmb.Press = pr.PressId
                        LEFT JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs_Count} brc WITH(NOLOCK) ON brc.BRCBookGuid = dmb.BookGuid COLLATE Chinese_PRC_CI_AS
                    WHERE brc.BRCCountDate BETWEEN '{startDate}' AND '{endDate}' {strWhere}
                    GROUP BY bs.SeriesName, pr.PressName
                    ORDER BY SUM(brc.BRCPersonCount) DESC";
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    long timeLength = ConvertHelper.ToInt32(dr["人均单次阅读时长秒"]);
                    int readCount = ConvertHelper.ToInt32(dr["阅读次数"]);
                    int avgLength = readCount == 0 ? 0 : (int)(timeLength / readCount);
                    dr["人均单次阅读时长秒"] = avgLength;
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取图书阅读分析统计的导出数据
        /// </summary>
        /// <param name="publishId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        private DataTable GetBookReadExportData_Book(int pressId, string bookGuid, string startDate, string endDate,int userId)
        {
            string strWhere = string.Empty;
            if (pressId > 0)
            {
                strWhere += $"AND dmb.Press = {pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGUID = '{bookGuid}' ";
            }
            string clickTimeJoin = $" AND chc.CHCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            string ugcTimeJoin = $" AND buc.BUCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            string strSql = $@"
                    SELECT dmb.BookName AS 图书名称
                        , ISNULL(pr.PressName,'无') AS 出版社
                        , SUM(ISNULL(brc.BRCPersonCount,0)) AS 阅读人数
                        , SUM(ISNULL(brc.BRCCount,0)) AS 阅读次数
                        , SUM(ISNULL(brc.BRCDuration,0.00)) AS 人均单次阅读时长
                        ,dmb.BookGuid
                    FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                        INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                        LEFT JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON dmb.Press = pr.PressId
                        LEFT JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs_Count} brc WITH(NOLOCK) ON brc.BRCBookGuid = dmb.BookGuid COLLATE Chinese_PRC_CI_AS
                    WHERE brc.BRCCountDate BETWEEN '{startDate}' AND '{endDate}' {strWhere}
                    GROUP BY dmb.BookGUID, dmb.BookName, pr.PressName
                    ORDER BY SUM(brc.BRCPersonCount) DESC";
            DataTable dtResult = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                foreach (DataRow dr in dtResult.Rows)
                {
                    long timeLength = ConvertHelper.ToInt32(dr["人均单次阅读时长"]);
                    int readCount = ConvertHelper.ToInt32(dr["阅读次数"]);
                    int avgLength = readCount == 0 ? 0 :(int) (timeLength / readCount);
                    dr["人均单次阅读时长"] = avgLength;
                }
            }
            dtResult.Columns.Add("离线热点", typeof(int));//离线热点 
            dtResult.Columns.Add("在线热点", typeof(int));//在线热点
            dtResult.Columns.Add("小梦辅导", typeof(int));//小梦辅导
            dtResult.Columns.Add("UGC音频", typeof(int));//UGC音频
            dtResult.Columns.Add("UGC视频", typeof(int));//UGC视频
            dtResult.Columns.Add("UGC图片", typeof(int));//UGC图片
            dtResult.Columns.Add("UGC网址", typeof(int));//UGC网址
            dtResult.Columns.Add("UGC头像", typeof(int));//UGC头像
            foreach (DataRow dr in dtResult.Rows)
            {
                dr["离线热点"] = 0;
                dr["在线热点"] = 0;
                dr["小梦辅导"] = 0;
                dr["UGC音频"] = 0;
                dr["UGC视频"] = 0;
                dr["UGC图片"] = 0;
                dr["UGC网址"] = 0;
                dr["UGC头像"] = 0;
            }

            //构建临时表
            string guidTable = Guid.NewGuid().ToString().Replace("-", "");
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append($@"
                CREATE TABLE #Temp{guidTable}(BookGuid VARCHAR(50));
                CREATE INDEX #Index_Temp{guidTable} ON #Temp{guidTable}(BOOKGUID);");
            foreach (DataRow dr in dtResult.Rows)
            {
                sbSql.Append($"INSERT INTO #Temp{guidTable} VALUES('{dr["BookGUID"].ToString()}');");
            }

            //构建UGC音频、视频、图片、网址、头像
            sbSql.Append($@"            
            SELECT tp.BookGuid
                , SUM(BUCAudioCount)AS UGC音频
                , SUM(BUCVideoCount) AS UGC视频
                , SUM(BUCImageCount) AS UGC图片
                , SUM(BUCWebCount) AS UGC网址
                , SUM(BUCFaceRecordCount) AS UGC头像
            FROM #Temp{guidTable} tp WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.BookUGC_Count} buc WITH(NOLOCK) ON buc.BUCBookGuid = tp.BookGuid {ugcTimeJoin} 
            GROUP BY tp.BookGuid;");

            //构建在线热点
            sbSql.Append($@"
                SELECT tp.BookGuid
                    , SUM(ISNULL(CHCAudioCount, 0) + ISNULL(CHCCommentCount, 0) + ISNULL(CHCEmptyCount, 0)
                        + ISNULL(CHCImageCount, 0) + ISNULL(CHCModelCount, 0)   + ISNULL(CHCUnknowCount, 0)
                        + ISNULL(CHCVideoCount, 0) + ISNULL(CHCWebCount, 0)     + ISNULL(CHCWebsiteCount, 0)+ISNULL(CHCXmkhCount,0)) AS AllCount,chc.CHCIsOnLine
                FROM #Temp{guidTable} tp WITH(NOLOCK)
                    INNER JOIN { SystemDBConfig.click_hotspot_count} chc WITH(NOLOCK) ON chc.CHCBookGuid = tp.BookGuid {clickTimeJoin}
                GROUP BY tp.BookGuid,chc.CHCIsOnLine; ");

            //构建小梦辅导
            sbSql.Append($@"
            SELECT tp.BookGuid, SUM(ISNULL(chc.CHCXmkhCount, 0)) AS 小梦辅导
            FROM #Temp{guidTable} tp WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.click_hotspot_count} chc on chc.CHCBookGuid = tp.BookGuid {clickTimeJoin}
            WHERE chc.CHCIsOnLine = 0
            group by tp.BookGuid;");
            sbSql.Append($"DROP INDEX #Index_Temp{guidTable} ON #Temp{guidTable};DROP TABLE #TEMP{guidTable};");

            DataSet ds = SqlDataAccess.sda.ExecSqlQuery(sbSql.ToString());
            if (ds == null || ds.Tables.Count < 3)
            {
                return dtResult;
            }
            foreach (DataRow drUgc in ds.Tables[0].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drUgc["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["UGC音频"] = ConvertHelper.ToInt32(drUgc["UGC音频"]);
                    drArr[0]["UGC视频"] = ConvertHelper.ToInt32(drUgc["UGC视频"]);
                    drArr[0]["UGC图片"] = ConvertHelper.ToInt32(drUgc["UGC图片"]);
                    drArr[0]["UGC网址"] = ConvertHelper.ToInt32(drUgc["UGC网址"]);
                    drArr[0]["UGC头像"] = ConvertHelper.ToInt32(drUgc["UGC头像"]);
                }
            }

            foreach (DataRow drOnline in ds.Tables[1].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drOnline["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    foreach(DataRow dr in drArr)
                    {
                        if(drOnline["CHCIsOnLine"].ToString()=="1")
                        {
                            dr["在线热点"] = ConvertHelper.ToInt32(drOnline["AllCount"]);
                        }
                        else
                        {
                            dr["离线热点"] = ConvertHelper.ToInt32(drOnline["AllCount"]);
                        }
                    }
                }
            }

            foreach (DataRow drXmkf in ds.Tables[2].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drXmkf["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["小梦辅导"] = ConvertHelper.ToInt32(drXmkf["小梦辅导"]);
                }
            }
            dtResult.Columns.Remove("BookGuid");
            return dtResult;
        }

        /// <summary>
        /// 获取图书阅读分析统计的明细导出数据
        /// </summary>
        /// <param name="publishId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBookReadExportDetailData(string bookGuid, string startDate, string endDate,int userId)
        {
            string strSql = $@"
            SELECT  dmb.BookName AS 图书名称
                    ,brl.F_DeviceId AS 设备ID
                    ,(CASE di.F_DeviceType WHEN 1 THEN 'IOS' ELSE 'ANDROID' END ) AS 设备类型
                    , pr.PressName AS 出版社
                    ,bs.SeriesName AS 系列
                    ,brl.F_ReadingDuration AS 阅读时长秒
	                ,brl.F_CreateTime AS 时间
                    ,brl.F_AppVersion AS 版本号
            FROM {SystemDBConfig.T_Book_ReadingDuration_Logs} brl WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.Dream_Multimedia_Book} dmb  WITH(NOLOCK) ON brl.F_BookGuid = dmb.BookGUID COLLATE Chinese_PRC_CI_AS
                INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                LEFT  JOIN {SystemDBConfig.book_series} bs WITH(NOLOCK) ON bs.SeriesID = dmb.BookSeries
                LEFT  JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON pr.PressID = dmb.Press 
                LEFT JOIN {SystemDBConfig.T_Device_Info } di WITH(NOLOCK) ON di.F_DeviceID=brl.F_DeviceId
            WHERE dmb.BookGuid = '{bookGuid}' AND brl.F_ReadingDuration>0 AND brl.F_CreateTime BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1)}'
            ORDER BY brl.F_CreateTime DESC";
            return SqlDataAccess.sda.ExecSqlTableQuery(strSql);
        }
        #endregion

        #region 图书阅读分析统计导出
        /// <summary>
        /// 获取图书阅读分析统计
        /// </summary>
        /// <param name="serialId">图书系列ID</param>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        public DataTable GetBookReadCountData_Export(int type, int pressId, string bookGuid
            , string startDate, string endDate, int userId)
        {
            if (type > 0)
            {
                return GetBookReadCountData_Serial_Export(pressId, bookGuid, startDate, endDate, userId);
            }
            else
            {
                return GetBookReadCountData_Book_Export(pressId, bookGuid, startDate, endDate, userId);
            }
        }

        /// <summary>
        /// 获取图书阅读分析统计
        /// </summary>
        /// <param name="serialId">图书系列D</param>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="recordCount">符合条件的记录总数</param>
        /// <returns></returns>
        private DataTable GetBookReadCountData_Book_Export(int pressId, string bookGuid, string startDate, string endDate, int userId)
        {
            string strWhere = string.Empty;
            if (pressId > 0)
            {
                strWhere += $"AND dmb.Press = {pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGUID = '{bookGuid}' ";
            }
            string clickTimeJoin = $" AND chc.CHCCountDate BETWEEN '{startDate}' AND '{endDate}' ";
            string ugcTimeJoin = $" AND buc.BUCCountDate BETWEEN '{startDate}' AND '{endDate}' ";

            string strSql = $@"
                    SELECT dmb.BookGUID
                        , dmb.BookName AS 图书名
                        , pr.PressName AS 出版社
                        , SUM(ISNULL(brc.BRCCount,0)) AS 阅读次数
                        ,0 AS 阅读人数
                        , SUM(ISNULL(brc.BRCDuration,0.00)) AS 阅读时长
                        ,0 AS 人均单次阅读时长
                    FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                        INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                        INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON dmb.Press = pr.PressId AND pr.PressId > 0
                        LEFT JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs_Count} brc WITH(NOLOCK) ON brc.BRCBookGuid = dmb.BookGuid COLLATE Chinese_PRC_CI_AS AND brc.BRCCountDate BETWEEN '{startDate}' AND '{endDate}'
                    WHERE  dmb.visible=1 {strWhere}
                    GROUP BY dmb.BookGUID, dmb.BookName, pr.PressName
                    ORDER BY SUM(brc.BRCPersonCount) DESC";
            DataTable dtResult = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                foreach (DataRow dr in dtResult.Rows)
                {
                    long timeLength = ConvertHelper.ToInt32(dr["阅读时长"]);
                    int ReadCount = ConvertHelper.ToInt32(dr["阅读次数"]);
                    decimal avgLength = ReadCount == 0 ? 0 : (timeLength / ReadCount);
                    dr["人均单次阅读时长"] = avgLength;
                }
            }
            else
            {
                return null;
            }
            dtResult.Columns.Add("在线热点", typeof(int));//在线热点
            dtResult.Columns.Add("离线热点", typeof(int));//离线热点
            dtResult.Columns.Add("小梦辅导", typeof(int));//小梦辅导
            dtResult.Columns.Add("UGC音频", typeof(int));//UGC音频
            dtResult.Columns.Add("UGC视频", typeof(int));//UGC视频
            dtResult.Columns.Add("UGC图片", typeof(int));//UGC图片
            dtResult.Columns.Add("UGC网址", typeof(int));//UGC网址
            dtResult.Columns.Add("UGC头像", typeof(int));//UGC头像
            foreach (DataRow dr in dtResult.Rows)
            {
                dr["离线热点"] = 0;
                dr["在线热点"] = 0;
                dr["小梦辅导"] = 0;
                dr["UGC音频"] = 0;
                dr["UGC视频"] = 0;
                dr["UGC图片"] = 0;
                dr["UGC网址"] = 0;
                dr["UGC头像"] = 0;
            }

            //构建临时表
            string guidTable = Guid.NewGuid().ToString().Replace("-", "");
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append($@"
                CREATE TABLE #Temp{guidTable}(BookGuid VARCHAR(50));
                CREATE INDEX #Index_Temp{guidTable} ON #Temp{guidTable}(BOOKGUID);");
            foreach (DataRow dr in dtResult.Rows)
            {
                sbSql.Append($"INSERT INTO #Temp{guidTable} VALUES('{dr["BookGUID"].ToString()}');");
            }

            //构建UGC音频、视频、图片、网址、头像
            sbSql.Append($@"            
            SELECT tp.BookGuid
                , SUM(BUCAudioCount)AS UGCAudioCount
                , SUM(BUCVideoCount) AS UGCVideoCount
                , SUM(BUCImageCount) AS UGCImageCount
                , SUM(BUCWebCount) AS UGCWebCount
                , SUM(BUCFaceRecordCount) AS UGCFaceRecordCount
            FROM #Temp{guidTable} tp WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.BookUGC_Count} buc WITH(NOLOCK) ON buc.BUCBookGuid = tp.BookGuid {ugcTimeJoin} 
            GROUP BY tp.BookGuid;");

            //构建在线热点
            sbSql.Append($@"
                SELECT tp.BookGuid
                    , SUM(ISNULL(CHCAudioCount, 0) + ISNULL(CHCCommentCount, 0) + ISNULL(CHCEmptyCount, 0)
                        + ISNULL(CHCImageCount, 0) + ISNULL(CHCModelCount, 0)   + ISNULL(CHCUnknowCount, 0)
                        + ISNULL(CHCVideoCount, 0) + ISNULL(CHCWebCount, 0)     + ISNULL(CHCWebsiteCount, 0)+ISNULL(CHCXmkhCount,0)) AS AllCount,chc.CHCIsOnLine
                FROM #Temp{guidTable} tp WITH(NOLOCK)
                    INNER JOIN { SystemDBConfig.click_hotspot_count} chc WITH(NOLOCK) ON chc.CHCBookGuid = tp.BookGuid {clickTimeJoin}
                GROUP BY tp.BookGuid,chc.CHCIsOnLine; ");

            //构建小梦辅导
            sbSql.Append($@"
            SELECT tp.BookGuid, SUM(ISNULL(chc.CHCXmkhCount, 0)) AS XmkhCount
            FROM #Temp{guidTable} tp WITH(NOLOCK)
                INNER JOIN {SystemDBConfig.click_hotspot_count} chc on chc.CHCBookGuid = tp.BookGuid {clickTimeJoin}
            WHERE chc.CHCIsOnLine = 0
            group by tp.BookGuid;");

            //构建阅读人数信息
            sbSql.Append($@"
                SELECT top 100 COUNT(DISTINCT brl.F_DeviceId) AS AllCount,brl.F_BookGuid 
                FROM #Temp{guidTable} tp WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs} brl WITH(NOLOCK) ON brl.F_BookGuid=tp.BookGuid
                WHERE brl.F_ReadingDuration BETWEEN 1 AND 30000 AND brl.F_CreateTime BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1)}'
                GROUP BY F_BookGuid;");

            sbSql.Append($"DROP INDEX #Index_Temp{guidTable} ON #Temp{guidTable};DROP TABLE #TEMP{guidTable};");

            DataSet ds = SqlDataAccess.sda.ExecSqlQuery(sbSql.ToString());
            if (ds == null || ds.Tables.Count < 3)
            {
                return dtResult;
            }
            foreach (DataRow drUgc in ds.Tables[0].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drUgc["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["UGC音频"] = ConvertHelper.ToInt32(drUgc["UGCAudioCount"]);
                    drArr[0]["UGC视频"] = ConvertHelper.ToInt32(drUgc["UGCVideoCount"]);
                    drArr[0]["UGC图片"] = ConvertHelper.ToInt32(drUgc["UGCImageCount"]);
                    drArr[0]["UGC网址"] = ConvertHelper.ToInt32(drUgc["UGCWebCount"]);
                    drArr[0]["UGC头像"] = ConvertHelper.ToInt32(drUgc["UGCFaceRecordCount"]);
                }
            }

            foreach (DataRow drOnline in ds.Tables[1].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drOnline["BookGuid"].ToString()}'");

                if (drArr != null && drArr.Length > 0)
                {
                    foreach (DataRow dr in drArr)
                    {
                        if (drOnline["CHCIsOnLine"].ToString() == "1")
                        {
                            dr["在线热点"] = ConvertHelper.ToInt32(drOnline["AllCount"]);
                        }
                        else
                        {
                            dr["离线热点"] = ConvertHelper.ToInt32(drOnline["AllCount"]);
                        }
                    }
                }
            }

            foreach (DataRow drXmkf in ds.Tables[2].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drXmkf["BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["小梦辅导"] = ConvertHelper.ToInt32(drXmkf["XmkhCount"]);
                }
            }
            foreach (DataRow drPerson in ds.Tables[3].Rows)
            {
                DataRow[] drArr = dtResult.Select($"BookGuid='{drPerson["F_BookGuid"].ToString()}'");
                if (drArr != null && drArr.Length > 0)
                {
                    drArr[0]["阅读人数"] = ConvertHelper.ToInt32(drPerson["AllCount"]);
                }
            }
            dtResult.Columns.Remove("BookGUID");
            return dtResult;
        }

        /// <summary>
        /// 获取图书阅读分析统计
        /// </summary>
        /// <param name="serialId">图书系列ID</param>
        /// <param name="pressId">出版社ID</param>
        /// <param name="bookGuid">图书GUID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        private DataTable GetBookReadCountData_Serial_Export(int pressId, string bookGuid , string startDate, string endDate, int userId)
        {
            string strWhere = string.Empty;
            if (pressId > 0)
            {
                strWhere += $"AND dmb.Press = {pressId} ";
            }
            if (!string.IsNullOrEmpty(bookGuid))
            {
                strWhere += $" AND dmb.BookGUID = '{bookGuid}' ";
            }
            string strSql = $@"
                    SELECT bs.SeriesID
                        ,bs.SeriesName AS 系列
                        ,pr.PressName 出版社
                        , SUM(ISNULL(brc.BRCCount,0)) AS 阅读次数
                        , 0 AS 阅读人数
                        , SUM(ISNULL(brc.BRCDuration,0.00)) AS 总计阅读时长
                        ,0 AS 人均单次阅读时长
                    FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
                        INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press AND ap.APUserId={userId}
                        INNER JOIN {SystemDBConfig.book_series} bs WITH(NOLOCK) ON bs.SeriesID=dmb.BookSeries
                        INNER JOIN {SystemDBConfig.Press} pr WITH(NOLOCK) ON dmb.Press = pr.PressId 
                        LEFT JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs_Count} brc WITH(NOLOCK) ON brc.BRCBookGuid = dmb.BookGuid COLLATE Chinese_PRC_CI_AS AND brc.BRCCountDate BETWEEN '{startDate}' AND '{endDate}' 
                    WHERE pr.PressId > 0 {strWhere}
                    GROUP BY bs.SeriesID,bs.SeriesName,pr.PressName
                    ORDER BY SUM(brc.BRCCount) DESC";
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            string guidTable = Guid.NewGuid().ToString().Replace("-", "");
            StringBuilder sbSqlPersonCount = new StringBuilder();
            sbSqlPersonCount.Append($@"
                CREATE TABLE #Temp{guidTable}(SeriesID INT);
                CREATE INDEX #Index_Temp{guidTable} ON #Temp{guidTable}(SeriesID);");
            foreach (DataRow dr in dt.Rows)
            {
                sbSqlPersonCount.Append($"INSERT INTO #Temp{guidTable} VALUES({dr["SeriesID"]});");
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    long timeLength = ConvertHelper.ToInt32(dr["总计阅读时长"]);
                    int ReadCount = ConvertHelper.ToInt32(dr["阅读次数"]);
                    decimal avgLength = ReadCount == 0 ? 0 : (timeLength / ReadCount);
                    dr["人均单次阅读时长"] = avgLength;
                }
            }

            sbSqlPersonCount.Append($@"
                SELECT dmb.BookSeries,COUNT(DISTINCT brl.F_DeviceId) AS AllCount
                FROM {SystemDBConfig.Dream_Multimedia_Book} dmb
                    INNER JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs} brl ON brl.F_BookGuid = dmb.BookGuid COLLATE Chinese_PRC_CI_AS
                    INNER JOIN #Temp{guidTable} tp ON tp.SeriesID=dmb.BookSeries
                WHERE brl.F_ReadingDuration BETWEEN 1 AND 30000 AND brl.F_CreateTime BETWEEN '{startDate}' AND '{ConvertHelper.ToDateTime(endDate).AddDays(1)}'
                GROUP BY dmb.BookSeries;");
            sbSqlPersonCount.Append($"DROP INDEX #Index{guidTable} ON #Temp{guidTable};DROP TABLE #Temp{guidTable};");
            DataTable dtPerson = SqlDataAccess.sda.ExecSqlTableQuery(sbSqlPersonCount.ToString());
            if (dtPerson != null && dtPerson.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow[] drArrPerson = dtPerson.Select($"BookSeries={dr["SeriesID"]}");
                    if (drArrPerson != null && drArrPerson.Length > 0)
                    {
                        dr["阅读人数"] = ConvertHelper.ToInt32(drArrPerson[0]["AllCount"]);
                    }
                }
            }
            dt.Columns.Remove("SeriesID");
            return dt;
        }
        #endregion
    }
}
