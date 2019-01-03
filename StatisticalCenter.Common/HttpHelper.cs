using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
namespace StatisticalCenter.Common
{
    /// <summary>
    /// HTTP请求通用类
    /// </summary>
    public class HttpHelper
    {
        /// <summary>
        /// GET方法
        /// </summary>
        /// <param name="strUrl">目标URL</param>
        /// <param name="strData">传送数据</param>
        /// <param name="strContentType">传送数据类型，默认text/xml</param>
        /// <returns></returns>
        public string HttpGet(string strUrl, string strContentType = "text/xml")
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                httpWebRequest.ContentType = strContentType;
                httpWebRequest.Method = "GET";
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string result = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return result;
            }
            catch (Exception e)
            {
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strUrl">目标URL</param>
        /// <param name="strData">传送数据</param>
        /// <param name="strContentType">传送数据类型，默认application/json</param>
        /// <returns></returns>
        public string HttpPost(string strUrl, string strData, string strContentType = "application/json")
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentType = strContentType;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.KeepAlive = true;
            byte[] data = Encoding.UTF8.GetBytes(strData);

            request.ContentLength = data.Length;

            using (Stream s = request.GetRequestStream())
            {
                s.Write(data, 0, data.Length);
            }

            string result = "";
            try
            {
                using (StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}
