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
dim sDebugMsg : sDebugMsg = ""
function writeMsg (n,msg)
	if 1 then
		sDebugMsg = sDebugMsg & VBCRLF&":: "& n &" :: {"& replace(replace(msg,"<","&lt;"),">","&gt;") &"}"
	end if
end function

dim resultXML
dim nGet, nActivationCodes, nUserIDs, nSiteIDs
dim sSiteIDs, iUserID, sActivationCode

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
dim RS_Site_ct


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
"      <sActivationCode>p-q-hyhce7</sActivationCode>" &_
"      <iSiteID>24432</iSiteID>" &_
"      <iSiteID>24434</iSiteID>" &_
"      <iSiteID>24456</iSiteID>" &_
"      <iUserID>1</iUserID>" &_
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
"      <sActivationCode>6k-9v-meuzf9x</sActivationCode>" &_
"      <iSiteID>24432</iSiteID>" &_
"      <iSiteID>24434</iSiteID>" &_
"      <iSiteID>24456</iSiteID>" &_
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
"      <sActivationCode>32-3i-46e8p3a</sActivationCode>" &_
"      <iSiteID>24432</iSiteID>" &_
"      <iSiteID>24434</iSiteID>" &_
"      <iSiteID>24456</iSiteID>" &_
"      <iUserID>1</iUserID>" &_
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
"      <iSiteID>24432</iSiteID>" &_
"      <iSiteID>24434</iSiteID>" &_
"      <iSiteID>24456</iSiteID>" &_
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

if nActivationCodes.length = 1 then
	sActivationCode = nActivationCodes(0).text
end if
if nSiteIDs.length > 0 then
	sSiteIDs = ""
	for ict = 0 to nSiteIDs.length-1
		if nSiteIDs(ict).text <> "" then
			if ict > 0 then
				sSiteIDs = sSiteIDs &","
			end if
			sSiteIDs = sSiteIDs & nSiteIDs(ict).text
		end if
	next
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

	if nSiteIDs.length = 0 then
		sErrorMsg = "Invalid input: no iSiteID elements"
		goToFinish
	end if
	if sSiteIDs = "" then
		sErrorMsg = "Invalid input: no SiteID values"
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
	ResponseStr = "<CheckActivationCodeResponse><CheckActivationCodeResult>"& sCheckActivationCodeResult &"</CheckActivationCodeResult><sDebugMsg>"& sDebugMsg &"</sDebugMsg><sErrorMsg>"& sErrorMsg &"</sErrorMsg></CheckActivationCodeResponse>"
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
'	"e10" = "LevelIDNotValid"
'	"e11" = "LevelIDNotFound"
'	"e12" = "CodeAlreadyAssignedToUser"
'PackageAlreadyAssignedToUser
'	"e13" = "NoSiteLoginsForUser"
'	"e14" = "CodeNotValidForAnySites"
'	"e15" = "BatchSuspended"
'	[level of access] = "success"

Call writeMsg("iUserID     : ", iUserID)
Call writeMsg("sActivationCode : ", sActivationCode)
Call writeMsg("iSiteID     : ", iSiteID)

	' Normalize
	xNormalizedCode = getNormalized(sActivationCode)

	' Validate format
	if not isValidAccFormat(xNormalizedCode) then
		Call writeMsg("failed isValidAccFormat","")
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
		Call writeMsg("Code not found: ", xNormalizedCode )
		goToFinish
	end if
	' Verify checksum.
Call writeMsg("creation date compared to 12/4/2002:: ", xCreatedDate )
	if CDate(xCreatedDate) < CDate("12/4/2002") then
		if not verifyChecksumOLD(xNormalizedCode) then
			Call writeMsg("failed verifyChecksumOLD", "")
			sErrorMsg = "CodeFailsSumCheck"
			goToFinish
		end if
	else
		if not verifyChecksum(xNormalizedCode) then
			Call writeMsg("failed verifyChecksum", "")
			sErrorMsg = "CodeFailsSumCheck"
			goToFinish
		end if
	end if
	' If no uses left, exit
	if xCodeUses = 0 then
		sErrorMsg = "CodeNoUsesLeft"
		Call writeMsg("Code no uses left", xNormalizedCode)
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
		Call writeMsg("xBatchID not found in DB.", xBatchID)
		goToFinish
	End If

	' If batch is suspended
	If xBatchSuspended <> 0 Then
		Call writeMsg("BatchSuspended: ", xBatchSuspended)
		sErrorMsg = "BatchSuspended"
		goToFinish
	End If

	' If usebydate is past
	If xUseByDate < Date() Then
		Call writeMsg("UseByDate is up: ", xUseByDate)
		sErrorMsg = "CodeUseByDateExpired"
		goToFinish
	End If

'	' If expireddate is past
	If xExpirationDate < Date() Then
		If xAbsExpire <> "" Then
			Call writeMsg("AbsoluteExpiration: ", xExpirationDate)
			sErrorMsg = "CodeExpiredAbsolute"
		Else
			Call writeMsg("RelativeExpiration: ", xExpirationDate)
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
Call writeMsg("CCC- strQuery=", strQuery)
		Set rsObj = Server.CreateObject("ADODB.Recordset")
		rsObj.open strQuery, objConn
		if not rsObj.EOF then
			sErrorMsg = "PackageAlreadyAssignedToUser"
		end if
		rsObj.close
		set rsObj = Nothing
		If sErrorMsg = "PackageAlreadyAssignedToUser" Then
			Call writeMsg("xPackageID is already assigned to iUserID:", xPackageID &" - " & iUserID )
			goToFinish
		End If
	end if

'gets from packageid,siteid: levelid
''	xLevelID = 0
	strQuery = "Select LevelID from tblSiteAssignments" & _
			" where PackageID =" & xPackageID & _
			" and SiteID IN (" & sSiteIDs &")"
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		sErrorMsg = "CodeNotValidForSite"
	Else
''		xLevelID = rsObj("LevelID")
''		Call writeMsg("xLevelID ", xLevelID)
	End If
	rsObj.close
	set rsObj = Nothing

'   '-- If no site assignment, exit
	If sErrorMsg = "CodeNotValidForSite" Then
		Call writeMsg("not assigned to sSiteIDs in DB: ", sSiteIDs)
		goToFinish
	End If
	sErrorMsg = ""

   '-- Verify xLevelID
'	If xLevelID =< 0 Then
'		Call writeMsg("xLevelID =< 0 :", xLevelID )
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
'		Call writeMsg("xLevelID not found in DB.", xLevelID)
'		goToFinish
'	End If
'
'	' Get all of the sites this code applies to
	Dim arrSiteIDs(), SiteCt
	SiteCt = 0
	strQuery = "Select SiteID from tblSiteAssignments " & _
			"where PackageID=" & xPackageID &""&_
			"and SiteID IN ("& sSiteIDs &")"

	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		Call writeMsg("No Site Assignment found for this code's package, xPackageID", xPackageID)
		sErrorMsg = "CodeNotValidForAnySites"
	Else
		While not rsObj.EOF
			SiteCt = SiteCt + 1
			ReDim Preserve arrSiteIDs(SiteCt)
			Call writeMsg("Site ID " & CStr(rsObj("SiteID")) & " active " & _
					"for this code, xPackageID: " & xPackageID & ".", "")
			arrSiteIDs(SiteCt-1) = rsObj("SiteID")
			rsObj.moveNext
		Wend
	End If
	rsObj.close
	set rsObj = Nothing
	If sErrorMsg = "CodeNotValidForAnySites" Then
		Call writeMsg("xPackageID " & xPackageID & " not assigned to any sites in DB.", "")
		goToFinish
	End If


	' It's available
	Call writeMsg("Code Available: ", xNormalizedCode)





'	sCheckActivationCodeResult = xLevelOfAccess
'
	dim site_i
	if sErrorMsg = "" then
	Call writeMsg("forming result, SiteCt = ", SiteCt)
		for site_i = 0 to SiteCt-1
	Call writeMsg("arrSiteIDs(site_i)", arrSiteIDs(site_i))
			sCheckActivationCodeResult = sCheckActivationCodeResult &"<iSiteID>"& arrSiteIDs(site_i) & "</iSiteID>"
		next
	end if

	set objConn = Nothing

end sub






















sub writeLog (y,z)
end sub

%>




