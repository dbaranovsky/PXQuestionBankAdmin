// The following section handles the initial redirect from the login, prioritizing the requesturl if set, then falling back to lastitem
var currentUrl = '' + window.location.href;
currentUrl = currentUrl.toLowerCase();
var requestUrl = trim(get_cookie('RequestUrl'));

if (currentUrl.indexOf('login') == -1 && requestUrl.length > 0) {
    delete_cookie('RequestUrl', '/', '');
    if (requestUrl.indexOf("/item/") >= 0 && requestUrl.indexOf("#") >= 0) {
        var redirItem = requestUrl.split("#");
        var redirHostPath = requestUrl.split("/item/");
        if (redirItem.length == 2) {
            var redirTemp = trim(redirItem[1]);
            if (redirTemp.length > 0) {
                window.location = redirHostPath[0] + '/item/' + redirItem[1];
            }
        }
    }
    window.location = requestUrl;
} // end redirect from the login section

// handle loading content from direct access
var locationHref = window.location.href.toLowerCase();
if (locationHref.indexOf("/item/") >= 0 && locationHref.indexOf("#") >= 0) {
    var redirLocation = locationHref.split("#");
    if (redirLocation.length == 2) {
        if ($.trim(redirLocation[1]).length > 0 && $.trim(redirLocation[0]) != $.trim(redirLocation[0])) {
            window.location = redirLocation[1];
        }
    }
}

// utility functions
function trim(s) {
    var l = 0; var r = s.length - 1;
    while (l < s.length && s[l] == ' ') { l++; }
    while (r > l && s[r] == ' ') { r -= 1; }
    return s.substring(l, r + 1);
}

function stripTrailingSlash(str) {
    var rv = "" + str;
    return rv.replace(/\/$/, "");
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}

// cookie handling functions
function get_cookie(name) {
    var search = name + "=";
    var returnvalue = "";
    if (document.cookie.length > 0) {
        var offset = document.cookie.indexOf(search);
        if (offset != -1) {
            offset += search.length
            var end = document.cookie.indexOf(";", offset);
            if (end == -1) end = document.cookie.length;
            returnvalue = unescape(document.cookie.substring(offset, end));
        }
    }
    return returnvalue;
}

function delete_cookie(name, path, domain) {
    if (get_cookie(name)) document.cookie = name + "=" +
        ((path) ? ";path=" + path : "") +
        ((domain) ? ";domain=" + domain : "") +
        ";expires=Thu, 01-Jan-1970 00:00:01 GMT";
}

function set_cookie(cookie_name, cookie_value, cookie_life, cookie_path) {
    var category = '' + getParameterByName('category');
    if (category.length > 0) {
        var qpath = cookie_value.split("?");
        cookie_value = qpath[0] + '?category=' + category;
    }
    var today = new Date();
    var expiry = new Date(today.getTime() + cookie_life * 24 * 60 * 60 * 1000);
    if (cookie_value != null && cookie_value != "") {
        var cookie_string = cookie_name + "=" + escape(cookie_value);
        
        if (cookie_life && cookie_life != '0') {
            cookie_string += "; expires=" + expiry.toGMTString();
        }
        
        if (cookie_path) {
            cookie_string += "; path=" + cookie_path;
        }
        document.cookie = cookie_string;
    }
}

function set_cookieLastItem(cookie_name, cookie_value, cookie_life, cookie_path) {
    var theUrl = window.location.href;
    var theSplit = theUrl.indexOf('#') + 1;
    if (theSplit > 0) {
        cookie_value = theUrl.substring(theSplit);
    } else {

    }

    var today = new Date();
    var expiry = new Date(today.getTime() + cookie_life * 24 * 60 * 60 * 1000);
    if (cookie_value != null && cookie_value != "") {
        var cookie_string = cookie_name + "=" + escape(cookie_value);
        if (cookie_life) { cookie_string += "; expires=" + expiry.toGMTString() }
        if (cookie_path) {
            cookie_string += "; path=" + cookie_path;
        }
        document.cookie = cookie_string;
    }
} //end cooking handling functions