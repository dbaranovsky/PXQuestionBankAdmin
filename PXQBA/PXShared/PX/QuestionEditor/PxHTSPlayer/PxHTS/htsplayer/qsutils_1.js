// Netscape's Javascript escape implementation doesn't escape spaces
// causes a parameter error when used in querystring
// This function will escape the string including spacses
// as well as Javascript escape single and double quotes
function _escape(str)
{
  var escstr = new String(str);

  // Unencode quotes so escape function doesn't re-encode later (only Netscape encodes them again IE doesn't)
  escstr = escstr.replace(/\%22/ig,"\"");
  escstr = escstr.replace(/\%27/ig,"'");

  
  escstr = escape(escstr);

  //!PM to escape + to %2B
  escstr = escstr.replace(/\+/g,"%2B");  

  // spaces to pluses
  escstr = escstr.replace(/\s/ig,"+");
  //escstr = escstr.replace(/\%20/g,"+");

  // NS 6.2 on MACK needs to have forward slashes escaped when going to a page that contains java applet
  escstr.replace(/\//ig,"%2F");

return escstr;
}



// Javascript unescape doesn't replace '+' with ' ' so we add that
function _unescape(str)
{
  var re = /\+/g;
  var newstr=str.replace(re, " ");
  return unescape(newstr);
}

// Retrieves the value of a paramater in the reqstr
function getParam(reqstr,param)
{
  var evstr, regexp, tmpstr, found;

  tmpstr = new String(reqstr);

  evstr = "regexp = /[\\?|\\&]"+param+"=([^\\&]+)/;"
  eval(evstr);
  found = tmpstr.match(regexp);
  // our parameter is actually the second element of the array of matches, the first is the whole match
  if (found && found[1]) 
    return _unescape(found[1]);
  else
    return false;
}

// Remove parameter from reqstr
function removeParam(reqstr,param)
{
  var evstr, dest;
  dest = new String(reqstr); 

  // strip out existing parameter
  //FOUND A BUG MAD 10/25/00
  //Routine did not handle "param=" at end of string
  //evstr = "dest = dest.replace(/[\\?|\\&]"+param+"=[^\\&]+/ig,\"\");"

  //FOUND A BUG AAL 03/27/03
  //Routine didn't handle removing empty param
  //evstr = "dest = dest.replace(/[\\?|\\&]"+param+"=(([^\\&]+)|\s*$)/ig,\"\");"  
  
  evstr = "dest = dest.replace(/[\\?|\\&]"+param+"=(([^\\&]*)|\s*$)/ig,\"\");"
  eval(evstr);

  // if we stripped out the first param, make the following param have the ampersand
  if (dest.indexOf("?")==-1 && dest.indexOf("&")!=-1)
    dest = dest.replace(/\&/,"?");

  return dest;
}

// Remove parameter that starts with this beginning from reqstr
function removeParamSubStr(reqstr,param)
{
  var evstr, dest;
  dest = new String(reqstr); 

  // strip out existing parameter
  evstr = "dest = dest.replace(/[\\?|\\&]"+param+"[a-zA-Z0-9]*=(([^\\&]*)|\s*$)/ig,\"\");"
  eval(evstr);

  // if we stripped out the first param, make the following param have the ampersand
  if (dest.indexOf("?")==-1 && dest.indexOf("&")!=-1)
    dest = dest.replace(/\&/,"?");

  return dest;
}

// If parameter exists in reqstr, change its value
// If not, add it
function replaceParam(reqstr,param,value)
{
  var evstr, dest;
  dest = new String(reqstr); 

  // strip out existing parameter
  dest = removeParam(dest,param)

  if (dest.indexOf("?")>=0)
    dest=dest+"&"+param+"="+value;
  else
    dest=dest+"?"+param+"="+value;

  return dest;
}

function js_write(text) {
  document.write(text); 
}


// hack for satisfactory funtioning in IE. Not required for Firefox. 
function clearObjs(obj)
{
    var theObj = eval(obj);
    theObj.style.display = "none";
    for (var prop in theObj)
	{
	    if (typeof(theObj[prop]) == "function")
	    {
		    theObj[prop]=null
	    }
	}	
  }

// hack for satisfactory funtioning in IE. Not required for Firefox.
function cleanup() 
{
    __flash_unloadHandler = function()
    {
	externalProbSet = true;
        if (externalProbSet) {return};
	clearObjs(explorer);
	clearObjs(flashcontent);
	if (__flash_savedUnloadHandler != null)
	{
	    __flash_savedUnloadHandler();
	}
    }

    if (window.onunload != __flash_unloadHandler)
    { 
	__flash_savedUnloadHandler = window.onunload;
	window.onunload = __flash_unloadHandler;
    }
}

// hack for satisfactory funtioning in IE. Not required for Firefox.
window.onbeforeunload=cleanup;