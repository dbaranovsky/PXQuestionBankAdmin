<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Widget>" %>
<%
    bool hasOnClick = Model.Properties["bfw_home_callback"] != null;
    var homecallback = hasOnClick ? Model.Properties["bfw_home_callback"].Value : null;
%>
<div id="fne-header-view">
    <!-- Home button -->
    <a href="#" id="page-defined-fne-unblock-action-home" class="home-icon page-defined-fne-home-icon callback-link" 
<% 
if (hasOnClick) { 
%>
     onclick="<%= homecallback %>()"
<% 
} 
%>
    >
        <span class="home-btn-icon"></span>
        Home
    </a>
    <!-- Title/Breadcrumb-->
    <div id="fne-title">
    </div>
</div>