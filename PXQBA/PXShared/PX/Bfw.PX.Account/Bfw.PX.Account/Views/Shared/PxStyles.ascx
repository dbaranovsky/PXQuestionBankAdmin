<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%
    var targetUrl = Request.QueryString["target"];
    var path = string.Empty;
    if (!string.IsNullOrEmpty(targetUrl))
    {
        var uri = new Uri(targetUrl);        
        int n;
        var segments = uri.Segments;        
        
        for (int s = 0; s < segments.Length; ++s)
        {
            path += segments[s];
            if (int.TryParse(segments[s], out n))
                break;
        }

        var port = (uri.Port != 80) ? string.Format(":{0}", uri.Port) : string.Empty;
        path = string.Format("{0}://{1}{2}{3}/Style", uri.Scheme, uri.Host, port, path);   
    }
%>

<link rel="Stylesheet" type="text/css" href="<%= path %>" />