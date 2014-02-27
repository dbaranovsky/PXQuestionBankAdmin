<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Commenting & Highlight Test</title>
    <script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/jquery/jquery-1.10.2.js") %>"></script>
    <script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/ZeroClipboard/jquery.zclip.js") %>"></script>
    
    <link href="<%= Url.ContentCache("~/Content/Widgets/Highlights.css") %>" rel="stylesheet" type="text/css" />
</head>
<body>
    <div class="highlightable">
        <p>
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse justo orci,
            bibendum sit amet ornare at, tempus in lorem. Maecenas placerat massa neque, id
            rhoncus justo. Duis pulvinar euismod cursus. Quisque pretium nunc eget felis semper
            adipiscing auctor nisl aliquam. Nullam sagittis tristique vulputate. Lorem ipsum
            dolor sit amet, consectetur adipiscing elit. Phasellus scelerisque commodo eleifend.
            Vivamus ornare dictum iaculis. Vivamus non justo risus, at volutpat eros. Praesent
            non arcu eros, at rutrum risus. Nullam a orci at orci commodo pulvinar eu ac nulla.
            Quisque mauris tortor, hendrerit sollicitudin sodales id, bibendum vitae augue.
            Maecenas ut interdum libero. Donec sit amet sem vel mauris pharetra adipiscing.
            Suspendisse posuere porta libero, id sagittis ante ullamcorper vitae. Duis risus
            tortor, mattis a pellentesque eu, cursus eu tortor.</p>
    </div>
</body>
</html>
