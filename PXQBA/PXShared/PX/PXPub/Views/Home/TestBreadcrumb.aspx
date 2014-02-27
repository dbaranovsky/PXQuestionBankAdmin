<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" MasterPageFile="~/Views/Shared/SingleColumn.Master" %>
<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    Breadcrumb Test
</asp:Content>

<asp:Content ContentPlaceHolderID="CenterContent" runat="server">
    <div class='bc'></div>
    <script type="text/javascript">
        $(document).ready(function() {
            $('#main').css('min-width', '10px');
            PxBreadcrumb.LoadItem($('.bc'), 'hockenbury5e_3_4', ['PX_TOC']);
        });
    </script>
</asp:Content>
