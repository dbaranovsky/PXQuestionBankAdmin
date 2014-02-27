<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<% using (Html.BeginForm(new { courseid = Model.Id })) { %>
        <%= Html.ValidationSummary(true) %>
    <%= Html.Hidden("courseFneTitle", "Your " + Model.CourseProductName + " is ready")%>
    <div class="creation-info-text">
        <span class="creator-name"><%=Html.DisplayFor(model=>model.CourseUserName)%></span>, your class 
        <span class="creator-title"><%=Model.CourseProductName%></span> 
        is ready.
    </div>
    <%= Html.HiddenFor(model => model.Id) %>
    <%= Html.HiddenFor(model => model.ProductName) %>
       
    <div class="course-setup"> 
        <div class="course-setup-steps step1"></div> 
        <div class="course-setup-steps step2"></div> 
        <%-- if instructor type is a sampler or demo dont allow the step 3 --%>
        <% if (Model.UserAccessType != Bfw.PX.Biz.ServiceContracts.AccessType.Demo) {%>
            <div class="course-setup-steps step3"></div>
        <%} %>
    </div>
      
    <div class="important" >
        <span class="important-alert">Important:</span> Your students will not be able to see your class until you activate the class. The activate button is always located in the home page of your class.
    </div>
    <div class="creation-description">This will allow you time to ensure that you are happy with the class and are ready to begin. Once you activate the class a URL can be distributed to your students allowing them to register for your class.</div>

    <div class="course-creation-buttons">
     <% var modelId = Model.Id ?? "";
       if (modelId == "undefined")
           modelId = ""; 
       %>
     <% var link = Url.RouteUrl("CourseSectionHome", new { courseid = modelId });%>
     <input type ="button" onclick="window.location='<%=link%>'"  value="Close"/>
     <input type="hidden" id="courseLocation" name="courseLocation" value="<%=link%>"/>
    </div>
<%} %>