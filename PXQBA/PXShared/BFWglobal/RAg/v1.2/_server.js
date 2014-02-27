function RAXS_GetURL_RootDomain () {
	var returl = '';
	switch (window.location.host) {
	case '192.168.77.114' :
		returl = '192.168.77.242';
		break;
	case '192.168.77.242' :
		returl = '192.168.77.242';
		break;
	case '192.168.77.243' :
		returl = '192.168.77.242';
		break;
	case '192.168.77.244' :
		returl = '192.168.77.242';
		break;
	case '192.168.77.245' :
		returl = '192.168.77.242';
		break;
	case 'dev-gradebook.bfwpub.com' :
		returl = '192.168.77.242';
		break;
	case 'dev-scorecard.bfwpub.com' :
		returl = '192.168.77.242';
		break;
	case 'stg-gradebook.bfwpub.com' :
		returl = '192.168.77.242';
		break;
	case 'stg-scorecard.bfwpub.com' :
		returl = '192.168.77.242';
		break;
	default :
		returl = 'bcs.bfwpub.com';
	}
	return returl;
}
function RAXS_GetURL_RAXS () {
	var returl = RAXS_GetURL_RootDomain();
	returl += '/RA/RAXS/v1.2'
	return returl;
}

function RAXS_GetURL_OldLoginRef () {
	var returl = RAXS_GetURL_RootDomain();
	returl += '/login_reference';
	returl = 'bcs.bfwpub.com/login_reference';
	return returl;
}

