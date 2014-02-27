<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.LayoutConfiguration>" %>

<asp:Content ContentPlaceHolderID="HeaderAdditions" runat="server"></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= Model.Title %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server" >
<% 
    if (Model.Course.CourseType != CourseType.LearningCurve && Model.Course.CourseType != CourseType.FACEPLATE)
    {
%>
    <% if (Model.IsAllowedToActivate && !Model.IsActivated && !Model.Course.IsSandboxCourse )
       { %>

       
        <div class="homepageheader hideHeader">
        <%  %>
            <% if (ViewData["ActivationText"] != null && !String.IsNullOrEmpty(ViewData["ActivationText"] as String))
               {
                   var activationText = ViewData["ActivationText"].ToString();
                   var activationLink = Html.ActionLink("Activate my Course", "ShowCreateNameCourse", "Course", null, new { @class = "fne-link fixed", title = "" }).ToHtmlString();
                   activationText = activationText.Replace("[[Activation]]", activationLink);
                   activationText = activationText.Replace("[[SaveAsTemplate]]", "Save as Template");
            %> 
            
                <%= activationText %>
              <% }
               else
               {
                   var activateButtonHtml = "<div class=\"create-instructions\"><span class=\"acticon\"></span><p style=\"font-style:larger;\">Make this course available to my students.</p><p>Activate once you're ready to distribute the course URL to your class</p></div>";

                   var lnkHtml = Html.ActionLink("Activate this Course", "ShowCreateNameCourse", "Course", null, new { @class = "creation-button activate-button activation-button fne-link", title = "" }).ToHtmlString();
                   lnkHtml = lnkHtml.Replace("Activate this Course", activateButtonHtml);
            %> 
            <%=lnkHtml%>
            <%} %>
        </div>       
    <% }
       else if (!Model.IsAllowedToActivate && !Model.IsActivated && !Model.Course.IsSandboxCourse)
       { %>
        <div class="homepageheader hideHeader">
            <div style='border: 1px solid #E99323;display: block; margin: 8px auto 0 auto; position: relative;text-align: center;width: 70%;height: 72px;font-size: 0.8em;'>
                <div style="margin:5px">Please bookmark the home page URL currently displayed above in your browser. <br/>
                This is your exclusive course URL. From the home page, you can edit your course to customize it.
                <BR/><BR/>
                Your Macmillan Higher Education sales representative can assist you with course adoption <br/>
                so that your students can access your course.</div>
            </div>
        </div>
    <%} %>
<%
    }
%>
    <%  
        if(Model.Course.IsSandboxCourse)        
            Html.RenderAction("Index", "Sandbox");
    %>        
    <% if ( Model.IsAllowedToCreateCourse ) {%>
        <div class="homepageheader hideHeader">
            <%
           var createButtonHtml = "<div class=\"create-step1\"> </div> <div class=\"create-instructions\"><p>Welcome " + ViewData["CurrentUser"].ToString() + "</p><p>Your access to this product is approved.</p><p>Click here to get started.</p></div> ";
                var lnkHtml = Html.ActionLink("Create a Course", "ShowCreateCourse", "Course", null, new { @class = "creation-button fne-link fixed", title="" }).ToHtmlString();
                //if (ViewData["DomainCount"].ToString().Equals("1"))
                //{
                //    lnkHtml = lnkHtml.Replace("fne-link", "");
                //}
                lnkHtml = lnkHtml.Replace("Create a Course", createButtonHtml);
            %> 
            <%=lnkHtml %>
            <br />

        </div>
    <% } %>

    <% var courseActivation = Model.IsActivated ? "course-active" : "course-inactive"; %>

    <div id="page-definition-wrapper" style="height:100%" class="<%= courseActivation %>">
    <%ViewData["IsAllowedToCreateCourse"] = Model.IsAllowedToCreateCourse;%>
    <%        
         Html.RenderPartial("PageContainer", Model.PageDefinitions);
    %>
    </div>
    <div style="display:none;">
   <% 
        var component = new BhComponent {
            ComponentName = "FrameApi"
        };
        Html.RenderPartial("BhIFrameComponent", component);
    %>
    </div>
    
</asp:Content>