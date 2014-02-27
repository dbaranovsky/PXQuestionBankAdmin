var BFW_SERVER_WS_PROXY_TYPE = 'asp';

// **********************************************************************
// **********************************************************************


function BFW_GetCompanyNameHTML (co) {
	var html = '';
	switch (co) {
		case 'WOR' :
			html += 'Worth Publishers';
			break;
		case 'BSM' :
			html += 'Bedford / St. Martin&rsquo;s';
			break;
		case 'WHF' :
			html += 'W. H. Freeman';
			break;
		default :
			html += 'Bedford, Freeman and Worth Publishers';
			break;
	}
	return html;
}


// **********************************************************************
// **********************************************************************
/*
* Perform a replace all on the incoming string 
* 
* @ Shubhranshu
*/
String.prototype.replaceAll = function(
	strTarget, // The substring you want to replace
	strSubString // The string you want to replace in.
	){
	var strText = this;
	var intIndexOfMatch = strText.indexOf( strTarget );
	 
	// Keep looping while an instance of the target string
	// still exists in the string.
	while (intIndexOfMatch != -1){
	// Relace out the current instance.
	strText = strText.replace( strTarget, strSubString )
	 
	// Get the index of any next matching substring.
	intIndexOfMatch = strText.indexOf( strTarget );
	 }
	  
	// Return the updated string with ALL the target strings
	// replaced out with the new substring.
	return( strText );
}


// Parse the url search string into a PHP-like GET array
// id=value pairs get parsed to BFW_QStr['id']=value
// single values get parsed to BFW_QStr['un1']=value
// e.g. 'home.html?blah&foo=bar' will be parsed to:
//    BFW_QStr['un0'] = 'blah'
//    BFW_QStr['foo'] = 'bar'
var BFW_QStr;
function ParseGet(win) {
	if (win == null) {
		win = this;
	}
	BFW_QStr = new Object();
	
	var getA = win.location.search.substr(1).split('&');
	for (var i = 0; i < getA.length; ++i) {
		var getI = getA[i].split('=');
		if (getI.length == 2) {
			BFW_QStr[getI[0]] = decodeURIComponent(getI[1]);
		} else {
			BFW_QStr['un' + i] = decodeURIComponent(getA[i]);
		}
	}
}
ParseGet();



// *******************************************************************
// *******************************************************************
// ccDebugger
// *******************************************************************

// Constructor
function ccDebugger (level) {
	if (Number(level)==NaN) level = 0;
	this.reportMsgsLevel = level;
}

ccDebugger.prototype.Alert = function (level,msg) {
	if (this.reportMsgsLevel >= level) alert('Debug : ' + msg);
}

ccDebugger.prototype.Prompt = function (level,msg) {
	if (this.reportMsgsLevel >= level) prompt('Debug : ', msg);
}







