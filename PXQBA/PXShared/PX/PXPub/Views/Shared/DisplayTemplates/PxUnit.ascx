<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.PxUnit>" %>

<%
    if (!string.IsNullOrWhiteSpace(Model.Url))
    {
        var externalContent = new ExternalContent()
        {
            Url = Model.Url,
            Title = Model.Title,
            DisciplineId = Model.DisciplineId,
            FacetMetadata = Model.FacetMetadata,
            UserAccess = Model.UserAccess,
            AllowComments = Model.AllowComments,
            ProxyConfig = Model.ProxyConfig,
            Id = Model.Id,
            NoteId = Model.NoteId,
            IsProductCourse = Model.IsProductCourse
        };%>

        <%= Html.DisplayFor(m => externalContent)%>
<%  }
    else
    { 
%>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {

        <%  var isReadOnly = Model.IsReadOnly;
            if (!isReadOnly)
            {
                isReadOnly = (Model.UserAccess != Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor);
            }%>

            var deps = <%= ResourceEngine.JsonFor("~/Scripts/contentwidget.js") %>;
            PxPage.Require(deps, function () {
                 $('.unit-content-wrapper').PxUnitDetails({ readOnly:<%=isReadOnly.ToString().ToLowerInvariant() %>}); 
                 
            });
        });

    } (jQuery));    
</script>
<%
    var isShowDelete = (Request.QueryString["isShowDelete"] == null) ? "True" : Request.QueryString["isShowDelete"].ToString();
    var lessonId = ExtensionMethods.GetMultipartPartLessonId(Request, ViewData);
    if (ViewData["isShowDelete"] != null)
    {
        isShowDelete = ViewData["isShowDelete"].ToString();
    }    
%>


<div id="assignmentcenternav"></div>
    <h2 class="content-title titleunderline">
        <div class="menu edit-link" style="float: right;">
            <%if (!string.IsNullOrEmpty(Model.AssociatedToCourse))
              { %>
                <%if (Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor && !isReadOnly)
                  {
                      var editUrl = Url.Action("DisplayItem", "ContentWidget",
                      new
                      {
                          id = Model.Id,
                          mode = ContentViewMode.Edit,
                          hasParentLesson = lessonId,
                          includeNavigation = false,
                          isBeingEdited = true
                      });    
                    %>
                    <a href="<%=editUrl %>" class="linkButton nonmodal-link">Edit</a>
                <%} %>
            <%} %>
        </div>
    </h2>
    <ol>        
        <li>
            <div class="baselessondetail">
                <div class="unit-header" style="overflow:hidden">
                     <%if (!String.IsNullOrEmpty(Model.Thumbnail) && !Model.Thumbnail.Equals(ConfigurationManager.AppSettings["DefaultImage"]))
                     { %>
                        <div class="lessonimage" style="float:left">
                            <img alt="<%=Model.Title%>" src="<%=Model.Thumbnail%>" />
                        </div>
                    <% } %>
                        <div class="unit-title-info">
                            <p class="lessonTitle"><%= HttpUtility.HtmlDecode(Model.Title)%></p>
                            <% var dateRange = Model.GetFriendlyDateRange();
                            if(!string.IsNullOrEmpty(dateRange))
                            { %>
                                <div id="divUnitDateAssigned" class="px-default-text"><label>Assigned Date:</label> <span><%=dateRange%></span></div>
                             <% } %>
                        </div>
                </div>     
           
                <div class="lessondescription">
                    
                        <div class="lessonActions">                        
                            <ol id="actionList">
                               <li style="padding-bottom:5px">
                                <% if (Model.Id != null && !string.IsNullOrEmpty(Model.EnrollmentId))
                                   { %>
                                    <%if (string.IsNullOrEmpty(Model.AssociatedToCourse) && Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor && !isReadOnly)
                                      { %>

                                     <%}
                                      else if (!string.IsNullOrEmpty(Model.AssociatedToCourse) && Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor && !isReadOnly)
                                      {%>
                                       <input type="button" title="Add" id="Button3" style="display:none;" value="<< Remove from Syllabus"  onclick="return PxSyllabusCategory.DeleteItemFromSyllabus('<%=Model.Id%>', true);" />     
                                    <%}
                                   }%>                              
                            </li>
                          </ol>
                            <div id="addtosyllabusResult" class="px-default-text"></div>
                        </div>
                    <span class="px-default-text"><%=Model.Description%></span>
                </div>
            </div>
        </li>
        <li>
          
        </li>
    </ol>

<%} %>