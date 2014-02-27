<% ' [ RA_version="v.b8" ]
Call writeLog("3", "***********************************  xx-Func-IsValidEmail.asp")
%>
<% 

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''
''   isValidEmail.
''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Function isValidEmail( inEmail )
	Call writeLog("3", "****** Entering xx-Func-IsValidEmail.asp:isValidEmail() ****** ")
	Call writeLog("10", "inEmail = " & inEmail)
	Dim AtPos, DotPos, charPos
	isValidEmail = TRUE

	AtPos = Instr(inEmail,"@")
	If AtPos = 0 Then
		isValidEmail = FALSE
		Exit Function
	End If
	DotPos = Instr(AtPos,inEmail,".")
	If DotPos = 0 Then
		isValidEmail = FALSE
		Exit Function
	End If
	charPos = Instr(inEmail,"'")
	If charPos <> 0 Then
		isValidEmail = FALSE
		Exit Function
	End If
	charPos = Instr(inEmail,"""")
	If charPos <> 0 Then
		isValidEmail = FALSE
		Exit Function
	End If
	charPos = Instr(inEmail,"+")
	If charPos <> 0 Then
		isValidEmail = FALSE
		Exit Function
	End If
	charPos = Instr(inEmail,"=")
	If charPos <> 0 Then
		isValidEmail = FALSE
		Exit Function
	End If
	charPos = Instr(inEmail,"^")
	If charPos <> 0 Then
		isValidEmail = FALSE
		Exit Function
	End If
	Call writeLog("10", "isValidEmail = " & isValidEmail)
End Function

%>

