<%
Response.buffer = true
'Response.contenttype = "text/html"
'
Response.contenttype = "text/xml"

'dim testInput : testInput = Request.QueryString("testInput")
'
dim i, str

dim RequestBinCt
dim RequestBin
dim RequestBinStr : RequestBinStr = ""

dim sGetOnyxSchoolsByZipResult : sGetOnyxSchoolsByZipResult = ""
dim sErrorMsg : sErrorMsg = ""

dim resultXML
dim nGet, nZipPrefix, nSchoolType
dim iZipPrefix, sSchoolType

dim objConn
dim rsObj
%>
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
<!--#include file="xx-Func-IsValidEmail.asp"-->
<%
dim strQuery


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

if 0 then
	RequestBinStr = "" &_
"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""" &_
" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""" &_
" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &_
"  <soap:Body>" &_
"    <GetOnyxSchoolsByZip xmlns=""http://tempuri.org/"">" &_
"      <iZipPrefix>023</iZipPrefix>" &_
"      <sSchoolType>H</sSchoolType>" &_
"    </GetOnyxSchoolsByZip>" &_
"  </soap:Body>" &_
"</soap:Envelope>" &_
""
end if


set resultXML = Server.CreateObject("Microsoft.XMLDOM")
resultXML.async = "false"
resultXML.loadXML( RequestBinStr )

set nGet = resultXML.getElementsByTagName("GetOnyxSchoolsByZip")
set nZipPrefix = resultXML.getElementsByTagName("iZipPrefix")
set nSchoolType = resultXML.getElementsByTagName("sSchoolType")

if nZipPrefix.length = 1 then
	iZipPrefix = nZipPrefix(0).text
end if

if nSchoolType.length = 1 then
	sSchoolType = nSchoolType(0).text
end if
if nSchoolType.length = 0 or sSchoolType = "" then
	sSchoolType = "'H','C'"
else
	sSchoolType = "'"& sSchoolType &"'"
end if

checkInput

ConnectToBFWUsersDB
CheckForErrors(objConn)

DB_GetOnyxSchoolsByZip iZipPrefix
''response.end

goToFinish




sub checkInput ()
	if nGet.length = 0 then
		sErrorMsg = "Invalid input: no GetOnyxSchoolsByZip element"
		goToFinish
	end if

	if nZipPrefix.length = 0 or iZipPrefix = "" then
		sErrorMsg = "Invalid input: no iZipPrefix"
		goToFinish
	end if

	if nZipPrefix.length > 1 then
		sErrorMsg = "Invalid input: multiple iZipPrefix"
		goToFinish
	end if

	if nSchoolType.length > 1 then
		sErrorMsg = "Invalid input: multiple sSchoolTypes"
		goToFinish
	end if

	if sSchoolType <> "'H','C'" and sSchoolType <> "'H'" and sSchoolType <> "'C'" then
		sErrorMsg = "Invalid input: invalid sSchoolType value ("& sSchoolType &")"
		goToFinish
	end if

end sub




sub goToFinish ()
''	objConn.close
	set objConn = nothing

	dim ResponseStr
	ResponseStr = "<GetOnyxSchoolsByZipResponse><GetOnyxSchoolsByZipResult>"& sGetOnyxSchoolsByZipResult &"</GetOnyxSchoolsByZipResult><sErrorMsg>"& sErrorMsg &"</sErrorMsg></GetOnyxSchoolsByZipResponse>"
	ResponseStr = "<?xml version=""1.0""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body>"& ResponseStr &"</soap:Body></soap:Envelope>"
	response.write ResponseStr
	response.end
end sub



sub DB_GetOnyxSchoolsByZip (iZipPrefix)
'errors:
'SchoolNotFound


	' Get User exists
	set rsObj = Server.CreateObject("ADODB.Recordset")
	strQuery = "Select name_school, id_school, cd_country, cd_state, city, zip, cd_school_class from tblOnyxSchool " &_
			" where zip like '" & iZipPrefix & "%' and cd_school_class in ("& sSchoolType &")" &_
			" order by name_school"
'response.write strQuery
'response.end
	Set rsObj = Server.CreateObject("ADODB.Recordset")
	rsObj.Open strQuery, objConn
	If rsObj.EOF Then
		sErrorMsg = "SchoolNotFound"
	Else
		While not rsObj.EOF
			sGetOnyxSchoolsByZipResult = sGetOnyxSchoolsByZipResult &"<udtSchool>" &_
					"<sSchoolName>"& zapXML( rsObj("name_school") ) &"</sSchoolName>" &_
					"<iSchoolID>"& zapXML( rsObj("id_school") ) &"</iSchoolID>" &_
					"<sSchoolCountry>"& zapXML( rsObj("cd_country") ) &"</sSchoolCountry>" &_
					"<sSchoolStateAbbr>"& zapXML( rsObj("cd_state") ) &"</sSchoolStateAbbr>" &_
					"<sSchoolCity>"& zapXML( rsObj("city") ) &"</sSchoolCity>" &_
					"<sSchoolZip>"& zapXML( rsObj("zip") ) &"</sSchoolZip>" &_
					"<sSchoolType>"& zapXML( rsObj("cd_school_class") ) &"</sSchoolType>" &_
				"</udtSchool>" & VBCRLF &_
				""
			rsObj.moveNext
		Wend
	End If
	rsObj.close
	set rsObj = Nothing
   	' If we didn't find it, exit
	if sErrorMsg = "SchoolNotFound" then
		Call writeLog("3", "School not found for zip prefix: " & iZipPrefix & "")
		goToFinish
	end if

end sub


function zapXML (v)
	if not( isNull(v) ) then
		zapXML = replace(replace(replace(v,"&","&amp;"),"<","&lt;"),">","&gt;")
	end if
end function


sub writeLog (y,z)
'	response.write z
'	response.write "<br/>"
end sub

%>




