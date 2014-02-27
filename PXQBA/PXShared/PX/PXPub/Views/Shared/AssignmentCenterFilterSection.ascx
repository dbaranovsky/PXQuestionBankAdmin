<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignmentCenterFilterSection>" %>

<%var item = Model;
  var vd = new ViewDataDictionary();
  vd["lessonToLoad"] = "";
  vd["contentIdToLoad"] = "" ;
  var expandNoDueDate = "";
  var isProductCourse = false;

  var isVisible = (ViewData["isVisible"] == null) ? true : (bool)ViewData["isVisible"];
%>
   
   <% if ( item.Id == "PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY" ) { %>
   
   <%if ( item.ItemCount() == 0 && isVisible == false) { %>
   <li id="PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY" style="display:none;" class="filtersection descriptionparent <%=item.GetVisibilityClasses(isProductCourse)%>">
   <%} else { %>
   <li id="PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY" class="filtersection descriptionparent <%=item.GetVisibilityClasses(isProductCourse)%>">
    <%} %>    
    <div class="outer">
        <div class="inner">
            <ul id="__lesson_parent" class="__lesson_parent">
                <% if (item.ItemCount() > 0)
                   { %>
                <li>
                    <% Html.RenderPartial("~/Views/Unit/multipart_course_items.ascx", item.GetChildItems(), vd);%></li>
                <% }
                   else
                   { %>
                <li id="__lesson_parent_empty"><small>Learning Modules group student activities such
                    as reading, writing and quizzes into meaningful engagements.<br />
                    <br />
                    You can use our prebuilt modules or create your module plans.
                    <%=item.Title%></small></li>
                <%} %>
            </ul>
        </div>
    </div>
</li>
   
   <%} else { %>

  <%if (isVisible || item.ItemCount() > 0)
    { %>
<li id="<%=item.Id%>" class="filtersection descriptionparent <%=item.GetVisibilityClasses(isProductCourse)%>">
<%}
    else
    { %>
    <li id="<%=item.Id%>" style="display:none;" class="filtersection descriptionparent <%=item.GetVisibilityClasses(isProductCourse)%>">
<%} %>
    <h4 class="h">
        <input type="hidden" id="item-type" value="filtersection" />
        <input type="hidden" id="item-id" value="<%=item.Id%>" />
        <input type="hidden" id="item-title" value="<%=item.Title%>" />
        <input type="hidden" class="filtersectionTitle" value="<%=item.Title%>" />
        <span class="description" style="display:none;"><%=Model.GetDescriptionOrTitle()%></span>
        <span class="itemmaxpoints" style="display:none;"><%=item.MaxPoints%></span>
            <% if (item.DueDate.Year != DateTime.MinValue.Year && Model.DueDate != null) { %>
                <span class="itemdue" style="display:none;">Due <%=item.DueDate.ToShortDateString()%><br /></span>
              <%} %>
        <span class="itemsubmission" style="display:none;">
      
            </span>
        <a href="#" style="display: block; font-weight: normal;" class="trigger filtersectiontrigger">
           <p class="actitle"><%=item.Title%></p>(<span class="filtercount"><%=item.ItemCount()%></span>)</a>
            
            <% if (item.Id != "PX_ASSIGNMENT_CENTER_SYLLABUS_INSTRUCTOR_WORKSPACE") { %>
                <span class="topic-due-date-link">
                    <% 
                       vd["parentStartDate"] = item.StartDate;
                       vd["parentEndDate"] = item.DueDate;
                       var dd = new ViewDataDictionary();
                       dd["itemStartDate"] = item.StartDate;
                       dd["itemDueDate"] = item.DueDate;
                       dd["itemId"] = item.Id;
                       dd["userAccess"] = item.UserAccess;
                       dd["type"] = "category";
                       Html.RenderPartial("~/Views/Unit/DatePicker.ascx", dd, dd);
                    %>
                </span>
            <%} %>
       
    </h4>
    
    <div class="outer<%=expandNoDueDate%> category" style="display: none;">
    <%if (Model.Id != "PX_ASSIGNMENT_CENTER_SYLLABUS_NO_CATEGORY")
              { %>
        <div class="calendar-container" style="display:none;">
            <div id="calendar-message_<%=item.Id %>" class="calendar-message">Select a start date.</div>
            <div id="calendar_<%=item.Id %>" class="range px-calendar"></div>
            <div class="calendar-no-duedate"><input type="checkbox" class="no-due-chkbox" /> No due date </div>
        </div>
         <%} %>
        <div class="inner">
            <ul id="__lesson_parent" class="__lesson_parent">
                <% if (item.ItemCount() > 0)
                   { %>
                <li>
                    <% Html.RenderPartial("~/Views/Unit/multipart_course_items.ascx", item.GetChildItems(), vd);%></li>
                <% }
                   else
                   { %>
                <li id="__lesson_parent_empty" class="category-empty"><small>Learning Modules group student activities such
                    as reading, writing and quizzes into meaningful engagements.<br />
                    <br />
                    You can use our prebuilt modules or create your module plans.
                    <%=item.Title%></small></li>
                <%} %>
            </ul>
        </div>
    </div>
</li>

<%} %>
