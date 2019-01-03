<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CountTool_Check.aspx.cs" Inherits="StatisticalCenter.CountTool.CountTool_Create" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="../Script/jquery-1.7.2.js"></script>
    <script src="http://apps.bdimg.com/libs/angular.js/1.5.0-beta.0/angular.js"></script>
    <link href="../Style/gridTable.css" rel="stylesheet" />
    <link href="../Style/pagination.css" rel="stylesheet" />
    <link href="../Style/TableDisplay.css" rel="stylesheet" />
    <title></title>
    <style type="text/css">
        .inputstyle {
            width: 270px;
            border: 1px solid #e0d6d6;
            background-color: #F2F2F2;
            height: 26px;
            line-height: 26px;
        }

        .inputstylesmall {
            width: 185px;
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
            width: 80px;
            text-align: center;
        }

        .inputstylehigh {
            width: 270px;
            border: 1px solid #e0d6d6;
            background-color: #F2F2F2;
            height: 120px;
            line-height: 26px;
        }
    </style>
    <script type="text/javascript">
        function GetQueryString(sProp) {
            var re = new RegExp("[&,?]" + sProp + "=([^\\&]*)", "i");
            var a = re.exec(document.location.search);
            if (a == null)
                return "";
            return a[1];
        };
        function CopyCode() {
            var Url2 = document.getElementById("code");
            Url2.select(); // 选择对象
            document.execCommand("Copy"); // 执行浏览器复制命令
            alert("复制完成，可贴粘。");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div ng-app="mainApp" ng-controller="countController">
            <div ng-repeat="item in dataList">
                <label style="float: left; margin-left: 20px;">
                    名称
                </label>
                <div style="clear: both; height: 10px;"></div>
                <div style="float: left; margin-left: 20px;">
                    <asp:TextBox ReadOnly="true" ID="Name" CssClass="inputstyle" runat="server" Text="{{item.CTRName}}"></asp:TextBox>
                </div>
                <div style="clear: both; height: 10px"></div>
                <label style="float: left; margin-left: 20px;">
                    网址
                </label>
                <div style="clear: both; height: 10px;"></div>
                <div style="float: left; margin-left: 20px;">
                    <asp:TextBox ReadOnly="true" ID="website" CssClass="inputstyle" runat="server" Text="{{item.CTRUrl}}"></asp:TextBox>
                </div>
                <div style="clear: both; height: 10px;"></div>
                <label style="margin-left: 20px; float: left">
                    代码
                </label>
                <div style="clear: both; height: 10px;"></div>
                <div style="float: left; margin-left: 20px;">
                    <asp:TextBox ReadOnly="true" ID="code" CssClass="inputstylehigh" runat="server" TextMode="MultiLine" Text="{{item.CTRCode}}"></asp:TextBox>
                </div>
                <div style="clear: both; height: 10px;"></div>
                <div style="float: left; margin-left: 215px;">
                    <input type="button" class="buttonstyle" runat="server" value="复制代码" onclick="CopyCode()" />
                </div>
            </div>
        </div>
    </form>
    <script>
        ///CountTool/ajax.ashx?action=GetResourceDetail&resourceId=1
        var codeid = GetQueryString("ID");
        angular.module('mainApp', []).controller('countController', function ($http, $scope) {
            $http.get('ajax.ashx?action=GetResourceDetail&resourceId=' + codeid + '').success(function (data) {
                $scope.dataList = data.list;
            }).error(function (data, status, headers, config) {
            });
        });
    </script>
</body>
</html>
