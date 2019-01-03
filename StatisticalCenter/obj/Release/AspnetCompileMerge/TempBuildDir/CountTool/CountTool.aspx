<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CountTool.aspx.cs" Inherits="StatisticalCenter.CountTool.CountTool" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="http://apps.bdimg.com/libs/angular.js/1.5.0-beta.0/angular.js"></script>
    <script src="../Script/jquery-1.7.2.js"></script>
    <script src="../Script/layer/layer.js"></script>
    <link href="../Style/TableDisplay.css" rel="stylesheet" />
    <link href="../Style/pagination.css" rel="stylesheet" />
    <link href="../Style/gridTable.css" rel="stylesheet" />
    <title></title>
    <style type="text/css">
        .styleoflist {
            line-height: 30px;
            height: 30px;
            width: 120px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            var counthtml = '';
            var num = $("#num").val();
            counthtml += '<div style="width:100%;height:30px;line-height:30px;text-align:left;padding-left:15px;color:white;background-color:#54B9CD">';
            counthtml += '共有' + num + '条消息';
            counthtml += '</div>';
            $("#listcount").html(counthtml);
        })
        function createCode() {
            layer.open({
                type: 2,
                title: '生成二维码',
                shadeClose: true,
                shade: 0.8,
                offset: ['0px', '0px'],
                area: ['100%', '100%'],
                content: 'CountTool_Create.aspx'
            });
            return false;
        }
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="num" runat="server" />
        <div>
            <div>
                <label style="float: left; line-height: 30px;">
                    类型
                </label>
                <div style="float: left; margin-left: 15px;">
                    <asp:DropDownList runat="server" ID="TypeChoose" CssClass="styleoflist">
                        <asp:ListItem Text="全部" Value="-1"></asp:ListItem>
                        <asp:ListItem Text="正常" Value="1"></asp:ListItem>
                        <asp:ListItem Text="未校验" Value="0"></asp:ListItem>
                        <asp:ListItem Text="校验失败" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <label style="float: left; margin-left: 20px; line-height: 30px;">
                    名称
                </label>
                <div style="float: left; margin-left: 15px;">
                    <asp:TextBox runat="server" ID="Name" CssClass="inputstyle"></asp:TextBox>
                </div>
                <div style="float: left; margin-left: 15px">
                    <asp:Button runat="server" ID="searchButton" CssClass="buttonstyle" Text="查询" OnClick="searchButton_Click" />
                </div>
                <div style="float: left; margin-left: 15px">
                    <asp:Button runat="server" ID="refresh" CssClass="buttonstyle" Text="刷新" OnClick="refresh_Click" />
                </div>
                <div style="float: left; margin-left: 80px;">
                    <input type="button" class="buttonstyle" value="生成" onclick="createCode()" />
                </div>
            </div>
            <div style="clear: both"></div>
            <hr />
            <div style="width: 820px; height: 335px">
                <table>
                    <tr class="trTitle">
                        <td style="width: 80px; text-align: center">序号</td>
                        <td style="width: 220px; text-align: center">网址</td>
                        <td style="width: 180px; text-align: center">名称</td>
                        <td style="width: 120px; text-align: center">状态</td>
                        <td style="width: 140px; text-align: center">操作</td>
                    </tr>
                </table>
                <div id="loading" style="text-align: center; padding-top: 80px; width: 100%">
                    <img src="../Content/img/loading.gif" style="width: auto; vertical-align: middle" />
                </div>
                <table id="contenttable" ng-app="mainApp" ng-controller="countController" style="display: none">
                    <tr ng-repeat="item in dataList">
                        <td style="width: 80px; text-align: center">{{$index+1}}</td>
                        <td style="width: 220px; text-align: center">{{item.CTRUrl}}</td>
                        <td style="width: 180px; text-align: center" ng-dblclick="StartUpdate($index+1)">
                            <span id="beforeClick{{$index+1}}">{{item.CTRName}}</span>
                            <div id="afterClick{{$index+1}}" style="display: none">
                                <input id="inputbox{{$index+1}}" type="text" class="inputstyle" style="width: 100%; height: 23px; line-height: 23px; text-align: center" value="{{item.CTRName}}" ng-blur="UpdateName($index+1,item.CTRId)" />
                            </div>
                        </td>
                        <td style="width: 120px; text-align: center" ng-if="item.CTRStatus==1">正常</td>
                        <td style="width: 120px; text-align: center" ng-if="item.CTRStatus==0">未校验</td>
                        <td style="width: 120px; text-align: center" ng-if="item.CTRStatus==2">校验失败</td>
                        <td style="width: 140px; text-align: center">
                            <input type="button" class="buttonstyle" value="查看" ng-click="CheckCode(item.CTRId)" />&nbsp;
                            <input type="button" class="buttonstyle" value="校验" ng-click="JumpCheck(item.CTRId)" />
                        </td>
                    </tr>
                </table>
            </div>
            <div style="clear: both; height: 10px;"></div>
            <div id="listcount" style="width: 805px;">
            </div>
            <div style="margin-top: 35px;">
                <div class="pagination">
                    <webdiyer:AspNetPager runat="server" ID="AspNetPager1" PageSize="10" OnPageChanged="AspNetPager1_PageChanged" CurrentPageButtonClass="current" ShowPageIndexBox="Never"
                        FirstLastButtonsClass="prev0" MoreButtonsClass="next0" LastPageText="末页" FirstPageText="首页" PrevPageText="上页" NextPageText="下页">
                    </webdiyer:AspNetPager>
                </div>
            </div>
            <asp:HiddenField ID="pageIndex" runat="server" />
        </div>
    </form>
    <script>
        var type = $("#TypeChoose").val();
        var name = $("#Name").val();
        var index = 0;
        var time = 1;
        if ($("#pageIndex").val() == "") {
            index = 1;
        }
        else {
            index = $("#pageIndex").val();
        }
        angular.module('mainApp', []).controller('countController', function ($http, $scope) {
            $http.get('ajax.ashx?action=GetResourceData&status=' + type + '&pageIndex=' + index + '&pageSize=10&resourceName=' + name + '').success(function (data) {
                $scope.dataList = data.list;
                timeHide(time);
                $scope.CheckCode = function (code) {
                    layer.open({
                        type: 2,
                        title: '查看二维码',
                        shadeClose: true,
                        shade: 0.8,
                        offset: ['0px', '0px'],
                        area: ['100%', '100%'],
                        content: 'CountTool_Check.aspx?ID=' + code + ''
                    });
                    return false;
                };
                $scope.JumpCheck = function (ID) {
                    $.ajax({
                        type: "get",
                        url: "ajax.ashx?resourceId=" + ID + "&action=ValidResource",
                        success: function (data) {
                            if (data == "1") {
                                alert("校验成功");
                                window.location.reload();
                            }
                            else {
                                alert(data);
                            }
                        }
                    });
                    //layer.open({
                    //    type: 2,
                    //    title: '二维码校验',
                    //    shadeClose: true,
                    //    shade: 0.8,
                    //    offset: ['0px', '0px'],
                    //    area: ['100%', '100%'],
                    //    content: 'JSCode_Check.aspx?ID=' + ID + ''
                    //});
                    return false;
                }
                $scope.StartUpdate = function (i, ID) {
                    $("#beforeClick" + i + "").hide();
                    $("#afterClick" + i + "").show();
                    $("#inputbox" + i + "").focus();
                    $("#inputbox" + i + "").select();
                }
                $scope.UpdateName = function (i, ID) {
                    var newName = $("#inputbox" + i + "").val();
                    $("#beforeClick" + i + "").show();
                    $("#afterClick" + i + "").hide();
                    if (newName != $("#beforeClick" + i + "").html()) {
                        $("#beforeClick" + i + "").html(newName);
                        $.ajax({
                            url: "ajax.ashx?action=UpdateNewName&Name=" + newName + "&ID=" + ID + ""
                        , type: "GET"
                        , success: function (data) {
                            alert("名称修改成功！");
                            $("#afterClick" + i + "").val(newName);
                        }
                        });
                    }
                }

            }).error(function (data, status, headers, config) {
            });
        });
        ///CountTool/ajax.ashx?action=GetResourceData&pageIndex=1&pageSize=10&status=1&resourceName=测试
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
