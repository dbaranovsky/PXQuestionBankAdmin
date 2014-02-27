<% Response.Buffer = True
Response.contenttype = "text/html"

%>
<!--#include file="./RAXS_server.asp"-->
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<%
dim objConn
dim rsObj
dim strQuery
set rsObj = Server.CreateObject("ADODB.Recordset")
ConnectToBFWUsersDB
CheckForErrors(objConn)


dim uemail : uemail = Request.Form("email")
if uemail ="" then
	uemail = Request.QueryString("email")
end if
	uemail = Request.QueryString("email")
dim pw : pw = Request.Form("Password")
if pw = "" then
	pw = Request.QueryString("Password")
end if
	pw = Request.QueryString("Password")
dim returl : returl = Request.QueryString("returl")
'returl = "http://"& replace(returl,"http://","")

response.write "<br/>input email: "& uemail
response.write "<br/>input pw: "& pw
response.write "<br/>"& returl
'response.end

dim iUserID

RADB_GetUserID


''http://192.168.34.21/BFWWebV2/login/loginbcs.aspx?isbn=0312472285&retURL=192%2E168%2E77%2E242%2FRA%2FRAXS%5FDoLogin%2Easp%3Freturl%3Dhttp%253A%252F%252F192%252E168%252E77%252E114%252Feverydaywriter4e%255Fcc%252Fdefault.html

''http://192.168.34.21/BFWWebV2/login/loginbcs.aspx?isbn=0312472285&retURL=192%2E168%2E77%2E242%2FRA%2FRAXS%5FDoLogin%2Easp%3Freturl%3Dhttp%253A%252F%252F192%252E168%252E77%252E114%252FBFWglobal%252FRAg%252Fv1%252FRAg%255Fexample%255Fframes%252Fdefault%252Ehtml%253Fradev%253Dyes



%>
<p><%=Server.URLEncode("192.168.77.242/RA/RAXS_DoLogin.asp?returl="& Server.URLEncode("http://192.168.77.114/BFWglobal/RAg/v1/RAg_example_frames/default.html?radev=yes") )%></p>


<p><a href="<%=RAXSRootURL &"/RAXS_Login.asp?uid="& iUserID &"&returl="& Server.URLEncode( returl )%>"><%=RAXSRootURL &"/RAXS_Login.asp?uid="& iUserID &"&returl="& Server.URLEncode( returl )%></a></p>
<%

Dim gogourl : gogourl = RAXSRootURL &"/RAXS_Login.asp?uid="& iUserID &"&returl="& Server.URLEncode( returl )
response.write "<a href="""& gogourl &""">go</a>"
response.end
'response.redirect gogourl

%>
<h3>incoming data from login</h3>
<p><b>form</b></p>
<%

for x = 1 to Request.Form.count()
	Response.Write(Request.Form.key(x) & " = ")
	Response.Write(Request.Form.item(x) & "<br>")
next
%>
<p><b>qstr</b></p>
<%
for x = 1 to Request.QueryString.count()
	Response.Write(Request.QueryString.key(x) & " = ")
	Response.Write(Request.QueryString.item(x) & "<br>")
next

response.end



'' ********************************************************************


sub RADB_GetUserID ()

	RA_PackagesCt = 0

	strQuery = ""&_
"SELECT  "&_
"	UserID "&_
"FROM         tblUserProfile WITH (nolock) "&_
"WHERE     (UserEmail = '"& uemail &"') AND (Password = '"& pw &"') "&_
""
debugWrite "x", strQuery
'response.end
	rsObj.open strQuery, objConn
	If rsObj.EOF Then
debugWrite "x", "...no matching records found"
'
	Else
		iUserID = rsObj("UserID")
	End If
	rsObj.close


debugWrite "x", "<br/>"
debugWrite "x", iUserID

'
end sub

set rsObj = nothing
objConn.close
set objConn = nothing








' *********************************************************************************
' *********************************************************************************
' *********************************************************************************
' *********************************************************************************


sub debugWrite (vs, msg)
	response.write msg
	response.write "<br/>"
end sub

%>

