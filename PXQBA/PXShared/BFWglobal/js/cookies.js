if (false) {
var chad = '- =;\n\t----\f\r\u00A0\u2028\u2029-';
// %20 %3D %3B %0A %09
var chad2 = escapeCookie(chad);
var chad3 = unescapeCookie(chad2);
alert(chad+'\n'+chad2+'\n'+chad3);
}

function unescapeCookie( value ) {
	try {
		value = value.toString();
		return value.replace(/(%20)/g, ' ').replace(/(%3D)/g,'=').replace(/(%3B)/g,';').replace(/(%0A)/g,'\n').replace(/(%09)/g,'\t')
	}catch(e){
//		alert(value +' - '+ e.message);
		return '';
	}
}

function escapeCookie( value ) {
	try {
		value = value.toString();
		value = value.replace(/[\f\r\v\u00A0\u2028\u2029]/g,'');
		return value.replace(/ /g, '%20').replace(/=/g,'%3D').replace(/;/g,'%3B').replace(/\n/g,'%0A').replace(/\t/g,'%09')
	}catch(e){
//		alert(value +' - '+ e.message);
		return '';
	}
}

function setCookie2( c_name,value,expiredays,expirehours ) {
	var exdate=new Date();
	exdate.setDate(exdate.getDate()+((expiredays==null) ? 0 : expiredays));
	exdate.setHours(exdate.getHours()+((expirehours==null) ? 0 : expirehours));
	var cookie_string = c_name + '=' + escapeCookie ( value );
	cookie_string += ((expiredays==null) ? '' : ';expires='+exdate.toGMTString());
	document.cookie = cookie_string;
}

function getCookie2( c_name ) {
	if (document.cookie.length>0) {
		c_start=document.cookie.indexOf(c_name + '=');
		if (c_start!=-1) { 
			c_start=c_start + c_name.length+1; 
			c_end=document.cookie.indexOf(';',c_start);
			if (c_end==-1) c_end=document.cookie.length;
			return unescapeCookie(document.cookie.substring(c_start,c_end));
		} 
	}
	return '';
}



function setCookie( c_name,value,expiredays,expirehours ) {
	var exdate=new Date();
	exdate.setDate(exdate.getDate()+((expiredays==null) ? 0 : expiredays));
	exdate.setHours(exdate.getHours()+((expirehours==null) ? 0 : expirehours));
	var cookie_string = c_name + '=' + escape ( value );
	cookie_string += ((expiredays==null) ? '' : ';expires='+exdate.toGMTString());
	document.cookie = cookie_string;
}

function getCookie( c_name ) {
	if (document.cookie.length>0) {
		c_start=document.cookie.indexOf(c_name + '=');
		if (c_start!=-1) { 
			c_start=c_start + c_name.length+1; 
			c_end=document.cookie.indexOf(';',c_start);
			if (c_end==-1) c_end=document.cookie.length;
			return unescape(document.cookie.substring(c_start,c_end));
		} 
	}
	return '';
}

function deleteCookie ( c_name )
{
  document.cookie = c_name += '=; expires=01-Jan-1970 00:00:01 GMT';
}




// EXAMPLE FROM http://www.elated.com/articles/javascript-and-cookies/

function set_cookie ( name, value, exp_y, exp_m, exp_d, path, domain, secure )
{
	var cookie_string = name + '=' + escape ( value );

	if ( exp_y )
	{
		var expires = new Date ( exp_y, exp_m, exp_d );
		cookie_string += '; expires=' + expires.toGMTString();
	}

	if ( path )
		cookie_string += '; path=' + escape ( path );

	if ( domain )
		cookie_string += '; domain=' + escape ( domain );

	if ( secure )
		cookie_string += '; secure';

	document.cookie = cookie_string;
}
