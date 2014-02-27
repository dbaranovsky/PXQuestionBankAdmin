<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>

<% var courseType = ViewData["courseType"] == null ? "" : ViewData["courseType"].ToString().ToLower(); %>
                <li><br />
					<label class="title" for="Title">Title</label><em><label>required</label></em><br />
					<%= Html.TextBoxFor(m => m.Title, new { @class = "title required clear-float" })%><br />
    <% 
        if (courseType == "faceplate" && !(Model is PxUnit)) 
        {  
    %>
    <li>
        <label class="subtitle" for="Subtitle">Subtitle</label><br />
        <%= Html.TextBoxFor(m => m.SubTitle, new { @class = "subtitleInput"})%><br /><br />
    </li>
    <% 
        } 
    %>
        <%= Html.ValidationMessage("Title") %>                    
		<%= Html.ValidationMessage("content.Title") %>  
        </li>                  