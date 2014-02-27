<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<% 
    var viewType = (ViewData["ViewType"] == null) ? "" : ViewData["ViewType"].ToString().ToLower();
    var fromDashboard = (ViewData["fromDashboard"] == null) ? false : Convert.ToBoolean(ViewData["fromDashboard"]);
%>

<div class="settingNav-links" style="float: left; width: 10%">        
    <ul>
        <li>
        <%
            if (ViewData["View"].ToString() == "General")
            {
        %>
            <%= ViewData["View"].ToString() %>
        <%
            }
            else 
            { 
        %>
            <a href="#state/instructorconsole/general" class="link">General</a>
        <%
            }
        %>
        </li><br />

        <% if (!fromDashboard) { %>
        <li>
        <%
            if (ViewData["View"].ToString() == "Navigation")
            {
        %>
            <%= ViewData["View"].ToString() %>
        <%
            }
            else 
            { 
        %>
            <a href="#state/instructorconsole/navigation" class="link">Navigation</a>
        <%
            }
        %>
        </li><br />
        <li>
        <%
            if (ViewData["View"].ToString() == "Launchpad")
            {
        %>
            <%= new MvcHtmlString("Launch Pad")%>
        <%
            }
            else 
            { 
        %>
            <a href="#state/instructorconsole/launchpad" class="link">Launch Pad</a>
        <%
            }
        %>
        </li><br />
        <% } %>

    </ul>
</div>

<div id="ConsoleRegion" class="instructor-console-settings" style="float: right; width: 80%">
    <div class="Title">
        <%= ViewData["View"].ToString() %>
    </div>
        
    <% 
        switch (ViewData["View"].ToString().Replace(" ", string.Empty))
        {
            case "General":
    %>
  
                <% Html.RenderPartial(String.Format("CourseForm", (Bfw.PX.PXPub.Models.Course)Model)); %>
                 <div class="settingsSubmit-wrapper">
                    <input type="submit" value="Save" id="submitForm" class="primary" />
                    <a href='<%= !fromDashboard ? "#state/instructorconsole" : "javascript:" %>' onclick="<%= fromDashboard ? "return PxPage.LargeFNE.CloseFNE()" : ""  %>" class="link">Cancel</a>
                </div>
    
    <%
    
                break;
                
            case "LaunchPad": %>
       
            <% Html.RenderPartial("LaunchPad", (Bfw.PX.PXPub.Models.Course)Model); %>


                    <%
               break;
               default:
    %>
    
                <% Html.RenderPartial(String.Format(ViewData["View"].ToString().Replace(" ", string.Empty), (Bfw.PX.PXPub.Models.Course)Model)); %>
                <div class="settingsSubmit-wrapper">
                    <input type="submit" value="Save" id="submitForm" class="primary" />
    <%= Ajax.ActionLink("Cancel", "View", new AjaxOptions() { OnBegin = "PxPage.Loading('fne-content');", OnSuccess = "PxPage.Loaded('fne-content');", UpdateTargetId = "instructor-console-wrapper" })%>
    </div>
    <%      
                    break;
        }
    %>
    


</div>
