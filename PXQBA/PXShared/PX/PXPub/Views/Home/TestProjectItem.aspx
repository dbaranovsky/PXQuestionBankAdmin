<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Bfw.PX.PXPub.Models.ContentItem>>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <div>
    <%--<%
    foreach(var e in Model)
    { %>
        project id = <%= e.Id %><br />
        project title = <%= e.Title %><br />
        show headNotes = <%= e.ShowHeadNotes %><br />
        show Transcript = <%= e.ShowTranscripts %><br />
        show Questions= <%= e.ShowQuestions %><br />

        <br />
        <br />
    <% }
     %>--%>

     <%
    foreach(var e in Model)
    { %>
        project id = <%= e.Id %><br />
        project title = <%= e.Title %><br />
        item sequence = <%= e.Sequence %><br />
        <br />
        <br />
    <% }
     %>


    
    
    </div>
</body>
</html>
