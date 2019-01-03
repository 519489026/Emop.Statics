<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataUpdate.aspx.cs" Inherits="StatisticalCenter.DataUpdate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        时间段：<asp:TextBox ID="txtStartTime" runat="server" ></asp:TextBox>
        -<asp:TextBox ID="txtEndTime" runat="server" ></asp:TextBox>
        <asp:DropDownList runat="server" ID="sltType" >
            <asp:ListItem Value="1" Text="梦想钻日统计"></asp:ListItem>
            <asp:ListItem Value="2" Text="图书下载日统计"></asp:ListItem>
            <asp:ListItem Value="3" Text="图书阅读日统计"></asp:ListItem>
            <asp:ListItem Value="4" Text="二维码日统计"></asp:ListItem>
            <asp:ListItem Value="5" Text="图书阅读时长日统计"></asp:ListItem>
            <asp:ListItem Value="6" Text="二维码转化率统计"></asp:ListItem>
            <asp:ListItem Value="7" Text="图书BANNER点击统计"></asp:ListItem>
        </asp:DropDownList>
        <asp:CheckBox Text="清空系统原有的数据" runat="server" ID="chkClearOldData" />
        <asp:Button runat="server" ID="btnCount" OnClick="btnCount_Click" Text="确认更新" />
    </div>
        <div>
            <a href="javascript:void(0)" onclick="window.open('/BookCount/BookBuyingCount.aspx')">图书购买统计</a><br />
            <a href="javascript:void(0)"  onclick="window.open('/BookCount/BookContentCount.aspx')">图片内容统计</a><br />
            <a href="javascript:void(0)"  onclick="window.open('/BookCount/BookDownloadCount.aspx')">图书下载统计</a><br />
            <a href="javascript:void(0)"  onclick="window.open('/BookCount/BookReadingCount.aspx')">图书阅读统计</a><br />
            <a href="javascript:void(0)"  onclick="window.open('/CountTool/CountTool.aspx')">二维码统计工具</a><br />
            <a href="javascript:void(0)"  onclick="window.open('/CountTool/CodeDownloadCount.aspx')">二维码下载统计</a><br />
            <a href="javascript:void(0)"  onclick="window.open('/CountTool/CodeAreaCount.aspx')">二维码区域统计</a><br />
            <a href="javascript:void(0)"  onclick="window.open('/MXZCount/MXZIncreaseCount.aspx')">梦想钻增长统计</a><br />
            <a></a>

        </div>
    </form>
</body>
</html>
