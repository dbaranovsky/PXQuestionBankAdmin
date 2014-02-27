<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>

<% var points = "";// (ViewData["maxPoints"] == null) ? "--" : ViewData["maxPoints"].ToString();
   var dueDate = (ViewData["dueDate"] == null) ? "--" : ViewData["dueDate"].ToString();

   if (Model.Content is Quiz)
   {
       try
       {
           points = ((Quiz)Model.Content).Submissions.First().Score.Value.ToString() + "%";
       }
       catch { }
   }
   
   
   %>
<div id="faceplate-fne-student-details">
    Due: <%=dueDate %> &nbsp; &nbsp; Grade: 
</div>

<script type="text/javascript" language="javascript">
    (function ($) {
        PxPage.OnReady(function () {
            $("#fne-title-left #faceplate-fne-student-details").remove();
            $("#content-item #faceplate-fne-student-details").appendTo($("#fne-title #fne-title-left"));
            $("#content-item #faceplate-fne-student-details").remove();

            var contentId = $("#content-item-id").text();
            if (contentId == "") {
                var contentId = $(".content-item-id").text();
            }
            var details = $("#fne-title #fne-title-left #faceplate-fne-student-details").text();
            if ($(".faux-tree-node[data-ft-id=\"" + contentId + "\"] .pxunit-display-points").is(":visible")) {
                var points = $(".faux-tree-node[data-ft-id=\"" + contentId + "\"] .pxunit-display-points").text();
                details = details.replace("Grade:", "Grade: " + points);
                $("#fne-title #fne-title-left #faceplate-fne-student-details").text(details);
            } else {
                details = details.replace("Grade:", "Grade: -- / --");
                $("#fne-title #fne-title-left #faceplate-fne-student-details").text(details);
            }

        });
    } (jQuery));
</script>