<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<Bfw.PX.PXPub.Models.TocItem>>" %>

     <% if (!Model.IsNullOrEmpty())
       {
           var index = 0;
           foreach (var section in Model)
           {
               var oddOrEven = ((index % 2) == 0);
               var childContainerId = string.Format("assign_children_of_{0}", section.Id);
               var isFolder = section.ItemType.ToLowerInvariant() == "folder";
               var isUnit = section.ItemType.ToLowerInvariant() == "pxunit";
               var expandAction = "ExpandContentPicker";               
                %>
                    <div class="level row<%=oddOrEven%>">
                <% if (!isFolder)
                   { %>                        
                        <span class="tocUnAssign" style="display:none;position:absolute;left:-25px;"><a href="#" onclick="return AssignmentCenterHost.contentRemoved(event,'<%=section.Id %>');" class="lnkAssign unassignitem">Unassign</a></span>
                        <span class="tocAssign" style="display:none;position:absolute;left:-25px;"><a href="#" onclick="return AssignmentCenterHost.addExistingContent(event, '<%=section.Id %>');" class="lnkAssign assignitem">Assign</a></span>
                        <% } %>
                        <input type="hidden" class="IsPartOfAssignmentCenter" id="IsPartOfAssignmentCenter" value="<%=section.IsPartOfAssignmentCenter%>" />

                        <span class="expandContainer <% if(isFolder){%>folder<%} %>" id="<%= section.Id %>">
                        <%
               
               if (isUnit)
               {
                   expandAction = "ExpandContentPickerUnit";
               }
                             %>

                            <%= Ajax.ActionLink(HttpUtility.HtmlDecode(section.Title), expandAction, "PageAction",
                                                                        new { id = section.Id },
                                                            new AjaxOptions
                                                            {
                                                                OnBegin="PxPage.Loading();",
                                                                HttpMethod = "POST",
                                                                OnSuccess = "PxPage.Loaded();jQuery.fn.acHelper.setSelectedNodeId('" + section.Id + "', '" + section.Title + "');jQuery.fn.acHelper.toggleSection();",
                                                                InsertionMode = InsertionMode.Replace,
                                                                UpdateTargetId = childContainerId
                                                            },
                                                               new { @class = "expand", style = section.IsPartOfAssignmentCenter ? "color:#D0D3CC" : "color:#333333" })%>
                                
                                <% if(!isFolder){ %>
                                <span class="tocAssignRight" style="display:none;">
                                    <a href="#" onclick="return jQuery(PxPage.switchboard).trigger('existingcontentadded', ['<%=section.Id %>', 'PX_ASSIGNMENT_CENTER_SYLLABUS_INSTRUCTOR_WORKSPACE']);" class="lnkAIW">Add to Workspace</a> 
                                    <%= Ajax.ActionLink("Preview", "DisplayItem", "ContentWidget", new { id = section.Id, mode = ContentViewMode.Preview }, new AjaxOptions() { }, new { @class = "fne-link lnkPreview" })%>
                                </span>                               
                                <% } %>

                                <%                                 
                                if (!section.Children.IsNullOrEmpty())
                                   { %>
                                    <% var vd = new ViewDataDictionary<IEnumerable<TocItem>>();
                                       vd.Model = section.Children;
                                       vd["includeToc"] = "";
                                       vd["includeDiscussion"] = "";
                                       vd["category"] = ViewData["category"];
                                       vd["HasUserMaterials"] = ViewData["HasUserMaterials"];
                                       Html.RenderPartial("ContentPicker", vd); %>
                                <% }%>
                        </span>
                        <span id="<%= childContainerId %>" class="children" style="display: none;"></span>
                    </div>
            <%
               index++;
                } %>
    <%}
       else  { %>

       <div class="level expandContainer">
            <span style="padding-left:18px;" class="Description"><%= (ViewData["Description"] == null || string.IsNullOrEmpty(ViewData["Description"].ToString())) ? "No description available" : ViewData["Description"].ToString()%></span>
        </div>
      <% } %>


