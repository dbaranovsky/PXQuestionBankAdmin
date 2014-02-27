<%

dim inTo : inTo = Request.QueryString("to")
dim inURL : inURL = Request.QueryString("url")
dim inName : inName = Request.QueryString("n")
dim inMsg : inMsg = Request.QueryString("msg")
inMsg = replace(replace(inMsg,"http://",""),Chr(10),VBCRLF)
inURL = replace(inURL,"http://","")

dim theTo, theFrom, theSubject, theMsg
select case inTo
	case "chad"
		theTo = "ccrume@bfwpub.com"
'	case "webops"
'		theTo = "webops@bfwpub.com"
	case else
		theTo = "ccrume@bfwpub.com"
end select

theFrom = "webstaff@bfwpub.com"
theSubject = "BFWglobal/LogError ("& inName &") "& inURL
theMsg = ""&_
	"severity: "& inName &VBCRLF&_
	"URL: "& inURL &VBCRLF&_
	"message: "&VBCRLF&_
	inMsg &VBCRLF&_
	VBCRLF&_
	"<!-- " &VBCRLF&_
	"HTTP_USER_AGENT: "& Request.ServerVariables("HTTP_USER_AGENT") &VBCRLF&_
	"REMOTE_ADDR: "& Request.ServerVariables("REMOTE_ADDR")  &VBCRLF&_
	"SERVER_NAME: "& Request.ServerVariables("SERVER_NAME") &VBCRLF&_
	" -->"

'response.write inURL
'response.end


SendEmail theTo, theFrom, theSubject, theMsg

Sub SendEmail(theTo, theFrom, theSubject, theMsg)

	Dim objNewMail
	Set objNewMail = Server.CreateObject("CDO.message")

	objNewMail.From = Chr(34) & theFrom & Chr(34)
	objNewMail.To = theTo
	objNewMail.Subject = theSubject
	objNewMail.TextBody = theMsg
	''objNewMail.Importance = 1
	objNewMail.Configuration.Fields.Item _

		("http://schemas.microsoft.com/cdo/configuration/sendusing") = 2

	objNewMail.Configuration.Fields.Item _

		("http://schemas.microsoft.com/cdo/configuration/smtpserver") = _

			"vsutl"

	objNewMail.Configuration.Fields.Item _

		("http://schemas.microsoft.com/cdo/configuration/smtpserverport") = 25

	objNewMail.Configuration.Fields.Update

	objNewMail.Send

	set objNewMail = nothing

'response.write theMsg
'response.end
End Sub

response.redirect "LogError_msg.asp?msg="& Server.URLEncode(theMsg)

%>
