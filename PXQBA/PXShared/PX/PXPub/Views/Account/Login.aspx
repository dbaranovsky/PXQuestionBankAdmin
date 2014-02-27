<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.Account>" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Login</title>
    <% Html.RenderPartial("BaseStyle"); %>
</head>
<body class="login-form product-type-<% Html.RenderAction("GetProductType", "Course"); %> <% Html.RenderPartial("Browser"); %>">
    
    <% Html.RenderPartial("BaseHeaderScripts"); %>
    <% Html.RenderPartial("BaseScripts"); %>

    <div class="login-header"></div>
    <div class="login-page-body">
           <% using (Html.BeginForm("Login", "Account", FormMethod.Post, new { @class = "loginform" }))
           { %>
            <div class="login-course-info">
                <div class="course-info-top">
                    <div class="banner-image"></div>
                </div>
                <div class="course-info-bottom">
                    <div class="book-cover"></div>
                </div>
            </div>
            <div id="MySite_UserInfo_AnonInner">
                <h2>Login</h2>
                <div id="form">
                    <%= Html.ValidationSummary(true)%>
                    <div id="divUserName">
                        <%= Html.LabelFor(m => m.Username)%>
                        <%= Html.TextBoxFor(m => m.Username)%>
                        <%= Html.ValidationMessageFor(m => m.Username)%>
                    </div>
                    <div id="divPwd">
                        <%= Html.LabelFor(m => m.Password)%>
                        <%= Html.PasswordFor(m => m.Password)%>
                        <%= Html.ValidationMessageFor(m => m.Password)%>
                    </div>
                    <div class="logbtn">
                        <input type="submit" value="Login" />
                    </div>
                </div>
            </div>
        <% } %>
        <div id="RAif_div"></div>
    </div>
    <div id="login-footer">
        <div id="platformX-logo"></div>
     <span>
        <%= Html.ActionLink("Terms and Conditions", "TermsAndConditions", "Account", "", new { @class = "fne-link", @title = "Terms and Conditions" })%> |
        <a class="fne-link" href="#">Privacy Policy</a> |
        <a class="fne-link" href="#">Contact Us</a> |
        <a class="fne-link" href="#">System Check</a>
        <span class="footer-copyright">© 2012 Macmillan. All rights Reserved</span>
     </span> 
    </div>
    <% Html.RenderAction("RenderFne","Course"); %>
    <% Html.RenderPartial("ModalWindows"); %>
</body>
</html>