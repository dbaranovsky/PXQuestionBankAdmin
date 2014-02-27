<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<%
    var productName = Model.CourseType.ToString();
    if (!string.IsNullOrEmpty(productName))
    {
        var productJS = string.Format("~/Scripts/{0}.js", productName);
%>
<script type="text/javascript">
	(function ($) {
	    PxPage.OnReady(function() {
	        var deps = <%= ResourceEngine.JsonFor(productJS) %>;
	        var depsIncluded = <%= ResourceEngine.JsonForAllFiles(productJS) %>;

	        PxPage.Require(deps, function() {
	            //We want to be able to call product loaded even if there is no product script
	            if (window.<%=productName %>) {
	                if (<%=productName %>.Init) {
	                    <%=productName %>.Init();
	                }
	                PxPage.ProductLoaded = true;
    				    $(PxPage.switchboard).trigger("productloaded");
    				} else {
    				    PxPage.ProductLoaded = true;
    				    $(PxPage.switchboard).trigger("productloaded");
    				}
  		    }, depsIncluded);
	    });   
    }(jQuery));
<% } %>
</script>
