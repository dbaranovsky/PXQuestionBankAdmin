<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>TestRaw</title>
        <% Html.RenderAction("RenderComponent", "BHProxy", new { componentUrl = "RawComponent/FrameAPI" }); %>  
        <script type="text/javascript" language="javascript">
            var _loadComponent = function() {
                var url = "http://localhost:1192/brainhoney/rawcomponent/QuestionEditor?EnrollmentId=2754&ItemId=3acbf2578f8746529b248added6dfd39&QuestionId=8a426cb22d564401a7168e31bb425e9f";
                Ext.get('px-component').load({
                    url: url,
                    callback: function(el, success, response) {
                        if (!success) {
                            alert('Failed to load page.');
                        }
                    },
                    scripts: true
                });
            };
            Ext.onReady(function() {
                Ext.get('load-component').on('click', function() {
                    _loadComponent();
                });
            });
        </script>
    </head>
    <body> 
        <button id="load-component">Load</button>
        <div id="px-component" style="width:800px; height:400px; position:relative;"></div>       
    </body>
</html>
