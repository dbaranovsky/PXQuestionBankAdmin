<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var raBaseUrl = ConfigurationManager.AppSettings["RABaseUrl"]; %>

<%-- link href ="<%= Url.Content("~/Rag/css/My_Site.css") %>" rel="stylesheet" type="text/css" / --%>
<%-- link href= "<%= Url.Content("~/Rag/css/RAg.css") %>" rel="stylesheet" type="text/css" / --%>
<link href="<%= raBaseUrl %>/BFWglobal/RAg/v3.0/PlatformFiles/PX/My_Site.css" rel="stylesheet" type="text/css" />
<link href="<%= raBaseUrl %>/BFWglobal/RAg/v3.0/PlatformFiles/PX/RAg.css" rel="stylesheet" type="text/css" />
<%-- link href= "<%= raBaseUrl %>/BFWglobal/RAg/NAMv1.0/css/RAg.css" rel="stylesheet" type="text/css" / --%>
<link href="<%= raBaseUrl %>/BFWglobal/RAg/v3.0/css/RAif.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">
    var MySite_CtrlWin = window.self;
    var MySite_PageWin = null;
    var RA_CtrlWin = window.self;
</script>

<%    
    if (raBaseUrl != null)
    {
        var scripts = new string[] { "/BFWglobal/js/jquery/jquery-1.4.4.min.js", "/BFWglobal/js/json2.js", "/BFWglobal/js/jquery/jquery.cookie.js", "/BFWglobal/js/global.js", "/BFWglobal/js/BFW_LogError.js", "/BFWglobal/js/BFW_Error.js", "/BFWglobal/RAg/v3.0/RA.js", "/BFWglobal/RAg/v3.0/RAWS.js", "/BFWglobal/RAg/v3.0/RAif.js" };
//        var scripts = new string[] { "/BFWglobal/js/jquery/jquery-1.4.4.min.js", "/BFWglobal/js/json2.js", "/BFWglobal/js/jquery/jquery.cookie.js", "/BFWglobal/js/global.js", "/BFWglobal/js/BFW_LogError.js", "/BFWglobal/js/BFW_Error.js", "/BFWglobal/RAg/NAMv1.0/RA.js" };
        foreach (var script in scripts)
        {
%>
            <script type="text/javascript" src="<%=raBaseUrl%><%= script %>" ></script>
<%
        }
    }   
%>
<%
    var PublicUrl = ViewData["PublicUrl"];
    var SecureUrl = ViewData["SecureUrl"];
    var TargetPublicUrl = ViewData["TargetPublicUrl"];
    var TargetSecureUrl = ViewData["TargetSecureUrl"];
%>

<script type="text/javascript">
    var $ = null;
    RA_CtrlWin.RA.setOptions({
        ProxyType: 'ASP.NET'
        , LocalProxyURL: '/RAg/RAgLocal.asmx'
        , LocalRAifURL: '/RAg/RAif.html'
        , UseIFrame: false
        , UsingClasses: false
        , CookiePath: '/'
        , RavellAuthentication: true
        , PublicUrl: '<%=PublicUrl%>'
        , SecureUrl: '<%=SecureUrl%>'
        , TargetPublicUrl: '<%=TargetPublicUrl%>'
        , TargetSecureUrl: '<%=TargetSecureUrl%>'
    });

    var popupsblocked = false;
    var cookiesblocked = !navigator.cookieEnabled;
    if (!cookiesblocked) {
        RA_CtrlWin.RA.RAXS.Check();
    }
    else {
    }
    
</script>
<%-- script type="text/javascript" src="<%= Url.Content("~/Scripts/Rag/RAg.js") %>"></script --%>
<script type="text/javascript" src="<%= raBaseUrl %>/BFWglobal/RAg/v3.0/PlatformFiles/PX/RAg.js"></script>
<script type="text/javascript" >
    jQuery(document).ready(function() {
        MySite_init(jQuery);
    });
</script>