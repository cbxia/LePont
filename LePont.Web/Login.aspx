<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LePont.Web.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>系统登录</title>
    <link href="css/login.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="dialog">
        <asp:TextBox ID="TextBox_UserID" runat="server"></asp:TextBox>
        <asp:TextBox ID="TextBox_Password"  TextMode="Password" runat="server"></asp:TextBox>
        <asp:Button ID="Button_Login" Text="登录系统" runat="server" 
            onclick="Button_Login_Click" />
    </div>
    </form>
</body>
</html>
