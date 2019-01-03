using System;
using System.Linq;

namespace StatisticalCenter.DataAccess.TableDataCollection
{
    public class T_CountToolHistory
    {
        /// <summary>
        /// 保存统计二维码数据
        /// </summary>
        /// <param name="qId">二维码Id</param>
        /// <param name="phoneType">设备类型</param>
        /// <param name="ip">IP地址</param>
        /// <param name="province">省份</param>
        /// <param name="enterType">进入模式1打开页面，2扫码（暂不用），3下载</param>
        /// <param name="cookie">用户cookie</param>
        /// <returns></returns>
        public bool SaveQrCodeStatistical(int qId, int phoneType, string ip, string province, int enterType, string cookie)
        {
            try
            {
                string p = string.Empty;
                string c = string.Empty;
                var addressArr = province.Split(';');

                if (!addressArr.Any() || addressArr.Length < 3)
                {
                    p = "其他";
                    c = "其他";
                }
                else
                {
                    p = addressArr[1];
                    c = addressArr[2];
                }

                var sql = $@"
                INSERT INTO {SystemDBConfig.CountToolHistory}(
                    CTHType,
                    CTH_CTRId,
                    CTHIp,
                    CTHProvince,
                    CTHCity,
                    CTHDeviceType,
                    CTHVisitTime,
                    CTHVisitDate,
                    CTCookie
                ) VALUES(
                    {enterType},
                    {qId},
                    '{ip}',
                    '{p}',
                    '{c}',
                    {phoneType},
                    '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',
                    '{DateTime.Now.ToString("yyyy-MM-dd")}',
                    '{cookie}'
                )
            ";

                return SqlDataAccess.sda.ExecuteNonQuery(sql) > 0;
            }
            catch (Exception e)
            {
                var dd = e;
                throw;
            }
        }
    }
}