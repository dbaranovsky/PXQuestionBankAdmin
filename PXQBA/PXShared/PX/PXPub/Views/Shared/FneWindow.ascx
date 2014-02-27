<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%@ Import Namespace="System.IO"%>
<%  var courseType = (ViewData["courseType"] == null) ? "" : ViewData["courseType"].ToString();
    bool fullScreen = (courseType == CourseType.FACEPLATE.ToString().ToLowerInvariant() || 
          courseType == CourseType.XBOOK.ToString().ToLowerInvariant() || 
          courseType == CourseType.XBookV2.ToString().ToLowerInvariant());
    string fneClass = fullScreen ? "fullScreen" : "basic";
    if (Model != null && Model.ContentIsBeingEdited())
    {
        fneClass += " fne-edit";
    }
    %>
    <div id="fne-window" class="fne-window-basic <%=fneClass %>" style="display:none;">
        <div id="fne-header">
        <% if (fullScreen)//TODO: instead have a flag on the course specifying the FNE window to use
            {
                Html.RenderPartial("FneHeaderFullScreen",Model);
                //don't include navigation inside content item, it's included in the header
                if(Model != null)
                    Model.IncludeNavigation = false;
               
            }
           else
           {
               Html.RenderPartial("FneHeaderSmall", Model);
           }
            %>
            <div class="clear-float"></div>

        </div>
        <div id="fne-content">
        <% if (Model != null && Model.Content != null)
           {
               if (Model.Content.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student && Model.Content.HiddenFromStudents)
               {
                   Html.RenderPartial("RestrictedItem", Model);
               }
               else
               {
                   var context = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Bfw.PX.Biz.ServiceContracts.IBusinessContext>(); 
                   ViewData["EnrollmentId"] = context.EnrollmentId;                 
                   Html.RenderPartial("DisplayItem", Model);
               }
           }
           else
           {
               Html.RenderPartial("DisplayItemNone", Model);
           }
            %>

        </div>
        <div id="fne-footer"></div>
    </div>
    <%if (!fullScreen)
      {%>
    <div id="fne-minimized" style="display:none;">
    <table>
    <tbody>
        <tr>
            <td id="fne-minimized-title" style="cursor:pointer"></td>
            <td id="fne-minimized-close" ></td>
        </tr>
        </tbody>
    </table>
  
    <div id="fne-minimized-content" style="display:none;"></div>
    </div>

    <div id="fne-search-minimized" style="display:none;">
    <span id="fne-minimized-title">Search Results</span>
    <a href="#" id="fne-minimized-close"></a>
    <input type="hidden" id="MinExcludeWords" />
    <input type="hidden" id="MinExactPhrase"/>
    <input type="hidden" id="MinIncludeWords"/>
    <input type="hidden" id="MinMetaCategories"/>    
    <input type="hidden" id="MinContentTypes"/>    
    </div>
    <% } %>
    
      <% if (fullScreen && Model != null)
         { %>
         <script type="text/javascript" language="javascript">
            (function($) {
                PxPage.OnReady(function() {       
                    if (typeof HTMLParser === "undefined") {
                        var deps = <%= ResourceEngine.JsonFor("~/Scripts/highlight.js") %>;
                        var allDeps = <%= ResourceEngine.JsonFor("~/Scripts/highlight.js") %>;
                        PxPage.Require(deps, null, allDeps);
                    }
                });
            }(jQuery));
        </script>
        <% } %>