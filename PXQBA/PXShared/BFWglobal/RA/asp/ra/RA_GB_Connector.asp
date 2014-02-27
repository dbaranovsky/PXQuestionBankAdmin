<!--#include file="../../../../bfw_include/ra_v3/asp/xxx-core_includes.asp"-->
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
<%
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''	DATABASE CONNECTION
	dim objConn
	ConnectToBFWUsersDB
	CheckForErrors(objConn)
%>
<!--#include file="../../../../bfw_include/ra_v3/asp/xxx-dbFunc-AddNewUser.asp"-->
<!--#include file="../../../../bfw_include/ra_v3/asp/xxx-dbFunc-VerifyCode.asp"-->
<!--#include file="../../../../bfw_include/ra_v3/asp/xxx-dbFunc-AssignUserAccess.asp"-->
<!--#include file="../../../../bfw_include/ra_v3/asp/xxx-dbFunc-GetUserProfileFromEmail.asp"-->
<!--#include file="../../../../bfw_include/ra_v3/asp/xxx-dbFunc-UserSiteLogin.asp"-->
<!--#include file="../../../../bfw_include/ra_v3/asp/xx-Func-Cookie.asp"-->
<%

inFunction = Request("Function")

If inFunction = "login" Then
	CheckForLogin()
ElseIf inFunction = "geturl" Then
	GetSiteInfo()
ElseIf inFunction = "addnewinst" Then
	AddNewInst()
ElseIf inFunction = "grantaccess" then
	AssignUserAccessToSite()
ElseIf inFunction = "grantadopteraccess" then
	AssignUserAdopterAccessToSite()
End if

function BuildParameterXMLFromRecordSet(oRecordSet, objectName)

	on error resume next
	dim fldF
	dim sXML
	sXML = ""

	Do While not oRecordSet.EOF
		sXML = sXML & "<" & objectName & ">"
		For Each fldF in oRecordSet.Fields
			sXML = sXML & "<" & fldF.Name & ">" & Server.HTMLEncode(fldF.Value) & "</" & fldF.Name & ">"
		next
		sXML = sXML & "</" & objectName & ">"
		oRecordSet.MoveNext
	Loop


	if(err.number <> 0) then
		BuildParameterXMLFromRecordSet = ""
	else
		BuildParameterXMLFromRecordSet = sXML
	end if

end function
function BuildParameterReturnNode(nodeName, nodeValue)

	BuildParameterReturnNode = "<" & nodeName & ">" & Server.HTMLEncode(nodeValue ) & "</" & nodeName & ">"

end function
function SetMethodStatusNode(status, message)
	SetMethodStatusNode = "<returnStatus><methodStatus>" & status & "</methodStatus><errorDescription>" & message & "</errorDescription></returnStatus>"
end function

Function GetSiteInfo()
	on error resume next
	Dim rsObj, strQuery, returnURL
	Dim sMethodStatus, sReturnXML
	dim qSiteID, qSiteISBN
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	qSiteISBN = Request("siteISBN")


		strQuery = "select s.*, i.SiteTemplated from tblsitekey s (nolock) inner join tblSiteISBN i on i.SiteId = s.SiteID " & _
				"where i.SiteISBN='" & qSiteISBN & "'"

	rsObj.open strQuery, objConn

	if(err.number <> 0) then
		sMethodStatus = SetMethodStatusNode("failure", err.Description)
	else
		If rsObj.EOF Then
			sMethodStatus = SetMethodStatusNode("failure", "Site not found")
		Else
				sMethodStatus = SetMethodStatusNode("success", "")
				sParameter = "<parameters><siteInfo>"
				sParameter = sParameter & BuildParameterReturnNode("siteID", rsObj("siteId"))
				sParameter = sParameter & BuildParameterReturnNode("baseURL", rsObj("baseURL"))
				sParameter = sParameter & BuildParameterReturnNode("siteTemplated ", rsObj("SiteTemplated"))
				sParameter = sParameter & "</siteInfo></parameters>"
		end if
	end if
	rsObj.Close

	response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"

End Function

Function AddNewInst()
on error resume next
	Dim rafStatus, rafError, xReturnPath, errorMsg, inSiteID
	rafStatus = "e"
	rafError = "e0"

	Dim rafName
	rafName = Request("raf")

	' Get the data off the form for registration
	Dim inUserEmail, inPasswordHint, inPassword, inVerifyPassword
	Dim inFirstName, inLastName, inMailPreferences, inOptInEmail, inOnyxId
	Dim inInstructorEmail, inUserID, sMethodStatus, sParameter
	inUserEmail = Lcase(trim(replace(Request("Email"),"mailto:","")))
	inPassword = Request("Password")
	inVerifyPassword = Request("verify")
	inPasswordHint = replace(Request("PasswordHint"),"'","''" )
	inFirstName = trim(Request("FirstName"))
	inLastName = trim(Request("LastName"))
	inSiteID = Request("siteid")
	inOnyxId = Request("onyxId")
	if inOptInEmail <> "No" then
		inOptInEmail = "Yes"
	end if
	'-----------------------------------------------

	inMailPreferences = "Text"
	inOptInEmail = "No"
	'inSiteID = 872

	'-----------------------------------------------
	inInstructorEmail = Lcase(trim(replace(Request("InstructorEmail"),"mailto:","")))

	'Error check useremail, instructor email -- Hugh added 3/29/2006
	inUserEmail = replace(inUserEmail,"<","")
	inUserEmail = replace(inUserEmail,"[","")
	inUserEmail = replace(inUserEmail,">","")
	inUserEmail = replace(inUserEmail,"]","")

	inInstructorEmail = replace(inInstructorEmail,"<","")
	inInstructorEmail = replace(inInstructorEmail,"[","")
	inInstructorEmail = replace(inInstructorEmail,">","")
	inInstructorEmail = replace(inInstructorEmail,"]","")
	'for user email:
	if left(inUserEmail,1) = "'" or left(inUserEmail,1) = """" then
		inUserEmail = right(inUserEmail,len(inUserEmail) - 1)
	end if
	if right(inUserEmail,1) = "'" or right(inUserEmail,1) = """" then
		inUserEmail = left(inUserEmail,len(inUserEmail) - 1)
	end if
	if right(inUserEmail,1) = "." then
		inUserEmail = left(inUserEmail,len(inUserEmail) - 1)
	end if
	'for instructor email:
	if left(inInstructorEmail,1) = "'" or left(inInstructorEmail,1) = """" then
		inInstructorEmail = right(inInstructorEmail,len(inInstructorEmail) - 1)
	end if
	if right(inInstructorEmail,1) = "'" or right(inInstructorEmail,1) = """" then
		inInstructorEmail = left(inInstructorEmail,len(inInstructorEmail) - 1)
	end if
	if right(inInstructorEmail,1) = "." then
		inInstructorEmail = left(inInstructorEmail,len(inInstructorEmail) - 1)
	end if

	'-------------------------------------------------------------------

	''''''''''''''''''''''''''''''''''''
	' try to Register the input user data
	''''''''''''''''''''''''''''''''''''

	If inPassword <> inVerifyPassword Then
		rafError = "e15"
		errorMsg =  "##error:Passwords do not match##"
	ElseIf Len(inPassword) < 4 Then
		rafError = "e16"
		errorMsg =  "##error:Password must be at least 4 characters in length##"
	ElseIf inOnyxId = "" then
		errorMsg =  "##error:OnyxId is missing##"
	Else
		rafError = AddNewUser( _
									Request.ServerVariables("remote_addr"), _
									inUserEmail, _
									inFirstName, _
									inLastName, _
									inPassword, _
									inPasswordHint, _
									inMailPreferences, _
									inOptInEmail, _
									inSiteID, _
									inInstructorEmail _
									)

		If (not isNumeric(rafError)) OR rafError = "" Then
		' check for addnewuser errors

			If rafError = "Incomplete" Then
				rafError = "e11"
				errorMsg =  "##error:Incomplete form##"
			ElseIf rafError = "BadEmailFormat" Then
				rafError = "e12"
				errorMsg =  "##error:Bad email format##"
			ElseIf rafError = "BadInstEmailFormat" Then
				rafError = "e17"
				errorMsg =  "##error:Bad instructor email format##"
			ElseIf rafError = "BadNameFormat" Then
				rafError = "e18"
				errorMsg =  "##error:Bad name format##"
			ElseIf rafError = "EmailAddressAlreadyAssigned" Then
				rafError = "e13"
				errorMsg =  "##error:Email address is already in use##"
			ElseIf rafError = "InvalidPasswordandHint" Then
				rafError = "e14"
				errorMsg =  "##error:Invalid Password Hint##"
			Else
				rafError = "e01"
				errorMsg =  "##error:UnknownError##"
			End If

		Else

			dim sqlAddUserOnyxId, rsObj

			Set rsObj = Server.CreateObject("ADODB.Recordset")
			sqlAddUserOnyxId = "insert into tblUserProfileOnyx values (" & inUserID & "," & inOnyxId & ")"
			rsObj.Open sqlAddUserOnyxId, objconn

			if(err.number <> 0) then
				errorMsg = "##error:" & err.description & "##"
			end if


			rsObj.close
			set rsObj = Nothing

			' successful addnewuser
			'errorMsg =  "##success##"
			inUserID = rafError

			rafStatus = "e"
			rafError = AssignUserAccess(inUserID, inSiteID)

					' check for AssignUserAccess errors
				If (not isNumeric(rafError)) OR rafError = "" Then
					If (	(rafError = "CodeInvalidFormat") OR (rafError = "CodeFailsSumCheck") OR (rafError = "CodeNotFound") OR (rafError = "BatchNotFound") OR (rafError = "LevelIDNotValid") OR (rafError = "LevelIDNotFound") OR (rafError = "CodeNotValidForAnySites") 	) Then
						errorMsg = "##CodeError1##"
					ElseIf ((rafError = "CodeNoUsesLeft") OR (rafError = "CodeUseByDateExpired") OR (rafError = "CodeExpiredAbsolute") OR (rafError = "CodeExpiredRelative") OR (rafError = "BatchSuspended")		) Then
						errorMsg = "##CodeError2##"
					ElseIf ( rafError = "CodeNotValidForSite" 		) Then
						errorMsg = "##CodeNotValidForSite##"
					ElseIf ( rafError = "CodeAlreadyAssignedToUser"		) Then
						errorMsg = "##CodeAlreadyAssignedToUser##"
					ElseIf ( rafError = "PackageAlreadyAssignedToUser"		) Then
						errorMsg = "##PackageAlreadyAssignedToUser##"
					ElseIf ( rafError = "NoSiteLoginsForUser"		) Then
						errorMsg = "##NoSiteLoginsForUser##"
					End If
				End If
		End If
	End If

	if(errorMsg = "") then
		sMethodStatus = SetMethodStatusNode("success", "")
		sParameter = "<parameters><userInfo>"
		sParameter = sParameter & BuildParameterReturnNode("userId", inUserID)
		sParameter = sParameter & "</userInfo></parameters>"

	else
		sMethodStatus = SetMethodStatusNode("failure", errorMsg)
	end if

	response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
End Function


Function CheckForLogin()
	on Error Resume Next

	Dim rafStatus, rafError

	rafStatus = "e"

	rafError = "e0"	'default error

	' Get the data off the form for login
	Dim qUserEMail, qUserPassword, qSiteID
	dim sMethodStatus, sParameter
	qUserEMail = Lcase(Request("EMail"))
	qUserPassword = Request("Password")
	qSiteID = Request("siteID")
	'Session("xUserEmail") = qUserEmail

	if qUserEmail = "" OR qUserPassword = "" Or qSiteID = "" then
	' incomplete data
		rafError = "e6"
		sMethodStatus = SetMethodStatusNode("failure", "Incomplete data.")
	else

		rafError = SessionLogin( qUserEmail, qUserPassword, qSiteID )
		if(err.number <> 0) then
			sMethodStatus = "<returnStatus><methodStatus>false</methodStatus><errorDescription>" & err.Description & "</errorDescription></returnStatus>"
		else
			if rafError <> "1" then
				if rafError = "UserNotFound" then
					sMethodStatus = SetMethodStatusNode("failure", "User not found")
				elseif rafError = "WrongPassword" then
					sMethodStatus = SetMethodStatusNode("failure", "Wrong password")
				elseif rafError = "UserRevoked" then
					sMethodStatus = SetMethodStatusNode("failure", "User revoked")
				elseif rafError = "AlreadyLoggedIn" then
					sMethodStatus = SetMethodStatusNode("failure", "User already logged in")
				elseif rafError = "UserSiteAccessRevoked" then
					sMethodStatus = SetMethodStatusNode("failure", "Access revoked")
				else
					sMethodStatus = SetMethodStatusNode("failure", "General error")
				end if
			else
				sMethodStatus = SetMethodStatusNode("success", "")
				sParameter = "<parameters><userInfo>"
				sParameter = sParameter & BuildParameterReturnNode("userEmail", Session("UserEmail"))
				sParameter = sParameter & BuildParameterReturnNode("password", Session("Password"))
				sParameter = sParameter & BuildParameterReturnNode("passwordHint", Session("PasswordHint"))
				sParameter = sParameter & BuildParameterReturnNode("firstName", Session("FirstName"))
				sParameter = sParameter & BuildParameterReturnNode("lastName", Session("LastName"))
				sParameter = sParameter & BuildParameterReturnNode("mailPreferences", Session("MailPreferences"))
				sParameter = sParameter & BuildParameterReturnNode("optInEmail", Session("OptInEmail"))
				sParameter = sParameter & BuildParameterReturnNode("userRevoked", Session("UserRevoked"))
				sParameter = sParameter & BuildParameterReturnNode("instructorEmail", Session("InstructorEmail"))
				sParameter = sParameter & BuildParameterReturnNode("accessID", Session("AccessID"))
				sParameter = sParameter & BuildParameterReturnNode("levelOfAccess", Session("LevelOfAccess"))
				sParameter = sParameter & "</userInfo></parameters>"
			end if
		end if
	end If

	response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
End Function

Function SessionLogin( inUserEmail, inUserPassword, inSiteID )

	Dim gotUserID, gotPassword, gotPasswordHint, gotFirstName, gotLastName, gotMailPreferences, gotUserRevoked
	Dim gotSiteAccessRevoked, gotLastLogin, gotInstEmail, gotAccessID, gotAccessExpiration, gotLevelOfAccess

	SessionLogin = "default error"

	'get user profile info, if email is valid
	SessionLogin = getUserProfileFromEmail( _
											inUserEmail, _
											gotUserID, _
											gotPassword, _
											gotPasswordHint, _
											gotFirstName, _
											gotLastName, _
											gotMailPreferences, _
											gotOptInEmail, _
											gotUserRevoked _
											)
	Call writeLog("10", "return from getUserProfile: " & SessionLogin )

	if SessionLogin <> "1" then
	' User profile not found
'GetUserProfileFromEmail return errors, for ref:
'	"default error"
'	"UserNotFound"
'	"BadEmailFormat"

		SessionLogin = "UserNotFound"

	else
	' User profile found
		if UCase(inUserPassword) <> UCase(gotPassword) then
		' passwords don't match
			Call writeLog("10", "passwords not matched: " & inUserPassword & " != " & gotPassword)

			SessionLogin = "WrongPassword"

		else
		' passwords match
			Call writeLog("10", "passwords matched")

			if gotUserRevoked <> 0 then
			' user access revoked
				Call writeLog("10", "UserRevoked")

				SessionLogin = "UserRevoked"

			else
			' user registration is not revoked
			' log user into this site

				SessionLogin = UserSiteLogin( gotUserID, _
												Request.ServerVariables("remote_addr"), _
												inSiteID, _
												gotSiteAccessRevoked, _
												gotLastLogin, _
												gotInstEmail, _
												gotAccessID, _
												gotAccessExpiration, _
												gotLevelOfAccess _
												)
				Call writeLog("10", "return from UserSiteLogin: " & SessionLogin )

				if SessionLogin <> "1" then
				' error logging user into site
'SiteLoginFormID errors, for reference:
'	"default error"
'	"AlreadyLoggedIn"
'	"UserSiteAccessRevoked"

					if SessionLogin = "AlreadyLoggedIn" then
					' check for concurrent logins.
						Call writeLog("10", "IsAlreadyLoggedIn")

						SessionLogin = "AlreadyLoggedIn"

					elseif SessionLogin = "UserSiteAccessRevoked" then
					' user site access is revoked
						Call writeLog("10", "UserSiteAccessRevoked")

						SessionLogin = "UserSiteAccessRevoked"

					else
					' some unknown error
						Call writeLog("10", "UnknownError:" & SessionLogin)
					end if

				else
				' user logged into site
				' set Session vars
					Call writeLog("2", "User Logged In")

					SessionLogin = "1"

					call sesVar_set("UserID", gotUserID )
					Session("UserEmail") 			= inUserEmail
					Session("Password") 			= inUserPassword
					Session("PasswordHint") 		= gotPasswordHint
					Session("FirstName") 			= gotFirstName
					Session("LastName") 			= gotLastName
					Session("MailPreferences") 		= gotMailPreferences
					Session("OptInEmail")	 		= gotOptInEmail
					Session("UserRevoked") 			= gotUserRevoked
					Session("InstructorEmail") 		= gotInstEmail
'					Session("SiteAccessRevoked") 	= gotSiteAccessRevoked
'					Session("LastLogin") 			= gotLastLogin
					Session("AccessID") 			= gotAccessID
'					Session("AccessExpiration") 	= gotAccessExpiration
					Session("LevelOfAccess") 		= gotLevelOfAccess

				end if
			end if
		end if
	end if
End Function

function AssignUserAccessToSite()
	on error resume next
	dim xBatchID, xPackageID, xLevelID
	dim xBatchSuspended, xUseByDate, xRelExpire, xExpirationDate
	dim xLevelOfAccess
	dim rsObj, strQuery
	dim inUserID, inSiteID
	dim sMethodStatus, sParameter


	'gets from batchid: packageid, useby, exp, suspended
	'--	Get PackageID from Batch
	' and xUseByDate and Expiration dates.
	inUserID = request("userId")
	inSiteID = request("siteId")
	sMethodStatus = ""
	sParameter = ""

	if(inUserID = "" or inSiteID = "") then
		sMethodStatus = SetMethodStatusNode("failure", "Missing data to complete request.")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	end if

	strQuery = "Select * from tblBatchKey where description= 'Onyx_Package_" & inSiteID & "'"
	Set rsObj = Server.CreateObject("ADODB.Recordset")

	rsObj.Open strQuery, objConn

	if(err.number <> 0) then
		sMethodStatus = SetMethodStatusNode("failure", err.description)
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	end if


	If rsObj.EOF Then
		sMethodStatus = SetMethodStatusNode("failure", "Batch not found")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
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



	' If batch is suspended
	If xBatchSuspended <> 0 Then
		sMethodStatus = SetMethodStatusNode("failure", "Batch suspended")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	End If

	' If usebydate is past
	If xUseByDate < Date() Then
		sMethodStatus = SetMethodStatusNode("failure", "CodeUseByDateExpired")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	End If

	' If expireddate is past
	If xExpirationDate < Date() Then
		If xAbsExpire <> "" Then
			sMethodStatus = SetMethodStatusNode("failure", "CodeExpiredAbsolute")
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
		Else
			sMethodStatus = SetMethodStatusNode("failure", "CodeExpiredRelative")
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end

		End If
		exit function
	End If

	'checks for userassignment with packageid
	' See if this package is already assigned to this user
	strQuery = "Select * from tblUserAssignments where " & _
		"UserID=" & inUserID & " and " & _
		"PackageID=" & xPackageID
	'response.write strquery
	'response.end
	Call writelog("3", "CCC- strQuery=" & strQuery)
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.open strQuery, objConn
	if (err.number <> 0) then
			sMethodStatus = SetMethodStatusNode("failure", err.description)
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
	end if


	if not rsObj.EOF then
		sMethodStatus = SetMethodStatusNode("failure", "PackageAlreadyAssignedToUser")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	end if
	rsObj.close
	set rsObj = Nothing



	'gets from packageid,siteid: levelid
	'-- Get LevelID from SiteAssignments
	xLevelID = 0
	strQuery = "Select * from tblSiteAssignments" & _
			" where PackageID=" & xPackageID & _
			" and SiteID=" & inSiteID
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn

	if (err.number <> 0) then
			sMethodStatus = SetMethodStatusNode("failure", err.description)
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
	end if

	If rsObj.EOF Then
		sMethodStatus = SetMethodStatusNode("failure", "CodeNotValidForSite")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	Else
		xLevelID = rsObj("LevelID")
		Call writeLog("3", "xLevelID " & xLevelID & ".")
	End If
	rsObj.close
	set rsObj = Nothing

   '-- Verify xLevelID
	If xLevelID =< 0 Then
		sMethodStatus = SetMethodStatusNode("failure", "LevelIDNotValid")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	End If

	'--	Get xLevelOfAccess data
	strQuery = "Select * from tblLevelType where LevelID=" & xLevelID
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn

	if (err.number <> 0) then
			sMethodStatus = SetMethodStatusNode("failure", err.description)
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
	end if

	If rsObj.EOF Then
		sMethodStatus = SetMethodStatusNode("failure", "LevelIDNotFound")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	Else
		xLevelOfAccess = rsObj("LevelOfAccess")
	End If
	rsObj.close
	set rsObj = Nothing


	' Get all of the sites this code applies to
	Dim arrSiteIDs(), SiteCt
	SiteCt = 0
	strQuery = "Select SiteID from tblSiteAssignments " & _
			"where PackageID=" & xPackageID
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn

	if (err.number <> 0) then
			sMethodStatus = SetMethodStatusNode("failure", err.description)
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
	end if

	If rsObj.EOF Then
		sMethodStatus = SetMethodStatusNode("failure", "CodeNotValidForAnySites")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	Else
		While not rsObj.EOF
			SiteCt = SiteCt + 1
			ReDim Preserve arrSiteIDs(SiteCt)
			Call writelog("3", "Site ID " & CStr(rsObj("SiteID")) & " active " & _
					"for this code, xPacakageID: " & xPackageID & ".")
			arrSiteIDs(SiteCt-1) = rsObj("SiteID")
			rsObj.moveNext
		Wend
	End If
	rsObj.close
	set rsObj = Nothing


	Call writelog("3", "Code Available: " & xNormalizedCode)

	Set rsObj = Server.CreateObject("ADODB.Recordset")

	dim sqlUserAssignments
	sqlUserAssignments = "insert into tbluserassignments values (" & inUserID & ",null,'" & cdate(xExpirationDate) & "'," & xPackageID & ")"
	rsObj.Open sqlUserAssignments, objconn

	if (err.number <> 0) then
			sMethodStatus = SetMethodStatusNode("failure", err.description)
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
	else
		sMethodStatus = SetMethodStatusNode("success", "")
		sParameter = "<parameters><grantAccess>"
		sParameter = sParameter & BuildParameterReturnNode("accessGranted", "true")
		sParameter = sParameter & "</grantAccess></parameters>"
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	end if
end function

function AssignUserAdopterAccessToSite()
	on error resume next
	dim xBatchID, xPackageID, xLevelID
	dim xBatchSuspended, xUseByDate, xRelExpire, xExpirationDate
	dim xLevelOfAccess
	dim rsObj, strQuery
	dim inUserID, inSiteID
	dim sMethodStatus, sParameter


	'gets from batchid: packageid, useby, exp, suspended
	'--	Get PackageID from Batch
	' and xUseByDate and Expiration dates.
	inUserID = request("userId")
	inSiteID = request("siteId")
	sMethodStatus = ""
	sParameter = ""

	if(inUserID = "" or inSiteID = "") then
		sMethodStatus = SetMethodStatusNode("failure", "Missing data to complete request.")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	end if

	strQuery = "Select * from tblBatchKey where description= 'Onyx_Adopter_" & inSiteID & "'"
	Set rsObj = Server.CreateObject("ADODB.Recordset")

	rsObj.Open strQuery, objConn

	if(err.number <> 0) then
		sMethodStatus = SetMethodStatusNode("failure", err.description)
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	end if


	If rsObj.EOF Then
		sMethodStatus = SetMethodStatusNode("failure", "Batch not found")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
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



	' If batch is suspended
	If xBatchSuspended <> 0 Then
		sMethodStatus = SetMethodStatusNode("failure", "Batch suspended")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	End If

	' If usebydate is past
	If xUseByDate < Date() Then
		sMethodStatus = SetMethodStatusNode("failure", "CodeUseByDateExpired")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	End If

	' If expireddate is past
	If xExpirationDate < Date() Then
		If xAbsExpire <> "" Then
			sMethodStatus = SetMethodStatusNode("failure", "CodeExpiredAbsolute")
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
		Else
			sMethodStatus = SetMethodStatusNode("failure", "CodeExpiredRelative")
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end

		End If
		exit function
	End If

	'checks for userassignment with packageid
	' See if this package is already assigned to this user
	strQuery = "Select * from tblUserAssignments where " & _
		"UserID=" & inUserID & " and " & _
		"PackageID=" & xPackageID
	'response.write strquery
	'response.end
	Call writelog("3", "CCC- strQuery=" & strQuery)
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.open strQuery, objConn
	if (err.number <> 0) then
			sMethodStatus = SetMethodStatusNode("failure", err.description)
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
	end if


	if not rsObj.EOF then
		sMethodStatus = SetMethodStatusNode("failure", "PackageAlreadyAssignedToUser")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	end if
	rsObj.close
	set rsObj = Nothing



	'gets from packageid,siteid: levelid
	'-- Get LevelID from SiteAssignments
	xLevelID = 0
	strQuery = "Select * from tblSiteAssignments" & _
			" where PackageID=" & xPackageID & _
			" and SiteID=" & inSiteID
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn

	if (err.number <> 0) then
			sMethodStatus = SetMethodStatusNode("failure", err.description)
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
	end if

	If rsObj.EOF Then
		sMethodStatus = SetMethodStatusNode("failure", "CodeNotValidForSite")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	Else
		xLevelID = rsObj("LevelID")
		Call writeLog("3", "xLevelID " & xLevelID & ".")
	End If
	rsObj.close
	set rsObj = Nothing

   '-- Verify xLevelID
	If xLevelID =< 0 Then
		sMethodStatus = SetMethodStatusNode("failure", "LevelIDNotValid")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	End If

	'--	Get xLevelOfAccess data
	strQuery = "Select * from tblLevelType where LevelID=" & xLevelID
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn

	if (err.number <> 0) then
			sMethodStatus = SetMethodStatusNode("failure", err.description)
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
	end if

	If rsObj.EOF Then
		sMethodStatus = SetMethodStatusNode("failure", "LevelIDNotFound")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	Else
		xLevelOfAccess = rsObj("LevelOfAccess")
	End If
	rsObj.close
	set rsObj = Nothing


	' Get all of the sites this code applies to
	Dim arrSiteIDs(), SiteCt
	SiteCt = 0
	strQuery = "Select SiteID from tblSiteAssignments " & _
			"where PackageID=" & xPackageID
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn

	if (err.number <> 0) then
			sMethodStatus = SetMethodStatusNode("failure", err.description)
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
	end if

	If rsObj.EOF Then
		sMethodStatus = SetMethodStatusNode("failure", "CodeNotValidForAnySites")
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	Else
		While not rsObj.EOF
			SiteCt = SiteCt + 1
			ReDim Preserve arrSiteIDs(SiteCt)
			Call writelog("3", "Site ID " & CStr(rsObj("SiteID")) & " active " & _
					"for this code, xPacakageID: " & xPackageID & ".")
			arrSiteIDs(SiteCt-1) = rsObj("SiteID")
			rsObj.moveNext
		Wend
	End If
	rsObj.close
	set rsObj = Nothing


	Call writelog("3", "Code Available: " & xNormalizedCode)

	Set rsObj = Server.CreateObject("ADODB.Recordset")

	dim sqlUserAssignments
	sqlUserAssignments = "insert into tbluserassignments values (" & inUserID & ",null,'" & cdate(xExpirationDate) & "'," & xPackageID & ")"
	rsObj.Open sqlUserAssignments, objconn

	if (err.number <> 0) then
			sMethodStatus = SetMethodStatusNode("failure", err.description)
			response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
			response.end
	else
		sMethodStatus = SetMethodStatusNode("success", "")
		sParameter = "<parameters><grantAccess>"
		sParameter = sParameter & BuildParameterReturnNode("accessGranted", "true")
		sParameter = sParameter & "</grantAccess></parameters>"
		response.write "<RA_Response>" & sMethodStatus & sParameter & "</RA_Response>"
		response.end
	end if
end function

function AssignUserAccess( inUserID, inSiteID )
	dim xBatchID, xPackageID, xLevelID
	dim xBatchSuspended, xUseByDate, xRelExpire, xExpirationDate
	dim xLevelOfAccess
	dim rsObj, strQuery

	'gets from batchid: packageid, useby, exp, suspended
	'--	Get PackageID from Batch
	' and xUseByDate and Expiration dates.
	strQuery = "Select * from tblBatchKey where description= 'Onyx_Package_" & inSiteID & "'"
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		AssignUserAccess = "BatchNotFound"
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
	If AssignUserAccess = "BatchNotFound" Then
		Call writelog("3", "xBatchID " & xBatchID & " not found in DB.")
		exit function
	End If

	' If batch is suspended
	If xBatchSuspended <> 0 Then
		Call writelog("3", "BatchSuspended: " & xBatchSuspended)
		AssignUserAccess = "BatchSuspended"
		exit function
	End If

	' If usebydate is past
	If xUseByDate < Date() Then
		Call writelog("3", "UseByDate is up: " & xUseByDate)
		AssignUserAccess = "CodeUseByDateExpired"
		exit function
	End If

	' If expireddate is past
	If xExpirationDate < Date() Then
		If xAbsExpire <> "" Then
			Call writelog("3", "AbsoluteExpiration: " & xExpirationDate)
			AssignUserAccess = "CodeExpiredAbsolute"
		Else
			Call writelog("3", "RelativeExpiration: " & xExpirationDate)
			AssignUserAccess = "CodeExpiredRelative"
		End If
		exit function
	End If

	'checks for userassignment with packageid
	' See if this package is already assigned to this user
	strQuery = "Select * from tblUserAssignments where " & _
		"UserID=" & inUserID & " and " & _
		"PackageID=" & xPackageID
	'response.write strquery
	'response.end
	Call writelog("3", "CCC- strQuery=" & strQuery)
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.open strQuery, objConn
	if not rsObj.EOF then
		AssignUserAccess = "PackageAlreadyAssignedToUser"
	end if
	rsObj.close
	set rsObj = Nothing
	If AssignUserAccess = "PackageAlreadyAssignedToUser" Then
		Call writelog("3", "xPackageID:" & xPackageID & _
				" is already assigned to inUserID:" & inUserID )
		exit function
	End If

	'gets from packageid,siteid: levelid
	'-- Get LevelID from SiteAssignments
	xLevelID = 0
	strQuery = "Select * from tblSiteAssignments" & _
			" where PackageID=" & xPackageID & _
			" and SiteID=" & inSiteID
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		AssignUserAccess = "CodeNotValidForSite"
	Else
		xLevelID = rsObj("LevelID")
		Call writeLog("3", "xLevelID " & xLevelID & ".")
	End If
	rsObj.close
	set rsObj = Nothing
   '-- If no site assignment, exit
	If AssignUserAccess = "CodeNotValidForSite" Then
		Call writelog("3", "xPackageID " & xPackageID & " not assigned to" & _
				" inSiteID " & inSiteID & " in DB.")
		Exit Function
	End If

   '-- Verify xLevelID
	If xLevelID =< 0 Then
		Call writelog("3", "xLevelID =< 0 :" & xLevelID )
		AssignUserAccess = "LevelIDNotValid"
		exit function
	End If

	'--	Get xLevelOfAccess data
	strQuery = "Select * from tblLevelType where LevelID=" & xLevelID
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		AssignUserAccess = "LevelIDNotFound"
	Else
		xLevelOfAccess = rsObj("LevelOfAccess")
	End If
	rsObj.close
	set rsObj = Nothing
	'-- If no leveltype found, exit
	If AssignUserAccess = "LevelIDNotFound" Then
		Call writelog("3", "xLevelID " & xLevelID & " not found in DB.")
		Exit Function
	End If

	' Get all of the sites this code applies to
	Dim arrSiteIDs(), SiteCt
	SiteCt = 0
	strQuery = "Select SiteID from tblSiteAssignments " & _
			"where PackageID=" & xPackageID
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		Call writelog("3", "No Site Assignment found for this code's package, xPacakageID: " & xPackageID & ".")
		AssignUserAccess = "CodeNotValidForAnySites"
	Else
		While not rsObj.EOF
			SiteCt = SiteCt + 1
			ReDim Preserve arrSiteIDs(SiteCt)
			Call writelog("3", "Site ID " & CStr(rsObj("SiteID")) & " active " & _
					"for this code, xPacakageID: " & xPackageID & ".")
			arrSiteIDs(SiteCt-1) = rsObj("SiteID")
			rsObj.moveNext
		Wend
	End If
	rsObj.close
	set rsObj = Nothing
	If AssignUserAccess = "CodeNotValidForAnySites" Then
		Call writelog("3", "xPackageID " & xPackageID & " not assigned to any sites in DB.")
		Exit Function
	End If

	Call writelog("3", "Code Available: " & xNormalizedCode)

	Set rsObj = Server.CreateObject("ADODB.Recordset")

	dim sqlUserAssignments
	sqlUserAssignments = "insert into tbluserassignments values (" & inUserID & ",null,'" & cdate(xExpirationDate) & "'," & xPackageID & ")"
	rsObj.Open sqlUserAssignments, objconn

	AssignUserAccess = xLevelOfAccess

end function
%>



