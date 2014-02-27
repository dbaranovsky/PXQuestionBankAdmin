<% Response.Buffer = True
Response.contenttype = "text/html"

%>
<!--#include file="./RAXS_server.asp"-->
<%

dim returl : returl = Request.Querystring("returl")
returl = "http://"& replace(returl,"http://","")
if returl = "http://" then
	returl = ""
end if

dim IDs_str
dim arr, i

for each i in Response.Cookies
	Response.Cookies(i) = ""
	Response.Cookies(i).Expires = Now()
next

if returl<>"" then
	response.redirect "http://bcs.bfwpub.com/login_reference/asp/c-ra.asp?m=o&returl="&Server.URLEncode(returl)
	response.end
else
	if Request.Querystring("lr") <> "true" then
		response.redirect RALoginRefURL &"?m=o&returl="&Server.URLEncode( RAXSRootURL &"/RAXS_Logout.asp?lr=true")
		response.end
	else
%>
<html>
<head>
<HTTP-EQUIV="PRAGMA" CONTENT="NO-CACHE">
<script language="JavaScript"></script>
<style>
body {
	font-family: Arial, sans-serif;
}
</style>
</head>
<body onload="window.opener.focus();setTimeout('window.close()',50)">
logging you out...
<!--
global RAXS_Logout.asp
<br/>
Request.Cookies("RAUserID") = <%=Request.Cookies("RAUserID")%>
<br/>
returl=<%=returl%>
<br/>
-->
</body>
<head>
<HTTP-EQUIV="PRAGMA" CONTENT="NO-CACHE">
</head>
</html>
<%
	end if
end if

%>

