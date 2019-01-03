<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BookReadingCount_Detail.aspx.cs" Inherits="StatisticalCenter.BookCount.BookReadingCount_Detail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../Script/jquery-1.7.2.js"></script>
    <script src="../Script/JS_DatePicker/WdatePicker.js"></script>
    <script src="../Script/layer/layer.js"></script>
    <script src="../Script/jquery.autocomplete.js"></script>
    <link href="../Style/gridTable.css" rel="stylesheet" />
    <link href="../Style/pagination.css" rel="stylesheet" />
    <link href="../Style/TableDisplay.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                    <tr class="trTitle">
                        <td style="width: 50px; text-align: center">页码</td>
                        <td style="width: 100px; text-align: center">在线热点</td>
                        <td style="width: 100px; text-align: center">离线热点</td>
                        <td style="width: 100px; text-align: center">UGC音频</td>
                        <td style="width: 100px; text-align: center">UGC视频</td>
                        <td style="width: 100px; text-align: center">UGC图片</td>
                        <td style="width: 100px; text-align: center">UGC网址</td>
                        <td style="width: 100px; text-align: center">头像视频</td>
                    </tr>

                <asp:Repeater runat="server" ID="repData">
                    <ItemTemplate>
                        <tr ng-repeat="itemx in resultList">
                            <td style="text-align: center"><%# Eval("pageNo") %></td>
                            <td style="text-align: center"><%# Eval("onLineCount") %></td>
                            <td style="text-align: center"><%# Eval("offLineCount") %></td>
                            <td style="text-align: center"><%# Eval("audioCount") %></td>
                            <td style="text-align: center"><%# Eval("videoCount") %></td>
                            <td style="text-align: center"><%# Eval("picCount") %></td>
                            <td style="text-align: center"><%# Eval("webCount") %></td>
                            <td style="text-align: center"><%# Eval("faceCount") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </form>
</body>
</html>
