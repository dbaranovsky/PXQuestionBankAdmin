<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>
<%@ Import Namespace="System.Xml.Linq" %>
<%@ Import Namespace="System.Dynamic" %>
<%@ Import Namespace="System.Web.Routing" %>
			

 <%
     using (Ajax.BeginForm("MetaDataIndex", "AdminMetaData", new AjaxOptions { HttpMethod = "Post", OnComplete = "", OnSuccess = "MetaDataSuccess", OnBegin = "" }, new { @id = "frmAdminMetaData" }))
  { 
 %> 
    <div id="metadata-container" >
		<div>
            <table width="100%">
            <%
      if (Model.AdminMetaData != null)
      {
          foreach (var metaElement in Model.AdminMetaData.MetaData.Elements)
          { %>	 										    
            <tr >
                <td class="title">
                    <%= metaElement.Description%>
                </td>
            <td class="metacontrol">
													
            <%									    
Html.RenderAction(metaElement.Action, metaElement.Controller, new RouteValueDictionary { { "contentItem", Model }, { "metaDataElement", metaElement } });																				
            %>
            </td>
				
            </tr> 
            <%
          }
      }
							    						
            %>
            </table>	
		</div>
  
        <div class="form-footer">
			<input type="submit" id="btnSave" class="savebtn submit-action" value="Save" />                      			
	   </div>
</div>

 

<%
 }
%>
<script type="text/javascript">
    
   	function MetaDataSuccess() {
   	    alert('Changes Saved.');
   	    PxPage.Loaded("#content-item");
        if(ContentWidget != undefined && ContentWidget != null)
       	    ContentWidget.NavigateAway();
    }
</script>