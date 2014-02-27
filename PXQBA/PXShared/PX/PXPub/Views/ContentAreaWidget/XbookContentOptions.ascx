<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.XbookContentOptions>" %>


    <div class="menu content-menu-tabs">
        <ul id="content-modes" class="link-list">
            <%
                if (Model.IsOptionAvailable(ContentViewMode.Preview) || Model.IsOptionAvailable(ContentViewMode.ReadOnly))
                {
                    var previewUrl = Url.GetComponentHash("item", Model.ItemId, new {
                            mode = ContentViewMode.Preview,
                            includeNavigation = true,
                            isBeingEdited = false,
                            renderFNE = true
                        });
                %>
                <li class="view-tab <%= (ContentViewMode.Preview == Model.ActiveOption) ? "active" : "" %> ">
                    <a href="<%= previewUrl %>" class="requireConfirmation" id="view">Basic Info</a>
                    <div class="assign-arrow">
                    </div>
                </li>
            <%  } %>
            <!-- Edit -->
            <%  if (Model.IsOptionAvailable(ContentViewMode.Edit))
                {
                    var editUrl = Url.GetComponentHash("item", Model.ItemId, new {
                            mode = ContentViewMode.Edit,
                            includeNavigation = true,
                            isBeingEdited = true,
                            renderFNE = true
                        }); 
                %>
                <li class="questions-tab <%= (ContentViewMode.Edit == Model.ActiveOption) ? "active" : "" %>">
                    <a href="<%= editUrl %>" class="requireConfirmation" id="requireConfirmationBasicInfo">Edit</a>
                    <div class="assign-arrow">
                    </div>
                </li>
                    
            <%      
                } %>
            <!-- Assign -->
            <%  if (Model.IsOptionAvailable(ContentViewMode.Assign))
                { 
                    var assignUrl = Url.GetComponentHash("item", Model.ItemId, new {
                            mode = ContentViewMode.Assign,
                            includeNavigation = true,
                            isBeingEdited = false,
                            renderFNE = true
                        }); 
                %>
                <li <%= (ContentViewMode.Assign == Model.ActiveOption)
                            ? "class=\"assign-tab active\""
                            : "class=\"assign-tab\"" %>>
                    <a href="<%= assignUrl %>" class="assignTabGroupLink requireConfirmation" id="requireConfirmationAssign">Assignment</a>
                    <div class="assign-arrow">
                    </div>
                </li>
            <%  } %>
            <!-- settings -->
            <%  if (Model.IsOptionAvailable(ContentViewMode.Settings))
                { 
                    var settingsUrl = Url.GetComponentHash("item", Model.ItemId, new {
                            mode = ContentViewMode.Settings,
                            includeNavigation = true,
                            isBeingEdited = false,
                            renderFNE = true
                        });    
                %>
                <li class="settings-tab <%= (ContentViewMode.Settings == Model.ActiveOption) ? "active" : "" %>">
                    <a href="<%= settingsUrl %>" class="settingsTabGroupLink requireConfirmation" id="requireConfirmationSettings">Settings</a>
                    <div class="assign-arrow">
                    </div>
                </li>
            <%  } %>
            <!-- questions -->
            <%  if (Model.IsOptionAvailable(ContentViewMode.Questions))
                { 
                    var questionUrl = Url.GetComponentHash("item", Model.ItemId, new {
                            mode = ContentViewMode.Questions,
                            includeNavigation = true,
                            isBeingEdited = false,
                            renderFNE = true
                        });    
                %>
                <li class="questions-tab <%= (ContentViewMode.Questions == Model.ActiveOption) ? "active" : "" %>">
                    <a href="<%= questionUrl %>" class="requireConfirmation" id="requireConfirmationQuestions">Questions</a>
                    <div class="assign-arrow">
                    </div>
                </li>
            <%  } %>
            <!-- rubrics -->
            <%  if (Model.IsOptionAvailable(ContentViewMode.Rubrics))
                {
                    var rubricUrl = Url.GetComponentHash("item", Model.ItemId, new {
                            mode = ContentViewMode.Rubrics,
                            includeNavigation = true,
                            isBeingEdited = false,
                            renderFNE = true
                        });
                %>
                <li <%= (ContentViewMode.Rubrics == Model.ActiveOption)
                            ? "class=\"rubric-tab active\""
                            : "class=\"rubric-tab\"" %>>
                    <a href="<%= rubricUrl %>" class="requireConfirmation" id="view">Rubric</a>
                    <div class="assign-arrow">
                    </div>
                </li>
                <%  if (Model.IsOptionAvailable(ContentViewMode.Results))
                    { 
                        var rUrl = Url.GetComponentHash("item", Model.ItemId, new {
                            mode = ContentViewMode.Results,
                            includeNavigation = true,
                            isBeingEdited = false,
                            renderFNE = true
                        });
                %>
                <li <%= (ContentViewMode.Results == Model.ActiveOption)
                            ? "class=\"results-tab active\""
                            : "class=\"results-tab\"" %>>
                    <a href="<%= rUrl %>" class="requireConfirmation" id="requireConfirmationResults">Results</a>
                    <div class="assign-arrow">
                    </div>
                </li>
                <%  } %>
            <%  }
                else
                {
                    if (Model.IsOptionAvailable(ContentViewMode.Results))
                    {
                        var resultsUrl = Url.GetComponentHash("item", Model.ItemId, new {
                            mode = ContentViewMode.Results,
                            includeNavigation = true,
                            isBeingEdited = false,
                            renderFNE = true
                        });
                %>
                <li <%= (ContentViewMode.Results == Model.ActiveOption)
                        ? "class=\"results-tab active\""
                        : "class=\"results-tab\"" %>>
                    <a href="<%= resultsUrl %>" class="requireConfirmation" id="requireConfirmationQuestionsResults">Results</a>
                    <div class="assign-arrow">
                    </div>
                </li>
            <%      }
                } %>
            <%
                if (Model.IsOptionAvailable(ContentViewMode.MoreResources)) 
                {
                    var mrUrl = Url.GetComponentHash("item", Model.ItemId, new {
                            mode = ContentViewMode.MoreResources,
                            includeNavigation = true,
                            isBeingEdited = false,
                            renderFNE = true
                    });
                %>
                <li <%= (ContentViewMode.MoreResources == Model.ActiveOption) ? "class=\"active\"" : "" %>>
                    <a href="<%= mrUrl %>" class="requireConfirmation" id="requireConfirmationMore">More Resources</a>
                    <div class="assign-arrow">
                    </div>
                </li>
            <%  } %>
            <%
                if (Model.IsOptionAvailable(ContentViewMode.MoreResourcesStatic)) 
                {
                    var mrsUrl = Url.GetComponentHash("item", Model.ItemId, new {
                            mode = ContentViewMode.MoreResourcesStatic,
                            includeNavigation = true,
                            isBeingEdited = false,
                            renderFNE = true
                    });
                %>
                <li <%= (ContentViewMode.MoreResourcesStatic == Model.ActiveOption) ? "class=\"active\"" : "" %>>
                    <a href="<%= mrsUrl %>" class="requireConfirmation" id="requireConfirmationBasicInfo">MoreResources</a>
                    <div class="assign-arrow">
                    </div>
                </li>
            <%  } %>
            <%  if (Model.IsOptionAvailable(ContentViewMode.Metadata))
                {
                    var metaUrl = Url.GetComponentHash("item", Model.ItemId, new {
                            mode = ContentViewMode.Metadata,
                            includeNavigation = true,
                            isBeingEdited = false,
                            renderFNE = true
                    });
                %>
                <li <%= (ContentViewMode.Metadata == Model.ActiveOption) ? "class=\"active\"" : "" %>>
                    <a href="<%= metaUrl %>" class="addActive" id="requireConfirmationMetadata">Metadata</a>
                    <div class="assign-arrow">
                    </div>
                </li>
            <%  } %>
        </ul>
    </div>