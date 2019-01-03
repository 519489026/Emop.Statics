using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace StatisticalCenter.DataAccess.TableArShop
{
    /// <summary>
    /// 账号
    /// </summary>
    public class MAS_ADMIN_ACCOUNT
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public DataTable getUserLogin(string account, string pwd)
        {

            SqlParameterCollection spc = new SqlCommand().Parameters;
            StringBuilder strSql = new StringBuilder("", 400);

            strSql.AppendFormat($@"
                SELECT MAS_ADMIN_ID,MAS_ADMIN_NAME,ISNULL(MAS_ADMIN_NICKNAME,'点读书小编') MAS_ADMIN_NICKNAME 
                FROM {SystemDBConfig.MAS_ADMIN_ACCOUNT}
                WHERE MAS_ADMIN_USED='0' AND MAS_ADMIN_NAME = @MAS_ADMIN_NAME and IS_VALID=1 and IS_PASS=1 
                    AND MAS_ADMIN_PWD= sys.fn_VarBinToHexStr(hashbytes('MD5', @MAS_ADMIN_PWD)) ");
            spc.Add("@MAS_ADMIN_NAME", SqlDbType.VarChar, 50).Value = account;
            spc.Add("@MAS_ADMIN_PWD", SqlDbType.VarChar, 34).Value = pwd;
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSql.ToString(), spc);
            return dt;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public DataTable getUserLogin(int userId)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.AppendFormat($@"
                SELECT MAS_ADMIN_ID,MAS_ADMIN_NAME,ISNULL(MAS_ADMIN_NICKNAME,'点读书小编') MAS_ADMIN_NICKNAME 
                FROM {SystemDBConfig.MAS_ADMIN_ACCOUNT} WITH(NOLOCK)
                WHERE  MAS_ADMIN_ID={userId} AND IS_VALID=1 AND IS_PASS=1 ");
            DataTable dt = SqlDataAccess.sda.ExecSqlTableQuery(strSql.ToString());
            return dt;
        }
    }
}
