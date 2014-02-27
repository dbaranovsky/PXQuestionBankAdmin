<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Widget>" %>
<%
//var settings = ViewData["settings"] == null ? new Hashtable() : (Hashtable)ViewData["settings"];

WidgetCallback callback = null;
if (this.Model.Callbacks.ContainsKey("View"))
{
    callback = this.Model.Callbacks["View"];
}

if (callback == null)
{
    return;
}

bool isEditable = false;
if (this.Model.Callbacks.ContainsKey("OnBeforeAdd") || this.Model.Callbacks.ContainsKey("OnBeforeEdit"))
{
    isEditable = true;
}
//isEditable = false;

var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
var inputHelpers = jsSerializer.Serialize(Model.InputHelpers);
  
%>
<% var courseType = ViewData["CourseType"] != null ?  
       ViewData["CourseType"].ToString() : ""; %>

<div class="widgetItem <%=Model.BfwSubType %>" silentCreation="true" id='<%=Model.Id %>'  itemId='<%=Model.Id %>' sequence = '<%=Model.Sequence %>' templateId ="<%= Model.Template %>" isMultipleAllowed="<%= Model.IsMultipleAllowed.ToString() %>" isEditable="<%=isEditable.ToString() %>">
    
    <!-- Display all the persistent Qtips associated with the widget item -->
      <% if (Model.IsShowPersistentQtip)
         {
             foreach (var tooltipId in Model.PersistentQtips)
             { %>
                   <% Html.RenderAction("ShowPersistentQtip", "PersistingQtips", new { tooltipId = tooltipId }); %>
            <% }
         } %>
    
    <div class = "widgetToolbar">
        <span class="grip-icon"></span>
        <a style="" rel="<%=Model.Id.ToLowerInvariant()%>" rev="<%=Model.ParentId%>" class="closeWidgetButton" href="#"></a>
    </div>

    <%-- widget helper selectors--%>
    <input type="hidden" name="itemId" id="itemId" value="c447d3a98ebf4d69b60840a98adbbe8e"/>

    <div class="widgetContents">
    <div class="blockWidgetUI widgetBlocker_OFF"></div>
    <div class="widgetBlocker widgetBlocker_OFF"><div class="widgetBlockMessage">Widget cannot be edited <div class="cannot-edit-lock"></div></div></div>
    <div class="widgetHeader" style='<%= this.Model.IsTitleHidden ? "display:none":"" %>'> 
        <div class="widgetHeaderText">
                <%=Model.Title%> 
        </div>
        <div class="widgetHeaderCollapse">
            <% if (Model.IsCollapseAllowed) 
                {
            %>
                <a style="" rel="<%=Model.Id.ToLowerInvariant()%>" rev="<%=Model.ParentId%>" class="widgetToggleDisplay widgetCollapse"  href="#"></a>
            <% } %>
        </div>
    </div>
    <div id="widgetBody" class="widgetBody <%=Model.Id%>">   
        <%  if (callback.IsASync) // ajax
            { %>
               <script type="text/javascript">
                    PxPage.OnReady(function () {
                        var inputHelpers = <%= inputHelpers %>;
                        var defaultCollection = { Id: "<%=Model.Id%>" };
                        var helperCollection = PxWidgetHelper.WidgetInputHelper(inputHelpers, '<%=Model.Id%>');
                        var combinedCollection = { };
                        $.extend(combinedCollection, defaultCollection, helperCollection);

                        var postData = JSON.stringify(combinedCollection);

                        PxPage.Loading('<%=Model.Id%>');
                        $.ajax({
                            url: '<%= Url.ActionCache(callback.Action, callback.Controller) %>',
                            type: 'POST',
                            data: combinedCollection,
                            success: function (data) {
                                $('.<%=Model.Id%>').html(data);
                                PxPage.Loaded('<%=Model.Id%>');
                                $(PxPage.switchboard).trigger("", ['<%=Model.Id%>']);
                                $(PxPage.switchboard).trigger("ASYNC_WIDGET_LOADED", ["SUCCESS", "", '<%=Model.Id%>']);
                            },
                            error: function (req, status, error) {
                                PxPage.log('ERROR_PIN_ANNOUNCEMENT');
                                PxPage.Loaded('<%=Model.Id%>');
                                $(PxPage.switchboard).trigger("ASYNC_WIDGET_LOADED", ["FAIL", "Failed", '<%=Model.Id%>']);
                            }
                        });
                    });
               </script>
               <%
            }
            else
            {
                try
                {
                    Html.RenderAction(callback.Action, callback.Controller, Model);
                }
                catch (HttpException ex)
                {
                    //This should at least let us know the name of the missing controller
                    throw new Exception("Controller: " + callback.Controller + " is missing", ex);
                }
            } %>
        </div>
    </div>
</div>