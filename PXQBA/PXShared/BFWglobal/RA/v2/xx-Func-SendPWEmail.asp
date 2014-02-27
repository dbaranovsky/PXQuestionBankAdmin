<% ' [ RA_version="v.b8" ]
%>
<!--#include file="xx-Func-SendEmail.asp"-->
<%
Call writeLog("2", " ****** Entering xx-Func-SendPWEmail.asp:main() ****** ")


Sub SendPWEmail(inTo, inPW)
	Call writeLog("2", " ****** Entering xx-Func-SendPWEmail.asp:SendPWEmail( " & inTo & ", " & inPW & " ) ****** ")

	Dim Subject, BodyText

	Subject = "Subject: Your Password"
	BodyText = "Here is your requested info:" & vbCRLF & vbCRLF
	BodyText = BodyText & "Email Address: " & inTo & vbCRLF
	BodyText = BodyText & "Password:      " & inPW & vbCRLF
 
	call SendEmail(inTo, Application("company_email"), Subject, BodyText)
End Sub

%>