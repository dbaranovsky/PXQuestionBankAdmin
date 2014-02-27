<%

dim RAWSRootDomain, RAXSRootURL, RALoginRefURL

RAXSRootURL = "http://"& Request.ServerVariables("HTTP_HOST") &"/RA/v2"

RALoginRefURL = "http://"& Request.ServerVariables("HTTP_HOST") &"/login_reference/asp/c-ra.asp"
''RALoginRefURL = "http://bcs.bfwpub.com/login_reference/asp/c-ra.asp"

if inStr(Request.ServerVariables("HTTP_HOST"),"192.168.77.114") > 0 then
''	RAWSRootDomain = "bcs.bfwpub.com"
	RAWSRootDomain = "192.168.77.242"
elseif inStr(Request.ServerVariables("HTTP_HOST"),"192.168.77.242") > 0 then
	RAWSRootDomain = "192.168.77.242"
''	RAWSRootDomain = "bcs.bfwpub.com"
else
	RAWSRootDomain = "bcs.bfwpub.com"
end if

%>