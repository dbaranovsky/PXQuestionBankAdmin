<%
Response.buffer = true
Response.contenttype = "text/html"

dim iUserID : iUserID = Request.Form("UserID")
dim iSiteID : iSiteID = Request.Form("SiteID")
dim iSchoolID : iSchoolID = Request.Form("SchoolID")
dim iSchoolZip : iSchoolZip = Request.Form("SchoolZip")
dim sSchoolName : sSchoolName = Request.Form("SchoolName")
sSchoolName = replace( sSchoolName, "'", "''" )
dim sSchoolType : sSchoolType = Request.Form("SchoolType")
dim xPackageID, xPackagePrice
dim xDate : xDate = Now()
dim sTransactionIDs : sTransactionIDs = ""
dim iTransactionIDs_ct : iTransactionIDs_ct = 0

dim objConn, rsObj, strQuery
%>
<!--#include virtual="/RA/server/v1/xxx-connect-ra.asp"-->
<!--#include virtual="/RA/server/v1/xxx-adovbs.asp"-->
<%

set rsObj = Server.CreateObject("ADODB.Recordset")
ConnectToBFWUsersDB
CheckForErrors(objConn)

for i=1 to 30
	if Request.Form("item_name_"&i) <> "" AND Request.Form("PackageID_"&i) <> "" AND Request.Form("amount_"&i) <> "" then
		xPackageID = Request.Form("PackageID_"&i)
		xPackagePrice = Request.Form("amount_"&i)

		strQuery = "INSERT INTO tblTransactions VALUES ("& iUserID &","& iSiteID &","& xPackageID &","& xPackagePrice &",'"& xDate &"',0,0, "& iSchoolID &", '"& iSchoolZip &"', '"& sSchoolName &"', '"& sSchoolType &"' )"
		rsObj.Open strQuery, objconn

		strQuery = "SELECT TransactionID FROM tblTransactions WHERE UserID="& iUserID &" AND SiteID="& iSiteID &" AND PackageID="& xPackageID&" AND ItemPrice='"& xPackagePrice &"' AND DatePurchased='"& xDate &"' AND Confirmed=0"
		rsObj.Open strQuery, objconn
		if rsObj.EOF then
			response.write "fatal error<br/>"
			response.write strQuery &"<br/>"
			response.end
		else
			iTransactionIDs_ct = iTransactionIDs_ct + 1
			if iTransactionIDs_ct > 1 then
				sTransactionIDs = sTransactionIDs &","
			end if
			sTransactionIDs = sTransactionIDs & rsObj("TransactionID")
'response.write iTransactionIDs_ct
'response.write " - "
'response.write rsObj("TransactionID")
'response.write " - "
'response.write sTransactionIDs
'response.write "<br/>"
			rsObj.moveNext
		end if

		rsObj.close

	end if
next

set rsObj = nothing
set objConn = Nothing

Response.Cookies("sTransactionIDs") = sTransactionIDs

%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"
        "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
	<meta http-equiv="content-type" content="text/html; charset=iso-8859-1">
	<title>Check out</title>

<style type="text/css">
BODY {
	PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px;
	FONT-FAMILY: Verdana, sans-serif;
}
</style>

<script language="JavaScript">
var mainf = null;
function init () {
	if (!window.opener) {
		alert('not allowed!');
	} else{
		var returl = '<%=Request.Form("return")%>';
		var cancelurl = '<%=Request.Form("cancel_return")%>';

		var payForm = document.getElementById('payForm');
		payForm.action = 'https://www.paypal.com/cgi-bin/webscr';
		var pp_cmd = document.getElementById('pp_cmd');
		pp_cmd.value = '_cart';
		var pp_upload = document.getElementById('pp_upload');
		pp_upload.value = '1';
		var pp_business = document.getElementById('pp_business');
		pp_business.value = 'rsherline@bfwpub.com';
		var pp_notify_url = document.getElementById('pp_notify_url');
		pp_notify_url.value = '#';
		var pp_return = document.getElementById('pp_return');
		pp_return.value = 'http://'+ window.location.hostname + '/RA/pp_return.asp?returl='+encodeURIComponent(returl)+'&cancelurl='+encodeURIComponent(cancelurl);
		var pp_cancel_return = document.getElementById('pp_cancel_return');
		pp_cancel_return.value = 'http://'+ window.location.hostname + '/RA/pp_cancel.asp?returl='+encodeURIComponent(returl)+'&cancelurl='+encodeURIComponent(cancelurl);
		var pp_rm = document.getElementById('pp_rm');
		pp_rm.value = '2';
		var pp_custom = document.getElementById('pp_custom');
		pp_custom.value = '<%=sTransactionIDs%>';
		var pp_no_shipping = document.getElementById('pp_no_shipping');
		pp_no_shipping.value = '1';
		var pp_page_style = document.getElementById('pp_page_style');
		pp_page_style.value = 'StudentCenter';
		var pp_cbt = document.getElementById('pp_cbt');
		pp_cbt.value = 'CLICK HERE TO FINISH UNLOCKING YOUR PREMIUM RESOURCES';

	}

	payForm.submit();
}
</script>

</head>
<body onload="init();">

<div id="ppf" style="display:none;">
<%=Request.Form("return")%><br/>
<%=Request.Form("cancel_return")%><br/>
<br/>

<form id="payForm" method="post" action="">
<input type="button" name="go" value="go" onclick="this.form.submit()"/><br/>
<input type="hidden" name="cmd" id="pp_cmd" /><br/>
<input type="hidden" name="upload" id="pp_upload" /><br/>
<input type="hidden" name="business" id="pp_business" /><br/>
<input type="hidden" name="notify_url" id="pp_notify_url" /><br/>
<input type="hidden" name="return" id="pp_return" /><br/>
<input type="hidden" name="cancel_return" id="pp_cancel_return" /><br/>
<input type="hidden" name="rm" id="pp_rm" /><br/>
<input type="hidden" name="custom" id="pp_custom" /><br/>
<input type="hidden" name="no_shipping" id="pp_no_shipping" /><br/>
<input type="hidden" name="page_style" id="pp_page_style" /><br/>
<input type="hidden" name="cbt" id="pp_cbt" /><br/>
<%
for i=1 to 30
	if Request.Form("item_name_"&i) <> "" then
%>
	<input type="hidden" name="item_name_<%=i%>" value="<%=Request.Form("item_name_"&i)%>" />
	<input type="hidden" name="amount_<%=i%>" value="<%=Request.Form("amount_"&i)%>" />
<%
	end if
next
%>
</form>

</div>

</body>
</html>
