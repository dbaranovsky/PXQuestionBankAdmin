<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Log out
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <% 
        Html.RenderPartial("~/Views/Shared/AuthenticationScripts.ascx"); 
    %>
    <%
        var PublicUrl = ViewData["PublicUrl"];
        var SecureUrl = ViewData["SecureUrl"];
        var TargetPublicUrl = ViewData["TargetPublicUrl"];
        var TargetSecureUrl = ViewData["TargetSecureUrl"];
    %>
    <style type="text/css">
    .debug {display:none;}
    </style>
    <script type="text/javascript">
        var PublicUrl = '';
        var SecureUrl = '';
        var TargetPublicUrl = '';
        var TargetSecureUrl = '';
        var testSSOData = {};
        try {
            PublicUrl = '<%=PublicUrl%>';
            SecureUrl = '<%=SecureUrl%>';
            TargetPublicUrl = '<%=TargetPublicUrl%>';
            TargetSecureUrl = '<%=TargetSecureUrl%>';
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
                if (RA_CtrlWin.RA.CurrentUser != null) {
                    var userHtml = 'Welcome ' + RA_CtrlWin.RA.CurrentUser.FName + '&nbsp;' + RA_CtrlWin.RA.CurrentUser.LName + '. You have ' + RA_CtrlWin.RA.GetLevelOfAccess_Description(RA_CtrlWin.RA.CurrentSiteAccess()) + ' access. -- <a href="Javascript:MySite_CtrlWin.MySite_RAif_init_go(\'dologout\')">Log out</a> (or <a href="<%= Url.Content("~/Account/Logout/") %>' + qstr + '">Log out</a>)';
                    $('#student-info').html(userHtml);
//                    MySite_CtrlWin.MySite_RAif_init_go('dologout');
                }
                else {
                    var userHtml = 'Welcome. -- <a href="Javascript:MySite_CtrlWin.MySite_RAif_init_go(\'login\')">Log in</a> (or <a href="'+PublicUrl +'">Log in</a>)';
                    $('#student-info').html(userHtml);
                    var MySite_BaseURL = '';
                    MySite_BaseURL = TargetPublicUrl;
                    if (!(MySite_BaseURL == '' || MySite_BaseURL == null)) {
                        //MySite_BaseURL = 'dev.px.bfwpub.com/henrettaconcise4ev2/bcs/379';
                        if (MySite_BaseURL.indexOf('http') != 0) {
                            MySite_BaseURL = 'http://' + MySite_BaseURL;
                        }
                        $('#Page_continue').html('<div style="padding:10px">You have logged out. Continue to the target page: <a href="' + MySite_BaseURL + '">' + MySite_BaseURL + '</a>.</div>');
                        $('#Page_continue').show();
                        //window.location.href = '/Request/Login/' + qstr;
                    }
                }
            }
        })(jQuery);
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
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
<div id="RAif_div"></div>
<div id="Page_continue" class="debug"></div>
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
</asp:Content>

