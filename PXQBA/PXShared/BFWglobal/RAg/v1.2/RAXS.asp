<% Response.Buffer = True
Response.contenttype = "text/javascript"
''Response.contenttype = "text/html"

dim responseStr : responseStr = ""
dim RAUserID
dim RAUserEmail
dim RAUserFName
dim RAUserLName
dim RAUserPasswordHint
dim RAUserMailPreference
dim RAUserOptInEmail
dim RAUserRememberMe
dim RAUserClassPrompt

''debugvars_HTML_Cookie


dim cookiesStr : cookiesStr = ""
dim x,y
for each x in Request.Cookies
	cookiesStr = cookiesStr& VBCRLF
	if Request.Cookies(x).HasKeys then
		for each y in Request.Cookies(x)
			cookiesStr = cookiesStr& x & ":" & y & "=" & Request.Cookies(x)(y)
			cookiesStr = cookiesStr& VBCRLF
		next
	else
		cookiesStr = cookiesStr& x & "=" & Request.Cookies(x)
	end if
	cookiesStr = cookiesStr& VBCRLF
next


responseStr = responseStr& VBCRLF
if Request.Cookies("RAUserID") <> "" then
	RAUserID = Request.Cookies("RAUserID")
	RAUserEmail = Request.Cookies("RAUserEmail")
	RAUserFName = Request.Cookies("RAUserFName")
	RAUserLName = Request.Cookies("RAUserLName")
	RAUserPasswordHint = Request.Cookies("RAUserPasswordHint")
	RAUserMailPreference = Request.Cookies("RAUserMailPreference")
	RAUserOptInEmail = Request.Cookies("RAUserOptInEmail")
	RAUserRememberMe = Request.Cookies("RAUserRememberMe")
	RAUserClassPrompt = Request.Cookies("RAUserClassPrompt")
	responseStr = responseStr& "RA.CurrentUser = RA.AddUser( "& RAUserID &", '"& RAUserEmail &"', '"& RAUserFName &"', '"& RAUserLName &"', '"& RAUserPasswordHint &"', '"& RAUserMailPreference &"', '"& RAUserOptInEmail &"', '"& RAUserRememberMe &"', '"& RAUserClassPrompt &"' );"&VBCRLF
else
	responseStr = responseStr& "alert('no user cookie' );"& VBCRLF
end if

'Response.Write responseStr
'Response.Write "prompt('', 'Session.SessionID: "& Session.SessionID &"' );"
'Response.Write "prompt('', 'Session.SessionID: "& Session.SessionID &"\n\n\n\nCOOKIES:\n"& replace(replace(cookiesStr,"'","\'"),VBCRLF,"\n") &"\n\n\nRESPONSE:\n" & replace(replace(responseStr,"'","\'"),VBCRLF,"\n") &"' );"












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














''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub debugvars_HTML_Cookie()
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	dim strName, iLoop
   'COOKIE VARS
Response.Write "    <TABLE WIDTH='100%' BORDER='2' CELLSPACING='0' CELLPADDING='2'>"& vbCRLF
   Response.Write "<TR><TD COLSPAN='2' CLASS='subhead'>Client Cookies -- count: "& Request.Cookies.Count &"</TD></TR>"& vbCRLF

	for iloop = 1 to Request.Cookies.Count
		strName = Request.Cookies.Key(iloop)
Response.Write displayVar( iloop &": "& strName, CStr(Request.Cookies.Item(strName)) )
'Response.Write "<TR><TD>"& iloop &": "& strName &"</TD><TD>"& Request.Cookies.Item(strName) &"</TD></TR>"& vbCRLF
	next
'       Response.Write "<TR><TD>USERID</TD><TD>"& Request.Cookies("UserID") &"</TD></TR>"& vbCRLF
Response.Write "    </TABLE>"& vbCRLF
end sub

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
function displayVar( i_varname, i_var)
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

xxx = xxx &_
"<TR><TD>"& i_varname &"</TD><TD><b>[ "&TypeName(i_var)&" ]</b> "&_
i_var &"</TD></TR>"& vbCRLF

		End If
	End If

	displayVar = xxx

end function



%>

