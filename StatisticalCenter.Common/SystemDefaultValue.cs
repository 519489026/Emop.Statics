using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalCenter.Common
{
    /// <summary>
    /// 系统默认值
    /// </summary>
    public class SystemDefaultValue
    {
        /// <summary>
        /// 字符串
        /// </summary>
        public static string StringValue = string.Empty;

        /// <summary>
        /// 日期默认值 
        /// </summary>
        public static DateTime DateTimeValue = Convert.ToDateTime("1900-01-01");

        /// <summary>
        /// Decimal默认值
        /// </summary>
        public static decimal DecimalValue = 0.0M;

        /// <summary>
        /// Int默认值
        /// </summary>
        public static int IntValue = 0;

        /// <summary>
        /// Short默认值
        /// </summary>
        public static short ShortValue = 0;
    }
}
