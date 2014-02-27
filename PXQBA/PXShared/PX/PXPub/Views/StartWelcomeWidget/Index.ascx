<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.StartWelcomeWidget>" %>
<%--<% Html.RenderAction("StartPageCourseHeader", "Header"); %>  --%>
<div class="facePlate-start_welcome">
    <div class="facePlate-start_welcome_title">
        <div>
            <strong><%= Model.Title %></strong>
        </div>
    </div>
    <div class="facePlate-start_welcome_msg">
        <%= HttpUtility.HtmlDecode(Model.Contents) %>
    </div>
</div>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/StartWelcomeWidget/StartWelcome.js") %>'], function () {
                StartWelcomeWidget.Init();
            });
        });

    } (jQuery));    
</script>
