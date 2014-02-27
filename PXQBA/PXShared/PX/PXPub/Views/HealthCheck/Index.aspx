<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Index</title>
</head>
<body>
    <div>
        <h2>Platform-X Status:</h2>
        <div>PXOK</div>
    </div>
    <div>
        <h2>DLAP Status:</h2>
        <div>
            <%= Html.Encode(ViewData["DlapStatus"]) %>
        </div>
    </div>
</body>
</html>
