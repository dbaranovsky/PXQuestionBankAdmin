<%@ Page Language="C#" MasterPageFile="~/Views/Shared/SingleColumn.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.AssignmentWidget>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ViewAll
</asp:Content>

<asp:Content ContentPlaceHolderID="CenterContent" runat="server">  
   <% Html.RenderAction("MultipartLessons","Course");%> 
   
</asp:Content>