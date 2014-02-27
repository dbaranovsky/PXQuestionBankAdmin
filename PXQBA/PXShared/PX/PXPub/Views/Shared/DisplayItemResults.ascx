<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%
    var items = new List<ContentItem>();
                   
    if (Model.Content.Type == "PxUnit")
    {
        items = ((LessonBase)Model.Content).GetAssociatedItems();
    }
    else //if (Model.Content.Type == "Quiz" || Model.Content.Type == "Assignment" || 1 == 1) //force all type for results
    {
        items.Add(Model.Content);
    }

%>

<div id="ItemDetailsReport">
    <%
        foreach (ContentItem ci in items)
        { 
            BhComponent bh1 = new BhComponent();
            bh1.ComponentName = "ItemDetails";
            bh1.Parameters = new { EnrollmentId = Model.Content.EnrollmentId, ItemId = ci.Id, entityid = Model.Content.CourseInfo.Id };
            Html.RenderPartial("BhIFrameComponent", bh1);
        %><div style="clear: both">
        </div>
        <%
        }
    %>
</div>
