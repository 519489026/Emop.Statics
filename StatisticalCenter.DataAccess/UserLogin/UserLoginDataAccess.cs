using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalCenter.DataAccess.UserLogin
{
    public class UserLoginDataAccess
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
            SqlDataAccess sda = new SqlDataAccess();
            StringBuilder strSql = new StringBuilder("", 400);

            strSql.AppendFormat($" SELECT MAS_ADMIN_ID,MAS_ADMIN_NAME,MAS_FIRST_LOGIN,MAS_ADMIN_MAIL,ISNULL(MAS_ADMIN_NICKNAME,'点读书小编') MAS_ADMIN_NICKNAME FROM {SystemDBConfig.MAS_ADMIN_ACCOUNT} ");
            strSql.Append(" WHERE  MAS_ADMIN_USED='0' AND MAS_ADMIN_NAME = @MAS_ADMIN_NAME and IS_VALID=1 and IS_PASS=1 ");
            strSql.Append(" AND MAS_ADMIN_PWD= sys.fn_VarBinToHexStr(hashbytes('MD5', @MAS_ADMIN_PWD)) ");

            spc.Add("@MAS_ADMIN_NAME", SqlDbType.VarChar, 50).Value = account;
            spc.Add("@MAS_ADMIN_PWD", SqlDbType.VarChar, 34).Value = pwd;


            DataTable dt = sda.ExecSqlReader(strSql.ToString(), spc).Tables[0];

            return dt;
        }
    }
}
