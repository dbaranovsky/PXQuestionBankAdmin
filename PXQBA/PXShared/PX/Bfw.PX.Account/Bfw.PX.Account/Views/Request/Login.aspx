<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Log in
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% 
        Html.RenderPartial("~/Views/Shared/AuthenticationScripts.ascx"); 
    %>
    <style type="text/css">
    .debug {display:none;}
    
    .ui-widget-overlay {
        background: black;
        opacity: 0.5;
        filter: alpha(opacity = 50);
        position: absolute;
        top: 0;
        left: 0;
     }

    .ui-widget-header
    {
        background: #FFFFFF;
        border:1px solid #000000;
        color:#FFFFFF;
        height:26px;
        font-weight:bold;
        border-collapse:collapse;
    }

    .ui-dialog-titlebar-close  
    {
        height:22px;
        padding:4px;
        position:absolute;
        right:0.3em;
    }

    .ui-dialog-content {
        border-bottom: 1px solid #000;
        border-left: 1px solid #000;
        border-right: 1px solid #000;
        background-color:#fff;
    }
    </style>

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.6.4/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.16/jquery-ui.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $('.fne-link').click(function () {
                var href = $(this).attr('href');
                $('<div><p class="popup-content"></p></div>').dialog({
                    autoOpen: true,
                    modal: true,
                    height: 400,
                    width: 600,
                    open: function () { $(this).find('.popup-content').load(href);},
                    close: function () { $(this).dialog('destroy');},
                    overlay: { opacity: 0.5, background: 'black'}
                });
                return false;
            });
        });
    </script>

    <script type="text/javascript">
        var testSSOData = {};
        try {
            testSSOData = <%=ViewData["Context"] %>;
        }
        catch(e){}
        //alert(PublicUrl +'\n'+ SecureUrl +'\n'+ TargetPublicUrl +'\n'+ TargetSecureUrl );
        (function($) {
            $(document).ready(function() {
	            if (RA_CtrlWin.RA.dev_check('any')) {
                    $('#Page_continue').show();
	                $('.debug').show();
	            }
            });
            MySite_CtrlWin.BFWj.Page.RAReady_Go = function() {
                if (RA_CtrlWin.RA.CurrentUser == null) {
                    MySite_CtrlWin.MySite_RAif_init_go('login');
                }
                else {
                    var MySite_BaseURL = '';
                    MySite_BaseURL = TargetPublicUrl;
                    if (!(MySite_BaseURL == '' || MySite_BaseURL == null)) {
                        //MySite_BaseURL = 'dev.px.bfwpub.com/henrettaconcise4ev2/bcs/379';
                        if (MySite_BaseURL.indexOf('http') != 0) {
                            MySite_BaseURL = 'http://' + MySite_BaseURL;
                        }
                        //window.location.href = MySite_BaseURL;
                        $('#Page_continue').html('<div style="padding:10px">You have logged in. Continue to the target page: <a href="' + MySite_BaseURL + '">' + MySite_BaseURL + '</a>.</div>');
                        $('#Page_continue').show();
                        var qstr = window.location.search;
                        if (qstr.indexOf('?') < 0) {
                            qstr = '?' + qstr;
                        }
                        var userHtml = 'Welcome ' + RA_CtrlWin.RA.CurrentUser.FName + '&nbsp;' + RA_CtrlWin.RA.CurrentUser.LName + '. You have ' + RA_CtrlWin.RA.GetLevelOfAccess_Description(RA_CtrlWin.RA.CurrentSiteAccess()) + ' access. -- <a href="Javascript:MySite_CtrlWin.MySite_RAif_init_go(\'dologout\')">Log out</a> (or <a href="'+ SecureUrl +'">Log out</a>)';
                        $('#student-info').html(userHtml);
                    }
                }
            }
        })(jQuery);
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="login-header"></div>
<div>
<form id="LoginForm" action="JavaScript:RA_CtrlWin.RA.RAif.app.goLogin();">
            <div class="login-course-info">
                <div class="course-info-top">
                    <div class="banner-image"></div>
                </div>
                <div class="course-info-bottom">
                    <div class="book-cover"></div>
                </div>
            </div>
            <div id="MySite_UserInfo_AnonInner">              
                <div id="RAif_div" style="display: block;"></div>
            </div>
        </form>
     </div>   
        <div id="Urls" class="debug">Urls = 
        <%
            var Urls = ViewData["Urls"];
             %>
             <%=Urls%>
        </div>
        <div id="BfwAuthSession" class="debug">BfwAuthSession = 
        <%
            var BfwAuthSession = ViewData["BfwAuthSession"];
             %>
             <%=BfwAuthSession%>
        </div>
        <div id="IsProtected" class="debug">IsProtected = 
        <%
            var IsProtected = ViewData["IsProtected"];
             %>
             <%=IsProtected%>
        </div>
        <div id="SwitchToProtected" class="debug">SwitchToProtected = 
        <%
            var SwitchToProtected = ViewData["SwitchToProtected"];
             %>
             <%=SwitchToProtected%>
        </div>
        <div id="SecureUrl" class="debug">SecureUrl = 
        <%
            var SecureUrl = ViewData["SecureUrl"];
             %>
             <%=SecureUrl%>
        </div>
        <div id="RaBaseUrl" class="debug">RaBaseUrl = 
        <%
            var RaBaseUrl = ViewData["RaBaseUrl"];
             %>
             <%=RaBaseUrl%>
        </div>
        <div id="AgilixCourseId" class="debug">AgilixCourseId = 
        <%
            var AgilixCourseId = ViewData["AgilixCourseId"];
             %>
             <%=AgilixCourseId%>
        </div>
        <div id="cxt" class="debug">
        <%
            var cxt = ViewData["Context"];
             %>
             <%=cxt%>
        </div>
<div id="Page_continue" class="debug"></div>
<%--<div id="RAif_div"></div>--%>
<div><a id="RA_debugmsg_toggle" href="JavaScript:void(0);">toggle display of RA debug message</a></div>
<div id="RA_debugmsg">
</div>
<div><a id="RA_info_div_toggle" href="JavaScript:void(0);">toggle display of BFW RA data</a></div>
<div id="RA_info_div">
<p><b>RA data dump</b>
</p>
<div id="RA_info_divInner">
</div>
</div>
    <div id="login-footer">
        <div id="platformX-logo"></div>
        © Copyright 2011 Bedford, Freeman & Worth Publishing Group, LLC. All rights reserved. 
        <%= Html.ActionLink("Terms and Conditions", "TermsAndConditions", "Account", "", new { @class = "fne-link", @title = "Terms and Conditions" })%>
    </div>
</asp:Content>

