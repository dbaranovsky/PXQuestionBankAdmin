<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%
    var returnUrl = string.Empty;
    var returnTitle = "Blackboard";

    var returnUrlCookie = String.Format("{0}_ReturnURL", ViewContext.RouteData.Values["courseid"]);
    var returnTitleCookie = String.Format("{0}_ReturnTitle", ViewContext.RouteData.Values["courseid"]);

    if (Request.Form["return_url"] != null && Request.Form["return_title"] != null)
    {
        //when we get the Return title and URL POSTed to the client, remove exisitng cookies and replaces them with POSTed values
        Response.Cookies.Remove(returnUrlCookie);
        Response.Cookies.Remove(returnTitleCookie);
        Request.Cookies.Remove(returnUrlCookie);
        Request.Cookies.Remove(returnTitleCookie);
        HttpCookie urlCookie = new HttpCookie(returnUrlCookie, Request.Form["return_url"]);
        HttpCookie titleCOokie = new HttpCookie(returnTitleCookie, Request.Form["return_title"]);
        Response.Cookies.Add(urlCookie);
        Response.Cookies.Add(titleCOokie);
        Request.Cookies.Add(urlCookie);
        Request.Cookies.Add(titleCOokie);
        
    }
    
    if (Request.Cookies.AllKeys.Contains(returnUrlCookie) && !Request.Cookies[returnUrlCookie].Value.IsNullOrEmpty())
    {
        returnUrl = Request.Cookies[returnUrlCookie].Value;
    }

    if (Request.Cookies.AllKeys.Contains(returnTitleCookie) && !Request.Cookies[returnTitleCookie].Value.IsNullOrEmpty())
    {
        returnTitle = Server.UrlDecode(Request.Cookies[returnTitleCookie].Value);
    }    

    var display = !returnUrl.IsNullOrEmpty() ? "block" : "none";    
%>

<div class="back-blackboard-btn blackboard-btn-wrapper button secondary small" style="display: <%= display %>"><span class="pxicon pxicon-reply return"></span><a href="<%= returnUrl %>" class="back-blackboard-link"><%= returnTitle %></a></div>