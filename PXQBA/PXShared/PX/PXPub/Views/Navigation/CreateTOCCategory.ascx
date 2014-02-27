<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.TocCategory>" %>

    <center>
<% using (Ajax.BeginForm("CreateCategory", "Navigation", new AjaxOptions() { }))
   { %>   
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
                <%= Html.LabelFor(model => model.Text) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.Text) %>
                <%= Html.ValidationMessageFor(model => model.Text) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.ItemParentId) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.ItemParentId) %>
                <%= Html.ValidationMessageFor(model => model.ItemParentId) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.Active) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.Active) %>
                <%= Html.ValidationMessageFor(model => model.Active) %>
            </div>
            
            <div class="editor-label">
                <%= Html.LabelFor(model => model.Sequence) %>
            </div>
            <div class="editor-field">
                <%= Html.TextBoxFor(model => model.Sequence) %>
                <%= Html.ValidationMessageFor(model => model.Sequence) %>
            </div>
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%= Html.ActionLink("Back to List", "Index") %>
    </div>


</center>