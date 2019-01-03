using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using StatisticalCenter.Common;
using Newtonsoft.Json;
namespace StatisticalCenter.DataAccess.TableDataCollection
{
    public class T_DownLoaded_Book_Data
    {
        /// <summary>
        /// 获取图书列表（仅含制作成二维码的图书）
        /// </summary>
        /// <returns></returns>
        public DataTable GetBookList()
        {
            string strSql = $@"
                SELECT [F_Customization_ID] AS id,[F_Customization_Name] AS name
                FROM {SystemDBConfig.T_Customization_Info}
                        WHERE F_Customization_Name <> ''";
            return SqlDataAccess.sda.ExecSqlTableQuery(strSql);
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
        public DataTable GetCountData(int pageIndex, int pageSize, string bookName, string dateStart, string dateEnd, ref int recordCount)
        {
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(bookName))
            {
                strWhere = $" WHERE ci.F_Customization_Name LIKE '{bookName.Replace("'", "")}%' ";
            }
            else
            {
                strWhere = $" WHERE ci.F_Customization_Name<> '' ";
            }
            string strSqlMain = $@"
                SELECT * FROM (
                SELECT ROW_NUMBER() OVER(ORDER BY SUM(download_count) DESC) AS RowNum
                    ,ci.[F_Customization_ID],ci.[F_Customization_Content]
                    ,ci.[F_Customization_Name],[F_Customization_Url]
                    ,ISNULL(SUM(download_count),0) AS BookDownLoadCount
                FROM {SystemDBConfig.T_Customization_Info} ci WITH(NOLOCK)
                    LEFT JOIN {SystemDBConfig.T_Download_Book_Count} dbc WITH(NOLOCK) ON dbc.cus_id=ci.F_Customization_ID  AND dbc.count_date BETWEEN '{dateStart}' AND '{ dateEnd}'
                {strWhere}
                GROUP BY ci.[F_Customization_ID],ci.[F_Customization_Content],ci.[F_Customization_Name],[F_Customization_Url]) AS A 
                WHERE A.RowNum BETWEEN {(pageIndex-1)*pageSize+1} AND {pageIndex*pageSize}
                SELECT COUNT(1) AS AllCount FROM {SystemDBConfig.T_Customization_Info} ci WITH(NOLOCK) 
                {strWhere}";
            DataSet dsResult = SqlDataAccess.sda.ExecSqlQuery(strSqlMain);
            if (dsResult == null || dsResult.Tables.Count < 2 || dsResult.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            dsResult.Tables[0].Columns.Add("PressId", typeof(int));//出版社ID
            dsResult.Tables[0].Columns.Add("PublishName", typeof(string));//出版社
            dsResult.Tables[0].Columns.Add("APKDownloadCount", typeof(int));
            foreach (DataRow drResult in dsResult.Tables[0].Rows)
            {
                drResult["PressId"] = 0;
                drResult["PublishName"] = "梦想人";
                drResult["APKDownloadCount"] = 0;
            }

            StringBuilder sbSqlPress = new StringBuilder();
            StringBuilder sbSqlBrowse = new StringBuilder();
            foreach (DataRow dr in dsResult.Tables[0].Rows)
            {
                sbSqlBrowse.Append(ConvertHelper.ToInt32(dr["F_Customization_ID"]) + ",");

                CusBookEntity bookEntity = JsonConvert.DeserializeObject<CusBookEntity>(dr["F_Customization_Content"].ToString());
                if (bookEntity == null)
                {
                    bookEntity = new CusBookEntity();
                }
                if (bookEntity.publishID > 0)
                {
                    sbSqlPress.Append(bookEntity.publishID + ",");
                    dr["PressID"] = bookEntity.publishID;
                }
            }
            if (sbSqlPress.Length > 0)
            {
                string strSqlPress = $"SELECT [PressID],[PressName] FROM {SystemDBConfig.Press} WITH(NOLOCK) WHERE PressID IN ({sbSqlPress.ToString().Trim(',')}) ";
                DataTable dtPress = SqlDataAccess.sda.ExecSqlTableQuery(strSqlPress);
                if (dtPress != null && dtPress.Rows.Count > 0)
                {
                    foreach (DataRow drPress in dtPress.Rows)
                    {
                        DataRow[] drArrResult = dsResult.Tables[0].Select("PressID=" + drPress["PressID"]);
                        if (drArrResult != null && drArrResult.Length > 0)
                        {
                            for(int i=0;i<drArrResult.Length;i++)
                            {
                                drArrResult[i]["PublishName"] = drPress["PressName"].ToString();
                            }
                        }
                    }
                }
            }

            if (sbSqlBrowse.Length > 0)
            {
                string strSqlBrowse = $@"
                SELECT ci.F_Customization_ID,SUM(ctc.CTCBrowseCount) AS AllCount
                FROM {SystemDBConfig.T_Customization_Info} ci WITH(NOLOCK)
	                INNER JOIN {SystemDBConfig.T_CountToolResource} ctr WITH(NOLOCK) ON ctr.CTRUrl=ci.F_Customization_Url
	                INNER JOIN {SystemDBConfig.T_CountToolCount} ctc WITH(NOLOCK) ON ctc.CTC_CTRId=ctr.CTRId AND ctc.CTCCountDate BETWEEN '{dateStart}' AND '{dateEnd}'
                WHERE ci.F_Customization_ID IN ({sbSqlBrowse.ToString().Trim(',')})
                GROUP BY ci.F_Customization_ID";
                DataTable dtBrowse = SqlDataAccess.sda.ExecSqlTableQuery(strSqlBrowse);
                if (dtBrowse != null && dtBrowse.Rows.Count > 0)
                {
                    foreach (DataRow drBrowse in dtBrowse.Rows)
                    {
                        DataRow[] drArrResult = dsResult.Tables[0].Select("F_Customization_ID=" + drBrowse["F_Customization_ID"]);
                        if (drArrResult != null && drArrResult.Length > 0)
                        {
                            drArrResult[0]["APKDownloadCount"] = ConvertHelper.ToInt32(drBrowse["AllCount"]);
                        }
                    }
                }
            }
            if (dsResult.Tables[1] != null && dsResult.Tables[1].Rows.Count > 0)
            {
                recordCount = ConvertHelper.ToInt32(dsResult.Tables[1].Rows[0]["AllCount"]);
            }
            return dsResult.Tables[0];
        }

        /// <summary>
        /// 从二维码中获取到的图书信息
        /// </summary>
        private class CusBookEntity
        {
            /// <summary>
            /// 出版社ID
            /// </summary>
            public int publishID { get; set; } = 0;
            
            /// <summary>
            /// 图书的GUID,如果有多个则用英文半角逗号隔开
            /// </summary>
            public string downloadBookList { get; set; }
            
            /// <summary>
            /// 出版社ICON
            /// </summary>
            public string publishIcon { get; set; }
            
            /// <summary>
            /// 激活码
            /// </summary>
            public string activationCode { get; set;}
        }

        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <param name="cusId">二维码ID</param>
        /// <param name="bookName">图书名</param>
        /// <param name="dateStart">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns></returns>
        public DataTable GetExportData(int cusId, string dateStart, string dateEnd)
        {
            string strSqlCus = $@"SELECT F_Customization_Name,F_Customization_Content FROM {SystemDBConfig.T_Customization_Info} WITH(NOLOCK) WHERE F_Customization_ID={cusId}";
            DataTable dtCus = SqlDataAccess.sda.ExecSqlTableQuery(strSqlCus);
            if(dtCus==null||dtCus.Rows.Count==0)
            {
                return dtCus;
            }
            CusBookEntity bookEntity = JsonConvert.DeserializeObject<CusBookEntity>(dtCus.Rows[0]["F_Customization_Content"].ToString());
            if(bookEntity==null)
            {
                bookEntity = new CusBookEntity();
            }
            string pressName = "梦想人";
            if (bookEntity.publishID > 0)
            {
                string strSqlPress = $"SELECT PressID,PressName FROM {SystemDBConfig.Press} WITH(NOLOCK) WHERE PressID={bookEntity.publishID}";
                DataTable dtPress = SqlDataAccess.sda.ExecSqlTableQuery(strSqlPress);
                if(dtPress!=null&&dtPress.Rows.Count>0)
                {
                    pressName = dtPress.Rows[0]["PressName"].ToString();
                }
            }

            string strSql = $@"
                SELECT dba.f_create_time AS 时间
		                ,ui.userNickName AS 用户昵称
		                ,ui.userID AS 用户ID
		                ,(dba.f_region+'-'+f_city) AS 区域
		                ,'' AS 书名
		                ,'' AS 出版社		
		                ,'下载电子书' AS 类型
                FROM book_store_data_collection.dbo.t_downloaded_book_data dba WITH(NOLOCK)
	                LEFT JOIN magic_book.dbo.T_User_Device_List udl WITH(NOLOCK) ON udl.F_DeviceID=dba.f_device_id
	                LEFT JOIN userdata.dbo.UserInfo ui WITH(NOLOCK) ON ui.userID=udl.F_UserID
                WHERE dba.f_source_type=4 AND dba.f_source_key_int={cusId} AND (dba.f_create_time BETWEEN '{dateStart}' AND '{ConvertHelper.ToDateTime( dateEnd).AddDays(1)}')
                ORDER BY dba.f_create_time DESC";
                
            DataTable dtResult = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if(dtResult==null||dtResult.Rows.Count==0)
            {
                return null;
            }
            foreach(DataRow dr in dtResult.Rows)
            {
                dr["书名"] = dtCus.Rows[0]["F_Customization_Name"].ToString();
                dr["出版社"] = pressName;
            }
            return dtResult;
        }
    }
}