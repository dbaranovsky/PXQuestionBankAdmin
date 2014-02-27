<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>

<div class="copyToMyTemplatesContainer">
    <input type="hidden" class="courseId" value="<%= Model.Id %>" />
    <div class="title">
        Template Title: &nbsp;&nbsp;&nbsp;  <input type="text" id="templateTitle" name="templateTitle" value="My copy of <%= Model.Title%>" />
    </div>
    <div class="buttonSet">
	    <input class="copy templateButton" type="button" value="Copy"/>&nbsp;
	    <input class="cancel templateButton" type="button" value="Cancel"/>
    </div>
</div>
<script type="text/javascript">
    PxPage.Require(['<%= Url.ContentCache("~/Scripts/EportfolioDashboard/EportfolioDashboard.js") %>'], function () {
        PxEportfolioDashboard.BindControls();
    });
</script>