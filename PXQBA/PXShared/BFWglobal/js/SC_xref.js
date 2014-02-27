var SCLINK_host = 'bcs.bedfordstmartins.com';
var SCLINK_host_1 = 'dev-'+SCLINK_host;
var SCLINK_host_2 = 'stg-'+SCLINK_host;
var SCLINK_host_0 = '192.168.77.242';
switch (SCLINK_host)
{
case 'bcs.bedfordstmartins.com':
	SCLINK_host_0 = '192.168.77.243';
break;
case 'bcs.whfreeman.com':
	SCLINK_host_0 = '192.168.77.244';
break;
case 'bcs.worthpublishers.com':
	SCLINK_host_0 = '192.168.77.245';
break;
}
switch (window.location.host)
{
case '192.168.77.222' :
	SCLINK_host = SCLINK_host_0;
break;
case 'dev-content.bfwpub.com' :
	SCLINK_host = SCLINK_host_1;
break;
case 'stg-content.bfwpub.com' :
	SCLINK_host = SCLINK_host_2;
break;
}
$(document).ready(function(){
	$('.SCLINK').html(SCLINK_TEXT);
	$('.SCLINK').live('click', SCLINK);
})
function SCLINK () {
//	alert('http://'+ SCLINK_host + SCLINK_HREF);
//	top.location.href = 'http://'+ SCLINK_host + SCLINK_HREF;
	var x = SCLINK_HREF.split('#');
	var d = new Date();
	var SCLINK_HREF_GO = x[0] +'?'+ d.getMilliseconds();
	for (var i=1; i<x.length; i++)
	{
		SCLINK_HREF_GO += '#'+ x[i];
	}
	SCLINK_HREF_GO = 'http://'+ SCLINK_host + SCLINK_HREF_GO;
//alert(SCLINK_HREF_GO);
	this.href = SCLINK_HREF_GO;
	return true;
}
