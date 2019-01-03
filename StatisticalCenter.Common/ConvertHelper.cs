using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalCenter.Common
{
    /// <summary>
    /// 数据转换类
    /// </summary>
    public class ConvertHelper
    {
        /// <summary>
        /// 转换为整型数字，出错时返回0
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int ToInt32(object num)
        {
            try
            {
                return Convert.ToInt32(num);
            }
            catch
            {
                return 0;

            }
        }
        /// <summary>
        /// 转换为短整型数字，出错时返回0
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static short ToInt16(object num)
        {
            try
            {
                return Convert.ToInt16(num);
            }
            catch
            {
                return 0;

            }
        }
        /// <summary>
        /// 转换为长整型数字，出错时返回0
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static long ToInt64(object num)
        {
            try
            {
                return Convert.ToInt64(num);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 转为decimal
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static decimal ToDecimal(object num)
        {
            try
            {
                return Convert.ToDecimal(num);
            }
            catch
            {
                return 0.0M;
            }
        }

        /// <summary>
        /// 转换为日期类型
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(object date)
        {
            try
            {
                return Convert.ToDateTime(date);
            }
            catch
            {
                return Convert.ToDateTime("1900-01-01");
            }
        }

        /// <summary>
        /// 转换成BOOL类型
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ToBoolean(object b)
        {
            try
            {
                return Convert.ToBoolean(b);
            }
            catch
            {
                return true;
            }
        }
    }
}
