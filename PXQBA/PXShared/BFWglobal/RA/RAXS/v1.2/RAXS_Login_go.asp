<%
Response.Buffer = True
Response.contenttype = "text/html"

%>
<!--#include virtual="/RA/RAXS/v1/RAXS_server.asp"-->
<%

'response.write "er?"
'response.end

dim UseClassesSP : UseClassesSP = true
dim UseProfileSP : UseProfileSP = true

dim RADB_GetSiteLogins_THRESHOLD : RADB_GetSiteLogins_THRESHOLD = 1

dim iUserID : iUserID = Request.Querystring("uid")
dim iSiteID : iSiteID = Request.Querystring("sid")
dim sPackageIDs : sPackageIDs = Request.Querystring("pids")
dim returl : returl = Request.Querystring("returl")
dim rau : rau = Request.Querystring("rau")
dim RAUserRememberMe : RAUserRememberMe = Request.Querystring("rem")
dim RAUserClassPrompt : RAUserClassPrompt = Request.Querystring("cp")
dim fake : fake = Request.Querystring("fake")

dim VERSION
''VERSION = "aspgo"
''VERSION = "jsgo"
''VERSION = "linkgo"
''VERSION = "ifgo"
VERSION = "lronlygo"
%>
<html>
<head>
<style>
body {
	font-family: Arial, sans-serif;
}
</style>
<script type="text/javascript">
var VERSION = '<%=VERSION%>';
function init () {
	if (!navigator.cookieEnabled) {
		document.getElementById('msg').innerHTML = '<p>Cookies must be enabled.</p>';
	} else {
		setTimeout('start()',500);
	}
}
function start () {
	if (VERSION=='lronlygo') {
		go();
//prompt('',go2url);
	} else if (VERSION=='ifgo') {
		document.getElementById('goif').src = 'RAXS_Login.asp?uid=<%=iUserID%>&sid=<%=iSiteID%>&pids=<%=sPackageIDs%>&rem=<%=RAUserRememberMe%>&cp=<%=RAUserClassPrompt%>&returl=<%=Server.URLEncode( "192.168.77.242/RA/RAXS/v1.1/RAXS_if_done.asp" )%>&rau=<%=rau%>&fake=<%=fake%>';
	} else if (VERSION=='linkgo') {
		document.getElementById('msg').innerHTML = '<a href="JavaScript:go();">continue</a>';
	} else if (VERSION=='jsgo') {
		go();
	}
}
function go () {
//alert('go');
	if (VERSION=='lronlygo') {
//		setTimeout('window.parent.go();',1000);
//alert('<%=returl%>');
		var chrome = false;
		try {
			chrome = /chrome/.test( navigator.userAgent.toLowerCase() );
		}catch(e){}
		if (chrome) {
//prompt('','<%=returl%>?uid=<%=iUserID%>');
			document.getElementById('msg').innerHTML = '<p>You are using Google Chrome.  Please <a href="<%=RALoginRefURL%>?m=i&u=<%=iUserID%>&returl=<%=Server.URLEncode(returl)%>">click here</a> to continue.</p>';
		} else {
			setTimeout('window.location = \'<%=RALoginRefURL%>?m=i&u=<%=iUserID%>&returl=<%=Server.URLEncode(returl)%>\';',1000);
		}
	} else if (VERSION=='jsgo' || VERSION=='ifgo' || VERSION=='linkgo') {
		window.location = 'RAXS_Login.asp?uid=<%=iUserID%>&sid=<%=iSiteID%>&pids=<%=sPackageIDs%>&rem=<%=RAUserRememberMe%>&cp=<%=RAUserClassPrompt%>&returl=<%=Server.URLEncode(returl)%>&rau=<%=rau%>&fake=<%=fake%>';
	} else if (VERSION=='if') {
//	document.getElementById('msg').innerHTML = '<a href="JavaScript:go();">continue</a>';
	}
}
</script>
</head>
<body onload="init()">
<div id="msg">logging you in...</div>
<div style="display:block;padding-top:10px;font-size:10px;color:#ccc"><%=VERSION%></div>
<!--
<iframe id="goif" src="" style=""></iframe>
-->
<iframe id="goif" src="" style="position:absolute;top:-500px;left:-500px;width:200px;height:200px;"></iframe>
</body>
</html>
<%

if VERSION = "aspgo" then
	response.redirect "RAXS_Login.asp?uid="& iUserID &"&sid="& iSiteID &"&pids="& sPackageIDs &"&rem="& RAUserRememberMe &"&cp="& RAUserClassPrompt &"&returl="& Server.URLEncode(returl) &"&rau="& rau &"&fake="& fake &""
end if

%>

