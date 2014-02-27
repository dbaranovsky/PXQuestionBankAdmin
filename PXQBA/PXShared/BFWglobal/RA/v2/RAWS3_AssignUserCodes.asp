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

dim sAssignUserCodesResult : sAssignUserCodesResult = ""
dim sErrorMsg : sErrorMsg = ""

dim resultXML
dim nGet, nActivationCodes, nUserIDs
dim iUserID, sActivationCodes
dim arrActivationCodes

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
	RequestBinStr = "" &_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <AssignUserCodes xmlns=""http://tempuri.org/"">" &_
"      <sActivationCodes>r6-3aj-48c4dz6a</sActivationCodes>" &_
"      <iUserID>3500718</iUserID>" &_
"    </AssignUserCodes>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
elseif 0 then
	RequestBinStr = "" &_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <AssignUserCodes xmlns=""http://tempuri.org/"">" &_
"      <sActivationCodes>r6-3aj-48c4dz6a,f4-y6-48c4emyc</sActivationCodes>" &_
"      <iUserID>3500726</iUserID>" &_
"    </AssignUserCodes>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
elseif 0 then
	RequestBinStr = "" &_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <AssignUserCodes xmlns=""http://tempuri.org/"">" &_
"      <sActivationCodes>r6-3aj-48c4dz6a,f4-y6-48c4emyc</sActivationCodes>" &_
"      <iUserID>3500718</iUserID>" &_
"    </AssignUserCodes>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
end if


set resultXML = Server.CreateObject("Microsoft.XMLDOM")
resultXML.async = "false"
resultXML.loadXML( RequestBinStr )

set nGet = resultXML.getElementsByTagName("AssignUserCodes")
set nActivationCodes = resultXML.getElementsByTagName("sActivationCodes")
set nUserIDs = resultXML.getElementsByTagName("iUserID")

if nActivationCodes.length = 1 then
	sActivationCodes = nActivationCodes(0).text
	arrActivationCodes = split(sActivationCodes, ",")
end if
if nUserIDs.length = 1 then
	iUserID = nUserIDs(0).text
else
	iUserID = ""
end if

checkInput

dim arr_i : arr_i = 0
while arr_i <= UBound(arrActivationCodes)
''response.write arr_i & "<br/>"
''response.write arrActivationCodes(arr_i) & "<br/>"
	DB_AssignUserCode arrActivationCodes(arr_i)
	arr_i = arr_i+1
wend
''response.end
goToFinish




sub checkInput ()
	if nGet.length = 0 then
		sErrorMsg = "Invalid input: no AssignUserCodes element"
		goToFinish
	end if

	if nUserIDs.length = 0 or iUserID = "" then
		sErrorMsg = "Invalid input: no iUserID"
		goToFinish
	end if

	if nActivationCodes.length = 0 or sActivationCodes = "" then
		sErrorMsg = "Invalid input: no sActivationCodes"
		goToFinish
	end if

end sub




sub goToFinish ()
	dim ResponseStr
	ResponseStr = "<AssignUserCodesResponse><AssignUserCodesResult>"& sAssignUserCodesResult &"</AssignUserCodesResult><sErrorMsg>"& sErrorMsg &"</sErrorMsg></AssignUserCodesResponse>"
	ResponseStr = "<?xml version=""1.0""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body>"& ResponseStr &"</soap:Body></soap:Envelope>"
	response.write ResponseStr
	response.end
end sub




sub DB_AssignUserCode (sCode)
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
'	"e12" = "CodeAlreadyAssignedToUser"
'PackageAlreadyAssignedToUser
'	"e15" = "BatchSuspended"

Call writelog("3", "iUserID     : " & iUserID)
Call writelog("3", "sCode : " & sCode)

	' Normalize
	xNormalizedCode = getNormalized(sCode)

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
'checks for valid user
		strQuery = "Select UserRevoked from tblUserProfile where " & _
			"UserID=" & iUserID
Call writelog("3", "CCC- strQuery=" & strQuery)
		Set rsObj = Server.CreateObject("ADODB.Recordset")
		rsObj.open strQuery, objConn
		if rsObj.EOF then
			sErrorMsg = "UserNotFound"
		else
			if rsObj("UserRevoked") = 1 then
				sErrorMsg = "UserAccessRevoked"
			end if
		end if
		rsObj.close
		set rsObj = Nothing
		If sErrorMsg = "UserNotFound" or sErrorMsg = "UserAccessRevoked" Then
			goToFinish
		End If
	end if

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
			sAssignUserCodesResult = sAssignUserCodesResult &"<udtPackage><iPackageID>"& xPackageID &"</iPackageID><dtExpiration>"& rsObj("Expiration") &"</dtExpiration></udtPackage>"
		end if
		rsObj.close
		set rsObj = Nothing
		If sErrorMsg = "PackageAlreadyAssignedToUser" Then
			Call writelog("3", "xPackageID:" & xPackageID & _
					" is already assigned to iUserID:" & iUserID )
			sErrorMsg = ""
''			goToFinish
			exit sub
		End If

	' Get all of the sites this code applies to
'	Dim arrSiteIDs(), SiteCt
'	SiteCt = 0
'	strQuery = "Select SiteID from tblSiteAssignments " & _
'			"where PackageID=" & xPackageID
'
'	Set rsObj = Server.CreateObject("ADODB.Recordset")
'	rsObj.Open strQuery, objConn
'	If rsObj.EOF Then
'		Call writelog("3", "No Site Assignment found for this code's package, xPackageID: " & xPackageID & ".")
'		sErrorMsg = "CodeNotValidForAnySites"
'	Else
'		While not rsObj.EOF
'			SiteCt = SiteCt + 1
'			ReDim Preserve arrSiteIDs(SiteCt)
'			Call writelog("3", "Site ID " & CStr(rsObj("SiteID")) & " active " & _
'					"for this code, xPackageID: " & xPackageID & ".")
'			arrSiteIDs(SiteCt-1) = rsObj("SiteID")
'			rsObj.moveNext
'		Wend
'	End If
'	rsObj.close
'	set rsObj = Nothing
'	If sErrorMsg = "CodeNotValidForAnySites" Then
'		Call writelog("3", "xPackageID " & xPackageID & " not assigned to any sites in DB.")
'		goToFinish
'	End If


	' It's available
	Call writelog("3", "Code Available: " & xNormalizedCode)


''sAssignUserCodesResult = sAssignUserCodesResult & "GO<br/>"
''goToFinish
''exit sub

	' All Checks are okay.  Now to update database.

''	strQuery = "insert into tbluserassignments values (" & inUserID & "," & xAccessID & ",'" & cdate(xExpirationDate) & "'," & xPackageID & ")"
	strQuery = "insert into tbluserassignments values (" & iUserID & "," & xAccessID & ",'" & cdate(xExpirationDate) & "'," & xPackageID & ")"
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objconn


	'Decrement number of uses
	Set rsObj = Server.CreateObject("ADODB.Connection")
	rsObj.Open  objConn
	strQuery = "UPDATE tblAccessCode" & _
			" SET Uses=" & CInt(xCodeUses - 1) & _
			" WHERE AccessID=" & xAccessID
	Set RSx = rsObj.Execute(strQuery)
	rsObj.Close
	set rsObj = Nothing



	if sErrorMsg = "" then
		sAssignUserCodesResult = sAssignUserCodesResult &"<udtPackage><iPackageID>"& xPackageID &"</iPackageID><dtExpiration>"& xExpirationDate &"</dtExpiration></udtPackage>"
	end if


	set objConn = Nothing

end sub






















sub writeLog (y,z)
end sub

%>




