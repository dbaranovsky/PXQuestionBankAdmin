<!--#include file="..\RAXS\v1.2\RAXS_server.asp"-->
<%

	dim RAWS_iLevelOfAccess : RAWS_iLevelOfAccess = 0

	dim RAWS_XML, resultXML
	set RAWS_XML = Server.CreateObject("Microsoft.XMLDOM")
	set resultXML = Server.CreateObject("Microsoft.XMLDOM")

function BCSv5_premcheck(iSiteID)
'response.write iSiteID
'response.end
	BCSv5_premcheck = 1

	if iSiteID <= 0 or session("userid") <= 0 then
		exit function
	end if

	dim strEnvelope
	strEnvelope = ""&_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"""&_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"""&_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">"&_
"  <soap:Body>"&_
"	<SiteLogin xmlns=""http://tempuri.org/"">"&_
"		<iUserID>"& session("userid") &"</iUserID>"&_
"		<iSiteID>"&iSiteID&"</iSiteID>"&_
"		<sIPAddr>"&Request.ServerVariables("REMOTE_ADDR")&"</sIPAddr>"&_
"	</SiteLogin>"&_
"  </soap:Body>"&_
"</soap:Envelope>"&_
""
	RAWS_Http_Request "SiteLogin", strEnvelope
	RAWS_SiteLogin_PROCESS

'response.write strEnvelope
'response.write "<br/>"
'response.write "<br/>"
'response.write RAWS_iLevelOfAccess
	BCSv5_premcheck = RAWS_iLevelOfAccess

end function




'' ********************************************************************


sub RAWS_Http_Request (wsF, strEnvelope)
	Dim RAWS_Http
	Set RAWS_Http = Server.CreateObject("Microsoft.XMLHTTP")
''	dim wsURL : wsURL = "http://"& RAWSRootDomain &"/raws_3/"& wsF &"/RA"& wsF &".asmx?wsdl"
	dim wsURL : wsURL = "http://"& RAWSRootDomain &"/ra_webservices/webservice1/service1.asmx?wsdl"
	RAWS_Http.Open "post", wsURL, false
	RAWS_Http.setRequestHeader "Soapaction", "http://tempuri.org/"& wsF
'	RAWS_Http.setRequestHeader "Content-Type", "text/xml; charset=utf-8"
'	RAWS_Http.setRequestHeader "Soapaction", "text/xml"
	RAWS_Http.setRequestHeader "Content-Type", "text/xml"
	RAWS_Http.Send strEnvelope

debugWrite "wsF", wsF
debugWrite "wsURL", wsURL
debugWrite "to send", replace(replace(strEnvelope,"<","&lt;"),">","&gt;")

'debugWrite "x", replace(RAWS_Http.responsetext, "<?xml version=""1.0"" encoding=""utf-8""?>", "")
'response.end

	if RAWS_Http.responsetext = "" then
			set RAWS_Http = nothing
			set RAWS_XML = nothing
			EndOnError
	else
debugWrite "response Text", RAWS_Http.responsetext
		RAWS_XML.loadXML(RAWS_Http.responsetext)
debugWrite "response XML", RAWS_XML.xml
debugWrite "x", RAWS_XML.getElementsByTagName("soap:Fault").length
		if RAWS_XML.getElementsByTagName("soap:Fault").length > 0 then
			set RAWS_Http = nothing
			EndOnError
		else
		end if
	end if

	set RAWS_Http = nothing
end sub




'' ********************************************************************


sub RAWS_SiteLogin_PROCESS ()
	RAWS_iLevelOfAccess = 0
	dim x, y
	set x = RAWS_XML.selectSingleNode("/soap:Envelope/soap:Body/SiteLoginResponse/SiteLoginResult")
	y = x.text

	resultXML.loadXML(y)
	dim xmlErr, resultNodes
	if resultXML.parseError.errorCode <> 0 then
		xmlErr = RAWS_XML.parseError
debugWrite "ERROR :::: ", "Error loading RAWS SiteLoginResult XML :: "& xmlErr.reason &VBCRLF
		response.end
	else
		set resultNodes = resultXML.selectSingleNode("/udtSiteLogin").childNodes
		dim iNode
		for each iNode in resultNodes
			select case iNode.nodeName
				case "iLevelOfAccess"
					RAWS_iLevelOfAccess = iNode.text
				case else
			end select
		next

debugWrite "RAWS_iLevelOfAccess", RAWS_iLevelOfAccess

		set resultNodes = nothing
		set x = nothing
	end if

end sub














' *********************************************************************************
' *********************************************************************************
' *********************************************************************************
' *********************************************************************************


sub debugWrite (vs, msg)
'	response.write vs &"<br/>"& msg
'	response.write "<br/>"
end sub

%>
