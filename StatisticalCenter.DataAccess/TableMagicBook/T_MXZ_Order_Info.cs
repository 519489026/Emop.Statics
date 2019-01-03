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
    /// 梦想钻订单记录
    /// </summary>
    public class T_MXZ_Order_Info
    {
        /// <summary>
        /// 获取梦想钻日统计信息
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页长</param>
        /// <param name="type">类型</param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable GetMXZOrderCountTable(int pageIndex, int pageSize, string type, string dateStart, string dateEnd,ref int recordCount)
        {
            DateTime dtStart = ConvertHelper.ToDateTime(dateStart);
            DateTime dtEnd = ConvertHelper.ToDateTime(dateEnd);
            recordCount = (int)(dtEnd - dtStart).TotalDays + 1;

            dtStart = dtStart.AddDays((pageIndex-1) * pageSize);
            dtEnd = dtEnd.AddDays(pageIndex * pageSize);

            string strSql = $@"
                SELECT ROW_NUMBER() OVER(ORDER BY CONVERT(VARCHAR(10),F_Create_Time,120) DESC) AS RowNum
                    ,CONVERT(VARCHAR(10),F_Create_Time,120) AS F_Create_Time
                    ,SUM([F_Order_MXZ_NUM]) AS MXZCount
                    ,COUNT(1) AS PersonCount
                FROM {SystemDBConfig.T_MXZ_Order_Info} WITH(NOLOCK)
                WHERE F_Create_Time >= '{dtStart}' AND F_Create_Time < '{dtEnd}'
                GROUP BY CONVERT(VARCHAR(10), F_Create_Time, 120)";
            DataTable dtCount = SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            return dtCount;
        }
    }
}
