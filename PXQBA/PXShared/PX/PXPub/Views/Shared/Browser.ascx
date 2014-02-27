<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
    var browser = Request.Browser.Browser.ToLowerInvariant();
    var versionCss = "";
    if(browser == "applemac-safari")
    {
        var userAgent = Request.UserAgent;
        if(userAgent.Contains("Chrome"))
        {
            browser = "chrome";
        }
        else if(userAgent.Contains("Safari"))
        {
            browser = "safari";
        }
    }
    else if (browser == "ie")
    {
        var version = Convert.ToDouble(Request.Browser.Version);
        versionCss = " ie" + Math.Truncate(version).ToString();
    }
    browser = " " + browser + versionCss;
%>
 <%= browser %>