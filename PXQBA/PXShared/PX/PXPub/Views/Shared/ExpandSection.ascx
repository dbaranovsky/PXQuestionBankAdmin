<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<Bfw.PX.PXPub.Models.TocItem>>" %>

<% if (!Model.IsNullOrEmpty())
   {
       try
       {
           if (ViewData["category"] != null && ViewData["category"].ToString() == System.Configuration.ConfigurationManager.AppSettings["MyMaterials"] && ViewData["HasUserMaterials"] != null && ViewData["HasUserMaterials"].ToString() == "False")
           {%>
           <span>There are no materials</span>
      <% }
           else
           {
               foreach (var section in Model)
               {
                   var title = string.IsNullOrEmpty(HttpUtility.HtmlDecode(section.Title)) ? "No Title" : HttpUtility.HtmlDecode(section.Title);
                   var childContainerId = string.Format("children_of_{0}", section.Id);
                   var childStyle = section.Children.IsNullOrEmpty() ? "display: none;" : "";
                   var expandClass = section.IsActive ? "expand active" : "expand";
                   
                   //expandClass = section.IsHiddenFromStudents ? string.Format("{0} hidden", expandClass) : expandClass;
                   var sectionClass = section.IsActive && section.ShowControls ? "section-controls active allowed" : (section.ShowControls ? "section-controls allowed" : "section-controls");
                   var incToc = ViewData.ContainsKey("includeToc") ? (bool)ViewData["includeToc"] : false;
                   var incDisc = ViewData.ContainsKey("includeDiscussion") ? (bool)ViewData["includeDiscussion"] : false;
                   var delUrl = Url.Action("DeleteItem", new { id = section.Id, parentId = section.ParentId });
                   var hideUrl = Url.Action("HideItem", new { id = section.Id, parentId = section.ParentId });
                   var showUrl = Url.Action("ShowItem", new { id = section.Id, parentId = section.ParentId });
                   var viewUrl = Url.Action("DisplayItem", "ContentWidget", new { id = section.Id, mode = ContentViewMode.Preview, includeToc = incToc, includeDiscussion = incDisc, courseid = ViewContext.RouteData.Values["courseid"], category = ViewData["category"], isStudentUpdated = isStudentUpdated });
                   var controlsStyle = section.ShowControls ? "display: none;" : "display: none;";
                   var sectionExpanded = section.Children.IsNullOrEmpty() ? "section jstree-closed" : "section jstree-open";
                   sectionExpanded = section.IsActive ? string.Format("{0} active", sectionExpanded) : sectionExpanded;
                   sectionExpanded = section.IsHiddenFromStudents ? string.Format("{0} hidden", sectionExpanded) : sectionExpanded;
                   sectionExpanded = section.IsAssigned ? string.Format("{0} assigned", sectionExpanded) : sectionExpanded;
                   var sectionType = string.Format("ci{0}", section.ItemType.ToLower());
                                      
                   var folderType = section.IsStudentCreated ? "student-created" : "";
                   
                   if (section.ItemType.Equals("reflectionassignment", StringComparison.InvariantCultureIgnoreCase))
                   {
                       if (section.IsAssigned)
                       {
                           sectionExpanded = "section assigned jstree-leaf ";
                       }
                       else
                       {
                           sectionExpanded = "section jstree-leaf ";
                       }
                   }

                   if (section.ItemType.ToLowerInvariant() == "quiz")
                   {
                       sectionExpanded = string.Format("{0} jstree-leaf", sectionExpanded);                        
                   }

                    sectionExpanded = string.Format("{0} unone", sectionExpanded);
                    expandClass = string.Format("{0} unone", expandClass);
                     
                      
           %>
           
           <%  var isPublicView = (ViewData["IsPublicView"] == null) ? false : Convert.ToBoolean(ViewData["IsPublicView"]);        %>
           <%  var isReadOnly = ViewData["IsReadOnly"] != null ? Convert.ToBoolean(ViewData["IsReadOnly"]) : false; %>

           <%
                   if (isPublicView)
                   {
                       sectionType = string.Format("{0}{1}", sectionType, "publicview");
                   }
           %>

            <li id="<%= section.Id %>" class="<%= sectionExpanded + " " + folderType %>" rel="<%= sectionType %>">
                <%
                   var tocItemUrl = Url.Action("Index", "Content",
                                                   new {
                                                           id = section.Id,
                                                           mode = ContentViewMode.ReadOnly,
                                                           includeToc = false,
                                                           includeDiscussion = false
                                       }); %>
                <input type="hidden" id="tocItemId" value='<%=section.Id%>' />
                <input type="hidden" id="tocItemUrl" value='<%= tocItemUrl %>' />
                <span class="tooltip"></span>
                <input type="hidden" id="isLocked" value="<%=section.IsLocked%>" class="isLocked" />
                <input type="hidden" id="isReadOnlyItem" value="<%=isReadOnly%>" class="isReadOnlyItem" />
                <%--<% Html.RenderPartial("Tooltip", section); %>--%>
                
                <span style="display: none;" class="section-sequence"><%= section.Sequence%></span>
                <% var sectionUrl = Url.Action("ExpandSection", "ContentWidget", new { id = section.Id, includeToc = incToc, includeDiscussion = incDisc, courseid = ViewContext.RouteData.Values["courseid"], category = ViewData["category"], isStudentUpdated = isStudentUpdated, IsEportfolioBrowser = ViewData["IsEportfolioBrowser"], IsPresentationCourse = ViewData["IsPresentationCourse"], UserAccess = ViewData["UserAccess"] }); %>
                <a class="<%= expandClass %>" href="<%= sectionUrl %>"><span><%= title%></span></a>
                
                <div class="<%= sectionClass %>" style="<%= controlsStyle %>">
                    <span href="<%= viewUrl %>" class="display">View</span> |
                    <% if (section.ShowControls)
                       { %>
                       
                        
                              <span href="<%= showUrl %>" class="show">Show</span>
                        |
                              <span href="<%= hideUrl %>" class="hide">Hide</span>
                        
                         | 
                              <span href="<%= delUrl %>" class="delete">Delete</span>
                    <% } %>                    
                </div>
                <% if (!section.Children.IsNullOrEmpty())
                   { %>
                <ul id="<%= childContainerId %>" class="children">
                    <% var vd = new ViewDataDictionary<IEnumerable<TocItem>>();
                       vd.Model = section.Children;
                       vd["includeToc"] = incToc;
                       vd["includeDiscussion"] = incDisc;
                       vd["category"] = ViewData["category"];
                       vd["HasUserMaterials"] = ViewData["HasUserMaterials"];
                       vd["IsPublicView"] = ViewData["IsPublicView"];
                       vd["IsReadOnly"] = isReadOnly;
                       vd["IsEportfolioBrowser"] = ViewData["IsEportfolioBrowser"];
                       vd["IsPresentationCourse"] = ViewData["IsPresentationCourse"];
                       vd["UserAccess"] = ViewData["UserAccess"];
                       Html.RenderPartial("ExpandSection", vd); %>
                </ul>
                <% } %>
            </li>
    <% }
       }
       }
       catch { }
   }%>