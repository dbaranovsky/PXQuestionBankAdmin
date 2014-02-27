<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ThreeColumn.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ProductHome
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">

    <h2>ProductHome</h2>
    
     <%= Html.ActionLink("Create a Course", "ShowCreateCourse", "Course", null, new { @class = "fne-link" })%> |

</asp:Content>