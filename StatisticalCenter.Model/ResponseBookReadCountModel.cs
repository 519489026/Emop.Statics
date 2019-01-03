using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalCenter.Model
{
    public class ResponseBookReadCountModel:BaseResponse
    {
        /// <summary>
        /// 阅读人数
        /// </summary>
        public int totalPersonCount { get; set; } = 0;
        /// <summary>
        /// 平均阅读时长
        /// </summary>
        public long avgTimeLength { get; set; } = 0;
        /// <summary>
        /// 阅读总次数
        /// </summary>
        public long totalReadCount { get; set; } = 0;
    }
}
