using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace StatisticalCenter.DataAccess.TableArShop
{
    /// <summary>
    /// 系统配置逻辑
    /// </summary>
    public class MAS_SYSTEMCONFIG
    {
        /// <summary>
        /// 根据KEY值获取Value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueByKey(string key)
        {
            string strSql = $@"SELECT [SCValue]
              FROM {SystemDBConfig.MAS_SYSTEMCONFIG} WITH(NOLOCK) 
              WHERE SCKey='{key.Replace("'", "")}'";
            DataTable dt= SqlDataAccess.sda.ExecSqlTableQuery(strSql);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["SCValue"].ToString();
            }
            return string.Empty;
        }
    }
}
