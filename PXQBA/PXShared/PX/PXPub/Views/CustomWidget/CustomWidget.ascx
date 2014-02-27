<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.HtmlDocument>" %>
<%   
var cancel = "PxPage.CancelForm()";
var OnSuccess = "";

if (Model != null && ViewData [ "isEdit"] == null)
{
    %>
    <div><%=Model.Body%></div>       
    <%
} else {
    using (Ajax.BeginForm("SaveCustomWidget", "Navigation", new AjaxOptions() { OnSuccess = "function() {window.location='?mode=unlock';}"} ))
    { %>
    <div id="form" class="customwidget">            
        <%= Html.HiddenFor(m => m.Id)%>
        <%= Html.HiddenFor(m => m.ParentId)%>
        <%= Html.HiddenFor(m => m.Url)%>
        <%= Html.HiddenFor(m => m.IsAssignable)%>
        <%= Html.HiddenFor(m => m.Sequence)%>      
        
        <% if (!ViewData.ModelState.IsValid)
           { %>
            <input type="hidden" id="content_save_failed" name="content_save_failed" value="1" />
        <% } %>
        <ol>
            <li>
                <%= Html.LabelFor(m => m.Title)%>
                <%= Html.TextBoxFor(m => m.Title, new { @class = "title required" })%>
                <%= Html.ValidationMessage("Title")%>
                <%= Html.ValidationMessage("content.Title")%>
            </li>
            <li>
                <%= Html.LabelFor(m => m.Body)%>
                <%=Html.TextAreaFor(m => m.Body,
                new {
                   @id = string.Format("Body_{0}", Model.Id),
                   @class = "html-editor",
                   style = "overflow-y:scroll;"
                })%>
            </li>
            <li>
                <label>&nbsp;</label> 
 
                <input type="submit" name="behavior" value="Save" onclick="return SaveCustomWidget();"/>

                <input type="button" value="Cancel" onclick="$('.divPopupClose').click();" />
            </li>
        </ol>
    </div>
<% }
}%>