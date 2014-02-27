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
	"     <data>yo</data>" &_
	"  </SOAP-ENV:Body>" &_
	"</SOAP-ENV:Envelope>" &_
	""
end if

dim nXML

set nXML = Server.CreateObject("Microsoft.XMLDOM")
nXML.async = "false"
nXML.loadXML( RequestBinStr )

dim txtGet
txtGet = Request.Cookies("c")

dim responseStr : responseStr =""

responseStr = txtGet

responseStr = "<resultdata>"& responseStr &"</resultdata>"

responseStr = "<?xml version=""1.0""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body>"& responseStr &"</soap:Body></soap:Envelope>"

response.write responseStr
'Response.Write "<textarea cols=""100"" rows=""200"">"& responseStr &"</textarea>"

%>