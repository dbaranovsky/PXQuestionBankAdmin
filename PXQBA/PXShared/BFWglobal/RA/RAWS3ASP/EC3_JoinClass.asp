<%
Response.buffer = true
'Response.contenttype = "text/html"
'
Response.contenttype = "text/xml"

'dim testInput : testInput = Request.QueryString("testInput")
'
dim i, str

dim RequestBinCt
dim RequestBin
dim RequestBinStr : RequestBinStr = ""

dim sJoinClassResult : sJoinClassResult = ""
dim sErrorMsg : sErrorMsg = ""

dim resultXML
dim nGet, nClassCode, nUserIDs
dim iUserID, sClassCode

dim iClassID, iCreatorID, sClassName, sClassDesc, dtExprn, dtStartDate, dtEndDate, bEmailScores, iRecordStatus
dim sCreatorEmail, sCreatorFName, sCreatorLName
dim bClassAccessRevoked, dtLastLogin

dim objConn
dim rsObj
%>
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
<!--#include file="xx-Func-IsValidEmail.asp"-->
<%
dim strQuery


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
"    <JoinClass xmlns=""http://tempuri.org/"">" &_
"      <iUserID>3501009</iUserID>" &_
"      <sClassCode>boo9999@s.e</sClassCode>" &_
"    </JoinClass>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
elseif 0 then
	RequestBinStr = "" &_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <JoinClass xmlns=""http://tempuri.org/"">" &_
"      <iUserID>3501009</iUserID>" &_
"      <sClassCode>ChCr18</sClassCode>" &_
"    </JoinClass>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
elseif 0 then
	RequestBinStr = "" &_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <JoinClass xmlns=""http://tempuri.org/"">" &_
"      <iUserID>350100900</iUserID>" &_
"      <sClassCode>badbadcode</sClassCode>" &_
"    </JoinClass>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
end if


set resultXML = Server.CreateObject("Microsoft.XMLDOM")
resultXML.async = "false"
resultXML.loadXML( RequestBinStr )

set nGet = resultXML.getElementsByTagName("JoinClass")
set nClassCode = resultXML.getElementsByTagName("sClassCode")
set nUserIDs = resultXML.getElementsByTagName("iUserID")

if nClassCode.length = 1 then
	sClassCode = nClassCode(0).text
end if
if nUserIDs.length = 1 then
	iUserID = nUserIDs(0).text
else
	iUserID = ""
end if

checkInput

ConnectToBFWUsersDB
CheckForErrors(objConn)

DB_AssignUserClass sClassCode
''response.end

goToFinish




sub checkInput ()
	if nGet.length = 0 then
		sErrorMsg = "Invalid input: no JoinClass element"
		goToFinish
	end if

	if nUserIDs.length = 0 or iUserID = "" then
		sErrorMsg = "Invalid input: no iUserID"
		goToFinish
	end if

	if nClassCode.length = 0 or sClassCode = "" then
		sErrorMsg = "Invalid input: no sClassCode"
		goToFinish
	end if

end sub




sub goToFinish ()
''	objConn.close
	set objConn = nothing

	dim ResponseStr
	ResponseStr = "<JoinClassResponse><JoinClassResult>"& sJoinClassResult &"</JoinClassResult><sErrorMsg>"& sErrorMsg &"</sErrorMsg></JoinClassResponse>"
	ResponseStr = "<?xml version=""1.0""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body>"& ResponseStr &"</soap:Body></soap:Envelope>"
	response.write ResponseStr
	response.end
end sub




sub DB_AssignUserClass (sCode)

'UserNotFound
'CodeNotFound
'CreatorNotFound
'AlreadyAssigned

	' Get User exists
	set rsObj = Server.CreateObject("ADODB.Recordset")
	strQuery = "Select UserID from tblUserProfile " & _
			"where UserID='" & iUserID & "'"
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		sErrorMsg = "UserNotFound"
	Else
	End If
	rsObj.close
	set rsObj = Nothing
   	' If we didn't find it, exit
	if sErrorMsg = "UserNotFound" then
		Call writeLog("3", "User: " & iUserID & " not found")
		goToFinish
	end if

	' Get Code data
	strQuery = "Select * from tblClass " & _
			"where Code='" & replace(sCode,"'","''") & "'"
writeLog "1", "DB_AssignUserClass: "& strQuery
writeLog "1", "<br/>"
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		sErrorMsg = "CodeNotFound"
	Else
		iClassID = rsObj("ClassID")
		iCreatorID = rsObj("CreatorID")
		sClassName = rsObj("Title")
		sClassDesc = rsObj("Description")
		sClassCode = rsObj("Code")
		dtExprn = rsObj("Expiration")
		dtStartDate = rsObj("StartDate")
		dtEndDate = rsObj("EndDate")
		bEmailScores = rsObj("EmailScores")
		iRecordStatus = rsObj("tiRecordStatus")
	End If
	rsObj.close
	set rsObj = Nothing
   	' If we didn't find it, exit
	if sErrorMsg = "CodeNotFound" then
		if not isValidEmail(sCode) then
			Call writeLog("3", "Code: " & sCode & " not found")
			goToFinish
		else
			RADB_DefaultClass sCode
			exit sub
		end if
	end if

	' Check for existing assignment
	strQuery = "Select ClassID from tblClassAssignments " & _
			"where ClassID=" & iClassID & " " &_
			"and UserID=" & iUserID
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
	Else
		sErrorMsg = "AlreadyAssigned"
	End If
	rsObj.close
	set rsObj = Nothing
   	' If we didn't find it, exit
'	if sErrorMsg = "AlreadyAssigned" then
'		Call writeLog("3", "Class: " & sClassName & " already assigned to User: "& iUserID)
'		goToFinish
'	end if
	if iCreatorID <> null OR iCreatorID > 0 then
		' Get Creator data
		strQuery = "Select UserEmail, FirstName, LastName from tblUserProfile " & _
				"where UserID=" & iCreatorID & ""
		Set rsObj = Server.CreateObject("ADODB.Recordset")
		rsObj.Open strQuery, objConn
		If rsObj.EOF Then
			sErrorMsg = "CreatorNotFound"
		Else
			sCreatorEmail = rsObj("UserEmail")
			sCreatorFName = rsObj("FirstName")
			sCreatorLName = rsObj("LastName")
		End If
		rsObj.close
		set rsObj = Nothing
		' If we didn't find it, exit
		if sErrorMsg = "CreatorNotFound" then
			Call writeLog("3", "Code: " & sCode & " not found")
			goToFinish
		end if
	end if



	' All Checks are okay.  Now to update database.

	bClassAccessRevoked = 0
	dtLastLogin = Now()

	if sErrorMsg <> "AlreadyAssigned" then
		strQuery = "INSERT INTO tblClassAssignments VALUES (" & iUserID & "," & iClassID & ", "& bClassAccessRevoked &", '" & dtLastLogin & "')"
		Set rsObj = Server.CreateObject("ADODB.Recordset")
		rsObj.Open strQuery, objconn
	end if

'	if sErrorMsg = "" then
		sJoinClassResult = sJoinClassResult &"<udtClassJoined>" &_
			"<iClassID>"& iClassID &"</iClassID>" &_
			"<iCreatorID>"& iCreatorID &"</iCreatorID>" &_
			"<sCreatorEmail>"& sCreatorEmail &"</sCreatorEmail>" &_
			"<sCreatorFName>"& sCreatorFName &"</sCreatorFName>" &_
			"<sCreatorLName>"& sCreatorLName &"</sCreatorLName>" &_
			"<sClassName>"& sClassName &"</sClassName>" &_
			"<sClassDesc>"& sClassDesc &"</sClassDesc>" &_
			"<sClassCode>"& sClassCode &"</sClassCode>" &_
			"<dtExprn>"& dtExprn &"</dtExprn>" &_
			"<iUserID>"& iUserID &"</iUserID>" &_
			"<bClassAccessRevoked>"& bClassAccessRevoked &"</bClassAccessRevoked>" &_
			"<dtLastLogin>"& dtLastLogin &"</dtLastLogin>" &_
			"<dtStartDate>"& dtStartDate &"</dtStartDate>" &_
			"<dtEndDate>"& dtEndDate &"</dtEndDate>" &_
			"<bEmailScores>"& bEmailScores &"</bEmailScores>" &_
			"<iRecordStatus>"& iRecordStatus &"</iRecordStatus>" &_
			"</udtClassJoined>"
'	end if

end sub




dim infLoopCatchCt : infLoopCatchCt = 0
sub RADB_DefaultClass (sInstEmail)
	Call writeLog("3", "RADB_DefaultClass( "& sInstEmail &" )")
	sErrorMsg = ""
	infLoopCatchCt = infLoopCatchCt+1
	if infLoopCatchCt > 10 then
		sErrorMsg = "InfLoop"
		Call writeLog("3", "InfLoop, Code: " & sCode & " not found")
		goToFinish
	end if

	dim foundInst : foundInst = false
	set rsObj = Server.CreateObject("ADODB.Recordset")
	strQuery = "Select UserID from tblUserProfile " & _
			"where UserEmail='" & replace(sInstEmail,"'","''") & "'"
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		foundInst = false
	Else
		foundInst = true
		iCreatorID = rsObj("UserID")
	End If
	rsObj.close
	set rsObj = Nothing

	' If we did find it, use it
	if foundInst then
		strQuery = "INSERT INTO tblClass VALUES ('Default group', 'Default group for "& replace(sInstEmail,"'","''") &"', '"& replace(sInstEmail,"'","''") &"', '1/1/2100', " & iCreatorID & ", '1/1/2000', '1/1/2000', '1/1/2100', 1, 1 )"

		Set rsObj = Server.CreateObject("ADODB.Recordset")
		rsObj.Open strQuery, objconn

		set rsObj = Nothing

		sErrorMsg = "Default class created for "& replace(sInstEmail,"'","--apos--")
		DB_AssignUserClass sInstEmail

		exit sub
	end if

	' If we didn't find it, look up defaultclassinst
	set rsObj = Server.CreateObject("ADODB.Recordset")
	strQuery = "Select UserID from tblUserProfile " & _
			"where UserEmail='UnknownInstructor@unknown.edu'"
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		foundInst = false
	Else
		foundInst = true
		iCreatorID = rsObj("UserID")
	End If
	rsObj.close
	set rsObj = Nothing

	' If we did find it, use it
	if foundInst then
		dim strInsert
		strInsert = "INSERT INTO tblClass VALUES ('Default group', 'Default group for "& replace(sInstEmail,"'","''") &"', '"& replace(sInstEmail,"'","''") &"', '1/1/2100', " & iCreatorID & ", '1/1/2000', '1/1/2000', '1/1/2100', 1, 1 )"
		Set rsObj = Server.CreateObject("ADODB.Recordset")
		rsObj.Open strInsert, objconn

		set rsObj = Nothing

		sErrorMsg = "Default class created for UNKNOWN "& replace(sInstEmail,"'","--apos--")
		DB_AssignUserClass sInstEmail

		exit sub
	end if

	' Not found, error exit
	sErrorMsg = "CodeNotFound"
	Call writeLog("3", "Code: " & sCode & " not found")
	goToFinish

end sub




sub writeLog (y,z)
'	response.write z
'	response.write "<br/>"
end sub

%>




