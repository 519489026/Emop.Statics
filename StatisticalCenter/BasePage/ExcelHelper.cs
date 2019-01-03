using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace StatisticalCenter.BasePage
{
    public class ExcelHelper
    {
        /// <summary>
        /// 导出文件至CSV文件
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="strFileName">导出后的文件名，不要加后缀名</param>
        public void ExportDataGridToCSV(DataTable dt, string strFileName)
        {
            //string strFile = "";
            string path = HttpContext.Current.Server.MapPath("/datafiles/syllabusExcel");

            //File info initialization
            if (!strFileName.EndsWith(".csv"))
            {
                strFileName = strFileName + ".csv";
            }
            strFileName = path + "/" + strFileName;
            //path = HttpContext.Current.Server.MapPath(strFile);
            if (File.Exists(strFileName))
            {
                File.Delete(strFileName);
            }

            System.IO.FileStream fs = new FileStream(strFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, new System.Text.UnicodeEncoding());
            //Tabel header
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sw.Write(dt.Columns[i].ColumnName);
                sw.Write("\t");
            }
            sw.WriteLine("");
            //Table body
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sw.Write(dt.Rows[i][j].ToString().TrimEnd('\t'));
                    sw.Write("\t");
                }
                sw.WriteLine("");
            }
            sw.Flush();
            sw.Close();

            DownLoadFile(strFileName, strFileName.Split('/')[strFileName.Split('/').Length - 1]);
        }

        private bool DownLoadFile(string _FileName, string fileNameLoad)
        {
            _FileName = _FileName.Replace("/", "\\");
            try
            {
                System.IO.FileStream fs = System.IO.File.OpenRead(_FileName);
                byte[] FileData = new byte[fs.Length];
                fs.Read(FileData, 0, (int)fs.Length);
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Charset = "UTF-8";
                HttpContext.Current.Response.AddHeader("Content-Type", "application/ms-excel");
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                //string FileName = System.Web.HttpUtility.UrlEncode(System.Text.Encoding.UTF8.GetBytes(_FileName));

                string FileName = _FileName.Split('\\')[_FileName.Split('\\').Length - 1];
                if (HttpContext.Current.Request.UserAgent.ToLower().IndexOf("firefox") >= 0)
                {
                    FileName = "\"" + FileName + "\"";
                }
                else
                {
                    FileName = System.Web.HttpUtility.UrlEncode(System.Text.Encoding.UTF8.GetBytes(FileName));
                }
                //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(fileNameLoad, System.Text.Encoding.UTF8));
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
                HttpContext.Current.Response.AddHeader("Content-Length", fs.Length.ToString());
                HttpContext.Current.Response.BinaryWrite(FileData);
                fs.Close();
                System.IO.File.Delete(_FileName);
                HttpContext.Current.Response.Flush();
                return true;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;
            }

        }
    }
}