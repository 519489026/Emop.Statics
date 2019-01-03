using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using StatisticalCenter.Common;
using StatisticalCenter.DataAccess;
namespace StatisticalCenter
{
    public partial class DataUpdate : BasePage.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //UpdateData();
        }

        protected void btnCount_Click(object sender, EventArgs e)
        {
            int type = ConvertHelper.ToInt32(sltType.SelectedItem.Value);
            DateTime dtStart = ConvertHelper.ToDateTime(txtStartTime.Text);
            DateTime dtEnd = ConvertHelper.ToDateTime(txtEndTime.Text);
            if (string.IsNullOrEmpty(txtStartTime.Text))
            {
                dtStart = DateTime.Now.Date.AddYears(-2);
            }
            if (string.IsNullOrEmpty(txtEndTime.Text))
            {
                dtEnd = DateTime.Now.Date.AddDays(-1);
            }
            switch (type)
            {
                case 1: CountBook(dtStart.ToShortDateString(), dtEnd.ToShortDateString()); break;
                case 2: CountDownLoad(dtStart.ToShortDateString(), dtEnd.ToShortDateString()); break;
                case 3: CountReadLog(dtStart.ToShortDateString(), dtEnd.ToShortDateString()); break;
                case 4: CountTwoCode(dtStart.ToShortDateString(), dtEnd.ToShortDateString()); break;
                case 5: CountReadLength(dtStart.ToShortDateString(), dtEnd.ToShortDateString()); break;
                case 6: CountCustionDownload(dtStart.ToShortDateString(), dtEnd.ToShortDateString()); break;
                case 7:CountBannerClick(dtStart.ToShortDateString(), dtEnd.ToShortDateString());break;
            }
        }

        private void CountBook(string startDate, string endDate)
        {
            DateTime dtStart = ConvertHelper.ToDateTime(startDate);
            DateTime dtEnd = ConvertHelper.ToDateTime(endDate);

            while (dtStart < dtEnd)
            {
                int result = 0;
                string strSql = $@"
                    INSERT INTO {SystemDBConfig.T_DevicePurchase_History_Count}
                         ([DHCBookGuid],[DHCCoinCount],[DHCRowCount],[DHCCountDate],[DHCCreateTime])
                    SELECT F_PurchaseContent,SUM(F_UsedCoinNum) AS AllSum,COUNT(1) AS AllCount,'{dtStart}',GETDATE() 
                    FROM {SystemDBConfig.T_DevicePurchase_History} dph WITH(NOLOCK)
	                    LEFT JOIN {SystemDBConfig.T_DevicePurchase_History_Count} dhc WITH(NOLOCK) ON dhc.DHCBookGuid=dph.F_PurchaseContent AND dhc.DHCCountDate='{dtStart}' AND dph.F_CreateTime>='{dtStart}' AND dph.F_CreateTime<'{dtStart.AddDays(1)}'
                    WHERE dph.F_CreateTime>='{dtStart}' AND dph.F_CreateTime<'{dtStart.AddDays(1)}' AND dhc.DHCBookGuid IS NULL AND dph.F_PurchaseType=1
                    GROUP BY F_PurchaseContent ";
                result = SqlDataAccess.sda.ExecuteNonQuery(strSql);
                dtStart = dtStart.AddDays(1);
            }
            dtStart = ConvertHelper.ToDateTime(startDate);
            while (dtStart < dtEnd)
            {
                int result = 0;
                string strSql = $@"
                    INSERT INTO {SystemDBConfig.T_DevicePurchase_History_Count}
                         ([DHCBookGuid],[DHCMxzCount],[DHCMxzRowCount],[DHCCountDate],[DHCCreateTime])
                    SELECT TOP 20 F_PurchaseContent,SUM(F_UsedCoinNum) AS AllSum,COUNT(1) AS AllCount,'{dtStart}',GETDATE() 
                    FROM {SystemDBConfig.T_DevicePurchase_History} dph WITH(NOLOCK)
	                    LEFT JOIN {SystemDBConfig.T_DevicePurchase_History_Count} dhc WITH(NOLOCK) ON dhc.DHCBookGuid=dph.F_PurchaseContent AND dhc.DHCCountDate='{dtStart}' AND dph.F_CreateTime>='{dtStart}' AND dph.F_CreateTime<'{dtStart.AddDays(1)}'
                    WHERE dph.F_CreateTime>='{dtStart}' AND dph.F_CreateTime<'{dtStart.AddDays(1)}' AND dhc.DHCBookGuid IS NULL AND dph.F_PurchaseType=4
                    GROUP BY F_PurchaseContent ";
                result = SqlDataAccess.sda.ExecuteNonQuery(strSql);

                strSql = $@"
                    UPDATE {SystemDBConfig.T_DevicePurchase_History_Count}
                    SET DHCMxzCount = A.CoinNum, DHCMxzRowCount = A.AllCount
                    FROM(
                        SELECT COUNT(1) AS AllCount
                            , SUM(F_UsedCoinNum) AS CoinNum
                            , F_PurchaseContent
                            , CONVERT(VARCHAR(10), F_CreateTime, 120) AS CountDate
                        FROM {SystemDBConfig.T_DevicePurchase_History}
                        WHERE F_PurchaseType = 4 AND F_CreateTime > '{dtStart}' AND F_CreateTime < '{dtStart.AddDays(1)}'
                        GROUP BY F_PurchaseContent, CONVERT(VARCHAR(10), F_CreateTime, 120)
                    ) AS A
                    WHERE A.CountDate = {SystemDBConfig.T_DevicePurchase_History_Count}.DHCCountDate 
                        AND A.F_PurchaseContent = T_DevicePurchase_History_COUNT.DHCBookGuid ";
                SqlDataAccess.sda.ExecuteNonQuery(strSql);

                if (result < 20)
                {
                    dtStart = dtStart.AddDays(1);
                }
            }
        }

        private void CountDownLoad(string startDate, string endDate)
        {
            DateTime dtStart = ConvertHelper.ToDateTime(startDate);
            DateTime dtEnd = ConvertHelper.ToDateTime(endDate);
            while (dtStart < dtEnd)
            {
                int result = 1;
                while (result > 0)
                {
                    string strSql = string.Format($@"
                INSERT INTO {SystemDBConfig.download_book_count}
                    ([DBCBookGuid], [DBCCountDate], [DBCCount], [DBCCreateTime])
                SELECT db.isbn,'{dtStart}',COUNT(db.flow_id),GETDATE()
                FROM {SystemDBConfig.download_book} db WITH(NOLOCK)
                    LEFT JOIN {SystemDBConfig.download_book_count} dbc WITH(NOLOCK) ON dbc.DBCBookGuid COLLATE Chinese_PRC_90_CI_AS  = db.isbn AND dbc.DBCCountDate = '{dtStart}'
                WHERE dbc.DBCId IS NULL AND db.time BETWEEN '{dtStart}' AND '{dtStart.AddDays(1)}'
                GROUP BY db.isbn");
                    result = SqlDataAccess.sda.ExecuteNonQuery(strSql);
                }
                result = 1;
                string strSql2 = $@"
                UPDATE {SystemDBConfig.download_book_count}
                SET DBCRegisterCount = A.AllCount, DBCRegisterPersonCount = A.PersonCount
                FROM(
                SELECT isbn, '{dtStart}' AS CountDate, COUNT(1) AS AllCount, COUNT(DISTINCT userID) AS PersonCount
                FROM {SystemDBConfig.download_book} WITH(NOLOCK)
                WHERE userid > 0 AND time >= '{dtStart}' AND time < '{dtStart.AddDays(1)}'
                GROUP BY isbn, CONVERT(VARCHAR(10), time, 120)
                ) AS A
                WHERE A.isbn COLLATE Chinese_PRC_90_CI_AS = download_book_count.DBCBookGuid  AND A.CountDate = download_book_count.DBCCountDate";
                result = SqlDataAccess.sda.ExecuteNonQuery(strSql2);

                result = 1;

                string strSql3 = $@"
                UPDATE {SystemDBConfig.download_book_count}
                SET DBCNoRegisterPersonCount = A.PersonCount
                FROM(
                SELECT isbn, '{dtStart}' AS CountDate, COUNT(DISTINCT phoneID) AS PersonCount
                FROM {SystemDBConfig.download_book} WITH(NOLOCK)
                WHERE userid = 0 AND time >= '{dtStart}' AND time < '{dtStart.AddDays(1)}'
                GROUP BY isbn, CONVERT(VARCHAR(10), time, 120)
                ) AS A
                WHERE A.isbn COLLATE Chinese_PRC_90_CI_AS = download_book_count.DBCBookGuid  AND A.CountDate = download_book_count.DBCCountDate";
                result = SqlDataAccess.sda.ExecuteNonQuery(strSql3);
                dtStart = dtStart.AddDays(1);
            }
        }

        private void CountReadLog(string startDate, string endDate)
        {
            DateTime dtStart = ConvertHelper.ToDateTime(startDate);
            DateTime dtEnd = ConvertHelper.ToDateTime(endDate);
            while (dtStart < dtEnd)
            {
                string strSql = $@"
                    INSERT INTO {SystemDBConfig.T_Book_ReadingDuration_Logs_Count}
                        ([BRCDuration],[BRCCount],[BRCPersonCount],[BRCBookGuid],[BRCCountDate],[BRCCreateTime])
                    SELECT SUM(F_ReadingDuration),COUNT(1),COUNT(DISTINCT F_DeviceId),F_BookGuid,'{dtStart}',GETDATE()
                    FROM {SystemDBConfig.T_Book_ReadingDuration_Logs} brl WITH(NOLOCK)
                        LEFT JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs_Count} brc WITH(NOLOCK) ON brc.BRCBookGuid=brl.F_BookGuid AND brc.BRCCountDate= '{dtStart}'
                    WHERE F_CreateTime BETWEEN '{dtStart}' AND '{dtStart.AddDays(1)}' AND brc.BRCBookGuid IS NULL
                    GROUP BY F_BookGuid";
                SqlDataAccess.sda.ExecuteNonQuery(strSql);
                dtStart = dtStart.AddDays(1);
            }
        }
        private void CountTwoCode(string startDate, string endDate)
        {
            DateTime dtStart = ConvertHelper.ToDateTime(startDate);
            DateTime dtEnd = ConvertHelper.ToDateTime(endDate);
            int result = 0;
            while (dtStart < dtEnd)
            {
                string strSql = $@"
                    UPDATE {SystemDBConfig.T_CountToolCount}
                    SET CTCDownloadCount = A.AllCount
                    FROM(
                    SELECT hi.CTH_CTRId, COUNT(1) AS AllCount
                    FROM {SystemDBConfig.T_CountToolHistory} hi
                         INNER JOIN {SystemDBConfig.T_CountToolCount} ct ON ct.CTC_CTRId = hi.CTH_CTRId AND ct.CTCCountDate = '{dtStart}'
                    WHERE hi.CTHType = 3 AND ct.CTC_CTRId > 0 AND hi.CTHVisitDate = '{dtStart}'
                    GROUP BY hi.CTH_CTRId) AS A
                    WHERE A.CTH_CTRId = {SystemDBConfig.T_CountToolCount}.CTC_CTRId AND {SystemDBConfig.T_CountToolCount}.CTCCountDate='{dtStart}'";
                result += SqlDataAccess.sda.ExecuteNonQuery(strSql);

                strSql = $@"
                  UPDATE {SystemDBConfig.T_CountToolCount}
                  SET CTCBrowseCount = A.AllCount, CTCBrowsePersonCount = A.AllPersonContent
                  FROM(
                  SELECT hi.CTH_CTRId, ct.CTC_CTRId, COUNT(1) AS AllCount, COUNT(DISTINCT hi.CTHIp) AS AllPersonContent
                  FROM  {SystemDBConfig.T_CountToolHistory} hi  
                      LEFT JOIN {SystemDBConfig.T_CountToolCount} ct ON ct.CTC_CTRId = hi.CTH_CTRId AND ct.CTCCountDate = '{dtStart}'
                  WHERE hi.CTHType = 1 AND ct.CTC_CTRId > 0 AND hi.CTHVisitDate = '{dtStart}'
                  GROUP BY hi.CTH_CTRId, ct.CTC_CTRId) AS A
                  WHERE A.CTC_CTRId = {SystemDBConfig.T_CountToolCount}.CTC_CTRId AND {SystemDBConfig.T_CountToolCount}.CTCCountDate='{dtStart}'";
                result += SqlDataAccess.sda.ExecuteNonQuery(strSql);
                dtStart = dtStart.AddDays(1);
            }
        }

        /// <summary>
        /// 阅读时长统计
        /// </summary>
        private void CountReadLength(string startDate, string endDate)
        {
            DateTime dtStart = ConvertHelper.ToDateTime(startDate);
            DateTime dtEnd = ConvertHelper.ToDateTime(endDate);
            if (chkClearOldData.Checked)
            {
                string strSql = $@"TRUNCATE TABLE {SystemDBConfig.T_Book_ReadingDuration_Logs_Count}";
                SqlDataAccess.sda.ExecuteNonQuery(strSql);
            }

            while (dtStart < dtEnd)
            {
                string strSql = $@"
                    INSERT INTO {SystemDBConfig.T_Book_ReadingDuration_Logs_Count}
                        ([BRCDuration],[BRCCount],[BRCPersonCount],[BRCBookGuid],[BRCCountDate],[BRCCreateTime])
                    SELECT SUM(F_ReadingDuration),COUNT(1),COUNT(DISTINCT F_DeviceId),F_BookGuid,'{dtStart}',GETDATE()
                    FROM {SystemDBConfig.T_Book_ReadingDuration_Logs} brl WITH(NOLOCK)
                        LEFT JOIN {SystemDBConfig.T_Book_ReadingDuration_Logs_Count} brc WITH(NOLOCK) ON brc.BRCBookGuid=brl.F_BookGuid AND brc.BRCCountDate= '{dtStart}'
                    WHERE F_CreateTime BETWEEN '{dtStart}' AND '{dtStart.AddDays(1)}' AND brl.F_ReadingDuration<30000 AND brc.BRCBookGuid IS NULL
                    GROUP BY F_BookGuid";
                int result = SqlDataAccess.sda.ExecuteNonQuery(strSql);
                dtStart = dtStart.AddDays(1);
            }
        }

        /// <summary>
        /// 统计二维码下载量
        /// </summary>
        /// <returns></returns>
        private void CountCustionDownload(string startDate, string endDate)
        {
            DateTime dtStart = ConvertHelper.ToDateTime(startDate);
            DateTime dtEnd = ConvertHelper.ToDateTime(endDate);
            if (chkClearOldData.Checked)
            {
                string strSql = $@"TRUNCATE TABLE {SystemDBConfig.T_Download_Book_Count}";
                SqlDataAccess.sda.ExecuteNonQuery(strSql);
            }

            while (dtStart < dtEnd)
            {
                string strSqlSelect = $@"
                SELECT dba.f_source_key_int,ci.F_Customization_Url
                    , COUNT(1) AS AllCount, COUNT(DISTINCT dba.f_device_id) AS AllPersonCount
                FROM {SystemDBConfig.T_DownLoaded_Book_Data} dba WITH(NOLOCK)
                    INNER JOIN {SystemDBConfig.T_Customization_Info} ci WITH(NOLOCK) ON ci.F_Customization_ID=dba.f_source_key_int 
                    LEFT JOIN {SystemDBConfig.T_Download_Book_Count} dbc WITH(NOLOCK)ON dbc.cus_id = dba.f_source_key_int AND dbc.count_date='{dtStart}'
                WHERE dba.f_source_type = 1 AND dba.F_Create_Time BETWEEN '{dtStart}' AND '{dtStart.AddDays(1)}' AND dbc.cus_id IS NULL
                GROUP BY dba.f_source_key_int,ci.F_Customization_Url";

                DataTable dtResource = SqlDataAccess.sda.ExecSqlTableQuery(strSqlSelect);
                if (dtResource.Rows.Count == 0)
                {
                    dtStart = dtStart.AddDays(1);
                    continue;
                }
                dtResource.Columns.Add("toolId", typeof(int));

                string strUrls = ",";
                foreach (DataRow dr in dtResource.Rows)
                {
                    strUrls += $"'{dr["F_Customization_Url"].ToString().Replace("'", "") }',";
                }
                strUrls = strUrls.Trim(',');
                if (!string.IsNullOrEmpty(strUrls))
                {
                    strUrls = $"SELECT [CTRId],[CTRUrl] FROM {SystemDBConfig.T_CountToolResource} WITH(NOLOCK) WHERE CTRUrl IN ({strUrls})";
                    DataTable dtTool = SqlDataAccess.sda.ExecSqlTableQuery(strUrls);
                    if (dtTool != null && dtTool.Rows.Count > 0)
                    {
                        foreach (DataRow drTool in dtTool.Rows)
                        {
                            DataRow[] drArrResource = dtResource.Select($"F_Customization_Url='{drTool["CTRUrl"].ToString().Replace("'", "")}'");
                            if (drArrResource != null && drArrResource.Length > 0)
                            {
                                foreach (DataRow drResource in drArrResource)
                                {
                                    drResource["toolId"] = ConvertHelper.ToInt32(drTool["CTRId"]);
                                }
                            }
                        }
                    }
                }

                StringBuilder sbSql = new StringBuilder();
                foreach (DataRow dr in dtResource.Rows)
                {
                    sbSql.Append($@"
                    INSERT INTO {SystemDBConfig.T_Download_Book_Count}
                        ([cus_id],[count_tool_id]
                        ,[download_count],[download_person_count]
                        ,[count_date])
                    VALUES({ConvertHelper.ToInt32(dr["f_source_key_int"])},{ConvertHelper.ToInt32(dr["toolId"])}
                        , {ConvertHelper.ToInt32(dr["AllCount"])},{ConvertHelper.ToInt32(dr["AllPersonCount"])}
                        , '{dtStart}');");
                }
                int result = 0;
                if (sbSql.Length > 0)
                {
                    result= SqlDataAccess.sda.ExecuteNonQuery(sbSql.ToString());
                }
                dtStart = dtStart.AddDays(1);
                Response.Write(dtStart.ToShortDateString());
            }
        }

        public void UpdateData()
        {
            string strSqlSelect = $"SELECT TOP 10000 [F_DeviceID] FROM {SystemDBConfig.T_Device_Info}";
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSqlSelect);
            int result = 1;
            Random rd = new Random();
            while (result > 0)
            {
                int index = rd.Next(0, 9999);
                int count = rd.Next(30, 50);
                string strSql = $@"
                    UPDATE book_store_data_collection.dbo.t_downloaded_book_data 
                    SET f_device_id = '{dt.Rows[index]["F_DeviceID"].ToString()}'
                    WHERE f_create_time in (
                        SELECT top {count} f_create_time 
                        FROM book_store_data_collection.dbo.t_downloaded_book_data down
                            LEFT JOIN magic_book.dbo.T_Device_Info di ON di.F_DeviceID = down.f_device_id
                        WHERE di.F_DeviceID IS NULL)";
                result = SqlDataAccess.sda.ExecuteNonQuery(strSql);
            }
        }



        /// <summary>
        /// Banner点击统计
        /// </summary>
        /// <returns></returns>
        private void CountBannerClick(string startDate, string endDate)
        {
            DateTime dtStart = ConvertHelper.ToDateTime(startDate);
            DateTime dtEnd = ConvertHelper.ToDateTime(endDate);
            if (chkClearOldData.Checked)
            {
                string strSql = $@"TRUNCATE TABLE {SystemDBConfig.T_Device_ClickBanner_Count}";
                SqlDataAccess.sda.ExecuteNonQuery(strSql);
            }

            while (dtStart < dtEnd)
            {
                string strSql = string.Format($@"
                    INSERT INTO {SystemDBConfig.T_Device_ClickBanner_Count}
                    ([DBCBannerId],[DBCClickCount],[DBCCountDate],[DBCCreateTime])
                    SELECT dcb.[F_BannerID],COUNT(1) AS AllCount,'{dtStart}', GETDATE()
                    FROM {SystemDBConfig.T_Device_ClickBanner} dcb WITH(NOLOCK)
                        LEFT JOIN {SystemDBConfig.T_Device_ClickBanner_Count} dbc WITH(NOLOCK) ON dbc.[DBCBannerId] = dcb.F_BannerID AND dbc.DBCCountDate='{dtStart}'
                    WHERE dbc.[DBCBannerId] IS NULL AND dcb.F_CreateTime BETWEEN '{dtStart}' AND '{dtStart.AddDays(1)}'
                    GROUP BY dcb.[F_BannerID]");
                int result = SqlDataAccess.sda.ExecuteNonQuery(strSql);
                dtStart = dtStart.AddDays(1);
            }
        }

    }
}