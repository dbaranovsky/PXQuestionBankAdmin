<%

debugvars_HTMLPage

''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub debugvars_HTMLPage()
debugvars_HTMLPage_open
debugvars_HTML
debugvars_HTMLPage_close
end sub
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub debugvars_HTMLPage_open()
Response.Write "<HTML>" & vbCRLF
Response.Write "  <HEAD><TITLE>Site Debug Page</TITLE>" & vbCRLF
Response.Write "    <META HTTP-EQUIV='Pragma' CONTENT='no-cache'/>"  & vbCRLF
%><STYLE>
BODY {
	font-family: Verdana;
	font-size: 10pt;
	background: ffffff;
}
TABLE {
	font-family: Verdana;
	font-size: 10pt;
}
TD {
	font-family: Verdana;
	font-size: 10pt;
	padding-left: 10px;
}
TD.subhead {
	font-family: Verdana;
	font-size: 12pt;
	font-weight: bold;
	color: ffffff;
	background: 555555;
	padding: 10px;
}
</STYLE><%
Response.Write "  </HEAD>" & vbCRLF
Response.Write "  <BODY>" & vbCRLF
Response.Write " <script language=javascript'>" & vbCRLF
Response.Write "  // Determine which client browser type" & vbCRLF
Response.Write "  var agent = navigator.userAgent.toLowerCase();" & vbCRLF
Response.Write "  var brw=1;" & vbCRLF
Response.Write "  if(agent.indexOf(" & chr(34) & "msie" & chr(34) & ") != -1 || agent.indexOf(" & chr(34) & "Gecko" & chr(34) & ") != -1){brw=1;}" & vbCRLF
Response.Write "  else{brw=2;}" & vbCRLF
Response.Write "</script>" & vbCRLF


Response.Write "<A HREF='" & Application("BasePath") & "/asp/ra/x-logoff.asp?fromPath=/asp/ra/xxx-debugvars.asp'>Log Off</A>"
end sub





''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub debugvars_HTML()
dim strName, iLoop
Response.Write "    <TABLE WIDTH='100%' BORDER='2' CELLSPACING='0' CELLPADDING='2'>" & vbCRLF
'Response.Write "<TR>" & vbCRLF
'Response.Write "<TD COLSPAN='2' ALIGN='center' STYLE='color:ffffff;background:000000'>" & vbCRLF
'Response.Write "<INPUT TYPE='BUTTON' VALUE='CANCEL' onClick='history.back()'/>" & vbCRLF
'Response.Write "</TD>" & vbCRLF
'Response.Write "</TR> " & vbCRLF
Response.Write "      <TR>" & vbCRLF
Response.Write "        <TD COLSPAN='2' ALIGN='middle' VALIGN='middle' STYLE='color:ffffff;background:000000'><DIV STYLE='font-size: 16pt; font-weight: bold;'>Debug Information</DIV></TD>" & vbCRLF

Response.Write "      </TR>" & vbCRLF
Response.Write "    </TABLE>" & vbCRLF


call debugvars_HTML_Application()
Response.Write "<br>"
call debugvars_HTML_Session()
Response.Write "<br>"
call debugvars_HTML_Query()
Response.Write "<br>"
call debugvars_HTML_Cookie()
Response.Write "<br>"
call debugvars_HTML_Server()
Response.Write "<br>"

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Response.Write "    <TABLE WIDTH='100%' BORDER='2' CELLSPACING='0' CELLPADDING='2'>" & vbCRLF
Response.Write "<TR><TD>UserID Cookie</TD><TD>" & Request.Cookies("UserID") & "</TD></TR>" & vbCRLF
Response.Write "<TR>" & vbCRLF
Response.Write "<TD COLSPAN='2' ALIGN='center' STYLE='color:ffffff;background:000000'>" & vbCRLF
'Response.Write "<INPUT TYPE='BUTTON' VALUE='CANCEL' onClick='history.back()'/>" & vbCRLF
Response.Write "&nbsp;" & vbCRLF
Response.Write "</TD>" & vbCRLF
Response.Write "</TR> " & vbCRLF
Response.Write "    </TABLE>" & vbCRLF


end sub





''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub debugvars_HTML_Application()
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	dim strName
   'APPLICATION VARS
Response.Write "    <TABLE WIDTH='100%' BORDER='2' CELLSPACING='0' CELLPADDING='2'>" & vbCRLF
   Response.Write "<TR><TD COLSPAN='2' CLASS='subhead'>Application Variables</TD></TR>" & vbCRLF

   'Use a For Each ... Next to loop through the entire collection
   For Each strName in Application.Contents

Response.Write displayVar( strName, Application(strName) )

   Next

Response.Write "    </TABLE>" & vbCRLF
end sub





''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub debugvars_HTML_Session()
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	dim strName
   'SESSION VARS
Response.Write "    <TABLE WIDTH='100%' BORDER='2' CELLSPACING='0' CELLPADDING='2'>" & vbCRLF
   Response.Write "<TR><TD COLSPAN='2' CLASS='subhead'>Session Variables -- SID: " & Session.SessionID & "</TD></TR>" & vbCRLF

   'Use a For Each ... Next to loop through the entire collection
   For Each strName in Session.Contents

Response.Write displayVar( strName, Session(strName) )

'     'Is this session variable an array?
'     If IsArray(Session(strName)) then
'       'If it is an array, loop through each element one at a time
'       For iLoop = LBound(Session(strName)) to UBound(Session(strName))
'          Response.Write "<TR><TD>" & strName & "(" & iLoop & "</TD><TD>" & Session(strName)(iLoop) & "</TD></TR>" & vbCRLF
'       Next
'     Else
'       'We aren't dealing with an array, so just display the variable
'       Response.Write "<TR><TD>" & strName & "</TD><TD>" & Session.Contents(strName) & "</TD></TR>" & vbCRLF
'     End If

   Next
Response.Write "    </TABLE>" & vbCRLF
end sub





''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub debugvars_HTML_Query()
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	dim strName, iLoop
   'QUERYSTRING VARS
Response.Write "    <TABLE WIDTH='100%' BORDER='2' CELLSPACING='0' CELLPADDING='2'>" & vbCRLF
   Response.Write "<TR><TD COLSPAN='2' CLASS='subhead'>QueryString Variables -- count: " & Request.QueryString.Count & "</TD></TR>" & vbCRLF

	for iloop = 1 to Request.QueryString.Count
		strName = Request.QueryString.Key(iloop)

Response.Write displayVar( iloop & ": " & strName, CStr(Request.QueryString.Item(strName)) )
'Response.Write "<TR><TD>" & iloop & ": " & strName & "</TD><TD>" & Request.QueryString.Item(strName) & "</TD></TR>" & vbCRLF

	next

Response.Write "    </TABLE>" & vbCRLF
end sub





''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub debugvars_HTML_Cookie()
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	dim strName, iLoop
   'COOKIE VARS
Response.Write "    <TABLE WIDTH='100%' BORDER='2' CELLSPACING='0' CELLPADDING='2'>" & vbCRLF
   Response.Write "<TR><TD COLSPAN='2' CLASS='subhead'>Client Cookies -- count: " & Request.Cookies.Count & "</TD></TR>" & vbCRLF

	for iloop = 1 to Request.Cookies.Count
		strName = Request.Cookies.Key(iloop)
Response.Write displayVar( iloop & ": " & strName, CStr(Request.Cookies.Item(strName)) )
'Response.Write "<TR><TD>" & iloop & ": " & strName & "</TD><TD>" & Request.Cookies.Item(strName) & "</TD></TR>" & vbCRLF
	next
'       Response.Write "<TR><TD>USERID</TD><TD>" & Request.Cookies("UserID") & "</TD></TR>" & vbCRLF
Response.Write "    </TABLE>" & vbCRLF
end sub





''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub debugvars_HTML_Server()
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	dim strName
    'Server Variables
Response.Write "    <TABLE WIDTH='100%' BORDER='2' CELLSPACING='0' CELLPADDING='2'>" & vbCRLF
    Response.Write "<TR><TD COLSPAN='2' CLASS='subhead'>Server Variables</TD></TR>" & vbCRLF


	For Each strName in Request.ServerVariables
	'We aren't dealing with an array, so just display the variable
Response.Write displayVar( strName, CStr(Request.ServerVariables(strName)) )

'Response.Write "<TR><TD>" & strName & "</TD><TD>" & Request.ServerVariables(strName) & "</TD></TR>" & vbCRLF

	Next
Response.Write "    </TABLE>" & vbCRLF
end sub





''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
sub debugvars_HTMLPage_close()
%>


<%
Response.Write "  </BODY>" & vbCRLF
Response.Write "</HTML>" & vbCRLF
end sub





''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
function displayVar( i_varname, i_var)
	dim xxx
	xxx = ""

	If IsArray( i_var ) then
	'If it is an array, loop through each element one at a time
xxx = xxx &_
"<TR><TD>" & i_varname & "</TD><TD><b>[ ARRAY ]</b></TD></TR>" & vbCRLF
		dim iLoop
		For iLoop = LBound( i_var ) to UBound( i_var )

xxx = xxx &_
displayVar( i_varname & "(" & iLoop & ")" , i_var(iLoop) )

		Next
	Else
	'We aren't dealing with an array, so just display the variable
		If isObject( i_var ) Then

xxx = xxx &_
"<TR><TD>" & i_varname & "</TD><TD><b>[ OBJECT, TYPE: " & TypeName(i_var) & " ]</b></TD></TR>" & vbCRLF

		Else

xxx = xxx &_
"<TR><TD>" & i_varname & "</TD><TD><b>[ "&TypeName(i_var)&" ]</b> " &_
i_var & "</TD></TR>" & vbCRLF

		End If
	End If

	displayVar = xxx

end function


%>

