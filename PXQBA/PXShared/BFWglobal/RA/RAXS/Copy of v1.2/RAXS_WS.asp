<%
Response.buffer = true
'Response.contenttype = "text/html"
'
Response.contenttype = "text/xml"

dim RequestBinCt
dim RequestBin
dim RequestBinStr : RequestBinStr = ""

RequestBinCt=Request.TotalBytes
RequestBin=Request.BinaryRead(RequestBinCt)

str = ""
For i = 1 To Request.TotalBytes
'	j = (i - 1) Mod 16
'
	c = MidB(RequestBin,i,1)
	if AscB(c) < 32 Or AscB(c) > 255 Then str = str Else str = str & Chr(AscB(c))

'	If j >= 15 Then
		RequestBinStr = RequestBinStr & str
		str = ""
'	End If
Next

if false or Request.Querystring("debug") = "true" then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://tempuri.org/""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <RAXS_WS xmlns=""http://tempuri.org/"">" &_
"      <iUserID>4156129</iUserID>" &_
"    </RAXS_WS>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
end if

dim responseStr : responseStr = ""
dim sErrorMsg : sErrorMsg = ""

dim requestXML
set requestXML = Server.CreateObject("Microsoft.XMLDOM")
requestXML.async = "false"
requestXML.loadXML( RequestBinStr )

dim nRAXS_WS, nIUserID
set nRAXS_WS = requestXML.getElementsByTagName("RAXS_WS")
set nIUserID = requestXML.getElementsByTagName("iUserID")

dim in_iUserID

checkInput

if nIUserID.length = 1 then
	in_iUserID = CLng(nIUserID(0).text)
end if

sub checkInput ()
	if nRAXS_WS.length = 0 then
		sErrorMsg = "Invalid input: no RAXS_WS element"
		goToFinish
	end if

	if nIUserID.length < 1 then
		sErrorMsg = "Invalid input: missing elements"
		goToFinish
	end if
	if nIUserID.length > 1 then
		sErrorMsg = "Invalid input: duplicate elements"
		goToFinish
	end if

end sub



sub goToFinish ()
	responseStr = "<RAXS_WS><results><IP>[[IP HERE]]</IP>"& responseStr &"</results><sErrorMsg>"& sErrorMsg &"</sErrorMsg></RAXS_WS>"

	responseStr = "<?xml version=""1.0""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body>"& responseStr &"</soap:Body></soap:Envelope>"

	Response.Write responseStr

	response.end
end sub

responseStr = responseStr & "<iUserID>"& in_iUserID &"</iUserID>"

''goToFinish

%>
<!--#include file="./RAXS_server.asp"-->
<%

if not referrerAuthenticated and not remoteAddrAuthenticated then
	sErrorMsg = "invalid usage : \n"&VBCRLF
	sErrorMsg = sErrorMsg &"  referrer = "& Request.ServerVariables("HTTP_REFERER") &" \n"&VBCRLF
	sErrorMsg = sErrorMsg &"  remoteAddr = "& Request.ServerVariables("REMOTE_ADDR") &" \n"&VBCRLF
sErrorMsg = sErrorMsg & inStr("http://"& Request.ServerVariables("REMOTE_ADDR"),"192.168.77.114") &" \n"&VBCRLF
sErrorMsg = sErrorMsg & inStr(Request.ServerVariables("REMOTE_ADDR"),"192.168.77.114") &" \n"&VBCRLF
'	For Each strName in Request.ServerVariables
'	'We aren't dealing with an array, so just display the variable
'sErrorMsg = sErrorMsg & displayVar( strName, CStr(Request.ServerVariables(strName)) )
'	Next
	goToFinish
end if













dim iUserID : iUserID = in_iUserID

dim UseClassesSP : UseClassesSP = true
dim UseProfileSP : UseProfileSP = true

dim RADB_GetSiteLogins_THRESHOLD : RADB_GetSiteLogins_THRESHOLD = 1


dim RAUserRememberMe : RAUserRememberMe = Request.Querystring("rem")
dim cookieExpDate
if RAUserRememberMe <> "1" then
	RAUserRememberMe = "0"
else
	cookieExpDate = DateAdd("m",6,Now())
end if
debugWrite "x", cookieExpDate
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
	sErrorMsg = "no iUserID"
	goToFinish
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
responseStr = responseStr & "<udtUserProfile>"
responseStr = responseStr & "<iUserID>"& RACU_iUserID &"</iUserID>"
responseStr = responseStr & "<sUserName>"& RACU_sUserName &"</sUserName>"
responseStr = responseStr & "<sFirstName>"& RACU_sFirstName &"</sFirstName>"
responseStr = responseStr & "<sLastName>"& RACU_sLastName &"</sLastName>"
responseStr = responseStr & "<sPasswordHint>"& RACU_sPasswordHint &"</sPasswordHint>"
responseStr = responseStr & "<sMailPreference>"& RACU_sMailPreference &"</sMailPreference>"
responseStr = responseStr & "<sOptInEmail>"& RACU_sOptInEmail &"</sOptInEmail>"
responseStr = responseStr & "<bClassPrompt>"& RAUserClassPrompt &"</bClassPrompt>"
responseStr = responseStr & "</udtUserProfile>"

i = 0

if UseClassesSP then

else


debugWrite "x", "<br> ct = "
debugWrite "x", RAWS_udtClassInfo_ct &"<br>"
''dim xx

if RAWS_udtClassInfo_ct > 0 then
responseStr = responseStr & "<udtClass>"
end if
for i = 0 to RAWS_udtClassInfo_ct-1
'	xx = RAWS_udtClassInfo(i)
'
	if i > 0 then
responseStr = responseStr & "</udtClass><udtClass>"
	end if
debugWrite "x", i&" : "& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__iClassID)
debugWrite "x", "<br>"
	responseStr = responseStr & "<iClassID>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__iClassID) &"</iClassID>"

	responseStr = responseStr & "<iCreatorID>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__iCreatorID) &"</iCreatorID>"

	responseStr = responseStr & "<sClassName>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sClassName) &"</sClassName>"

	responseStr = responseStr & "<sClassDesc>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sClassDesc) &"</sClassDesc>"

'cc20081104 - streamline cookies saved
'	responseStr = responseStr & "<sClassCode>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sClassCode) &"</sClassCode>"

	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	responseStr = responseStr & "<dtExprn>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__dtExprn) &"</dtExprn>"

	responseStr = responseStr & "<iUserID>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__iUserID) &"</iUserID>"

	responseStr = responseStr & "<bClassAccessRevoked>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__bClassAccessRevoked) &"</bClassAccessRevoked>"

	responseStr = responseStr & "<dtLastLogin>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__dtLastLogin) &"</dtLastLogin>"

'cc20081104 - streamline cookies saved
'	responseStr = responseStr & "<dtStartDate>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__dtStartDate) &"</dtStartDate>"

'	responseStr = responseStr & "<dtEndDate>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__dtEndDate) &"</dtEndDate>"

	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	responseStr = responseStr & "<bEmailScores>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__bEmailScores) &"</bEmailScores>"

'cc20081104 - streamline cookies saved
'	responseStr = responseStr & "<iRecordStatus>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__iRecordStatus) &"</iRecordStatus>"

	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	responseStr = responseStr & "<sCreatorEmail>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sCreatorEmail) &"</sCreatorEmail>"

	responseStr = responseStr & "<sCreatorFName>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sCreatorFName) &"</sCreatorFName>"

	responseStr = responseStr & "<sCreatorLName>"& RAWS_udtClassInfo(i)(RAWS_udtClassInfo__sCreatorLName) &"</sCreatorLName>"

next
if RAWS_udtClassInfo_ct > 0 then
responseStr = responseStr & "</udtClass>"
end if

end if


goToFinish





''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
function displayVar( i_varname, i_var)
	dim xxx
	xxx = ""

	If IsArray( i_var ) then
'	'If it is an array, loop through each element one at a time
xxx = xxx &_
"" & i_varname & " = [ ARRAY ] \n" & vbCRLF
		dim iLoop
		For iLoop = LBound( i_var ) to UBound( i_var )

xxx = xxx &_
displayVar( i_varname & "(" & iLoop & ")" , i_var(iLoop) )

		Next
	Else
	'We aren't dealing with an array, so just display the variable
		If isObject( i_var ) Then

xxx = xxx &_
"" & i_varname & " = [ OBJECT, TYPE: " & TypeName(i_var) & " ] \n" & vbCRLF

		Else

xxx = xxx &_
"" & i_varname & " = [ "&TypeName(i_var)&" ]  " &_
i_var & " \n" & vbCRLF

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

debugWrite "x", replace(RAWS_Http.responsetext, "<?xml version=""1.0"" encoding=""utf-8""?>", "")
'response.end
'
	if RAWS_Http.responsetext = "" then
			set RAWS_Http = nothing
			set RAWS_XML = nothing
			EndOnError
	else
		RAWS_XML.loadXML(RAWS_Http.responsetext)
debugWrite "x", RAWS_XML.xml
debugWrite "x", RAWS_XML.getElementsByTagName("soap:Fault").length
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
debugWrite "x", "Error loading RAWS UserProfileResult XML :: "& xmlErr.reason &VBCRLF
'
		response.end
	else
		set resultNodes = resultXML.selectSingleNode("/udtUserProfile").childNodes
		dim iNode
		for each iNode in resultNodes
debugWrite "x", iNode.nodeName &" = "& iNode.text &VBCRLF
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
debugWrite "x", "Error loading RAWS GetClassResult XML :: "& xmlErr.reason &VBCRLF
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

debugWrite "strQuery",strQuery
'response.write strQuery
'response.end

	set rsObj = objConn.Execute(strQuery)

	If rsObj.EOF Then
debugWrite "x", "...no matching records found"
'
	Else
responseStr = responseStr & "<udtPackage>"
		while not rsObj.EOF
			RA_PackagesCt = RA_PackagesCt + 1
			if RA_PackagesCt > 1 then
responseStr = responseStr & "</udtPackage><udtPackage>"
			end if
responseStr = responseStr & "<iPackageID>"& rsObj("PackageID") &"</iPackageID>"
responseStr = responseStr & "<dExpiration>"& rsObj("Expiration") &"</dExpiration>"
			rsObj.moveNext
		Wend
responseStr = responseStr & "</udtPackage>"
	End If
	rsObj.close


debugWrite "x", "<br/>"
debugWrite "x", RA_PackagesCt-1
debugWrite "x", " : "
debugWrite "x", RA_Packages_str
debugWrite "x", "<br/>"

'

end sub




'' ********************************************************************


sub RADB_GetClasses ()


	RA_ClassCt = 0

	strQuery = "EXEC raxs_login_getUserClasses @userid='"& CLng(RACU_iUserID) &"'"

debugWrite "strQuery",strQuery
'response.write strQuery
'response.end

	set rsObj = objConn.Execute(strQuery)

	If rsObj.EOF Then
debugWrite "x", "...no matching records found"
'
	Else
responseStr = responseStr & "<udtClass>"
		while not rsObj.EOF
			RA_ClassCt = RA_ClassCt + 1
			if RA_ClassCt > 1 then
responseStr = responseStr & "</udtClass><udtClass>"
			end if

	responseStr = responseStr & "<iClassID>"& rsObj("ClassID") &"</iClassID>"
	responseStr = responseStr & "<iCreatorID>"& rsObj("CreatorID") &"</iCreatorID>"
'cc20090211 - streamline cookies saved
'	responseStr = responseStr & "<sClassName>"& rsObj("ClassName") &"</sClassName>"
'	responseStr = responseStr & "<sClassDesc>"& rsObj("ClassDesc") &"</sClassDesc>"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	responseStr = responseStr & "<sClassCode>"& rsObj("Code") &"</sClassCode>"
'cc20090211 - streamline cookies saved
'	responseStr = responseStr & "<dtExprn>"& rsObj("Exprn") &"</dtExprn>"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	responseStr = responseStr & "<iUserID>"& rsObj("UserID") &"</iUserID>"
'cc20090211 - streamline cookies saved
'	responseStr = responseStr & "<bClassAccessRevoked>"& rsObj("ClassAccessRevoked") &"</bClassAccessRevoked>"
'	responseStr = responseStr & "<dtLastLogin>"& rsObj("LastLogin") &"</dtLastLogin>"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
'cc20081104 - streamline cookies saved
'	responseStr = responseStr & "<dtStartDate>"& rsObj("StartDate") &"</dtStartDate>"
'	responseStr = responseStr & "<dtEndDate>"& rsObj("EndDate") &"</dtEndDate>"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
'cc20090211 - streamline cookies saved
'	responseStr = responseStr & "<bEmailScores>"& rsObj("EmailScores") &"</bEmailScores>"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
'cc20081104 - streamline cookies saved
'	responseStr = responseStr & "<iRecordStatus>"& rsObj("RecordStatus") &"</iRecordStatus>"
	RAc_Classes_str = RAc_Classes_str & "'-'" &"|||"
	responseStr = responseStr & "<sCreatorEmail>"& rsObj("CreatorEmail") &"</sCreatorEmail>"
	responseStr = responseStr & "<sCreatorFName>"& rsObj("CreatorFName") &"</sCreatorFName>"
	responseStr = responseStr & "<sCreatorLName>"& rsObj("CreatorLName") &"</sCreatorLName>"



			rsObj.moveNext
		Wend
responseStr = responseStr & "</udtClass>"
	End If
	rsObj.close


debugWrite "x", "<br/>"
debugWrite "x", RA_ClassCt-1
debugWrite "x", " : "
debugWrite "x", RAc_Classes_str
debugWrite "x", "<br/>"

'

end sub




'' ********************************************************************


sub RADB_GetProfile ()


	RA_UserCt = 0

	strQuery = "EXEC raxs_login_getUserProfile @userid='"& CLng(iUserID) &"'"

debugWrite "strQuery",strQuery
'response.write strQuery
'response.end

	set rsObj = objConn.Execute(strQuery)

	If rsObj.EOF Then
debugWrite "x", "...no matching records found"
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


debugWrite "x", "<br/>"
debugWrite "x", RA_UserCt-1
debugWrite "x", "<br/>"

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

