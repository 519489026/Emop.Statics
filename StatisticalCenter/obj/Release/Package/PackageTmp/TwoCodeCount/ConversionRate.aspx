<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConversionRate.aspx.cs" Inherits="StatisticalCenter.CountTool.ConversionRate" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="http://apps.bdimg.com/libs/angular.js/1.5.0-beta.0/angular.js"></script>
    <script src="../Script/jquery-1.7.2.js"></script>
    <script src="../Script/JS_DatePicker/WdatePicker.js"></script>
    <script src="../Script/layer/layer.js"></script>
    <script src="../Script/jquery.autocomplete.js"></script>
    <link href="../Style/gridTable.css" rel="stylesheet" />
    <link href="../Style/pagination.css" rel="stylesheet" />
    <link href="../Style/TableDisplay.css" rel="stylesheet" />
    <link href="../Style/jquery.autocomplete.css" rel="stylesheet" />
    <title></title>
    <script type="text/javascript">
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
        $(function () {
            var iswrong = $("#iswrong").val();
            if (iswrong == '1') {
                alert("开始时间必须小于结束时间！");
            }
        })
    </script>
    <style type="text/css">
        .styleoflist {
            line-height: 30px;
            height: 30px;
            width: 120px;
        }

        .inputstyle {
            width: 100px;
            border: 1px solid #e0d6d6;
            background-color: #F2F2F2;
            height: 26px;
            line-height: 26px;
        }

        .inputstylesmall {
            width: 150px;
            border: 1px solid #e0d6d6;
            background-color: #F2F2F2;
            height: 26px;
            line-height: 26px;
        }

        .buttonstylesmall {
            background-color: #54b9cd;
            color: white;
            cursor: pointer;
            border: none;
            height: 30px;
            line-height: 30px;
            width: 70px;
            text-align: center;
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
    <script type="text/javascript">
        $(function () {
            GetAutoControlData();
            $('option:selected', '#pressChoose').index(0);
            var counthtml = '';
            var num = $("#num").val();
            counthtml += '<div style="width:100%;height:30px;line-height:30px;text-align:left;padding-left:15px;color:white;background-color:#54B9CD">';
            counthtml += '共有' + num + '条记录';
            counthtml += '</div>';
            $("#listcount").html(counthtml);
        })
        function Export(type) {
            var pressid = $("#hidPressID").val();
            var bookGUID = $("#txtBookGuidLink").val();
            var startTime = $("#textTimeStart").val();
            var endTime = $("#textTimeEnd").val();
            if (type == 'total') {
                $("#frmExport").attr("src", "ajax.ashx?action=GetBuyBookExportData&pressId=" + pressid + "&bookGuid=" + bookGUID + "&startDate=" + startTime + "&endDate=" + endTime + "");
            }
            else if (type == 'detail') {
                $("#frmExport").attr("src", "ajax.ashx?action=GetBuyBookExportDetailData&pressId=" + pressid + "&bookGuid=" + bookGUID + "&startDate=" + startTime + "&endDate=" + endTime + "");
            }
        }
        ///图书下拉控件数据
        var vTempPress;
        // 获取下拉控件的数据
        function GetAutoControlData() {
            $.get("ajax.ashx", { action: "GetBookList", ParaRandom: Math.random() }, function (data) {
                if (data != "[]") {
                    vTempPress = eval('(' + data + ')');
                    SetAutoControl("txtBooklink");
                }
            });
        }
        //设置控件的下拉
        //bookNameId:图书名控件ID
        //bookGuidId:图书GUID控件ID
        function SetAutoControl(bookNameId) {
            $("#" + bookNameId).autocomplete(vTempPress, {
                minChars: 0,
                max: 100,
                autoFill: false,
                mustMatch: false,
                matchContains: true,
                scrollHeight: 220,
                width: 260,
                formatItem: function (item) {
                    return item.name;
                }
            }).result(function (event, item) {

            });
        }
        function PressChange() {
            var pressid = $("#pressChoose").val();
            $("#hidPressID").val(pressid);
            $("#hidPressIndex").val($('option:selected', '#pressChoose').index());
            $("#hidPressText").val($("#pressChoose").find("option:selected").text().trim());
            GetAutoControlData(pressid);
        }
    </script>
</head>
<body style="overflow:hidden">
    <form id="form1" runat="server" autocomplete="off">
        <asp:HiddenField ID="num" runat="server" />
        <asp:HiddenField runat="server" ID="iswrong" />
        <asp:HiddenField runat="server" ID="hidneedrefresh" Value="0" />
        <div ng-app="mainApp" ng-controller="countController" style="width: 850px;overflow:auto">
            <div>
                <label style="float: left; margin-left: 15px; line-height: 26px;">
                    图书名称
                </label>
                <div style="float: left; margin-left: 10px;overflow-x:hidden">
                    <asp:TextBox runat="server" ID="txtBooklink" CssClass="inputstylesmall" Text="全部"></asp:TextBox>
                </div>
                <label style="float: left; margin-left: 20px; line-height: 26px;">
                    时间
                </label>
                <div style="float: left; margin-left: 10px;">
                    <asp:TextBox runat="server" CssClass="inputstyle" ID="textTimeStart" Text="" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                    &nbsp;&nbsp;
                    <asp:TextBox runat="server" CssClass="inputstyle" ID="textTimeEnd" Text="" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                </div>
                <div style="float: right; margin-right: 24px;">
                    <asp:Button runat="server" ID="btnSearch" Text="查询" CssClass="buttonstylesmall" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <asp:Button runat="server" ID="btnRefresh" Text="刷新" CssClass="buttonstylesmall" OnClick="btnRefresh_Click" />
                </div>
            </div>
            <div style="clear: both"></div>
            <hr style="margin-top: 20px;" />
                            <div>*据电子书下载量排序，显示至昨天的数据</div>
            <div style="height: 390px">

                <table id="titletable" style="margin-top: 20px;">
                    <tr class="trTitle">
                        <td style="width: 60px; text-align: center">序号</td>
                        <td style="width: 140px; text-align: center">图书名称</td>
                        <td style="width: 140px; text-align: center">出版社</td>
                        <td style="width: 120px; text-align: center">下载扫描数</td>
                        <td style="width: 140px; text-align: center">电子书扫描下载数</td>
                        <td style="width: 80px; text-align: center">操作</td>
                    </tr>
                </table>
                <div id="loading" style="text-align: center; padding-top: 80px; width: 100%">
                    <img src="../Content/img/loading.gif" style="width: auto; vertical-align: middle" />
                </div>
                <table id="contenttable" style="display: none; line-height: 30px;">
                    <tr ng-repeat="itemx in resultList">
                        <td style="width: 60px; text-align: center">{{ $index + 1 }}</td>
                        <td style="width: 140px; text-align: center" title="{{itemx.F_Customization_Name}}">
                            <div style="width: 140px; text-align: center; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                {{itemx.F_Customization_Name}}
                            </div>
                        </td>
                        <td style="width: 140px; text-align: center" title="{{itemx.PublishName}}">
                            <div style="width: 140px; text-align: center; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                {{itemx.PublishName}}
                            </div>
                        </td>
                        <td style="width: 120px; text-align: center">{{itemx.APKDownloadCount}}</td>
                        <td style="width: 140px; text-align: center">{{itemx.BookDownLoadCount}}</td>
                        <td style="width: 80px; text-align: center">
                            <input type="button" class="buttonstyle" value="导出明细" ng-click="Export(itemx.F_Customization_ID)" />
                        </td>
                    </tr>
                </table>
            </div>
            <div style="clear: both; height: 10px;"></div>
            <div id="listcount" style="width: 835px;">
            </div>
            <div style="margin-top: 55px;">
                <div class="pagination">
                    <webdiyer:AspNetPager runat="server" ID="AspNetPager1" PageSize="10" OnPageChanged="AspNetPager1_PageChanged" CurrentPageButtonClass="current" ShowPageIndexBox="Never"
                        FirstLastButtonsClass="prev0" MoreButtonsClass="next0" PrevNextButtonsClass="next0" LastPageText="末页" FirstPageText="首页" PrevPageText="上页" NextPageText="下页">
                    </webdiyer:AspNetPager>
                </div>
            </div>
            <asp:HiddenField ID="pageIndex" runat="server" />
        </div>
        <div style="display: none">
            <iframe id="frmExport"></iframe>
        </div>
    </form>
    <script>
        $("#contenttable").hide();
        var width = $("#titletable").width();
        var height = $("#titletable").height();
        $("#loading").width(width);
        $("#loading").height(height);
        $("#loading").show();
        var time = 1;
        var monthago = GetStartTime();
        var now = GetEndTime();
        var type = $("#CountType").val();
        var index = 0;
        if ($("#pageIndex").val() == "") {
            index = 1;
        }
        else {
            index = $("#pageIndex").val();
        }
        var start = $("#textTimeStart").val();
        var end = $("#textTimeEnd").val();
        var bookName = $("#txtBooklink").val();
        if (bookName == '全部') {
            bookName = '';
        }
        angular.module('mainApp', []).controller('countController', function ($http, $scope) {
            $http.get('ajax.ashx?action=GetCountData&bookName=' + bookName + '&dateStart=' + start + '&dateEnd=' + end + '&pageIndex=' + index + '&pageSize=10').success(function (result) {
                $scope.resultList = result;
                $scope.Export = function (id) {
                    $("#frmExport").attr("src", "ajax.ashx?action=GetExportData&cusId=" + id + "&bookName=" + bookName + "&dateStart=" + start + "&dateEnd=" + end + "");
                };
                    timeHide(time);
                }).error(function (result, status, headers, config) {
                });
        });
        function timeHide(time) {
            var pressindex = $("#hidPressIndex").val();
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
    </script>
</body>
</html>
