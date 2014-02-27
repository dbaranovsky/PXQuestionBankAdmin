<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Logout</title>

    <% Html.RenderPartial("BaseScripts"); %>
    <% Html.RenderAction("AuthenticationScripts", "Account"); %>
    <% var url = ViewData.ContainsKey("HomeUrl") ? ViewData["HomeUrl"].ToString() : string.Empty;
       if (!string.IsNullOrEmpty(url))
       { %>
       <script language="javascript" type="text/javascript">
           jQuery(document).ready(function() { window.location = '<%= url %>'; });
       </script>
    <% }
       else
       { %>
    <script type="text/javascript" language="javascript">
        jQuery(document).ready(function() {
            setTimeout(function() { MySite_RAif_init('dologout'); }, 3000);
        });
    </script>   
    <% } %>
</head>
<body></body>
</html>