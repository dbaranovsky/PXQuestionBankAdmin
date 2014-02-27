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
		$('#RA_info_div_toggle').show();
	} else {
		$('#RA_info_div_toggle').hide();
	}
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
	RA_CtrlWin.RA.Init( MySite_RA_Site_Inited );
}
function MySite_RA_Site_Inited () {
//alert('MySite_RA_Site_Inited');
	RA_CtrlWin.RA.WaitFor_clear();
}

// **********************************************************************
// Function to run if RA initialization errors out

function MySite_RAReady_Error (severity,htmlMsg) {
//	yoda.haha;
	MySite_CtrlWin.MySite_Error( severity, htmlMsg );

	$('#MySite_SiteInfoInner').html( htmlMsg );
	$('#MySite_UserInfo_AnonInner').html( htmlMsg );
	$('#MySite_UserInfo_Anon').show();
	$('#MySite_UserInfo_LoggedIn').hide();
}

// **********************************************************************
// Function to run after RA is initialized
// Draw the page HTML, or redraw parts of it
// Or redirect/reload for server-side scripts needed after RA

function MySite_RAReady_Go () {
	MySite_CtrlWin.MySite_Ready = true;
/*
	optional "page loading" overlay
*/
	$('#MySite_page_grayed').hide();
	$('#MySite_overlay_msg').hide();
/**/

	if (RA_CtrlWin.RA.dev_check()) {
		var html = ''
		html += RA_CtrlWin.RA.DisplayAll();
		$('#RA_info_divInner').html( html );
	}

	// draw DHTML elements
	MySite_CtrlWin.MySite_Draw_SiteInfo();
	MySite_CtrlWin.MySite_Draw_UserInfo();
	MySite_CtrlWin.MySite_Draw_RAInfo();
}


// **********************************************************************
// **********************************************************************


// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// Example of useful CurrentSite data

function MySite_Draw_SiteInfo () {
	var html = ''
	if (RA_CtrlWin.RA.CurrentSite != null) {
		html += '<p>';
		html += 'BaseURL: '+ RA_CtrlWin.RA.CurrentSite.BaseURL;
		html += '<br/>';
		html += 'ID: '+ RA_CtrlWin.RA.CurrentSite.ID;
		html += '<br/>';
		html += '</p>';
	} else {
		html += '<p>';
		html += 'CurrentSite is null';
		html += '</p>';
	}
	$('#MySite_SiteInfoInner').html( html );
}

// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// Example of displaying useful CurrentUser data and options

function MySite_Draw_UserInfo () {
	if (RA_CtrlWin.RA.CurrentUser != null) {
		var html = '';
		html += ''+ RA_CtrlWin.RA.CurrentUser.FName +' '+ RA_CtrlWin.RA.CurrentUser.LName +' ('+ RA_CtrlWin.RA.CurrentUser.Email +')';
		$('#MySite_RA_UserName').html( html );
		html = '';
		html += RA_CtrlWin.RA.GetLevelOfAccess_Description(RA_CtrlWin.RA.CurrentSiteAccess());
		$('#MySite_RA_UserAccess').html( html );

		$('#MySite_UserInfo_Anon').hide();
		$('#MySite_UserInfo_LoggedIn').show();
	} else {
		$('#MySite_UserInfo_Anon').show();
		$('#MySite_UserInfo_LoggedIn').hide();
	}
}
function MySite_RA_doLogin () {
	RA_CtrlWin.RA.RAif.vars['Email'] = $('#RA_Email').val(); // KT: trim() must be done in each site's JS to trim whitespace
	RA_CtrlWin.RA.RAif.vars['Password'] = $('#RA_Password').val();
	MySite_CtrlWin.MySite_RAif_init('dologin');
}
function MySite_RA_doCheckEmail () {
	RA_CtrlWin.RA.RAif.vars['Email'] = $('#RA_CheckEmail')[0].value;
	MySite_CtrlWin.MySite_RAif_init('docheckemail');
}

// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// Example of useful complete debug dump info
// Must have "radev=yes" on the querystring or nothing displays

function MySite_Draw_RAInfo () {
	if (RA_CtrlWin.RA.dev_check()) {
		var html = ''
		html += RA_CtrlWin.RA.DisplayAll();
		$('#RA_info_divInner').html( html );
		$('#RA_info_div').show();
	} else {
		$('#RA_info_div').hide();
	}
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
		MySite_CtrlWin.MySite_Draw_SiteInfo();
		MySite_CtrlWin.MySite_Draw_UserInfo();
		MySite_CtrlWin.MySite_Draw_RAInfo();
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

function trim(str, chars) {
	return ltrim(rtrim(str, chars), chars);
}

function ltrim(str, chars) {
	chars = chars || "\\s";
	return str.replace(new RegExp("^[" + chars + "]+", "g"), "");
}

function rtrim(str, chars) {
	chars = chars || "\\s";
	return str.replace(new RegExp("[" + chars + "]+$", "g"), "");
}





// **********************************************************************
function MySite_uninit() {
	// not required
	window.scrollTo(0,0);
/*
	optional "page loading" overlay
*/
	$('#MySite_page_grayed').show();
	$('#MySite_overlay_msg').show();
/**/
}



