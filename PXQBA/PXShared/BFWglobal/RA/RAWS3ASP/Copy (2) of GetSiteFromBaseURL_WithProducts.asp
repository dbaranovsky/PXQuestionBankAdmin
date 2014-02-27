<%
Response.buffer = true
'Response.contenttype = "text/html"
'
Response.contenttype = "text/xml"

dim debug : debug = false
if Request.QueryString("debug") <> "" then
	debug = true
end if

dim testInput : testInput = Request.QueryString("testInput")

dim i, str

dim RequestBinCt
dim RequestBin
dim RequestBinStr : RequestBinStr = ""

dim sGetSiteFromBaseURLResult : sGetSiteFromBaseURLResult = ""
dim sErrorMsg : sErrorMsg = ""
dim sDebugMsg : sDebugMsg = ""
function writeMsg (n,msg)
	if debug then
		sDebugMsg = sDebugMsg & VBCRLF&":: "& n &" :: {"& replace(replace(msg,"<","&lt;"),">","&gt;") &"}"
	end if
end function

dim resultXML
dim nGet, nBaseURLs
dim sBaseURLs : sBaseURLs = ""

dim sSiteIDs : sSiteIDs = ""
dim arrSite()
dim udtSite_i
dim udtSite_ct : udtSite_ct = 0
dim udtSite()
dim udtSite_props_ct : udtSite_props_ct = 3
const udtSite__iSiteID = 0
const udtSite__sBaseURL = 1
const udtSite__sDescription = 2

dim arrProduct()
dim udtProduct_i
dim udtProduct_ct : udtProduct_ct = 0
dim udtProduct()
dim udtProduct_props_ct : udtProduct_props_ct = 8
const udtProduct__iSiteID = 0
const udtProduct__sProductID = 1
const udtProduct__sType = 2
const udtProduct__sTitle = 3
const udtProduct__sBaseURL = 4
const udtProduct__sTag = 5
const udtProduct__sMore = 6
const udtProduct__sData = 7

dim sPackageIDs : sPackageIDs = ""
dim sProductPackageIDs : sProductPackageIDs = ""
dim arrSiteAssignment()
dim udtSiteAssignment_i
dim udtSiteAssignment_ct : udtSiteAssignment_ct = 0
dim udtSiteAssignment()
dim udtSiteAssignment_props_ct : udtSiteAssignment_props_ct = 8
const udtSiteAssignment__iSiteID = 0
const udtSiteAssignment__iPackageID = 1
const udtSiteAssignment__sDescription = 2
const udtSiteAssignment__sType = 3
const udtSiteAssignment__iSiteAssignmentID = 4
const udtSiteAssignment__iLevelOfAccess = 5
const udtSiteAssignment__sBaseURL = 6
const udtSiteAssignment__sSiteDesc = 7

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
"<soap:Envelope xmlns:xsi=""http://tempuri.org/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">  <soap:Body>    <GetSiteFromBaseURL xmlns=""http://tempuri.org/"">      <sBaseURL>bcs.bfwpub.com/everydaywriter4e_qa</sBaseURL>    </GetSiteFromBaseURL>  </soap:Body></soap:Envelope>" &_
""
elseif 0 then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://tempuri.org/""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <GetSiteFromBaseURL xmlns=""http://tempuri.org/"">" &_
"      <sBaseURL>bcs.bfwpub.com/everydaywriter4e_qa</sBaseURL>" &_
"    </GetSiteFromBaseURL>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
elseif 0 then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://tempuri.org/""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <GetSiteFromBaseURL xmlns=""http://tempuri.org/"">" &_
"      <sBaseURL>192.168.77.114/everydaywriter4e_cc</sBaseURL>" &_
"    </GetSiteFromBaseURL>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
elseif 0 then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://tempuri.org/""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <GetSiteFromBaseURL xmlns=""http://tempuri.org/"">" &_
"      <sBaseURL>192.168.77.114/BCSv3_cc/BCS</sBaseURL>" &_
"    </GetSiteFromBaseURL>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
elseif debug then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://tempuri.org/""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <GetSiteFromBaseURL xmlns=""http://tempuri.org/"">" &_
"      <sBaseURL>dev-gradebook.bfwpub.com</sBaseURL>      <sBaseURL>dev-gradebook.bfwpub.com/</sBaseURL>" &_
"    </GetSiteFromBaseURL>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
elseif debug then
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://tempuri.org/""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <GetSiteFromBaseURL xmlns=""http://tempuri.org/"">" &_
"      <sBaseURL>192.168.77.243</sBaseURL>      <sBaseURL>192.168.77.243/BFWglobal</sBaseURL>      <sBaseURL>192.168.77.243/BFWglobal/RAg_examples</sBaseURL>      <sBaseURL>192.168.77.243/BFWglobal/RAg_examples/v1.2</sBaseURL>      <sBaseURL>192.168.77.243/BFWglobal/RAg_examples/v1.2/_iframe</sBaseURL>      <sBaseURL>192.168.77.243/BFWglobal/RAg_examples/v1.2/_iframe/default.html</sBaseURL>" &_
"    </GetSiteFromBaseURL>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://tempuri.org/""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <GetSiteFromBaseURL xmlns=""http://tempuri.org/"">" &_
"      <sBaseURL>192.168.77.244</sBaseURL>      <sBaseURL>192.168.77.244/CrunchIt2Pass</sBaseURL>      <sBaseURL>192.168.77.244/CrunchIt2Pass/bps4e</sBaseURL>" &_
"    </GetSiteFromBaseURL>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
RequestBinStr = ""&_
"<soap:Envelope xmlns:xsi=""http://tempuri.org/""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <GetSiteFromBaseURL xmlns=""http://tempuri.org/"">" &_
"      <sBaseURL>192.168.77.243</sBaseURL>      <sBaseURL>192.168.77.243/everydaywriter4e</sBaseURL>      <sBaseURL>192.168.77.243/everydaywriter4e/defaultJJ.asp?radev=yes#t_11472____</sBaseURL>" &_
"    </GetSiteFromBaseURL>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
end if

call writeMsg ("input",RequestBinStr)
'response.write RequestBinStr
'response.end

set resultXML = Server.CreateObject("Microsoft.XMLDOM")
resultXML.async = "false"
resultXML.loadXML( RequestBinStr )

set nGet = resultXML.getElementsByTagName("GetSiteFromBaseURL")
set nBaseURLs = resultXML.getElementsByTagName("sBaseURL")

call writeMsg ("nBaseURLs.length",nBaseURLs.length)

dim ict

if nBaseURLs.length > 0 then
	sBaseURLs = ""
	for ict = 0 to nBaseURLs.length-1
		if nBaseURLs(ict).text <> "" then
			if ict > 0 then
				sBaseURLs = sBaseURLs &","
			end if
			sBaseURLs = sBaseURLs & "'"& nBaseURLs(ict).text &"'"
		end if
	next
end if

call writeMsg ("sBaseURLs",sBaseURLs)

checkInput

DB_GetSiteFromBaseURL

goToFinish




sub checkInput ()
	if nGet.length = 0 then
		sErrorMsg = "Invalid input: no GetSiteFromBaseURL element"
		goToFinish
	end if

	if nBaseURLs.length = 0 then
		sErrorMsg = "Invalid input: no sBaseURL elements"
		goToFinish
	end if
'	if nBaseURLs.length > 1 then
'		sErrorMsg = "Invalid input: multiple sBaseURL elements"
'		goToFinish
'	end if
	if sBaseURLs = "" then
		sErrorMsg = "Invalid input: no sBaseURL values"
		goToFinish
	end if
end sub




sub goToFinish ()
	dim ResponseStr
	ResponseStr = "<GetSiteFromBaseURLResponse>"&VBCRLF&"<GetSiteFromBaseURLResult>"& sGetSiteFromBaseURLResult &"</GetSiteFromBaseURLResult>"&VBCRLF&"<sDebugMsg>"& sDebugMsg &"</sDebugMsg>"&VBCRLF&"<sErrorMsg>"& sErrorMsg &"</sErrorMsg>"&VBCRLF&"</GetSiteFromBaseURLResponse>"
	ResponseStr = "<?xml version=""1.0""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body>"& ResponseStr &"</soap:Body></soap:Envelope>"
	response.write ResponseStr
	response.end
end sub




sub DB_GetSiteFromBaseURL ()
	set rsObj = Server.CreateObject("ADODB.Recordset")
	ConnectToBFWUsersDB
	CheckForErrors(objConn)

	strQuery = ""&_
"SELECT DISTINCT TOP 1 "&_
"	tblSiteKey.SiteID, "&_
"	tblSiteKey.BaseURL "&_
"FROM	tblSiteKey WITH (nolock) "&_
"WHERE tblSiteKey.BaseURL IN ("& sBaseURLs &") "&_
"ORDER BY tblSiteKey.BaseURL DESC"&_
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
			arrSite(udtSite__sDescription) = ""

call writeMsg ("row #",udtSite_ct &" - "& arrSite(udtSite__iSiteID) &" - "& arrSite(udtSite__sBaseURL))
			udtSite_ct = udtSite_ct + 1
			if udtSite_ct > 1 then
				sSiteIDs = sSiteIDs & ","
			end if
			sSiteIDs = sSiteIDs & arrSite(udtSite__iSiteID)
			ReDim Preserve udtSite(udtSite_ct)
			udtSite(udtSite_ct-1) = arrSite

			rsObj.moveNext
		wend

	end if
	rsObj.close


call writeMsg ("sSiteIDs",sSiteIDs)

	if sSiteIDs <> "" then

if 1 then

	strQuery = ""&_
"SELECT DISTINCT "&_
"	tblSiteCartProducts.SiteID, "&_
"	tblSiteCartProducts.IDData, "&_
"	tblSiteCartProducts.Type, "&_
"	tblSiteCartProducts.Title, "&_
"	tblSiteCartProducts.Tag, "&_
"	tblSiteCartProducts.More, "&_
"	tblSiteCartProducts.Data "&_
"FROM	tblSiteCartProducts WITH (nolock) "&_
"WHERE tblSiteCartProducts.SiteID IN ("& sSiteIDs &") "&_
"ORDER BY tblSiteCartProducts.Type, tblSiteCartProducts.Title"&_
""

call writeMsg ("strQuery",strQuery)
'response.write strQuery
'response.end
	rsObj.open strQuery, objConn
	if rsObj.EOF then
call writeMsg ("No products found","")
	else
		while not rsObj.EOF
			ReDim arrProduct(udtProduct_props_ct)

			arrProduct(udtProduct__iSiteID) = rsObj("SiteID")
			arrProduct(udtProduct__sProductID) = rsObj("IDData")
			arrProduct(udtProduct__sType) = rsObj("Type")
			arrProduct(udtProduct__sTitle) = rsObj("Title")
			arrProduct(udtProduct__sTag) = rsObj("Tag")
			arrProduct(udtProduct__sMore) = rsObj("More")
			arrProduct(udtProduct__sData) = rsObj("Data")
if not isNull(arrProduct(udtProduct__sMore)) then
arrProduct(udtProduct__sMore) = replace(arrProduct(udtProduct__sMore),"&","&amp;")
end if

call writeMsg ("row #",udtProduct_ct &" - "& arrProduct(udtProduct__sProductID) &" - "& arrProduct(udtProduct__sType) &" - "& arrProduct(udtProduct__sTitle))
			udtProduct_ct = udtProduct_ct + 1
			select case arrProduct(udtProduct__sType)
			case "RA PACKAGE"
call writeMsg ("is RA PACKAGE","yep")
''				arrProduct(udtProduct__sBaseURL) = ""
				if sPackageIDs<>"" then
					sPackageIDs = sPackageIDs & ","
				end if
				sPackageIDs = sPackageIDs & arrProduct(udtProduct__sProductID)
			case "RA SITE"
call writeMsg ("is RA SITE","yep")
''				arrProduct(udtProduct__sBaseURL) = rsObj("BaseURL")
				sSiteIDs = sSiteIDs & ","
				sSiteIDs = sSiteIDs & arrProduct(udtProduct__sProductID)
			case else
			end select

			ReDim Preserve udtProduct(udtProduct_ct)
			udtProduct(udtProduct_ct-1) = arrProduct

			rsObj.moveNext
		wend

	end if
	rsObj.close

call writeMsg ("sPackageIDs",sPackageIDs)

	if sPackageIDs<>"" then

	strQuery = ""&_
"SELECT DISTINCT "&_
"	tblSiteAssignments.SiteID "&_
"FROM	tblSiteAssignments WITH (nolock) "&_
"WHERE tblSiteAssignments.PackageID IN ("& sPackageIDs &") "&_
""

call writeMsg ("strQuery",strQuery)
'response.write strQuery
'response.end
	rsObj.open strQuery, objConn
	if rsObj.EOF then
call writeMsg ("No site assignments found","")
	else
		while not rsObj.EOF
			sSiteIDs = sSiteIDs & ","
			sSiteIDs = sSiteIDs & rsObj("SiteID")

			rsObj.moveNext
		wend

	end if
	rsObj.close

	end if

call writeMsg ("sSiteIDs",sSiteIDs)
'response.write sSiteIDs
'response.end

end if


if 0 then
	strQuery = ""&_
"SELECT DISTINCT "&_
"	tblSiteCartProducts.SiteID, "&_
"	tblSiteCartProducts.IDData, "&_
"	tblSiteCartProducts.Type, "&_
"	tblSiteCartProducts.Title, "&_
"	tblSiteCartProducts.Tag, "&_
"	tblSiteCartProducts.More, "&_
"	tblSiteCartProducts.Data, "&_
"	tblSiteKey.BaseURL "&_
"FROM	tblSiteCartProducts WITH (nolock) "&_
"	LEFT JOIN tblSiteKey WITH (nolock) ON (tblSiteCartProducts.Type='RA SITE' AND tblSiteCartProducts.IDData=tblSiteKey.SiteID) "&_
"WHERE tblSiteCartProducts.SiteID IN ("& sSiteIDs &") "&_
"ORDER BY tblSiteCartProducts.Type, tblSiteCartProducts.Title"&_
""

call writeMsg ("strQuery",strQuery)
'response.write strQuery
'response.end
	rsObj.open strQuery, objConn
	if rsObj.EOF then
call writeMsg ("No products found","")
	else
		while not rsObj.EOF
			ReDim arrProduct(udtProduct_props_ct)

			arrProduct(udtProduct__iSiteID) = rsObj("SiteID")
			arrProduct(udtProduct__sProductID) = rsObj("IDData")
			arrProduct(udtProduct__sType) = rsObj("Type")
			arrProduct(udtProduct__sTitle) = rsObj("Title")
			arrProduct(udtProduct__sTag) = rsObj("Tag")
			arrProduct(udtProduct__sMore) = rsObj("More")
			arrProduct(udtProduct__sData) = rsObj("Data")
arrProduct(udtProduct__sMore) = replace(arrProduct(udtProduct__sMore),"&","&amp;")

call writeMsg ("row #",udtProduct_ct &" - "& arrProduct(udtProduct__sProductID) &" - "& arrProduct(udtProduct__sType) &" - "& arrProduct(udtProduct__sTitle))
			udtProduct_ct = udtProduct_ct + 1
			select case arrProduct(udtProduct__sType)
			case "RA SITE"
call writeMsg ("is RA SITE","yep")
''				arrProduct(udtProduct__sBaseURL) = rsObj("BaseURL")
				sSiteIDs = sSiteIDs & ","
				sSiteIDs = sSiteIDs & arrProduct(udtProduct__sProductID)
			case else
''				arrProduct(udtProduct__sBaseURL) = "null"
			end select

			ReDim Preserve udtProduct(udtProduct_ct)
			udtProduct(udtProduct_ct-1) = arrProduct

			rsObj.moveNext
		wend

	end if
	rsObj.close

call writeMsg ("sSiteIDs",sSiteIDs)
end if



	strQuery = ""&_
"SELECT DISTINCT "&_
"	tblPackage.PackageID, "&_
"	tblPackage.Description, "&_
"	tblPackage.Type, "&_
"	tblSiteAssignments.AssignmentID, "&_
"	tblSiteAssignments.SiteID, "&_
"	tblSiteKey.BaseURL, "&_
"	convert(varchar(255),tblSiteKey.SiteDescription) AS SiteDesc, "&_
"	tblLevelType.LevelOfAccess "&_
"FROM	tblPackage WITH (nolock) "&_
"	INNER JOIN tblSiteAssignments (nolock) "&_
"		ON tblPackage.PackageID = tblSiteAssignments.PackageID "&_
"	LEFT JOIN tblSiteKey WITH (nolock) "&_
"		ON tblSiteKey.SiteID = tblSiteAssignments.SiteID "&_
"	INNER JOIN tblLevelType WITH (nolock) "&_
"		ON tblLevelType.LevelID = tblSiteAssignments.LevelID "&_
"WHERE tblSiteAssignments.SiteID IN ("& sSiteIDs &") "&_
"ORDER BY tblPackage.PackageID, tblSiteAssignments.SiteID, tblLevelType.LevelOfAccess DESC"&_
""
''"	'description blocked' AS Description, "&_
''"WHERE tblSiteAssignments.SiteID IN ("& sSiteIDs &") OR tblPackage.PackageID IN ("& sPackageIDs &") "&_

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
			arrSiteAssignment(udtSiteAssignment__sBaseURL) = rsObj("BaseURL")
			arrSiteAssignment(udtSiteAssignment__sSiteDesc) = rsObj("SiteDesc")
call writeMsg ("row #",udtSiteAssignment_ct &" - "& arrSiteAssignment(udtSiteAssignment__iSiteID) &" - "& arrSiteAssignment(udtSiteAssignment__iPackageID))
			udtSiteAssignment_ct = udtSiteAssignment_ct + 1
			if udtSiteAssignment_ct > 1 OR sPackageIDs<>"" then
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
"	tblBatchKey.Type, "&_
"	tblBatchKey.Price "&_
"FROM	tblBatchKey WITH (nolock) "&_
"WHERE tblBatchKey.PackageID IN ("& sPackageIDs &") "&_
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
''			arrBatch(udtBatch__bSuspended) = CInt(rsObj("Suspended"))
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
	end if




	dim newP
	for udtSite_i = 0 to udtSite_ct-1

sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<udtSite>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<iSiteID>"& udtSite(udtSite_i)(udtSite__iSiteID) &"</iSiteID>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sBaseURL>"& udtSite(udtSite_i)(udtSite__sBaseURL) &"</sBaseURL>"

		for udtSiteAssignment_i = 0 to udtSiteAssignment_ct-1
			newP = false
''			if udtSite(udtSite_i)(udtSite__iSiteID) = udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iSiteID) then

			if udtSiteAssignment_i > 0 then
				if udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iPackageID) <> udtSiteAssignment(udtSiteAssignment_i-1)(udtSiteAssignment__iPackageID) then
					newP = true
				end if
			else
				newP = true
			end if

			if newP then
				if udtSiteAssignment_i > 0 then
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"</udtPackage>"
				end if
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<udtPackage>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<iPackageID>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iPackageID) &"</iPackageID>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sDescription>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__sDescription) &"</sDescription>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sType>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__sType) &"</sType>"

				for udtBatch_i = 0 to udtBatch_ct-1
					if udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iPackageID) = udtBatch(udtBatch_i)(udtBatch__iPackageID) then

sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<udtBatch>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<iBatchID>"& udtBatch(udtBatch_i)(udtBatch__iBatchID) &"</iBatchID>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sDescription>"& udtBatch(udtBatch_i)(udtBatch__sDescription) &"</sDescription>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<dtUseByDate>"& udtBatch(udtBatch_i)(udtBatch__dtUseByDate) &"</dtUseByDate>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<iRelativeExpiration>"& udtBatch(udtBatch_i)(udtBatch__iRelativeExpiration) &"</iRelativeExpiration>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<dtAbsoluteExpiration>"& udtBatch(udtBatch_i)(udtBatch__dtAbsoluteExpiration) &"</dtAbsoluteExpiration>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<bSuspended>"& udtBatch(udtBatch_i)(udtBatch__bSuspended) &"</bSuspended>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sType>"& udtBatch(udtBatch_i)(udtBatch__sType) &"</sType>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<mPrice>"& udtBatch(udtBatch_i)(udtBatch__mPrice) &"</mPrice>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"</udtBatch>"

					end if
				next
			end if

sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<udtSiteAssignment>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<iSiteAssignmentID>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iSiteAssignmentID) &"</iSiteAssignmentID>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<iLevelOfAccess>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iLevelOfAccess) &"</iLevelOfAccess>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<iSiteID>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__iSiteID) &"</iSiteID>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sBaseURL>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__sBaseURL) &"</sBaseURL>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sSiteDesc>"& udtSiteAssignment(udtSiteAssignment_i)(udtSiteAssignment__sSiteDesc) &"</sSiteDesc>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"</udtSiteAssignment>"

			if udtSiteAssignment_i = udtSiteAssignment_ct-1 then
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"</udtPackage>"
			end if

''			end if
		next


		for udtProduct_i = 0 to udtProduct_ct-1
			if udtSite(udtSite_i)(udtSite__iSiteID) = udtProduct(udtProduct_i)(udtProduct__iSiteID) then

sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<udtProduct>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<iSiteID>"& udtProduct(udtProduct_i)(udtProduct__iSiteID) &"</iSiteID>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sProductID>"& udtProduct(udtProduct_i)(udtProduct__sProductID) &"</sProductID>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sType>"& udtProduct(udtProduct_i)(udtProduct__sType) &"</sType>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sTitle>"& udtProduct(udtProduct_i)(udtProduct__sTitle) &"</sTitle>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sTag>"& udtProduct(udtProduct_i)(udtProduct__sTag) &"</sTag>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sMore>"& udtProduct(udtProduct_i)(udtProduct__sMore) &"</sMore>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sData>"& udtProduct(udtProduct_i)(udtProduct__sData) &"</sData>"
''sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"<sBaseURL>"& udtProduct(udtProduct_i)(udtProduct__sBaseURL) &"</sBaseURL>"
sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"</udtProduct>"

			end if
		next

sGetSiteFromBaseURLResult = sGetSiteFromBaseURLResult &VBCRLF&"</udtSite>"

	next

	set rsObj = nothing

	objConn.close
	set objConn = nothing

goToFinish

end sub
%>

