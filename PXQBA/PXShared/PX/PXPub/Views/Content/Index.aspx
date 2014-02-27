<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SingleColumn.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.ContentIndex>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderAdditions" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">    
    <% Html.RenderAction("Detail", "ContentWidget", new { itemId = ViewData["id"] }); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FooterAdditions" runat="server">    
    <script type="text/javascript" language="javascript">
        (function($) {
            PxPage.OnReady(function() {
                var deps = <%= ResourceEngine.JsonFor("~/Scripts/highlight.js", "~/Scripts/contentwidget.js", "~/Scripts/eportfolio.js") %>;
                PxPage.Require(deps, function() {
                    $(PxPage.switchboard).trigger("contentloaded");
                    <% if(!String.IsNullOrEmpty(Model.fromSearch)){ %>
                        PxSearch.SearchSave('Model.fromSearch', '<%=Model.IncludeWords%>', '<%=Model.ExactPhrase%>', '<%=Model.ExcludeWords%>', '<%=Model.ContentTypes%>', '<%=Model.Metacategories%>');
                    <% } %>
                    
                    ContentWidget.Init();
                });
            });
        } (jQuery));
    </script>
</asp:Content>
