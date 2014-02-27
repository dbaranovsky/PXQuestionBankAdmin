<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%@ Import Namespace="Bfw.PX.Biz.ServiceContracts" %>
<div id="fne-header-results">
    <!-- Done button -->

   <%Html.RenderPartial("FneHeaderDoneButton", Model); %>
    <div id="fne-title">
        Results for "
        <%= Model != null && Model.Content != null ? Model.Content.Title : string.Empty%>"
        <%--<div class="breadcrumb">
            </div>--%>
        <%--<span id="fne-title-breadcrumb"></span>--%>
    </div>
  
</div>
