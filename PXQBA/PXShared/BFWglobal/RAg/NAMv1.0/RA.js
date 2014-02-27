if (!this.RA_CtrlWin) 
{
	this.RA_CtrlWin = window.self;
}
if (!RA_CtrlWin.RA) 
{
(function ($) {

    if (!$) 
    {
        throw new Error('jQuery required for RAg');
		return;
    }
    else if (typeof $.fn.position !== 'function') 
    {
        throw new Error('jQuery position required for RAg');
		return;
    }
	// *****************************************************************************
	// *****************************************************************************
	// RA object
	RA_CtrlWin.RA = {
			SiteInfo: {}
		,	SiteUserInfo: {}
		,	RavellAuthentication: false
		,	SecureUrl: ''
		,	PublicUrl: ''
		,	TargetSecureUrl: '' 
		,	TargetPublicUrl: ''
		,	CookiePath: '/'
	};

	RA_CtrlWin.RA.setOptions = function (inOptions) {
	/*
		this.debugMsg = '';
		this.dev = BFW_QStr['radev'];
		if (!this.dev) this.dev = '';
		this.ProxyType = RA_CtrlWin.BFW_SERVER_WS_PROXY_TYPE;
		this.LocalProxyURL = '';
		this.LocalRAUIURL = '';
		this.PersistentSession = false
		this.UsingClasses = false;
		this.RAUICloseAlwaysUpdate = false;
		this.CookiePath = '/';
		this.RavellAuthentication = false;
		this.UseLoginRef = true;
		this.GhostURL = null;
		this.RARootDomain = 'bcs.bfwpub.com';
		this.RAXSURL = this.RARootDomain +'/RA/RAXS/v1.3'
		this.OldLoginRefURL = this.RARootDomain +'/login_reference';
		this.RAUI.UseIFrame = false;
		this.RAUI.OnUIResizeCallback = null;
		this.SecureUrl = '';
		this.PublicUrl = '';
		this.TargetSecureUrl = '';
		this.TargetPublicUrl = '';
	*/
		if (inOptions.OnUIResizeCallback)
		{
			this.RAUI.OnUIResizeCallback = inOptions.OnUIResizeCallback;
		}
		if (inOptions.SecureUrl) {
			this.SecureUrl = inOptions.SecureUrl;
		}
		if (inOptions.PublicUrl) {
			this.PublicUrl = inOptions.PublicUrl;
		}
		if (inOptions.TargetSecureUrl) {
			this.TargetSecureUrl = inOptions.TargetSecureUrl;
		}
		if (inOptions.TargetPublicUrl) {
			this.TargetPublicUrl = inOptions.TargetPublicUrl;
		}
		if (inOptions.UseIFrame) {
			this.RAUI.UseIFrame = inOptions.UseIFrame;
		}
		if (inOptions.dev) {
			this.dev = inOptions.dev;
		}
		if (inOptions.ProxyType) {
			switch (inOptions.ProxyType)
			{
				case 'ASP.NET':
					this.ProxyType = inOptions.ProxyType;
					break
				default:
					this.ProxyType = inOptions.BFW_SERVER_WS_PROXY_TYPE;
			}
		}
		if (this.ProxyType=='ASP.NET' && inOptions.LocalProxyURL) {
			this.LocalProxyURL = inOptions.LocalProxyURL;
		}
		if (this.ProxyType=='ASP.NET' && inOptions.LocalRAUIURL) {
			this.LocalRAUIURL = inOptions.LocalRAUIURL;
		}
		if (inOptions.PersistentSession) {
			this.PersistentSession = inOptions.PersistentSession;
		}
		if (inOptions.UsingClasses) {
			this.UsingClasses = inOptions.UsingClasses;
		}
		if (inOptions.RAUICloseAlwaysUpdate) {
			this.RAUICloseAlwaysUpdate = inOptions.RAUICloseAlwaysUpdate;
		}
		if (inOptions.RavellAuthentication) {
			this.RavellAuthentication = inOptions.RavellAuthentication;
		}
		if (inOptions.UseLoginRef) {
			this.UseLoginRef = inOptions.UseLoginRef;
		}
		if (inOptions.CookiePath) {
			this.CookiePathOption = inOptions.CookiePath;
		}
		if (inOptions.WaitFor_go && typeof inOptions.WaitFor_go === 'function') {
			this.WaitFor_go = inOptions.WaitFor_go;
		}
		if (inOptions.WaitFor_error_go && typeof inOptions.WaitFor_error_go === 'function') {
			this.WaitFor_error_go = inOptions.WaitFor_error_go;
		}

	}
	RA_CtrlWin.RA.InitCallback = function () {};
	RA_CtrlWin.RA.Init = function ( callbackFn, ghostURL ) {
		if (callbackFn) this.InitCallback = callbackFn;
		else this.InitCallback = function () {};
		if (ghostURL) {
			this.GhostURL = ghostURL;
			this.usingGhostURL = true;
		}
		if (this.RavellAuthentication)
		{
			try {
				var JsonStr = $.cookie('SiteData');
				if (JsonStr!='')
				{
					this.SiteInfo = JSON.parse( decodeURIComponent(JsonStr), RA_CtrlWin.RA.parseReviver );
				}
				else
				{
					this.SiteInfo = null;
				}
			} catch(e) {
				this.recordError(e);
			}
			try {
				var JsonStr = $.cookie('SiteUserData');
				if (JsonStr!='')
				{
					this.SiteUserInfo = JSON.parse( decodeURIComponent(JsonStr), RA_CtrlWin.RA.parseReviver );
				}
				else
				{
					this.SiteUserInfo = null;
				}
			} catch(e) {
				this.recordError(e);
			}
			try {
				this.InitCallback();
			} catch(e) {
				this.recordError(e);
			}
		}
	}

	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	// WaitFor

	RA_CtrlWin.RA.WaitFor_interval = null;
	RA_CtrlWin.RA.WaitFor_timeout = null;
	RA_CtrlWin.RA.WaitFor_timeout_ct = 30000;
	RA_CtrlWin.RA.WaitFor_timedout = false;
	RA_CtrlWin.RA.WaitFor = function () {
	//alert('RA.WaitFor');
		RA_CtrlWin.RA.WaitFor_interval = window.setInterval('RA_CtrlWin.RA.WaitFor_check()', 100);
		RA_CtrlWin.RA.WaitFor_timedout = false;
		RA_CtrlWin.RA.WaitFor_timeout = window.setTimeout('RA_CtrlWin.RA.WaitFor_timedout=true',RA_CtrlWin.RA.WaitFor_timeout_ct);
	}

	RA_CtrlWin.RA.WaitFor_check = function () {
	//alert('RA.WaitFor_check');
		if (RA_CtrlWin.RA.WaitFor_timedout) {
			RA_CtrlWin.RA.WaitFor_error(RA_CtrlWin.RAWaitForTimeOutError);
		}
	}

	RA_CtrlWin.RA.WaitFor_clear = function () {
	//alert('RA.WaitFor_clear');
		RA_CtrlWin.RA.WaitFor_interval = window.clearInterval(RA_CtrlWin.RA.WaitFor_interval);
		RA_CtrlWin.RA.WaitFor_timeout = window.clearTimeout(RA_CtrlWin.RA.WaitFor_timeout);
		if ( ! RA_CtrlWin.RA.RAXS.Inited ) {
			RA_CtrlWin.RA.WaitFor_error(RA_CtrlWin.RAXSInitializeError);
		} else {
			try { 
				RA_CtrlWin.RA.WaitFor_go(); 
			} catch(e) { 
				alert('Error in site custom RA.WaitFor_go function: '+ e.name +' --- '+ e.message); 
				RA_CtrlWin.RA.WaitFor_error(e);
			}
		}
	}

	RA_CtrlWin.RA.WaitFor_error = function (err) {
	alert('RA.WaitFor_error');
		RA_CtrlWin.RA.WaitFor_interval = window.clearInterval(RA_CtrlWin.RA.WaitFor_interval);
		RA_CtrlWin.RA.WaitFor_timeout = window.clearTimeout(RA_CtrlWin.RA.WaitFor_timeout);
		if (!err)
		{
			err = new Error('unknown error');
		}
		BFW_Errors.add(err,1000);
		var htmlMsg = '';
		switch (err.name) {
		case 'RAXSInitializeError' :
			htmlMsg = 'We couldn\'t initialize your session. As a result, you may not be logged in properly.'
			break;
		case 'RAWaitForTimeOutError' :
			htmlMsg = 'We couldn\'t initialize your session. As a result, you may not be logged in properly..'
			break;
		}
		alert('RA.WaitFor_error 2');
		try {
			RA_CtrlWin.RA.WaitFor_error_go(1000,htmlMsg); 
		} catch(e) { 
			alert('RA.WaitFor_error 3');
			alert('Error in site custom RA.WaitFor_error_go function: '+ e.name +' --- '+ e.message); 
			BFW_Errors.add(e,1);
		} finally {
			BFW_Errors.LogErrors();
		}
	}

	RA_CtrlWin.RA.WaitFor_go = function () {};

	RA_CtrlWin.RA.WaitFor_error_go = function (severity,htmlMsg) {};

	// -----------------------------------------------------------------------------
	// Trace/debug functions

	RA_CtrlWin.RA.recordError = function ( e ) {
		alert( e.lineNumber+' --- '+e.message );
	}

	RA_CtrlWin.RA.dev = '';
	if (BFW_QStr['radev']) RA_CtrlWin.RA.dev = BFW_QStr['radev'];

	RA_CtrlWin.RA.dev_check = function ( val ) {
		if (val) {
			if (RA_CtrlWin.RA.dev==val || (val=='any' && RA_CtrlWin.RA.dev!='')) return true
		} else {
			if (RA_CtrlWin.RA.dev=='yes') return true
		}
		return false;
	}

	RA_CtrlWin.RA.logTrace = function (msg) {
		var traceLog = $('#'+ RA_CtrlWin.RA.RAUI.divId ).data('RAtrace');
		traceLog += msg;
		$('#'+ RA_CtrlWin.RA.RAUI.divId ).data('RAtrace', traceLog);
	}

	RA_CtrlWin.RA.dumpTrace = function () {
		var traceLog = $('#'+ RA_CtrlWin.RA.RAUI.divId ).data('RAtrace');
		$('#RA_debugmsg').append( traceLog );
	}

	RA_CtrlWin.RA.resetTrace = function () {
		$('#'+ RA_CtrlWin.RA.RAUI.divId ).data('RAtrace', '');
	}
	
	// *****************************************************************************
	// *****************************************************************************
	// RAXS Object

	RA_CtrlWin.RA.OldLoginRefURL = 'bcs.bfwpub.com/login_reference';
	RA_CtrlWin.RA.RAXS = new Object();
	RA_CtrlWin.RA.RAXS.Inited = false;
	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	RA_CtrlWin.RA.RAXS.CheckTimeStamp = function (RAXSsd) {
		var msg = 'RA_CtrlWin.RA.RAXS.CheckTimeStamp';
		var result = false;
		var raxsts = BFW_QStr['raxsts'];
		if (!raxsts)
		{
			if (!RAXSsd)
			{
				result = true;
			}
		}
		else
		{
			result = (RAXSsd == raxsts);
		}
		msg += ' : result='+result;
		RA_CtrlWin.RA.logTrace(msg);
		RA_CtrlWin.RA.dumpTrace();
		return result;
	}

	RA_CtrlWin.RA.RAXS.GetTimeStamp = function (m) {
		var d = new Date();
		var raxsts = m + d.getTime().toString();
		return raxsts;
	}

	RA_CtrlWin.RA.RAXS.GetURL = function (raxsts) {
		var msg = 'RA_CtrlWin.RA.RAXS.GetURL ( '+raxsts+' ) -- ';
		var xhref = jQuery.cookie('RAXSret');
		if (xhref == '' || xhref == null) {
	//alert(msg +'NO cookie');
			msg += ' cookie was blank, resetting ';
			xhref = top.location.host + decodeURIComponent(top.location.pathname) + top.location.search + top.location.hash;
			xhref = xhref.replace(/[?&]uid=[0-9]*&rau=[0-9]*/g,'');
			xhref = xhref.replace(/[?&]raxsts=[cio]?[0-9]*/g,'');
			jQuery.cookie('RAXSret',xhref, { path: '/' } );
			var ar = xhref.split('#');
			var ar2 = ar[0].split('?');
			xhref = '';
			for (var i=0; i<ar2.length; i++)
			{
				if (i==1)
					xhref += '?';
				else if (i>1)
					xhref += '&';
				xhref += ar2[i];
			}
			if (raxsts)
			{
				xhref += (ar2.length>1) ? '&raxsts='+raxsts : '?raxsts='+raxsts;
			}
			xhref += (ar.length>1) ? '#'+ar[1]+'' : '';
		}
		else
		{
	//alert(msg +'cookie');
			jQuery.cookie('RAXSret','', { path: '/' } );
		}
		msg += ' -- '+ xhref +'<br/>';
		RA_CtrlWin.RA.logTrace(msg);
		RA_CtrlWin.RA.dumpTrace();
		//prompt(msg, xhref);
		return xhref;
	}

	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	// RAXS.Check (RALR & RAXS)
	RA_CtrlWin.RA.RAXS.Check = function () {
		//return; //NEED TO FIX
var msg = '';
msg += 'RAXS.Check<br/>     ';
var dbcm = jQuery.cookie('dbcm');
dbcm = '';

		//REMOVE ????
		if (!navigator.cookieEnabled) return;

		if (top.location.search=='' && top.location.hash=='' && top.location.pathname.indexOf('%')>=0) 
		{
			top.location.href = decodeURIComponent(top.location.href);
			return;
		}

		var RAXSsl = jQuery.cookie('RAXSsl');
		var RAXSsd = jQuery.cookie('RAXSsd');
		if ( (RAXSsl==''||RAXSsl==null) && (RAXSsd==''||RAXSsd==null) )
		{
		//need to check
dbcm += '***docheck';
if (RA_CtrlWin.RA.dev_check('raxs')) jQuery.cookie('dbcm',dbcm, { path: '/' });
			var RAXSTimeStamp = RA_CtrlWin.RA.RAXS.GetTimeStamp('c');
dbcm += '[ts='+RAXSTimeStamp+'-sd='+RAXSsd+'-Qts'+BFW_QStr['raxsts']+']';
			jQuery.cookie('RAXSsd',RAXSTimeStamp, { path: '/' });
			var xhref = RA_CtrlWin.RA.RAXS.GetURL(RAXSTimeStamp);
			var gourl = RA_CtrlWin.RA.OldLoginRefURL +'/asp/c-ra.asp?m=c&returl='+ encodeURIComponent('http://'+xhref);
			top.location.href = 'http://'+ gourl;
		}
		else if (RAXSsd!=null && RAXSsd.indexOf('c')==0)
		{//back from check
dbcm += '---checked';
if (RA_CtrlWin.RA.dev_check('raxs')) jQuery.cookie('dbcm',dbcm, { path: '/' });
dbcm += '[CHts='+RA_CtrlWin.RA.RAXS.CheckTimeStamp(RAXSsd)+'-sd='+RAXSsd+'-Qts'+BFW_QStr['raxsts']+']';
			if (!RA_CtrlWin.RA.RAXS.CheckTimeStamp(RAXSsd))
			{
dbcm += ':::sdRESET';
				jQuery.cookie('RAXSsd','', { path: '/' });
			}
			else
			{
				if (BFW_QStr['uid'] && BFW_QStr['rau'] && !isNaN(BFW_QStr['uid']))
				{
dbcm += ':::go';
if (RA_CtrlWin.RA.dev_check('raxs')) jQuery.cookie('dbcm',dbcm, { path: '/' });
					jQuery.cookie('RAXSsd',BFW_QStr['uid'], { path: '/' });
					var xhref = RA_CtrlWin.RA.RAXS.GetURL();
					top.location.href = 'http://'+ xhref;
				} 
				else 
				{
dbcm += ':::NO go';
				}
			}
		}
		else if (RAXSsd!=null && RAXSsd.indexOf('i')==0)
		{//back from in
dbcm += '---in';
if (RA_CtrlWin.RA.dev_check('raxs')) jQuery.cookie('dbcm',dbcm, { path: '/' });
dbcm += '[CHts='+RA_CtrlWin.RA.RAXS.CheckTimeStamp(RAXSsd)+'-sd='+RAXSsd+'-Qts'+BFW_QStr['raxsts']+']';
			if (!RA_CtrlWin.RA.RAXS.CheckTimeStamp(RAXSsd))
			{
dbcm += ':::sdRESET';
				jQuery.cookie('RAXSsd','', { path: '/' });
			}
			else
			{
				jQuery.cookie('RAXSsd','', { path: '/' });
				var xhref = RA_CtrlWin.RA.RAXS.GetURL();
dbcm += ':::go';
				top.location.href = 'http://'+ xhref;
			}
		}
		else if (RAXSsd!=null && RAXSsd.indexOf('o')==0)
		{//back from out
dbcm += '---out';
if (RA_CtrlWin.RA.dev_check('raxs')) jQuery.cookie('dbcm',dbcm, { path: '/' });
dbcm += '[CHts='+RA_CtrlWin.RA.RAXS.CheckTimeStamp(RAXSsd)+'-sd='+RAXSsd+'-Qts'+BFW_QStr['raxsts']+']';
			if (!RA_CtrlWin.RA.RAXS.CheckTimeStamp(RAXSsd))
			{
dbcm += ':::sdRESET';
				jQuery.cookie('RAXSsd','', { path: '/' });
			}
			else
			{
				jQuery.cookie('RAXSsd',0, { path: '/' });
				var xhref = RA_CtrlWin.RA.RAXS.GetURL();
dbcm += ':::go';
if (RA_CtrlWin.RA.dev_check('raxs')) jQuery.cookie('dbcm',dbcm, { path: '/' });
				top.location.href = 'http://'+ xhref;
			}
		}
		else if (RAXSsd!=null && !isNaN(RAXSsd)&&RAXSsd!='')
		{//use to set
dbcm += '---use';
dbcm += '[CHts='+RA_CtrlWin.RA.RAXS.CheckTimeStamp(RAXSsd)+'-sd='+RAXSsd+'-Qts'+BFW_QStr['raxsts']+']';
			if (RAXSsd<=0)
			{
dbcm += ':::set zero';
				jQuery.cookie('RAXSsd',0, { path: '/' });
			}
			else 
			{//don't use to set, local already (or still) defined
dbcm += ':::set';
			}
		}
		else
		{
		//static
dbcm += '---static';
dbcm += '[CHts='+RA_CtrlWin.RA.RAXS.CheckTimeStamp(RAXSsd)+'-sd='+RAXSsd+'-Qts'+BFW_QStr['raxsts']+']';
			jQuery.cookie('RAXSsd','', { path: '/' });
		}
if (RA_CtrlWin.RA.dev_check('raxs')) jQuery.cookie('dbcm',dbcm, { path: '/' });

RA_CtrlWin.RA.logTrace( msg );
if (RA_CtrlWin.RA.dev_check('raxs')) RA_CtrlWin.RA.logTrace( '<br/>'+ dbcm );

RA_CtrlWin.RA.dumpTrace();
	}


	
	// *****************************************************************************
	// *****************************************************************************
	// RAUI Object

	RA_CtrlWin.RA.RAUI = {
			divId: 'RAUI_div'
		,	vars: []
	};

	RA_CtrlWin.RA.RAUI.state = {
			stack: []
		,	last: $.cookie('RAUIL')
		,	current: function () {
				if (this.stack.length > 0) {
					return this.stack[this.stack.length-1];
				} else {
					return '';
				}
			}
		,	push: function (s) {
				if (s) {
					if (s!='') {
						if (this.stack.length>0) {
							if (s!=this.stack[this.stack.length-1]) {
								this.stack.push(s);
							}
						} else {
							this.stack.push(s);
						}
					}
				}
				return this.stack[this.stack.length-1];
			}
		,	pop: function () {
				var ret = '';
				if (this.stack.length > 0) {
					this.last = this.current();
					$.cookie('RAUIL',this.last, { path: RA_CtrlWin.RA.CookiePath });
					this.stack.pop();
					if (this.stack.length > 0) {
						ret = this.stack[this.stack.length-1];
					}
				}
				return ret;
			}
		,	next: function (ct) {
				var ret = '';
				var xct = 2;
				if (ct) if (ct>2) xct = ct;
				if (this.stack.length > 1) {
					ret = this.stack[this.stack.length-xct];
				}
				return ret;
			}
	};

	RA_CtrlWin.RA.RAUI.Init = function ( state ) {
		this.state.push( state );
		var box = '' +
			'<table id="RAUI_divTbl" border="0" cellpadding="0" cellspacing="0">' +
			'<tr>' +
			'<td id="RAUI_divTbl_topLeft">&nbsp;</td>' +
			'<td id="RAUI_divTbl_topCenter">&nbsp;</td>' +
			'<td id="RAUI_divTbl_topRight">&nbsp;</td>' +
			'</tr>' +
			'<tr>' +
			'<td id="RAUI_divTbl_midLeft">&nbsp;</td>' +
			'<td id="RAUI_divTbl_midCenter">';
		// iframe variation could go here
		box += '<div id="RA_Page"></div>'
		box += '</td>' +
			'<td id="RAUI_divTbl_midRight">&nbsp;</td>' +
			'</tr>' +
			'<tr>' +
			'<td id="RAUI_divTbl_botLeft">&nbsp;</td>' +
			'<td id="RAUI_divTbl_botCenter">&nbsp;</td>' +
			'<td id="RAUI_divTbl_botRight">&nbsp;</td>' +
			'</tr>' +
			'</table>';
//			RAUI_div.innerHTML = box;
		$('#'+ this.divId ).html(box);
		RA_CtrlWin.RA.RAUI.CreateApp();
		RA_CtrlWin.RA.RAUI.app.Init();
		$('#'+ this.divId ).show();
	}

	RA_CtrlWin.RA.RAUI.Resize = function () {
		if (typeof RA_CtrlWin.RA.RAUI.OnUIResizeCallback === 'function')
		{
			try {
				RA_CtrlWin.RA.RAUI.OnUIResizeCallback();
			} catch(e) {}
		}
	}

	RA_CtrlWin.RA.RAUI.app = {};

	RA_CtrlWin.RA.RAUI.DestroyApp = function () {
		RA_CtrlWin.RA.RAUI.app = {};
	}

	RA_CtrlWin.RA.RAUI.CreateApp = function () {
		RA_CtrlWin.RA.RAUI.app = {};

		RA_CtrlWin.RA.RAUI.app.Resize = function () {
			RA_CtrlWin.RA.RAUI.Resize();
		}

		RA_CtrlWin.RA.RAUI.app.Init = function () {
			switch (RA_CtrlWin.RA.RAUI.state.current()) {
				case 'dologin' :
					RA_CtrlWin.RA.RAUI.state.pop();
					RA_CtrlWin.RA.RAUI.state.push('checkemail');
					var RAUI_timeout_dummy = setTimeout('RA_CtrlWin.RA.RAUI.app.CheckEmailANDLogin()', 5);
				break;
				case 'dologout' :
					RA_CtrlWin.RA.RAUI.state.pop();
					RA_CtrlWin.RA.RAUI.state.push('logout');
					var RAUI_timeout_dummy = setTimeout('RA_CtrlWin.RA.RAUI.app.RAXSLogout()', 5);
				break;
				default :
					RA_CtrlWin.RA.RAUI.app.DrawPage();
			}
		}

		RA_CtrlWin.RA.RAUI.app.DrawPage = function () {
			var str = '';
			str = '';
			switch (RA_CtrlWin.RA.RAUI.state.current()) {
			//  ********************************************************************************
				case 'login' :
					if (RA_CtrlWin.RA.CurrentUser!=null) {
						str += '<p class="RA_formText">';
						str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
						str += '</p>';
						str += '<p class="RA_formText">';
						str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.RAXSLogout(true);">log out</a> - <a href="JavaScript:RA_CtrlWin.RA.RAUI.app.FormCancel()">cancel</a>';
						str += '</p>';
					} else {
						switch (RA_CtrlWin.RA.RAUI.state.next()) {
							case 'quizprompt' :
								str += '<p class="RA_formTitle">Quiz log in</p>';
								str += '<p class="RA_formText">You need to be logged in to take this quiz.</p>';
								break;
							case 'classprompt' :
								str += '<p class="RA_formTitle">Quiz log in</p>';
								str += '<p class="RA_formText">You need to be logged in to take this quiz.</p>';
								break;
							default :
								str += '<p class="RA_formTitle">Log in</p>';
						}
						str += '<p id="RA_formError_login" class="RA_formError">'+ RA_CtrlWin.RA.RAUI.vars['Error'] +'</p>';
						str += '<form name="RA_Login" action="JavaScript:RA_CtrlWin.RA.RAUI.app.goLogin();">';
						str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
						str += '<td width="10" class="RA_formLabel"><nobr>E-mail address:&nbsp;</nobr></td>';
						str += '<td width="10" class="RA_formField"><input maxlength="150" type="text" id="RA_Email" name="email" value="';
						if (RA_CtrlWin.RA.RAUI.vars['Email']) {
						str += RA_CtrlWin.RA.RAUI.vars['Email'];
						} else {
						str += '';
						}
						str += '"/></td>';
						str += '<td>&nbsp;</td>';
						str += '</tr><tr>';
						str += '<td width="10" class="RA_formLabel"><nobr>Password:&nbsp;</nobr></td>';
						str += '<td width="10" class="RA_formField"><input type="password" maxlength="16" id="RA_Password" name="pw" value="" style="width:90px;"/></td>';
						str += '<td>&nbsp;</td>';
						str += '</tr><tr>';
						str += '<td></td><td>';
						str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
						str += '<td width="60" class="RA_button"><a class="BFWj_SimpleButton" href="JavaScript:RA_CtrlWin.RA.RAUI.app.goLogin();">LOG IN</a></td>';
						str += '<td width="60" class="RA_button"><a class="BFWj_SimpleButton" href="JavaScript:RA_CtrlWin.RA.RAUI.app.FormCancel();">CANCEL</a></td>';
						str += '</tr></table>';
						str += '</td>';
						str += '<td>&nbsp;</td>';
						str += '</tr></table>';
						str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
						str += '<p class="RA_formText">';
						str += 'If you have forgotten your password, <a href="JavaScript:RA_CtrlWin.RA.RAUI.app.doEmailPassword();">click here</a> and we\'ll e-mail it to you.';
						str += '</p>';
					}
				break;
				case 'loggingin' :
					str += '<p class="RA_formText">logging in...</p>';
				break;
				case 'loggedin' :
		//			str += '<p class="RA_formTitle">Log in</p>';
					if (RA_CtrlWin.RA.CurrentUser!=null) {
						str += '<p class="RA_formText">logging in...</p>';
						setTimeout('RA_CtrlWin.RA.RAUI.app.FormContinue()', 5);
		/*
						str += '<p class="RA_formText">';
						str += 'You are now logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
						str += '</p>';
						str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
						str += '<td width="70" class="RA_button"><a class="BFWj_SimpleButton" href="JavaScript:RA_CtrlWin.RA.RAUI.app.FormContinue();">CONTINUE</a></td>';
						str += '</tr></table>';
		*/
					} else {
						str += '<p class="RA_formText">';
						str += 'You are not logged in.  <a href="JavaScript:RA_CtrlWin.RA.RAUI.app.ShowLogin(\'login\',true);">Log in here</a>, or <a href="JavaScript:RA_CtrlWin.RA.RAUI.app.FormCancel()">cancel</a>';
						str += '</p>';
					}
				break;
			//  ********************************************************************************
				default :
					str += '<p style="text-align:right;font-size:10px;">';
					str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.FormCancel()">close</a>';
					str += '</p>';
					if (RA_CtrlWin.RA.dev_check('any')) {
						str += '<p class="RA_formText"><b>';
						str += 'RA info';
						str += '</b></p>';
					}
					if (RA_CtrlWin.RA.CurrentUser!=null) {
						str += '<p class="RA_formText">';
						str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
						str += '</p>';
						str += '<p class="RA_formText">';
						str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.RAXSLogout(true);">Log out here</a>';
						str += '</p>';
						if (RA_CtrlWin.RA.dev_check('any')) {
							str += '<p class="RA_formText">';
							str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.ShowLogin(\'checkemail\',true);">Register</a>';
							str += '</p>';
							str += '<p class="RA_formText">';
							str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.Reload(\'quizprompt\',true);">Quiz prompt (instructor email)</a>';
							str += '</p>';
							str += '<p class="RA_formText">';
							str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.Reload(\'quizclassprompt\',true);">Quiz prompt (class)</a>';
							str += '</p>';
							str += '<p class="RA_formText">';
							str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.Reload(\'checkcode\',true);">Enter activation code</a>';
							str += '</p>';
							str += '<p class="RA_formText">';
							str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.Reload(\'cart\',true);">Cart</a>';
							str += '</p>';
						}
					} else {
						str += '<p class="RA_formText">';
						str += 'You are not logged in.  <a href="JavaScript:RA_CtrlWin.RA.RAUI.app.ShowLogin(\'login\',true);">Log in here</a>';
						str += '</p>';
						if (RA_CtrlWin.RA.dev_check('any')) {
							str += '<p class="RA_formText">';
							str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.ShowLogin(\'checkemail\',true);">Register</a>';
							str += '</p>';
							str += '<p class="RA_formText">';
							str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.Reload(\'quizprompt\',true);">Quiz prompt (instructor email)</a>';
							str += '</p>';
							str += '<p class="RA_formText">';
							str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.Reload(\'quizclassprompt\',true);">Quiz prompt (class)</a>';
							str += '</p>';
							str += '<p class="RA_formText">';
							str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.Reload(\'checkcode\',true);">Enter activation code</a>';
							str += '</p>';
							str += '<p class="RA_formText">';
							str += '<a href="JavaScript:RA_CtrlWin.RA.RAUI.app.Reload(\'cart\',true);">Cart</a>';
							str += '</p>';
						}
					}
					str += RA_CtrlWin.RA.DisplayAll();
			}
			if (RA_CtrlWin.RA.dev_check('any')) {
			str += '<p id="tester" style="color:#ccc;font-size:10px;padding:0;margin:0;">testing: '+ RA_CtrlWin.RA.RAUI.state.current() +'</p>';
			}
			str += '<br/><br/><div style="font-size:9pt;"><hr/>Having trouble? <a target="blank" href="http://www.bfwpub.com/newcatalog.aspx?page=support\\techsupport.html">Click here for technical support</a></div>'

			$('#RA_Page').html( str );

		/*
			text: "LOG IN" 
		,   url: "JavaScript:RA_CtrlWin.RA.RAUI.app.goLogin();"
		,   id: "BFWj_SimpleButton_LOGIN"
		,
		 */
			$('.BFWj_SimpleButton').BFWj_SimpleButton({
				extra: 'style="float:none; text-align:center"'
			});

			RA_CtrlWin.RA.RAUI.app.Resize();

		}


		//  ********************************************************************************
		// State Continue/Cancel

		RA_CtrlWin.RA.RAUI.app.FormContinue = function () {
			RA_CtrlWin.RA.RAUI.state.pop();
			if (RA_CtrlWin.RA.RAUI.state.current() == '') {
				RA_CtrlWin.RA.RAUI.app.FormCancel();
			} else {
				if (RA_CtrlWin.RA.RAUI.UseIFrame == true) 
				{
					window.location.reload();
				}
				else
				{
					RA_CtrlWin.RA.RAUI.DestroyApp();
					RA_CtrlWin.RA.RAUI.CreateApp();
					setTimeout('RA_CtrlWin.RA.RAUI.app.Init()',20);
				}
			}
		}

		RA_CtrlWin.RA.RAUI.app.FormCancel = function () {
			for (var key in RA_CtrlWin.RA.RAUI.vars)
			{
				delete RA_CtrlWin.RA.RAUI.vars[key];
			}
			RA_CtrlWin.RA.RAUI.close();
			RA_CtrlWin.RA.RAUI.state.last = 'formcancel';
		}

		// *****************************************************************************
		// State action methods

		// -----------------------------------------------------------------------------
		// Login

		RA_CtrlWin.RA.RAUI.app.goLogin = function () {
			RA_CtrlWin.RA.RAUI.vars['Email'] = RA_CtrlWin.RA.RAUI.app.RAWSCleanInput($('#RA_Email')[0].value);
			RA_CtrlWin.RA.RAUI.vars['Email'] = RA_CtrlWin.RA.RAUI.vars['Email'].toLowerCase();
			RA_CtrlWin.RA.RAUI.vars['Password'] = RA_CtrlWin.RA.RAUI.app.RAWSCleanInput($('#RA_Password')[0].value);
			RA_CtrlWin.RA.RAUI.vars['RememberMe'] = 0;
			if (!RA_CtrlWin.RA.RAUI.app.IsValidEmail(RA_CtrlWin.RA.RAUI.vars['Email'])) {
				RA_CtrlWin.RA.RAUI.vars['Error'] = 'Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
				RA_CtrlWin.RA.RAUI.app.DrawPage();
				$('#RA_formError_login').show('','');
				$('#RA_formError_login').html(RA_CtrlWin.RA.RAUI.vars['Error']);
				RA_CtrlWin.RA.RAUI.app.Resize();
				return;
			}
		/*
			if ($('#RA_RememberMe')[0]) {
				if ($('#RA_RememberMe')[0].checked) {
					RA_CtrlWin.RA.RAUI.vars['RememberMe'] = 1;
				}
			}
		*/
			RA_CtrlWin.RA.RAUI.app.Login();
		}

		RA_CtrlWin.RA.RAUI.app.Login = function () {
		//alert(RA_CtrlWin.RA.RAUI.vars['Email'] +' / '+ RA_CtrlWin.RA.RAUI.vars['Password'])
			RA_CtrlWin.RA.RAUI.state.push('loggingin');
			RA_CtrlWin.RA.RAUI.app.DrawPage();
			RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAUI.app.Login_0_go, RA_CtrlWin.RA.RAWS.Ravell_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAUI.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAUI.vars['Password'] ) );
		}
		RA_CtrlWin.RA.RAUI.app.Login_0_go = function () {
			RA_CtrlWin.RA.RAUI.app.Login_0();
		}
		RA_CtrlWin.RA.RAUI.app.Login_0 = function () {
			RA_CtrlWin.RA.RAUI.state.pop();
			if ( RA_CtrlWin.RA.RAWS.error=='' ) {
				RA_CtrlWin.RA.RAUI.app.RavellLogin_continue(RA_CtrlWin.RA.RAUI.vars['Email'], RA_CtrlWin.RA.RAUI.vars['Password']);
			} else {
				if ( RA_CtrlWin.RA.RAWS.error.indexOf('Login failed, please try again.') > -1 ) {
			//alert( 'error 1 ('+ RA_CtrlWin.RA.RAWS.iUserID +') --- '+ RA_CtrlWin.RA.RAWS.error );
					RA_CtrlWin.RA.RAUI.vars['Error'] = RA_CtrlWin.RA.RAWS.error + ' Your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
					RA_CtrlWin.RA.RAUI.app.DrawPage();
					$('#RA_formError_login').show('','');
					$('#RA_formError_login').html(RA_CtrlWin.RA.RAUI.vars['Error']);
					RA_CtrlWin.RA.RAUI.app.Resize();
				} else {
			//alert( 'error 0 ('+ RA_CtrlWin.RA.RAWS.iUserID +') --- '+ RA_CtrlWin.RA.RAWS.error );
			//prompt( 'error 0 ('+ RA_CtrlWin.RA.RAWS.iUserID +') --- ', RA_CtrlWin.RA.RAWS.error );
					RA_CtrlWin.RA.RAUI.vars['Error'] = RA_CtrlWin.RA.RAWS.error;
					RA_CtrlWin.RA.RAUI.app.DrawPage();
					$('#RA_formError_login').show('','');
					$('#RA_formError_login').html(RA_CtrlWin.RA.RAUI.vars['Error']);
					RA_CtrlWin.RA.RAUI.app.Resize();
				}
			}
		}

		// -----------------------------------------------------------------------------
		// Logout

		RA_CtrlWin.RA.RAUI.app.goLogout = function () {
			RA_CtrlWin.RA.RAUI.app.RAXSLogout();
		}

		RA_CtrlWin.RA.RAUI.app.RAXSLogout = function (nopop) {
			if (!nopop) RA_CtrlWin.RA.RAUI.state.pop();
			RA_CtrlWin.RA.RAUI.state.push('loggingout');
			RA_CtrlWin.RA.RAUI.app.DrawPage();
			RA_CtrlWin.RA.RAUI.state.pop();
			RA_CtrlWin.RA.RAUI.app.RavellLogout ( 'loggedout', nopop, 0, true );
		}


		// *****************************************************************************
		// Ravell methods

		RA_CtrlWin.RA.RAUI.app.RavellLogin_continue = function ( uname, upw ) {
		//alert( 'RA_CtrlWin.RA.RAUI.app.RAXSLogin_continue: '+ uname +' - '+ upw );
			var target = RA_CtrlWin.RA.SecureUrl;
			if (!target)
			{
				target = 'http://no.target';
			}
			$('#IDPLogin').remove();
			//
			// /account/login?id=emlogin
			var action = 'idp.px.bfwpub.com:8080/nidp/app/login?id=emlogin';
			if (window.location.host.indexOf('dev.') == 0)
			{
				action = 'dev-' + action;
			}
			else if (window.location.host.indexOf('qa.') == 0)
			{
				action = 'qa-' + action;
			}
			action = 'http://' + action;
			alert(action);
			var htmlForm = '<form style="display:none;" id="IDPLogin" name="IDPLogin" method="POST" action="'+ action +'" enctype="application/x-www-form-urlencoded">'+
				'<input id="option" name="option" type="hidden" value="credential">'+
				'<input id="target" name="target" type="hidden" value="'+ target +'"></input>'+
				'<input type="hidden" style="display:block;" id="Ecom_User_ID" name="Ecom_User_ID" size="30" maxlength="150" value="'+uname+'"/>'+
				'<input type="password" style="display:block;" id="Ecom_Password" name="Ecom_Password" size="30" maxlength="150" value="'+upw+'"/>'+
				'<input type="submit" value="go"/>'+
				'</form>';
			$('#RA_Page').append(htmlForm);
			//alert($('#IDPLogin').attr('action'));
			$('#IDPLogin').submit();
		}

		RA_CtrlWin.RA.RAUI.app.RavellLogout = function () {
			RA_CtrlWin.RA.ClearUser();
			RA_CtrlWin.$.cookie('RaUData','', { path: '/' });
			var target = BFW_QStr['target'];
			if (!target)
			{
				target = 'http://no.target';
			}
			$('#IDPLogin').remove();
			var action = 'http://sb.bfwpub.com/nesp/app/plogout?target='+ encodeURIComponent(target);
			var winhost = window.location.host;
		//	if ( winhost.indexOf('localhost:')==0 )
		//	{
		//		action = 'http://dev.bfwpub.com/AGLogout';
		//	}
			var htmlForm = '<form style="display:none;" id="IDPLogin" name="IDPLogin" method="POST" action="'+ action +'" enctype="application/x-www-form-urlencoded">'+
				'<input id="target" name="target" type="hidden" value="'+target+'"></input>'+
				'</form>';
			$('#RA_Page').append(htmlForm);
			if (RA_CtrlWin.RA.dev_check('any')) {
				//alert( $('#IDPLogin').attr('action') +' -- '+ $('#target').val() );
				$('#IDPLogin').append('<input type="submit" value="go"/>');
				$('#IDPLogin').show();
			} else {
				$('#IDPLogin').submit();
			}
		}

		// *****************************************************************************
		// Utility functions

		RA_CtrlWin.RA.RAUI.app.RAWSCleanInput = function (val, type) {
			switch (type) {
			case '' :
				val = RA_CtrlWin.RA.RAUI.app.trim(val);
				break;
			default :
				val = val.replace('<','').replace('>','');
				val = RA_CtrlWin.RA.RAUI.app.trim(val);
			}
			return val;
		}

		RA_CtrlWin.RA.RAUI.app.IsValidEmail = function (email) {
			try {
				email = RA_CtrlWin.RA.RAUI.app.trim(email);
				var valid1 = email.indexOf('.') > -1;
				var valid2 = email.indexOf('..') == -1;
				var valid3 = email.indexOf('@') > -1;
				var valid4 = email.indexOf('@') == email.lastIndexOf('@');
				var rexpStr = '[\!#$%^\*< >\(\)\+\\\\\&\\s]';
				var rexp = new RegExp( rexpStr,'gim');
				var valid5 = !rexp.test(email);
				var valid6 = true;
				valid6 = (email.indexOf("\'")>-1) ? false : true;
				var valid = valid1 && valid2 && valid3 && valid4 && valid5 && valid6;
				return valid;
			} catch(e) {
				return false;
			}
		}

		RA_CtrlWin.RA.RAUI.app.trim = function (str, chars) {
			return RA_CtrlWin.RA.RAUI.app.ltrim(RA_CtrlWin.RA.RAUI.app.rtrim(str, chars), chars);
		}

		RA_CtrlWin.RA.RAUI.app.ltrim = function (str, chars) {
			chars = chars || "\\s";
			return str.replace(new RegExp("^[" + chars + "]+", "g"), "");
		}

		RA_CtrlWin.RA.RAUI.app.rtrim = function (str, chars) {
			chars = chars || "\\s";
			return str.replace(new RegExp("[" + chars + "]+$", "g"), "");
		}


	}

	// *****************************************************************************
	// *****************************************************************************
	// RAWS

	RA_CtrlWin.RA.RAWS = {
			WaitFor_timeout: null
		,	WaitFor_interval: null
		,	WaitFor_go: function(){}
		,	WaitFor_timeout_ct: 30000
		,	WaitFor_timedout: false
		,	WaitFor_Ready: false
		,	fn: null
	};

	RA_CtrlWin.RA.RAWS.Send = function (gofn,RAWSfn,RAWSargs) {
	//alert('RA.RAWS.Send');
		if (!(typeof RAWSfn === 'function')) return;
		if (!(typeof gofn === 'function'))
		{
			gofn = function(){}
		}
		this.WaitFor(gofn);
		this.fn = RAWSfn;
		try {
			this.fn(RAWSargs);
		} catch(e) {
			RA_CtrlWin.RAWSCallError = new Error( 'RAWS call error :: '+ e.name +' --- '+ e.message +'\n {{{ \n'+ new String(this.fn) +'\n }}} \n' );
			RA_CtrlWin.RAWSCallError.name = 'RAWSCallError';
			this.WaitFor_error(RA_CtrlWin.RAWSCallError);
		}
	}

	RA_CtrlWin.RA.RAWS.WaitFor = function (fn) {
	//alert('RA.RAWS.WaitFor');
		this.WaitFor_Ready = false;
		this.WaitFor_go = fn;
		this.WaitFor_interval = window.setInterval('RA_CtrlWin.RA.RAWS.WaitFor_check()', 2000);
		this.WaitFor_timedout = false;
		this.WaitFor_timeout = window.setTimeout('RA_CtrlWin.RA.RAWS.WaitFor_timedout=true',this.WaitFor_timeout_ct);
	}

	RA_CtrlWin.RA.RAWS.WaitFor_check = function () {
	//alert('RA.RAWS.RAWS.WaitFor_check --- '+ this.WaitFor_timedout +' --- '+ this.WaitFor_Ready);
		if (this.WaitFor_timedout) {
			this.WaitFor_error(RA_CtrlWin.RAWSWaitForTimeOutError);
		} else if (this.WaitFor_Ready) {
			this.WaitFor_clear();
		}
	}

	RA_CtrlWin.RA.RAWS.WaitFor_clear = function () {
	//alert('RA.RAWS.RAWS.WaitFor_clear');
		this.WaitFor_interval = window.clearInterval(this.WaitFor_interval);
		this.WaitFor_timeout = window.clearTimeout(this.WaitFor_timeout);
		try { 
			setTimeout(RA_CtrlWin.RA.RAWS.WaitFor_go,10);
		} catch(e) { 
			alert('Error in site custom RA.RAWS.WaitFor_go function: '+ e.name +' --- '+ e.message); 
			this.WaitFor_error(e);
		}
	}

	RA_CtrlWin.RA.RAWS.WaitFor_error = function (e) {
	//alert('RA.RAWS.RAWS.WaitFor_error');
		this.WaitFor_interval = window.clearInterval(this.WaitFor_interval);
		this.WaitFor_timeout = window.clearTimeout(this.WaitFor_timeout);
		var htmlMsg = '';
		switch (e.name) {
		case 'RAWSCallError' :
			htmlMsg = 'Unable to process your request. An error log is being sent to our system administrators and we\'ll be looking into immediately. Please try again in a few minutes.'
			if (RA_CtrlWin.RA.dev_check('any'))
			{
				htmlMsg += '\n\n'+ e.name +' - '+ e.lineNumber +' - '+ e.message;
			}
			break;
		case 'RAWSWaitForTimeOutError' :
			htmlMsg = 'The server is busy and unable to process your request at this time. Please try again in a few minutes.'
			break;
		}
		BFW_Errors.add(e,1000);
		try {
			this.WaitFor_error_go(1000,htmlMsg); 
		} catch(e) { 
			alert('Error in site custom RA.RAWS.WaitFor_error_go function: '+ e.name +' --- '+ e.message); 
			BFW_Errors.add(e,1);
		} finally {
			BFW_Errors.LogErrors();
		}
	}

	RA_CtrlWin.RA.RAWS.WaitFor_go = function () {};

	RA_CtrlWin.RA.RAWS.WaitFor_error_go = function (severity,htmlMsg) {if (severity>=100) alert(htmlMsg);};

	RA_CtrlWin.RA.RAWS.UpdateServer = function () {
		if (RA_CtrlWin.RA.ProxyType!='ASP.NET')
		{
			return;
		}
		szAction = 'http://ra.bfwpub.com/RAg.Net/UpdateServer';
//            RAgLocal.Proxy( szUrl , strEnvelope ,  RAgLocalProxy_Process, RAgLocalProxy_HttpResp_Error);   
/*
POST /RAg.Net.Example/RAg/RAgLocal.asmx HTTP/1.1
Host: localhost
Content-Type: text/xml; charset=utf-8
Content-Length: length
SOAPAction: "http://tempuri.org/Proxy"

'*/
		var sendData = ''+
'<?xml version="1.0" encoding="utf-8"?>'+
'<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">'+
'  <soap:Body>'+
'    <SetServerSession xmlns="http://ra.bfwpub.com/RAg.Net/">'+
'    </SetServerSession>'+
'  </soap:Body>'+
'</soap:Envelope>';
		RA_CtrlWin.RA.RAWS.Http = $.ajax({
			type: "POST",
			url: RA_CtrlWin.RA.LocalProxyURL+'?op=UpdateServer',
			dataType: "xml",
			data: sendData,
			processData: false,
			//contentType: 'application/xml; charset=utf-8', 
			contentType: 'text/xml; charset=utf-8', 
			beforeSend: function(req) {
				req.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
				req.setRequestHeader('SOAPAction', szAction) ;
			},
			success: function(data,textStatus){
	var errEl;
	var gotIt = '';
	errEl = data.getElementsByTagName('UpdateServerResult')[0];
	for (var i=0; i<errEl.childNodes.length; i++) {
		if (errEl.childNodes[i].nodeType==3) {
			gotIt += errEl.childNodes[i].nodeValue;
		}
	}
if (RA_CtrlWin.RA.dev_check('.net')) prompt('UpdateServerResult',gotIt);
			},
			error: function(XMLHttpRequest, textStatus, errorThrown){
	var msg = '';
	msg += RA_CtrlWin.RA.RAWS.Http.status +' ---- ';
	msg += XMLHttpRequest.status +' ---- ';
	msg += XMLHttpRequest.statusText +' ---- ';
	msg += XMLHttpRequest.responseText +' ---- ';
	msg += XMLHttpRequest.responseXML +' ---- ';
	msg += textStatus.status +' ---- ';
	msg += errorThrown +' ---- ';
if (RA_CtrlWin.RA.dev_check('.net')) prompt('UpdateServerResult error: ', msg);
			}
		});
}

// *********************************************************
// *********************************************************
// *********************************************************
// *********************************************************

RA_CtrlWin.RA.RAWS.HTTPREQUEST_timeOut = null;
RA_CtrlWin.RA.RAWS.HTTPREQUEST_timedOut = false;
RA_CtrlWin.RA.RAWS.interval_ct = 100;
RA_CtrlWin.RA.RAWS.WaitFor_Ready = true;
RA_CtrlWin.RA.RAWS.WaitFor_interval = null;
RA_CtrlWin.RA.RAWS.requestStatus_t;
RA_CtrlWin.RA.RAWS.error = '';
RA_CtrlWin.RA.RAWS.iUserID = null;
RA_CtrlWin.RA.RAWS.iClassUsingUserID = null;
RA_CtrlWin.RA.RAWS.iClassUsingClassID = null;
RA_CtrlWin.RA.RAWS.sUserName = null;
RA_CtrlWin.RA.RAWS.sPassword = null;
RA_CtrlWin.RA.RAWS.sFirstName = null;
RA_CtrlWin.RA.RAWS.sLastName = null;
RA_CtrlWin.RA.RAWS.sPasswordHint = null;
RA_CtrlWin.RA.RAWS.sMailPreference = null;
RA_CtrlWin.RA.RAWS.sOptInEmail = null;

RA_CtrlWin.RA.RAWS.XSSessionGUID = null;

RA_CtrlWin.RA.RAWS.iUserInstructorStatus = 0;

RA_CtrlWin.RA.RAWS.udtClassInfo = new Array();
RA_CtrlWin.RA.RAWS.udtPackages = new Object();
RA_CtrlWin.RA.RAWS.udtSites = new Object();
RA_CtrlWin.RA.RAWS.udtSchools = new Array();

RA_CtrlWin.RA.RAWS.sBaseUrl = null;
RA_CtrlWin.RA.RAWS.sSiteDesc = null;
RA_CtrlWin.RA.RAWS.iSiteID = null;
RA_CtrlWin.RA.RAWS.sConfirmation = null;

RA_CtrlWin.RA.RAWS.iPackageID = null;

RA_CtrlWin.RA.RAWS.arrSiteIDs = new Array();

RA_CtrlWin.RA.RAWS.sInstructorEmail = null;
RA_CtrlWin.RA.RAWS.sIPAddr = null;
RA_CtrlWin.RA.RAWS.sInstEmail = null;
RA_CtrlWin.RA.RAWS.iLevelOfAccess = null;


RA_CtrlWin.RA.RAWS.ClearVars = function () {
	RA_CtrlWin.RA.RAWS.error = '';
	RA_CtrlWin.RA.RAWS.iUserID = '';
	RA_CtrlWin.RA.RAWS.sUserName = '';
	RA_CtrlWin.RA.RAWS.sPassword = '';
	RA_CtrlWin.RA.RAWS.sFirstName = '';
	RA_CtrlWin.RA.RAWS.sLastName = '';
	RA_CtrlWin.RA.RAWS.sPasswordHint = '';
	RA_CtrlWin.RA.RAWS.sMailPreference = '';
	RA_CtrlWin.RA.RAWS.sOptInEmail = '';
	
	RA_CtrlWin.RA.RAWS.XSSessionGUID = '';

	RA_CtrlWin.RA.RAWS.udtClassInfo = new Array();
	RA_CtrlWin.RA.RAWS.udtPackages = new Object();
	RA_CtrlWin.RA.RAWS.udtSites = new Object();
	RA_CtrlWin.RA.RAWS.udtSchools = new Array();
/*
	RA_CtrlWin.RA.RAWS.iClassID = '';
	RA_CtrlWin.RA.RAWS.iCreatorID = '';
	RA_CtrlWin.RA.RAWS.sClassName = '';
	RA_CtrlWin.RA.RAWS.sClassDesc = '';
	RA_CtrlWin.RA.RAWS.sClassCode = '';
	RA_CtrlWin.RA.RAWS.dtExprn = '';
	RA_CtrlWin.RA.RAWS.iUserID = '';
	RA_CtrlWin.RA.RAWS.bClassAccessRevoked = '';
	RA_CtrlWin.RA.RAWS.dtLastLogin = '';
	RA_CtrlWin.RA.RAWS.dtStartDate = '';
	RA_CtrlWin.RA.RAWS.dtEndDate = '';
	RA_CtrlWin.RA.RAWS.bEmailScores = '';
	RA_CtrlWin.RA.RAWS.iRecordStatus = '';
*/
	RA_CtrlWin.RA.RAWS.sBaseUrl = '';
	RA_CtrlWin.RA.RAWS.sSiteDesc = '';
	RA_CtrlWin.RA.RAWS.iSiteID = '';
	RA_CtrlWin.RA.RAWS.sConfirmation = '';

	RA_CtrlWin.RA.RAWS.sInstructorEmail = '';
	RA_CtrlWin.RA.RAWS.sIPAddr = '';
	RA_CtrlWin.RA.RAWS.iLevelOfAccess = '';
}

RA_CtrlWin.RA.RAWS.HTTPREQUEST_doTimeOut = function () {
	alert('RAWS timed out');
//	alert(RA_CtrlWin.RA.RAWS.Http.status);
	RA_CtrlWin.RA.RAWS.HTTPREQUEST_timedOut = true;
}

RA_CtrlWin.RA.RAWS.WaitFor = function (fn) {
	RA_CtrlWin.RA.RAWS.WaitFor_Ready = false;
	RA_CtrlWin.RA.RAWS.WaitFor_interval = window.setInterval('RA_CtrlWin.RA.RAWS.WaitFor_check('+fn+')',RA_CtrlWin.RA.RAWS.interval_ct);
}
RA_CtrlWin.RA.RAWS.WaitFor_check = function (fn) {
//alert(RA_CtrlWin.RA.RAWS.WaitFor_Ready +' - '+ RA_CtrlWin.RA.RAWS.HTTPREQUEST_timedOut);

	if (RA_CtrlWin.RA.RAWS.WaitFor_Ready) {
		RA_CtrlWin.RA.RAWS.WaitFor_clear(fn);
	}
}
RA_CtrlWin.RA.RAWS.WaitFor_clear = function (fn) {
	RA_CtrlWin.RA.RAWS.WaitFor_interval = window.clearInterval(RA_CtrlWin.RA.RAWS.WaitFor_interval);
	RA_CtrlWin.RA.RAWS.WaitFor_Ready = true;
	RA_CtrlWin.RA.RAWS.WaitFor_Ready_Go(fn);
}
RA_CtrlWin.RA.RAWS.WaitFor_Ready_Go = function (fn) {
//alert('RA_CtrlWin.RA.RAWS.WaitFor_Ready_Go');
	setTimeout(fn, 100);
}

/* ------------------------------------------------------------ */
RA_CtrlWin.RA.RAWS.Http = null;
RA_CtrlWin.RA.RAWS.XML = null;


RA_CtrlWin.RA.RAWS.NO_PROCESSOR = function () {
	alert('No processor switch for function \''+ RA_CtrlWin.RA.RAWS._function +'\'');
}
RA_CtrlWin.RA.RAWS._ERROR = function () {
	if (RA_CtrlWin.RA.RAWS.Http.status != 0 ) {
//		alert('Problem retrieving RA data: '+ RA_CtrlWin.RA.RAWS.Http.status);
//		top.location.reload();
	} else {
//		alert('YO');
	}
}

RA_CtrlWin.RA.RAWS.NET_cbk;
RA_CtrlWin.RA.RAWS.HTTPREQUEST = function (_function, strEnvelope) {
//alert('RA_CtrlWin.RA.RAWS.HTTPREQUEST  :::::: '+ RA_CtrlWin.RA.RAWS._function);
	RA_CtrlWin.RA.RAWS._function = _function;

	var cbk;

	switch (RA_CtrlWin.RA.RAWS._function) {
	case 'IL_CheckInstructorAccess' :
		cbk = IL_CheckInstructorAccess_PROCESS;
	break;
	case 'RAXS_WS':
		cbk = RAXS_WS_PROCESS;
	break;
	case '1_FetchSiteID':
		cbk = RAWS1_FetchSiteID_PROCESS;
	break;
	case 'RA_CtrlWin.RA.RAWS.Ravell_UserAuthentication':
		cbk = RA_CtrlWin.RA.RAWS.Ravell_UserAuthentication_PROCESS;
	break;
	case '1_UserAuthentication':
		cbk = RAWS1_UserAuthentication_PROCESS;
	break;
	case '3_UserAuthentication':
		cbk = RAWS3_UserAuthentication_PROCESS;
	break;
	case '3_StudentRegistration':
		cbk = RAWS3_StudentRegistration_PROCESS;
	break;
	case '3_UpdateProfile':
		cbk = RAWS3_UpdateProfile_PROCESS;
	break;
	case '3_CheckDefaultClass' :
		cbk = RAWS3_CheckDefaultClass_PROCESS;
	break;
	case '3_GetDefaultClass' :
		cbk = RAWS3_GetDefaultClass_PROCESS;
	break;
	case '1_SiteLogin':
		cbk = RAWS1_SiteLogin_PROCESS;
	break;
	case '1_GetClass':
		cbk = RAWS1_GetClass_PROCESS;
	break;
	case '1_UpdateProfile':
		cbk = RAWS1_UpdateProfile_PROCESS;
	break;
	case '1_UpdatePassword':
		cbk = RAWS1_UpdatePassword_PROCESS;
	break;
	case '1_GetUsernamePasswordHint':
		cbk = RAWS1_GetUsernamePasswordHint_PROCESS;
	break;
	case '1_StudentRegistration':
		cbk = RAWS1_StudentRegistration_PROCESS;
	break;
	case '1_EnterActivationCode':
		cbk = RAWS1_EnterActivationCode_PROCESS;
	break;
	case '1_JoinClass':
		cbk = RAWS1_JoinClass_PROCESS;
	break;
	case '1_UserProfile':
		cbk = RAWS1_UserProfile_PROCESS;
	break;
	case '1_UpdateSiteLogin':
		cbk = RAWS1_UpdateSiteLogin_PROCESS;
	break;
	case '1_UpdateSiteProfile':
		cbk = RAWS1_UpdateSiteProfile_PROCESS;
	break;
	case '2_FetchSiteID':
		cbk = RAWS2_FetchSiteID_PROCESS;
	break;
	case '2_EmailLogins':
		cbk = RAWS1HCL_EmailLogins_PROCESS;
	break;
	case '2_GetUsernamePasswordHint':
		cbk = RAWS2_GetUsernamePasswordHint_PROCESS;
	break;
	case '2_CheckUsername':
		cbk = RAWS2_CheckUsername_PROCESS;
	break;
	case '3_CheckUsername':
		cbk = RAWS3_CheckUsername_PROCESS;
	break;
	case '3_GetPackages':
		cbk = RAWS3_GetPackages_PROCESS;
	break;
	case '3_CheckActivationCode_packages':
		cbk = RAWS3_CheckActivationCode_packages_PROCESS;
	break;
	case '3_CheckActivationCode':
		cbk = RAWS3_CheckActivationCode_PROCESS;
	break;
	case '3_AssignUserCodes':
		cbk = RAWS3_AssignUserCodes_PROCESS;
	break;
	case '3_JoinClass':
		cbk = RAWS3_JoinClass_PROCESS;
	break;
	case '3_JoinUsertoClass':
		cbk = RAWS3_JoinUsertoClass_PROCESS;
	break;
	case '3_EmailLogins':
		cbk = RAWS3_EmailLogins_PROCESS;
	break;
	case '3_GetSiteFromBaseURL':
		cbk = RAWS3_GetSiteFromBaseURL_PROCESS;
	break;
	case '3_GetSiteFromBaseURL_WithProducts':
		cbk = RAWS3_GetSiteFromBaseURL_WithProducts_PROCESS;
	break;
	case '3_RAXSSession':
		cbk = RAWS3_RAXSSession_PROCESS;
	break;
	case '3_GetSitesFromSiteIDs':
		cbk = RAWS3_GetSitesFromSiteIDs_PROCESS;
	break;
	case '3_GetOnyxSchoolsByZip':
		cbk = RAWS3_GetOnyxSchoolsByZip_PROCESS;
	break;
	default:
		cbk = RA_CtrlWin.RA.RAWS.NO_PROCESSOR;
	}

	var szAction;
	switch (RA_CtrlWin.RA.RAWS._function) {
		case 'IL_CheckInstructorAccess' :
	szAction = 'http://tempuri.org/webservices/'+ RA_CtrlWin.RA.RAWS._function;
			break;
		default :
	szAction = 'http://tempuri.org/'+ RA_CtrlWin.RA.RAWS._function;
	}

	var szUrl;
	switch (RA_CtrlWin.RA.ProxyType) {
		case 'ASP.NET' :
	szUrl = '/BFWglobal/ws/proxy/v1.1/proxy.asp';
			break;
		case 'asp' :
	szUrl = '/BFWglobal/ws/proxy/v1.1/proxy.asp';
			break;
		default :
	szUrl = '/BFWglobal/ws/proxy/v1.1/proxy.asp';
	}
	switch (RA_CtrlWin.RA.RAWS._function) {
		case 'IL_CheckInstructorAccess' :
	szUrl += '?wsID=IL_CheckInstructorAccess';
			break;
		case 'RAXS_WS' :
	szUrl += '?wsID=RAXS_WS';
			break;
		case '2_CheckUsername' :
	szUrl += '?wsID=RAWS2&wsF=2_CheckUsername';
			break;
		case '3_StudentRegistration' :
	szUrl += '?wsID=RAWS3&wsF=3_StudentRegistration';
			break;
		case '3_UpdateProfile' :
	szUrl += '?wsID=RAWS3&wsF=3_UpdateProfile';
if (RA_CtrlWin.RA.dev_check('ET')) prompt('szUrl',szUrl);
			break;
		case '3_CheckUsername' :
	szUrl += '?wsID=RAWS3&wsF=3_CheckUsername';
			break;
		case 'RA_CtrlWin.RA.RAWS.Ravell_UserAuthentication' :
	szUrl += '?wsID=Ravell&wsF=UserAuthentication&'+strEnvelope;
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<RavellAuthentication xmlns=\"http://tempuri.org/\"/>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
'';
			break;
		case '3_UserAuthentication' :
	szUrl += '?wsID=RAWS3&wsF=3_UserAuthentication';
			break;
		case '3_CheckDefaultClass' :
	szUrl += '?wsID=RAWS3&wsF=3_CheckDefaultClass';
			break;
		case '3_GetDefaultClass' :
	szUrl += '?wsID=RAWS3&wsF=3_GetDefaultClass';
			break;
		case '2_FetchSiteID' :
	szUrl += '?wsID=RAWS2&wsF=2_FetchSiteID';
			break;
		case '2_GetUsernamePasswordHint' :
	szUrl += '?wsID=RAWS2&wsF=GetUsernamePasswordHint';
			break;
		case '2_EmailLogins' :
	szUrl += '?wsID=RAWS1HCL&wsF=EmailLogins';
			break;
		case '3_EmailLogins' :
	szUrl += '?wsID=RAWS3&wsF=3_EmailLogins';
			break;
		case '3_GetPackages' :
	szUrl += '?wsID=RAWS3_GetPackages';
			break;
		case '3_CheckActivationCode_packages' :
	szUrl += '?wsID=RAWS3_CheckActivationCode_packages';
			break;
		case '3_CheckActivationCode' :
	szUrl += '?wsID=RAWS3_CheckActivationCode';
			break;
		case '3_AssignUserCodes' :
	szUrl += '?wsID=RAWS3_AssignUserCodes';
			break;
		case '3_JoinClass' :
	szUrl += '?wsID=RAWS3_JoinClass';
//	szUrl += '?wsID=RAWS3&wsF=3_JoinClass';
			break;
		case '3_JoinUsertoClass' :
	szUrl += '?wsID=RAWS3&wsF=3_JoinUsertoClass';
			break;
		case '3_GetSiteFromBaseURL' :
	szUrl += '?wsID=RAWS3_GetSiteFromBaseURL';
			break;
		case '3_GetSiteFromBaseURL_WithProducts' :
	szUrl += '?wsID=RAWS3_GetSiteFromBaseURL_WithProducts';
			break;
		case '3_RAXSSession' :
	szUrl += '?wsID=RAWS3_RAXSSession';
			break;
		case '3_GetSitesFromSiteIDs' :
	szUrl += '?wsID=RAWS3_GetSitesFromSiteIDs';
			break;
		case '3_GetOnyxSchoolsByZip' :
	szUrl += '?wsID=RAWS3_GetOnyxSchoolsByZip';
			break;
		default :
	szUrl += '?wsID=RAWS1&wsF='+ RA_CtrlWin.RA.RAWS._function;
	}

	var winhost = window.location.host;
	if (winhost.indexOf('localhost')==0 && RA_CtrlWin.RA.ProxyType=='ASP.NET' && RA_CtrlWin.RA.LocalProxyURL!='')
	{
		RA_CtrlWin.RA.RAWS.NET_cbk = cbk;
		szUrl = window.location.protocol +'//'+ window.location.hostname + szUrl.replace(/&/g,'&amp;');
//alert(szUrl);
		szAction = 'http://ra.bfwpub.com/RAg.Net/Proxy';
//            RAgLocal.Proxy( szUrl , strEnvelope ,  RAgLocalProxy_Process, RAgLocalProxy_HttpResp_Error);   
/*
POST /RAg.Net.Example/RAg/RAgLocal.asmx HTTP/1.1
Host: localhost
Content-Type: text/xml; charset=utf-8
Content-Length: length
SOAPAction: "http://tempuri.org/Proxy"

'*/
		var sendData = ''+
'<?xml version="1.0" encoding="utf-8"?>'+
'<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">'+
'  <soap:Body>'+
'    <Proxy xmlns="http://ra.bfwpub.com/RAg.Net/">'+
'      <szURL>'+szUrl+'</szURL>'+
'      <PostData><![CDATA['+strEnvelope+']]></PostData>'+
'    </Proxy>'+
'  </soap:Body>'+
'</soap:Envelope>';
if (RA_CtrlWin.RA.dev_check('.net'))
{
var msg = '';
msg += 'Url = '+szUrl +' --- strEnvelope = '+ strEnvelope +' --- '+ sendData;
msg += ' --- '+ sendData;
prompt('sending :', msg);
}
		RA_CtrlWin.RA.RAWS.Http = jQuery.ajax({
			type: "POST",
			url: RA_CtrlWin.RA.LocalProxyURL+'?op=Proxy',
			dataType: "xml",
			data: sendData,
			processData: false,
			//contentType: 'application/xml; charset=utf-8', 
			contentType: 'text/xml; charset=utf-8', 
			beforeSend: function(req) {
				req.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
				req.setRequestHeader('SOAPAction', szAction) ;
			},
			success: RAgLocalProxy_Process,
			error: RAgLocalProxy_HttpResp_Error
		});

	}
	else
	{
		RA_CtrlWin.RA.RAWS.Http = jQuery.ajax({
			type: 'POST',
			url: szUrl,
			dataType: 'xml',
			data: strEnvelope,
			processData: false,
			beforeSend: function(req) {
				req.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
				req.setRequestHeader('SOAPAction', szAction) ;
			},
			success: function(data,textStatus) {
				RA_CtrlWin.RA.RAWS.XML = data;
				try {
							cbk();
				} catch(e) {
alert('cbk ERROR :: '+ e.lineNumber +' - '+ e.message);
				}
			},
			error: RA_CtrlWin.RA.RAWS._ERROR
		});
	}

}

//function RAgLocalProxy_Process (result) {
RA_CtrlWin.RA.RAWS.RAgLocalProxy_Process = function (data,textStatus) {
	var cbk = RA_CtrlWin.RA.RAWS.NET_cbk;
	RA_CtrlWin.RA.RAWS.NET_cbk = null;
	var errEl;
	var gotIt = '';
	errEl = data.getElementsByTagName('ProxyResult')[0];
	for (var i=0; i<errEl.childNodes.length; i++) {
		if (errEl.childNodes[i].nodeType==3) {
			gotIt += errEl.childNodes[i].nodeValue;
		}
	}
if (RA_CtrlWin.RA.dev_check('.net')) {
prompt('RAgLocalProxy_Process ProxyResult',gotIt);
var msg = '';
msg += data;
msg += ' ---- ';
	window.scrollTo(0,800);
}
	if (window.DOMParser)
	{
		parser=new DOMParser();
		xmlDoc=parser.parseFromString(gotIt,"text/xml");
		parser=null;
	}
	else // Internet Explorer
	{
		xmlDoc=new ActiveXObject("Microsoft.XMLDOM");
		xmlDoc.async="false";
		xmlDoc.loadXML(gotIt); 
	}
	RA_CtrlWin.RA.RAWS.XML = xmlDoc;
	xmlDoc = null;
if (RA_CtrlWin.RA.dev_check('.net'))
{
msg += RA_CtrlWin.RA.RAWS.XML;
prompt('RAgLocalProxy_Process success', msg);
}
	try {
		cbk();
	} catch(e) {
alert('RAgLocalProxy_Process cbk ERROR :: '+ e.lineNumber +' - '+ e.message);
	}
}
function RAgLocalProxy_HttpResp_Error (XMLHttpRequest, textStatus, errorThrown) {
	var msg = '';
	msg += RA_CtrlWin.RA.RAWS.Http.status +' ---- ';
if (RA_CtrlWin.RA.dev_check('.net'))
{
	msg += XMLHttpRequest.status +' ---- ';
	msg += XMLHttpRequest.statusText +' ---- ';
	msg += XMLHttpRequest.responseText +' ---- ';
	msg += XMLHttpRequest.responseXML +' ---- ';
	msg += textStatus.status +' ---- ';
	msg += errorThrown +' ---- ';
}
//	alert('RAgLocalProxy_HttpResp_Error ::: '+ result);
	if (RA_CtrlWin.RA.RAWS.Http.status != 0 ) {
		prompt('Problem retrieving RA data: ', msg);
//		top.location.reload();
	} else {
//		alert('YO');
	}
}
//415 ---- 415 ---- Unsupported Media Type ----  ---- undefinedundefined ---- undefined ---- 

/* ------------------------------------------------------------ */
/* ------------------------------------------------------------ */

RA_CtrlWin.RA.RAWS.ERROR_PROCESS = function () {
	var errEl;
	if (RA_CtrlWin.RA.RAWS.XML.documentElement==null) {
//alert(0);
		RA_CtrlWin.RA.RAWS.error += 'NO RESPONSE';
	} else if ( RA_CtrlWin.RA.RAWS.XML.getElementsByTagName('soap:Fault').length>0 ) {
//alert(1);
		RA_CtrlWin.RA.RAWS.error += 'soap:Fault';
		errEl = RA_CtrlWin.RA.RAWS.XML.getElementsByTagName('soap:Fault')[0];
		for (var i=0; i<errEl.childNodes.length; i++) {
			if (errEl.childNodes[i].nodeType==1) {
				RA_CtrlWin.RA.RAWS.error += errEl.childNodes[i].nodeName;
				RA_CtrlWin.RA.RAWS.error += ' :: \n';
				for (var j=0; j<errEl.childNodes[i].childNodes.length; j++) {
					if (errEl.childNodes[i].childNodes[j].nodeType==3) {
						RA_CtrlWin.RA.RAWS.error += errEl.childNodes[i].childNodes[j].nodeValue;
						RA_CtrlWin.RA.RAWS.error += '\n';
					}
				}
			}
		}
	} else if ( RA_CtrlWin.RA.RAWS.XML.getElementsByTagName('Fault').length>0 ) {
//alert(2);
		RA_CtrlWin.RA.RAWS.error += 'soap:Fault';
		RA_CtrlWin.RA.RAWS.error += ' :::::::: \n';
		errEl = RA_CtrlWin.RA.RAWS.XML.getElementsByTagName('Fault')[0];
		for (var i=0; i<errEl.childNodes.length; i++) {
			if (errEl.childNodes[i].nodeType==1) {
				RA_CtrlWin.RA.RAWS.error += errEl.childNodes[i].nodeName;
				RA_CtrlWin.RA.RAWS.error += ' :: \n';
				for (var j=0; j<errEl.childNodes[i].childNodes.length; j++) {
					if (errEl.childNodes[i].childNodes[j].nodeType==3) {
						RA_CtrlWin.RA.RAWS.error += errEl.childNodes[i].childNodes[j].nodeValue;
						RA_CtrlWin.RA.RAWS.error += '\n';
					}
				}
			}
		}
	} else if ( RA_CtrlWin.RA.RAWS.XML.getElementsByTagName('sErrorMsg').length>0 ) {
//alert(3);
		errEl = RA_CtrlWin.RA.RAWS.XML.getElementsByTagName('sErrorMsg')[0];
		for (var i=0; i<errEl.childNodes.length; i++) {
			if (errEl.childNodes[i].nodeType==3) {
				RA_CtrlWin.RA.RAWS.error += errEl.childNodes[i].nodeValue;
			}
		}
	} else if ( RA_CtrlWin.RA.RAWS.XML.getElementsByTagName('sErrorMessage').length>0 ) {
//alert(4);
		errEl = RA_CtrlWin.RA.RAWS.XML.getElementsByTagName('sErrorMessage')[0];
		for (var i=0; i<errEl.childNodes.length; i++) {
			if (errEl.childNodes[i].nodeType==3) {
				RA_CtrlWin.RA.RAWS.error += errEl.childNodes[i].nodeValue;
			}
		}
	}
	if (RA_CtrlWin.RA.RAWS.error!='') {
		return true;
	}
	return false;
}

/* ------------------------------------------------------------ */
/* ------------------------------------------------------------ */

RA_CtrlWin.RA.RAWS.Ravell_UserAuthentication = function (arguments) {
	RA_CtrlWin.RA.RAWS.ClearVars();
	var sUserName;
	var sPassword;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserName') sUserName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sPassword') sPassword = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'Ecom_User_ID='+ sUserName +'&Ecom_Password='+ sPassword +'&target=no.target'+
'';
//prompt('strEnvelope',strEnvelope);
	RA_CtrlWin.RA.RAWS.HTTPREQUEST('RA_CtrlWin.RA.RAWS.Ravell_UserAuthentication',strEnvelope);
}
RA_CtrlWin.RA.RAWS.Ravell_UserAuthentication_PROCESS = function () {

	if ( ! RA_CtrlWin.RA.RAWS.ERROR_PROCESS() ) {
/*
prompt('RA_CtrlWin.RA.RAWS.Ravell_UserAuthentication_PROCESS RA_CtrlWin.RA.RAWS.XML',RA_CtrlWin.RA.RAWS.XML);
prompt('#dvErrorMsg length',jQuery('#dvErrorMsg', RA_CtrlWin.RA.RAWS.XML).length);
prompt('#dvSuccess length',jQuery('#dvSuccess', RA_CtrlWin.RA.RAWS.XML).length);
prompt('#dvSuccess text',jQuery('#dvSuccess', RA_CtrlWin.RA.RAWS.XML).text());
prompt('RA_CtrlWin.RA.RAWS.Ravell_UserAuthentication_PROCESS RA_CtrlWin.RA.RAWS.XML',RA_CtrlWin.RA.RAWS.XML);
prompt('RA_CtrlWin.RA.RAWS.Ravell_UserAuthentication_PROCESS RA_CtrlWin.RA.RAWS.XML.xml',RA_CtrlWin.RA.RAWS.XML.xml);
*/
		var ErrorMsgExists = (jQuery('#dvErrorMsg', RA_CtrlWin.RA.RAWS.XML).length > 0) ? true : false;
		var SuccessMsgExists = (jQuery('#dvSuccess', RA_CtrlWin.RA.RAWS.XML).length > 0) ? true : false;
		var SuccessMsg = jQuery('#dvSuccess', RA_CtrlWin.RA.RAWS.XML).text();

		if (SuccessMsgExists && SuccessMsg.toUpperCase()=='OK')
		{
		}
		else if (ErrorMsgExists)
		{
			RA_CtrlWin.RA.RAWS.error = jQuery('Error', RA_CtrlWin.RA.RAWS.XML).text().trim();
			if (RA_CtrlWin.RA.RAWS.error  == '')
			{
				RA_CtrlWin.RA.RAWS.error = 'Unknown error.';
			}
		}
//prompt('Error length',jQuery('Error', RA_CtrlWin.RA.RAWS.XML).length);
//prompt('RA_CtrlWin.RA.RAWS.error',RA_CtrlWin.RA.RAWS.error);
	}

	RA_CtrlWin.RA.RAWS.WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}


	// *****************************************************************************
	// *****************************************************************************
	// on document ready, check required UI elements

	$(document).ready(function(){

		if ($('#'+ RA_CtrlWin.RA.RAUI.divId ).length < 1)
		{
			$('#'+ RA_CtrlWin.RA.RAUI.divId ).remove();
			$('body').append('<div id="'+ RA_CtrlWin.RA.RAUI.divId +'" style="display:none"></div>');
		}

		if (RA_CtrlWin.RA.dev_check('any'))
		{
			if ($('#RA_info_div').length != 1)
			{
				$('#RA_info_div').remove();
				$('body').append('<div id="RA_info_div" style="display:none"><div><a id="RA_debugmsg_toggle" class="RA_toggler" href="JavaScript:void(0);">toggle display of RA debug message</a></div><div id="RA_debugmsg" style="display:none" class="RA_data"></div><div><a id="RA_data_div_toggle" class="RA_toggler" href="JavaScript:void(0);">toggle display of RA data</a></div><div id="RA_data_div" style="display:none" class="RA_data"></div></div>');
				$('.RA_toggler').click( function() {
					var thisId = $(this).attr('id');
					thisId = thisId.replace('_toggle','');
					$('#'+thisId).toggle();
				});
				$('.RA_toggler').show();
			}
			$('#RA_info_div').show();
		}
	});

} (jQuery) );

}




// BFWj_SimpleButton
/*
 REQUIRES:
    jQuery
*/ 
(function($) {
    if (!$) 
    {
        throw new Error('jQuery required for BFWj.SimpleButton');
		return;
    }
    else if (typeof $.fn.position !== 'function') 
    {
        throw new Error('jQuery position required for BFWj.SimpleButton');
		return;
    }
/*
$(...).BFWj_SimpleButton({
    text: "LOG IN" 
,   url: "JavaScript:RA_CtrlWin.RA.RAUI.app.goLogin();"
,   id: ""
,   type: "primary"
,   block: false
,   size: "medium"
,   extra: "style='float:none; text-align:center'"
})
 */
	$.fn.BFWj_SimpleButton = function( options ) {
		var settings = {
				text: 'CLICK'
			,   url: 'Javascript:alert("button clicked");'
			,   id: ''
			,   type: 'primary'
			,   size: 'medium'
			,   block: false
			,   extra: ''
		};
		if (options) {
			$.extend( settings, options);
		}
		return this.each(function(){
			var html, typStyle;
			
			if (settings.size == "" || settings.size == null) {
				settings.size = "medium";
			}
			
			if (settings.type == "link" || settings.type == "primary" || settings.type == "settingBlue") {
				typStyle = "link";
			} else {
				typStyle = "setting";
			}
			
			html = "<a class='BFWj_SimpleButton BFWj_SimpleButton_" + settings.size + " BFWj_SimpleButton_" + settings.size + "_" + typStyle + "'";
			// html = "<a class='squarebutton'";

			if (settings.extra != "" && settings.extra != null) {
				html += " " + settings.extra;	// ' i.e. " style='float:right'"
			}
			
			if ($(this).attr('id') != "") {	
				html += " id='" + $(this).attr('id') + "'";
			} else if (settings.id != "" && settings.id != null) {	
				html += " id='" + settings.id + "'";
			}
			
			// Note that for javascript urls, *double* quotes need to be escaped
			// We do this because in vb script, it's easier to code single quotes without escaping
			if ($(this).attr('href') != "") {	
				html += ' href="' + $(this).attr('href') + '"><span>';
			} else {
				html += ' href="' + settings.url + '"><span>';
			}
			html += $(this).html();

			if (settings.type == "setting" || settings.type == "settingBlue") {
				html += "<img class='starr' src='/BFW/pics/launch-arrow.gif' alt='' />";
			}
			
			html += "</span></a>";
			
			if (settings.block == true) {
				html = "<div class='buttonwrapper'>" + html + "</div><div style='clear: both;'></div>";
			}
			
			$(this).replaceWith(html);
		});
	};
})(jQuery);


