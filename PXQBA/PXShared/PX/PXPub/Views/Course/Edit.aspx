<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ThreeColumn.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.Course>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%if (Model.Id == null) {%>
	    New
	<%}else { %>
	    Edit
    <%} %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">

    <h2>Edit</h2>

    <% using (Html.BeginForm("SaveCourse", "Course")) {%>
        <%= Html.ValidationSummary(true) %>
        
        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.Id) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.Id) %>
                <%= Html.ValidationMessageFor(model => model.Id) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.Title) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.Title) %>
                <%= Html.ValidationMessageFor(model => model.Title) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.CourseUserName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.CourseUserName) %>
                <%= Html.ValidationMessageFor(model => model.CourseUserName) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.CourseProductName) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.CourseProductName) %>
                <%= Html.ValidationMessageFor(model => model.CourseProductName) %>
            </div>
            
            <p>
                <input type="submit" value="Save"  />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%= Html.ActionLink("Back to List", "ViewAll", "CourseWidget") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

