<%
Response.buffer = true
Response.contenttype = "text/html"
'
'Response.contenttype = "text/xml"

dim testInput : testInput = Request.QueryString("testInput")

dim i, str

dim RequestBinCt
dim RequestBin
dim RequestBinStr : RequestBinStr = ""

dim sCheckActivationCodeResult : sCheckActivationCodeResult = ""
dim sErrorMsg : sErrorMsg = ""

dim resultXML
dim nGet, nPackages, nActivationCodes, nUserIDs, nSiteIDs
dim sPackageIDs, iSiteID, iUserID, sActivationCode
dim arrPackageIDs, arrSiteIDs, arrUserIDs

dim xNormalizedCode
dim xAccessID, xBatchID, xPackageID, xLevelID
dim xCodeUses, xBatchSuspended, xUseByDate, xRelExpire, xExpirationDate
dim xLevelOfAccess

dim objConn
dim rsObj
%>
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
<!--#include file="xx-Func-VerifyChecksum.asp"-->
<!--#include file="xx-Func-isValidAccFormat.asp"-->
<!--#include file="xx-Func-GetNormalized.asp"-->
<%
dim strQuery
dim RS_Package_ct
dim RS_lastPackageID
dim RS_Site_ct
dim RS_lastSiteID


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

if 0 then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <CheckActivationCode xmlns=""http://tempuri.org/"">" &_
"      <sActivationCode>r6-3aj-48c4dz6a</sActivationCode>" &_
"      <sPackageIDs>362,663</sPackageIDs>" &_
"      <iSiteID>24382</iSiteID>" &_
"      <iUserID>118</iUserID>" &_
"    </CheckActivationCode>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
elseif 0 then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <CheckActivationCode xmlns=""http://tempuri.org/"">" &_
"      <sActivationCode>r6-3aj-48c4dz6a</sActivationCode>" &_
"      <sPackageIDs>362,663</sPackageIDs>" &_
"      <iSiteID>24382</iSiteID>" &_
"      <iUserID>118</iUserID>" &_
"    </CheckActivationCode>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
end if


set resultXML = Server.CreateObject("Microsoft.XMLDOM")
resultXML.async = "false"
resultXML.loadXML( RequestBinStr )

set nGet = resultXML.getElementsByTagName("CheckActivationCode")
set nActivationCodes = resultXML.getElementsByTagName("sActivationCode")
set nSiteIDs = resultXML.getElementsByTagName("iSiteID")
set nUserIDs = resultXML.getElementsByTagName("iUserID")
set nPackages = resultXML.getElementsByTagName("sPackageIDs")

if nPackages.length = 1 then
	sPackageIDs = nPackages(0).text
	arrPackageIDs = split(sPackageIDs, ",")
end if
if nActivationCodes.length = 1 then
	sActivationCode = nActivationCodes(0).text
end if
if nSiteIDs.length = 1 then
	iSiteID = nSiteIDs(0).text
end if
if nUserIDs.length = 1 then
	iUserID = nUserIDs(0).text
else
	iUserID = ""
end if

checkInput

DB_CheckActivationCode

goToFinish




sub checkInput ()
	if nGet.length = 0 then
		sErrorMsg = "Invalid input: no CheckActivationCode element"
		goToFinish
	end if

	if nPackages.length = 0 or sPackageIDs ="" then
		sErrorMsg = "Invalid input: no sPackageIDs"
		goToFinish
	end if

	if nSiteIDs.length = 0 or iSiteID = "" then
		sErrorMsg = "Invalid input: no iSiteID"
		goToFinish
	end if

'	if nUserIDs.length = 0 or iUserID = "" then
'		sErrorMsg = "Invalid input: no iUserID"
'		goToFinish
'	end if

	if nActivationCodes.length = 0 or sActivationCode = "" then
		sErrorMsg = "Invalid input: no sActivationCode"
		goToFinish
	end if

	if lcase(sActivationCode) = "bfw41inst" then
		sActivationCode = "p-q-hyhce7"
	end if
end sub




sub goToFinish ()
	dim ResponseStr
	ResponseStr = "<CheckActivationCodeResponse><CheckActivationCodeResult>"& sCheckActivationCodeResult &"</CheckActivationCodeResult><sErrorMsg>"& sErrorMsg &"</sErrorMsg></CheckActivationCodeResponse>"
	ResponseStr = "<?xml version=""1.0""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body>"& ResponseStr &"</soap:Body></soap:Envelope>"
	response.write ResponseStr
	response.end
end sub




sub DB_CheckActivationCode ()
	set rsObj = Server.CreateObject("ADODB.Recordset")
	ConnectToBFWUsersDB
	CheckForErrors(objConn)


'	"e1" = "CodeInvalidFormat"
'	"e2" = "CodeFailsSumCheck"
'	"e3" = "CodeNotFound"
'	"e4" = "CodeNoUsesLeft"
'	"e5" = "BatchNotFound"
'	"e6" = "CodeUseByDateExpired"
'	"e7" = "CodeExpiredAbsolute"
'	"e8" = "CodeExpiredRelative"
'	"e9" = "CodeNotValidForSite"
'	"e9NEW" = "CodeNotValidForPackages"
'	"e10" = "LevelIDNotValid"
'	"e11" = "LevelIDNotFound"
'	"e12" = "CodeAlreadyAssignedToUser"
'PackageAlreadyAssignedToUser
'	"e13" = "NoSiteLoginsForUser"
'	"e14" = "CodeNotValidForAnySites"
'	"e15" = "BatchSuspended"
'	[level of access] = "success"

Call writelog("3", "iUserID     : " & iUserID)
Call writelog("3", "sActivationCode : " & sActivationCode)
Call writelog("3", "iSiteID     : " & iSiteID)

	' Normalize
	xNormalizedCode = getNormalized(sActivationCode)

	' Validate format
	if not isValidAccFormat(xNormalizedCode) then
		Call writelog("3", "failed isValidAccFormat")
		sErrorMsg = "CodeInvalidFormat"
		goToFinish
	end if

'get from accesscode: accessid, batchid, uses
	' Get Code data
	' Query the db with this checksum to narrow the search
	strQuery = "Select * from tblAccessCode " & _
			"where AccessCode='" & xNormalizedCode & "'"
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		sErrorMsg = "CodeNotFound"
	Else
		xAccessID = rsObj("AccessID")
		xBatchID = rsObj("BatchID")
		xCodeUses = CInt(rsObj("Uses"))
		xCreatedDate = CStr(rsObj("CreationDate"))
	End If
	rsObj.close
	set rsObj = Nothing
   	' If we didn't find it, exit
	if sErrorMsg = "CodeNotFound" then
		Call writelog("3", "Code: " & xNormalizedCode & " not found")
		goToFinish
	end if
	' Verify checksum.
Call writelog("3", "creation date compared to 12/4/2002:: " & xCreatedDate )
	if CDate(xCreatedDate) < CDate("12/4/2002") then
		if not verifyChecksumOLD(xNormalizedCode) then
			Call writelog("3", "failed verifyChecksumOLD")
			sErrorMsg = "CodeFailsSumCheck"
			goToFinish
		end if
	else
		if not verifyChecksum(xNormalizedCode) then
			Call writelog("3", "failed verifyChecksum")
			sErrorMsg = "CodeFailsSumCheck"
			goToFinish
		end if
	end if
	' If no uses left, exit
	if xCodeUses = 0 then
		sErrorMsg = "CodeNoUsesLeft"
		Call writelog("3", "Code: " & xNormalizedCode & " no uses left")
		goToFinish
	end if

'gets from batchid: packageid, useby, exp, suspended
	'--	Get PackageID from Batch
	' and xUseByDate and Expiration dates.
	strQuery = "Select * from tblBatchKey where BatchID=" & xBatchID
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		sErrorMsg = "BatchNotFound"
	Else
		xPackageID = rsObj("PackageID")
		sCheckActivationCodeResult = "<iPackageID>"& xPackageID & "</iPackageID>"
		xUseByDate = rsObj("UseByDate")
		xBatchSuspended = rsObj("Suspended")
		xExpirationDate = CDate(xUseByDate)
		If rsObj("AbsoluteExpiration") <> "" Then
			xAbsExpire = rsObj("AbsoluteExpiration")
			if CDate(xAbsExpire) < xExpirationDate then
				xExpirationDate = CDate(xAbsExpire)
			end if
		Else
			xRelExpire = CInt(rsObj("RelativeExpiration"))
			xExpirationDate = CDate(dateadd("d",xRelExpire,Date()))
		End If
	End If
	rsObj.close
	set rsObj = Nothing
	' If no batch, exit
	If sErrorMsg = "BatchNotFound" Then
		Call writelog("3", "xBatchID " & xBatchID & " not found in DB.")
		goToFinish
	End If

	' If batch is suspended
	If xBatchSuspended <> 0 Then
		Call writelog("3", "BatchSuspended: " & xBatchSuspended)
		sErrorMsg = "BatchSuspended"
		goToFinish
	End If

	' If usebydate is past
	If xUseByDate < Date() Then
		Call writelog("3", "UseByDate is up: " & xUseByDate)
		sErrorMsg = "CodeUseByDateExpired"
		goToFinish
	End If

'	' If expireddate is past
	If xExpirationDate < Date() Then
		If xAbsExpire <> "" Then
			Call writelog("3", "AbsoluteExpiration: " & xExpirationDate)
			sErrorMsg = "CodeExpiredAbsolute"
		Else
			Call writelog("3", "RelativeExpiration: " & xExpirationDate)
			sErrorMsg = "CodeExpiredRelative"
		End If
		goToFinish
	End If

	if iUserID <> "" then
'cccx ra8X
'checks for userassignment with packageid
		' See if this package is already assigned to this user
		strQuery = "Select * from tblUserAssignments where " & _
			"UserID=" & iUserID & " and " & _
			"PackageID=" & xPackageID
Call writelog("3", "CCC- strQuery=" & strQuery)
		Set rsObj = Server.CreateObject("ADODB.Recordset")
		rsObj.open strQuery, objConn
		if not rsObj.EOF then
			sErrorMsg = "PackageAlreadyAssignedToUser"
		end if
		rsObj.close
		set rsObj = Nothing
		If sErrorMsg = "PackageAlreadyAssignedToUser" Then
			Call writelog("3", "xPackageID:" & xPackageID & _
					" is already assigned to iUserID:" & iUserID )
			goToFinish
		End If
	end if

'gets from packageid,siteid: levelid
	'-- Check SiteAssignments
	If inStr(","&sPackageIDs&",",","&xPackageID&",") = 0 Then
		sErrorMsg = "CodeNotValidForPackages"
''		goToFinish
	End If

	if sErrorMsg = "CodeNotValidForPackages" then
''		xLevelID = 0
		strQuery = "Select LevelID from tblSiteAssignments" & _
				" where PackageID =" & xPackageID & _
				" and SiteID=" & iSiteID
		Set rsObj = Server.CreateObject("ADODB.Recordset")
		rsObj.Open strQuery, objConn
		If rsObj.EOF Then
			sErrorMsg = "CodeNotValidForSite"
		Else
''			xLevelID = rsObj("LevelID")
''			Call writeLog("3", "xLevelID " & xLevelID & ".")
		End If
		rsObj.close
		set rsObj = Nothing
	end if

'   '-- If no site assignment, exit
	If sErrorMsg = "CodeNotValidForSite" OR sErrorMsg = "CodeNotValidForPackages" Then
		Call writelog("3", "sPackageIDs " & sPackageIDs & " not assigned to" & _
				" iSiteID " & iSiteID & " in DB.")
		goToFinish
	End If
	sErrorMsg = ""

   '-- Verify xLevelID
'	If xLevelID =< 0 Then
'		Call writelog("3", "xLevelID =< 0 :" & xLevelID )
'		sErrorMsg = "LevelIDNotValid"
'		goToFinish
'	End If

	'--	Get xLevelOfAccess data
'	strQuery = "Select * from tblLevelType where LevelID=" & xLevelID
'	Set rsObj = Server.CreateObject("ADODB.Recordset")
'	rsObj.Open strQuery, objConn
'	If rsObj.EOF Then
'		sErrorMsg = "LevelIDNotFound"
'	Else
'		xLevelOfAccess = rsObj("LevelOfAccess")
'	End If
'
'	rsObj.close
'	set rsObj = Nothing
	'-- If no leveltype found, exit
'	If sErrorMsg = "LevelIDNotFound" Then
'		Call writelog("3", "xLevelID " & xLevelID & " not found in DB.")
'		goToFinish
'	End If

	' Get all of the sites this code applies to
	Dim arrSiteIDs(), SiteCt
	SiteCt = 0
	strQuery = "Select SiteID from tblSiteAssignments " & _
			"where PackageID=" & xPackageID

	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		Call writelog("3", "No Site Assignment found for this code's package, xPackageID: " & xPackageID & ".")
		sErrorMsg = "CodeNotValidForAnySites"
	Else
		While not rsObj.EOF
			SiteCt = SiteCt + 1
			ReDim Preserve arrSiteIDs(SiteCt)
			Call writelog("3", "Site ID " & CStr(rsObj("SiteID")) & " active " & _
					"for this code, xPackageID: " & xPackageID & ".")
			arrSiteIDs(SiteCt-1) = rsObj("SiteID")
			rsObj.moveNext
		Wend
	End If
	rsObj.close
	set rsObj = Nothing
	If sErrorMsg = "CodeNotValidForAnySites" Then
		Call writelog("3", "xPackageID " & xPackageID & " not assigned to any sites in DB.")
		goToFinish
	End If


	' It's available
	Call writelog("3", "Code Available: " & xNormalizedCode)





'	sCheckActivationCodeResult = xLevelOfAccess
'
	if sErrorMsg = "" then
		sCheckActivationCodeResult = "<iPackageID>"& xPackageID & "</iPackageID>"
	end if


	set objConn = Nothing

end sub






















sub writeLog (y,z)
end sub

%>




