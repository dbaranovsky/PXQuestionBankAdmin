<%
Response.buffer = true
'Response.contenttype = "text/html"
'
Response.contenttype = "text/xml"

dim testInput : testInput = Request.QueryString("testInput")

dim i, str

dim RequestBinCt
dim RequestBin
dim RequestBinStr : RequestBinStr = ""

dim sGetPackagesResult : sGetPackagesResult = ""
dim sErrorMsg : sErrorMsg = ""

dim resultXML
dim nGet, nPackages, nSites, nUsers
dim sPackageIDs, sSiteIDs, sUserIDs
dim arrPackageIDs, arrSiteIDs, arrUserIDs

dim objConn
dim rsObj
%>
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
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
"<soap:Envelope xmlns:xsi=""http://tempuri.org/""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <GetPackages xmlns=""http://tempuri.org/"">" &_
"      <sPackageIDs>362,663</sPackageIDs>" &_
"      <sSiteIDs>24382</sSiteIDs>" &_
"      <sUserIDs></sUserIDs>" &_
"    </GetPackages>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
end if


set resultXML = Server.CreateObject("Microsoft.XMLDOM")
resultXML.async = "false"
resultXML.loadXML( RequestBinStr )

set nGet = resultXML.getElementsByTagName("GetPackages")
set nPackages = resultXML.getElementsByTagName("sPackageIDs")
set nSites = resultXML.getElementsByTagName("sSiteIDs")
set nUsers = resultXML.getElementsByTagName("sUserIDs")

checkInput

if nPackages.length = 1 then
	sPackageIDs = nPackages(0).text
	arrPackageIDs = split(sPackageIDs, ",")
end if
if nSites.length = 1 then
	sSiteIDs = nSites(0).text
	arrSiteIDs = split(sSiteIDs, ",")
end if
if nUsers.length = 1 then
	sUserIDs = nUsers(0).text
	arrUserIDs = split(sUserIDs, ",")
end if

DB_GetPackages

goToFinish




sub checkInput ()
	if nGet.length = 0 then
		sErrorMsg = "Invalid input: no GetPackages element"
		goToFinish
	end if

	str = nPackages.length &" "& nSites.length &" "& nUsers.length
	if str = "0 0 0" then
		sErrorMsg = "Invalid input: no sPackageIDs or sSiteIDs or sUserIDs elements"
		goToFinish
	end if
	if not ( str="1 1 1" or str="1 1 0" or str="1 0 1" or str="0 1 1" or str="1 0 0" or str="0 1 0" or str="0 0 1" ) then
		sErrorMsg = "Invalid input: more than one sPackageIDs or sSiteIDs or sUserIDs elements"
		goToFinish
	end if

	str = ""
	str = str& " vPs:"
	if nPackages.length = 1 then
		str = str& nPackages(0).text
	end if
	str = str& " vSs:"
	if nSites.length = 1 then
		str = str& nSites(0).text
	end if
	str = str& " vUs:"
	if nUsers.length = 1 then
		str = str& nUsers(0).text
	end if
	if str = " vPs: vSs: vUs:" then
		sErrorMsg = "Invalid input: no sPackageIDs or sSiteIDs or sUserIDs values"
		goToFinish
	end if
end sub




sub goToFinish ()
	dim ResponseStr
	ResponseStr = "<GetPackagesResponse><GetPackagesResult>"& sGetPackagesResult &"</GetPackagesResult><sErrorMsg>"& sErrorMsg &"</sErrorMsg></GetPackagesResponse>"
	ResponseStr = "<?xml version=""1.0""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body>"& ResponseStr &"</soap:Body></soap:Envelope>"
	response.write ResponseStr
	response.end
end sub




sub DB_GetPackages ()
	set rsObj = Server.CreateObject("ADODB.Recordset")
	ConnectToBFWUsersDB
	CheckForErrors(objConn)

	strQuery = ""&_
"SELECT "&_
"	tblPackage.PackageID, "&_
"	tblPackage.Description, "&_
"	tblSiteAssignments.AssignmentID, "&_
"	tblSiteKey.SiteID, "&_
"	tblSiteKey.BaseURL, "&_
"	tblSiteKey.SiteDescription, "&_
"	tblLevelType.LevelOfAccess "&_
"FROM	tblPackage WITH (nolock) "&_
"		INNER JOIN tblSiteAssignments WITH (nolock) "&_
"			ON tblPackage.PackageID = tblSiteAssignments.PackageID "&_
"		INNER JOIN tblSiteKey WITH (nolock) "&_
"			ON tblSiteKey.SiteID = tblSiteAssignments.SiteID "&_
"		INNER JOIN tblLevelType WITH (nolock) "&_
"			ON tblLevelType.LevelID = tblSiteAssignments.LevelID "&_
"WHERE "
	if sSiteIDs <> "" and sPackageIDs <> "" then
		strQuery = strQuery&_
"( "
	end if
	if sPackageIDs <> "" then
		strQuery = strQuery&_
"tblPackage.PackageID IN ("& sPackageIDs &") "
	end if
	if sSiteIDs <> "" and sPackageIDs <> "" then
		strQuery = strQuery&_
"OR "
	end if
	if sSiteIDs <> "" then
		strQuery = strQuery&_
"tblSiteKey.SiteID IN ("& sSiteIDs &") "
	end if
	if sSiteIDs <> "" and sPackageIDs <> "" then
		strQuery = strQuery&_
") "
	end if
	if sUserIDs <> "" and (sSiteIDs <> "" or sPackageIDs <> "") then
		strQuery = strQuery&_
"AND "
	end if
	if sUserIDs <> "" then
		strQuery = strQuery&_
"(tblPackage.PackageID IN (SELECT PackageID FROM tblUserAssignments WHERE UserID IN ("& sUserIDs &"))) "
	end if
	strQuery = strQuery&_
"ORDER BY tblPackage.PackageID, tblSiteKey.SiteID, tblLevelType.LevelOfAccess DESC "&_
""
'response.write strQuery
'response.end
	rsObj.open strQuery, objConn
	If rsObj.EOF Then
'response.write "...no matching records found"
'
sGetPackagesResult = sGetPackagesResult & strQuery
	Else
		RS_Package_ct = 0
		RS_lastPackageID = ""
		RS_Site_ct = 0
		RS_lastSiteID = ""
		while not rsObj.EOF

			if RS_lastPackageID <> rsObj("PackageID") then
				if RS_Package_ct <> 0 then
sGetPackagesResult = sGetPackagesResult &"</udtSiteAssignment>"
sGetPackagesResult = sGetPackagesResult &"</udtPackage>"
				end if
				RS_Package_ct = RS_Package_ct + 1
				RS_lastPackageID = rsObj("PackageID")
				RS_Site_ct = 0
				RS_lastSiteID = ""
sGetPackagesResult = sGetPackagesResult &"<udtPackage>"
sGetPackagesResult = sGetPackagesResult &"<iPackageID>"& rsObj("PackageID") &"</iPackageID>"
sGetPackagesResult = sGetPackagesResult &"<sPackageDescription>"& rsObj("Description") &"</sPackageDescription>"
			end if

			if RS_lastSiteID <> rsObj("SiteID") then
				if RS_Site_ct <> 0 then
sGetPackagesResult = sGetPackagesResult &"</udtSiteAssignment>"
				end if
				RS_Site_ct = RS_Site_ct + 1
				RS_lastSiteID = ""
sGetPackagesResult = sGetPackagesResult &"<udtSiteAssignment>"
sGetPackagesResult = sGetPackagesResult &"<iSiteAssignmentID>"& rsObj("AssignmentID") &"</iSiteAssignmentID>"
sGetPackagesResult = sGetPackagesResult &"<iSiteID>"& rsObj("SiteID") &"</iSiteID>"
sGetPackagesResult = sGetPackagesResult &"<sSiteDescription>"& rsObj("SiteDescription") &"</sSiteDescription>"
sGetPackagesResult = sGetPackagesResult &"<sSiteBaseURL>"& rsObj("BaseURL") &"</sSiteBaseURL>"
sGetPackagesResult = sGetPackagesResult &"<iSiteLevelOfAccess>"& rsObj("LevelOfAccess") &"</iSiteLevelOfAccess>"
			end if

			rsObj.moveNext
		Wend
sGetPackagesResult = sGetPackagesResult &"</udtSiteAssignment>"
sGetPackagesResult = sGetPackagesResult &"</udtPackage>"
	End If

	rsObj.close
	set rsObj = nothing

	objConn.close
	set objConn = nothing

end sub
%>

