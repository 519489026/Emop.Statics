<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserLogin.aspx.cs" Inherits="StatisticalCenter.UserLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="Script/jquery-1.7.2.js"></script>
    <link href="Style/login.css" rel="stylesheet" />
    <title>运营管理平台登录</title>
    <style type="text/css">
        .inp3Tips {
            margin-left: 44px;
            height: 40px;
            line-height: 40px;
            border: none;
            width: 266px;
            height: 38px;
            border: 1px solid #e4e4e4;
            background: #f2f2f2;
            text-indent: 10px;
            display: none;
        }

        .loginbutton {
            background: #54B9CD;
            width: 312px;
            height: 40px;
            margin: 0 auto;
            margin-top: 20px;
            display: block;
            color: white;
            font-size: 14px;
            text-align: center;
            line-height: 300%;
            text-decoration: none;
            cursor: pointer;
        }
    </style>
    <script type="text/javascript">
    </script>
    <script>
        $(document).ready(function () {
            $("#txtAccount").css({ "color": "#888" })
            $('#txtPwd1').css({ "color": "#888" })

            $("#txtAccount").focus(function () {
                if ($(this).val() == '用户名') {
                    $(this).val('');
                }
            });
            $("#txtAccount").blur(function () {
                if ($(this).val() == '') { $(this).val('用户名') }
            });
            $("#txtAccount").keydown(function () { $(this).css({ "color": "#000" }) })
            $('#txtPwd1').focus(function () {
                if ($(this).val() == '输入密码') {
                    $(this).hide();
                    $('#txtPwd').show();
                    $('#txtPwd').focus();
                }

            })
            $('#txtPwd').blur(function () {
                if ($(this).val() == '') {
                    $(this).hide();
                    $('#txtPwd1').show();
                }
            })
            $('#txtPwd').keydown(function () { $(this).css({ "color": "#000" }) })
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="mid_bg">
            <div id="title">运营管理后台登录</div>
            <div id="user">
                <%--<input id="txtAccount" type="text" value="用户名" class="inp1Tips" />--%>
                <asp:TextBox CssClass="inp1Tips" runat="server" ID="txtAccount" Text="用户名"></asp:TextBox>
            </div>
            <div id="psw">
                <asp:TextBox ID="txtPwd1" runat="server" Text="输入密码" CssClass="inp2Tips"></asp:TextBox>
                <%--<input id="txtPwd1" type="text" value="输入密码" class="inp2Tips" />--%>
                <asp:TextBox ID="txtPwd" CssClass="inp3Tips" runat="server" TextMode="Password"></asp:TextBox>
                <%--<input id="txtPwd" type="password" style="display: none;" class="inp2Tips" />--%>
            </div>
            <asp:Button ID="a_login" CssClass="loginbutton" runat="server" Text="登 录" OnClick="a_login_Click" />
            <%--<a id="a_login" href="#" onclick="checkInput();">登 录</a>--%>
        </div>
    </form>
</body>
</html>
