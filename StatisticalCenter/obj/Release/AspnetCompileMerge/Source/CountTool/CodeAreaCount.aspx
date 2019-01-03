<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CodeAreaCount.aspx.cs" Inherits="StatisticalCenter.CountTool.CodeAreaCount" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="../Script/jquery-1.7.2.js"></script>
    <script src="../Script/JS_DatePicker/WdatePicker.js"></script>
    <script src="../Script/layer/layer.js"></script>
    <script src="../Script/jquery.autocomplete.js"></script>
    <link href="../Style/jquery.autocomplete.css" rel="stylesheet" />
    <link href="../Style/gridTable.css" rel="stylesheet" />
    <link href="../Style/pagination.css" rel="stylesheet" />
    <link href="../Style/TableDisplay.css" rel="stylesheet" />
    <title></title>
    <script type="text/javascript">
        $(function () {
            GetAutoControlData();
            var iswrong = $("#iswrong").val();
            if (iswrong == '1') {
                alert("开始时间必须小于结束时间！");
            }
        })
        function GetStartTime() {
            var date = new Date();
            var str = "";
            if (date.getMonth == 0) {
                str = (date.getFullYear() - 1) + '/';
                str += '12/';
                str += date.getDate();
            }
            else {
                str = date.getFullYear() + '/';
                str += date.getMonth() + '/';
                str += date.getDate();
            }
            return str;
        }
        function GetEndTime() {
            var date = new Date();
            var str = "";
            str = date.getFullYear() + '/';
            str += (date.getMonth() + 1) + '/';
            str += date.getDate();
            return str;
        }
        ///图书下拉控件数据
        var vTempPress;
        // 获取下拉控件的数据
        function GetAutoControlData() {
            $.get("ajax.ashx", { action: "GetResourceValuePair", ParaRandom: Math.random() }, function (data) {
                if (data != "[]") {
                    vTempPress = eval('(' + data + ')');
                    SetAutoControl("txtBooklink", "txtBookGuidLink");
                }
            });
        }
        //设置控件的下拉
        //bookNameId:图书名控件ID
        //bookGuidId:图书GUID控件ID
        function SetAutoControl(bookNameId, bookGuidId) {
            $("#" + bookNameId).autocomplete(vTempPress, {
                minChars: 0,
                max: 100,
                autoFill: false,
                mustMatch: false,
                matchContains: true,
                scrollHeight: 220,
                formatItem: function (item) {
                    return item.CTRName;
                }
            }).result(function (event, item) {

                $("#" + bookGuidId).val(item.CTRId);
            });
        }
        function Export() {
            $("#frmExport").attr("src", "ajax.ashx?action=GetExportAreaData&resourceId=" + $("#txtBookGuidLink").val() + "&startDate=" + $("#textTimeStart").val() + "&endDate=" + $("#textTimeEnd").val());
        }
    </script>
    <style type="text/css">
        .styleoflist {
            line-height: 30px;
            height: 30px;
            width: 120px;
        }

        .inputstyle {
            width: 140px;
            border: 1px solid #e0d6d6;
            background-color: #F2F2F2;
            height: 26px;
            line-height: 26px;
        }

        .buttonstyle {
            background-color: #54b9cd;
            color: white;
            cursor: pointer;
            border: none;
            height: 30px;
            line-height: 30px;
            width: 100px;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField runat="server" ID="iswrong" />
        <div style="width: 850px">
            <div>
                <label style="float: left; margin-left: 20px; line-height: 26px;">
                    时间
                </label>
                <div style="float: left; margin-left: 10px;">
                    <asp:TextBox runat="server" CssClass="inputstyle" ID="textTimeStart" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                    &nbsp;&nbsp;
                    <asp:TextBox runat="server" CssClass="inputstyle" ID="textTimeEnd" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                </div>
                <label style="float: left; margin-left: 40px; line-height: 26px;">
                    二维码名称
                </label>
                <div style="float: left; margin-left: 10px;">
                    <asp:TextBox runat="server" ID="txtBooklink" CssClass="inputstyle" ></asp:TextBox>
                    <asp:HiddenField runat="server" ID="txtBookGuidLink" />
                </div>
                <div style="float: left; margin-left: 10px;">
                    <asp:Button runat="server" CssClass="buttonstyle" ID="btnSearch" OnClick="btnSearch_Click" Text="查询" />
                    &nbsp;&nbsp;
                    <input type="button" id="btnExport" value="导出" class="buttonstyle" onclick="Export()" />
                </div>
            </div>
            <div style="clear: both"></div>
            <hr style="margin-top: 20px;" />
            <div style="height: 210px;">
                <table style="margin-top: 15px; background-color: white" id="titletable">
                    <tr>
                        <td style="width: 80px; text-align: center">序号</td>
                        <td style="width: 140px; text-align: center">省份</td>
                        <td style="width: 180px; text-align: center">扫码次数</td>
                        <td style="width: 180px; text-align: center">扫码人数</td>
                        <td style="width: 150px; text-align: center">下载次数
                        </td>
                    </tr>
                </table>
                <%--<div id="loading" style="text-align: center; padding-top: 80px; width: 100%">
                    <img src="../Content/img/loading.gif" style="width: auto; vertical-align: middle" />
                </div>--%>
                <table id="contenttable">
                    <asp:Repeater runat="server" ID="repData">
                        <ItemTemplate>
                            <tr>
                                <td style="width: 80px; text-align: center">
                                    <%# Container.ItemIndex+1 %>
                                </td>
                                <td style="width: 140px; text-align: center">
                                    <%#Eval("CTHProvince") %>
                                </td>
                                <td style="width: 180px; text-align: center">
                                    <%#Eval("CTHBrowseCount") %>
                                </td>
                                <td style="width: 180px; text-align: center">
                                    <%#Eval("CTHPersonCount") %>
                                </td>
                                <td style="width: 150px; text-align: center">
                                    <%#Eval("CTHDownLoadCount") %>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>

        <div style="display:none"><iframe id="frmExport"></iframe></div>
    </form>
    <%--<script>
        ///CountTool/ajax.ashx?action=GetWayCountData&resourceId=1&startDate=2015-01-01&endDate=2016-01-01'
        BindData();
        angular.module('secondApp', []).controller('nameController', function ($http, $scope) {
            $http.get('ajax.ashx?action=GetResourceValuePair').success(function (result) {
                $scope.seconddataList = result.list;
            }).error(function (data, status, headers, config) {
            });
        })
        function timeHide(time) {
            if (time == 0) {
                $("#loading").hide();
                $("#contenttable").show();
                return;
            }
            else if (time != 0) {
                time--;
            }
            if (time != 1 && time != -1) {
                setTimeout(function () {
                    timeHide(time);
                }, 1000);
            }
        };
        function BindData() {
            var width = $("#titletable").width();
            var height = $("#titletable").height();
            $("#loading").width(width);
            $("#loading").height(height);
            $("#loading").show();
            var time = 1;
            var monthago = GetStartTime();
            var now = GetEndTime();
            var id = $("#CodeID").val();
            if (id == "") {
                id = 0;
            }
            var index = 0;
            if ($("#textTimeStart").val() == "") {
                $("#textTimeStart").val(monthago);
            }
            if ($("#textTimeEnd").val() == "") {
                $("#textTimeEnd").val(now);
            }
            var start = $("#textTimeStart").val();
            var end = $("#textTimeEnd").val();
            angular.module('mainApp', []).controller('countController', function ($http, $scope) {
                $http.get('ajax.ashx?action=GetAreaCoutTable&resourceId=' + id + '&startDate=' + start + '&endDate=' + end + '').success(function (data) {
                    $scope.dataList = data.list;
                    timeHide(time);
                }).error(function (data, status, headers, config) {

                });
            });
        }
        function SearchClick() {
            BindData();
        }
    </script>--%>
</body>
</html>
