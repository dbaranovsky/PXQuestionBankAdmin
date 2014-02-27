<%
Response.buffer = true
'Response.contenttype = "text/html"
'
Response.contenttype = "text/xml"

dim debug : debug = false

dim testInput : testInput = Request.QueryString("testInput")

dim i, str

dim RequestBinCt
dim RequestBin
dim RequestBinStr : RequestBinStr = ""

dim sGetSitesFromSiteIDsResult : sGetSitesFromSiteIDsResult = ""
dim sErrorMsg : sErrorMsg = ""
dim sDebugMsg : sDebugMsg = ""
function writeMsg (n,msg)
	if debug then
		sDebugMsg = sDebugMsg & VBCRLF&":: "& n &" :: {"& replace(replace(msg,"<","&lt;"),">","&gt;") &"}"
	end if
end function

dim resultXML
dim nGet, nSiteIDs
dim sSiteIDs : sSiteIDs = ""

dim arrSite()
dim udtSite_i
dim udtSite_ct : udtSite_ct = 0
dim udtSite()
dim udtSite_props_ct : udtSite_props_ct = 2
const udtSite__iSiteID = 0
const udtSite__sBaseURL = 1

dim sPackageIDs : sPackageIDs = ""
dim arrSiteAssignment()
dim udtSiteAssignment_i
dim udtSiteAssignment_ct : udtSiteAssignment_ct = 0
dim udtSiteAssignment()
dim udtSiteAssignment_props_ct : udtSiteAssignment_props_ct = 6
const udtSiteAssignment__iSiteID = 0
const udtSiteAssignment__iPackageID = 1
const udtSiteAssignment__sDescription = 2
const udtSiteAssignment__sType = 3
const udtSiteAssignment__iSiteAssignmentID = 4
const udtSiteAssignment__iLevelOfAccess = 5

dim arrBatch()
dim udtBatch_i
dim udtBatch_ct : udtBatch_ct = 0
dim udtBatch()
dim udtBatch_props_ct : udtBatch_props_ct = 9
const udtBatch__iPackageID = 0
const udtBatch__iBatchID = 1
const udtBatch__sDescription = 2
const udtBatch__dtUseByDate = 3
const udtBatch__iRelativeExpiration = 4
const udtBatch__dtAbsoluteExpiration = 5
const udtBatch__bSuspended = 6
const udtBatch__sType = 7
const udtBatch__mPrice = 8

dim objConn
dim rsObj
%>
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
<%
dim strQuery
dim RS_Site_ct
dim RS_lastSiteID
dim RS_Package_ct
dim RS_lastPackageID
dim RS_Batch_ct
dim RS_lastBatchID


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
"<soap:Envelope xmlns:xsi=""http://tempuri.org/""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <GetSitesFromSiteIDs xmlns=""http://tempuri.org/"">" &_
"      <iSiteID>24479</iSiteID>" &_
"      <iSiteID>24434</iSiteID>" &_
"    </GetSitesFromSiteIDs>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
elseif 0 then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://tempuri.org/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">  <soap:Body>    <GetSitesFromSiteIDs xmlns=""http://tempuri.org/"">      <iSiteID>24049</iSiteID>      <iSiteID>374</iSiteID>      <iSiteID>706</iSiteID>      <iSiteID>705</iSiteID>    </GetSitesFromSiteIDs>  </soap:Body></soap:Envelope>" &_
""
elseif 0 then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://tempuri.org/""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <GetSitesFromSiteIDs xmlns=""http://tempuri.org/"">" &_
"      <iSiteID>24432</iSiteID>" &_
"      <iSiteID>24434</iSiteID>" &_
"      <iSiteID>24456</iSiteID>" &_
"    </GetSitesFromSiteIDs>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
end if

call writeMsg ("input",RequestBinStr)

set resultXML = Server.CreateObject("Microsoft.XMLDOM")
resultXML.async = "false"
resultXML.loadXML( RequestBinStr )

set nGet = resultXML.getElementsByTagName("GetSitesFromSiteIDs")
set nSiteIDs = resultXML.getElementsByTagName("iSiteID")

call writeMsg ("nSiteIDs.length",nSiteIDs.length)

dim ict

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

call writeMsg ("sSiteIDs",sSiteIDs)

checkInput

DB_GetSitesFromSiteIDs

goToFinish




sub checkInput ()
	if nGet.length = 0 then
		sErrorMsg = "Invalid input: no GetSitesFromSiteIDs element"
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
end sub




sub goToFinish ()
	dim ResponseStr
	ResponseStr = "<GetSitesFromSiteIDsResponse>"&VBCRLF&"<GetSitesFromSiteIDsResult>"& sGetSitesFromSiteIDsResult &"</GetSitesFromSiteIDsResult>"&VBCRLF&"<sDebugMsg>"& sDebugMsg &"</sDebugMsg>"&VBCRLF&"<sErrorMsg>"& sErrorMsg &"</sErrorMsg>"&VBCRLF&"</GetSitesFromSiteIDsResponse>"
	ResponseStr = "<?xml version=""1.0""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body>"& ResponseStr &"</soap:Body></soap:Envelope>"
	response.write ResponseStr
	response.end
end sub




sub DB_GetSitesFromSiteIDs ()
	set rsObj = Server.CreateObject("ADODB.Recordset")
	ConnectToBFWUsersDB
	CheckForErrors(objConn)

	strQuery = ""&_
"SELECT DISTINCT "&_
"	tblSiteKey.SiteID, "&_
"	tblSiteKey.BaseURL "&_
"FROM	tblSiteKey WITH (nolock) "&_
"WHERE tblSiteKey.SiteID IN ("& sSiteIDs &")"&_
"ORDER BY tblSiteKey.SiteID"&_
""

call writeMsg ("strQuery",strQuery)
'response.write strQuery
'response.end
	rsObj.open strQuery, objConn
	if rsObj.EOF then
		sErrorMsg = sErrorMsg & " :: No sites found"
		goToFinish
	else
		while not rsObj.EOF
			ReDim arrSite(udtSite_props_ct)

			arrSite(udtSite__iSiteID) = rsObj("SiteID")
			arrSite(udtSite__sBaseURL) = rsObj("BaseURL")

call writeMsg ("row #",udtSite_ct)
			udtSite_ct = udtSite_ct + 1
			ReDim Preserve udtSite(udtSite_ct)
			udtSite(udtSite_ct-1) = arrSite

			rsObj.moveNext
		wend

	end if
	rsObj.close


	strQuery = ""&_
"SELECT DISTINCT "&_
"	tblPackage.PackageID, "&_
"	tblPackage.Description, "&_
"	tblPackage.Type, "&_
"	tblSiteAssignments.AssignmentID, "&_
"	tblSiteAssignments.SiteID, "&_
"	tblLevelType.LevelOfAccess "&_
"FROM	tblPackage WITH (nolock) "&_
"	INNER JOIN tblSiteAssignments (nolock) "&_
"		ON tblPackage.PackageID = tblSiteAssignments.PackageID "&_
"	INNER JOIN tblLevelType (nolock) "&_
"		ON tblLevelType.LevelID = tblSiteAssignments.LevelID "&_
"WHERE tblSiteAssignments.SiteID IN ("& sSiteIDs &")"&_
"ORDER BY tblSiteAssignments.SiteID, tblPackage.PackageID"&_
""
''"	'description blocked' AS Description, "&_

call writeMsg ("strQuery",strQuery)
'response.write strQuery
'response.end
	rsObj.open strQuery, objConn
	if rsObj.EOF then
call writeMsg ("No packages found","")
	else
		while not rsObj.EOF
			ReDim arrSiteAssignment(udtSiteAssignment_props_ct)

			arrSiteAssignment(udtSiteAssignment__iSiteID) = rsObj("SiteID")
			arrSiteAssignment(udtSiteAssignment__iPackageID) = rsObj("PackageID")
			arrSiteAssignment(udtSiteAssignment__sDescription) = rsObj("Description")
			arrSiteAssignment(udtSiteAssignment__sType) = rsObj("Type")
			arrSiteAssignment(udtSiteAssignment__iSiteAssignmentID) = rsObj("AssignmentID")
			arrSiteAssignment(udtSiteAssignment__iLevelOfAccess) = rsObj("LevelOfAccess")

call writeMsg ("row #",udtSiteAssignment_ct)
			udtSiteAssignment_ct = udtSiteAssignment_ct + 1
			if udtSiteAssignment_ct > 1 then
				sPackageIDs = sPackageIDs & ","
			end if
			sPackageIDs = sPackageIDs & arrSiteAssignment(udtSiteAssignment__iPackageID)
			ReDim Preserve udtSiteAssignment(udtSiteAssignment_ct)
			udtSiteAssignment(udtSiteAssignment_ct-1) = arrSiteAssignment

			rsObj.moveNext
		wend

	end if
	rsObj.close


	if sPackageIDs <> "" then

	strQuery = ""&_
"SELECT DISTINCT "&_
"	tblBatchKey.BatchID, "&_
"	tblBatchKey.PackageID, "&_
"	tblBatchKey.Description, "&_
"	tblBatchKey.UseByDate, "&_
"	tblBatchKey.RelativeExpiration, "&_
"	tblBatchKey.AbsoluteExpiration, "&_
"	tblBatchKey.Suspended, "&_
"	tblBatchKey.Type, "&_
"	tblBatchKey.Price "&_
"FROM	tblBatchKey WITH (nolock) "&_
"WHERE tblBatchKey.PackageID IN ("& sPackageIDs &")"&_
"ORDER BY tblBatchKey.PackageID, tblBatchKey.AbsoluteExpiration DESC"&_
""

call writeMsg ("strQuery",strQuery)
'response.write strQuery
'response.end
	rsObj.open strQuery, objConn
	if rsObj.EOF then
call writeMsg ("No packages found","")
	else
		while not rsObj.EOF
			ReDim arrBatch(udtBatch_props_ct)

			arrBatch(udtBatch__iPackageID) = rsObj("PackageID")
			arrBatch(udtBatch__iBatchID) = rsObj("BatchID")
			arrBatch(udtBatch__sDescription) = rsObj("Description")
			arrBatch(udtBatch__dtUseByDate) = rsObj("UseByDate")
			arrBatch(udtBatch__iRelativeExpiration) = rsObj("RelativeExpiration")
			arrBatch(udtBatch__dtAbsoluteExpiration) = rsObj("AbsoluteExpiration")
			arrBatch(udtBatch__bSuspended) = CInt(rsObj("Suspended"))
			arrBatch(udtBatch__sType) = rsObj("Type")
			arrBatch(udtBatch__mPrice) = rsObj("Price")

call writeMsg ("row #",udtBatch_ct)
			udtBatch_ct = udtBatch_ct + 1
			ReDim Preserve udtBatch(udtBatch_ct)
			udtBatch(udtBatch_ct-1) = arrBatch

			rsObj.moveNext
		wend

	end if
	rsObj.close




	end if




	for udtSite_i = 0 to udtSite_ct-1

sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<udtSite>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<iSiteID>"& udtSite(udtSite_i)(udtSite__iSiteID) &"</iSiteID>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<sBaseURL>"& udtSite(udtSite_i)(udtSite__sBaseURL) &"</sBaseURL>"

		for udtSiteAssignment_i = 0 to udtSiteAssignment_ct-1
			if udtSite(udtSite_i)(udtSite__iSiteID) = udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iSiteID) then

sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<udtPackage>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<iPackageID>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iPackageID) &"</iPackageID>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<sDescription>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__sDescription) &"</sDescription>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<sType>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__sType) &"</sType>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<iSiteAssignmentID>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iSiteAssignmentID) &"</iSiteAssignmentID>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<iLevelOfAccess>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iLevelOfAccess) &"</iLevelOfAccess>"
				for udtBatch_i = 0 to udtBatch_ct-1
					if udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iPackageID) = udtBatch(udtBatch_i)(udtBatch__iPackageID) then

sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<udtBatch>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<iBatchID>"& udtBatch(udtBatch_i)(udtBatch__iBatchID) &"</iBatchID>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<sDescription>"& udtBatch(udtBatch_i)(udtBatch__sDescription) &"</sDescription>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<dtUseByDate>"& udtBatch(udtBatch_i)(udtBatch__dtUseByDate) &"</dtUseByDate>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<iRelativeExpiration>"& udtBatch(udtBatch_i)(udtBatch__iRelativeExpiration) &"</iRelativeExpiration>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<dtAbsoluteExpiration>"& udtBatch(udtBatch_i)(udtBatch__dtAbsoluteExpiration) &"</dtAbsoluteExpiration>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<bSuspended>"& udtBatch(udtBatch_i)(udtBatch__bSuspended) &"</bSuspended>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<sType>"& udtBatch(udtBatch_i)(udtBatch__sType) &"</sType>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"<mPrice>"& udtBatch(udtBatch_i)(udtBatch__mPrice) &"</mPrice>"
sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"</udtBatch>"

					end if
				next

sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"</udtPackage>"

			end if
		next

sGetSitesFromSiteIDsResult = sGetSitesFromSiteIDsResult &VBCRLF&"</udtSite>"

	next

	set rsObj = nothing

	objConn.close
	set objConn = nothing

goToFinish

end sub
%>

