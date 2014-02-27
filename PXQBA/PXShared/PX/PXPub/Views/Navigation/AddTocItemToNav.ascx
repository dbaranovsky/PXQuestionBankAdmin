<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.NavigationItem>>" %>


<%
    var templateLinks = ViewData["templateLinks"] == null ? new List<Link>() : (IEnumerable)ViewData["templateLinks"];
    var settings = ViewData["settings"] == null ? new Hashtable() : (Hashtable)ViewData["settings"];
%>
        <ul class="navigationItemList">
            <%foreach (var item in Model){ %>
            <li>
                <ul id="sortable" style="padding-bottom: 25px;">
                    <%Html.RenderPartial("NavigationItem", (NavigationItem)item);%>
                </ul>
            </li>
            <%} %>
            <% foreach (var item in templateLinks){
                    var lnk = (Link)item;%>
                    <%Html.RenderPartial("NavTemplateLink", lnk);%>
                <%} %>
        </ul>

