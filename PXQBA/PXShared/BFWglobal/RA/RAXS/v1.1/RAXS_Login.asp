<% Response.Buffer = True
Response.contenttype = "text/html"

'response.write "er?"
'response.end

%>
<!--#include virtual="/RA/RAXS/v1/RAXS_server.asp"-->
<%

dim UseClassesSP : UseClassesSP = true
dim UseProfileSP : UseProfileSP = true

dim RADB_GetSiteLogins_THRESHOLD : RADB_GetSiteLogins_THRESHOLD = 1

dim iUserID : iUserID = Request.Querystring("uid")
dim iSiteID : iSiteID = Request.Querystring("sid")
dim sPackageIDs : sPackageIDs = Request.Querystring("pids")
dim returl : returl = Request.Querystring("returl")
returl = "http://"& replace(returl,"http://","")
if returl = "http://" then
	returl = ""
end if

'if 1 then
'response.write "uid = "& iUserID
'response.write "<br/>"
'response.write "returl = "& returl
'response.write "<br/>"
'response.end
'end if

if 0 then
	iUserID = 118
	iSiteID = 24382
	sPackageIDs = "663,362"
end if

if returl = "" then
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
document.getElementById('msg').innerHTML = 'logging in...';
var cookiesblocked = !navigator.cookieEnabled;
if (cookiesblocked) {
	document.getElementById('msg').innerHTML = '<p>Cookies must be enabled.</p>';
}

}
</script>
</head>
<!--
<body>
-->
<body onload="">
<div id="msg">logging in...</div>
<script type="text/javascript">
//document.write( navigator.cookieEnabled );
</script>
<!--
<br/>
global RAXS_set.asp?uid=<%=iUserID%>
<br/>
Request.Cookies("RAUserID") = <%=Request.Cookies("RAUserID")%>
<br/>
Request.Cookies("RAUserProfile") = <%=Request.Cookies("RAUserProfile")%>
<br/>
returl=<%=returl%>
<br/>
<%
debugvars_HTML_Cookie
%>
-->
</body>
</html>
<%
end if

if Request.Querystring("rau")=iUserID AND iUserID=Request.Cookies("RAUserID") then
	if returl = "" then

%>
		<script language="JavaScript">setTimeout('window.close()',50)</script>
<%
	else
'response.write "test 1: "& returl
'response.end
		response.redirect returl
	end if
	response.end
end if


dim RAUserRememberMe : RAUserRememberMe = Request.Querystring("rem")
dim cookieExpDate
if RAUserRememberMe <> "1" then
	RAUserRememberMe = "0"
else
	cookieExpDate = DateAdd("m",6,Now())
end if
'debugWrite "x", cookieExpDate
'
'response.end

dim RAUserClassPrompt : RAUserClassPrompt = Request.Querystring("cp")
if RAUserClassPrompt <> "0" then
	RAUserClassPrompt = "1"
end if
RAUserClassPrompt = "1"

dim RAUserProfile_str
dim RA_arrUP()

dim RA_Packages_str : RA_Packages_str = ""
dim RA_PackagesCt : RA_PackagesCt = 0

dim RAc_Classes_str : RAc_Classes_str = ""
dim RA_ClassIDs_str : RA_ClassIDs_str = ""
dim RAWS_udtClassInfo_ct : RAWS_udtClassInfo_ct = 0
dim RAWS_udtClassInfo()
const RAWS_udtClassInfo__iClassID = 0
const RAWS_udtClassInfo__iCreatorID = 1
const RAWS_udtClassInfo__sClassName = 2
const RAWS_udtClassInfo__sClassDesc = 3
const RAWS_udtClassInfo__sClassCode = 4
const RAWS_udtClassInfo__dtExprn = 5
const RAWS_udtClassInfo__iUserID = 6
const RAWS_udtClassInfo__bClassAccessRevoked = 7
const RAWS_udtClassInfo__dtLastLogin = 8
const RAWS_udtClassInfo__dtStartDate = 9
const RAWS_udtClassInfo__dtEndDate = 10
const RAWS_udtClassInfo__bEmailScores = 11
const RAWS_udtClassInfo__iRecordStatus = 12
const RAWS_udtClassInfo__sCreatorEmail = 13
const RAWS_udtClassInfo__sCreatorFName = 14
const RAWS_udtClassInfo__sCreatorLName = 15

if not( iUserID > 0 ) then
	response.end
end if

dim RAWS_iUserID, RAWS_sUserName, RAWS_sFirstName, RAWS_sLastName, RAWS_sPasswordHint, RAWS_sMailPreference, RAWS_sOptInEmail
dim RACU_iUserID, RACU_sUserName, RACU_sFirstName, RACU_sLastName, RACU_sPasswordHint, RACU_sMailPreference, RACU_sOptInEmail

if Request.Querystring("fake") = "true" then

	RAWS_iUserID = iUserID
	RAWS_sUserName = "chad@s.e"
	RAWS_sFirstName = "Chad"
	RAWS_sLastName = "Test"
	RAWS_sPasswordHint = ""
	RAWS_sMailPreference = ""
	RAWS_sOptInEmail = "0"
	RAUserRememberMe = "0"
	RAUserClassPrompt = "0"

else

	dim RAWS_XML, resultXML
	set RAWS_XML = Server.CreateObject("Microsoft.XMLDOM")
	set resultXML = Server.CreateObject("Microsoft.XMLDOM")
%>
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<%
	dim objConn
	dim rsObj
	dim strQuery
	set rsObj = Server.CreateObject("ADODB.Recordset")
	ConnectToBFWUsersDB
	CheckForErrors(objConn)

	dim strEnvelope



if UseProfileSP then

RADB_GetProfile

else

	strEnvelope = ""&_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"""&_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"""&_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">"&_
"  <soap:Body>"&_
"	<UserProfile xmlns=""http://tempuri.org/"">"&_
"		<iUserID>"&iUserID&"</iUserID>"&_
"		<sBaseUrl/> "&_
"	</UserProfile>"&_
"  </soap:Body>"&_
"</soap:Envelope>"&_
""
	RAWS_Http_Request "UserProfile", strEnvelope
	RAWS_UserProfile_PROCESS
	RACU_iUserID = RAWS_iUserID
	RACU_sUserName = RAWS_sUserName
	RACU_sFirstName = RAWS_sFirstName
	RACU_sLastName = RAWS_sLastName
	RACU_sPasswordHint = RAWS_sPasswordHint
	RACU_sMailPreference = RAWS_sMailPreference
	RACU_sOptInEmail = RAWS_sOptInEmail

end if

if UseClassesSP then

RADB_GetClasses

else

	strEnvelope = ""&_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"""&_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"""&_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">"&_
"  <soap:Body>"&_
"	<GetClass xmlns=""http://tempuri.org/"">" &_
"		<iUserID>"&iUserID&"</iUserID>" &_
"	</GetClass>" &_
"  </soap:Body>"&_
"</soap:Envelope>"&_
""
	RAWS_Http_Request "GetClass", strEnvelope
	RAWS_GetClass_PROCESS

	dim jjj
	for jjj = 0 to RAWS_udtClassInfo_ct-1
debugWrite "classcreator", jjj &" ============"
		strEnvelope = ""&_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"""&_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"""&_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">"&_
"  <soap:Body>"&_
"	<UserProfile xmlns=""http://tempuri.org/"">"&_
"		<iUserID>"&RAWS_udtClassInfo(jjj)(RAWS_udtClassInfo__iCreatorID)&"</iUserID>"&_
"		<sBaseUrl/> "&_
"	</UserProfile>"&_
"  </soap:Body>"&_
"</soap:Envelope>"&_
""
debugWrite "classcreator", strEnvelope
'response.end
		RAWS_Http_Request "UserProfile", strEnvelope
		RAWS_UserProfile_PROCESS
		RAWS_udtClassInfo(jjj)(RAWS_udtClassInfo__sCreatorEmail) = RAWS_sUserName
		RAWS_udtClassInfo(jjj)(RAWS_udtClassInfo__sCreatorFName) = RAWS_sFirstName
		RAWS_udtClassInfo(jjj)(RAWS_udtClassInfo__sCreatorLName) = RAWS_sLastName
	next
'response.end

end if


	RADB_GetPackages

	set RAWS_XML = nothing
	set resultXML = nothing
	set rsObj = Nothing
	objConn.close
	set objConn = nothing

end if
RAUserProfile_str = ""
RAUserProfile_str = RAUserProfile_str& RACU_iUserID
RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sUserName
RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sFirstName
RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sLastName
RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sPasswordHint
RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sMailPreference
RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sOptInEmail
RAUserProfile_str = RAUserProfile_str&"|||"& RAUserClassPrompt
Response.Cookies("RAUserID") = RACU_iUserID
Response.Cookies("RAUserProfile") = RAUserProfile_str
''Response.Cookies("RAUserRememberMe") = RAUserRememberMe
if RAUserRememberMe="1" then
	Response.Cookies("RAUserID").Expires = cookieExpDate
	Response.Cookies("RAUserProfile").Expires = cookieExpDate
end if

dim i : i = 0

Response.Cookies("RAUserPkgs") = RA_Packages_str
if RAUserRememberMe="1" then
	Response.Cookies("RAUserPkgs").Expires = cookieExpDate
end if

Response.Cookies("RAUserClassIDs") = RA_ClassIDs_str
if RAUserRememberMe="1" then
	Response.Cookies("RAUserClassIDs").Expires = cookieExpDate
end if
'debugWrite "x", "<br> str = "
'debugWrite "x", RA_ClassIDs_str
'debugWrite "x", "<br>"
'

if UseClassesSP then

else


'debugWrite "x", "<br> ct = "
'debugWrite "x", RAWS_udtClassInfo_ct &"<br>"
''dim xx
for i = 0 to RAWS_udtClassInfo_ct-1
'	xx = RAWS_udtClassInfo(i)
'
	if i > 0 then
		RAc_Classes_str = RAc_Classes_str &"|RA|"
	end if
'debugWrite "x", i&" : "& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__iClassID)
'debugWrite "x", "<br>"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__iClassID) &"|||"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__iCreatorID) &"|||"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sClassName) &"|||"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sClassDesc) &"|||"
'cc20081104 - streamline cookies saved
'	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sClassCode) &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__dtExprn) &"|||"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__iUserID) &"|||"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__bClassAccessRevoked) &"|||"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__dtLastLogin) &"|||"
'cc20081104 - streamline cookies saved
'	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__dtStartDate) &"|||"
'	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__dtEndDate) &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__bEmailScores) &"|||"
'cc20081104 - streamline cookies saved
'	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__iRecordStatus) &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sCreatorEmail) &"|||"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sCreatorFName) &"|||"
	RAc_Classes_str = RAc_Classes_str & RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sCreatorLName)
next

end if

Response.Cookies("RAUserClasses") = RAc_Classes_str
if RAUserRememberMe="1" then
	Response.Cookies("RAUserClasses").Expires = cookieExpDate
end if
'debugWrite "x", RAc_Classes_str
'debugWrite "x", "<br>"
'
'response.end



if 1 then
if Request.Querystring("rau")<>iUserID then
	if returl<>"" then
		response.redirect RALoginRefURL &"?m=i&returl="&Server.URLEncode( RAXSRootURL &"/RAXS_Login.asp?sid="&iSiteID&"&pids="&sPackageIDs&"&returl="&Server.URLEncode( returl )&"")&"&u="&iUserID
	else
		response.redirect RALoginRefURL &"?m=i&returl="&Server.URLEncode( RAXSRootURL &"/RAXS_Login.asp?sid="&iSiteID&"&pids="&sPackageIDs&"")&"&u="&iUserID
	end if
	response.write "test 2: <br/>"
	response.write Request.Querystring("rau") &"<br/>"
	response.write iUserID &"<br/>"
	response.write returl &"<br/>"
	if returl<>"" then
		response.write RALoginRefURL &"?m=i&returl="&Server.URLEncode( RAXSRootURL &"/RAXS_Login.asp?sid="&iSiteID&"&pids="&sPackageIDs&"&returl="&Server.URLEncode( returl )&"")&"&u="&iUserID
	else
		response.write RALoginRefURL &"?m=i&returl="&Server.URLEncode( RAXSRootURL &"/RAXS_Login.asp?sid="&iSiteID&"&pids="&sPackageIDs&"")&"&u="&iUserID
	end if
	response.end
end if
end if




''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub debugvars_HTML_Cookie()
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	dim strName, iLoop
   'COOKIE VARS
Response.Write "    <TABLE WIDTH='100%' BORDER='2' CELLSPACING='0' CELLPADDING='2'>" & vbCRLF
   Response.Write "<TR><TD COLSPAN='2' CLASS='subhead'>Client Cookies -- count: " & Request.Cookies.Count & "</TD></TR>" & vbCRLF

	for iloop = 1 to Request.Cookies.Count
		strName = Request.Cookies.Key(iloop)
Response.Write displayVar( iloop & ": " & strName, CStr(Request.Cookies.Item(strName)) )
'Response.Write "<TR><TD>" & iloop & ": " & strName & "</TD><TD>" & Request.Cookies.Item(strName) & "</TD></TR>" & vbCRLF
	next
'       Response.Write "<TR><TD>USERID</TD><TD>" & Request.Cookies("RAUserID") & "</TD></TR>" & vbCRLF
Response.Write "    </TABLE>" & vbCRLF
end sub
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
function displayVar( i_varname, i_var)
	dim xxx
	xxx = ""

	If IsArray( i_var ) then
'	'If it is an array, loop through each element one at a time
xxx = xxx &_
"<TR><TD>" & i_varname & "</TD><TD><b>[ ARRAY ]</b></TD></TR>" & vbCRLF
		dim iLoop
		For iLoop = LBound( i_var ) to UBound( i_var )

xxx = xxx &_
displayVar( i_varname & "(" & iLoop & ")" , i_var(iLoop) )

		Next
	Else
	'We aren't dealing with an array, so just display the variable
		If isObject( i_var ) Then

xxx = xxx &_
"<TR><TD>" & i_varname & "</TD><TD><b>[ OBJECT, TYPE: " & TypeName(i_var) & " ]</b></TD></TR>" & vbCRLF

		Else

xxx = xxx &_
"<TR><TD>" & i_varname & "</TD><TD><b>[ "&TypeName(i_var)&" ]</b> " &_
i_var & "</TD></TR>" & vbCRLF

		End If
	End If

	displayVar = xxx

end function




sub EndOnError ()
	set RAWS_XML = nothing
	set resultXML = nothing
	set rsObj = Nothing
	objConn.close
	set objConn = nothing
	if returl<>"" then
		response.redirect returl
		response.end

	else
%>
<html>
<head>
<script language="JavaScript"></script>
<style>
body {
	font-family: Arial, sans-serif;
}
</style>
</head>
<!--
<body>
-->
<body onload="window.opener.focus();setTimeout('window.close()',3000)">
An error occurred while logging you in.
</body>
</html>
<%
	end if
	response.end
end sub



sub RAWS_Http_Request (wsF, strEnvelope)
	Dim RAWS_Http
	Set RAWS_Http = Server.CreateObject("Microsoft.XMLHTTP")
	dim wsURL : wsURL = "http://"& RAWSRootDomain &"/ra_webservices/webservice1/service1.asmx?wsdl"
	RAWS_Http.Open "post", wsURL, false
	RAWS_Http.setRequestHeader "Soapaction", "http://tempuri.org/"& wsF
'	RAWS_Http.setRequestHeader "Content-Type", "text/xml; charset=utf-8"
'	RAWS_Http.setRequestHeader "Soapaction", "text/xml"
	RAWS_Http.setRequestHeader "Content-Type", "text/xml"
	RAWS_Http.Send strEnvelope

'debugWrite "x", replace(RAWS_Http.responsetext, "<?xml version=""1.0"" encoding=""utf-8""?>", "")
'response.end

	if RAWS_Http.responsetext = "" then
			set RAWS_Http = nothing
			set RAWS_XML = nothing
			EndOnError
	else
		RAWS_XML.loadXML(RAWS_Http.responsetext)
'debugWrite "x", RAWS_XML.xml
'debugWrite "x", RAWS_XML.getElementsByTagName("soap:Fault").length
		if RAWS_XML.getElementsByTagName("soap:Fault").length > 0 then
			set RAWS_Http = nothing
			EndOnError
		else
		end if
	end if

	set RAWS_Http = nothing
end sub




'' ********************************************************************


sub RAWS_UserProfile_PROCESS ()
	RAWS_iUserID = ""
	RAWS_sUserName = ""
	RAWS_sPassword = ""
	RAWS_sFirstName = ""
	RAWS_sLastName = ""
	RAWS_sPasswordHint = ""
	RAWS_sMailPreference = ""
	RAWS_sOptInEmail = ""
	dim x, y
	set x = RAWS_XML.selectSingleNode("/soap:Envelope/soap:Body/UserProfileResponse/UserProfileResult")
	y = x.text
debugWrite "classcreator", y

	resultXML.loadXML(y)
	dim xmlErr, resultNodes
	if resultXML.parseError.errorCode <> 0 then
		xmlErr = RAWS_XML.parseError
'debugWrite "x", "Error loading RAWS UserProfileResult XML :: "& xmlErr.reason &VBCRLF
'
		response.end
	else
		set resultNodes = resultXML.selectSingleNode("/udtUserProfile").childNodes
		dim iNode
		for each iNode in resultNodes
'debugWrite "x", iNode.nodeName &" = "& iNode.text &VBCRLF
'
			select case iNode.nodeName
				case "iUserID"
					RAWS_iUserID = iNode.text
				case "sUserName"
					RAWS_sUserName = iNode.text
				case "sPassword"
					RAWS_sPassword = iNode.text
				case "sFirstName"
					RAWS_sFirstName = iNode.text
				case "sLastName"
					RAWS_sLastName = iNode.text
				case "sPasswordHint"
					RAWS_sPasswordHint = iNode.text
				case "sMailPreference"
					RAWS_sMailPreference = iNode.text
				case "sOptInEmail"
					RAWS_sOptInEmail = iNode.text
				case else
			end select
		next

		set resultNodes = nothing
		set x = nothing
	end if

end sub




'' ********************************************************************


sub RAWS_GetClass_PROCESS ()
	dim x, y
	set x = RAWS_XML.selectSingleNode("/soap:Envelope/soap:Body/GetClassResponse/GetClassResult")
	y = x.text

	resultXML.loadXML(y)
	dim xmlErr, resultNodes
	if resultXML.parseError.errorCode <> 0 then
		xmlErr = RAWS_XML.parseError
'debugWrite "x", "Error loading RAWS GetClassResult XML :: "& xmlErr.reason &VBCRLF
'
		response.end
	else
		set resultNodes = resultXML.selectSingleNode("/ArrayOfUdtClassInfo").childNodes
		dim iNode, iNode2, arr(), arrXct
		for each iNode in resultNodes

			RAWS_udtClassInfo_ct = RAWS_udtClassInfo_ct + 1

			Redim Preserve RAWS_udtClassInfo(RAWS_udtClassInfo_ct)

			arrXct = 0
			redim arrX(16)
			for each iNode2 in iNode.childNodes
				arrXct = arrXct + 1

				select case iNode2.nodeName
					case "iClassID"
						arrX(RAWS_udtClassInfo__iClassID) = iNode2.text
						if 0 = inStr( ","&RA_ClassIDs_str&","  ,  ","&iNode2.text&","  ) then
							if RAWS_udtClassInfo_ct > 1 then
								RA_ClassIDs_str = RA_ClassIDs_str & ","
							end if
							RA_ClassIDs_str = RA_ClassIDs_str & iNode2.text
						end if
					case "iCreatorID"
						arrX(RAWS_udtClassInfo__iCreatorID) = iNode2.text
					case "sClassName"
						arrX(RAWS_udtClassInfo__sClassName) = iNode2.text
					case "sClassDesc"
						arrX(RAWS_udtClassInfo__sClassDesc) = iNode2.text
					case "sClassCode"
						arrX(RAWS_udtClassInfo__sClassCode) = iNode2.text
					case "dtExprn"
						arrX(RAWS_udtClassInfo__dtExprn) = iNode2.text
					case "iUserID"
						arrX(RAWS_udtClassInfo__iUserID) = iNode2.text
					case "bClassAccessRevoked"
						arrX(RAWS_udtClassInfo__bClassAccessRevoked) = iNode2.text
					case "dtLastLogin"
						arrX(RAWS_udtClassInfo__dtLastLogin) = iNode2.text
					case "dtStartDate"
						arrX(RAWS_udtClassInfo__dtStartDate) = iNode2.text
					case "dtEndDate"
						arrX(RAWS_udtClassInfo__dtEndDate) = iNode2.text
					case "bEmailScores"
						arrX(RAWS_udtClassInfo__bEmailScores) = iNode2.text
					case "iRecordStatus"
						arrX(RAWS_udtClassInfo__iRecordStatus) = iNode2.text
					case "iRecordStatus"
						arrX(RAWS_udtClassInfo__iRecordStatus) = iNode2.text
					case "iRecordStatus"
						arrX(RAWS_udtClassInfo__iRecordStatus) = iNode2.text
					case "iRecordStatus"
						arrX(RAWS_udtClassInfo__iRecordStatus) = iNode2.text
					case else
				end select

			next

			RAWS_udtClassInfo(RAWS_udtClassInfo_ct-1) = arrX

		next

		set resultNodes = nothing
		set x = nothing
	end if

end sub




'' ********************************************************************


sub RADB_GetPackages ()


	RA_PackagesCt = 0

	strQuery = "EXEC raxs_login_getUserPackages @userid='"& CLng(RACU_iUserID) &"'"

'debugWrite "strQuery",strQuery
'response.write strQuery
'response.end

	set rsObj = objConn.Execute(strQuery)

	If rsObj.EOF Then
'debugWrite "x", "...no matching records found"
'
	Else
		while not rsObj.EOF
			RA_PackagesCt = RA_PackagesCt + 1
			if RA_PackagesCt > 1 then
				RA_Packages_str = RA_Packages_str & "|RA|"
			end if
			RA_Packages_str = RA_Packages_str & rsObj("PackageID") &"|||"& rsObj("Expiration")
			rsObj.moveNext
		Wend
	End If
	rsObj.close


'debugWrite "x", "<br/>"
'debugWrite "x", RA_PackagesCt-1
'debugWrite "x", " : "
'debugWrite "x", RA_Packages_str
'debugWrite "x", "<br/>"

'

end sub




'' ********************************************************************


sub RADB_GetClasses ()


	RA_ClassCt = 0

	strQuery = "EXEC raxs_login_getUserClasses @userid='"& CLng(RACU_iUserID) &"'"

'debugWrite "strQuery",strQuery
'response.write strQuery
'response.end

	set rsObj = objConn.Execute(strQuery)

	If rsObj.EOF Then
'debugWrite "x", "...no matching records found"
'
	Else
		while not rsObj.EOF
			RA_ClassCt = RA_ClassCt + 1
			if RA_ClassCt > 1 then
				RAc_Classes_str = RAc_Classes_str & "|RA|"
			end if

	RAc_Classes_str = RAc_Classes_str & rsObj("ClassID") &"|||"
	RAc_Classes_str = RAc_Classes_str & rsObj("CreatorID") &"|||"
'cc20090211 - streamline cookies saved
'	RAc_Classes_str = RAc_Classes_str & rsObj("ClassName") &"|||"
'	RAc_Classes_str = RAc_Classes_str & rsObj("ClassDesc") &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & rsObj("Code") &"|||"
'cc20090211 - streamline cookies saved
'	RAc_Classes_str = RAc_Classes_str & rsObj("Exprn") &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & rsObj("UserID") &"|||"
'cc20090211 - streamline cookies saved
'	RAc_Classes_str = RAc_Classes_str & rsObj("ClassAccessRevoked") &"|||"
'	RAc_Classes_str = RAc_Classes_str & rsObj("LastLogin") &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
'cc20081104 - streamline cookies saved
'	RAc_Classes_str = RAc_Classes_str & rsObj("StartDate") &"|||"
'	RAc_Classes_str = RAc_Classes_str & rsObj("EndDate") &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
'cc20090211 - streamline cookies saved
'	RAc_Classes_str = RAc_Classes_str & rsObj("EmailScores") &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
'cc20081104 - streamline cookies saved
'	RAc_Classes_str = RAc_Classes_str & rsObj("RecordStatus") &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & rsObj("CreatorEmail") &"|||"
	RAc_Classes_str = RAc_Classes_str & rsObj("CreatorFName") &"|||"
	RAc_Classes_str = RAc_Classes_str & rsObj("CreatorLName")



			rsObj.moveNext
		Wend
	End If
	rsObj.close


'debugWrite "x", "<br/>"
'debugWrite "x", RA_ClassCt-1
'debugWrite "x", " : "
'debugWrite "x", RAc_Classes_str
'debugWrite "x", "<br/>"

'

end sub




'' ********************************************************************


sub RADB_GetProfile ()


	RA_UserCt = 0

	strQuery = "EXEC raxs_login_getUserProfile @userid='"& CLng(iUserID) &"'"

'debugWrite "strQuery",strQuery
'response.write strQuery
'response.end

	set rsObj = objConn.Execute(strQuery)

	If rsObj.EOF Then
'debugWrite "x", "...no matching records found"
'
	Else
		while not rsObj.EOF
			RA_UserCt = RA_UserCt + 1
			if RA_UserCt = 1 then

				RACU_iUserID = rsObj("UserID")
				RACU_sUserName = rsObj("UserEmail")
				RACU_sFirstName = rsObj("FirstName")
				RACU_sLastName = rsObj("LastName")
				RACU_sPasswordHint = rsObj("PasswordHint")
				RACU_sMailPreference = rsObj("MailPreferences")
				RACU_sOptInEmail = rsObj("OptInEmail")

			end if



			rsObj.moveNext
		Wend
	End If
	rsObj.close


'debugWrite "x", "<br/>"
'debugWrite "x", RA_UserCt-1
'debugWrite "x", "<br/>"

'

end sub








' *********************************************************************************
' *********************************************************************************
' *********************************************************************************
' *********************************************************************************


sub debugWrite (vs, msg)
'	response.write msg
'	response.write "<br/>"
end sub

%>

