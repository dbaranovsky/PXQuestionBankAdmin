<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>

<%  if (Model != null)
    {
        var vd = new ViewDataDictionary();
        vd["parentId"] = Model.Content.ParentId;
        if (Model.Content.Type.ToLowerInvariant() == "folder" || Model.Content.Type.ToLowerInvariant() == "pxunit")
        {
            vd["parentId"] = Model.Content.Id;
        }
        vd["title"] = HttpUtility.HtmlDecode(Model.Content.Title);
        
%>

<%Html.RenderPartial("ContentModes", Model, vd); %>
<% 
    }%>
