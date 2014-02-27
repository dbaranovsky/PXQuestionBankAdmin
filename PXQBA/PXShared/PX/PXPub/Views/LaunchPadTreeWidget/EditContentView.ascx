<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>


<% 
    var titleAdd = (ViewData["mode"] == null ? "" : ViewData["mode"].ToString());
    
    titleAdd = (titleAdd == "copy" ? " copy" : "");
    Model.Title += titleAdd;
    
    var sourceWindow = ViewData["sourceWindow"] == null ? "" : ViewData["sourceWindow"].ToString();
    var action = (ViewData["mode"] != null && ViewData["mode"].ToString() == "rename") ? "EditContentSave" : "CopyContent";
    var controller = (ViewData["mode"] != null && ViewData["mode"].ToString().ToLower() == "copy") ? "FacePlate" : "LaunchpadTreeWidget";   
    var OnSuccess = (action == "EditContentSave") ? "$.fn.ContentTreeWidgetObj()._static.fn.updateItemAfterRename" : "PxFacePlate.UpdateItemAfterCopyAndMove";
    var OnSuccessSaveAndOpen = (action == "EditContentSave") ? "" : "PxFacePlate.UpdateItemAfterCopyAndMoveAndOpen";
   
    bool isEbook = false;
    
    if (Model.Type.ToLower().Equals("externalcontent"))
    {
        isEbook = true;
    } 
%>


<% using (Html.BeginForm(action, controller, FormMethod.Post, new { id = "saveItem" }))
   {        
	   %>
		<div id="form" class="faceplatecopy">  
            <input type="hidden" value="<%=ViewData["newParentId"]%>" id="newParentId" name="newParentId" />

            <%= Html.Hidden("EnrollmentId", ViewData["courseId"], new { @class = "resourceEntityId" })%>
			<%= Html.HiddenFor(m => m.Id) %>
			<%= Html.HiddenFor(m => m.ParentId) %>
			<input type="hidden" name="Level" value="<%=ViewData["level"]%>" />
			<ol class="formlist">
				 <% Html.RenderPartial("ItemEditorTemplate", Model, ViewData); %>
				<% if (!isEbook)
       {%>
				<li class="directions">
				   <b><%= Html.Label("Directions")%></b>
				   <%= Html.TextAreaFor(m => m.Description, new { @class = "html-editor random", @id = string.Format("Body_{0}", Model.Id), style = "visibility:hidden;width:auto;" })%>
				</li>
                <% } %>
                <input type="hidden" value="<%=Model.Title %>" id="Hidden1" name="title" />
                <input type="hidden" value="<%=Model.Id %>" id="Hidden2" name="itemId" />
                <!-- input type="hidden" value="<%=Model.Description %>" id="Hidden3" name="description" /-->
                <input type="hidden" value="<%=Model.Thumbnail %>" id="thumbnail" name="thumbnail" />
                <input type="hidden" value="<%=Model.SubTitle %>" id="Hidden4" name="subtitle" />
                <input type="hidden" value="<%=sourceWindow %>" id="Hidden6" name="window" />

				<li class="formbtns editcontent">
                    <input class="savebtn button primary large" type="button" name="behavior" value="Save" onclick="EditUnitInfo.UpdateThumbnail(); if(PxPage.ValidateTitle()){ PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }<%= Model.IsContentCreateAssign ? ", externalData:ContentWidget.CreateAndAssign" : "" %>, updateSelector: '#content-item', success: <%= OnSuccess %>}, ContentWidget.NavigateAway)}" />
                                       
                   <% if (OnSuccessSaveAndOpen != "" && sourceWindow != "browsemoredialog")
                      { %>
                    <input class="savebtn button primary large" type="button" name="behavior" value="Save & Open" onclick="EditUnitInfo.UpdateThumbnail(); if(PxPage.ValidateTitle()){ PxPage.OnFormSubmit('Processing...', true, { form: '#saveItem', data: { behavior: 'Save' }<%= Model.IsContentCreateAssign ? ", externalData:ContentWidget.CreateAndAssign" : "" %>, updateSelector: '#content-item', success: <%= OnSuccessSaveAndOpen %>})}" />
                    <%} %>
         		<a href="#" class="create-closecancel" onclick="PxPage.CloseFne()">Cancel</a>					
				</li>
			</ol>
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

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/EditUnitInfo/EditUnitInfo.js") %>'], function () {
                EditUnitInfo.Init();
            });
        });
    } (jQuery));    
</script>