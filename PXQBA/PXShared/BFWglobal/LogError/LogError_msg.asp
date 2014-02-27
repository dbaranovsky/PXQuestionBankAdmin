<%

dim inMsg : inMsg = Request.QueryString("msg")

%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"
        "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
	<meta http-equiv="content-type" content="text/html; charset=iso-8859-1">
	<title>error</title>
</head>
<body>

<p>
An error has occured.
</p>
<div>
<%=replace(replace(inMsg,"NO BFW_LogError IFRAME (user will get a pop-up)"&VBCRLF&VBCRLF,""),VBCRLF,"<br/>")%>
</div>
<p>
<script type="text/javascript">
if (!window.parent) document.write('<a href="JavaScript:window.close();">close window</a>');
</script>
</p>

</body>

</html>
