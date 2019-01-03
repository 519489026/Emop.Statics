<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BookReadingCount.aspx.cs" Inherits="StatisticalCenter.BookCount.BookReadingCount" %>

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
            width: 80px;
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
        function Export() {
            var pressid = $("#hidPressID").val();
            var bookGUID = $("#txtBookGuidLink").val();
            var startTime = $("#textTimeStart").val();
            var endTime = $("#textTimeEnd").val();
            var bookType = $("#bookType").val();
            $("#frmExport").attr("src", "ajax.ashx?action=GetBookReadExportData&pressId=" + pressid + "&type=" + bookType + "&bookGuid=" + bookGUID + "&startDate=" + startTime + "&endDate=" + endTime + "");
        }
        ///图书下拉控件数据
        var vTempPress;
        // 获取下拉控件的数据
        function GetAutoControlData(pressid) {
            $.get("ajax.ashx", { action: "GetBookTable", ParaRandom: Math.random(), PressID: pressid }, function (data) {
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
                width: 260,
                formatItem: function (item) {
                    return item.BookName;
                }
            }).result(function (event, item) {

                $("#" + bookGuidId).val(item.BookGUID);
            });
        }
        function PressChange() {
            var pressid = $("#pressChoose").val();
            $("#hidPressID").val(pressid);
            $("#hidPressIndex").val($('option:selected', '#pressChoose').index());
            $("#hidPressText").val($("#pressChoose").find("option:selected").text().trim());
            GetAutoControlData(pressid);
        }

        function OpenDetail(bookGuid) {
            var startDate = $("#textTimeStart").val();
            var endDate = $("#textTimeEnd").val();
            var url = "ajax.ashx?action=GetBookReadCount_Detail&bookGuid=" + bookGuid + "&startDate=" + startDate + "&endDate=" + endDate;
            $("#frmExport").attr("src", url);
        }

        function ExportData() {
            var pressId = $("#pressChoose").val();//出版社ID
            var bookType = $("#bookType").val();//0图书，1系列
            var bookGuid = $("#txtBookGuidLink").val();//图书GUID
            var timeStart = $("#textTimeStart").val();//开始时间
            var timeEnd = $("#textTimeEnd").val();//结束时间
            var url = "ajax.ashx?action=GetBookReadExportCount&pressId=" + pressid
                + "&bookType=" + bookType + "&bookGuid=" + bookGuid
                + "&timeStart=" + timeStart + "&timeEnd=" + timeEnd;
            $("#frmExport").attr("src", url);
        }

    </script>
</head>
<body style="overflow:hidden">
    <form id="form1" runat="server" autocomplete="off">
        <asp:HiddenField ID="num" runat="server" />
        <asp:HiddenField runat="server" ID="iswrong" />
        <asp:HiddenField runat="server" ID="hidneedrefresh" Value="0" />
        <div ng-app="mainApp" ng-controller="countController" style="width: 840px; overflow: auto">
            <div>
                <label style="float: left; margin-left: 20px; line-height: 26px;">
                    出版社
                </label>
                <div style="float: left; margin-left: 10px;">
                    <select runat="server" id="pressChoose" class="styleoflist" onchange="PressChange()">
                        <option value="{{item.PressID}}" ng-repeat="item in dataList">{{item.PressName}}
                        </option>
                    </select>
                    <asp:HiddenField runat="server" ID="hidPressID" Value="0" />
                    <asp:HiddenField runat="server" ID="hidPressIndex" Value="0" />
                    <asp:HiddenField runat="server" ID="hidPressText" Value="全部" />
                </div>
                <label style="float: left; margin-left: 10px; line-height: 26px;">
                    类型
                </label>
                <div style="float: left; margin-left: 10px;">
                    <asp:DropDownList runat="server" ID="bookType" CssClass="styleoflist" AutoPostBack="true" OnSelectedIndexChanged="bookType_SelectedIndexChanged">
                        <asp:ListItem Value="0" Text="图书" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="系列"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:HiddenField ID="dataType" runat="server" Value="0" />
                </div>
                <label style="float: left; margin-left: 15px; line-height: 26px;">
                    名称
                </label>
                <div style="float: left; margin-left: 10px;">
                    <asp:TextBox runat="server" ID="txtBooklink" CssClass="inputstylesmall" Text="全部"></asp:TextBox>
                    <asp:HiddenField runat="server" ID="txtBookGuidLink" />
                </div>
                <label style="float: left; margin-left: 20px; line-height: 26px;">
                    时间
                </label>
                <div style="float: left; margin-left: 10px;">
                    <asp:TextBox runat="server" CssClass="inputstyle" ID="textTimeStart" Text="" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                    &nbsp;&nbsp;
                    <asp:TextBox runat="server" CssClass="inputstyle" ID="textTimeEnd" Text="" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                </div>
                <div style="clear: both; height: 10px;"></div>
                <div style="float: right; margin-right: 24px;">
                    <asp:Button runat="server" ID="btnSearch" Text="查询" CssClass="buttonstylesmall" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <asp:Button runat="server" ID="btnRefresh" Text="刷新" CssClass="buttonstylesmall" OnClick="btnRefresh_Click" />&nbsp;&nbsp;
                    <input id="btnExport" type="button" value="导出" class="buttonstylesmall" onclick="ExportData()" />
                </div>
            </div>
            <div style="clear: both"></div>
            <hr style="margin-top: 20px;" />
            <table style="margin-top: 15px; background-color: white; display:none">
                <tr>
                    <td style="width: 200px; text-align: center">时间段</td>
                    <td style="width: 150px; text-align: center">出版社</td>
                    <td style="width: 70px; text-align: center">图书/系列</td>
                    <td style="width: 70px; text-align: center">阅读次数</td>
                    <td style="width: 70px; text-align: center">阅读人数</td>
                    <td style="width: 120px; text-align: center">人均单次阅读时长</td>
                    <td style="width: 100px; text-align: center">
                        <input type="button" class="buttonstyle" value="导出" onclick="Export()" />
                    </td>
                </tr>
                <tr>
                    <td id="timerange" style="width: 200px; text-align: center"></td>
                    <td id="pressname" style="width: 150px; text-align: center"></td>
                    <td id="bookname" style="width: 70px; text-align: center"></td>
                    <td style="width: 70px; text-align: center" id="readingTimes"></td>
                    <td style="width: 70px; text-align: center" id="readingCount"></td>
                    <td style="width: 120px; text-align: center" id="prereadingTime"></td>
                    <td style="width: 100px; text-align: center"></td>
                </tr>
            </table>
            <div id="divForBook" style="height: 460px; width: 820px; overflow-x:hidden">
                <table id="titletable" style="margin-top: 20px; width: 2380px;">
                    <tr class="trTitle">
                        <td style="width: 50px; text-align: center">序号</td>
                        <td style="width: 300px; text-align: center">图书名称</td>
                        <td style="width: 300px; text-align: center">出版社</td>
                        <td style="width: 90px; text-align: center">阅读人数</td>
                        <td style="width: 90px; text-align: center">阅读次数</td>
                        <td style="width: 200px; text-align: center">人均单次阅读时长</td>
                        <td style="width: 150px; text-align: center">在线热点</td>
                        <td style="width: 150px; text-align: center">离线热点</td>
                        <td style="width: 150px; text-align: center">小梦辅导</td>
                        <td style="width: 150px; text-align: center">UGC音频</td>
                        <td style="width: 150px; text-align: center">UGC视频</td>
                        <td style="width: 150px; text-align: center">UGC图片</td>
                        <td style="width: 150px; text-align: center">UGC网址</td>
                        <td style="width: 150px; text-align: center">UGC头像</td>
                        <td style="width: 150px; text-align: center">操作</td>
                    </tr>
                </table>
                <div id="loadingforbook" style="text-align: center; padding-top: 80px; width: 100%">
                    <img src="../Content/img/loading.gif" style="width: auto; vertical-align: middle" />
                </div>
                <table id="contenttable" style="display: none; width: 2380px;">
                    <tr ng-repeat="itemx in resultList">
                        <td style="width: 50px; text-align: center">{{ $index + 1 }}</td>
                        <td style="width: 300px; text-align: center; overflow: hidden; white-space: nowrap; text-overflow: ellipsis" title="{{itemx.BookName}}">{{itemx.BookName}}
                        </td>
                        <td style="width: 300px; text-align: center">{{itemx.PressName}}</td>
                        <td style="width: 90px; text-align: center">{{itemx.ReadPesonCount}}</td>
                        <td style="width: 90px; text-align: center">{{itemx.ReadCount}}</td>
                        <td style="width: 200px; text-align: center">{{itemx.ReadTimeLength}}秒</td>
                        <td style="width: 150px; text-align: center">{{itemx.OnLineCount}}</td>
                        <td style="width: 150px; text-align: center">{{itemx.OffLineCount}}</td>
                        <td style="width: 150px; text-align: center">{{itemx.XmkhCount}}</td>
                        <td style="width: 150px; text-align: center">{{itemx.UGCAudioCount}}</td>
                        <td style="width: 150px; text-align: center">{{itemx.UGCVideoCount}}</td>
                        <td style="width: 150px; text-align: center">{{itemx.UGCImageCount}}</td>
                        <td style="width: 150px; text-align: center">{{itemx.UGCWebCount}}</td>
                        <td style="width: 150px; text-align: center">{{itemx.UGCFaceRecordCount}}</td>
                        <td style="width: 150px; text-align: center">
                            <input class="buttonstylesmall" style="padding: 0" value="阅读明细" type="button" ng-click="Exportdetail(itemx.BookGUID)" />
                            <input class="buttonstylesmall" style="padding: 0" value="热点明细" type="button" ng-click="OpenDetail(itemx.BookGUID)" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="divForSeries" style="height: 460px; width: 820px;overflow:hidden;display:none">
                <table id="titleofSeries" style="margin-top: 20px; width: 820px;">
                    <tr class="trTitle">
                        <td style="width: 50px; text-align: center">序号</td>
                        <td style="width: 220px; text-align: center">系列</td>
                        <td style="width: 220px; text-align: center">出版社</td>
                        <td style="width: 100px; text-align: center">阅读人数</td>
                        <td style="width: 100px; text-align: center">阅读次数</td>
                        <td style="width: 130px; text-align: center">人均单次阅读时长</td>
                    </tr>
                </table>
                <div id="loadingforseries" style="text-align: center; padding-top: 80px; width: 100%">
                    <img src="../Content/img/loading.gif" style="width: auto; vertical-align: middle" />
                </div>

                <table id="tableofSeries" style=" width: 820px; line-height: 30px;display:none">
                    <tr id="dataofSeries" ng-repeat="itemx in resultList">
                        <td style="width: 50px; text-align: center">{{ $index + 1 }}</td>
                        <td style="width: 220px; text-align: center; overflow: hidden; white-space: nowrap; text-overflow: ellipsis" title="{{itemx.SeriesName}}">
                            <div style="width: 220px; text-align: center; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                {{itemx.SeriesName}}
                            </div>
                        </td>
                        <td style="width: 220px; text-align: center">{{itemx.PressName}}</td>
                        <td style="width: 100px; text-align: center">{{itemx.ReadPesonCount}}</td>
                        <td style="width: 100px; text-align: center">{{itemx.ReadCount}}</td>
                        <td style="width: 130px; text-align: center">{{itemx.ReadTimeLength}}秒</td>
                    </tr>
                </table>
            </div>
            <div style="clear: both; height: 10px;"></div>
            <div id="listcount" style="width: 820px;">
            </div>
            <div style="margin-top: 55px;">
                <div class="pagination">
                    <webdiyer:AspNetPager runat="server" ID="AspNetPager1" PageSize="10" OnPageChanged="AspNetPager1_PageChanged" CurrentPageButtonClass="current" ShowPageIndexBox="Never"
                        FirstLastButtonsClass="prev0" PrevNextButtonsClass="next0" MoreButtonsClass="next0" LastPageText="末页" FirstPageText="首页" PrevPageText="上页" NextPageText="下页">
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
        var index = 0;
        if ($("#pageIndex").val() == "") {
            index = 1;
        }
        else {
            index = $("#pageIndex").val();
        }
        var start = $("#textTimeStart").val();
        var end = $("#textTimeEnd").val();
        var pressid = $("#hidPressID").val();
        var bookGuid = $("#txtBookGuidLink").val();
        var pressName = $("#hidPressText").val();
        //var RangeHtml = "";
        //RangeHtml += '' + start + '到' + end + '';
        //var bookName = $("#txtBooklink").val();
        var bookType = $("#bookType").val();
        //$("#timerange").html(RangeHtml);
        //$("#bookname").html(bookName);
        angular.module('mainApp', []).controller('countController', function ($http, $scope) {
            $http.get('ajax.ashx?action=GetUserPressTable').success(function (data) {
                $http.get('ajax.ashx?action=GetBookReadCountData&pressId=' + pressid + '&type=' + bookType + '&bookGuid=' + bookGuid + '&startDate=' + start + '&endDate=' + end + '&pageIndex=' + index + '&pageSize=10').success(function (result) {
                    $scope.resultList = result.list;
                    $scope.dataList = data;
                    //$("#readingTimes").html(result.totalReadCount);
                    //$("#readingCount").html(result.totalPersonCount);
                    //$("#prereadingTime").html(result.avgTimeLength + '秒');
                    //$("#pressname").html(pressName);
                    timeHide(time);
                    $scope.Exportdetail = function (guid) {
                        $("#frmExport").attr("src", "ajax.ashx?action=GetBookReadExportDetailData&bookGuid=" + guid + "&startDate=" + start + "&endDate=" + end + "");
                    };
                    $scope.OpenDetail = function (guid) {
                        OpenDetail(guid)
                    };
                }).error(function (result, status, headers, config) {
                });
            }).error(function (data, status, headers, config) {
            });
        });
        function timeHide(time) {
            var pressindex = $("#hidPressIndex").val();
            $("#pressChoose").get(0).selectedIndex = pressindex;
            var datatype = $("#dataType").val();
            if (time == 0) {
                $("#loadingforbook").hide();
                $("#loadingforseries").hide();
                if (datatype == "0") {
                    $("#divForBook").show();
                    $("#contenttable").show();
                    $("#divForSeries").hide();
                    $("#tableofSeries").hide();
                }
                else if (datatype == "1") {
                    $("#divForBook").hide();
                    $("#contenttable").hide();
                    $("#divForSeries").show();
                    $("#tableofSeries").show();
                }
                $("#divForBook").css("overflow", "auto");
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
