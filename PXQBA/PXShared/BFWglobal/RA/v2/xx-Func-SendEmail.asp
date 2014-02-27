<% ' [ RA_version="v.b8" ]
Call writeLog("2", " ****** Entering xx-Func-SendEmail.asp:main() ****** ")


Sub SendEmail(inTo, inFrom, inSubject, inBody)
	Call writeLog("2", " ****** Entering xx-Func-SendEmail.asp:SendEmail( " & inTo & ", " & inFrom & ", " & inSubject & ", " & inBody & " ) ****** ")

	Dim objNewMail
	Set objNewMail = Server.CreateObject("CDO.message")
 
	objNewMail.From = Chr(34) & inFrom & Chr(34)
	objNewMail.To = inTo
	objNewMail.Subject = inSubject
	objNewMail.TextBody = inBody
	'objNewMail.Importance = 1
	objNewMail.Send 
End Sub

%>