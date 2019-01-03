using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalCenter.Model
{
    /// <summary>
    /// 梦想钻购买统计类
    /// </summary>
    public class ResponseBookBuyCountModel:BaseResponse
    {
        /// <summary>
        /// 梦想钻合计
        /// </summary>
        public int totalMxzCount { get; set; } = 0;

        /// <summary>
        /// 梦想币合计
        /// </summary>
        public int totalMxbCount { get; set; } = 0;
    }
}
