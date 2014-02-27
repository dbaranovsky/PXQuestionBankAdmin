<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.EbookBrowser>" %>

<!DOCTYPE html>

<html class="html">
<head runat="server">
	<title>EbookBrowser</title>
	<%var theme = ViewData["theme"] != null ? ViewData["theme"].ToString() : ""; %>	
    <link href="<%= Url.RouteUrl("CourseSectionDefault", new { controller="Style", kill=9, type = "platform" }) %>" rel="stylesheet" type="text/css" />
    <link href="<%= Url.RouteUrl("CourseSectionDefault", new { controller="Style", kill=9, type = "widget1" }) %>" rel="stylesheet" type="text/css" />
    <link href="<%= Url.RouteUrl("CourseSectionDefault", new { controller="Style", kill=9, type = "widget2" }) %>" rel="stylesheet" type="text/css" />
    <link href="<%= Url.RouteUrl("CourseSectionDefault", new { controller="Style", kill=9, theme=theme, type = "course" }) %>" rel="stylesheet" type="text/css" />
	<%= ResourceEngine.IncludesFor("~/Content/ebook.css", Url.RouteUrl("CourseSectionHome")) %>
	<% Html.RenderPartial("BaseHeaderScripts"); %>
</head>
<body>

	<div id="ebook_main_container">
				
		<div id="toc_main">
                <input type="hidden" id="ebookitemid" value="<%= Model.Id%>" />
				<%Html.RenderAction( "Detail", "ContentWidget",
                                          new { itemid = Model.Id, category = Model.CategoryId, isEbook = true });%>
										  
 
		</div>
		<div id="ebook_frame">

		<!-- Do not chnage markup this structure is needed for contents from the TOC to render -->
		<div id="ebook">
			<div id="main">
			</div>
		</div>
	</div>
	</div>
    <div id="footer">
        <% Html.RenderPartial("BaseScripts"); %>
    </div>

    <script type="text/javascript">
        PxPage.OnReady(function() {
				var deps = <%= ResourceEngine.JsonFor("~/Scripts/contentwidget.js", "~/Scripts/highlight.js") %>;
				PxPage.Require(deps, function() {
                    var itemId = $("#ebookitemid").val();
                    if(itemId != '' && ($('#' + itemId).attr("rel") == "ciuploadorcompose" || $('#' + itemId).attr("rel") == "cihtmldocument"))
                    {
					    $('#' + itemId + ' a.expand').click();
                    }
                    else
                    {
                        var firstComposedItem = $("#toc li").first();
                        if($(firstComposedItem).attr("rel") == "ciuploadorcompose" || $(firstComposedItem).attr("rel") == "cihtmldocument")
                        {
                            $(firstComposedItem).find("a.expand").click();
                        }
                    }

                    $(PxPage.switchboard).trigger("contentloaded");
					ContentWidget.Init();
				});

			$('.gearbox').css('position', 'absolute');
			$('.folder-container').remove();
		} (jQuery));
		
		function moveContent() {
			// contents() gives you a list of element and text node children
			var working = $("#right").contents();
			var ref = $("#ebook_frame").contents();

			// append can be given a list of nodes to append instead of HTML text
			$("#right").append(ref);
			$("#ebook_frame").append(working);
		}


	</script>
    </body>
</html>