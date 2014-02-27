<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>

<div id="createcourse">
    <div class="confirmation-header">
        <span class="congrats">Congratulations!</span> <span>You have successfully joined:</span>
    </div>
    <form action="" class="confirmation-message">
    <table class="course-info">
        <tr>
            <td class="first-col">
                COURSE TITLE:
            </td>
            <td class="second-col">
                <%=Model.Title %>
            </td>
        </tr>
        <tr>
            <td class="first-col">
                COURSE START DATE:
            </td>
            <td class="second-col">
                <%if (Model.IsActivated){ %>
                    <%=Model.ActivatedDate %>
                <%} %>
            </td>
        </tr>
    </table>
    </form>
    <div class="url-bookmark-message">
        The URL for this course is:
        <br />
        <% 
            var link = Url.RouteUrl("CourseSectionHome", new { courseid = Model.Id }, Request.Url.Scheme);
            var appDomainUrl = (ConfigurationManager.AppSettings["AppDomainUrl"] != null) ? ConfigurationManager.AppSettings["AppDomainUrl"].ToString() : string.Empty;

            if (!string.IsNullOrEmpty(appDomainUrl) && Request.Url.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                string find = (Request.Url.IsDefaultPort) ? string.Concat(Request.Url.Scheme, "://", Request.Url.Host) : string.Concat(Request.Url.Scheme, "://", Request.Url.Host, ":", Request.Url.Port);
                link = link.Replace(find, appDomainUrl);
            }
                    
        %>
        <%= Html.ActionLink(link, actionName: "", controllerName: "", routeValues: new { courseid = Model.Id }, htmlAttributes: null)%>
    </div>
    <div class="enroll-continue">
        <%= Html.ActionLink("Continue", actionName: "", controllerName: "", routeValues: new { courseid = Model.Id }, htmlAttributes: new { Class = "congrates-msg course-continue linkButton " })%>
        <span class="creation-url-descript">We recommend bookmarking this link to make it easy
            to return to your course.</span>
    </div>
</div>
