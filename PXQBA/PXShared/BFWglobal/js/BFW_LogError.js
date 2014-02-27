// **********************************************************************
/*
BFW LogError function
Author: Chad Crume
Requires:
	<iframe id="BFW_LogError" ...>
*/

function BFW_LogError (to,pageUrl,ename,msg) {
//alert('BFW_LogError');
return;
	var LogErrorURL = 'http://bcs.bfwpub.com/BFWglobal/LogError/LogError.asp';
	switch (window.location.host) {
	case '192.168.77.114' :
//		LogErrorURL = 'http://192.168.77.114/BFWglobal/LogError/LogError.asp';
		break;
	case '192.168.77.242' :
		break;
	default :
	}
	pageUrl = pageUrl.replace('http://','');
/*
*/
	try {
		document.getElementById('BFW_LogError').src = ''+
LogErrorURL +'?to='+ to +'&url='+ encodeURIComponent(pageUrl) +'&n='+ ename +'&msg='+ encodeURIComponent(msg);
	} catch (e) {
		try {
			window.open(''+
LogErrorURL +'?to='+ to +'&url='+ encodeURIComponent(pageUrl) +'&n='+ ename +'&msg='+ encodeURIComponent('NO BFW_LogError IFRAME (user will get a pop-up)\n\n'+ msg) );
		} catch (e) {
			alert('SOME ERROR ... ? :: '+ e.name +' --- '+ e.message +' === '+''+
LogErrorURL +'?to='+ to +'&url='+ encodeURIComponent(pageUrl) +'&n='+ ename +'&msg='+ encodeURIComponent('NO BFW_LogError IFRAME (user will get a pop-up)\n\n'+ msg) );
		}
	}
}
