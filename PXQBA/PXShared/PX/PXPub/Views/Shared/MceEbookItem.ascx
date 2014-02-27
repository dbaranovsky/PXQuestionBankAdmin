<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<Bfw.PX.PXPub.Models.TocItem>>" %>

<% foreach (var tocItem in Model)
   {
       var itemId = tocItem.Id;
       var title = HttpUtility.HtmlDecode(tocItem.Title);
       var ParentId = (tocItem.ParentId.ToUpperInvariant() == "PX_EBOOKS") ? string.Empty : tocItem.ParentId;
       var ebookCategory = (tocItem.ParentId.ToUpperInvariant() == "PX_EBOOKS") ? tocItem.ItemType : ViewData["ebookCategory"];
       var sequence = tocItem.Sequence;
       
       var tocItemUrl = Url.Action("Index", "EbookBrowser",
                                                   new
                                                   {
                                                       itemid = tocItem.Id,
                                                       category = ebookCategory
                                                   });
       %>

    <li class="faux-tree-node ebookitem" data-ft-parent="<%= ParentId %>" data-ft-id="<%= itemId %>">
        <span class="icon"></span>
        <span class="eportfoliofolder">&nbsp;&nbsp;&nbsp;</span> 
        <a id="<%= itemId %>" href=""> <span><%= title%></span> </a>
        <input type="hidden" id="tocItemId" value='<%=tocItem.Id%>' />
        <input type="hidden" id="tocItemUrl" value='<%= tocItemUrl %>' />
        <input type="hidden" id="ebookCategory" value='<%= ebookCategory  %>' />
    </li>

    <% if (tocItem.Children.Any())
       {
            var vd = new ViewDataDictionary<IEnumerable<Bfw.PX.PXPub.Models.TocItem>>();
            vd.Model = tocItem.Children;
            vd["ebookCategory"] = ebookCategory;
            Html.RenderPartial("MceEbookItem", vd);
       }

 } %>
