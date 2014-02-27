<% Response.Buffer = False
Response.contenttype = "text/html"
dim RARootDomain
%>
<!-- #include file="RAXS_server.inc" -->
<%

dim iUserID : iUserID = Request.Querystring("uid")
dim iSiteID : iSiteID = Request.Querystring("sid")
dim sPackageIDs : sPackageIDs = Request.Querystring("pids")
dim returl : returl = Request.Querystring("returl")
dim show : returl = Request.Querystring("show")
if show = "" then
	show = "none"
end if

if 0 then
	iUserID = 118
	iSiteID = 24382
	sPackageIDs = "663,362"
end if

show = "true"

if show = "true" then
%>
<html>
<head>
<style>
body {
	font-family: Arial, sans-serif;
}
</style>
<script language="JavaScript">
function init() {
}
function RAXS_Login_done (uid) {
}
</script>
</head>
<body onload="init()">
<iframe src="<%

	response.write "RAXS_Login_iF_do.asp?uid="& iUserID &"&sid="& iSiteID &"&pids="& sPackageIDs &"&show=true"

%>" style="position:absolute;top:-500px;left:-500px;width:50px;height:50px;"></iframe>
logging in...
</body>
</html>
<%
else
	response.redirect "RAXS_Login_do.asp?uid="& iUserID &"&sid="& iSiteID &"&pids="& sPackageIDs &"&show=false&returl="& Server.URLEncode(returl)
end if




' *********************************************************************************
' *********************************************************************************
' *********************************************************************************
' *********************************************************************************


sub debugWrite (vs, msg)
'	response.write msg
'	response.write "<br/>"
end sub

%>

