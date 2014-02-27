<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Start.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.LayoutConfiguration>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderAdditions" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContent" runat="server">
    <%= Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="CenterContent" runat="server" >
    
    <%ViewData["IsAllowedToCreateCourse"] = Model.IsAllowedToCreateCourse;
      ViewData["LmsId"] = ViewData["LmsId"] != null ? ViewData["LmsId"].ToString() : "";
      ViewData["LmsIdRequired"] = Model.Course.LmsIdRequired;
      ViewData["LmsIdLabel"] = Model.Course.LmsIdLabel;
      %>
    <%        
        Html.RenderPartial("StartPageContainer", Model.PageDefinitions);
    %>

    <div style="display:none;">
     <% 
        //var component = new BhComponent {
        //    ComponentName = "FrameApi"
        //};
        //Html.RenderPartial("BhIFrameComponent", component);
    %>
    </div>
    
</asp:Content>