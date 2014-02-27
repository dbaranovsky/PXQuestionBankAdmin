<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.PageDefinition>" %>

    <link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.8/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
    <%--<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.4/jquery.min.js" type="text/javascript"></script>
    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.8/jquery-ui.min.js" type="text/javascript"></script>
    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.8/i18n/jquery-ui-i18n.min.js" type="text/javascript"></script>
--%>

<!DOCTYPE html>

<html>
<head runat="server">

<% Html.RenderPartial("BaseStyle"); %>
<% Html.RenderPartial("BaseScripts"); %>

<%-- <script type="text/javascript">
     jQuery(document).ready(function () {

     });

</script>--%>

    <title>Home</title>

    <style type="text/css">            
        
    #PX_HOME_ZONE_2 
    {        
        display: block;
        float: left;
        padding-left: 10px;
        padding-right: 10px;
        width: 77%;
    }
    
    #PX_HOME_ZONE_3 {
        background-color: #D2D1B1;
        border-top: 1px solid #CDCCCC;
        bottom: 0;
        float: right;
        padding: 0 12px;
        position: relative;
        right: 0;
        top: 0;
        width: 210px;
    }

    </style>
</head>
<body class="home-layout <% Html.RenderAction("GetProductType", "Course"); %> <% Html.RenderPartial("Browser"); %>">
    <div class="single-column <% Html.RenderAction("GetProductType", "Course"); %> <% Html.RenderPartial("Browser"); %>">
        <div id="brandbanner" class="home-banner">
            <div id="rightbanner"><% Html.RenderPartial("BaseHeader"); %></div>
            <div id="leftbanner"> <% Html.RenderPartial("BaseBanner"); %></div>
        </div>


       <% Html.RenderAction("HomePageCourseHeader", "Header"); %>
 <%--      <% Html.RenderPartial("Menu", Model.Menu); %>--%>
       <div id="main">
            <%Html.RenderPartial("PageContainer", Model); %>
       </div>
   </div>
    <% Html.RenderPartial("Footer"); %>
    <% Html.RenderPartial("FneWindow"); %>
    <% Html.RenderPartial("ModalWindows"); %>
       

</body>
</html>
