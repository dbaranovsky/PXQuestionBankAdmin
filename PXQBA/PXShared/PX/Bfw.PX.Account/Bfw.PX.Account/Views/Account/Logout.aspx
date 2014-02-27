<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Log out
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/BFWglobal/js/jquery/jquery-1.4.4.min.js"></script>
    <%
        var PublicUrl = ViewData["PublicUrl"];
        var SecureUrl = ViewData["SecureUrl"];
        var TargetPublicUrl = ViewData["TargetPublicUrl"];
        var TargetSecureUrl = ViewData["TargetSecureUrl"];
    %>

    <style type="text/css">
        .debug
        {
            display: none;
        }
    </style>

    <script type="text/javascript">
        var PublicUrl = '';
        var SecureUrl = '';
        var TargetPublicUrl = '';
        var TargetSecureUrl = '';
        var testSSOData = {};
        var MySite_ReturnURL = '';

    function expireAllCookies(name, paths) {
             var expires = new Date(0).toUTCString();

        // expire null-path cookies as well
        document.cookie = name + '=; expires=' + expires;

        for (var i = 0, l = paths.length; i < l; i++) {
        document.cookie = name + '=; path=' + paths[i] + '; expires=' + expires;
        }
    }

    function expireActiveCookies(name) {
        var pathname = location.pathname.replace(/\/$/, ''),
        segments = pathname.split('/'),
        paths = [];
    
        for (var i = 0, l = segments.length, path; i < l; i++) {
        path = segments.slice(0, i + 1).join('/');
        
        paths.push(path);       // as file
        paths.push(path + '/'); // as directory
        }
        expireAllCookies(name, paths);
    }

        function sendRedirect() {
            expireActiveCookies('SiteUserData');
            expireActiveCookies('JSESSIONID');
            top.location = MySite_ReturnURL;
        }

        try {
            PublicUrl = '<%=PublicUrl%>';
            SecureUrl = '<%=SecureUrl%>';
            TargetPublicUrl = '<%=TargetPublicUrl%>';
            TargetSecureUrl = '<%=TargetSecureUrl%>';
            testSSOData = <%=ViewData["Context"] %>;
            MySite_ReturnURL = '<%= ViewData["GoToTarget"] %>';
        }
        catch(e){}
            jQuery(document).ready(function() {
                sendRedirect();
            });

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

<p><b>Logging out...</b></p>

</asp:Content>

