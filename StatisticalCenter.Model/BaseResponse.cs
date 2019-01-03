using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace StatisticalCenter.Model
{
    public class BaseResponse
    {
        /// <summary>
        /// 返回记录总数
        /// </summary>
        public int recordCount { get; set; } = 0;

        /// <summary>
        /// 返回列表
        /// </summary>
        public DataTable list { get; set; } = new DataTable();


    }
}
