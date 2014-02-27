<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.TreeWidgetViewItem>>" %>
<%    
    Action<TreeWidgetViewItem> renderItem = null;
    renderItem = (TreeWidgetViewItem item) =>
     {
         item.Level++;
         Html.RenderPartial("~/Views/LaunchpadTreeWidget/LaunchPadItem.ascx", new List<TreeWidgetViewItem>() { item });
     };

    foreach (var item in Model)
    {
        renderItem(item);
    } 
%>
