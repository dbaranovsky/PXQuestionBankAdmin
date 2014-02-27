<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%
    var id = Guid.NewGuid();
 %>
<div id="progressbar" class='<%=id %>'><div class="progress-label"></div></div>

<script type="text/javascript" language="javascript">
	(function ($) {
	    PxPage.OnReady(function () {
	        var percentage = '<%=ViewData["ProgressPercentage"]%>';
	        var className = '<%=id %>';
	        className = '.' + className;
	        if (percentage != '') {
	            $(className).progressbar({
	                value: parseFloat(percentage)
	            });
	        }
	    });
	} (jQuery));
</script>