<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RssFeed>" %>

<%
	var cancel = "PxPage.CloseNonModal(); $('li.open-templates').click();";
	var OnSuccess = "ContentWidget.ContentCreated";
    var OnSuccessSaveAndOpen = "ContentWidget.ContentCreatedAndOpen";

    

%>
<%    using (Html.BeginForm("SaveRssFeed", "RssFeed", FormMethod.Post, new { id = "saveItem" })) {
		var title = (Model.Status == ContentStatus.New) ? "Create New Activity" : Model.Title; %>
<h2 class="content-title">
	<%= title %></h2>
<div id="form" class="rss">
	<%= Html.HiddenFor(m => m.Id) %>
	<%= Html.HiddenFor(m => m.ParentId) %>
	<%= Html.HiddenFor(m => m.Type) %>
	<%= Html.HiddenFor(m => m.IsAssignable) %>
	<%= Html.HiddenFor(m => m.Sequence)%>
	<% Html.RenderPartial("HiddenCategoryList", Model.Categories); %>
	<%= Html.HiddenFor(m => m.DefaultCategoryParentId) %>
	<%= Html.Hidden("SyllabusFilter", Model.SyllabusFilter)%>
	<input type="hidden" id="targetMode" value="normal" />
	<%=Html.HiddenFor(m => m.SourceTemplateId) %>
	<%=Html.HiddenFor(m => m.IsBeingEdited) %>
    <div class="create-new-wrapper">
	<ol>
		<li>
			<%= Html.LabelFor(m => m.Title) %>
			<%= Html.TextBoxFor(m => m.Title, new { @class = "title required InputForControllerAction " })%>
			<%= Html.ValidationMessage("Title") %>
			<%= Html.ValidationMessage("content.Title") %>
		</li>
        <br />
		<li>
			<%= Html.LabelFor(m => m.RssUrl) %>
			<%= Html.TextBoxFor(m => m.RssUrl, new { @class = "title InputForControllerAction" , id="inputRssUrl" })%>
			<%= Html.ValidationMessage("RssUrl")%>
			<%= Html.ValidationMessage("content.RssUrl")%>
		</li>
        <br />
        <br />
        </ol>
        </div>
        
</div>
<% } %>
<div id="assign">
    <ol>
        <%if (Model.IsContentCreateAssign){  %>
        <li>
            <div id="assignContainer" class="contentcreate contentwrapper"><%Html.RenderAction("AssignTab", "ContentWidget", new { item = new ContentView { Content = Model, GroupId = Model.GroupId }, IsContentCreateAssign = true}); %></div>
        </li>
        <% } %>
        <li>
			<div class="create-new-btn-wrapper">
			<%
					   var showSave = true;
					   var saveAndOpenText = "Save & Open";
					   if (Model.IsBeingEdited)
					   {
							showSave = false;
							saveAndOpenText = "Save";
					   } %>
            
            <input type="button" name="behavior" value="Save & Open" onclick="PxRssArticle.OnClickSaveAndOpen(<%= OnSuccessSaveAndOpen %>);" /> 
           
			<% if(showSave)
			{ %>
         		<input type="button" name="behavior" value="Save" class="savebtn submit-action" onclick="PxRssArticle.OnClickSave(<%= OnSuccess %>);" /> 
            <% } %>

			| <a href="#" class="create-closecancel" onclick="return PxPage.CloseCreateNewScreen();">Cancel</a> 
    </div>
		</li>
    </ol>
</div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/RssFeed/RssFeed.js") %>'], function () {
                PxRssArticle.BindControls();
            });
        });

    } (jQuery));    
</script>
