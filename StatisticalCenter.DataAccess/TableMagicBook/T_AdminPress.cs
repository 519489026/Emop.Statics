using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace StatisticalCenter.DataAccess.TableMagicBook
{
    public class T_AdminPress
    {
        /// <summary>
        /// 获取用户名下的出版社
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataTable GetPublishTable(int userId)
        {
            string strSql = $@"
            SELECT pr.PressID,pr.PressName 
            FROM {SystemDBConfig.T_AdminPress} ap
                INNER JOIN {SystemDBConfig.Press} pr ON pr.PressID = ap.APPressId
            WHERE APStatus=1 AND ap.APUserId={userId}";
            return SqlDataAccess.sda.ExecSqlTableQuery(strSql);
        }
    }
}
