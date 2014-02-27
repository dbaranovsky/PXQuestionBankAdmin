<%
Response.buffer = true
Response.contenttype = "text/html"

dim sTransactionIDs : sTransactionIDs = Request.Form("custom")
dim iUserID
dim sPackageIDs : sPackageIDs = ""
dim iPackageIDs_ct : iPackageIDs_ct = 0
dim arrPackageIDs

if sTransactionIDs = "" then
	response.write "fatal error"
	response.end
end if

dim objConn, rsObj, RSx, strQuery
%>
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
<%

set rsObj = Server.CreateObject("ADODB.Recordset")
ConnectToBFWUsersDB
CheckForErrors(objConn)

strQuery = "SELECT UserID, PackageID FROM tblTransactions WHERE TransactionID IN ("& sTransactionIDs &") AND Confirmed=0 AND Cancelled=0"
rsObj.Open strQuery, objconn
While NOT rsObj.EOF
	iUserID = rsObj("UserID")
	iPackageIDs_ct = iPackageIDs_ct + 1
	if iPackageIDs_ct > 1 then
		sPackageIDs = sPackageIDs &","
	end if
	sPackageIDs = sPackageIDs & rsObj("PackageID")
	rsObj.moveNext
WEnd
rsObj.close

if sPackageIDs = "" then
	response.write "fatal error"
	response.end
end if

arrPackageIDs = split(sPackageIDs,",")

dim i
for i=0 to UBound(arrPackageIDs)
	strQuery = "INSERT INTO tblUserAssignments VALUES (" & iUserID & ",null,'" & CDate("1/1/2050") & "'," & arrPackageIDs(i) & ")"
	rsObj.Open strQuery, objconn
next

Set rsObj = Server.CreateObject("ADODB.Connection")
rsObj.Open  objConn
strQuery = "UPDATE tblTransactions SET Confirmed=1 WHERE TransactionID IN ("& sTransactionIDs &") AND Confirmed=0 AND Cancelled=0"
Set RSx = rsObj.Execute(strQuery)
rsObj.Close

set rsObj = nothing
set objConn = Nothing

Response.Cookies("sTransactionIDs").Expires = Now()

%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"
        "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
	<meta http-equiv="content-type" content="text/html; charset=iso-8859-1">
	<title>Check out successful!</title>

<style type="text/css">
BODY {
	PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px;
	FONT-FAMILY: Verdana, sans-serif;
}
</style>

<script language="JavaScript">

var returl = '<%=Request.QueryString("returl")%>';
var cancelurl = '<%=Request.QueryString("cancelurl")%>';

function init () {
	if (!window.opener) {
		alert('not allowed!');
	} else {
		goreturn();
	}
}
function goreturn (){
	window.location = returl;
}
</script>

</head>
<body onload="init();">
<div id="pp" style="display:none;">
custom F: <%=Request.Form("custom")%><br/>
returl: <%=Request.QueryString("returl")%><br/>
<a href="JavaScript:goreturn()">go</a>
</div>
</body>
</html>
