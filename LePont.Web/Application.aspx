<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Application.aspx.cs" Inherits="LePont.Web.Application" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>司法局矛盾纠纷调解管理系统</title>
    <%--css--%>
    <link href="Libraries/jquery-ui/themes/Aristo/jquery-ui-1.8.7.custom.css" rel="stylesheet"
        type="text/css" />
    <%--    <link href="Libraries/jquery-ui/themes/redmond/jquery-ui-1.8.13.custom.css" rel="stylesheet"
        type="text/css" />--%>
    <link href="Libraries/dynatree/css/skin/ui.dynatree.css" rel="stylesheet" type="text/css" />
    <link href="Libraries/jquery-validation/css/core.css" rel="stylesheet" type="text/css" />
    <link href="css/global.css?nocache" rel="stylesheet" type="text/css" />
    <link href="css/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <%--scripts--%>
    <script src="Libraries/jquery/jquery-1.5.1.min.js" type="text/javascript"></script>
    <script src="Libraries/jquery-ui/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script src="Libraries/jquery-ui/jquery.ui.datepicker-zh-CN.js" type="text/javascript"></script>
    <script src="Libraries/jquery-template/jquery.tmpl.min.js" type="text/javascript"></script>
    <script src="Libraries/dynatree/jquery.dynatree.js" type="text/javascript"></script>
    <script src="Libraries/jquery-validation/jquery.validate.min.js" type="text/javascript"></script>
    <script src="Libraries/jquery-validation/jquery.metadata.js" type="text/javascript"></script>
    <script src="Libraries/jquery-validation/messages_cn.js" type="text/javascript"></script>
    <script src="Libraries/jquery-format/jquery.format.js" type="text/javascript"></script>
    <script src="js/json2.js" type="text/javascript"></script>
    <script src="Libraries/ckeditor/ckeditor.js" type="text/javascript"></script>
    <script src="Libraries/ckeditor/adapters/jquery.js" type="text/javascript"></script>
    <script src="Libraries/AjaxFileUploader/ajaxfileupload.js" type="text/javascript"></script>
    <script src="js/jquery.treeview.js" type="text/javascript"></script>
    <script src="js/application.js?nocache" type="text/javascript"></script>
    <script src="js/modules.js" type="text/javascript"></script>
    <script src="js/utils.js?nocache" type="text/javascript"></script>
    <script src="js/dep-tree.js?nocache" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //// Centering
            function centerContent() {
                var x = ($(window).width() - $("#page").width()) / 2;
                $("#page").offset({ left: x > 0 ? x : 0 });
            }
            $(window).resize(function () {
                centerContent();
            });
            $("#page").css("margin", "0 0");
            centerContent();

            $("#ajaxLoading").ajaxStart(function () {
                $(this).show();
            });
            $("#ajaxLoading").ajaxStop(function () {
                $(this).hide();
            });

            //// App context & view switching 
            Application.Load().done(function () {
                // Generate menu
                $("#menu-tree").treeview();
                // Attach menu events
                $("#menu-tree").delegate(".file", "click", function () {
                    if (typeof (this.attributes["module"]) !== "undefined")
                        Application.LoadModule(this.attributes["module"].value);
                });
                $("#header .top-links").delegate(".home", "click", function () {
                    Application.LoadModule("Home");
                });
                // 
                showWelcomeMessage();
                showSessionSummary();
                Application.LoadModule("TopNav", null, "#work-area .top-nav");
                Application.LoadModule("Home");
            });

            // TODO: Move this to module.
            function showWelcomeMessage() {
                $("#welcome-message .name").html("欢迎：" + Application.GetCurrentUser().Name);
                $("#welcome-message .last-logon").html("最近登录时间：");
                $("#welcome-message .ip").html("登录IP：");
                $("#welcome-message .log-count").html("本月第次登录");
                $("#welcome-message .op-count").html("本月已报送条信息");
                $("#welcome-message").dialog({
                    title: "欢迎登录",
                    width: 300,
                    modal: false,
                    buttons: {
                        "确定": function () {
                            $(this).dialog("close");
                        }
                    }
                });
            }

            function showSessionSummary() {
                var d = new Date();
                $("#header .session-summary").html($.format("当前用户：%s  今天是：%d年%d月%d日  天气：", [Application.GetCurrentUser().Name, d.getFullYear(), d.getMonth() + 1, d.getDate()]));
            }

            $("#menu-grip").mouseover(function () {
                $(this).addClass("hover");
            });

            $("#menu-grip").mouseout(function () {
                $(this).removeClass("hover");
            });

            $("#menu-grip").click(function () {
                if ($(this).hasClass("opened")) {
                    $(this).removeClass("opened").addClass("closed");
                    $(this).html("显示菜单");
                    $("#nav-area").addClass("hidden");
                    $("#work-area").addClass("max");
                }
                else {
                    $(this).removeClass("closed").addClass("opened");
                    $(this).html("隐藏菜单");
                    $("#nav-area").removeClass("hidden");
                    $("#work-area").removeClass("max");
                }
            });
        });

    </script>
    <%--Firebug lite--%>
<%--    <script type="text/javascript" src="https://getfirebug.com/firebug-lite.js"></script>
--%></head>
<body>
    <div id="page">
        <div id="header">
            <div class="roller">
                预警信息</div>
            <div class="session-summary">
            </div>
            <div class="top-links">
                <form runat="server">
                <span class="home">首页</span> <span class="help">帮助</span>
                <asp:LoginStatus ID="LoginStatus" LoginText="logintext" LogoutText="登出" runat="server" />
                </form>
            </div>
        </div>
        <div id="main">
            <div id="nav-area" class="nav-area hidden">
                <ul id="menu-tree" class="filetree treeview">
                    <li><span class="file bright home" module="Home">最新动态</span></li>
                    <li><span class="folder bright">信息管理</span>
                        <ul>
                            <li><span class="file bright" module="CaseManager">按类别管理</span></li>
                            <li><span class="file bright" module="CaseManager">按地区管理</span></li>
                            <li><span class="file bright">回收站</span></li>
                        </ul>
                    </li>
                    <li><span class="folder">统计查询</span>
                        <ul>
                            <li><span class="file">信息统计</span></li>
                            <li><span class="file">登录统计</span></li>
                        </ul>
                    </li>
                    <li><span class="folder">系统管理</span>
                        <ul>
                            <li><span class="file" module="UserManager">用户管理</span></li>
                            <li><span class="file" module="DepartmentManager">部门管理</span></li>
                            <li><span class="file" module="DataDictionaryManager">类别管理</span></li>
                            <li><span class="file" module="WorkNetwork">调解网络</span></li>
                        </ul>
                    </li>
                    <li><span class="file" module="BBS">网上交流</span></li>
                </ul>
            </div>
            <div id="work-area" class="max">
                <div class="top-nav">
                </div>
                <div class="module">
                </div>
                <div class="layers">
                    <div id="ajaxLoading">
                        <img src="css/images/ajax-loading.gif" alt="Ajax loading..." />
                    </div>
                    <div id="welcome-message">
                        <div class="name">
                        </div>
                        <div class="last-logon">
                        </div>
                        <div class="ip">
                        </div>
                        <div class="log-count">
                        </div>
                        <div class="op-count">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="footer">
            济南司法局矛盾纠纷调解管理系统 版权所有 © 2011年6月
        </div>
        <div id="menu-grip" class="closed">
            隐藏菜单
        </div>
    </div>
</body>
</html>
