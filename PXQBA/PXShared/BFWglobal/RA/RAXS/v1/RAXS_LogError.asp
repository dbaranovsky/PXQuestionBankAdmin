<%

dim inTo : inTo = Request.QueryString("to")
dim inURL : inURL = Request.QueryString("url")
dim inSeverity : inSeverity = Request.QueryString("s")
dim inMsg : inMsg = Request.QueryString("msg")
inMsg = replace(inMsg,"http://","")
inURL = replace(inURL,"http://","")

dim theTo, theFrom, theSubject, theMsg
select case inTo
	case "chad"
		theTo = "ccrume@bfwpub.com"
	case "webops"
		theTo = "webops@bfwpub.com"
	case else
		theTo = "ccrume@bfwpub.com"
end select

theFrom = "webstaff@bfwpub.com"
theSubject = "RAXS_LogError ("& inSeverity &") "& inURL
theMsg = ""&_
	"severity: "& inSeverity &VBCRLF&_
	"URL: "& inURL &VBCRLF&_
	"message: "&VBCRLF&_
	inMsg

SendEmail theTo, theFrom, theSubject, theMsg

Sub SendEmail(theTo, theFrom, theSubject, theMsg)

	Dim objNewMail
	Set objNewMail = Server.CreateObject("CDO.message")

	objNewMail.From = Chr(34) & theFrom & Chr(34)
	objNewMail.To = theTo
	objNewMail.Subject = theSubject
	objNewMail.TextBody = theMsg
	''objNewMail.Importance = 1
	objNewMail.Send

	set objNewMail = nothing

'response.write msg
'response.end
End Sub

%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"
        "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
	<meta http-equiv="content-type" content="text/html; charset=iso-8859-1">
	<title>error</title>
<script language="JavaScript">
function init () {
	try {
		window.opener.focus();
		setTimeout('window.close();',100);
	}catch(e){alert(e.name+' --- '+e.message);}
}
</script>
</head>
<body onload="init();">

<p>
An error has occured.
</p>
<p>
<a href="JavaScript:window.close();">close window</a>
</p>

</body>

</html>
