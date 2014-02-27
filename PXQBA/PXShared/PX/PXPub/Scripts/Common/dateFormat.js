
/*
* Date Format 1.2.3
* (c) 2007-2009 Steven Levithan <stevenlevithan.com>
* MIT license
*
* Includes enhancements by Scott Trenda <scott.trenda.net>
* and Kris Kowal <cixar.com/~kris.kowal/>
*
* Accepts a date, a mask, or a date and a mask.
* Returns a formatted version of the given date.
* The date defaults to the current date/time.
* The mask defaults to dateFormat.masks.default.
*/

var dateFormat = function () {
    var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
		timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
		timezoneClip = /[^-+\dA-Z]/g,
		pad = function (val, len) {
		    val = String(val);
		    len = len || 2;
		    while (val.length < len) val = "0" + val;
		    return val;
		};

    // Regexes and supporting functions are cached through closure
    return function (date, mask, utc) {
        var dF = dateFormat;

        // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
        if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
            mask = date;
            date = undefined;
        }

        // Passing date through Date applies Date.parse, if necessary
        date = date ? new Date(date) : new Date;
        //if (isNaN(date)) throw SyntaxError("invalid date");
        function isDate(date) {
            if (date.match(/^\d{4}$/))
                return false;
            var temp = new Date(date);
            if (temp.toString() == "NaN" || temp.toString() == "Invalid Date") {
                throw SyntaxError("invalid date");
            } 
        }

        mask = String(dF.masks[mask] || mask || dF.masks["default"]);

        // Allow setting the utc argument via the mask
        if (mask.slice(0, 4) == "UTC:") {
            mask = mask.slice(4);
            utc = true;
        }

        var _ = utc ? "getUTC" : "get",
			d = date[_ + "Date"](),
			D = date[_ + "Day"](),
			m = date[_ + "Month"](),
			y = date[_ + "FullYear"](),
			H = date[_ + "Hours"](),
			M = date[_ + "Minutes"](),
			s = date[_ + "Seconds"](),
			L = date[_ + "Milliseconds"](),
			o = utc ? 0 : date.getTimezoneOffset(),
			flags = {
			    d: d,
			    dd: pad(d),
			    ddd: dF.i18n.dayNames[D],
			    dddd: dF.i18n.dayNames[D + 7],
			    m: m + 1,
			    mm: pad(m + 1),
			    mmm: dF.i18n.monthNames[m],
			    mmmm: dF.i18n.monthNames[m + 12],
			    yy: String(y).slice(2),
			    yyyy: y,
			    h: H % 12 || 12,
			    hh: pad(H % 12 || 12),
			    H: H,
			    HH: pad(H),
			    M: M,
			    MM: pad(M),
			    s: s,
			    ss: pad(s),
			    l: pad(L, 3),
			    L: pad(L > 99 ? Math.round(L / 10) : L),
			    t: H < 12 ? "a" : "p",
			    tt: H < 12 ? "am" : "pm",
			    T: H < 12 ? "A" : "P",
			    TT: H < 12 ? "AM" : "PM",
			    Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
			    o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
			    S: ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
			};

        return mask.replace(token, function ($0) {
            return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
        });
    };
} ();

// Some common format strings
dateFormat.masks = {
    "default": "ddd mmm dd yyyy HH:MM:ss",
    shortDate: "m/d/yy",
    mediumDate: "mmm d, yyyy",
    longDate: "mmmm d, yyyy",
    fullDate: "dddd, mmmm d, yyyy",
    shortTime: "h:MM TT",
    mediumTime: "h:MM:ss TT",
    longTime: "h:MM:ss TT Z",
    isoDate: "yyyy-mm-dd",
    isoTime: "HH:MM:ss",
    isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
    isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
};

// Internationalization strings
dateFormat.i18n = {
    dayNames: [
		"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
		"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
	],
    monthNames: [
		"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
		"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
	]
};
// For convenience...
Date.prototype.format = function (mask, utc) {
    return dateFormat(this, mask, utc);
};

String.prototype.startsWith = function (prefix) {
    return (this.substr(0, prefix.length) === prefix);
}

//Raise the editor changed event for the editor
function setEditorValueChanged(instance) {
    $(PxPage.switchboard).trigger("editor_value_changed");
}

//This function is called tinyMCE editor instance is initialized
function editorInitialized(ed) {
    //work around to fix the known apostrophe issue with tinyMCE editor
    ed.onSaveContent.add(function (ed, o) {
        o.content = o.content.replace(/&#39/g, "&apos");
    });
}

dateFormat.getDateFromDateTime = function(d, time) {
    if (d.format == null) {
        d = new Date(d);
    }
    var date = new Date(d.format("mm/dd/yyyy") + " " + time); //add time element to date
    return date;
};

dateFormat.isInDst = function(date) {
    var daylightStartTime = new Date($("#TimeZoneDaylightStartTime").val());
    var daylightEndTime = new Date($("#TimeZoneStandardStartTime").val());
    var daylightStartTimeNextYear = new Date($("#TimeZoneDaylightStartTimeNextYear").val());
    var daylightEndTimeNextYear = new Date($("#TimeZoneStandardStartTimeNextYear").val());

    return date > daylightStartTime && date < daylightEndTime ||
        date > daylightStartTimeNextYear && date < daylightEndTimeNextYear;
};


//Coverts date and time from course time to GMT
//ie: 11:59 PM in a PST course => 3:59 AM GMT 
dateFormat.dateConvertToUtc = function (d, time) {
    if (d.format == null) {
        d = new Date(d);
    }
    if (d == null || isNaN(d)) {
        return null;
    }
    if (time == null || time == "") {
        time = d.format(dateFormat.masks.shortTime);
    }
    var MS_PER_MINUTE = 60000;
    
    var date = new Date(d.format("mm/dd/yyyy") + " " + time + " GMT"); //read date as GMT
    var isInDst = dateFormat.isInDst(date);
    var timeZoneOffset = 0;
    if (isInDst) {
        timeZoneOffset = $("#TimeZoneDaylightOffset").val() * MS_PER_MINUTE;
    } else {
        timeZoneOffset = $("#TimeZoneStandardOffset").val() * MS_PER_MINUTE;
    }
    
    date = new Date(date - timeZoneOffset).toUTCString(); //remove timezone offset for course
    return new Date(date);

};

/// Converts time from the course time  to local time (ie: 11:59 PST (8:59 EST) => 11:59 EST)
/// Use when displaying date data rendered from the server as JSON
dateFormat.dateConvertFromCourseTimeZone = function (d) {
    if (d != null && d.format == null) {
        d = new Date(d);
    }
    if (d == null || isNaN(d)) {
        return null;
    }
    if (d.getFullYear() > 1) {
        var MS_PER_MINUTE = 60000;
        
        var isInDst = dateFormat.isInDst(d);
        var timeZoneOffset = 0;
        if (isInDst) {
            timeZoneOffset = $("#TimeZoneDaylightOffset").val() * MS_PER_MINUTE;
        } else {
            timeZoneOffset = $("#TimeZoneStandardOffset").val() * MS_PER_MINUTE;
        }
        //find difference between course Time Zone and User time zone
        timeZoneOffset = -1 * timeZoneOffset - (d.getTimezoneOffset() * MS_PER_MINUTE);
        date = new Date(d - (timeZoneOffset)); //adjust for differences between time zones            
        return new Date(date);
    }
};

dateFormat.jsonDate = function (json) {
    if (json.indexOf("/Date(") >= 0) {
        return new Date(parseInt(json.replace("/Date(", "").replace(")/", ""), 10));
    } else {
        return new Date(json);
    }
};

window.ToggleSolution = function (id, doc) {
    jQuery(doc).find("#sa" + id).toggle();
    var ans = jQuery(doc).find("#sa" + id);
    if ($(ans).is(":visible")) {
        var newHeight = $("#document-body-iframe").height();
        newHeight += $(ans).height();
        $("#document-body-iframe").height(newHeight);
    } else {
        var newHeight = $("#document-body-iframe").height();
        newHeight -= $(ans).height();
        $("#document-body-iframe").height(newHeight);
    }
};


function padZeroes(str, max) {
    return str.length < max ? padZeroes("0" + str, max) : str;
}