
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.CustomWidget>" %>

<% 
    using (Html.BeginForm("AddCustomWidget", "CustomWidget", FormMethod.Post, new { id = "saveItem" }))
   { %>
               <input type="hidden" name="Id" id="Id" value = <%= ViewData["WidgetId"] %> />
               <input type="hidden" name="Mode" id="Mode" value = <%= ViewData["Mode"] %> />

<div class="customEditWidget"  dialogWidth="645" dialogHeight="450" dialogMinWidth="800" dialogMinHeight="450" dialogTitle="Custom Widget">
    <div class="feedURLContainer">
        <ol class="custom-widget-labels">
                <li>
                    <div class="CustomWidgetLabel">Widget Name:</div>
                    <div class="widgetName CustomWidgetText">
                        <%= Html.TextBoxFor(m => m.Title, new { @class = "title", size = 97 })%>
                        <%= Html.ValidationMessage("Title")%>
                        <%= Html.ValidationMessage("content.Title")%>
                    </div>
                </li>


                <li>
                    <div class="CustomWidgetLabel">Contents:</div>
                    <div class="widgetContent CustomWidgetText">
                        <%= Html.TextAreaFor(m => m.WidgetContents, new { @class = "html-editor", @id = string.Format("Body_{0}", new Random().Next()), style = "visibility:hidden;width:auto;" })%>                    
                    </div>
                </li>
                </ol>
     </div>


     <%--<button onclick="PxCustomWidget.SubmitFromButton();">Save</button>
--%>
</div>
<% } %>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/CustomWidget/Custom.js") %>'], function () {
                PxCustomWidget.BindControls();
            });
        });

    } (jQuery));    
</script>
