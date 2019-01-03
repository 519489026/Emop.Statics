using System;
using System.Collections.Generic;
using System.Web.UI;

namespace StatisticalCenter.Common
{
    public class CommonWeb
    {
        /// <summary>
        /// 弹出对话框
        /// </summary>
        /// <param name="pg">页面，调用时传this即可</param>
        /// <param name="msg">弹框信息内容</param>
        public static void Alert(Page pg, string msg)
        {
            pg.ClientScript.RegisterStartupScript(pg.GetType(), "myscript", string.Format("<script>alert('{0}');</script>", msg));
        }

        /// <summary>
        /// 弹出对话框，并且关闭当前界面
        /// </summary>
        /// <param name="pg">页面，调用时传this即可</param>
        /// <param name="msg">弹框信息内容</param>
        public static void AlertAndClose(Page pg, string msg)
        {
            pg.ClientScript.RegisterStartupScript(pg.GetType(), "myscript", string.Format("<script>alert('{0}');parent.layer.close(index);</script>", msg));            
        }

        /// <summary>
        /// 弹出对话框，并且关闭当前界面，并且刷新父页面
        /// </summary>
        /// <param name="pg">页面，调用时传this即可</param>
        /// <param name="msg">弹框信息内容</param>
        public static void AlertAndCloseAndReload(Page pg,string msg)
        {
            pg.ClientScript.RegisterStartupScript(pg.GetType(), "myscript", string.Format("<script>alert('{0}');var index = parent.layer.getFrameIndex(window.name);parent.layer.close(index);parent.location.reload();</script>", msg)); 
        }

        /// <summary>
        /// 调用JS函数
        /// </summary>
        /// <param name="pg">页面，调用时传this即可</param>
        /// <param name="function">函数（需要含参数）</param>
        public static void RegisterFunction(Page pg,string function)
        {
            pg.ClientScript.RegisterStartupScript(pg.GetType(), "myscript", string.Format("<script>{0}</script>", function)); 
        }

    }
}
