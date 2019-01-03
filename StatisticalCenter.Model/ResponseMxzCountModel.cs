using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalCenter.Model
{
    public class ResponseMxzCountModel:BaseResponse
    {
        /// <summary>
        /// 总计人数
        /// </summary>
        public int totalPersonCount { get; set; } = 0;

        /// <summary>
        /// 总计数量
        /// </summary>
        public int totalCoinCount { get; set; } = 0;
    }
}
