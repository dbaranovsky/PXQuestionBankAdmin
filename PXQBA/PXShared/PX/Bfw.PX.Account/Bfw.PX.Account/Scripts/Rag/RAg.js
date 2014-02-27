var BFWj = {
    Page: {
        RAReady_Go: function () {}
    }
};

/* **********************************************************************
	The following JS include contains REQUIRED RAg functions for this 
	site, along with site-specific functions dependent on the RA 
	initialization state.
*/
// **********************************************************************
/*
    jQuery(document).ready should start MySite_init, which ONLY sets up "loading" state UI and starts the RA init.
    MySite_RAReady_go can set up the page, trigger RAif for RA forms, etc.
    MySite_RAif_Close can trigger CallBack, but note that this function is skipped if user's logged in status changed during RAif operation.
    The CallBack doesn't need to update session if !RA_CtrlWin.RA.RAif.updated
    
    For RAif state 'dologin', you need to populate RA.RAif.vars['Email'] and RA.RAif.vars['Password'] before calling RAif.init.
    See MySite_RA_doLogin .
*/
function MySite_init() {
    jQuery(window).unload(function() { jQuery.cookie('RAXSsl', '', { path: RA_CtrlWin.RA.CookiePath }); });
    //alert('inside MySite_init in RAg.js');
    // -----------------------------------------------------------------
    //optional "page loading" overlay
	MySite_PageWin = window.self;
    //MySite_RA_SetEventWireup();
	jQuery('#MySite_page_grayed').show();
	jQuery('#MySite_overlay_msg').show();
    //jQuery('#MySite_page_grayed').height( jQuery(document).height() );
	//MySite_CtrlWin.MySite_page_grayed_Resize();

    // -----------------------------------------------------------------
	//REQUIRED. The following sets up RAg live debugging mechanism.
	if (RA_CtrlWin.RA.dev_check('any')) {
		jQuery('#RA_debugmsg_toggle').show();
		jQuery('#RA_debugmsg').show();
		jQuery('#RA_info_div_toggle').show();
		jQuery('#RA_info_div').show();
	}
	jQuery('#RA_debugmsg_toggle').click( function() {
		jQuery('#RA_debugmsg').toggle();
	});
	jQuery('#RA_info_div_toggle').click( function() {
		jQuery('#RA_info_div').toggle();
	});

    // -----------------------------------------------------------------
    // REQUIRED
	// call to wait for RA to be initialized, or throw a timeout error
	RA_CtrlWin.RA.WaitFor();
    // This function needs to start the RA site and user initialization sequence.
    // Any site-specific logic which depends on RA to be ready should go in
    // the WaitFor_go function defined in the <head>.
    // IMPORTANT: The second parameter is used to mask your dev site to a different 
    //            BaseURL registered in RA. It should NEVER be used outside of a
	//            debugging mode unless explicitly approved.
	//var MySite_BaseURL = 'dev.px.bfwpub.com/bps5e/bcs';
	//var MySite_BaseURL = 'int-bcs.bedfordstmartins.com/bbibliographer';
	var MySite_BaseURL = '';
	MySite_BaseURL = BFW_QStr['target'];
	if ((!MySite_BaseURL) && window.location.host.substring(0, 9) == 'localhost') {
	    //MySite_BaseURL = 'dev.px.bfwpub.com/henrettaconcise4ev2/bcs/379';
	    MySite_BaseURL = 'dev.px.bfwpub.com/bps5e/lms/20';
	}
	MySite_BaseURL = MySite_BaseURL.replace('http://', '').replace('https://', '');
	if (MySite_BaseURL.indexOf('http://') == 0) {
	    MySite_BaseURL = MySite_BaseURL.substr(6);
	}
	else if (MySite_BaseURL.indexOf('https://') == 0) {
	    MySite_BaseURL = MySite_BaseURL.substr(7);
	}
	if ((MySite_BaseURL == '' || MySite_BaseURL == null) && window.location.host.substring(0, 9) == 'localhost') {
	    //MySite_BaseURL = 'dev.px.bfwpub.com/henrettaconcise4ev2/bcs/379';
	    MySite_BaseURL = 'dev.px.bfwpub.com/bps5e/lms/20';
	}
//	else
//	    MySite_BaseURL = window.location.host;
	    
	RA_CtrlWin.RA.UsingClasses = true;
	RA_CtrlWin.RA.Init( MySite_RA_Site_Inited, MySite_BaseURL );
	// USE THIS INSTEAD: 
	//RA_CtrlWin.RA.Init( MySite_RA_Site_Inited );
}
// **********************************************************************
// WaitFor_go function
// Define a function to call after RA is ready.

RA_CtrlWin.RA.WaitFor_go = MySite_RAReady_Go;
RA_CtrlWin.RA.WaitFor_error_go = MySite_RAReady_Error;
RA_CtrlWin.RA.UseClasses = false;


function MySite_RA_Site_Inited () {
//alert('MySite_RA_Site_Inited');
	RA_CtrlWin.RA.WaitFor_clear();
}

// **********************************************************************
// Function to run if RA initialization errors out
function MySite_RAReady_Error (severity,htmlMsg) {
	MySite_CtrlWin.MySite_Error( severity, htmlMsg );

    // -----------------------------------------------------------------
    // REQUIRED
    // Handle error messaging here. How it is handled it is open.
    // The only known state is that RA initialization has failed and
    // any RA-related operations on the site will probably fail.
	jQuery('#MySite_UserInfo_AnonInner').html( htmlMsg );
	jQuery('#MySite_UserInfo_Anon').show();
	jQuery('#MySite_UserInfo_LoggedIn').hide();
}

// **********************************************************************
// Function to run after RA is initialized
// Draw the page HTML, or redraw parts of it
// Or redirect/reload for server-side scripts needed after RA
function MySite_RAReady_Go () {
    // -----------------------------------------------------------------
    // REQUIRED - more live debugging set-up
	if (RA_CtrlWin.RA.dev_check('any')) {
		var html = ''
		html += RA_CtrlWin.RA.DisplayAll();
		jQuery('#RA_info_divInner').html( html );
	}
    // -----------------------------------------------------------------
    // REQUIRED
	MySite_CtrlWin.MySite_Ready = true;

    // -----------------------------------------------------------------
    // optional post-RA init, such as UI handling
	//MySite_CtrlWin.MySite_Draw_UserInfo();
	jQuery('#MySite_page_grayed').hide();
	jQuery('#MySite_overlay_msg').hide();
	// draw DHTML elements
	MySite_CtrlWin.MySite_Draw_SiteInfo();
	MySite_CtrlWin.MySite_Draw_UserInfo();
	MySite_CtrlWin.BFWj.Page.RAReady_Go();
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
	jQuery('#MySite_SiteInfoInner').html( html );
}

// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// Example of displaying useful CurrentUser data and options

function MySite_Draw_UserInfo () {
	if (RA_CtrlWin.RA.CurrentUser != null) {
		var html = '';
		html += ''+ RA_CtrlWin.RA.CurrentUser.FName +' '+ RA_CtrlWin.RA.CurrentUser.LName +' ('+ RA_CtrlWin.RA.CurrentUser.Email +')';
		jQuery('#MySite_RA_UserName').html( html );
		html = '';
		html += RA_CtrlWin.RA.GetLevelOfAccess_Description(RA_CtrlWin.RA.CurrentSiteAccess());
		jQuery('#MySite_RA_UserAccess').html( html );

		jQuery('#MySite_UserInfo_Anon').hide();
		jQuery('#MySite_UserInfo_LoggedIn').show();
	} else {
		jQuery('#MySite_UserInfo_Anon').show();
		jQuery('#MySite_UserInfo_LoggedIn').hide();
	}
}

// **********************************************************************
// EXAMPLE - 
// How to trigger RAg login from local form.
function MySite_RA_doLogin () {
   // alert('inside MySite_RA_doLogin');
	
	RA_CtrlWin.RA.RAif.vars['Email'] = jQuery('#RA_Email').val();
	RA_CtrlWin.RA.RAif.vars['Password'] = jQuery('#RA_Password').val();
	MySite_CtrlWin.MySite_RAif_init('dologin');
}

// **********************************************************************
// EXAMPLE - 
// How to trigger RAg check email (the starting point for the 
// registration sequence) from local form.
function MySite_RA_doCheckEmail () {
	//alert('MySite_RA_doCheckEmail');
	RA_CtrlWin.RA.RAif.vars['Email'] = jQuery('#RA_CheckEmail')[0].value;
	MySite_CtrlWin.MySite_RAif_init('docheckemail');
}



// **********************************************************************
// REQUIRED
// RA iframe initialize function, required in order to customize display of RAif
function MySite_RAif_init(state, mode) {

    //alert('inside MySite_RAif_init');
	setTimeout('MySite_CtrlWin.MySite_RAif_init_go(\''+state+'\',\''+mode+'\')',100);
}
function MySite_RAif_init_go (state,mode) {
	if (!state) state = '';
	if (!mode) mode = '';
    // -----------------------------------------------------------------
    // optional
    // When RAif is opened, the UI should change to accommodate it.
    // One method is to use a tranparent layer between the page and RAif.
	jQuery('#MySite_page_grayed').show();
	RA_CtrlWin.scrollTo(0,0);
    // -----------------------------------------------------------------
    // REQUIRED
    // Set the RAif on-close function, then init RAif
	RA_CtrlWin.RA.RAif.close_site = MySite_RAif_Close;
	RA_CtrlWin.RA.RAif.init(state,mode);
//	RA_CtrlWin.RA.RAUI.close_site = MySite_RAif_Close;
//	RA_CtrlWin.RA.RAUI.Init( state );
	// -----------------------------------------------------------------
    // optional UI continued
	//MySite_CtrlWin.MySite_page_grayed_Resize();
}
// **********************************************************************
// RA iframe close function
// Required to update page UI after RAif closes
function MySite_RAif_Close () {
	//alert('Close Called');
	if (RA_CtrlWin.RA.RAif.updated) {
		MySite_CtrlWin.MySite_Draw_UserInfo();
	}
	jQuery('#MySite_page_grayed').hide();
}
// **********************************************************************
// RA iframe resize function
// Required in order for RAif page to resize this page as needed.
// (In this example, only the page_grayed overlay needs to be resized)
function RAif_Site_Resize () {
	//MySite_CtrlWin.MySite_page_grayed_Resize();
}


// **********************************************************************
// optional
function MySite_uninit() {
	window.scrollTo(0,0);
	try {
		jQuery('#MySite_page_grayed').show();
		jQuery('#MySite_overlay_msg').show();
	} catch (e) {}
}


// **********************************************************************
// Example of custom Error function
// To see it work, remove one of the required js files, like /BFWglobal/js/global.js


function MySite_Error(severity, htmlMsg) {
    //alert(htmlMsg);
    //return;
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
//	x.style.display='block';
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
    //return; //RAUI
    //alert('............MySite_page_grayed_Resize...........');
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
alert('resize : '+ h);
}

function MySite_RA_SetEventWireup(){
	jQuery('body')[0].setAttribute('onunload','MySite_uninit()');
}

