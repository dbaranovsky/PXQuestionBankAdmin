<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<link href ="<%= Url.Content("~/Rag/css/My_Site.css") %>" rel="stylesheet" type="text/css" />
<link href= "<%= Url.Content("~/Rag/css/RAg.css") %>" rel="stylesheet" type="text/css" />

<script type="text/javascript">
    var MySite_CtrlWin = window.self;
    var MySite_PageWin = null;
    var RA_CtrlWin = window.self;	   
</script>

<%
    if (ViewData["rascripts"] != null)
    {
        foreach (string src in (string[])ViewData["rascripts"])
        {                         
%>
            <script type="text/javascript" src="<%=src%>" ></script>

<%
        }
    }   
%>  

<link type="text/css" href= "<%= ViewData["racss"] %>"  rel="stylesheet"  />
<script type="text/javascript">
   
    RA_CtrlWin.RA.setOptions({
        ProxyType: 'ASP.NET'
        , LocalProxyURL: '/RAg/RAgLocal.asmx'
        , LocalRAifURL: '/RAg/RAif.html'
        , UseIFrame: false
    });
</script>
<script type="text/javascript">
   
    var popupsblocked = false;
    var cookiesblocked = !navigator.cookieEnabled;
    if (!cookiesblocked) {
        RA_CtrlWin.RA.RAXS.Check();
    }       
</script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Rag/RAg.js") %>"></script>

<script type="text/javascript" >
    //RA_CtrlWin.RA.WaitFor_go = MySite_RAReady_Go_Logout;
    jQuery(document).ready(function() {
        MySite_init();
    });
</script>