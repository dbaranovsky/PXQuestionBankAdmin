<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ProductMinimal.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.Course>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Confirmation
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">
 <div id="PX_HOME_ZONE_2" class="zoneParent">
    <div id="createcourse">
       <div class="confirmation-header">
            <span class="congrats">Congratulations!</span>
               <span>You have successfully enrolled in:</span>
        </div>
         <form action="" class="confirmation-message">
         <table class="course-info">
            <tr>
                <td class="first-col">COURSE TITLE:</td>
                <td class="second-col"><%=Model.CourseProductName %></td>
            </tr>
            <tr>
                <td class="first-col">COURSE NUMBER:</td>
                 <td class="second-col"><%=Model.CourseNumber %></td>
            </tr>
            <tr>
                <td class="first-col">COURSE SECTION:</td>
                 <td class="second-col"><%=Model.SectionNumber  %></td>
            </tr>
            <tr>
                <td class="first-col">COURSE START DATE:</td>
                 <td class="second-col"><%=Model.ActivatedDate %></td>

        </table>
        </form>

        <div class="url-bookmark-message">
            The URL for this course is
            <br />
            <% 
                //var port = Request.Url.Port; 
                //var hostname = Request.Url.Host.ToString(); 
                var link = Url.RouteUrl("CourseSectionHome", new { courseid = Model.Id }, "http");
        //remove port #
        //link = link.Replace ( ":" + Request.Url.Port, "" ); 
            %>
            <a id="course-home-link" href="<%=link%>">
                <%=link%></a>
            </div>
        <div>
            <a href="<%=link%>" style="text-decoration: none;" class="linkButton">
                    Continue</a><span class="creation-url-descript">We recommend bookmarking this link to make it easy to return to your course.</span>
        </div>

    </div>
    </div>

    <div id="PX_HOME_ZONE_3" class="zoneParent"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderAdditions" runat="server">
</asp:Content>