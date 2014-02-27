<%
Response.buffer = true
'Response.contenttype = "text/html"
'
Response.contenttype = "text/xml"

dim testing : testing = CBool(Request.QueryString("testing"))
''response.write testing

dim RequestBinCt
dim RequestBin
dim RequestBinStr : RequestBinStr = ""

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

if testing then
	RequestBinStr = ""&_
	"<?xml version=""1.0""?>" &_
	"<SOAP-ENV:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
	"  <SOAP-ENV:Body>" &_
	"     <nStr>yo</nStr>" &_
	"  </SOAP-ENV:Body>" &_
	"</SOAP-ENV:Envelope>" &_
	""
end if

dim nXML

set nXML = Server.CreateObject("Microsoft.XMLDOM")
nXML.async = "false"
nXML.loadXML( RequestBinStr )

dim nGet
set nGet = nXML.getElementsByTagName("nStr")

'response.write nGet(0).xml
'response.end

dim txtGet
txtGet = request.cookies("aspcookie") &" :: "& request.cookies("callct") &"/"& nGet(0).text

response.cookies("aspcookie") = txtGet
response.cookies("aspcookie").domain = "192.168.77.114"


dim responseStr : responseStr =""

responseStr = txtGet

responseStr = "<responseStrD>"& responseStr &"</responseStrD>"

responseStr = "<?xml version=""1.0""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body>"& responseStr &"</soap:Body></soap:Envelope>"

response.write responseStr

%>