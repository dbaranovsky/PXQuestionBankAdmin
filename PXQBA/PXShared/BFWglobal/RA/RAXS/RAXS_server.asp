<%

dim referrer : referrer = Request.ServerVariables("HTTP_REFERER")
'responseStr = responseStr &"alert('referrer: "& referrer &"');"& VBCRLF
'
dim referrerAuthenticated : referrerAuthenticated = false
dim remoteAddr : remoteAddr = Request.ServerVariables("REMOTE_ADDR")
'responseStr = responseStr &"alert('remoteAddr: "& remoteAddr &"');"& VBCRLF
'
dim remoteAddrAuthenticated : remoteAddrAuthenticated = false

if inStr(referrer,"http://bcs.bfwpub.com") = 1  OR inStr(referrer,"bcs.bfwpub.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://bcs.bedfordstmartins.com") = 1  OR inStr(referrer,"bcs.bedfordstmartins.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://bcs.whfreeman.com") = 1  OR inStr(referrer,"bcs.whfreeman.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://bcs.worthpublishers.com") = 1  OR inStr(referrer,"bcs.worthpublishers.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://www.bfwpub.com") = 1  OR inStr(referrer,"www.bfwpub.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://www.bedfordstmartins.com") = 1  OR inStr(referrer,"www.bedfordstmartins.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://www.whfreeman.com") = 1  OR inStr(referrer,"www.whfreeman.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://www.worthpublishers.com") = 1  OR inStr(referrer,"www.worthpublishers.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://dev.bfwpub.com") = 1  OR inStr(referrer,"dev.bfwpub.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://dev.bedfordstmartins.com") = 1  OR inStr(referrer,"dev.bedfordstmartins.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://dev.whfreeman.com") = 1  OR inStr(referrer,"dev.whfreeman.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://dev.worthpublishers.com") = 1  OR inStr(referrer,"dev.worthpublishers.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://192.168.77.114") = 1  OR inStr(referrer,"192.168.77.114") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://192.168.77.242") = 1  OR inStr(referrer,"192.168.77.242") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://192.168.77.243") = 1  OR inStr(referrer,"192.168.77.243") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://192.168.77.244") = 1  OR inStr(referrer,"192.168.77.244") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://192.168.77.245") = 1  OR inStr(referrer,"192.168.77.245") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://dev-gradebook.bfwpub.com") = 1  OR inStr(referrer,"dev-gradebook.bfwpub.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://dev-scorecard.bfwpub.com") = 1  OR inStr(referrer,"dev-scorecard.bfwpub.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://stg-gradebook.bfwpub.com") = 1  OR inStr(referrer,"stg-gradebook.bfwpub.com") = 1 then
	referrerAuthenticated = true
elseif inStr(referrer,"http://stg-scorecard.bfwpub.com") = 1  OR inStr(referrer,"stg-scorecard.bfwpub.com") = 1 then
	referrerAuthenticated = true
elseif inStr(Request.ServerVariables("HTTP_HOST"),"dev-bcs.") = 1 then
	referrerAuthenticated = true
elseif inStr(Request.ServerVariables("HTTP_HOST"),"stg-bcs.") = 1 then
	referrerAuthenticated = true
end if

if inStr(remoteAddr,"http://bcs.bfwpub.com") = 1  OR inStr(remoteAddr,"bcs.bfwpub.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://bcs.bedfordstmartins.com") = 1  OR inStr(remoteAddr,"bcs.bedfordstmartins.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://bcs.whfreeman.com") = 1  OR inStr(remoteAddr,"bcs.whfreeman.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://bcs.worthpublishers.com") = 1  OR inStr(remoteAddr,"bcs.worthpublishers.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://www.bfwpub.com") = 1  OR inStr(remoteAddr,"www.bfwpub.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://www.bedfordstmartins.com") = 1  OR inStr(remoteAddr,"www.bedfordstmartins.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://www.whfreeman.com") = 1  OR inStr(remoteAddr,"www.whfreeman.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://www.worthpublishers.com") = 1  OR inStr(remoteAddr,"www.worthpublishers.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://dev.bfwpub.com") = 1  OR inStr(remoteAddr,"dev.bfwpub.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://dev.bedfordstmartins.com") = 1  OR inStr(remoteAddr,"dev.bedfordstmartins.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://dev.whfreeman.com") = 1  OR inStr(remoteAddr,"dev.whfreeman.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://dev.worthpublishers.com") = 1  OR inStr(remoteAddr,"dev.worthpublishers.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://192.168.77.114") = 1  OR inStr(remoteAddr,"192.168.77.114") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://192.168.77.242") = 1  OR inStr(remoteAddr,"192.168.77.242") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://192.168.77.243") = 1  OR inStr(remoteAddr,"192.168.77.243") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://192.168.77.244") = 1  OR inStr(remoteAddr,"192.168.77.244") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(remoteAddr,"http://192.168.77.245") = 1  OR inStr(remoteAddr,"192.168.77.245") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(referrer,"http://dev-gradebook.bfwpub.com") = 1  OR inStr(referrer,"dev-gradebook.bfwpub.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(referrer,"http://dev-scorecard.bfwpub.com") = 1  OR inStr(referrer,"dev-scorecard.bfwpub.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(referrer,"http://stg-gradebook.bfwpub.com") = 1  OR inStr(referrer,"stg-gradebook.bfwpub.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(referrer,"http://stg-scorecard.bfwpub.com") = 1  OR inStr(referrer,"stg-scorecard.bfwpub.com") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(Request.ServerVariables("HTTP_HOST"),"dev-bcs.") = 1 then
	remoteAddrAuthenticated = true
elseif inStr(Request.ServerVariables("HTTP_HOST"),"stg-bcs.") = 1 then
	remoteAddrAuthenticated = true
end if

referrerAuthenticated = true
remoteAddrAuthenticated = true

dim RAWSRootDomain, RAXSRootURL, RALoginRefURL

RAXSRootURL = "http://"& Request.ServerVariables("HTTP_HOST") &"/RA/RAXS/v1"

RALoginRefURL = "http://"& Request.ServerVariables("HTTP_HOST") &"/login_reference/asp/c-ra.asp"
RALoginRefURL = "http://bcs.bfwpub.com/login_reference/asp/c-ra.asp"

if inStr(Request.ServerVariables("HTTP_HOST"),"192.168.77.114") > 0 then
	RAWSRootDomain = "192.168.77.242"
''	RAWSRootDomain = "bcs.bfwpub.com"
elseif inStr(Request.ServerVariables("HTTP_HOST"),"192.168.77.242") > 0 then
	RAWSRootDomain = "192.168.77.242"
''	RAWSRootDomain = "bcs.bfwpub.com"
elseif inStr(Request.ServerVariables("HTTP_HOST"),"192.168.77.243") > 0 then
	RAWSRootDomain = "192.168.77.242"
elseif inStr(Request.ServerVariables("HTTP_HOST"),"dev-bcs.") = 1 then
	RAWSRootDomain = "dev-raws.bfwpub.com"
elseif inStr(Request.ServerVariables("HTTP_HOST"),"stg-bcs.") = 1 then
	RAWSRootDomain = "stg-raws.bfwpub.com"
else
	RAWSRootDomain = "bcs.bfwpub.com"
end if

%>
