<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<html>
    <head>
        <% Html.RenderPartial("BaseStyle"); %>
    </head>
    <body>
      <% Html.RenderPartial("BaseHeaderScripts"); %>
        <h2>Component Test</h2>

        <div 
            id="bh-component-frame-blahblah"
            class="bh-component" 
            style="width:600px; height:600px;"
            rel="http://pxmigration.bhdev.bfwpub.com/brainhoney/component/ActivityPlayer?EnrollmentId=29535&ItemId=47a8e45287124a3ba110a8aad64554a9&ShowHeader=False&Extra=autostart^true|group^0|action=active"
        ></div>

        <% Html.RenderPartial("BaseScripts"); %>
        <%= ResourceEngine.IncludesFor("~/Scripts/common.js") %>
        <%= ResourceEngine.IncludesFor("~/Scripts/contentwidget.js") %>
        <script type="text/javascript">
            $(function () {
                //PxPage.SetFrameApiHooks();
            });
        </script>
    </body>
</html>