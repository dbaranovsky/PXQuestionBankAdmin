<%
Response.buffer = true
'Response.contenttype = "text/html"
'
Response.contenttype = "text/xml"



'' **********************************************************************
'' **********************************************************************
'' **********************************************************************
'' BEGIN TESTING EXACT TARGET
'' **********************************************************************
dim ET_XML
set ET_XML = Server.CreateObject("Microsoft.XMLDOM")
set resultXML = Server.CreateObject("Microsoft.XMLDOM")


	strEnvelope = ""&_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"""&_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"""&_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">"&_
"  <soap:Body>"&_
"    <RetrieveUser xmlns=""http://tempuri.org/"">"&_
"      <Email__Address>ccrume@bfwpub.com</Email__Address>"&_
"    </RetrieveUser>"&_
"  </soap:Body>"&_
"</soap:Envelope>"&_
""
	ET_Http_Request "RetrieveUser", strEnvelope
	ET_RetrieveUser_PROCESS


response.end
'' **********************************************************************
'' END TESTING EXACT TARGET
'' **********************************************************************
'' **********************************************************************
'' **********************************************************************



'' ********************************************************************


sub ET_Http_Request (wsF,strEnvelope)
	Dim ET_Http
	Set ET_Http = Server.CreateObject("Microsoft.XMLHTTP")
	dim wsURL : wsURL = "http://192.168.34.21/bfwwebv2/exacttarget/emailtarget.asmx"
	ET_Http.Open "post", wsURL, false
	ET_Http.setRequestHeader "Soapaction", "http://tempuri.org/"& wsF
	ET_Http.setRequestHeader "Content-Type", "text/xml; charset=utf-8"
	ET_Http.Send strEnvelope

	if ET_Http.responsetext = "" then
			set ET_Http = nothing
			set ET_XML = nothing
			EndOnError
	else
		ET_XML.loadXML(ET_Http.responsetext)
'debugWrite "x", ET_XML.xml
'debugWrite "x", ET_XML.getElementsByTagName("soap:Fault").length
		if ET_XML.getElementsByTagName("soap:Fault").length > 0 then
			set ET_Http = nothing
			EndOnError
		else
		end if
	end if

	set ET_Http = nothing
end sub




'' ********************************************************************


sub ET_RetrieveUser_PROCESS ()
	ET_methodStatus = ""
	ET_methodErrors = ""
	ET_StatusMessage = ""
	ET_SubscriberMessage = ""
	set resultXML = ET_XML.selectSingleNode("/soap:Envelope/soap:Body/RetrieveUserResponse/RetrieveUserResult/returnStatus")
debugWrite "x", "ET Response :: "& ET_XML.xml &VBCRLF &VBCRLF &VBCRLF &VBCRLF

	set ET_methodStatus = resultXML.selectSingleNode("./methodStatus")
	if ET_methodStatus.text <> "Success" then
		set ET_methodStatus = nothing
		set ET_methodErrors = resultXML.selectNodes("./errors/error")
		dim ET_methodErrors_ct : ET_methodErrors_ct = 0
		dim ET_methodErrors_text : ET_methodErrors_text = ""
		dim iNode
		for each iNode in ET_methodErrors
			ET_methodErrors_ct = ET_methodErrors_ct + 1
			ET_methodErrors_text = ET_methodErrors_text&VBCRLF& ET_methodErrors_ct &" :: "& iNode.text
		next
		if ET_methodErrors_ct > 0 then
			set ET_methodErrors = nothing
debugWrite "x", "ET ERROR Failure ("& ET_methodErrors_ct &" errors) :: "& ET_methodErrors_text &VBCRLF
			response.end
		end if
		set ET_SubscriberMessage = resultXML.selectSingleNode("./SubscriberMessage/message")
debugWrite "x", "ET Failure :: "& ET_SubscriberMessage.text &VBCRLF
		set ET_SubscriberMessage = nothing
		response.end
	end if
	set ET_methodStatus = nothing
	set ET_StatusMessage = resultXML.selectSingleNode("./Status/message")
debugWrite "x", "ET User Status :: "& ET_StatusMessage.text &VBCRLF
	set ET_StatusMessage = nothing
response.end

end sub



' *********************************************************************************
' *********************************************************************************
' *********************************************************************************
' *********************************************************************************


sub debugWrite (vs, msg)
	response.write msg
	response.write "<br/>"
end sub

%>

