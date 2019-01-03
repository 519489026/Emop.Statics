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
    /// 梦想钻变动记录
    /// </summary>
    public class T_User_MXZ_History
    {
        /// <summary>
        /// 获取梦想钻日统计信息
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页长</param>
        /// <param name="type">类型,2充值，4赠送，0梦想币-梦想钻，1梦想钻-梦想币，3消费</param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable GetMXZOrderCountTable(int pageIndex, int pageSize, int type, string dateStart, string dateEnd,ref int totalPersonCount,ref int totalCoinCount, ref int recordCount)
        {
            DateTime dtStart = ConvertHelper.ToDateTime(dateStart);
            DateTime dtEnd = ConvertHelper.ToDateTime(dateEnd);
            recordCount = (int)(dtEnd - dtStart).TotalDays + 1;
            string strWhere = string.Empty;
            if (type > 0)
            {
                strWhere = " AND hi.F_Source_Type=" + type;
            }
            else
            {
                strWhere = " AND hi.F_Source_Type IN (2,4)";
            }

            StringBuilder sbSql = new StringBuilder();
            string tempTableName = "#TEMP" + Guid.NewGuid().ToString().Replace("-", "");
            sbSql.Append($@"
                CREATE TABLE {tempTableName}
                (
                    RowNum INT,
                    F_Create_Time VARCHAR(10)
                );");
            int index = 1;
            while(dtEnd>dtStart)
            {
                sbSql.Append($"INSERT INTO {tempTableName} VALUES({index},'{dtEnd.ToString("yyyy-MM-dd")}');");
                dtEnd = dtEnd.AddDays(-1);
                index++;
            }

            sbSql.Append($@"
                SELECT temp.RowNum,temp.F_Create_Time,ISNULL(A.MXZCount,0) AS MXZCount,ISNULL(A.PersonCount,0) AS PersonCount
                FROM {tempTableName} temp
                    LEFT JOIN (
                        SELECT CONVERT(VARCHAR(10),hi.F_Create_Time,120) AS F_Create_Time
                            ,SUM(hi.F_MXZ_NUM) AS MXZCount
                            ,COUNT(DISTINCT hi.F_User_Id) AS PersonCount
                        FROM {SystemDBConfig.T_User_MXZ_History} hi WITH(NOLOCK)
                        WHERE hi.F_Create_Time >= '{dateStart}' AND hi.F_Create_Time < '{ConvertHelper.ToDateTime(dateEnd).AddDays(1)}' {strWhere}
                        GROUP BY CONVERT(VARCHAR(10),hi.F_Create_Time, 120)) AS A ON A.F_Create_Time=temp.F_Create_Time
                WHERE temp.RowNum BETWEEN {(pageIndex - 1) * pageSize + 1} AND {pageIndex * pageSize}
                ORDER BY temp.F_Create_Time DESC;");
            sbSql.Append($"DROP TABLE {tempTableName}");
            DataTable dtCount = SqlDataAccess.sda.ExecSqlTableQuery(sbSql.ToString());

            string strSqlTotalCount = $@"
                SELECT SUM(hi.F_MXZ_NUM) AS MXZCount
                   ,COUNT(DISTINCT F_User_Id) AS PersonCount
                FROM { SystemDBConfig.T_User_MXZ_History} hi WITH(NOLOCK)
                WHERE hi.F_Create_Time >= '{dateStart}' AND hi.F_Create_Time < '{ConvertHelper.ToDateTime(dateEnd).AddDays(1)}' { strWhere}";
            DataTable dtTotalCount = SqlDataAccess.sda.ExecSqlTableQuery(strSqlTotalCount);
            if(dtTotalCount!=null&&dtTotalCount.Rows.Count>0)
            {
                totalCoinCount = ConvertHelper.ToInt32(dtTotalCount.Rows[0]["MXZCount"]);
                totalPersonCount = ConvertHelper.ToInt32(dtTotalCount.Rows[0]["PersonCount"]);
            }
            return dtCount;
        }

        /// <summary>
        /// 获取梦想钻日统计信息(不分页，用于导出)
        /// </summary>
        /// <param name="type">类型,2充值，4赠送，0梦想币-梦想钻，1梦想钻-梦想币，3消费</param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <returns></returns>
        public DataTable GetMXZOrderCountTable(int type, string dateStart, string dateEnd)
        {
            DateTime dtStart = ConvertHelper.ToDateTime(dateStart);
            DateTime dtEnd = ConvertHelper.ToDateTime(dateEnd).AddDays(1);
            string strWhere = string.Empty;
            if (type > 0)
            {
                strWhere = " AND hi.F_Source_Type=" + type;
            }
            else
            {
                strWhere = " AND hi.F_Source_Type IN (2,4)";
            }

            StringBuilder sbSql = new StringBuilder();
            string tempTableName = "#TEMP" + Guid.NewGuid().ToString().Replace("-", "");
            sbSql.Append($@"
                CREATE TABLE {tempTableName}
                (
                    RowNum INT,
                    F_CreateTime VARCHAR(10)
                );");
            int index = 1;
            while (dtEnd > dtStart)
            {
                sbSql.Append($"INSERT INTO {tempTableName} VALUES({index},'{dtStart.ToString("yyyy-MM-dd")}');");
                dtStart =dtStart.AddDays(1);
                index++;
            }

            sbSql.Append($@"
                SELECT temp.F_CreateTime AS 创建时间,ISNULL(A.梦想钻,0) AS 梦想钻,ISNULL(A.人数,0) AS 人数
                FROM {tempTableName} temp
                    LEFT JOIN(    
                        SELECT CONVERT(VARCHAR(10),hi.F_Create_Time,120) AS 创建时间
                            ,SUM(hi.F_MXZ_NUM) AS 梦想钻
                            ,COUNT(DISTINCT hi.F_User_ID) AS 人数
                    FROM {SystemDBConfig.T_User_MXZ_History} hi WITH(NOLOCK) 
                    WHERE hi.F_Create_Time >= '{dateStart}' AND hi.F_Create_Time < '{ConvertHelper.ToDateTime(dateEnd).AddDays(1)}' {strWhere}
                    GROUP BY CONVERT(VARCHAR(10), hi.F_Create_Time, 120)) AS A 
                 ON A.创建时间=temp.F_CreateTime
                ORDER BY 创建时间;");
            sbSql.Append($"DROP TABLE {tempTableName} ");

            DataTable dtCount = SqlDataAccess.sda.ExecSqlTableQuery(sbSql.ToString());
            return dtCount;
        }

        /// <summary>
        /// 获取梦想钻明细信息(不分页，用于导出)
        /// </summary>
        /// <param name="type">类型,2充值，4赠送，0梦想币-梦想钻，1梦想钻-梦想币，3消费</param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <returns></returns>
        public DataTable GetMXZOrderDetails(int type, string dateStart, string dateEnd)
        {
            string strWhere = string.Empty;
            if (type > 0)
            {
                strWhere = " AND hi.F_Source_Type=" + type;
            }
            else
            {
                strWhere = " AND hi.F_Source_Type IN (2,4)";
            }
            string strSql = $@"
                SELECT CONVERT(VARCHAR(19),hi.F_Create_Time,120) AS 时间
                    ,ISNULL(hi.F_MXZ_NUM,0) AS 梦想钻
                    ,ISNULL(hi.F_User_ID,0) AS 用户ID
                    ,ISNULL(ui.userNickName,0) AS 用户名称
                    ,ISNULL((CASE hi.F_Source_Type WHEN 2 THEN '充值' ELSE '赠送' END ),'') AS 获得渠道
                FROM {SystemDBConfig.T_User_MXZ_History} hi WITH(NOLOCK)
                    LEFT JOIN {SystemDBConfig.UserInfo} ui WITH(NOLOCK) ON ui.userID = hi.F_User_ID
                WHERE   hi.F_Create_Time >= '{dateStart}' 
                    AND hi.F_Create_Time < '{ConvertHelper.ToDateTime(dateEnd).AddDays(1)}' 
                    {strWhere}
                ORDER BY hi.F_Create_Time DESC";
            return SqlDataAccess.sda.ExecSqlTableQuery(strSql);
        }
    }
}
