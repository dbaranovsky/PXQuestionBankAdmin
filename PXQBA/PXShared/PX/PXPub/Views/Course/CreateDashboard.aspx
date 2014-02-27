<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.Course>" %>

<asp:Content ContentPlaceHolderID="HeaderAdditions" runat="server">    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= Model.Title %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">
    

    <div id="createcourse">
        <%= Html.Hidden("courseFneTitle", "Create your course for " + Model.ProductName)%>
       <script type="text/javascript">
           (function ($) {
               PxPage.OnReady(function () {
                   PxPage.SetFneCreateCourseTitle();
                   $('#fne-unblock-action').click(function () {
                       var path = GetCourseLocation();
                       if (path.length > 0) {
                           window.location = GetCourseLocation();
                       }
                   });

                   $('.create-course-title').keyup(function (event) {
                       var ignoreKeys = [8, 16, 17, 18, 20, 144, 45, 46, 33, 34, 35, 36, 37, 38, 39, 40];
                       if ($.inArray(event.which, ignoreKeys) >= 0)
                           return;

                       var productName = $('#ProductName').val();
                       var initVal = $(this).val();
                       var courseName = $(this).val().replace('&#', '');

                       if (initVal != courseName) {
                           $(this).val(courseName);
                       }

                       $('#CourseProductName').val(courseName + ' ' + productName);
                   });
                  
               });
           } (jQuery))

           function GetCourseLocation() {

               var path = "";
               if ($('#courseLocation').length > 0)
                   path = $('#courseLocation').val();

               return path;
           }

       </script>

       <%var OnSuccess = "function(response) {var url = response.replace ('urlEportfolio:', ''); PxPage.Loading();window.location = url;PxPage.Loading();}"; %>
   <% using (Html.BeginForm("CreateDashboard", "EportfolioCourse", FormMethod.Post, new { id = "frmCreateDashboard" }))
      {%>
        <%= Html.ValidationSummary(true)%>
        <div class="creation-info-text">
            Welcome <span class="creator-name"><%=Html.DisplayFor(model => model.CurrentUserName)%></span>! setup your <span class="creator-title"><%=Model.ProductName%></span>.
        </div>
        <%= Html.HiddenFor(model => model.Id)%>
        <%= Html.HiddenFor(model => model.ProductName)%>
             
        <ol class="course-item">
            <li>
                <%= Html.LabelFor(m => m.Title)%>
                <%= Html.TextBoxFor(model => model.Title, new { @class = "create-course-title", size = 50 })%>
                <p>The class title your students will see. (en ENG101 Creative Writing Spring 2010)</p>
                <%= Html.ValidationMessageFor(model => model.Title)%>
                <p>
                    The domain:
                    <%= Html.DropDownListFor(m => Model.SelectedDerivativeDomain, new SelectList(Model.PossibleDerivativeDomains, "Id", "Name"))%>
                </p>
            </li>
            <li>
                <%= Html.LabelFor(m => m.CourseUserName)%>
                <%= Html.TextBoxFor(model => model.CourseUserName, new { size = 50 })%>
                <p>You can change the name your students will see.</p>
                <%= Html.ValidationMessageFor(model => model.CourseUserName)%>
            </li>
            <li>
                <%= Html.LabelFor(m => m.CourseTimeZone)%>
                <%= Html.DropDownListFor(model => model.CourseTimeZone, new SelectList(TimeZoneInfo.GetSystemTimeZones(), "Id", "DisplayName"))%>
                <p>The time zone used when displaying time information in your course.</p>
                <%= Html.ValidationMessageFor(model => model.CourseTimeZone)%>
            </li>
            <li>
                <%= Html.LabelFor(m => m.CourseProductName)%>
                <%= Html.TextBoxFor(model => model.CourseProductName, new { size = 50, disabled = "disabled", @class = "course_product-name" })%>
                <p>This is the product we'll be using</p>
                <%= Html.ValidationMessageFor(model => model.CourseProductName)%>
            </li>
             <li>
              <%= Html.LabelFor(m => m.CourseType)%>
               <%=Html.HiddenFor(m => m.CourseType)%>
            </li>
            <li> <%=Html.Label("Delete existing:")%> <input type="checkbox" id="DeleteExistingEportfolioCourse" name="DeleteExistingEportfolioCourse" onchange="return PxPage.ShowDeleteDashboardMessage(this);" /></li>
<%--            <li>
                <%= Html.LabelFor(m => m.CourseHomePage)%>
                <%= Html.TextBoxFor(model => model.CourseHomePage)%>
            </li>
            <li>
                <%= Html.LabelFor(m => m.CourseOwner)%>
                <%= Html.TextBoxFor(model => model.CourseOwner)%>
            </li>--%>
            <li>
                <label>&nbsp;</label>
                <input type="button" value="Submit" onclick="PxPage.Loading(); if(PxPage.SetCourseType()) { PxPage.OnFormSubmit('Processing...', true, { form: '#frmCreateDashboard' ,data: { behavior: 'Save' }, updateSelector: '#content-item', success: <%=OnSuccess %>});}" />
                <input type="submit" value="Cancel" onclick="window.location ='';return false;" />
            </li>
        </ol>
    <% } %>    
  </div>


</asp:Content>