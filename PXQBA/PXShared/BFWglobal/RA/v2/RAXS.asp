<% Response.Buffer = True
Response.contenttype = "text/javascript"

%>
<!--#include file="./RAXS_server.asp"-->
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<%

dim responseStr : responseStr = ""
responseStr = responseStr& VBCRLF
responseStr = responseStr&"try {"& VBCRLF
'Response.Write responseStr
'response.end

dim inRAlr_uid : inRAlr_uid = Request.QueryString("RAlr_uid")
debugWrite "inRAlr_uid", inRAlr_uid

debugWrite "rau", Request.QueryString("rau")

dim t : t = Request.QueryString("t")
debugWrite "t", t

dim inRASite : inRASite = Request.QueryString("s")
dim DoRASite : DoRASite = false
if inRASite = "1" then
	DoRASite = true
else
''	inRASite = "0"
end if
debugWrite "DoRASite", DoRASite

dim inBSI : inBSI = Request.QueryString("bsi")
dim DoBSI : DoBSI = false
if inBSI = "1" then
	DoBSI = true
else
	inBSI = "0"
end if
debugWrite "DoBSI", DoBSI

dim inRAUser : inRAUser = Request.QueryString("u")
dim DoRAUser : DoRAUser = false
if inRAUser = "1" then
	DoRAUser = true
else
''	inRAUser = "0"
end if
debugWrite "DoRAUser", DoRAUser

dim inRAURL : inRAURL = Request.QueryString("url")
dim redirectQstrURL : redirectQstrURL = inRAURL
debugWrite "inRAURL", inRAURL
inRAURL = replace(inRAURL,"http://","")
if inRAURL = "" then
	DoRASite = false
''	inRASite = "0"
end if
if not DoRASite then
	inRAURL = ""
''	redirectQstrURL = ""
end if
debugWrite "inRAURL", inRAURL
dim strRAURLs : strRAURLs = "'"& replace(inRAURL,"'","''") &"'"

''response.end

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


dim RAUserID
dim RAUserEmail
dim RAUserFName
dim RAUserLName
dim RAUserPasswordHint
dim RAUserMailPreference
dim RAUserOptInEmail
dim RAUserRememberMe
dim RAUserClassPrompt
dim RAUserSiteIDs

dim arrSs, arrSLs, arrSAs, arrPs, arrCs, arrValues, i, j

responseStr = responseStr& VBCRLF

'' ***************************************************************
'' RAUser Classic X-Site
'' ***************************************************************
if DoRAUser then

debugWrite "", ""
debugWrite "X-Site ?", inRAlr_uid &" - "& Request.QueryString("rau") &" - "& Request.Cookies("RAUserID") &" - "& Request.Cookies("RAUserProfile")
debugWrite "", ""

if inRAlr_uid = "" then
	dim oldXSreturl
	oldXSreturl = RAXSRootURL&"/RAXS.asp?u="&inRAUser&"&s="&inRASite&"&url="&Server.URLEncode(redirectQstrURL)&"&t="&Request.QueryString("t")
	if Request.QueryString("rau") = "" then
		oldXSreturl = oldXSreturl &"&returned=oldxs"
debugWrite "X-Site 1", oldXSreturl
		response.redirect RALoginRefURL &"?m=c&returl="& Server.URLEncode(oldXSreturl) &""
		response.end
	elseif Request.QueryString("rau") <> 0 AND Request.QueryString("rau") <> Request.Cookies("RAUserID") AND (Request.QueryString("returned") <> "login" OR Request.Cookies("RAUserProfile") = "") then
debugWrite "X-Site 2", oldXSreturl
		oldXSreturl = oldXSreturl &"&returned=login"
		response.redirect RAXSRootURL &"/RAXS_Login.asp?uid="&Request.QueryString("rau")&"&returl="& Server.URLEncode(oldXSreturl) &""
		response.end
	elseif Request.QueryString("rau") = 0 AND Request.QueryString("returned") <> "logout" AND Request.Cookies("RAUserProfile") <> "" then
debugWrite "X-Site 3", oldXSreturl
		oldXSreturl = oldXSreturl &"&returned=logout"
		response.redirect RAXSRootURL &"/RAXS_Logout.asp?uid="&Request.QueryString("rau")&"&returl="& Server.URLEncode(oldXSreturl) &""
		response.end
	end if

elseif inRAlr_uid <> Request.Cookies("RAUserID") AND (Request.QueryString("returned") <> "login" OR Request.Cookies("RAUserProfile") = "") then
debugWrite "JSX-Site 1", oldXSreturl
	oldXSreturl = oldXSreturl &"&returned=login"
	response.redirect RAXSRootURL &"/RAXS_Login.asp?uid="&inRAlr_uid&"&returl="& Server.URLEncode(oldXSreturl) &""
	response.end
elseif inRAlr_uid = 0 AND Request.QueryString("returned") <> "logout" AND Request.Cookies("RAUserProfile") <> "" then
debugWrite "JSX-Site 2", oldXSreturl
	oldXSreturl = oldXSreturl &"&returned=logout"
	response.redirect RAXSRootURL &"/RAXS_Logout.asp?uid="&inRAlr_uid&"&returl="& Server.URLEncode(oldXSreturl) &""
	response.end
end if

'response.write responseStr
'response.end
end if '' DoRAUser


'' ***************************************************************
'' RASite
'' ***************************************************************
if DoRASite AND inRAURL <> "" then
	debugWrite "inRAURL", inRAURL
	while inStr(inRAURL,"/")
		inRAURL = left( inRAURL, inStrRev(inRAURL,"/")-1 )
		debugWrite "inRAURL", inRAURL
		strRAURLs = strRAURLs &",'"& replace(inRAURL,"'","''") &"'"
	wend
	debugWrite "strRAURLs", strRAURLs
''	response.end

%>
<%
	dim objConn
	dim rsObj
	dim strQuery
	set rsObj = Server.CreateObject("ADODB.Recordset")
	ConnectToBFWUsersDB
	CheckForErrors(objConn)

	strQuery = ""&_
"SELECT Top 1 "&_
"	SiteID, "&_
"	BaseURL "&_
"FROM         tblSiteKey WITH (nolock) "&_
"WHERE     BaseURL IN ("& strRAURLs &") "&_
"ORDER BY BaseURL DESC"&_
""
debugWrite "strQuery", strQuery
	rsObj.open strQuery, objConn
	If rsObj.EOF Then
debugWrite "x", "...no matching records found"
	Else
debugWrite "SiteID", rsObj("SiteID")
		while not rsObj.EOF
			ReDim arrSite(udtSite_props_ct)

			arrSite(udtSite__iSiteID) = rsObj("SiteID")
			arrSite(udtSite__sBaseURL) = rsObj("BaseURL")

			udtSite_ct = udtSite_ct + 1
''debugWrite "row #", udtSite_ct
			if udtSite_ct > 1 then
				sSiteIDs = sSiteIDs & ","
			end if
			sSiteIDs = sSiteIDs & arrSite(udtSite__iSiteID)
			ReDim Preserve udtSite(udtSite_ct)
			udtSite(udtSite_ct-1) = arrSite

			rsObj.moveNext
		wend
	End If
	rsObj.close

debugWrite "site ct", udtSite_ct
debugWrite "sSiteIDs", sSiteIDs
	if sSiteIDs <> "" then

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
"WHERE tblSiteAssignments.SiteID IN ("& sSiteIDs &") "&_
"ORDER BY tblSiteAssignments.SiteID, tblPackage.PackageID"&_
""
''"	'description blocked' AS Description, "&_

debugWrite "strQuery",strQuery
'response.write strQuery
'response.end
	rsObj.open strQuery, objConn
	if rsObj.EOF then
debugWrite "No packages found",""
	else
		while not rsObj.EOF
			ReDim arrSiteAssignment(udtSiteAssignment_props_ct)

			arrSiteAssignment(udtSiteAssignment__iSiteID) = rsObj("SiteID")
			arrSiteAssignment(udtSiteAssignment__iPackageID) = rsObj("PackageID")
			arrSiteAssignment(udtSiteAssignment__sDescription) = rsObj("Description")
			arrSiteAssignment(udtSiteAssignment__sType) = rsObj("Type")
			arrSiteAssignment(udtSiteAssignment__iSiteAssignmentID) = rsObj("AssignmentID")
			arrSiteAssignment(udtSiteAssignment__iLevelOfAccess) = rsObj("LevelOfAccess")

			udtSiteAssignment_ct = udtSiteAssignment_ct + 1
''debugWrite "row #",udtSiteAssignment_ct
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

debugWrite "site assignment ct",udtSiteAssignment_ct
debugWrite "sPackageIDs", sPackageIDs
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
"WHERE tblBatchKey.PackageID IN ("& sPackageIDs &") "&_
"ORDER BY tblBatchKey.PackageID, tblBatchKey.AbsoluteExpiration DESC"&_
""

debugWrite "strQuery",strQuery
'response.write strQuery
'response.end
	rsObj.open strQuery, objConn
	if rsObj.EOF then
debugWrite "No packages found",""
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

			udtBatch_ct = udtBatch_ct + 1
''debugWrite "row #",udtBatch_ct
			ReDim Preserve udtBatch(udtBatch_ct)
			udtBatch(udtBatch_ct-1) = arrBatch

			rsObj.moveNext
		wend

	end if
	rsObj.close

debugWrite "batch ct",udtBatch_ct

	end if 'sPackageIDs

	end if 'sSiteIDs

dim donePIDs : donePIDs = ","

''responseStr = responseStr& "var RAXS_S, RAXS_P, RAXS_SA, RAXS_B;"&VBCRLF
	for i=0 to udtSite_ct-1
debugWrite "site",udtSite(i)(udtSite__iSiteID)
responseStr = responseStr& "RA_CtrlWin.RA.CurrentSite = RA_CtrlWin.RA.AddSite( "& udtSite(i)(udtSite__iSiteID) &", '"& udtSite(i)(udtSite__sBaseURL) &"', '' );"&VBCRLF

responseStr = responseStr& "//alert(RA_CtrlWin.RA.CurrentSite);"& VBCRLF

		for j=0 to udtSiteAssignment_ct-1
debugWrite "siteassignment",udtSiteAssignment(j)(udtSiteAssignment__iPackageID)
			if inStr(donePIDs,","&udtSiteAssignment(j)(udtSiteAssignment__iPackageID)&",") = 0 then
responseStr = responseStr& "var RAXS_P"&udtSiteAssignment(j)(udtSiteAssignment__iPackageID)&" = RA_CtrlWin.RA.AddPackage( "& udtSiteAssignment(j)(udtSiteAssignment__iPackageID) &", '"& udtSiteAssignment(j)(udtSiteAssignment__sDescription) &"', '', '"& udtSiteAssignment(j)(udtSiteAssignment__sType) &"' );"&VBCRLF
				donePIDs = donePIDs & udtSiteAssignment(j)(udtSiteAssignment__iPackageID) &","
			end if

responseStr = responseStr& "var RAXS_SA"&j&" = RA_CtrlWin.RA.AddSiteAssignment( "& udtSiteAssignment(j)(udtSiteAssignment__iSiteAssignmentID) &", RAXS_P"&udtSiteAssignment(j)(udtSiteAssignment__iPackageID)&", RA_CtrlWin.RA.CurrentSite, '"& udtSiteAssignment(j)(udtSiteAssignment__iLevelOfAccess) &"' );"&VBCRLF

		next

	next

	for k=0 to udtBatch_ct-1
debugWrite "batch",udtBatch(k)(udtBatch__iBatchID)
responseStr = responseStr& "var RAXS_B"&j&" = RA_CtrlWin.RA.AddBatch( "& udtBatch(k)(udtBatch__iBatchID) &", '"& udtBatch(k)(udtBatch__sDescription) &"', RAXS_P"&udtBatch(k)(udtBatch__iPackageID)&", '"& udtBatch(k)(udtBatch__dtUseByDate) &"', '"& udtBatch(k)(udtBatch__iRelativeExpiration) &"', '"& udtBatch(k)(udtBatch__dtAbsoluteExpiration) &"', "& udtBatch(k)(udtBatch__bSuspended) &"==1?true:false, '"& udtBatch(k)(udtBatch__sType) &"', '"& udtBatch(k)(udtBatch__mPrice) &"' );"&VBCRLF

	next

'	responseStr = responseStr& "RA_CtrlWin.RA.CurrentUser = RA_CtrlWin.RA.AddUser( "& RAUserID &", '"&

'			S = RA_CtrlWin.RA.AddSite( RAWS_udtSites[iS].SiteID, RAWS_udtSites[iS].BaseURL, '' );
'			RA_CtrlWin.RA.CurrentSite = S;
'			RA_CtrlWin.RA.CurrentSiteInited = true;
'			for (var iP in RAWS_udtSites[iS].Packages) {if (RAWS_udtSites[iS].Packages.hasOwnProperty(iP)) {

'				P = RA_CtrlWin.RA.AddPackage( RAWS_udtSites[iS].Packages[iP].PackageID, RAWS_udtSites[iS].Packages[iP].Description, RAWS_udtSites[iS].Packages[iP].Type );

'				SA = RA_CtrlWin.RA.AddSiteAssignment( RAWS_udtSites[iS].Packages[iP].SiteAssignmentID, P, S, RAWS_udtSites[iS].Packages[iP].LevelOfAccess );
'				for (var iB in RAWS_udtSites[iS].Packages[iP].Batches) {if (RAWS_udtSites[iS].Packages[iP].Batches.hasOwnProperty(iB)) {

'					B = RA_CtrlWin.RA.AddBatch( RAWS_udtSites[iS].Packages[iP].Batches[iB].BatchID, RAWS_udtSites[iS].Packages[iP].Batches[iB].Description, P, RAWS_udtSites[iS].Packages[iP].Batches[iB].UseByDate, RAWS_udtSites[iS].Packages[iP].Batches[iB].RelativeExpiration,  RAWS_udtSites[iS].Packages[iP].Batches[iB].AbsoluteExpiration,  RAWS_udtSites[iS].Packages[iP].Batches[iB].Suspended, RAWS_udtSites[iS].Packages[iP].Batches[iB].Type, RAWS_udtSites[iS].Packages[iP].Batches[iB].Price );
'				}}
'			}}
'		}}




'' ***************************************************************
'' BSISite
'' ***************************************************************
	if DoBSI then
	end if

	set rsObj = nothing
	objConn.close
	set objConn = nothing
end if
'debugWrite "responseStr", responseStr
'response.end

'' ***************************************************************
'' RAUser
'' ***************************************************************
if DoRAUser then

if Request.Cookies("RAUserProfile") = "" then
		responseStr = responseStr& "RA_CtrlWin.RA.CurrentUser = null"&VBCRLF
else

'' +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
'' RAUser Profile
'' +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
'response.write "<br>"
'response.write Request.Cookies("RAUserProfile")
'response.write "<br>"
'response.write "<br>"
	arrUP = split(Request.Cookies("RAUserProfile"),"|||")
'' +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
''RAUserProfile_str = RAUserProfile_str& RACU_iUserID
''RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sUserName
''RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sFirstName
''RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sLastName
''RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sPasswordHint
''RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sMailPreference
''RAUserProfile_str = RAUserProfile_str&"|||"& RACU_sOptInEmail
''RAUserProfile_str = RAUserProfile_str&"|||"& RAUserClassPrompt
	RAUserID = arrUP(0)
	RAUserEmail = arrUP(1)
	RAUserFName = arrUP(2)
	RAUserLName = arrUP(3)
	RAUserPasswordHint = arrUP(4)
	RAUserMailPreference = arrUP(5)
	RAUserOptInEmail = arrUP(6)
	RAUserClassPrompt = arrUP(7)
''	RAUserRememberMe = Request.Cookies("RAUserRememberMe")
	RAUserRememberMe = 0
		if 0 then
	RAUserID = Request.Cookies("RAUserID")
	RAUserEmail = Request.Cookies("RAUserEmail")
	RAUserFName = Request.Cookies("RAUserFName")
	RAUserLName = Request.Cookies("RAUserLName")
	RAUserPasswordHint = Request.Cookies("RAUserPasswordHint")
	RAUserMailPreference = Request.Cookies("RAUserMailPreference")
	RAUserOptInEmail = Request.Cookies("RAUserOptInEmail")
''	RAUserRememberMe = Request.Cookies("RAUserRememberMe")
	RAUserClassPrompt = Request.Cookies("RAUserClassPrompt")
		end if

	responseStr = responseStr& "RA_CtrlWin.RA.CurrentUser = RA_CtrlWin.RA.AddUser( "& RAUserID &", '"& replace(RAUserEmail,"'","\'") &"', '"& replace(RAUserFName,"'","\'") &"', '"& replace(RAUserLName,"'","\'") &"', '"& replace(RAUserPasswordHint,"'","\'") &"', '"& RAUserMailPreference &"', '"& RAUserOptInEmail &"', '"& RAUserRememberMe &"', '"& RAUserClassPrompt &"' );"&VBCRLF
	responseStr = responseStr& "RA_CtrlWin.RA.CurrentUser.IP = '"& Request.ServerVariables("REMOTE_ADDR") &"'"& VBCRLF


'' +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
'' RAUser Packages
'' +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
'response.write "<br>"
'response.write Request.Cookies("RAUserPkgs")
'response.write "<br>"
'response.write "<br>"
	if "" <> Request.Cookies("RAUserPkgs") then
		arrPs = split(Request.Cookies("RAUserPkgs"),"|RA|")
'' +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		for i=0 to UBound(arrPs)
'response.write arrPs(i)
'response.write "<br>"
			if "" <> arrPs(i) then
				arrValues = split( arrPs(i), "|||")
				responseStr = responseStr& "RA_CtrlWin.RA.AddPackage( "& arrValues(0) &", '-' );"&VBCRLF
				responseStr = responseStr& "RA_CtrlWin.RA.AddUserAssignment( '', RA_CtrlWin.RA.CurrentUser, RA_CtrlWin.RA.Packages['"& arrValues(0) &"'], '', '"& arrValues(1) &"' );"&VBCRLF
			end if
		next
	end if
'' +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
'' RAUser Classes
'' +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
'response.write "<br>"
'response.write Request.Cookies("RAUserClasses")
'response.write "<br>"
'response.write "<br>"
	if "" <> Request.Cookies("RAUserClasses") then
		arrCs = split(Request.Cookies("RAUserClasses"),"|RA|")
'' +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
'' const RAWS_udtClassInfo__iClassID = 0
'' const RAWS_udtClassInfo__iCreatorID = 1
'' const RAWS_udtClassInfo__sClassName = 2
'' const RAWS_udtClassInfo__sClassDesc = 3
'' const RAWS_udtClassInfo__sClassCode = 4
'' const RAWS_udtClassInfo__dtExprn = 5
'' const RAWS_udtClassInfo__iUserID = 6
'' const RAWS_udtClassInfo__bClassAccessRevoked = 7
'' const RAWS_udtClassInfo__dtLastLogin = 8
'' const RAWS_udtClassInfo__dtStartDate = 9
'' const RAWS_udtClassInfo__dtEndDate = 10
'' const RAWS_udtClassInfo__bEmailScores = 11
'' const RAWS_udtClassInfo__iRecordStatus = 12
'' const RAWS_udtClassInfo__iCreatorEmail = 13
'' const RAWS_udtClassInfo__iCreatorFName = 14
'' const RAWS_udtClassInfo__iCreatorLName = 15
'' RAObj.prototype.AddUser = function(
'' 1	ID,
'' 13	Email,
'' 14	FName,
'' 15	LName,
'' ?	PasswordHint,
'' ?	MailPreference,
'' ?	OptInEmail,
'' ?	RememberMe,
'' ?	ClassPrompt
'' )
''
		responseStr = responseStr& "var RAXS_ClassCreators = new Object();"&VBCRLF
		for i=0 to UBound(arrCs)
'response.write arrCs(i)
'response.write "<br>"
			if "" <> arrCs(i) then
				arrValues = split( arrCs(i), "|||")
'response.write arrValues(11)
'response.write "<br>"
				if RAUserID = arrValues(1) then
					responseStr = responseStr& "RAXS_ClassCreators["& arrValues(0) &"] = RA_CtrlWin.RA.CurrentUser;"&VBCRLF
				else
					responseStr = responseStr& "RAXS_ClassCreators["& arrValues(0) &"] = RA_CtrlWin.RA.AddUser( "& arrValues(1) &", '"& replace(arrValues(13),"'","\'") &"', '"& replace(arrValues(14),"'","\'") &"', '"& replace(arrValues(15),"'","\'") &"', '', '', '', '', '' );"&VBCRLF
				end if
			end if
		next
'' +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
'' const RAWS_udtClassInfo__iClassID = 0
'' const RAWS_udtClassInfo__iCreatorID = 1
'' const RAWS_udtClassInfo__sClassName = 2
'' const RAWS_udtClassInfo__sClassDesc = 3
'' const RAWS_udtClassInfo__sClassCode = 4
'' const RAWS_udtClassInfo__dtExprn = 5
'' const RAWS_udtClassInfo__iUserID = 6
'' const RAWS_udtClassInfo__bClassAccessRevoked = 7
'' const RAWS_udtClassInfo__dtLastLogin = 8
'' const RAWS_udtClassInfo__dtStartDate = 9
'' const RAWS_udtClassInfo__dtEndDate = 10
'' const RAWS_udtClassInfo__bEmailScores = 11
'' const RAWS_udtClassInfo__iRecordStatus = 12
'' const RAWS_udtClassInfo__iCreatorEmail = 13
'' const RAWS_udtClassInfo__iCreatorFName = 14
'' const RAWS_udtClassInfo__iCreatorLName = 15
'' RAObj.prototype.AddClass = function(
'' 0	ID,
'' 2	Title,
'' 3	Desc,
'' 4	Code,
'' 5	Exiration,
'' xx0	Creator_User,
'' 9	StartDate,
'' 10	EndDate,
'' 11	EmailScores,
'' 12	RecordStatus
'' )
''
		for i=0 to UBound(arrCs)
'response.write arrCs(i)
'response.write "<br>"
			if "" <> arrCs(i) then
				arrValues = split( arrCs(i), "|||")
'response.write arrValues(11)
'response.write "<br>"
				responseStr = responseStr& "RA_CtrlWin.RA.AddClass( "& arrValues(0) &", '"& replace(arrValues(2),"'","\'") &"', '"& replace(arrValues(3),"'","\'") &"', '"& replace(arrValues(4),"'","\'") &"', '"& replace(arrValues(5),"'","\'") &"', RAXS_ClassCreators["& arrValues(0) &"], '"& replace(arrValues(9),"'","\'") &"', '"& replace(arrValues(10),"'","\'") &"', "& arrValues(11) &", "& arrValues(12) &" );"&VBCRLF
			end if
		next
'' +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
'' const RAWS_udtClassInfo__iClassID = 0
'' const RAWS_udtClassInfo__iCreatorID = 1
'' const RAWS_udtClassInfo__sClassName = 2
'' const RAWS_udtClassInfo__sClassDesc = 3
'' const RAWS_udtClassInfo__sClassCode = 4
'' const RAWS_udtClassInfo__dtExprn = 5
'' const RAWS_udtClassInfo__iUserID = 6
'' const RAWS_udtClassInfo__bClassAccessRevoked = 7
'' const RAWS_udtClassInfo__dtLastLogin = 8
'' const RAWS_udtClassInfo__dtStartDate = 9
'' const RAWS_udtClassInfo__dtEndDate = 10
'' const RAWS_udtClassInfo__bEmailScores = 11
'' const RAWS_udtClassInfo__iRecordStatus = 12
'' const RAWS_udtClassInfo__iCreatorEmail = 13
'' const RAWS_udtClassInfo__iCreatorFName = 14
'' const RAWS_udtClassInfo__iCreatorLName = 15
'' RAObj.prototype.AddClassLogin = function(
''x 	User,
''x0 	Class,
''7 	AccessRevoked,
''8 	LastLogin
'' )
''
		for i=0 to UBound(arrCs)
'response.write arrCs(i)
'response.write "<br>"
			if "" <> arrCs(i) then
				arrValues = split( arrCs(i), "|||")
'response.write arrValues(11)
'response.write "<br>"
				responseStr = responseStr& "RA_CtrlWin.RA.AddClassLogin( RA_CtrlWin.RA.CurrentUser, RA_CtrlWin.RA.Classes['"& arrValues(0) &"'], '"& arrValues(7) &"', '"& replace(arrValues(8),"'","\'") &"' );"&VBCRLF
			end if
		next
	end if


end if

end if '' DoRAUser

responseStr = responseStr &"//alert('1: '+RA_CtrlWin.RAXSInitialized);"& VBCRLF
responseStr = responseStr &"RA_CtrlWin.RA.RAXS.Inited = true;"& VBCRLF
responseStr = responseStr &"//alert('2: '+RA_CtrlWin.RAXSInitialized);"& VBCRLF


''responseStr = responseStr &"} catch(e) {window.open('"& RAXSRootURL &"/RAXS_LogError.asp?to=chad&s=100&url='+ encodeURIComponent('"& Request.QueryString("url") &"') +'&msg='+ encodeURIComponent(e.name+' --- '+e.message) +'','RAXS_LogError_win','width=300,height=100,left=0,top=0,location=no,menubar=no,scrollbars=yes,titlebar=no,toolbar=no')} //main try-catch"& VBCRLF

responseStr = responseStr &"} catch(e) {window.open('http://bcs.bfwpub.com/RA/RAXS_LogError.asp?to=chad&s=100&url='+ encodeURIComponent('"& Request.QueryString("url") &"') +'&msg='+ encodeURIComponent(e.name+' --- '+e.message) +'','RAXS_LogError_win','width=300,height=100,left=0,top=0,location=no,menubar=no,scrollbars=yes,titlebar=no,toolbar=no')} //main try-catch"& VBCRLF

Response.Write responseStr












''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
function writeJSVar( i_varname, i_var)
	dim xxx
	xxx = ""

	If IsArray( i_var ) then
	'If it is an array, loop through each element one at a time
xxx = xxx &_
"<TR><TD>"& i_varname &"</TD><TD><b>[ ARRAY ]</b></TD></TR>"& vbCRLF
		dim iLoop
		For iLoop = LBound( i_var ) to UBound( i_var )

xxx = xxx &_
displayVar( i_varname &"("& iLoop &")" , i_var(iLoop) )

		Next
	Else
	'We aren't dealing with an array, so just display the variable
		If isObject( i_var ) Then

xxx = xxx &_
"<TR><TD>"& i_varname &"</TD><TD><b>[ OBJECT, TYPE: "& TypeName(i_var) &" ]</b></TD></TR>"& vbCRLF

		Else

xxx = ""& i_varname &" = "& i_var & vbCRLF

		End If
	End If

	writeJSVar = xxx

end function














' *********************************************************************************
' *********************************************************************************
' *********************************************************************************
' *********************************************************************************


sub debugWrite (vs, msg)
	responseStr = responseStr& VBCRLF
	responseStr = responseStr& "/*    "
	responseStr = responseStr& vs &" :: "& msg
	responseStr = responseStr& "   */ "& VBCRLF
end sub

%>

