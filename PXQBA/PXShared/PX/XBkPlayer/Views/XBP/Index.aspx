<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.XBkPlayer.Models.XBkModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Xbook Player
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="question-player" style="display: none;">
        <div class="content-wrapper">
            <div class="content-main">
            </div>
        </div>
        <% Html.RenderPartial("PlayerActionMenu"); %>
    </div>
    <script type="text/javascript">
        (function ($) {
            $(document).ready(function () {
                var args = '<%=  Model.XBkJsonResult %>';
                var argsCollection = jQuery.parseJSON(args);

                $('#question-player').XBkPlayer(argsCollection);

                <% if (!Model.InputError)
                {
                %>
                   $('#question-player').XBkPlayer("build");
             <% } %>
            })
        } (jQuery))
    </script>
</asp:Content>
