using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalCenter.Common
{
    public class MXREncryption
    {
        protected const int PACKET_HEADER_SIZE = 5;
        public static string MD5(string input)
        {
            string res;
            MD5 m = new MD5CryptoServiceProvider();
            byte[] s = m.ComputeHash(UnicodeEncoding.UTF8.GetBytes(input));
            res = BitConverter.ToString(s);
            res = res.ToLower();
            res = res.Replace("-", "");
            return res;
        }
        public static string MD5(FileStream input)
        {
            string res;
            MD5 m = new MD5CryptoServiceProvider();
            byte[] s = m.ComputeHash(input);
            res = BitConverter.ToString(s);
            res = res.ToLower();
            res = res.Replace("-", "");
            return res;
        }
        public static string MXR(string strSndBuf, bool bEncryption)
        {
            try
            {
                byte[] pSndBuf = UTF8Encoding.UTF8.GetBytes(strSndBuf);
                int iSize = pSndBuf.Length;

                byte[] pTempBuffer = new byte[iSize + PACKET_HEADER_SIZE];

                if (bEncryption)
                {
                    do
                    {
                        Random rd = new Random();
                        int iRand = rd.Next();
                        pTempBuffer[0] = Convert.ToByte(iRand % 128);
                    }
                    while (pTempBuffer[0] == 0);
                }
                else
                {
                    pTempBuffer[0] = 0;
                }

                byte[] byteLength = BitConverter.GetBytes(Convert.ToInt32(iSize + PACKET_HEADER_SIZE));

                pTempBuffer[1] = byteLength[0];
                pTempBuffer[2] = byteLength[1];
                pTempBuffer[3] = byteLength[2];
                pTempBuffer[4] = byteLength[3];

                if (pTempBuffer[0] != 0)
                {
                    for (int i = 0; i < iSize; i++)
                    {
                        pTempBuffer[PACKET_HEADER_SIZE + i] = Convert.ToByte((Convert.ToInt32(pSndBuf[i]) + (i ^ (Convert.ToInt32(pTempBuffer[0])))) % 256);
                        pTempBuffer[PACKET_HEADER_SIZE + i] = Convert.ToByte((Convert.ToInt32(pTempBuffer[PACKET_HEADER_SIZE + i]) ^ (Convert.ToInt32(pTempBuffer[0]) ^ (iSize - i))) % 256);
                    }
                }
                else
                {
                    for (int i = 0; i < iSize; i++)
                    {
                        pTempBuffer[PACKET_HEADER_SIZE + i] = pSndBuf[i];
                    }
                }

                return Convert.ToBase64String(pTempBuffer);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}


