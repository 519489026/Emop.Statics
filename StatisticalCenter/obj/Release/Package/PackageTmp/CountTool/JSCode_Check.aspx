<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JSCode_Check.aspx.cs" Inherits="StatisticalCenter.CountTool.JSCode_Check" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="http://apps.bdimg.com/libs/angular.js/1.5.0-beta.0/angular.js"></script>
    <script src="../Script/jquery-1.7.2.js"></script>
    <script src="../Script/layer/layer.js"></script>
    <script src="../Script/layer/layer.js"></script>
    <link href="../Style/TableDisplay.css" rel="stylesheet" />
    <link href="../Style/pagination.css" rel="stylesheet" />
    <link href="../Style/gridTable.css" rel="stylesheet" />
    <title></title>
    <style type="text/css">
        .inputstylehigh {
            width: 270px;
            border: 1px solid #e0d6d6;
            background-color: #F2F2F2;
            height: 120px;
            line-height: 26px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
         <div style="margin-left:38%;margin-top:15%;">
             <label>校验中......</label>
             <div>
                 <img src="../Content/img/loading.gif" />
             </div>
             <input type="button" id="testButton" style="display:none" />
         </div>
            <%=strCode %>
            <script type="text/javascript">
                var time = 3;
                var count = 0;
                function GetQueryString(sProp) {
                    var re = new RegExp("[&,?]" + sProp + "=([^\\&]*)", "i");
                    var a = re.exec(document.location.search);
                    if (a == null)
                        return "";
                    return a[1];
                };
                function timeHide(time) {
                    var id = GetQueryString("ID");
                    if (time == 0 && count < 19) {
                        $.ajax({
                            url: "ajax.ashx?action=GetValidResult&resourceId=" + id + ""
                            , type: "GET"
                            , success: function (data) {
                                if (data == 1) {  
                                    alert("校验成功！");
                                    var index = parent.layer.getFrameIndex(window.name); //获取窗口索引
                                    parent.layer.close(index);
                                }
                                else if (data != 1) {
                                    count++;
                                    timeHide(3);
                                }
                            }
                        });
                    }
                    else if (time != 0 && count < 19) {
                        time--;
                    }
                    else if (count == 19) {
                        alert("校验失败！");
                        var index = parent.layer.getFrameIndex(window.name); //获取窗口索引
                        parent.layer.close(index);
                        return;
                    }
                    if (time != 3 && time != -1 && count < 19) {
                        setTimeout(function () {
                            timeHide(time);
                        }, 1000);
                    }
                };
                $(function () {
                    timeHide(time);
                })
            </script>
    </form>
</body>
</html>
