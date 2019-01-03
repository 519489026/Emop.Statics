using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace StatisticalCenter.DataAccess.TableMagicBook
{
    public class dream_multimedia_book
    {
        /// <summary>
        /// 获取所有图书列表，用于构建下拉框
        /// </summary>
        /// <returns></returns>
        public DataTable GetBookTable(int PressID,int userId)
        {
            string sqlWhere = "";
            if (PressID != 0)
            {
                sqlWhere += $" AND Press={PressID} ";
            }
            if(userId!=0)
            {
                sqlWhere += $" AND ap.APUserId={userId} ";
            }
            string strSql = $@"
                SELECT dmb.[BookGUID],dmb.[BookName],dmb.[Press]
                FROM {SystemDBConfig.Dream_Multimedia_Book} dmb WITH(NOLOCK)
	                INNER JOIN {SystemDBConfig.T_AdminPress} ap WITH(NOLOCK) ON ap.APPressId=dmb.Press
                WHERE visible = 1 " +sqlWhere;
            return SqlDataAccess.sda.ExecSqlTableQuery(strSql);
        }
    }
}
