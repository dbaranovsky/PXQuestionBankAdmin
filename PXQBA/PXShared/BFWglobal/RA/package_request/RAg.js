
$(document).ready(function(){
	MySite_init();
});
/* **********************************************************************
	The following JS include contains custom functions for this site.
*/


// **********************************************************************
// WaitFor_go function
// Define a function to call after RA is ready.

RA_CtrlWin.RA.WaitFor_go = MySite_RAReady_Go;
RA_CtrlWin.RA.WaitFor_error_go = MySite_RAReady_Error;
RA_CtrlWin.RA.UseClasses = false;


// **********************************************************************
// Body OnLoad (or document.ready) function

function MySite_init() {
//alert('MySite_init');
/*
	optional "page loading" overlay
*/
	MySite_PageWin = window.self;
	$('#MySite_page_grayed').show();
	$('#MySite_overlay_msg').show();
	MySite_CtrlWin.MySite_page_grayed_Resize();
/**/

	if (RA_CtrlWin.RA.dev_check()) {
		$('#RA_debugmsg_toggle').show();
		$('#RA_debugmsg').show();
		$('#RA_info_div_toggle').show();
	} else {
		$('#RA_info_div_toggle').hide();
	}
	$('#RA_debugmsg_toggle').click( function() {
		$('#RA_debugmsg').toggle();
	});
	$('#RA_info_div_toggle').click( function() {
		$('#RA_info_div').toggle();
	});

	// call to wait for RA to be initialized, or throw a timeout error
	RA_CtrlWin.RA.WaitFor();

// **********************************************************************
// This function needs to start the RA site and user initialization sequence.
// Any site-specific logic which depends on RA to be ready should go in
// the WaitFor_go function defined in the <head>.
	// call to initialize current site (GetSiteFromBaseURL_WithProducts)
	RA_CtrlWin.RA.Init( MySite_RA_Site_Inited, MySite_BaseURL );
}
function MySite_RA_Site_Inited () {
//alert('MySite_RA_Site_Inited');
	RA_CtrlWin.RA.WaitFor_clear();
}

// **********************************************************************
// Function to run if RA initialization errors out

function MySite_RAReady_Error (severity,htmlMsg) {

	MySite_CtrlWin.MySite_Error( severity, htmlMsg );

	$('#MySite_UserInfo_AnonInner').html( htmlMsg );
	$('#MySite_UserInfo_Anon').show();
	$('#MySite_UserInfo_LoggedIn').hide();
}

// **********************************************************************
// Function to run after RA is initialized
// Draw the page HTML, or redraw parts of it
// Or redirect/reload for server-side scripts needed after RA

function MySite_RAReady_Go () {

	if (RA_CtrlWin.RA.dev_check()) {
		var html = ''
		html += RA_CtrlWin.RA.DisplayAll();
		$('#RA_info_divInner').html( html );
	}

	MySite_CtrlWin.MySite_Ready = true;

	// draw DHTML elements
	MySite_CtrlWin.MySite_Draw_UserInfo();
/*
	optional "page loading" overlay
*/
	$('#MySite_page_grayed').hide();
	$('#MySite_overlay_msg').hide();
/**/

}


// **********************************************************************
// **********************************************************************


// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// Example of displaying useful CurrentUser data and options

function MySite_Draw_UserInfo () {
	var html = '';

	$('#MySite_loading').hide();
	if (RA_CtrlWin.RA.CurrentUser != null) {
		if (!MySite_BaseURL || MySite_BaseURL=='') {
			html = '';
			html += '<p>No resource was specified.</p>';
			if (RA_CtrlWin.RA.CurrentUser != null) {
				html += '<br/><hr/>';
				html += '<table class="MySite_RA_Login_Tbl" width="100%" border="0" cellpadding="0" cellspacing="0"><TR><td width="50" class="MySite_RA_Login_TblTDButton"><input type="button" name="logoff" onclick="JavaScript:MySite_RAif_init(\'dologout\')" value="LOG OUT"/></td><td>&nbsp;</td></TR></table>';
			}
			$('#MySite_UserInfo_LoggedInInner').html( html );
			$('#MySite_UserInfo_LoggedIn').show();
			return;
		} else if (BFW_QStr['test']!='yes') {
			var goURL = 'http://'+ MySite_BaseURL +'';
			window.location.href = goURL;
			return;
		}
//alert(goURL);

		var goURL = 'http://'+ MySite_BaseURL +'';
		$('#MySite_UserInfo_Anon').hide();

		html = '';
		html += '<p><nobr>Welcome, <div class="MySite_RA_UserName"></div>.</nobr>';
		if (RA_CtrlWin.RA.dev_check()) {
			html += ' <nobr>You have <div class="MySite_RA_UserAccess"></div> access.</nobr>';
		}
		html += '</p>';

		html += '<p><a href="'+ goURL +'">Click here</a> to continue</p>';

		html += '<br/><hr/>';
		html += '<table class="MySite_RA_Login_Tbl" width="100%" border="0" cellpadding="0" cellspacing="0"><TR><td width="50" class="MySite_RA_Login_TblTDButton"><input type="button" name="logoff" onclick="JavaScript:MySite_RAif_init(\'dologout\')" value="LOG OUT"/></td><td>&nbsp;</td></TR></table>';

		$('#MySite_UserInfo_LoggedInInner').html( html );
		$('#MySite_UserInfo_LoggedIn').show();

		html = '';
		html += ''+ RA_CtrlWin.RA.CurrentUser.FName +' '+ RA_CtrlWin.RA.CurrentUser.LName +' ('+ RA_CtrlWin.RA.CurrentUser.Email +')';
		$('.MySite_RA_UserName').html( html );
		html = '';
		html += RA_CtrlWin.RA.GetLevelOfAccess_Description(RA_CtrlWin.RA.CurrentSiteAccess());
		$('.MySite_RA_UserAccess').html( html );

	} else {
		$('#MySite_UserInfo_Anon').show();
		$('#MySite_UserInfo_LoggedIn').hide();
	}
}
function MySite_RA_doLogin () {
	RA_CtrlWin.RA.RAif.vars['Email'] = $('#RA_Email').val();
	RA_CtrlWin.RA.RAif.vars['Password'] = $('#RA_Password').val();
	MySite_CtrlWin.MySite_RAif_init('dologin');
}
function MySite_RA_doCheckEmail () {
	RA_CtrlWin.RA.RAif.vars['Email'] = $('#RA_CheckEmail')[0].value;
	MySite_CtrlWin.MySite_RAif_init('docheckemail');
}

// **********************************************************************
// RA iframe initialize function
// 	required in order to customize display of RAif
function MySite_RAif_init (state,mode) {
	setTimeout('MySite_CtrlWin.MySite_RAif_init_go(\''+state+'\',\''+mode+'\')',100);
}
function MySite_RAif_init_go (state,mode) {
	if (!state) state = '';
	if (!mode) mode = '';
	$('#MySite_page_grayed').show();
	RA_CtrlWin.RA.RAif.close_site = MySite_RAif_Close;
	RA_CtrlWin.RA.RAif.init(state,mode);
	RA_CtrlWin.scrollTo(0,0);
	MySite_CtrlWin.MySite_page_grayed_Resize();
}
// **********************************************************************
// RA iframe close function
// 	required in order to update page after RAif, if user is updated
function MySite_RAif_Close () {
	if (RA_CtrlWin.RA.RAif.updated) {
		MySite_CtrlWin.MySite_Draw_UserInfo();
	}
	$('#MySite_page_grayed').hide();
}
// **********************************************************************
// RA iframe resize function
// 	required in order for RAif page to resize this page as needed
// 	(in this example, only the page_grayed overlay needs to be resized)
function RAif_Site_Resize () {
	MySite_CtrlWin.MySite_page_grayed_Resize();
}







// **********************************************************************
function MySite_uninit() {
	// not required
	window.scrollTo(0,0);
	try {
/*
	optional "page loading" overlay
*/
		$('#MySite_page_grayed').show();
		$('#MySite_overlay_msg').show();
/**/
	} catch (e) {}
}


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




