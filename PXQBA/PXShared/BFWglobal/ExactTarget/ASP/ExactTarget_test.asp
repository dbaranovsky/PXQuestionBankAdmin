<%
Response.buffer = true
Response.contenttype = "text/html"
'
'Response.contenttype = "text/xml"

' ******************************************************************
' REQUIRED INCLUDE FILE
%>
<!--#include virtual="/BFWglobal/ExactTarget/ASP/ExactTarget.inc"-->
<%


' ******************************************************************
' EXAMPLE INPUT DATA
dim email : email = "raghu_aug271@school.edu"
if Request.QueryString("email")<>"" then
	email = Request.QueryString("email")
end if
dim fname : fname = "Chad"
dim lname : lname = "Test"
dim RABaseURL : RABaseURL = "192.168.77.243/everydaywriter4e"
dim dtUTC : dtUTC = ET_GetUTCDate("12/11/2008 11:01:01 AM")


response.write "<b>email: </b>"& email &"<br/>"
response.write "<b>fname: </b>"& fname &"<br/>"
response.write "<b>lname: </b>"& lname &"<br/>"
response.write "<b>RABaseURL: </b>"& RABaseURL &"<br/>"
response.write "<b>RADateCreated (dtUTC) : </b>"& dtUTC &"<br/>"
response.write "<br/>"




ET_sOutputStatus = ET_RetrieveUser( email )
''EmailAddress

response.write "ET_RetrieveUser('"& email &"')<br/>"
response.write "ET_RetrieveUser ET_sOutputStatus :: <b><i>"& ET_sOutputStatus &"</i></b><br/><br/>"

if Request.QueryString("act")="sub" and ET_sOutputStatus="Subscriber not found" then

	ET_sOutputStatus = ET_SubscribeUser( email, fname, lname, dtUTC, RABaseURL )
	''EmailAddress, FName, LName, RADateCreated, RABaseURL

response.write "ET_SubscribeUser('"& email &"', '"& fname &"', '"& lname &"', '"& dtUTC &"', '"& RABaseURL &"')<br/>"
response.write "ET_SubscribeUser ET_sOutputStatus :: <b><i>"& ET_sOutputStatus &"</i></b><br/><br/>"

elseif Request.QueryString("act")="unsub" and ET_sOutputStatus="Active" then

	ET_sOutputStatus = ET_UpdateUser( email, fname, lname, "Unsubscribe" )

response.write "ET_UpdateUser('"& email &"', '"& fname &"', '"& lname &"', 'Unsubscribe')<br/>"
response.write "ET_UpdateUser ET_sOutputStatus :: <b><i>"& ET_sOutputStatus &"</i></b><br/><br/>"

elseif Request.QueryString("act")="sub" and ET_sOutputStatus="Unsubscribed" then

	ET_sOutputStatus = ET_UpdateUser( email, fname, lname, "Subscribe" )

response.write "ET_UpdateUser('"& email &"', '"& fname &"', '"& lname &"', 'Subscribe')<br/>"
response.write "ET_UpdateUser ET_sOutputStatus :: <b><i>"& ET_sOutputStatus &"</i></b><br/><br/>"

end if




%>


