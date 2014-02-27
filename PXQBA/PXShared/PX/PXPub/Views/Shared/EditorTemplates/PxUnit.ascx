<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.PxUnit>" %>
<script type="text/javascript" language="javascript">
    PxPage.Require(['<%= Url.ContentCache("~/Scripts/Module/PxUnit.js") %>']);
</script>
<%var cancel = "return PxPage.CloseCreateNewScreen({ reason: 'cancel', id: '" + Model.Id + "' })";
    var OnSuccess = "ContentWidget.ContentCreated";
    var OnSuccessSaveAndOpen = "ContentWidget.ContentCreatedAndOpen";

    var title = (Model.Status == ContentStatus.New) ? "Create New Course Unit" : Model.Title;
    using (Html.BeginForm("SaveLesson", "PxUnit", FormMethod.Post, new { id = "saveItem" }))
    { %>  
        <div id="form" class="doc-collection faceplatecopy" style="float: left">
            <%= Html.Hidden("Id", Model.Id) %>
            <%= Html.Hidden("ParentId", Model.ParentId) %>
            <%= Html.Hidden("Type", Model.Type) %>
            <%= Html.Hidden("Url", Model.Url) %>
            <%= Html.Hidden("IsAssignable", Model.IsAssignable) %>
            <% Html.RenderPartial("HiddenCategoryList", Model.Categories); %>
            <%= Html.HiddenFor(m => m.DefaultCategoryParentId) %>

            <%= Html.Hidden("SyllabusFilter", Model.SyllabusFilter)%>
            <input type="hidden" id="assignToCourse" />
            <input type="hidden" id="targetMode" value="normal" />
            <%=Html.HiddenFor(m => m.SourceTemplateId) %>
            <%=Html.HiddenFor(m => m.IsBeingEdited) %>

            <%= Html.Hidden("Thumbnail", Model.Thumbnail)%>
          
            <div class="create-new-wrapper">
            <ol>

                <% Html.RenderPartial("ItemEditorTemplate", Model); %>
                <li>
                   <%= Html.Label("Description") %>
                    <%= Html.TextArea("Description", Model.Description, new { @class = "html-editor", @id = String.Format("Body_{0}", Model.Id), style = "visibility:hidden;width:auto;" })%>                    
                </li>                       
            </ol>
            </div>
            <div class="create-new-btn-wrapper">
                    <input type="hidden" id="DueDate" name="DueDate" value="<%=Model.DueDate %>"/>    
                    <label>&nbsp;</label>
                    <%if (CourseType.FACEPLATE.ToString().ToLowerInvariant() != Model.CourseInfo.CourseType.ToString().ToLowerInvariant())
                      { %>
                    <input type="button" name="behavior" value="Save & Open" class="savebtn submit-action" onclick="PXUnit.OnClickSaveandOpen(<%= OnSuccessSaveAndOpen %>)" />

                    <%} %>
                    <input type="button" name="behavior" value="Save" class="savebtn submit-action" onclick="PXUnit.OnClickSave(<%= OnSuccess %>)" />
                 <a href="#" class="create-closecancel" onclick="<%= cancel %>">Cancel</a>
            </div>
        </div>
        <div class="faceplatecopy" style="float: left; padding: 10px;">  
                <%
               if (Model.Type.ToLower() == "pxunit")
               {
                   if (string.IsNullOrEmpty(((PxUnit)Model).Thumbnail))
                   {
                       ((PxUnit)Model).Thumbnail = Url.Action("Index", "Style", new { path = ConfigurationManager.AppSettings.Get("DefaultImage") });
                   }

                   if (HttpContext.Current.Request.Url.Host == "localhost" && ((PxUnit)Model).Thumbnail.Contains("brainhoney/resource"))
                   {
                       ((PxUnit)Model).Thumbnail = String.Format("{0}{1}", "http://dev.worthpublishers.com", ((PxUnit)Model).Thumbnail);
                   }                           
                %>
                <img class="fpimageLarge unitImage" src="<%= ((PxUnit)Model).Thumbnail %>" style="min-width:125px;max-width: 200px;min-height:100px;max-height:150px;" />
                <br />
                <a href='#' class="link unitImage">Click to Edit</a>                    
                <%
                }
                %>        
        </div>
        <div style="clear:both"></div>
<% } %>

<script type="text/javascript" language="javascript">
    (function ($) {
        PxPage.OnReady(function () {
            $('#saveItem').css('width', 'auto');

            $('.unitImage').bind('click', function () {
                tinyMCE.activeEditor.IsExternalCall = true;
                tinyMCE.activeEditor.ExternalPlaceholder = $('.unitImage');
                $(tinyMCE.activeEditor.buttons.image).click();
            });

        });
    } (jQuery));
</script>