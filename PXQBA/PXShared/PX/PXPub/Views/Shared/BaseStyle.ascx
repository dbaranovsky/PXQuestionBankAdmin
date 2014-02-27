<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var productType = Html.Action("GetCourseType", "Course").ToString().ToLowerInvariant();
   var productCourseId = Html.Action("GetProductCourseId", "Course").ToString().ToLowerInvariant(); 
   var courseTitle = Html.Action("GetCourseProductName", "Course").ToString().ToLowerInvariant();
   var theme = Request.QueryString["theme"] != null ? Request.QueryString["theme"].ToString() : Html.Action("GetCourseTheme", "Course").ToString();
   var version = ResourceEngine.Version; %>

<%-- Loads platorm css (defaultlayout.css) --%>
<%= ResourceEngine.LinkTag(Url.RouteUrl("CourseSectionDefault", new { controller="Style", type = "platform", version }))  %>

<%-- Loads widget css from the /widgets/ directory. Widgets to load are defined in web.config for each product (ie: ~/Content/faceplate_widgets.css)--%>
<% Html.RenderAction("IncludesForWidgetCSS", "Style", new RouteValueDictionary(new {productType = productType})); %>

<%-- The line below was used to load title-specific CSS (ie: myers10e.css). This is no longer necessary 
    TODO: Should be replaced with a way to inject a CSS file into the course--%>
<%--<%= ResourceEngine.LinkTag(Url.RouteUrl("CourseSectionDefault", new { controller="Style", theme=theme, type = "course", version })) %>--%>

<%-- Loads product-specific css (ie: faceplate.css) --%>
<%= ResourceEngine.LinkTag(Url.RouteUrl("CourseStyleCourseCss", new { theme=theme, courseType = productType, version })) %>

<%-- Loads title-specific css by using the product_title.less (ie: faceplate_title.less) template and injecting course-specific css paramaters --%>
<%= ResourceEngine.LinkTag(Url.RouteUrl("CourseStyleTitleCss", new { courseType = productType, courseProductName = courseTitle, productCourseId, version})) %>
<% if (productType == "faceplate")
   { %>
    <link href='http://fonts.googleapis.com/css?family=Lato:400,700,400italic,700italic|Open+Sans:400,700,400italic,700italic' rel='stylesheet' type='text/css' />
<% } %>
<!-- Loads css files defined in web.config (basestyle.css) -->
<%= ResourceEngine.IncludesFor("~/Content/basestyle.css", Url.RouteUrl("CourseSectionHome")) %>
