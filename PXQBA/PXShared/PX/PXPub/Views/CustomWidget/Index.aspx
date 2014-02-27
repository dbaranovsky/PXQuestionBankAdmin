<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SingleColumn.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderAdditions" runat="server">
	
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Px Content Search
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">
	

	<div id="left">    
	
	</div>
	
	<div id="right">   
	<% Html.RenderAction("ShowWidgetConfig", "CustomWidget", null);%>
	
	 </div>
	
</asp:Content>


