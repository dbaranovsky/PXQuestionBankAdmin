<%
Response.Buffer = True
Response.contenttype = "text/html"
%>
<html>
<head>
<style>
body {
	font-family: Arial, sans-serif;
}
</style>
<script type="text/javascript">
function init () {
//alert('done');
	window.parent.go();
}
</script>
</head>
<body onload="init()">
</body>
</html>
<%
%>

