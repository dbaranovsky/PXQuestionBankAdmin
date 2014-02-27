<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%@ Import Namespace="System.IO"%>
<h2 id="fne-title">
        <span id="fne-title-left">
         <a href="javascript:" id="fne-unblock-action-home" class="fne-home-icon" style="display:none;">
         <span class="home-btn-icon"></span>
         Home
         </a>
            <span id="fne-title-breadcrumb"></span>

            <span id="fne-title-actions">
                <span id="fne-title-action-left"></span>
                <span id="fne-title-action-right">
                    <a href="<% =Path.GetFileName(Request.Path) %>" id="fne-unblock-action"></a><a href="<% =Path.GetFileName(Request.Path) %>" id="fne-minimize-action" style="display:none;"></a>
                </span>
            </span>
        </span>
        <!--<span id="nav-container">
            <span id="back" class="navigate-back"><span class="navbtn-back-icon"></span></span>
            <span id="next" class="navigate-next">Next <span class="navbtn-next-icon"></span></span>
        </span>-->
        <span id="fne-title-right">
            <span id="fne-title-right-nav"></span>
        </span>
        <% Html.RenderPartial("ContentModes", Model, ViewData); %>
    </h2>