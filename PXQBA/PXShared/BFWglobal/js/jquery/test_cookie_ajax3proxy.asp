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
	c = MidB(RequestBin,i,1)
	if AscB(c) < 32 Or AscB(c) > 255 Then str = str Else str = str & Chr(AscB(c))

	RequestBinStr = RequestBinStr & str
	str = ""
Next

if testing then
	RequestBinStr = ""&_
	"<?xml version=""1.0""?>" &_
	"<SOAP-ENV:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
	"  <SOAP-ENV:Body>" &_
	"     <data>0</data>" &_
	"  </SOAP-ENV:Body>" &_
	"</SOAP-ENV:Envelope>" &_
	""
end if


Dim xml
Set xml = Server.CreateObject("Microsoft.XMLHTTP")
dim wsURL
Dim strEnvelope

strEnvelope = RequestBinStr

'response.write strEnvelope
'response.end

wsURL = "http://192.168.77.242/BFWGlobal/js/jquery/test_cookie_ajax3.asp"
xml.Open "post", wsURL, false
xml.setRequestHeader "Soapaction", "blank"
xml.setRequestHeader "Content-Type", "text/xml"

'response.write "yo1"
'response.end

xml.Send strEnvelope

'response.write "yo2"
'response.end

dim resp
resp = xml.responsetext

'response.write resp
'response.end

resp = replace(resp,"’","&#x2019;")
resp = replace(resp,"&apos;","&#x2019;")

Response.Write resp
'Response.Write "<textarea cols=""100"" rows=""200"">"& resp &"</textarea>"

%>