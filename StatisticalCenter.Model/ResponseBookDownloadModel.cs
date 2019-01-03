using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalCenter.Model
{
    public class ResponseBookDownloadModel : BaseResponse
    {
        /// <summary>
        /// 总计下载人数
        /// </summary>
        public int totalPersonCount { get; set; }

        /// <summary>
        /// 总计下载次数
        /// </summary>
        public int totalDownLoadCount { get; set; }
    }
}
