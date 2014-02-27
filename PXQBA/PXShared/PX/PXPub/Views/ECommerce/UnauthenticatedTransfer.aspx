<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <title>UnauthenticatedTransfer</title>

    <script>
        // gets all segments of the url
        var segments = window.location.pathname.split('/');
        var newPathName = '';

        // reconstructs pathname based only on 3 segments (product-type, title, courseId)
        if (segments.length < 3) {
            document.write("Invalid URL, missing product type, tityle or course");
            throw Exception("Invalid URL, missing product type, title or course");
        } else {
            for (var i = 0; i < segments.length; i++) {
                if (segments[i] != '' && i < 4) {
                    newPathName += segments[i] + '/';
                }
            }

            // combine the url back together
            var newPath = window.location.protocol + "//" + window.location.host + "/" + newPathName + "ECommerce/Unauthenticated?returnUrl=";

            window.location = newPath + escape(window.location.href);
        }
      
    </script>

</head>
<body>
    <div>
        
    </div>
</body>
</html>