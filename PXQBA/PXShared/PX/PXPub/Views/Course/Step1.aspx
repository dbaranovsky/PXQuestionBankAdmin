<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ThreeColumn.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.Course>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Step1
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">

    <h2>Step1</h2>

    <% using (Html.BeginForm("Create", "Course"))
       {%>
        <%= Html.ValidationSummary(true) %>
        
        <div>Welcome <%=Html.DisplayFor(model=>model.CurrentUserName)%>! setup your [Digital product name].</div>
          <br />

                <%= Html.HiddenFor(model => model.Id) %>
             
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
            
            <div class="editor-field">
                <%= Html.DisplayFor(model => model.CourseProductName) %>
            </div>
            
            <p>
                <input type="submit" value="Save" />
                <input type="submit" value="Cancel" />
            </p>

    <% } %>

    <div>
        <%= Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

