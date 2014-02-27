// **********************************************************************
// Example of custom Error function
// To see it work, remove one of the required js files, like /BFWglobal/js/global.js


function MySite_Error (severity,htmlMsg) {
	if (!htmlMsg) htmlMsg = 'An error occurred. We\'re sorry for the inconvenience. Please try again in a few minutes.';
	if (severity>5) {
		htmlMsg += '<br/><br/><a href="JavaScript:window.location.reload()">try again</a>';
//		htmlMsg += '<br/><br/><a href="JavaScript:MySite_Error_Close()">continue</a>';
	} else {
		htmlMsg += '<br/><br/><a href="JavaScript:window.location.reload()">try again</a>';
		htmlMsg += '<br/><br/><a href="JavaScript:MySite_Error_Close()">continue</a>';
	}
/*
	optional "error" overlay
*/
	MySite_PageWin = window.self;
	var x = MySite_PageWin.document.getElementById('MySite_page_grayed');
	x.style.display='block';
	var yInner = MySite_PageWin.document.getElementById('MySite_overlay_msgInner');
	yInner.style.textAlign = 'left';
	yInner.style.width = '450px';
	yInner.style.height = '200px';
	yInner.innerHTML = htmlMsg;
	var y = MySite_PageWin.document.getElementById('MySite_overlay_msg');
	y.style.display='block';
/**/
}
// Function to close "error" overlay
function MySite_Error_Close () {
/*
	optional "error" overlay
*/
	var x = MySite_PageWin.document.getElementById('MySite_page_grayed');
	var y = MySite_PageWin.document.getElementById('MySite_overlay_msg');
	var yInner = MySite_PageWin.document.getElementById('MySite_overlay_msgInner');
	var htmlMsg = '<br/><br/><br/>loading, please wait...';
	yInner.style.textAlign = 'center';
	yInner.style.width = '300px';
	yInner.style.height = '100px';
	y.innerHTML = htmlMsg;
	x.style.display='none';
	y.style.display='none';
/**/
}

// **********************************************************************
// **********************************************************************

function MySite_page_grayed_Resize() {
	var h = document.body.offsetHeight;
/*
	optional "page loading" overlay
*/
	var hy = MySite_PageWin.document.getElementById('MySite_overlay_msg').offsetHeight;
	if (hy>h) h = hy;
/**/
	var hRA = MySite_PageWin.document.getElementById('RAif_div').offsetHeight;
	if (hRA>h) h = hRA;
	h += 180;

	// resize MySite_page_grayed to fit page
	var x = MySite_PageWin.document.getElementById('MySite_page_grayed');
	if (h>1000) {
		x.style.height = h + 'px';
	} else {
		x.style.height = '1000px';
	}
//alert('resize : '+ h);
}

