<%
Response.buffer = true
'Response.contenttype = "text/html"
'
Response.contenttype = "text/xml"

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'INPUT FORMAT FOR CHECKING:
'<XSSession>
'	<Check>
'		<iGUID>[GUID]</iGUID>
'	</Check>
'</XSSession>
'
'INPUT FORMAT FOR SETTING:
'<XSSession>
'	<Set>
'		<iUserID>[UserID]</iUserID>
'	</Set>
'</XSSession>
'
'OUTPUT FORMAT FOR CHECKING:
'<XSSession>
'	<CheckResult>
'		<iUserID>[UserID]</iUserID>
'	</Check>
'	<sErrorMsg></sErrorMsg>
'</XSSession>
'
'OUTPUT FORMAT FOR SETTING:
'<XSSession>
'	<SetResult>
'		<iGUID>[GUID]</iGUID>
'	</SetResult>
'	<sErrorMsg></sErrorMsg>
'</XSSession>
'

dim debug : debug = -1
'debug = 3
'

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
dim RequestBinCt
dim RequestBin
dim RequestBinStr : RequestBinStr = ""

RequestBinCt=Request.TotalBytes
RequestBin=Request.BinaryRead(RequestBinCt)

dim str : str = ""
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

setDebugInput

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
dim resultXML, sXSSessionResult, sErrorMsg

set resultXML = Server.CreateObject("Microsoft.XMLDOM")
resultXML.async = "false"
resultXML.loadXML( RequestBinStr )

dim nGet, nSet, nCheck, nGUID, nUserID
dim mode : mode = "check"
dim i_GUID, i_UserID, o_GUID, o_UserID

set nGet = resultXML.getElementsByTagName("XSSession")
set nCheck = resultXML.getElementsByTagName("Check")
set nGUID = resultXML.getElementsByTagName("iGUID")
set nSet = resultXML.getElementsByTagName("Set")
set nUserID = resultXML.getElementsByTagName("iUserID")

checkInput


''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
dim objConn
dim rsObj
%>
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
<%

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
dim AMPM
if Hour(Now())<7 then
	AMPM = "AM"
	if inStr(Now(),"PM") > 0 then
		AMPM = "PM"
	end if
else
	AMPM = "PM"
	if inStr(Now(),"PM") > 0 then
		AMPM = "AM"
	end if
end if
dim SessionUTCDate : SessionUTCDate = Month(Now()) &"/"& Day(Now()) &"/"& Year(Now()) &" "& Hour(Now())-7 &":"& Minute(Now()) &":00 "& AMPM
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
if mode = "check" then

	RADB_CheckSession i_GUID
	goToFinish

elseif mode = "set" then

	RADB_SetSession i_UserID
	goToFinish

else
	sErrorMsg = "Fault error: no mode"
	goToFinish
end if




''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub RADB_CheckSession ( i_GUID )

	ConnectToBFWUsersDB
	CheckForErrors(objConn)

	strQuery = "EXEC raxs_check @guid='"& CLng(i_GUID) &"'"
'	strQuery = "Select TOP 1 UserID from tblSession " & _
'			"where Date >= '" & SessionUTCDate & "'" & _
'			"order by Date desc, GUID desc"
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		o_UserID = ""
	Else
		o_UserID = rsObj("UserID")
	End If

	rsObj.close
	set rsObj = Nothing
	set objConn = Nothing

	sXSSessionResult = "<iUserID>"& o_UserID &"</iUserID>"

end sub

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub RADB_SetSession ( i_UserID )

	dim SessionFound : SessionFound = FALSE

	ConnectToBFWUsersDB
	CheckForErrors(objConn)

	strQuery = "EXEC raxs_login_getUserProfile @userid='"& CLng(i_UserID) &"'"
'	strQuery = "Select top 1 GUID from tblSession " & _
'			"where Date >= '" & SessionUTCDate & "' " & _
'			"and UserID = "& i_UserID &" " & _
'			"order by Date desc, GUID desc"

	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn

	If rsObj.EOF Then
		o_GUID = ""
	Else
		o_GUID = rsObj("GUID")
	End If

	rsObj.close

	sXSSessionResult = "<iGUID>"& o_GUID &"</iGUID>"

	set rsObj = Nothing
	set objConn = Nothing

end sub

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub checkInput ()
	if nGet.length = 0 then
		sErrorMsg = "Invalid input: no XSSession element"
		goToFinish
	end if

	if nCheck.length = 0 AND nSet.length = 0 then
		sErrorMsg = "Invalid input: no Check/Set element"
		goToFinish
	end if

	if (nCheck.length > 0 AND nSet.length > 0) OR nCheck.length > 1 OR nSet.length > 1 then
		sErrorMsg = "Invalid input: multiple Check/Set elements"
		goToFinish
	end if

	if nSet.length > 0 then
		mode = "set"
		if nUserID.length <> 1 then
			sErrorMsg = "Invalid input: UserID element"
			goToFinish
		else
			i_UserID = nUserID(0).text
		end if
	else
		mode = "check"
		if nGUID.length <> 1 then
			sErrorMsg = "Invalid input: GUID element"
			goToFinish
		else
			i_GUID = nGUID(0).text
		end if
	end if

end sub

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub goToFinish ()
	dim ResponseStr
	if mode = "set" then
		ResponseStr = "<SetResult>"& sXSSessionResult &"</SetResult><sErrorMsg>"& sErrorMsg &"</sErrorMsg>"
	else
		ResponseStr = "<CheckResult>"& sXSSessionResult &"</CheckResult><sErrorMsg>"& sErrorMsg &"</sErrorMsg>"
	end if
	ResponseStr = "<XSSession>"& ResponseStr &"</XSSession>"
	ResponseStr = "<?xml version=""1.0""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body>"& ResponseStr &"</soap:Body></soap:Envelope>"
	response.write ResponseStr
	response.end
end sub

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub setDebugInput ()

	if debug = 1 then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"<XSSession>"&_
"	<Check>"&_
"		<iGUID>1</iGUID>"&_
"	</Check>"&_
"</XSSession>"&_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""

	elseif debug = 2 then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"<XSSession>"&_
"	<Set>"&_
"		<iUserID>118</iUserID>"&_
"	</Set>"&_
"</XSSession>"&_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""

	elseif debug = 3 then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"<XSSession>"&_
"	<Set>"&_
"		<iUserID>152</iUserID>"&_
"	</Set>"&_
"</XSSession>"&_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""

	else
	end if
'
'INPUT FORMAT FOR SETTING:
'<XSSession>
'	<Set>
'		<iUserID>[UserID]</iUserID>
'	</Set>
'</XSSession>
end sub




%>

