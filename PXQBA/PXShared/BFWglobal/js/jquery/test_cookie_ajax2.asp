<html>
<head>
<script type="text/javascript" src="jquery-1.3.2.min.js"></script>
<script type="text/javascript" src="/BFWGlobal/js/cookies.js"></script>
<script type="text/javascript" src="/BFWGlobal/js/global.js"></script>
<script type="text/javascript">

var c = BFW_QStr['c'];
var returl = BFW_QStr['returl'];

$(document).ready(function(){
	setCookie('c',c);
<%
Response.Cookies("cc") = Request.QueryString("c")
Response.Cookies("cc").domain = "192.168.77.114"
%>
	var str = 'c = '+ c +'<br/>';
	str += 'cookie = <%=Request.Cookies("cc")%><br/>'
	$('#msg').html( str );
	setTimeout('window.location.href = \''+ returl +'\';', 3000);
});

</script>
</head>
<body>
<br/>
<br/>

<div id="msg">
loading...
</div

<br/>
<br/>

</body>
</html>