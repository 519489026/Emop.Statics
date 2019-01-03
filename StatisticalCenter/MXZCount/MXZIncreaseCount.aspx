<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MXZIncreaseCount.aspx.cs" Inherits="StatisticalCenter.MXZCount.MXZIncreaseCount" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="http://apps.bdimg.com/libs/angular.js/1.5.0-beta.0/angular.js"></script>
    <script src="../Script/jquery-1.7.2.js"></script>
    <script src="../Script/JS_DatePicker/WdatePicker.js"></script>
    <script src="../Script/layer/layer.js"></script>
    <link href="../Style/gridTable.css" rel="stylesheet" />
    <link href="../Style/pagination.css" rel="stylesheet" />
    <link href="../Style/TableDisplay.css" rel="stylesheet" />
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
            //$.ajax({
            //    url: "ajax.ashx?action=GetMXZCountTable&type=" + type + "&startDate=" + start + "&endDate=" + end + "&pageIndex=1&pageSize=10",
            //    type: "GET",
            //    success: function (result) {
            //        var data = result;
            //        var repliesData = JSON.parse(data);
            //        var mainApp = angular.module('mainApp', []);
            //        mainApp.controller('countController', function ($scope) {
            //            $scope.replies = null;
            //            $scope.replies = repliesData;
            //        });
            //    }
            //})
        })
        //angular.module('todoApp', []).controller('RealDataController', function ($http, $scope) {
        //    var self = this;
        //    $http.get('/SOA/GetMenus').success(function (data) {
        //        self.dataList = data;
        //    }).error(function (data, status, headers, config) { });
    </script>
    <style type="text/css">
        .styleoflist {
            line-height: 30px;
            height: 30px;
            width: 120px;
        }

        .inputstyle {
            width: 180px;
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
        function Export(type) {
            var choosetype = $("#CountType").val();
            var startTime = $("#textTimeStart").val();
            var endTime = $("#textTimeEnd").val();
            if (type == 'total') {
                $("#frmExport").attr("src", "ajax.ashx?action=ExportMXZCountTable&type=" + type + "&startDate=" + startTime + "&endDate=" + endTime);
            }
            else if (type == 'detail') {
                $("#frmExport").attr("src", "ajax.ashx?action=ExportMXZDetail&type=" + type + "&startDate=" + startTime + "&endDate=" + endTime);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField runat="server" ID="iswrong" />
        <div style="width: 820px;">
            <div>
                <label style="float: left; margin-left: 20px; line-height: 26px;">
                    类型
                </label>
                <div style="float: left; margin-left: 10px;">
                    <asp:DropDownList runat="server" CssClass="styleoflist" ID="CountType">
                        <asp:ListItem Text="全部" Value="-1" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="充值" Value="2"></asp:ListItem>
                        <asp:ListItem Text="赠送" Value="4"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <label style="float: left; margin-left: 20px; line-height: 26px;">
                    时间
                </label>
                <div style="float: left; margin-left: 10px;">
                    <asp:TextBox runat="server" CssClass="inputstyle" ID="textTimeStart" Text="" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                    &nbsp;&nbsp;
                    <asp:TextBox runat="server" CssClass="inputstyle" ID="textTimeEnd" Text="" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                </div>
                <div style="float: left; margin-left: 10px;">
                    <asp:Button runat="server" ID="btnSearch" Text="查询" CssClass="buttonstylesmall" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <asp:Button runat="server" ID="btnRefresh" Text="刷新" CssClass="buttonstylesmall" OnClick="btnRefresh_Click" />
                </div>
            </div>
            <div style="clear: both"></div>
            <hr style="margin-top: 20px;" />
            <table style="margin-top: 15px; background-color: white">
                <tr>
                    <td style="width: 200px; text-align: center">时间段</td>
                    <td style="width: 200px; text-align: center">梦想钻</td>
                    <td style="width: 150px; text-align: center">人数</td>
                    <td style="width: 100px; text-align: center">
                        <input type="button" class="buttonstyle" value="导出" onclick="Export('total')" />
                    </td>
                </tr>
                <tr>
                    <td id="timerange" style="width: 200px; text-align: center"></td>
                    <td style="width: 200px; text-align: center" id="coinNumber"></td>
                    <td style="width: 150px; text-align: center" id="personNumber"></td>
                    <td style="width: 100px; text-align: center">
                        <input type="button" class="buttonstyle" value="导出明细" onclick="Export('detail')" />
                    </td>
                </tr>
            </table>
            <div style="height: 210px;">
                <table id="titletable" style="margin-top: 20px;">
                    <tr class="trTitle">
                        <td style="width: 100px; text-align: center">序号</td>
                        <td style="width: 160px; text-align: center">时间</td>
                        <td style="width: 300px; text-align: center">梦想钻</td>
                        <td style="width: 160px; text-align: center">人数</td>
                    </tr>
                </table>
                <div id="loading" style="text-align: center; padding-top: 80px; width: 100%">
                    <img src="../Content/img/loading.gif" style="width: auto; vertical-align: middle" />
                </div>
                <table id="contenttable" ng-app="mainApp" ng-controller="countController" style="display: none">
                    <tr ng-repeat="item in dataList">
                        <td style="width: 100px; text-align: center">{{ $index + 1 }}</td>
                        <td style="width: 160px; text-align: center">{{item.F_Create_Time}}</td>
                        <td style="width: 300px; text-align: center">{{item.MXZCount}}</td>
                        <td style="width: 160px; text-align: center">{{item.PersonCount}}</td>
                    </tr>
                </table>
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
        <div style="display:none">
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
        var RangeHtml = "";
        RangeHtml += '' + start + '到' + end + '';
        $("#timerange").html(RangeHtml);
        angular.module('mainApp', []).controller('countController', function ($http, $scope) {
            $http.get('ajax.ashx?action=GetMXZCountTable&type=' + type + '&startDate=' + start + '&endDate=' + end + '&pageIndex=' + index + '&pageSize=10').success(function (data) {
                $scope.dataList = data.list;
                $("#personNumber").html(data.totalPersonCount);
                $("#coinNumber").html(data.totalCoinCount);
                timeHide(time);
            }).error(function (data, status, headers, config) {
            });
        });
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
    </script>
</body>
</html>
