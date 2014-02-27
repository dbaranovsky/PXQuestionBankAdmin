<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Menu>" %>

<% 
    var usehash = Model.Properties.ContainsKey("bfw_usehash") ? Convert.ToBoolean(Model.Properties["bfw_usehash"].Value) : false;
%>
<div class="PX_Menu PX_MenuWidget_Wrapper">
    <ul class="parent">
        <% foreach (var child in Model.MenuItems)
           {
               if (child.IsHidden != true)
               {
                   Html.RenderPartial("~/Views/MenuWidget/MenuItem.ascx", child);
               }
           } %>
    </ul>

    <% Html.RenderPartial("~/Views/MenuWidget/Pagination.ascx", Model); %> 

</div>
<script type="text/javascript">
    PxPage.OnProductLoaded(function () {
        var deps = ['<%= Url.ContentCache("~/Scripts/MenuWidget/PxMenuWidget.js") %>'];
        
        PxPage.Require(deps, function () {
                    var args = {
                        usehash: '<%=usehash%>'
                    };
                    $("#main").PxMenuWidget(args);
                });
           });
</script>
