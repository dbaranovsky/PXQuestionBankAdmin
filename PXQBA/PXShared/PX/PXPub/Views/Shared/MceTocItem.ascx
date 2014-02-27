<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<Bfw.PX.PXPub.Models.TocItem>>" %>

<% foreach (var tocItem in Model)
   {
       var itemId = tocItem.Id;
       var title = HttpUtility.HtmlDecode(tocItem.Title);
       var ParentId = (tocItem.ParentId == "PX_COURSE_EPORTFOLIO") ? string.Empty : tocItem.ParentId;
       var sequence = tocItem.Sequence;

       var tocItemUrl = Url.Action("Index", "Content",
                                                   new
                                                   {
                                                       id = tocItem.Id,
                                                       mode = ContentViewMode.ReadOnly,
                                                       includeToc = false,
                                                       includeDiscussion = false
                                                   }, HttpContext.Current.Request.Url.Scheme);

       
       var itemclass = tocItem.ItemType.ToLowerInvariant() == "eportfolio" ? "eportfoliofolder" : "reflectionassignment";

       %>

    <li class="faux-tree-node mcetocitem" data-ft-parent="<%= ParentId %>" data-ft-id="<%= itemId %>">
        <span class="icon"></span>
        <span class="<%= itemclass %>">&nbsp;&nbsp;&nbsp;</span> 
        <a id="<%= itemId %>" href=""><span><%= title%></span> </a>
        <input type="hidden" id="tocItemId" value='<%=tocItem.Id%>' />
        <input type="hidden" id="tocItemUrl" value='<%= tocItemUrl %>' />
    </li>

    <% if (tocItem.Children.Any())
       {
            var vd = new ViewDataDictionary<IEnumerable<Bfw.PX.PXPub.Models.TocItem>>();
            vd.Model = tocItem.Children;
            Html.RenderPartial("MceTocItem", vd);
       }

 } %>


