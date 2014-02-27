// *********************************************************
// *********************************************************
// *********************************************************
// *********************************************************











var RAWS_HTTPREQUEST_timeOut = null;
var RAWS_HTTPREQUEST_timedOut = false;
var RAWS_interval_ct = 100;
var RAWS_WaitFor_Ready = true;
var RAWS_WaitFor_interval = null;
var RAWS_requestStatus_t;
var RAWS_error = '';
var RAWS_iUserID;
var RAWS_iClassUsingUserID;
var RAWS_iClassUsingClassID;
var RAWS_sUserName;
var RAWS_sPassword;
var RAWS_sFirstName;
var RAWS_sLastName;
var RAWS_sPasswordHint;
var RAWS_sMailPreference;
var RAWS_sOptInEmail;

var RAWS_XSSessionGUID;

var RAWS_iUserInstructorStatus = 0;

var RAWS_udtClassInfo = new Array();
var RAWS_udtPackages = new Object();
var RAWS_udtSites = new Object();
var RAWS_udtSchools = new Array();

var RAWS_sBaseUrl;
var RAWS_sSiteDesc;
var RAWS_iSiteID;
var RAWS_sConfirmation;

var RAWS_iPackageID;

var RAWS_arrSiteIDs = new Array();

var RAWS_sInstructorEmail;
var RAWS_sIPAddr;
var RAWS_sInstEmail;
var RAWS_iLevelOfAccess;


function RAWS_ClearVars () {
	RA_CtrlWin.RAWS_error = '';
	RA_CtrlWin.RAWS_iUserID = '';
	RA_CtrlWin.RAWS_sUserName = '';
	RA_CtrlWin.RAWS_sPassword = '';
	RA_CtrlWin.RAWS_sFirstName = '';
	RA_CtrlWin.RAWS_sLastName = '';
	RA_CtrlWin.RAWS_sPasswordHint = '';
	RA_CtrlWin.RAWS_sMailPreference = '';
	RA_CtrlWin.RAWS_sOptInEmail = '';
	
	RA_CtrlWin.RAWS_XSSessionGUID = '';

	RA_CtrlWin.RAWS_udtClassInfo = new Array();
	RA_CtrlWin.RAWS_udtPackages = new Object();
	RA_CtrlWin.RAWS_udtSites = new Object();
	RA_CtrlWin.RAWS_udtSchools = new Array();
/*
	RA_CtrlWin.RAWS_iClassID = '';
	RA_CtrlWin.RAWS_iCreatorID = '';
	RA_CtrlWin.RAWS_sClassName = '';
	RA_CtrlWin.RAWS_sClassDesc = '';
	RA_CtrlWin.RAWS_sClassCode = '';
	RA_CtrlWin.RAWS_dtExprn = '';
	RA_CtrlWin.RAWS_iUserID = '';
	RA_CtrlWin.RAWS_bClassAccessRevoked = '';
	RA_CtrlWin.RAWS_dtLastLogin = '';
	RA_CtrlWin.RAWS_dtStartDate = '';
	RA_CtrlWin.RAWS_dtEndDate = '';
	RA_CtrlWin.RAWS_bEmailScores = '';
	RA_CtrlWin.RAWS_iRecordStatus = '';
*/
	RA_CtrlWin.RAWS_sBaseUrl = '';
	RA_CtrlWin.RAWS_sSiteDesc = '';
	RA_CtrlWin.RAWS_iSiteID = '';
	RA_CtrlWin.RAWS_sConfirmation = '';

	RA_CtrlWin.RAWS_sInstructorEmail = '';
	RA_CtrlWin.RAWS_sIPAddr = '';
	RA_CtrlWin.RAWS_iLevelOfAccess = '';
}

function RAWS_HTTPREQUEST_doTimeOut() {
	alert('RAWS timed out');
//	alert(RA_CtrlWin.RAWS_Http.status);
	RA_CtrlWin.RAWS_HTTPREQUEST_timedOut = true;
}

function RAWS_WaitFor (fn) {
	RA_CtrlWin.RAWS_WaitFor_Ready = false;
	RA_CtrlWin.RAWS_WaitFor_interval = window.setInterval('RA_CtrlWin.RAWS_WaitFor_check('+fn+')',RA_CtrlWin.RAWS_interval_ct);
}
function RAWS_WaitFor_check (fn) {
//alert(RA_CtrlWin.RAWS_WaitFor_Ready +' - '+ RA_CtrlWin.RAWS_HTTPREQUEST_timedOut);

	if (RA_CtrlWin.RAWS_WaitFor_Ready) {
		RA_CtrlWin.RAWS_WaitFor_clear(fn);
	}
}
function RAWS_WaitFor_clear (fn) {
	RA_CtrlWin.RAWS_WaitFor_interval = window.clearInterval(RA_CtrlWin.RAWS_WaitFor_interval);
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	RA_CtrlWin.RAWS_WaitFor_Ready_Go(fn);
}
function RAWS_WaitFor_Ready_Go (fn) {
//alert('RA_CtrlWin.RAWS_WaitFor_Ready_Go');
	setTimeout(fn, 100);
}

/* ------------------------------------------------------------ */
var RAWS_Http;
var RAWS_XML;


function RAWS0_NO_PROCESSOR() {
	alert('No processor switch for function \''+ RAWS_function +'\'');
}
function RAWS0_ERROR() {
	if (RA_CtrlWin.RAWS_Http.status != 0 ) {
//		alert('Problem retrieving RA data: '+ RA_CtrlWin.RAWS_Http.status);
//		top.location.reload();
	} else {
//		alert('YO');
	}
}

var RAWSNET_cbk;
function RAWS_HTTPREQUEST (RAWS_function, strEnvelope) {
//alert('RAWS_HTTPREQUEST  :::::: '+ RAWS_function);

	var cbk;

	switch (RAWS_function) {
	case 'IL_CheckInstructorAccess' :
		cbk = IL_CheckInstructorAccess_PROCESS;
	break;
	case 'RAXS_WS':
		cbk = RAXS_WS_PROCESS;
	break;
	case '1_FetchSiteID':
		cbk = RAWS1_FetchSiteID_PROCESS;
	break;
	case 'Ravell_UserAuthentication':
		cbk = Ravell_UserAuthentication_PROCESS;
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
		cbk = RAWS0_NO_PROCESSOR;
	}

	var szAction;
	switch (RAWS_function) {
		case 'IL_CheckInstructorAccess' :
	szAction = 'http://tempuri.org/webservices/'+ RAWS_function;
			break;
		default :
	szAction = 'http://tempuri.org/'+ RAWS_function;
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
	switch (RAWS_function) {
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
		case 'Ravell_UserAuthentication' :
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
	szUrl += '?wsID=RAWS1&wsF='+ RAWS_function;
	}

	var winhost = window.location.host;
	if (winhost.indexOf('localhost')==0 && RA_CtrlWin.RA.ProxyType=='ASP.NET' && RA_CtrlWin.RA.LocalProxyURL!='')
	{
		RAWSNET_cbk = cbk;
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
		RA_CtrlWin.RAWS_Http = jQuery.ajax({
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
		RA_CtrlWin.RAWS_Http = jQuery.ajax({
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
				RA_CtrlWin.RAWS_XML = data;
				try {
							cbk();
				} catch(e) {
alert('cbk ERROR :: '+ e.lineNumber +' - '+ e.message);
				}
			},
			error: RAWS0_ERROR
		});
	}

}

//function RAgLocalProxy_Process (result) {
function RAgLocalProxy_Process (data,textStatus) {
	var cbk = RAWSNET_cbk;
	RAWSNET_cbk = null;
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
	RA_CtrlWin.RAWS_XML = xmlDoc;
	xmlDoc = null;
if (RA_CtrlWin.RA.dev_check('.net'))
{
msg += RA_CtrlWin.RAWS_XML;
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
	msg += RA_CtrlWin.RAWS_Http.status +' ---- ';
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
	if (RA_CtrlWin.RAWS_Http.status != 0 ) {
		prompt('Problem retrieving RA data: ', msg);
//		top.location.reload();
	} else {
//		alert('YO');
	}
}
//415 ---- 415 ---- Unsupported Media Type ----  ---- undefinedundefined ---- undefined ---- 

/* ------------------------------------------------------------ */
/* ------------------------------------------------------------ */

function RAWS_ERROR_PROCESS () {
	var errEl;
	if (RA_CtrlWin.RAWS_XML.documentElement==null) {
//alert(0);
		RA_CtrlWin.RAWS_error += 'NO RESPONSE';
	} else if ( RA_CtrlWin.RAWS_XML.getElementsByTagName('soap:Fault').length>0 ) {
//alert(1);
		RA_CtrlWin.RAWS_error += 'soap:Fault';
		errEl = RA_CtrlWin.RAWS_XML.getElementsByTagName('soap:Fault')[0];
		for (var i=0; i<errEl.childNodes.length; i++) {
			if (errEl.childNodes[i].nodeType==1) {
				RA_CtrlWin.RAWS_error += errEl.childNodes[i].nodeName;
				RA_CtrlWin.RAWS_error += ' :: \n';
				for (var j=0; j<errEl.childNodes[i].childNodes.length; j++) {
					if (errEl.childNodes[i].childNodes[j].nodeType==3) {
						RA_CtrlWin.RAWS_error += errEl.childNodes[i].childNodes[j].nodeValue;
						RA_CtrlWin.RAWS_error += '\n';
					}
				}
			}
		}
	} else if ( RA_CtrlWin.RAWS_XML.getElementsByTagName('Fault').length>0 ) {
//alert(2);
		RA_CtrlWin.RAWS_error += 'soap:Fault';
		RA_CtrlWin.RAWS_error += ' :::::::: \n';
		errEl = RA_CtrlWin.RAWS_XML.getElementsByTagName('Fault')[0];
		for (var i=0; i<errEl.childNodes.length; i++) {
			if (errEl.childNodes[i].nodeType==1) {
				RA_CtrlWin.RAWS_error += errEl.childNodes[i].nodeName;
				RA_CtrlWin.RAWS_error += ' :: \n';
				for (var j=0; j<errEl.childNodes[i].childNodes.length; j++) {
					if (errEl.childNodes[i].childNodes[j].nodeType==3) {
						RA_CtrlWin.RAWS_error += errEl.childNodes[i].childNodes[j].nodeValue;
						RA_CtrlWin.RAWS_error += '\n';
					}
				}
			}
		}
	} else if ( RA_CtrlWin.RAWS_XML.getElementsByTagName('sErrorMsg').length>0 ) {
//alert(3);
		errEl = RA_CtrlWin.RAWS_XML.getElementsByTagName('sErrorMsg')[0];
		for (var i=0; i<errEl.childNodes.length; i++) {
			if (errEl.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_error += errEl.childNodes[i].nodeValue;
			}
		}
	} else if ( RA_CtrlWin.RAWS_XML.getElementsByTagName('sErrorMessage').length>0 ) {
//alert(4);
		errEl = RA_CtrlWin.RAWS_XML.getElementsByTagName('sErrorMessage')[0];
		for (var i=0; i<errEl.childNodes.length; i++) {
			if (errEl.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_error += errEl.childNodes[i].nodeValue;
			}
		}
	}
	if (RA_CtrlWin.RAWS_error!='') {
		return true;
	}
	return false;
}

/* ------------------------------------------------------------ */

function RAXS_WS (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sBaseUrl;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://tempuri.org/\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <RAXS_WS xmlns=\"http://tempuri.org/\">' +
'      <iUserID>'+ iUserID +'</iUserID>' +
'    </RAXS_WS>' +
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
if (RA_CtrlWin.RA.dev_check()) prompt('strEnvelope',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('RAXS_WS',strEnvelope);
}

var RAXS_WS_XML = null;
function RAXS_WS_PROCESS () {

	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		RA_CtrlWin.RAXS_WS_XML = RA_CtrlWin.RAWS_XML;
if (RA_CtrlWin.RA.dev_check()) prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAXS_WS_XML.xml );
//prompt('RA_CtrlWin.RAWS_XML.xml',jQuery('RAXS_WS',RA_CtrlWin.RAXS_WS_XML) );
//prompt('RA_CtrlWin.RAWS_XML.xml',jQuery('RAXS_WS:first > results:first > udtUserProfile:first > iUserID',RA_CtrlWin.RAWS_XML).text() );
	} else {
alert(RA_CtrlWin.RAWS_error);
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}


/* ------------------------------------------------------------ */

function RAWS3_GetPackages (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sBaseUrl;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sPackageIDs') sPackageIDs = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sSiteIDs') sSiteIDs = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserIDs') sUserIDs = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://tempuri.org/\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <GetPackages xmlns=\"http://tempuri.org/\">' +
'      <sPackageIDs>'+ sPackageIDs +'</sPackageIDs>' +
'      <sSiteIDs>'+ sSiteIDs +'</sSiteIDs>' +
'      <sUserIDs>'+ sUserIDs +'</sUserIDs>' +
'    </GetPackages>' +
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//prompt('strEnvelope',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('3_GetPackages',strEnvelope);
}

function RAWS3_GetPackages_PROCESS () {

	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
//prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('GetPackagesResult')[0];

		RA_CtrlWin.RAWS_udtPackages = new Object();
		var nPs = RA_CtrlWin.RAWS_XML.getElementsByTagName('udtPackage');
		var xEl;
		var xPID;
		var nSAs;
		var xSID;
		for (var iP=0; iP<nPs.length; iP++) {
			xEl = nPs[iP].getElementsByTagName('iPackageID')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) xPID = xEl.childNodes[i].nodeValue;
			}
			RA_CtrlWin.RAWS_udtPackages[xPID] = new Object();
			RA_CtrlWin.RAWS_udtPackages[xPID].PackageID = xPID;
			xEl = nPs[iP].getElementsByTagName('sPackageDescription')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtPackages[xPID].PackageDescription = xEl.childNodes[i].nodeValue;
			}
			xEl = nPs[iP].getElementsByTagName('sPackageType')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtPackages[xPID].PackageType = xEl.childNodes[i].nodeValue;
			}
			nSAs = nPs[iP].getElementsByTagName('udtSiteAssignment');
			RA_CtrlWin.RAWS_udtPackages[xPID].SiteAssignments = new Object();
			for (var iSA=0; iSA<nSAs.length; iSA++) {
				xEl = nSAs[iSA].getElementsByTagName('iSiteID')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) xSID = xEl.childNodes[i].nodeValue;
				}
				RA_CtrlWin.RAWS_udtPackages[xPID].SiteAssignments[xSID] = new Object();
				RA_CtrlWin.RAWS_udtPackages[xPID].SiteAssignments[xSID].SiteID = xSID;

				xEl = nSAs[iSA].getElementsByTagName('iSiteAssignmentID')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtPackages[xPID].SiteAssignments[xSID].SiteAssignmentID = xEl.childNodes[i].nodeValue;
				}
				xEl = nSAs[iSA].getElementsByTagName('sSiteBaseURL')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtPackages[xPID].SiteAssignments[xSID].SiteBaseURL = xEl.childNodes[i].nodeValue;
				}
				xEl = nSAs[iSA].getElementsByTagName('sSiteDescription')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtPackages[xPID].SiteAssignments[xSID].SiteDescription = xEl.childNodes[i].nodeValue;
				}
				xEl = nSAs[iSA].getElementsByTagName('iSiteLevelOfAccess')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtPackages[xPID].SiteAssignments[xSID].SiteLevelOfAccess = xEl.childNodes[i].nodeValue;
				}
			}
		}
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}


/* ------------------------------------------------------------ */

function RAWS1_FetchSiteID  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sBaseUrl;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sBaseUrl') sBaseUrl = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://tempuri.org/\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <FetchSiteID xmlns=\"http://tempuri.org/\">' +
'      <sBaseURL>'+ sBaseUrl +'</sBaseURL>' +
'    </FetchSiteID>' +
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//prompt('RA_CtrlWin.RAWS_FetchSiteID',strEnvelope);
	RAWS_HTTPREQUEST('1_FetchSiteID',strEnvelope);
}

function RAWS1_FetchSiteID_PROCESS () {
//alert('RAWS_FetchSiteID_PROCESS');
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('FetchSiteIDResult')[0];
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_iSiteID = Result_Node.childNodes[i].nodeValue;
		}
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}

/* ------------------------------------------------------------ */

function RAWS2_FetchSiteID  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sBaseUrl;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sBaseUrl') sBaseUrl = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://tempuri.org/\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <FetchSiteID xmlns=\"http://tempuri.org/\">' +
'      <sBaseURL>'+ sBaseUrl +'</sBaseURL>' +
'    </FetchSiteID>' +
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
prompt('RA_CtrlWin.RAWS_FetchSiteID',strEnvelope);
	RAWS_HTTPREQUEST('2_FetchSiteID',strEnvelope);
}

function RAWS2_FetchSiteID_PROCESS () {
//alert('RAWS_FetchSiteID_PROCESS');
prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('FetchSiteIDResult')[0];
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_iSiteID = Result_Node.childNodes[i].nodeValue;
		}
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}

/* ------------------------------------------------------------ */

function Ravell_UserAuthentication  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
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
	RA_CtrlWin.RAWS_HTTPREQUEST('Ravell_UserAuthentication',strEnvelope);
}
function Ravell_UserAuthentication_PROCESS () {

	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
/*
prompt('Ravell_UserAuthentication_PROCESS RA_CtrlWin.RAWS_XML',RA_CtrlWin.RAWS_XML);
prompt('#dvErrorMsg length',jQuery('#dvErrorMsg', RA_CtrlWin.RAWS_XML).length);
prompt('#dvSuccess length',jQuery('#dvSuccess', RA_CtrlWin.RAWS_XML).length);
prompt('#dvSuccess text',jQuery('#dvSuccess', RA_CtrlWin.RAWS_XML).text());
prompt('Ravell_UserAuthentication_PROCESS RA_CtrlWin.RAWS_XML',RA_CtrlWin.RAWS_XML);
prompt('Ravell_UserAuthentication_PROCESS RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
*/
		var ErrorMsgExists = (jQuery('#dvErrorMsg', RA_CtrlWin.RAWS_XML).length > 0) ? true : false;
		var SuccessMsgExists = (jQuery('#dvSuccess', RA_CtrlWin.RAWS_XML).length > 0) ? true : false;
		var SuccessMsg = jQuery('#dvSuccess', RA_CtrlWin.RAWS_XML).text();

		if (SuccessMsgExists && SuccessMsg.toUpperCase()=='OK')
		{
		}
		else if (ErrorMsgExists)
		{
			RA_CtrlWin.RAWS_error = jQuery.trim( jQuery('Error', RA_CtrlWin.RAWS_XML).text() );
			if (RA_CtrlWin.RAWS_error  == '')
			{
				RA_CtrlWin.RAWS_error = 'Unknown error.';
			}
		}
//prompt('Error length',jQuery('Error', RA_CtrlWin.RAWS_XML).length);
//prompt('RA_CtrlWin.RAWS_error',RA_CtrlWin.RAWS_error);
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}



/* ------------------------------------------------------------ */

function RAWS1_UserAuthentication  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserName;
	var sPassword;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserName') sUserName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sPassword') sPassword = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<UserAuthentication xmlns=\"http://tempuri.org/\">' + 
'		<sUserName>'+sUserName+'</sUserName>' + 
'		<sPassword>'+sPassword+'</sPassword>' + 
'	</UserAuthentication>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
'';
//prompt('strEnvelope',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('1_UserAuthentication',strEnvelope);
}
function RAWS1_UserAuthentication_PROCESS () {
//prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);

	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
/*
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('UserAuthenticationResult')[0];
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_iUserID = Result_Node.childNodes[i].nodeValue;
		}
*/
		RA_CtrlWin.RAWS_iUserID = jQuery('UserAuthenticationResult', RA_CtrlWin.RAWS_XML).text();
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}



/* ------------------------------------------------------------ */

function RAWS3_UserAuthentication  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserName;
	var sPassword;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserName') sUserName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sPassword') sPassword = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<UserAuthentication xmlns=\"RAUserAuthentication\">' + 
'		<sUserName>'+sUserName+'</sUserName>' + 
'		<sPassword>'+sPassword+'</sPassword>' + 
'	</UserAuthentication>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
'';
//if (RA_CtrlWin.RA.dev_check()) prompt('strEnvelope',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('3_UserAuthentication',strEnvelope);
}
function RAWS3_UserAuthentication_PROCESS () {
//if (RA_CtrlWin.RA.dev_check()) prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);

	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
/*
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('UserAuthenticationResult')[0];
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_iUserID = Result_Node.childNodes[i].nodeValue;
		}
*/
		RA_CtrlWin.RAWS_iUserID = jQuery('UserAuthenticationResult', RA_CtrlWin.RAWS_XML).text();
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}


/* ------------------------------------------------------------ */

function RAWS3_GetDefaultClass  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserEmail;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserEmail') sUserEmail = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<CheckClass xmlns=\"http://bfwpub.com/\">' + 
'		<UserEmail>'+sUserEmail+'</UserEmail>' + 
'	</CheckClass>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
'';
//if (RA_CtrlWin.RA.dev_check()) prompt('strEnvelope',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('3_GetDefaultClass',strEnvelope);
}
function RAWS3_GetDefaultClass_PROCESS () {
//if (RA_CtrlWin.RA.dev_check()) prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);

	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('CheckClassResult')[0];
//prompt('Result_Node.xml',Result_Node.xml);
		var result_xml_str;
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) result_xml_str = Result_Node.childNodes[i].nodeValue;
		}
		var xmlDoc;
		try //Internet Explorer
		{
			xmlDoc=new ActiveXObject("Microsoft.XMLDOM");
			xmlDoc.async="false";
			xmlDoc.loadXML(result_xml_str);
		}
		catch(e)
		{
			try //Firefox, Mozilla, Opera, etc.
			{
//				xmlDoc=document.implementation.createDocument("","",null);
				var parser=new DOMParser();
				xmlDoc=parser.parseFromString(result_xml_str,"text/xml"); 
			}
			catch(e) {alert(e.message)}
		}
		var el = xmlDoc.getElementsByTagName('sClassID')[0]
		for (var i=0; i<el.childNodes.length; i++) {
			if (el.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_iClassUsingClassID = el.childNodes[i].nodeValue;
			}
		}
		el = xmlDoc.getElementsByTagName('sRAUID')[0]
		for (var i=0; i<el.childNodes.length; i++) {
			if (el.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_iClassUsingUserID = el.childNodes[i].nodeValue;
			}
		}
//prompt( 'CheckClassResult', RA_CtrlWin.RAWS_iClassUsingClassID +' - '+ RA_CtrlWin.RAWS_iClassUsingUserID );
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}


/* ------------------------------------------------------------ */

function RAWS1_UpdateProfile  (arguments) {
	
	RA_CtrlWin.RAWS_ClearVars();
	
	var iUserID;
	var sUserEmail;
	var sInstructorEmail;
	var sUserFirstName;
	var sUserLastName;
	var sMailPreferences = 'TEXT';
	var sOptInEmail = 0;
	var sBaseURL;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserEmail') sUserEmail = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sInstructorEmail') sInstructorEmail = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserFirstName') sUserFirstName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserLastName') sUserLastName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sMailPreferences') sMailPreferences = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sOptInEmail') sOptInEmail = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sBaseURL') sBaseURL = arguments[i].split('=')[1];
		//if (arguments[i].split('=')[0] == 'sBaseURL') sBaseURL ='bcs.worthpublishers.com/myers8e'; 
	}
	
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <UpdateProfile xmlns=\"http://tempuri.org/\">' + 
'      <iUserID>'+iUserID+'</iUserID>' + 
'      <sUserEmail>'+sUserEmail+'</sUserEmail>' + 
'      <sInstructorEmail>'+sInstructorEmail+'</sInstructorEmail>' + 
'      <sFirstName>'+sUserFirstName+'</sFirstName>' + 
'      <sLastName>'+sUserLastName+'</sLastName>' + 
'      <sMailPreferences>'+sMailPreferences+'</sMailPreferences>' + 
'      <sOptInEmail>'+sOptInEmail+'</sOptInEmail>' + 
'      <sBaseURL>'+sBaseURL+'</sBaseURL>' + 
'    </UpdateProfile>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
'';

//alert (strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('1_UpdateProfile',strEnvelope);
}
function RAWS1_UpdateProfile_PROCESS () {

	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('UpdateProfileResult')[0];
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}

}

/* ------------------------------------------------------------ */

function RAWS3_UpdateProfile  (arguments) {
	
	RA_CtrlWin.RAWS_ClearVars();
	
	var iUserID;
	var sUserEmail;
	var sInstructorEmail;
	var sUserFirstName;
	var sUserLastName;
	var sMailPreferences = 'TEXT';
	var sOptInEmail = 0;
	var sBaseURL;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserEmail') sUserEmail = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sInstructorEmail') sInstructorEmail = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserFirstName') sUserFirstName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserLastName') sUserLastName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sMailPreferences') sMailPreferences = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sOptInEmail') sOptInEmail = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sBaseURL') sBaseURL = arguments[i].split('=')[1];
		//if (arguments[i].split('=')[0] == 'sBaseURL') sBaseURL ='bcs.worthpublishers.com/myers8e'; 
	}
	
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <UpdateProfile xmlns=\"RAUpdateProfile\">' + 
'      <iUserID>'+iUserID+'</iUserID>' + 
'      <sUserEmail>'+sUserEmail+'</sUserEmail>' + 
'      <sInstructorEmail>'+sInstructorEmail+'</sInstructorEmail>' + 
'      <sFirstName>'+sUserFirstName+'</sFirstName>' + 
'      <sLastName>'+sUserLastName+'</sLastName>' + 
'      <sMailPreferences>'+sMailPreferences+'</sMailPreferences>' + 
'      <sOptInEmail>'+sOptInEmail+'</sOptInEmail>' + 
'      <sBaseURL>'+sBaseURL+'</sBaseURL>' + 
'    </UpdateProfile>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
'';

if (RA_CtrlWin.RA.dev_check('ET')) prompt('strEnvelope', strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('3_UpdateProfile',strEnvelope);
}
function RAWS3_UpdateProfile_PROCESS () {

if (RA_CtrlWin.RA.dev_check('ET')) prompt('RA_CtrlWin.RAWS_error', RA_CtrlWin.RAWS_error);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
if (RA_CtrlWin.RA.dev_check('ET')) prompt('RA_CtrlWin.RAWS_XML.xml', RA_CtrlWin.RAWS_XML.xml);
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('UpdateProfileResult')[0];
	} else {
	}
if (RA_CtrlWin.RA.dev_check()) prompt('RA_CtrlWin.RAWS_XML', jQuery('UpdateProfileResult',RA_CtrlWin.RAWS_XML).text());

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}

}

/* ------------------------------------------------------------ */

function RAWS1_UpdatePassword  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var iUserID;
	var sUserName;
	var sOldPwd;
	var sNewPwd;
	var sVPwd;
	var sHintPwd;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserName') sUserName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sOldPwd') sOldPwd = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sNewPwd') sNewPwd = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sVPwd') sVPwd = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sHintPwd') sHintPwd = arguments[i].split('=')[1]; 
	}

	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <UpdatePassword xmlns="http://tempuri.org/">' + 
'      <iUserID>'+iUserID+'</iUserID>' + 
'      <sUserName>'+sUserName+'</sUserName>' + 
'      <sOldPwd>'+sOldPwd+'</sOldPwd>' + 
'      <sNewPwd>'+sNewPwd+'</sNewPwd>' + 
'      <sVPwd>'+sVPwd+'</sVPwd>' + 
'      <sHintPwd>'+sHintPwd+'</sHintPwd>' + 
'    </UpdatePassword>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//prompt('strEnvelope',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('1_UpdatePassword',strEnvelope);
}
function RAWS1_UpdatePassword_PROCESS () {

	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('UpdatePasswordResult')[0];
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}

}

/* ------------------------------------------------------------ */

function RAWS1_GetUsernamePasswordHint (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserName;
	var sBaseUrl;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserName') sUserName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sBaseUrl') sBaseUrl = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<GetUsernamePasswordHint xmlns=\"http://tempuri.org/\">' + 
'		<sUserName>'+sUserName+'</sUserName>' + 
'		<sBaseUrl>'+sBaseUrl+'</sBaseUrl>' + 
'	</GetUsernamePasswordHint>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''

//prompt('strEnvelope', strEnvelope);

	RA_CtrlWin.RAWS_HTTPREQUEST('1_GetUsernamePasswordHint',strEnvelope);
}
function RAWS1_GetUsernamePasswordHint_PROCESS () {
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('sPasswordHint')[0];
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){alert(e.name+' --- '+e.message)}
}

/* ------------------------------------------------------------ */

function RAWS2_GetUsernamePasswordHint (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserName;
	var sBaseUrl;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserName') sUserName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sBaseUrl') sBaseUrl = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">' + 
'  <soap12:Body>' + 
'	<GetUsernamePasswordHint xmlns=\"RAGetUsernamePasswordHint\">' + 
'		<sUserName>'+sUserName+'</sUserName>' + 
'		<sBaseUrl>'+sBaseUrl+'</sBaseUrl>' + 
'	</GetUsernamePasswordHint>' + 
'  </soap12:Body>' + 
'</soap12:Envelope>' + 
''

//prompt('strEnvelope', strEnvelope);

	RA_CtrlWin.RAWS_HTTPREQUEST('2_GetUsernamePasswordHint',strEnvelope);
}
function RAWS2_GetUsernamePasswordHint_PROCESS () {
//prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('sPasswordHint')[0];
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){alert(e.name+' --- '+e.message)}
}

/* ------------------------------------------------------------ */

function RAWS2_CheckUsername (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserName;
	var sBaseUrl;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserName') sUserName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sBaseUrl') sBaseUrl = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<CheckUsername xmlns=\"http://tempuri.org/\">' + 
'		<sUserName>'+sUserName+'</sUserName>' + 
'	</CheckUsername>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''

//prompt('strEnvelope', strEnvelope);

	RA_CtrlWin.RAWS_HTTPREQUEST('2_CheckUsername',strEnvelope);
}
function RAWS2_CheckUsername_PROCESS () {
//prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('CheckUsernameResult')[0];
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){alert(e.name+' --- '+e.message)}
}

/* ------------------------------------------------------------ */

function RAWS3_CheckUsername (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserName;
	var sBaseUrl;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserName') sUserName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sBaseUrl') sBaseUrl = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<CheckUsername xmlns=\"RACheckUsername\">' + 
'		<sUserName>'+sUserName+'</sUserName>' + 
'	</CheckUsername>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''

//if (RA_CtrlWin.RA.dev_check()) prompt('strEnvelope', strEnvelope);

	RA_CtrlWin.RAWS_HTTPREQUEST('3_CheckUsername',strEnvelope);
}
function RAWS3_CheckUsername_PROCESS () {
//if (RA_CtrlWin.RA.dev_check()) prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	RA_CtrlWin.RAWS_iUserID = 0;
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
//		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('CheckUsernameResult')[0];
		try {
			var el = RA_CtrlWin.RAWS_XML.getElementsByTagName('CheckUsernameResult')[0];
			for (var i=0; i<el.childNodes.length; i++) {
				if (el.childNodes[i].nodeType==3) {
					RA_CtrlWin.RAWS_iUserID = el.childNodes[i].nodeValue;
				}
			}
		} catch(e) {}
	}
//if (RA_CtrlWin.RA.dev_check()) prompt('RA_CtrlWin.RAWS_iUserID',RA_CtrlWin.RAWS_iUserID);

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){alert(e.name+' --- '+e.message)}
}

/* ------------------------------------------------------------ */

function RAWS1_StudentRegistration  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserName;
	var sInstructorEmail;
	var sUserFirstName;
	var sUserLastName;
	var sUserPwd;
	var sVerifyPwd;
	var sUserPwdHint;
	var sRemoteIPAddr;
	var sBaseURL;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserName') sUserName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sInstructorEmail') sInstructorEmail = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserFirstName') sUserFirstName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserLastName') sUserLastName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserPwd') sUserPwd = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sVerifyPwd') sVerifyPwd = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserPwdHint') sUserPwdHint = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sRemoteIPAddr') sRemoteIPAddr = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sBaseURL') sBaseURL = arguments[i].split('=')[1]; 
	}
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <StudentRegistration xmlns="http://tempuri.org/">' + 
'      <sUserName>'+sUserName+'</sUserName>' + 
'      <sInstructorEmail>'+sInstructorEmail+'</sInstructorEmail>' + 
'      <sUserFirstName>'+sUserFirstName+'</sUserFirstName>' + 
'      <sUserLastName>'+sUserLastName+'</sUserLastName>' + 
'      <sUserPwd>'+sUserPwd+'</sUserPwd>' + 
'      <sVerifyPwd>'+sVerifyPwd+'</sVerifyPwd>' + 
'      <sUserPwdHint>'+sUserPwdHint+'</sUserPwdHint>' + 
'      <sRemoteIPAddr>'+sRemoteIPAddr+'</sRemoteIPAddr>' + 
'      <sBaseURL>'+sBaseURL+'</sBaseURL>' + 
'    </StudentRegistration>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//prompt('request',strEnvelope)
	RA_CtrlWin.RAWS_HTTPREQUEST('1_StudentRegistration',strEnvelope);
return true;
}
function RAWS1_StudentRegistration_PROCESS () {
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('StudentRegistrationResult')[0];
//prompt('Result_Node.xml',Result_Node.xml);
		var result_xml_str;
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) result_xml_str = Result_Node.childNodes[i].nodeValue;
		}
		var xmlDoc;
		try //Internet Explorer
		{
			xmlDoc=new ActiveXObject("Microsoft.XMLDOM");
			xmlDoc.async="false";
			xmlDoc.loadXML(result_xml_str);
		}
		catch(e)
		{
			try //Firefox, Mozilla, Opera, etc.
			{
//				xmlDoc=document.implementation.createDocument("","",null);
				var parser=new DOMParser();
				xmlDoc=parser.parseFromString(result_xml_str,"text/xml"); 
			}
			catch(e) {alert(e.message)}
		}
		var el = xmlDoc.getElementsByTagName('iUserID')[0]
		for (var i=0; i<el.childNodes.length; i++) {
			if (el.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_iUserID = el.childNodes[i].nodeValue;
			}
		}
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}

}

/* ------------------------------------------------------------ */

function RAWS3_StudentRegistration  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserName;
	var sInstructorEmail;
	var sUserFirstName;
	var sUserLastName;
	var sUserPwd;
	var sVerifyPwd;
	var sUserPwdHint;
	var sRemoteIPAddr;
	var sBaseURL;
	var sOptInEmail = 0;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserName') sUserName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sInstructorEmail') sInstructorEmail = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserFirstName') sUserFirstName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserLastName') sUserLastName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserPwd') sUserPwd = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sVerifyPwd') sVerifyPwd = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sUserPwdHint') sUserPwdHint = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sRemoteIPAddr') sRemoteIPAddr = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sBaseURL') sBaseURL = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sOptInEmail') sOptInEmail = arguments[i].split('=')[1]; 
	}
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <StudentRegistration xmlns="RAStudentRegistration">' + 
'      <sUserName>'+sUserName+'</sUserName>' + 
'      <sInstructorEmail>'+sInstructorEmail+'</sInstructorEmail>' + 
'      <sUserFirstName>'+sUserFirstName+'</sUserFirstName>' + 
'      <sUserLastName>'+sUserLastName+'</sUserLastName>' + 
'      <sUserPwd>'+sUserPwd+'</sUserPwd>' + 
'      <sVerifyPwd>'+sVerifyPwd+'</sVerifyPwd>' + 
'      <sUserPwdHint>'+sUserPwdHint+'</sUserPwdHint>' + 
'      <sRemoteIPAddr>'+sRemoteIPAddr+'</sRemoteIPAddr>' + 
'      <sBaseURL>'+sBaseURL+'</sBaseURL>' + 
'      <sOptInEmail>'+sOptInEmail+'</sOptInEmail>' + 
'    </StudentRegistration>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
if (RA_CtrlWin.RA.dev_check()) prompt('strEnvelope', strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('3_StudentRegistration',strEnvelope);
return true;
}
function RAWS3_StudentRegistration_PROCESS () {
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('StudentRegistrationResult')[0];
//prompt('Result_Node.xml',Result_Node.xml);
		var result_xml_str;
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) result_xml_str = Result_Node.childNodes[i].nodeValue;
		}
		var xmlDoc;
		try //Internet Explorer
		{
			xmlDoc=new ActiveXObject("Microsoft.XMLDOM");
			xmlDoc.async="false";
			xmlDoc.loadXML(result_xml_str);
		}
		catch(e)
		{
			try //Firefox, Mozilla, Opera, etc.
			{
//				xmlDoc=document.implementation.createDocument("","",null);
				var parser=new DOMParser();
				xmlDoc=parser.parseFromString(result_xml_str,"text/xml"); 
			}
			catch(e) {alert(e.message)}
		}
		var el = xmlDoc.getElementsByTagName('iUserID')[0]
		for (var i=0; i<el.childNodes.length; i++) {
			if (el.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_iUserID = el.childNodes[i].nodeValue;
			}
		}
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}

}

/* ------------------------------------------------------------ */

function RAWS3_CheckActivationCode_packages  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sActivationCode;
	var sPackageIDs;
	var iSiteID;
	var iUserID;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sActivationCode') sActivationCode = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sPackageIDs') sPackageIDs = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'iSiteID') iSiteID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
	}
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <CheckActivationCode xmlns="http://tempuri.org/">' + 
'      <sActivationCode>'+sActivationCode+'</sActivationCode>' + 
'      <sPackageIDs>'+sPackageIDs+'</sPackageIDs>' + 
'      <iSiteID>'+iSiteID+'</iSiteID>' + 
'      <iUserID>'+iUserID+'</iUserID>' + 
'    </CheckActivationCode>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//prompt('request',strEnvelope)
	RA_CtrlWin.RAWS_HTTPREQUEST('3_CheckActivationCode_packages',strEnvelope);
return true;
}
function RAWS3_CheckActivationCode_packages_PROCESS () {
//prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() || RA_CtrlWin.RAWS_XML.getElementsByTagName('iPackageID')[0]) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('CheckActivationCodeResult')[0];

		var xEl = Result_Node.getElementsByTagName('iPackageID')[0];
		for (var i=0; i<xEl.childNodes.length; i++) {
			if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_iPackageID = xEl.childNodes[i].nodeValue;
		}

	} else {
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}

}


/* ------------------------------------------------------------ */

function RAWS3_CheckActivationCode  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sActivationCode;
	var sPackageIDs;
	var sSiteIDs = '';
	var iUserID;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sActivationCode') sActivationCode = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sPackageIDs') sPackageIDs = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sSiteIDs') sSiteIDs = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
	}
	var arrSiteIDs = sSiteIDs.split(',');
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <CheckActivationCode xmlns="http://tempuri.org/">' + 
'      <sActivationCode>'+sActivationCode+'</sActivationCode>'
	for (var i=0; i<arrSiteIDs.length; i++) {
	strEnvelope += '' +
'      <iSiteID>'+arrSiteIDs[i]+'</iSiteID>'
	}
	strEnvelope += '' +
'      <iUserID>'+iUserID+'</iUserID>' + 
'    </CheckActivationCode>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//prompt('request',strEnvelope)
	RA_CtrlWin.RAWS_HTTPREQUEST('3_CheckActivationCode',strEnvelope);
return true;
}
function RAWS3_CheckActivationCode_PROCESS () {
//prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	RA_CtrlWin.RAWS_arrSiteIDs = new Array();
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() || RA_CtrlWin.RAWS_XML.getElementsByTagName('iPackageID')[0]) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('CheckActivationCodeResult')[0];

		var xEls = Result_Node.getElementsByTagName('iSiteID');
		for (var j=0; j<xEls.length; j++) {
			for (var i=0; i<xEls[j].childNodes.length; i++) {
				if (xEls[j].childNodes[i].nodeType==3) RA_CtrlWin.RAWS_arrSiteIDs[RA_CtrlWin.RAWS_arrSiteIDs.length] = xEls[j].childNodes[i].nodeValue;
			}
		}

	} else {
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}

}

/* ------------------------------------------------------------ */

function RAWS3_AssignUserCodes  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sActivationCodes;
	var iUserID;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sActivationCodes') sActivationCodes = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
	}
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <AssignUserCodes xmlns="http://tempuri.org/">' + 
'      <sActivationCodes>'+sActivationCodes+'</sActivationCodes>' + 
'      <iUserID>'+iUserID+'</iUserID>' + 
'    </AssignUserCodes>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//prompt('request',strEnvelope)
	RA_CtrlWin.RAWS_HTTPREQUEST('3_AssignUserCodes',strEnvelope);
return true;
}
function RAWS3_AssignUserCodes_PROCESS () {
//prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
//		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('AssignUserCodesResult')[0];
		RA_CtrlWin.RAWS_udtPackages = new Object();
//		var nPs = Result_Node.getElementsByTagName('udtPackage');
		var nPs = RA_CtrlWin.RAWS_XML.getElementsByTagName('udtPackage');
		var xEl;
		for (var iP=0; iP<nPs.length; iP++) {
			xEl = nPs[iP].getElementsByTagName('iPackageID')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) xPID = xEl.childNodes[i].nodeValue;
			}
			RA_CtrlWin.RAWS_udtPackages[xPID] = new Object();
			RA_CtrlWin.RAWS_udtPackages[xPID].PackageID = xPID;
			xEl = nPs[iP].getElementsByTagName('dtExpiration')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtPackages[xPID].Expiration = xEl.childNodes[i].nodeValue;
			}
		}
	} else {
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}

}

/* ------------------------------------------------------------ */

function RAWS1_SiteLogin  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var iUserID;
	var iSiteID;
	var sIPAddr;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'iSiteID') iSiteID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sIPAddr') sIPAddr = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<SiteLogin xmlns=\"http://tempuri.org/\">' +
'		<iUserID>'+iUserID+'</iUserID>' +
'		<iSiteID>'+iSiteID+'</iSiteID>' +
'		<sIPAddr>'+sIPAddr+'</sIPAddr>' +
'	</SiteLogin>' +
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
if (RA_CtrlWin.RA.dev_check()) prompt('',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('1_SiteLogin',strEnvelope);
}
function RAWS1_SiteLogin_PROCESS () {
if (RA_CtrlWin.RA.dev_check()) prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('SiteLoginResult')[0];
//prompt('Result_Node.xml',Result_Node.xml);
		var result_xml_str;
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) result_xml_str = Result_Node.childNodes[i].nodeValue;
		}
		var xmlDoc;
		try //Internet Explorer
		{
			xmlDoc=new ActiveXObject("Microsoft.XMLDOM");
			xmlDoc.async="false";
			xmlDoc.loadXML(result_xml_str);
		}
		catch(e)
		{
			try //Firefox, Mozilla, Opera, etc.
			{
//				xmlDoc=document.implementation.createDocument("","",null);
				var parser=new DOMParser();
				xmlDoc=parser.parseFromString(result_xml_str,"text/xml"); 
			}
			catch(e) {alert(e.message)}
		}
		var el = xmlDoc.getElementsByTagName('sInstructorEmail')[0]
		for (var i=0; i<el.childNodes.length; i++) {
			if (el.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_sInstructorEmail = el.childNodes[i].nodeValue;
			}
		}
		var el = xmlDoc.getElementsByTagName('iLevelOfAccess')[0]
		for (var i=0; i<el.childNodes.length; i++) {
			if (el.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_iLevelOfAccess = el.childNodes[i].nodeValue;
			}
		}

	} else {
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}

/* ------------------------------------------------------------ */

function RAWS1_UpdateSiteLogin  (arguments) {
	
	RA_CtrlWin.RAWS_ClearVars();
	
	var iUserID;
	var iSiteID;
	var sInstructorEmail;
	var sIPAddr;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'iSiteID') iSiteID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sInstructorEmail') sInstructorEmail = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sIPAddr') sIPAddr = arguments[i].split('=')[1]; 
	}
	
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <UpdateSiteLogin xmlns=\"http://tempuri.org/\">' + 
'      <iUserID>'+iUserID+'</iUserID>' + 
'      <iSiteID>'+iSiteID+'</iSiteID>' + 
'      <sInstructorEmail>'+sInstructorEmail+'</sInstructorEmail>' + 
'      <sIPAddr>'+sIPAddr+'</sIPAddr>' + 
'    </UpdateSiteLogin>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
'';

//alert (strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('1_UpdateSiteLogin',strEnvelope);
}
function RAWS1_UpdateSiteLogin_PROCESS () {
//prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('UpdateSiteLoginResult')[0];
//prompt('Result_Node.xml',Result_Node.xml);
		var result_xml_str;
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) result_xml_str = Result_Node.childNodes[i].nodeValue;
		}
		var xmlDoc;
		try //Internet Explorer
		{
			xmlDoc=new ActiveXObject("Microsoft.XMLDOM");
			xmlDoc.async="false";
			xmlDoc.loadXML(result_xml_str);
		}
		catch(e)
		{
			try //Firefox, Mozilla, Opera, etc.
			{
//				xmlDoc=document.implementation.createDocument("","",null);
				var parser=new DOMParser();
				xmlDoc=parser.parseFromString(result_xml_str,"text/xml"); 
			}
			catch(e) {alert(e.message)}
		}
		var el = xmlDoc.getElementsByTagName('sInstructorEmail')[0]
		for (var i=0; i<el.childNodes.length; i++) {
			if (el.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_sInstructorEmail = el.childNodes[i].nodeValue;
			}
		}

	} else {
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}

/* ------------------------------------------------------------ */

function RAWS1_JoinClass  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var iUserID;
	var sClassCode;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sClassCode') sClassCode = arguments[i].split('=')[1]; 
	}
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<JoinClass xmlns=\"http://tempuri.org/\">' + 
'		<iUserID>'+iUserID+'</iUserID>' + 
'		<sClassCode>'+sClassCode+'</sClassCode>' + 
'	</JoinClass>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''

//prompt('strEnvelope', strEnvelope);

	RA_CtrlWin.RAWS_HTTPREQUEST('1_JoinClass',strEnvelope);
}
function RAWS1_JoinClass_PROCESS (arguments) {
//prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('JoinClassResult')[0];
//prompt('Result_Node.xml',Result_Node.xml);
		var result_xml_str;
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) result_xml_str = Result_Node.childNodes[i].nodeValue;
		}
		var xmlDoc;
		try //Internet Explorer
		{
			xmlDoc=new ActiveXObject("Microsoft.XMLDOM");
			xmlDoc.async="false";
			xmlDoc.loadXML(result_xml_str);
		}
		catch(e)
		{
			try //Firefox, Mozilla, Opera, etc.
			{
//				xmlDoc=document.implementation.createDocument("","",null);
				var parser=new DOMParser();
				xmlDoc=parser.parseFromString(result_xml_str,"text/xml"); 
			}
			catch(e) {alert(e.message)}
		}
		var el = xmlDoc.getElementsByTagName('iClassID')[0]
		for (var i=0; i<el.childNodes.length; i++) {
			if (el.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_iClassID = el.childNodes[i].nodeValue;
			}
		}
	} else {
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}


/* ------------------------------------------------------------ */

function RAWS1HCL_EmailLogins (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserEmail;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserEmail') sUserEmail = arguments[i].split('=')[1]; 
	}
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<EmailLogins xmlns=\"http://tempuri.org/\">' + 
'		<sUserEmail>'+sUserEmail+'</sUserEmail>' + 
'	</EmailLogins>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''

//prompt('strEnvelope', strEnvelope);

	RA_CtrlWin.RAWS_HTTPREQUEST('2_EmailLogins',strEnvelope);
}
function RAWS1HCL_EmailLogins_PROCESS () {
//prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('sPasswordHint')[0];
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}

}


/* ------------------------------------------------------------ */

function RAWS3_EmailLogins (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserEmail;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserEmail') sUserEmail = arguments[i].split('=')[1]; 
	}
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<EmailLogins xmlns=\"RAEmailLogins\">' + 
'		<sUserEmail>'+sUserEmail+'</sUserEmail>' + 
'	</EmailLogins>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''

if (RA_CtrlWin.RA.dev_check()) prompt('strEnvelope', strEnvelope);

	RA_CtrlWin.RAWS_HTTPREQUEST('3_EmailLogins',strEnvelope);
}
function RAWS3_EmailLogins_PROCESS () {
if (RA_CtrlWin.RA.dev_check()) prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var result = 'nada';
/*
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('UserAuthenticationResult')[0];
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_iUserID = Result_Node.childNodes[i].nodeValue;
		}
*/
		result = jQuery('EmailLoginsResult', RA_CtrlWin.RAWS_XML).text();
if (RA_CtrlWin.RA.dev_check()) prompt('result',result);
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}

}


/* ------------------------------------------------------------ */

function RAWS3_JoinClass  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var iUserID;
	var sClassCode;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sClassCode') sClassCode = arguments[i].split('=')[1]; 
	}
/* CHAD'S RAWS3ASP VERSION
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<JoinClass xmlns=\"http://tempuri.org/\">' + 
'		<iUserID>'+iUserID+'</iUserID>' + 
'		<sClassCode>'+sClassCode+'</sClassCode>' + 
'	</JoinClass>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
*/
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<JoinClass xmlns=\"RAJoinClass\">' + 
'		<iUserID>'+iUserID+'</iUserID>' + 
'		<sClassCode>'+sClassCode+'</sClassCode>' + 
'	</JoinClass>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''

if (RA_CtrlWin.RA.dev_check()) prompt('strEnvelope', strEnvelope);

	RA_CtrlWin.RAWS_HTTPREQUEST('3_JoinClass',strEnvelope);
}
function RAWS3_JoinClass_PROCESS () {
if (RA_CtrlWin.RA.dev_check()) prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('JoinClassResult')[0];
//prompt('Result_Node.xml',Result_Node.xml);
		var el = Result_Node.getElementsByTagName('iClassID')[0]
		for (var i=0; i<el.childNodes.length; i++) {
			if (el.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_iClassID = el.childNodes[i].nodeValue;
			}
		}
/*
sJoinClassResult = sJoinClassResult &"<udtClassJoined>" &_
	"<iClassID>"& iClassID &"</iClassID>" &_
	"<iCreatorID>"& iCreatorID &"</iCreatorID>" &_
	"<sClassName>"& sClassName &"</sClassName>" &_
	"<sClassDesc>"& sClassDesc &"</sClassDesc>" &_
	"<sClassCode>"& sClassCode &"</sClassCode>" &_
	"<dtExprn>"& dtExprn &"</dtExprn>" &_
	"<iUserID>"& iUserID &"</iUserID>" &_
	"<bClassAccessRevoked>"& bClassAccessRevoked &"</bClassAccessRevoked>" &_
	"<dtLastLogin>"& dtLastLogin &"</dtLastLogin>" &_
	"<dtStartDate>"& dtStartDate &"</dtStartDate>" &_
	"<dtEndDate>"& dtEndDate &"</dtEndDate>" &_
	"<bEmailScores>"& bEmailScores &"</bEmailScores>" &_
	"<iRecordStatus>"& iRecordStatus &"</iRecordStatus>" &_
	"</udtClassJoined>"
*/
	} else {
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}


/* ------------------------------------------------------------ */

function RAWS3_JoinUsertoClass  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var iUserID;
	var iClassID;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'iClassID') iClassID = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<JoinUsertoClass xmlns=\"RAJoinUsertoClass\">' + 
'		<UserID>'+iUserID+'</UserID>' + 
'		<ClassID>'+iClassID+'</ClassID>' + 
'	</JoinUsertoClass>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''

//if (RA_CtrlWin.RA.dev_check()) prompt('strEnvelope', strEnvelope);

	RA_CtrlWin.RAWS_HTTPREQUEST('3_JoinUsertoClass',strEnvelope);
}
function RAWS3_JoinUsertoClass_PROCESS () {
//if (RA_CtrlWin.RA.dev_check()) prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
	} else {
if (RA_CtrlWin.RA.dev_check()) prompt('RA_CtrlWin.RAWS_XML.xml',RA_CtrlWin.RAWS_XML.xml);
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}


/* ------------------------------------------------------------ */

function RAWS3_GetSiteFromBaseURL  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sBaseUrl = new Array();
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sBaseUrl') sBaseUrl[sBaseUrl.length] = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://tempuri.org/\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <GetSiteFromBaseURL xmlns=\"http://tempuri.org/\">';
	for (var i=0; i<sBaseUrl.length; i++) {
		strEnvelope += '' +
'      <sBaseURL>'+ sBaseUrl[i] +'</sBaseURL>';
	}
	strEnvelope += '' +
'    </GetSiteFromBaseURL>' +
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//prompt('RAWS3_GetSiteFromBaseURL',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('3_GetSiteFromBaseURL',strEnvelope);
}

function RAWS3_GetSiteFromBaseURL_PROCESS () {
//alert('RAWS3_GetSiteFromBaseURL_PROCESS');
//prompt('RA_CtrlWin.RAWS_XML',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('GetSiteFromBaseURLResult')[0];

		RA_CtrlWin.RAWS_udtSites = new Object();
		var xEl;
		var nSs = RA_CtrlWin.RAWS_XML.getElementsByTagName('udtSite');
		var xSID;
		var nPs;
		var xPID;
		var nBs;
		var xBID;
		for (var iS=0; iS<nSs.length; iS++) {
			xEl = nSs[iS].getElementsByTagName('iSiteID')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) xSID = new Number(xEl.childNodes[i].nodeValue);
			}
			RA_CtrlWin.RAWS_udtSites[xSID] = new Object();
			RA_CtrlWin.RAWS_udtSites[xSID].SiteID = xSID;
			xEl = nSs[iS].getElementsByTagName('sBaseURL')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].BaseURL = xEl.childNodes[i].nodeValue;
			}

			nPs = nSs[iS].getElementsByTagName('udtPackage');
			RA_CtrlWin.RAWS_udtSites[xSID].Packages = new Object();
			for (var iP=0; iP<nPs.length; iP++) {
				xEl = nPs[iP].getElementsByTagName('iPackageID')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) xPID = new Number(xEl.childNodes[i].nodeValue);
				}
				RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID] = new Object();
				RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].PackageID = xPID;
				xEl = nPs[iP].getElementsByTagName('sDescription')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Description = xEl.childNodes[i].nodeValue;
				}
				xEl = nPs[iP].getElementsByTagName('sType')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Type = xEl.childNodes[i].nodeValue;
				}
				xEl = nPs[iP].getElementsByTagName('iLevelOfAccess')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].LevelOfAccess = new Number(xEl.childNodes[i].nodeValue);
				}

				nBs = nPs[iP].getElementsByTagName('udtBatch');
				RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches = new Object();
				for (var iB=0; iB<nBs.length; iB++) {
					xEl = nBs[iB].getElementsByTagName('iBatchID')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) xBID = new Number(xEl.childNodes[i].nodeValue);
					}
					RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID] = new Object();
					RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].BatchID = xBID;
					xEl = nBs[iB].getElementsByTagName('sDescription')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Description = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('dtUseByDate')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].UseByDate = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('iRelativeExpiration')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].RelativeExpiration = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('dtAbsoluteExpiration')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].AbsoluteExpiration = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('bSuspended')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Suspended = xEl.childNodes[i].nodeValue==1 ? true : false;
					}
					xEl = nBs[iB].getElementsByTagName('sType')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Type = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('mPrice')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Price = new Number(xEl.childNodes[i].nodeValue).toFixed(2);
					}
				}
			}
		}


	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}


/* ------------------------------------------------------------ */

function RAWS3_GetSiteFromBaseURL_WithProducts  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sBaseUrl = new Array();
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sBaseUrl') sBaseUrl[sBaseUrl.length] = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://tempuri.org/\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <GetSiteFromBaseURL xmlns=\"http://tempuri.org/\">';
	for (var i=0; i<sBaseUrl.length; i++) {
		strEnvelope += '' +
'      <sBaseURL>'+ sBaseUrl[i] +'</sBaseURL>';
	}
	strEnvelope += '' +
'    </GetSiteFromBaseURL>' +
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
if (RA_CtrlWin.RA.dev_check('cart')) prompt('RAWS3_GetSiteFromBaseURL_WithProducts',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('3_GetSiteFromBaseURL_WithProducts',strEnvelope);
}

function RAWS3_GetSiteFromBaseURL_WithProducts_PROCESS () {
//alert('RAWS3_GetSiteFromBaseURL_WithProducts_PROCESS');
if (RA_CtrlWin.RA.dev_check()) {
//prompt('RA_CtrlWin.RAWS_XML',RA_CtrlWin.RAWS_XML.xml);
}
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('GetSiteFromBaseURLResult')[0];

		RA_CtrlWin.RAWS_udtSites = new Object();
		var xEl;
		var nSs = RA_CtrlWin.RAWS_XML.getElementsByTagName('udtSite');
		var xSID;
		var nProds;
		var nPs;
		var xPID;
		var nSAs;
		var xSAID;
		var nBs;
		var xBID;
		for (var iS=0; iS<nSs.length; iS++) {
			xEl = nSs[iS].getElementsByTagName('iSiteID')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) xSID = new Number(xEl.childNodes[i].nodeValue);
			}
			RA_CtrlWin.RAWS_udtSites[xSID] = new Object();
			RA_CtrlWin.RAWS_udtSites[xSID].SiteID = xSID;
			xEl = nSs[iS].getElementsByTagName('sBaseURL')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].BaseURL = xEl.childNodes[i].nodeValue;
			}
/*
			xEl = nSs[iS].getElementsByTagName('sData')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Description = xEl.childNodes[i].nodeValue;
			}
*/
			nProds = nSs[iS].getElementsByTagName('udtProduct');
			RA_CtrlWin.RAWS_udtSites[xSID].Products = new Array();
			for (var iP=0; iP<nProds.length; iP++) {
				xPID = RA_CtrlWin.RAWS_udtSites[xSID].Products.length;
				RA_CtrlWin.RAWS_udtSites[xSID].Products[xPID] = new Object();

				xEl = nProds[iP].getElementsByTagName('sProductID')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Products[xPID].ProductID = xEl.childNodes[i].nodeValue;
				}
				xEl = nProds[iP].getElementsByTagName('sType')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Products[xPID].Type = xEl.childNodes[i].nodeValue;
				}
				xEl = nProds[iP].getElementsByTagName('sTitle')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Products[xPID].Title = xEl.childNodes[i].nodeValue;
				}
				xEl = nProds[iP].getElementsByTagName('sTag')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Products[xPID].Tag = xEl.childNodes[i].nodeValue;
				}
				xEl = nProds[iP].getElementsByTagName('sMore')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Products[xPID].LearnMoreLink = xEl.childNodes[i].nodeValue;
				}
//				RA_CtrlWin.RAWS_udtSites[xSID].Products[xPID].Description = 'test description (hard coded)';
				xEl = nProds[iP].getElementsByTagName('sData')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Products[xPID].Data = xEl.childNodes[i].nodeValue;
				}
				switch (RA_CtrlWin.RAWS_udtSites[xSID].Products[xPID].Type) {
				case 'RA CONTENT' :
/*
					xEl = nProds[iP].getElementsByTagName('sBaseURL')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Products[xPID].BaseURL = xEl.childNodes[i].nodeValue;
					}
*/
					break;
				case 'RA SITE' :
/*
					xEl = nProds[iP].getElementsByTagName('sBaseURL')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Products[xPID].BaseURL = xEl.childNodes[i].nodeValue;
					}
*/
					break;
				case 'RA PACKAGE' :
					break;
				default :
				}
			}

			nPs = nSs[iS].getElementsByTagName('udtPackage');
			RA_CtrlWin.RAWS_udtSites[xSID].Packages = new Object();
			for (var iP=0; iP<nPs.length; iP++) {
				xEl = nPs[iP].getElementsByTagName('iPackageID')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) xPID = new Number(xEl.childNodes[i].nodeValue);
				}
				RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID] = new Object();
				RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].PackageID = xPID;
				xEl = nPs[iP].getElementsByTagName('sDescription')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Description = xEl.childNodes[i].nodeValue;
				}
				xEl = nPs[iP].getElementsByTagName('sType')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Type = xEl.childNodes[i].nodeValue;
				}
				nSAs = nPs[iP].getElementsByTagName('udtSiteAssignment');
				RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].SiteAssignments = new Object();
				for (var iSA=0; iSA<nSAs.length; iSA++) {
					xEl = nSAs[iSA].getElementsByTagName('iSiteAssignmentID')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) xSAID = xEl.childNodes[i].nodeValue;
					}
					RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].SiteAssignments[xSAID] = new Object();
					RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].SiteAssignments[xSAID].SiteAssignmentID = xSAID;

					xEl = nSAs[iSA].getElementsByTagName('iSiteID')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].SiteAssignments[xSAID].SiteID = xEl.childNodes[i].nodeValue;
					}
					xEl = nSAs[iSA].getElementsByTagName('sBaseURL')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].SiteAssignments[xSAID].BaseURL = xEl.childNodes[i].nodeValue;
					}
					xEl = nSAs[iSA].getElementsByTagName('sSiteDesc')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].SiteAssignments[xSAID].SiteDesc = xEl.childNodes[i].nodeValue;
					}
					xEl = nSAs[iSA].getElementsByTagName('iLevelOfAccess')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].SiteAssignments[xSAID].LevelOfAccess = xEl.childNodes[i].nodeValue;
					}
/*
					xEl = nSAs[iSA].getElementsByTagName('sSiteDescription')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].SiteAssignments[xSAID].SiteDescription = xEl.childNodes[i].nodeValue;
					}
*/
				}
/*
				xEl = nPs[iP].getElementsByTagName('iLevelOfAccess')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].LevelOfAccess = new Number(xEl.childNodes[i].nodeValue);
				}
				xEl = nPs[iP].getElementsByTagName('iSiteID')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].SiteID = new Number(xEl.childNodes[i].nodeValue);
				}
*/
				nBs = nPs[iP].getElementsByTagName('udtBatch');
				RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches = new Object();
				for (var iB=0; iB<nBs.length; iB++) {
					xEl = nBs[iB].getElementsByTagName('iBatchID')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) xBID = new Number(xEl.childNodes[i].nodeValue);
					}
					RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID] = new Object();
					RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].BatchID = xBID;
					xEl = nBs[iB].getElementsByTagName('sDescription')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Description = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('dtUseByDate')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].UseByDate = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('iRelativeExpiration')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].RelativeExpiration = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('dtAbsoluteExpiration')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].AbsoluteExpiration = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('bSuspended')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Suspended = xEl.childNodes[i].nodeValue==1 ? true : false;
					}
					xEl = nBs[iB].getElementsByTagName('sType')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Type = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('mPrice')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Price = new Number(xEl.childNodes[i].nodeValue).toFixed(2);
					}
				}
			}
		}


	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}


/* ------------------------------------------------------------ */

function RAWS3_RAXSSession  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var iUserID = '';
	var iGUID = '';
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'iGUID') iGUID = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://tempuri.org/\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'<XSSession>'
	if (iUserID!='') {
		strEnvelope += '' +
'	<Set>'+
'		<iUserID>'+ iUserID +'</iUserID>'
'	</Set>'
	} else if (iGUID!='') {
		strEnvelope += '' +
'	<Check>'+
'		<iGUID>'+ iGUID +'</iGUID>'
'	</Check>'
	}
	strEnvelope += '' +
'</XSSession>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
if (RA_CtrlWin.RA.dev_check()) {
//prompt('RAWS3_RAXSSession',strEnvelope);
}
	RA_CtrlWin.RAWS_HTTPREQUEST('3_RAXSSession',strEnvelope);
}

function RAWS3_RAXSSession_PROCESS () {
//alert('RAWS3_RAXSSession_PROCESS');
if (RA_CtrlWin.RA.dev_check()) {
//prompt('RA_CtrlWin.RAWS_XML',RA_CtrlWin.RAWS_XML.xml);
}
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('XSSession')[0];

		var xEl = RA_CtrlWin.RAWS_XML.getElementsByTagName('SetResult');
		if (xEl.length==1) {
			var xGUID = xEl[0].getElementsByTagName('iGUID')[0];
			for (var i=0; i<xGUID.childNodes.length; i++) {
				if (xGUID.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_XSSessionGUID = new Number(xGUID.childNodes[i].nodeValue);
			}
		} else {
			xEl = RA_CtrlWin.RAWS_XML.getElementsByTagName('CheckResult');
			if (xEl.length==1) {
				var xUID = xEl[0].getElementsByTagName('iUserID')[0];
				for (var i=0; i<xUID.childNodes.length; i++) {
					if (xUID.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_iUserID = new Number(xUID.childNodes[i].nodeValue);
				}
			}
		}
	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}


/* ------------------------------------------------------------ */

function RAWS3_GetSitesFromSiteIDs  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sSiteIDs, arrSiteIDs;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sSiteIDs') sSiteIDs = arguments[i].split('=')[1]; 
	}
	arrSiteIDs = sSiteIDs.split(',');
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://tempuri.org/\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <GetSitesFromSiteIDs xmlns=\"http://tempuri.org/\">'
	for (var i=0; i<arrSiteIDs.length; i++) {
		strEnvelope += '' +
'      <iSiteID>'+ arrSiteIDs[i] +'</iSiteID>'
	}
	strEnvelope += '' +
'    </GetSitesFromSiteIDs>' +
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//prompt('RAWS3_GetSitesFromSiteIDs',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('3_GetSitesFromSiteIDs',strEnvelope);
}

function RAWS3_GetSitesFromSiteIDs_PROCESS () {
//alert('RAWS3_GetSitesFromSiteIDs_PROCESS');
//prompt('RA_CtrlWin.RAWS_XML',RA_CtrlWin.RAWS_XML);
//prompt('RA_CtrlWin.RAWS_XML',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('GetSitesFromSiteIDsResult')[0];

		RA_CtrlWin.RAWS_udtSites = new Object();
		var xEl;
		var nSs = RA_CtrlWin.RAWS_XML.getElementsByTagName('udtSite');
		var xSID;
		var nPs;
		var xPID;
		var nBs;
		var xBID;
		for (var iS=0; iS<nSs.length; iS++) {
			xEl = nSs[iS].getElementsByTagName('iSiteID')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) xSID = new Number(xEl.childNodes[i].nodeValue);
			}
			RA_CtrlWin.RAWS_udtSites[xSID] = new Object();
			RA_CtrlWin.RAWS_udtSites[xSID].SiteID = xSID;
			xEl = nSs[iS].getElementsByTagName('sBaseURL')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].BaseURL = xEl.childNodes[i].nodeValue;
			}

			nPs = nSs[iS].getElementsByTagName('udtPackage');
			RA_CtrlWin.RAWS_udtSites[xSID].Packages = new Object();
			for (var iP=0; iP<nPs.length; iP++) {
				xEl = nPs[iP].getElementsByTagName('iPackageID')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) xPID = new Number(xEl.childNodes[i].nodeValue);
				}
				RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID] = new Object();
				RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].PackageID = xPID;
				xEl = nPs[iP].getElementsByTagName('sDescription')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Description = xEl.childNodes[i].nodeValue;
				}
				xEl = nPs[iP].getElementsByTagName('sType')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Type = xEl.childNodes[i].nodeValue;
				}
				xEl = nPs[iP].getElementsByTagName('iLevelOfAccess')[0];
				for (var i=0; i<xEl.childNodes.length; i++) {
					if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].LevelOfAccess = new Number(xEl.childNodes[i].nodeValue);
				}

				nBs = nPs[iP].getElementsByTagName('udtBatch');
				RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches = new Object();
				for (var iB=0; iB<nBs.length; iB++) {
					xEl = nBs[iB].getElementsByTagName('iBatchID')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) xBID = new Number(xEl.childNodes[i].nodeValue);
					}
					RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID] = new Object();
					RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].BatchID = xBID;
					xEl = nBs[iB].getElementsByTagName('sDescription')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Description = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('dtUseByDate')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].UseByDate = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('iRelativeExpiration')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].RelativeExpiration = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('dtAbsoluteExpiration')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].AbsoluteExpiration = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('bSuspended')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Suspended = xEl.childNodes[i].nodeValue==1 ? true : false;
					}
					xEl = nBs[iB].getElementsByTagName('sType')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Type = xEl.childNodes[i].nodeValue;
					}
					xEl = nBs[iB].getElementsByTagName('mPrice')[0];
					for (var i=0; i<xEl.childNodes.length; i++) {
						if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSites[xSID].Packages[xPID].Batches[xBID].Price = new Number(xEl.childNodes[i].nodeValue).toFixed(2);
					}
				}
			}
		}


	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}



/* ------------------------------------------------------------ */

function RAWS3_GetOnyxSchoolsByZip  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var iZipPrefix;
	var sSchoolType;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iZipPrefix') iZipPrefix = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sSchoolType') sSchoolType = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://tempuri.org/\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <GetOnyxSchoolsByZip xmlns=\"http://tempuri.org/\">' +
'      <iZipPrefix>'+ iZipPrefix +'</iZipPrefix>' +
'      <sSchoolType>'+ sSchoolType +'</sSchoolType>' +
'    </GetOnyxSchoolsByZip>' +
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//prompt('RAWS3_GetOnyxSchoolsByZip',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('3_GetOnyxSchoolsByZip',strEnvelope);
}

function RAWS3_GetOnyxSchoolsByZip_PROCESS () {
//alert('RAWS3_GetOnyxSchoolsByZip_PROCESS');
//prompt('RA_CtrlWin.RAWS_XML',RA_CtrlWin.RAWS_XML.xml);
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('GetOnyxSchoolsByZipResult')[0];

		RA_CtrlWin.RAWS_udtSchools = new Array();
		var xEl;
		var nSs = RA_CtrlWin.RAWS_XML.getElementsByTagName('udtSchool');
		var xSID;
		for (var iS=0; iS<nSs.length; iS++) {
			xSID = RA_CtrlWin.RAWS_udtSchools.length;
			RA_CtrlWin.RAWS_udtSchools[xSID] = new Object();

			xEl = nSs[iS].getElementsByTagName('sSchoolName')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSchools[xSID].Name = xEl.childNodes[i].nodeValue;
			}
			xEl = nSs[iS].getElementsByTagName('iSchoolID')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSchools[xSID].ID = xEl.childNodes[i].nodeValue;
			}
			xEl = nSs[iS].getElementsByTagName('sSchoolCountry')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSchools[xSID].Country = xEl.childNodes[i].nodeValue;
			}
			xEl = nSs[iS].getElementsByTagName('sSchoolStateAbbr')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSchools[xSID].StateAbbr = xEl.childNodes[i].nodeValue;
			}
			xEl = nSs[iS].getElementsByTagName('sSchoolCity')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSchools[xSID].City = xEl.childNodes[i].nodeValue;
			}
			xEl = nSs[iS].getElementsByTagName('sSchoolZip')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSchools[xSID].Zip = xEl.childNodes[i].nodeValue;
			}
			xEl = nSs[iS].getElementsByTagName('sSchoolType')[0];
			for (var i=0; i<xEl.childNodes.length; i++) {
				if (xEl.childNodes[i].nodeType==3) RA_CtrlWin.RAWS_udtSchools[xSID].Type = xEl.childNodes[i].nodeValue;
			}
		}


	}

	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}















/* ------------------------------------------------------------ */

function RAWS1_EnterActivationCode  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sActivationCode;
	var iUserID;
	var iSiteID;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sActivationCode') sActivationCode = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'iSiteID') iSiteID = arguments[i].split('=')[1]; 
	}
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <EnterActivationCode xmlns="http://tempuri.org/">' + 
'      <sActivationCode>'+sActivationCode+'</sActivationCode>' + 
'      <iUserID>'+iUserID+'</iUserID>' + 
'      <iSiteID>'+iSiteID+'</iSiteID>' + 
'    </EnterActivationCode>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//prompt('request',strEnvelope)
	RA_CtrlWin.RAWS_HTTPREQUEST('1_EnterActivationCode',strEnvelope);
return true;
}
function RAWS1_EnterActivationCode_PROCESS () {
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('EnterActivationCodeResult')[0];
//prompt('Result_Node.xml',Result_Node.xml);
		var result_xml_str;
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) result_xml_str = Result_Node.childNodes[i].nodeValue;
		}
		var xmlDoc;
		try //Internet Explorer
		{
			xmlDoc=new ActiveXObject("Microsoft.XMLDOM");
			xmlDoc.async="false";
			xmlDoc.loadXML(result_xml_str);
		}
		catch(e)
		{
			try //Firefox, Mozilla, Opera, etc.
			{
//				xmlDoc=document.implementation.createDocument("","",null);
				var parser=new DOMParser();
				xmlDoc=parser.parseFromString(result_xml_str,"text/xml"); 
			}
			catch(e) {alert(e.message)}
		}
		var el = xmlDoc.getElementsByTagName('iUserID')[0]
		for (var i=0; i<el.childNodes.length; i++) {
			if (el.childNodes[i].nodeType==3) {
				RA_CtrlWin.RAWS_iUserID = el.childNodes[i].nodeValue;
			}
		}
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}

}



















/* ------------------------------------------------------------ */

function RAWS1_DeleteClass (arguments) {
}

/* ------------------------------------------------------------ */

function RAWS1_CreateClass (arguments) {
} 

/* ------------------------------------------------------------ */

function RAWS1_ReadContentAssignment  (arguments) {
}

/* ------------------------------------------------------------ */

function RAWS1_AddContentAssignment  (arguments) {
}

/* ------------------------------------------------------------ */

function RAWS1_UserProfile  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var iUserID = 0;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
	}
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<UserProfile xmlns=\"http://tempuri.org/\">' + 
'		<iUserID>'+iUserID+'</iUserID>' + 
'		<sBaseUrl/> ' + 
'	</UserProfile>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//alert(strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('1_UserProfile',strEnvelope);
}
function RAWS1_UserProfile_PROCESS () {
	var x = RA_CtrlWin.RAWS_XML.selectSingleNode('/soap:Envelope/soap:Body/UserProfileResponse/UserProfileResult')
	var y = x.text;
	// create the XML object
	var resultXML = new ActiveXObject('Msxml2.DOMDocument');
	if (resultXML == null) {
		alert('Unable to create DOM document!');
	} else {
		resultXML.loadXML(y);
		if (resultXML.parseError.errorCode != 0) {
			var xmlErr = RA_CtrlWin.RAWS_XML.parseError;
			alert('Error loading RAWS UserProfileResult XML' + xmlErr.reason);
		} else {
			var resultNodes = resultXML.selectSingleNode('/udtUserProfile').childNodes; 
			for (var i=0; i < resultNodes.length; i++) {
				switch (resultNodes(i).nodeName) {
				case 'iUserID':
					RA_CtrlWin.RAWS_iUserID = resultNodes(i).text;
				break;
				case 'sUserName':
					RA_CtrlWin.RAWS_sUserName = resultNodes(i).text;
				break;
				case 'sPassword':
					RA_CtrlWin.RAWS_sPassword = resultNodes(i).text;
				break;
				case 'sFirstName':
					RA_CtrlWin.RAWS_sFirstName = resultNodes(i).text;
				break;
				case 'sLastName':
					RA_CtrlWin.RAWS_sLastName = resultNodes(i).text;
				break;
				case 'sPasswordHint':
					RA_CtrlWin.RAWS_sPasswordHint = resultNodes(i).text;
				break;
				case 'sMailPreference':
					RA_CtrlWin.RAWS_sMailPreference = resultNodes(i).text;
				break;
				case 'sOptInEmail':
					RA_CtrlWin.RAWS_sOptInEmail = resultNodes(i).text;
				break;
				case 'sInstructorEmail':
					RA_CtrlWin.RAWS_sInstructorEmail = resultNodes(i).text;
				break;
				case 'iLevelOfAccess':
					RA_CtrlWin.RAWS_iLevelOfAccess = resultNodes(i).text;
				break;
				default:
				}
			}
		}
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}

/* ------------------------------------------------------------ */

function RAWS1_Logout  (arguments) {
}

/* ------------------------------------------------------------ */

function RAWS1_UpdateUserName  (arguments) {
}

/* ------------------------------------------------------------ */

function RAWS1_CheckEnterActivationCode  (arguments) {
}

/* ------------------------------------------------------------ */

function RAWS1_EditClass  (arguments) {
}

/* ------------------------------------------------------------ */

function RAWS1_GetClass  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var iUserID;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
	}
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'	<GetClass xmlns=\"http://tempuri.org/\">' + 
'		<iUserID>'+iUserID+'</iUserID>' + 
'	</GetClass>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
''

//prompt('strEnvelope', strEnvelope);

	RA_CtrlWin.RAWS_HTTPREQUEST('1_GetClass',strEnvelope);
}
function RAWS1_GetClass_PROCESS () {
	var x = RA_CtrlWin.RAWS_XML.selectSingleNode('/soap:Envelope/soap:Body/GetClassResponse/GetClassResult')
	var y = x.text;
	// create the XML object
	var resultXML = new ActiveXObject('Msxml2.DOMDocument');
	if (resultXML == null) {
		alert('Unable to create DOM document!');
	} else {
		resultXML.loadXML(y);
		if (resultXML.parseError.errorCode != 0) {
			var xmlErr = RA_CtrlWin.RAWS_XML.parseError;
			alert('Error loading RAWS GetClassResult XML' + xmlErr.reason);
		} else {
			var resultNodes = resultXML.selectSingleNode('/ArrayOfUdtClassInfo').childNodes; 
			for (var i=0; i < resultNodes.length; i++) {
				RA_CtrlWin.RAWS_udtClassInfo[i] = new Array;
				var resultNodes2 = resultNodes(i).childNodes
				for (var j=0; j < resultNodes2.length; j++) {
					switch (resultNodes2(j).nodeName) {
					case 'iClassID':
						RA_CtrlWin.RAWS_udtClassInfo[i]['iClassID'] = resultNodes2(j).text;
					break;
					case 'iCreatorID':
						RA_CtrlWin.RAWS_udtClassInfo[i]['iCreatorID'] = resultNodes2(j).text;
					break;
					case 'sClassName':
						RA_CtrlWin.RAWS_udtClassInfo[i]['sClassName'] = resultNodes2(j).text;
					break;
					case 'sClassDesc':
						RA_CtrlWin.RAWS_udtClassInfo[i]['sClassDesc'] = resultNodes2(j).text;
					break;
					case 'sClassCode':
						RA_CtrlWin.RAWS_udtClassInfo[i]['sClassCode'] = resultNodes2(j).text;
					break;
					case 'dtExprn':
						RA_CtrlWin.RAWS_udtClassInfo[i]['dtExprn'] = resultNodes2(j).text;
					break;
					case 'iUserID':
						RA_CtrlWin.RAWS_udtClassInfo[i]['iUserID'] = resultNodes2(j).text;
					break;
					case 'bClassAccessRevoked':
						RA_CtrlWin.RAWS_udtClassInfo[i]['bClassAccessRevoked'] = resultNodes2(j).text;
					break;
					case 'dtLastLogin':
						RA_CtrlWin.RAWS_udtClassInfo[i]['dtLastLogin'] = resultNodes2(j).text;
					break;
					case 'dtStartDate':
						RA_CtrlWin.RAWS_udtClassInfo[i]['dtStartDate'] = resultNodes2(j).text;
					break;
					case 'dtEndDate':
						RA_CtrlWin.RAWS_udtClassInfo[i]['dtEndDate'] = resultNodes2(j).text;
					break;
					case 'bEmailScores':
						RA_CtrlWin.RAWS_udtClassInfo[i]['bEmailScores'] = resultNodes2(j).text;
					break;
					case 'iRecordStatus':
						RA_CtrlWin.RAWS_udtClassInfo[i]['iRecordStatus'] = resultNodes2(j).text;
					break;					
					default:
					}
				}
			}
		}
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}

/* ------------------------------------------------------------ */

function RAWS1_UpdateClass  (arguments) {
}

/* ------------------------------------------------------------ */

function RAWS1_UpdateSiteProfile  (arguments) {
	
	RA_CtrlWin.RAWS_ClearVars();
	
	var iUserID;
	var iSiteID;
	var sInstEmail;
	var sFirstName;
	var sLastName;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'iSiteID') iSiteID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sInstEmail') sInstEmail = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sFirstName') sFirstName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sLastName') sLastName = arguments[i].split('=')[1]; 
	}
	
// REAL !!!
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <UpdateSiteProfile xmlns=\"http://tempuri.org/\">' + 
'      <iUserID>'+iUserID+'</iUserID>' + 
'      <iSiteID>'+iSiteID+'</iSiteID>' + 
'      <sInstEmail>'+sInstEmail+'</sInstEmail>' + 
'      <sFirstName>'+sFirstName+'</sFirstName>' + 
'      <sLastName>'+sLastName+'</sLastName>' + 
'    </UpdateSiteProfile>' + 
'  </soap:Body>' + 
'</soap:Envelope>' + 
'';

//alert (strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('1_UpdateSiteProfile',strEnvelope);
}
function RAWS1_UpdateSiteProfile_PROCESS () {
	var x = RA_CtrlWin.RAWS_XML.selectSingleNode('/soap:Envelope/soap:Body/UpdateSiteProfileResponse/sErrorMsg');
	if (x && x.text!='') {
		RA_CtrlWin.RAWS_error = x.text;
	} else {
		var x = RA_CtrlWin.RAWS_XML.selectSingleNode('/soap:Envelope/soap:Body/UpdateSiteProfileResponse/UpdateSiteProfileResult');
//alert(x.text);
		// create the XML object
		var resultXML = new ActiveXObject('Msxml2.DOMDocument');
		if (resultXML == null) {
			alert('Unable to create DOM document!');
		} else {
		}
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}



function RAWS1_RegisterSite  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sBaseUrl;
	var sSiteDesc;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sBaseUrl') sBaseUrl = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sSiteDesc') sSiteDesc = arguments[i].split('=')[1]; 
	}
}

/* ------------------------------------------------------------ */

function RAWS1_GetClassUsers  (arguments) {
}

/* ------------------------------------------------------------ */

function RAWS1_ClassLogin  (arguments) {
}



// NEW FUNCTIONS NEEDED **********************************************************


/* ------------------------------------------------------------ */

function RAWS1_CheckEmail  (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var sUserName;
	var sPassword;
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'sUserName') sUserName = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sPassword') sPassword = arguments[i].split('=')[1]; 
	}
}





















/* ------------------------------------------------------------ */

function IL_CheckInstructorAccess (arguments) {
	RA_CtrlWin.RAWS_ClearVars();
	var iUserID = 0;
	var sISBN = '';
	for (var i=0; i<arguments.length; i++) {
		if (arguments[i].split('=')[0] == 'iUserID') iUserID = arguments[i].split('=')[1]; 
		if (arguments[i].split('=')[0] == 'sISBN') sISBN = arguments[i].split('=')[1]; 
	}
	var strEnvelope = '' +
'<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"' + 
' xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"' + 
' xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">' + 
'  <soap:Body>' + 
'    <CheckInstructorAccess xmlns=\"http://tempuri.org/webservices/\">' +
'      <uid>'+ iUserID +'</uid>' +
'      <ISBN>'+ sISBN +'</ISBN>' +
'    </CheckInstructorAccess>' +
'  </soap:Body>' + 
'</soap:Envelope>' + 
''
//if (RA_CtrlWin.RA.dev_check()) prompt('strEnvelope',strEnvelope);
	RA_CtrlWin.RAWS_HTTPREQUEST('IL_CheckInstructorAccess',strEnvelope);
}
function IL_CheckInstructorAccess_PROCESS () {
	if ( ! RA_CtrlWin.RAWS_ERROR_PROCESS() ) {
		var Result_Node = RA_CtrlWin.RAWS_XML.getElementsByTagName('CheckInstructorAccessResult')[0];
//prompt('Result_Node.xml',Result_Node.xml);
		var result_xml_str;
		for (var i=0; i<Result_Node.childNodes.length; i++) {
			if (Result_Node.childNodes[i].nodeType==3) result_xml_str = Result_Node.childNodes[i].nodeValue;
		}
		var xmlDoc;
		try //Internet Explorer
		{
			xmlDoc=new ActiveXObject("Microsoft.XMLDOM");
			xmlDoc.async="false";
			xmlDoc.loadXML(result_xml_str);
		}
		catch(e)
		{
			try //Firefox, Mozilla, Opera, etc.
			{
//				xmlDoc=document.implementation.createDocument("","",null);
				var parser=new DOMParser();
				xmlDoc=parser.parseFromString(result_xml_str,"text/xml"); 
			}
			catch(e) {alert(e.message)}
		}
		var el = xmlDoc.getElementsByTagName('sretvalue')[0]
		for (var i=0; i<el.childNodes.length; i++) {
			if (el.childNodes[i].nodeType==3) {
RA_CtrlWin.RAWS_iUserInstructorStatus = el.childNodes[i].nodeValue;
			}
		}

	} else {
//alert(1);
//prompt('ERROR', RA_CtrlWin.RAWS_XML.xml);
	}
	RA_CtrlWin.RAWS_WaitFor_Ready = true;
	try { RA_CtrlWin.RA.RAWS.WaitFor_Ready = true; }catch(e){}
}
