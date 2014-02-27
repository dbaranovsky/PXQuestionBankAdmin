<%@ Page Title="404 Page Not Found" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">

<html>
<head>
<title>Error 404</title>

<style>
body{
font-family: arial, sans-serif;
background-color:#f2f2f2;
}

#errorpage-container{
    border: 4px solid #D4484B;
    height: 300px;
    padding: 50px 25px 0;
    width: 550px;
    margin:10% auto;
    background-color:#fff;
}
#errorpage-container .icon{
    background-image: url("../../Content/images/404-icon.png"); 
    display:block;
    height:90px;
    width:96px;
    float:left;
}
#errorpage-container .message{
    float: left;
    padding: 25px 0 0 25px;
    width: 420px;
    color:#333333;
}
#errorpage-container .sorry{
font-size:30px;
font-weight:bold;
padding-bottom:20px;
border-bottom:3px solid #333;
}
#errorpage-container .sorry span{
font-weight:normal;
color:#D4484B;
}
#errorpage-container .page-exist{
font-weight:bold;
font-size:20px;
padding-top:5px;
}
#errorpage-container .url-check{
border-top:1px dotted  #b3b3b3;
border-bottom:1px dotted  #b3b3b3;
padding: 8px 70px 8px 0;
font-family:georgia;
font-size:medium;
}
#errorpage-container .url-check a{
 color: #D4484B;
 text-decoration:none;
}
#errorpage-container .showdetailslink{
    font-size:10px;
    text-decoration:underline;
    cursor:pointer;
}
#detail-msg-container .errorMessage1{
font-size:16px;
font-weight:bold;
padding-bottom:5px;
}
#detail-msg-container .errorMessage2{
font-size:14px;
}
</style>
</head>
    <body>
        <% var bShowErrors = bool.Parse(ViewData["ShowErrorDetails"].ToString()); %>
        <div id="errorpage-container">
		 <div class="icon"></div>
		 <div class="message">
		 	<div class="sorry">We’re sorry—there’s a technical issue with this page.</div>
            <p class="url-check">Please <a href="http://support.bfwpub.com/supportform/form.php?View=contact">click here</a> to report this issue to the Technical Support team.</p>
		 </div>
            <% if (bShowErrors)  {%>
                    <span onclick="document.getElementById('detail-msg-container').style.display='block'; document.getElementById('detailLinkHide').style.display='block'; this.style.display='none';" id="detailLink" class="showdetailslink">Show details</span>
                    <span onclick="document.getElementById('detail-msg-container').style.display='none'; document.getElementById('detailLink').style.display='block'; this.style.display='none';" id="detailLinkHide" class="showdetailslink" style="display:none;">Hide details</span>
            <% } %>
        </div>

        <% if (bShowErrors)
           {
        %>
        <div id="detail-msg-container" style="display:none;">
            <div class="errorMessage1"><%= Model == null ? "" : Model.Message %></div>
            <div class="errorMessage2" >
                    <% if (Model != null && Model.Message != null)
                       { %>
                    <%= Model.StackTrace %>
                        <% if (Model.StackTrace.IsNullOrEmpty())
                           { %>
                        <%= Model.InnerException.ToString() %>
                        <% } %>
		            <% } %>
            </div>
        </div>
        <% } %>
    </body>
</html>
