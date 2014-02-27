<%@ Page Title="404 Page Not Found" Language="C#" Inherits="System.Web.Mvc.ViewPage<System.Web.HttpException>" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">

<html>
<head>
<title>Internal Server Error 500</title>

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
    margin: 10% auto;
    background-color: #fff;
}
/* error image is embedded so we dont trigger an error cascade */
#errorpage-container .icon{
    background-image: url(data:image/png;base64,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);
    display:block;
    height:90px;
    width:96px;
    float:left;
}
#errorpage-container .message {
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
    font-size:15px;
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
            <% if (bShowErrors)
               { %>
                    <span onclick="document.getElementById('detail-msg-container').style.display='block'; document.getElementById('detailLinkHide').style.display='block'; this.style.display='none';" id="detailLink" class="showdetailslink">Show details</span>
                    <span onclick="document.getElementById('detail-msg-container').style.display='none'; document.getElementById('detailLink').style.display='block'; this.style.display='none';" id="detailLinkHide" class="showdetailslink" style="display:none;">Hide details</span>
            <% } %>
        </div>

        <% if (bShowErrors)
           {
        %>
            <div id="detail-msg-container" style="display:none;">
                <div class="errorMessage1"><%= Model.Message %></div>
                <div class="errorMessage2" >
                     <% if (Model.Message != null)
                        { %>
                        <%= Server.HtmlEncode(Model.StackTrace) %>
                         <% if (Model.StackTrace.IsNullOrEmpty())
                            { %>
                            <%= Server.HtmlEncode(Model.InnerException.ToString()) %>
                         <% } %>
		             <% } %>
                </div>
            </div>
        <% } %>
    </body>
</html>