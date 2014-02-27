/**************************************
SystemCheck.js

Check to see if the user's browser is supported, and if the necessary functionality/plugins are available for content that uses Flash and/or Brightcove video.  
We then inject a div in the lower-right corner of the screen with the results of the system check, along with a link to the full system requirements.

This code is designed to be included on the "Login page" of PlatformX.
This code assumes that jquery has been included on the page.

We assume by default that Brightcove video and Flash are necessary. If either of these are not necessary, include one or both of the following javascript snippets anywhere on the login page (the system check code will run on window load):

PDX_System_Check.brightcove_required = false;
PDX_System_Check.flash_required = false;

author: Pepper Williams
version 1.0

**************************************/

//Updated by: Marc Nash
//Added to PX library

// define "console" so that explorer won't choke if we include a console.log
if (window.console == null) {
	window.console = {log:function() {}}
}

// http://www.quirksmode.org/js/detect.html
var BrowserDetect = {
	init: function () {
		this.browser = this.searchString(this.dataBrowser) || "An unknown browser";
		this.version = this.searchVersion(navigator.userAgent)
			|| this.searchVersion(navigator.appVersion)
			|| "an unknown version";
		this.OS = this.searchString(this.dataOS) || "an unknown OS";
	},
	searchString: function (data) {
		for (var i=0;i<data.length;i++)	{
			var dataString = data[i].string;
			var dataProp = data[i].prop;
			this.versionSearchString = data[i].versionSearch || data[i].identity;
			if (dataString) {
				if (dataString.indexOf(data[i].subString) != -1)
					return data[i].identity;
			}
			else if (dataProp)
				return data[i].identity;
		}
	},
	searchVersion: function (dataString) {
		var index = dataString.indexOf(this.versionSearchString);
		if (index == -1) return;
		return parseFloat(dataString.substring(index+this.versionSearchString.length+1));
	},
	dataBrowser: [
		{
			string: navigator.userAgent,
			subString: "Chrome",
			identity: "Chrome"
		},
		{ 	string: navigator.userAgent,
			subString: "OmniWeb",
			versionSearch: "OmniWeb/",
			identity: "OmniWeb"
		},
		{
			string: navigator.vendor,
			subString: "Apple",
			identity: "Safari",
			versionSearch: "Version"
		},
		{
			prop: window.opera,
			identity: "Opera",
			versionSearch: "Version"
		},
		{
			string: navigator.vendor,
			subString: "iCab",
			identity: "iCab"
		},
		{
			string: navigator.vendor,
			subString: "KDE",
			identity: "Konqueror"
		},
		{
			string: navigator.userAgent,
			subString: "Firefox",
			identity: "Firefox"
		},
		{
			string: navigator.vendor,
			subString: "Camino",
			identity: "Camino"
		},
		{		// for newer Netscapes (6+)
			string: navigator.userAgent,
			subString: "Netscape",
			identity: "Netscape"
		},
		{       //for MSIE
			string: navigator.userAgent,
			subString: "Trident",
			identity: "Explorer",
			versionSearch: "Trident"
		},
		{
			string: navigator.userAgent,
			subString: "Gecko",
			identity: "Mozilla",
			versionSearch: "rv"
		},
		{ 		// for older Netscapes (4-)
			string: navigator.userAgent,
			subString: "Mozilla",
			identity: "Netscape",
			versionSearch: "Mozilla"
		}
	],
	dataOS : [
		{
			string: navigator.platform,
			subString: "Win",
			identity: "Windows"
		},
		{
			string: navigator.platform,
			subString: "Mac",
			identity: "Mac"
		},
		{
			   string: navigator.userAgent,
			   subString: "iPhone",
			   identity: "iPhone/iPod"
	    },
		{
			string: navigator.platform,
			subString: "Linux",
			identity: "Linux"
		}
	]

};

// http://www.featureblend.com/javascript-flash-detection-library.html
/*
Copyright (c) Copyright (c) 2007, Carl S. Yestrau All rights reserved.
Code licensed under the BSD License: http://www.featureblend.com/license.txt
Version: 1.0.4
*/
var FlashDetect = new function(){
    var self = this;
    self.installed = false;
    self.raw = "";
    self.major = -1;
    self.minor = -1;
    self.revision = -1;
    self.revisionStr = "";
    var activeXDetectRules = [
        {
            "name":"ShockwaveFlash.ShockwaveFlash.7",
            "version":function(obj){
                return getActiveXVersion(obj);
            }
        },
        {
            "name":"ShockwaveFlash.ShockwaveFlash.6",
            "version":function(obj){
                var version = "6,0,21";
                try{
                    obj.AllowScriptAccess = "always";
                    version = getActiveXVersion(obj);
                }catch(err){}
                return version;
            }
        },
        {
            "name":"ShockwaveFlash.ShockwaveFlash",
            "version":function(obj){
                return getActiveXVersion(obj);
            }
        }
    ];
    /**
     * Extract the ActiveX version of the plugin.
     * 
     * @param {Object} The flash ActiveX object.
     * @type String
     */
    var getActiveXVersion = function(activeXObj){
        var version = -1;
        try{
            version = activeXObj.GetVariable("$version");
        }catch(err){}
        return version;
    };
    /**
     * Try and retrieve an ActiveX object having a specified name.
     * 
     * @param {String} name The ActiveX object name lookup.
     * @return One of ActiveX object or a simple object having an attribute of activeXError with a value of true.
     * @type Object
     */
    var getActiveXObject = function(name){
        var obj = -1;
        try{
            obj = new ActiveXObject(name);
        }catch(err){
            obj = {activeXError:true};
        }
        return obj;
    };
    /**
     * Parse an ActiveX $version string into an object.
     * 
     * @param {String} str The ActiveX Object GetVariable($version) return value. 
     * @return An object having raw, major, minor, revision and revisionStr attributes.
     * @type Object
     */
    var parseActiveXVersion = function(str){
        var versionArray = str.split(",");//replace with regex
        return {
            "raw":str,
            "major":parseInt(versionArray[0].split(" ")[1], 10),
            "minor":parseInt(versionArray[1], 10),
            "revision":parseInt(versionArray[2], 10),
            "revisionStr":versionArray[2]
        };
    };
    /**
     * Parse a standard enabledPlugin.description into an object.
     * 
     * @param {String} str The enabledPlugin.description value.
     * @return An object having raw, major, minor, revision and revisionStr attributes.
     * @type Object
     */
    var parseStandardVersion = function(str){
        var descParts = str.split(/ +/);
        var majorMinor = descParts[2].split(/\./);
        var revisionStr = descParts[3];
        return {
            "raw":str,
            "major":parseInt(majorMinor[0], 10),
            "minor":parseInt(majorMinor[1], 10), 
            "revisionStr":revisionStr,
            "revision":parseRevisionStrToInt(revisionStr)
        };
    };
    /**
     * Parse the plugin revision string into an integer.
     * 
     * @param {String} The revision in string format.
     * @type Number
     */
    var parseRevisionStrToInt = function(str){
        return parseInt(str.replace(/[a-zA-Z]/g, ""), 10) || self.revision;
    };
    /**
     * Is the major version greater than or equal to a specified version.
     * 
     * @param {Number} version The minimum required major version.
     * @type Boolean
     */
    self.majorAtLeast = function(version){
        return self.major >= version;
    };
    /**
     * Is the minor version greater than or equal to a specified version.
     * 
     * @param {Number} version The minimum required minor version.
     * @type Boolean
     */
    self.minorAtLeast = function(version){
        return self.minor >= version;
    };
    /**
     * Is the revision version greater than or equal to a specified version.
     * 
     * @param {Number} version The minimum required revision version.
     * @type Boolean
     */
    self.revisionAtLeast = function(version){
        return self.revision >= version;
    };
    /**
     * Is the version greater than or equal to a specified major, minor and revision.
     * 
     * @param {Number} major The minimum required major version.
     * @param {Number} (Optional) minor The minimum required minor version.
     * @param {Number} (Optional) revision The minimum required revision version.
     * @type Boolean
     */
    self.versionAtLeast = function(major){
        var properties = [self.major, self.minor, self.revision];
        var len = Math.min(properties.length, arguments.length);
        for(i=0; i<len; i++){
            if(properties[i]>=arguments[i]){
                if(i+1<len && properties[i]==arguments[i]){
                    continue;
                }else{
                    return true;
                }
            }else{
                return false;
            }
        }
    };
    /**
     * Constructor, sets raw, major, minor, revisionStr, revision and installed public properties.
     */
    self.FlashDetect = function(){
        if(navigator.plugins && navigator.plugins.length>0){
            var type = 'application/x-shockwave-flash';
            var mimeTypes = navigator.mimeTypes;
            if(mimeTypes && mimeTypes[type] && mimeTypes[type].enabledPlugin && mimeTypes[type].enabledPlugin.description){
                var version = mimeTypes[type].enabledPlugin.description;
                var versionObj = parseStandardVersion(version);
                self.raw = versionObj.raw;
                self.major = versionObj.major;
                self.minor = versionObj.minor; 
                self.revisionStr = versionObj.revisionStr;
                self.revision = versionObj.revision;
                self.installed = true;
            }
        }else if(navigator.appVersion.indexOf("Mac")==-1 && window.execScript){
            var version = -1;
            for(var i=0; i<activeXDetectRules.length && version==-1; i++){
                var obj = getActiveXObject(activeXDetectRules[i].name);
                if(!obj.activeXError){
                    self.installed = true;
                    version = activeXDetectRules[i].version(obj);
                    if(version!=-1){
                        var versionObj = parseActiveXVersion(version);
                        self.raw = versionObj.raw;
                        self.major = versionObj.major;
                        self.minor = versionObj.minor; 
                        self.revision = versionObj.revision;
                        self.revisionStr = versionObj.revisionStr;
                    }
                }
            }
        }
    }();
};
FlashDetect.JS_RELEASE = "1.0.4";

////////////////////////////////////

var PDX_System_Check = function() {
	var html5_video_enabled = false;
	var flash_installed = true;
	var flash_version = 0;
	
	var flash_href = "http://adobe.com/flashplayer";
	
	var sysreq_href = "http://cmg.screenstepslive.com/s/3918/m/11562/l/121193-what-are-the-minimum-system-requirements-for-macmillan-media"
	
	var flashing = 1;
	var details_showing = false;
	
	var underconstruction_html = "<div style='position:absolute;left:50%;top:0'><div style='position:absolute;left:-175px;top:0'><img src='/BrainHoney/Resource/6712/assets/pdx_files/underconstruction.png' width='350' height='150' /></div></div>";
	
	// PRIVATE VARS AND FUNCTIONS
	var div_style = "position:absolute;"
		+ "bottom:20px;"
		+ "right:20px;"
		+ "width:%width%;"
		+ "background-color:%bgdcolor%;"
		+ "border:4px solid %color%;"
		+ "border-radius:10px;"
		+ "color:#333;"
		+ "padding:6px 8px;"
		+ "font-family:Verdana;"
		+ "font-size:12px;"
		+ "line-height:1.3em;"
		;
	
	function line(s) {
		return "<div style='margin-left:20px;text-indent:-15px'>&bull; " + s + "</div>";
	}
	
	// PUBLIC VARS AND FUNCTIONS
	return {
	
	// the inluding page should set these to true if they're required.
	brightcove_required: false,
	flash_required: false,
	
	// set this to true if the site is under construction
	underconstruction: false,
	
	report: function() {
		var html = "";
		
		var all_clear = true;

		// check for browser
		var default_good_line = line("Browser: Supported (" + BrowserDetect.browser + " v" + BrowserDetect.version + ")");
		var default_bad_line = line("Browser: Not optimized (" + BrowserDetect.browser + " v" + BrowserDetect.version + ")<br /><span style='font-size:smaller'>LaunchPad is not optimized for this version of your browser. Please upgrade to the most recent version.");
		
		// chrome: warn if < 28
		if (BrowserDetect.browser == "Chrome") {
			if (BrowserDetect.version >= 28) {
				html += default_good_line;
			} else {
				html += default_bad_line;
				all_clear = false;
			}
		
		// Safari: warn if < 6
		} else if (BrowserDetect.browser == "Safari") {
		    if (BrowserDetect.version >= 6 && BrowserDetect.OS !== "Windows") {
		        html += default_good_line;
		    } else if (BrowserDetect.version >= 5.1 && BrowserDetect.OS === "Windows") {
		        html += default_good_line;
		    } else {
				html += default_bad_line;
				all_clear = false;
			}
		
		// Firefox: warn if < 13
		} else if (BrowserDetect.browser == "Firefox") {
			if (BrowserDetect.version >= 13) {
				html += default_good_line;
			} else {
				html += default_bad_line;
				all_clear = false;
			}

		// Explorer: warn if < version 9
		} else if (BrowserDetect.browser == "Explorer") {
		    var vers = BrowserDetect.version + 4;
		    if (BrowserDetect.version >= 5 && BrowserDetect.version < 7) {
		        html += line("Browser: Supported (" + BrowserDetect.browser + " v" + vers + ")");
			} else if (BrowserDetect.version == 4) {
				html += line("<b style='color:%color%'>Browser: Not optimized</b><br /><span style='font-size:smaller'>LaunchPad is not optimized for your browser (Internet Explorer version 8). Please upgrade to a more recent version of Internet Explorer, or use Chrome, Safari, or FireFox, for optimal performance.</span>");
				all_clear = false;
			} else if (BrowserDetect.version == 7) {
			    html += line("<b style='color:%color%'>Browser: Not optimized</b><br /><span style='font-size:smaller'>LaunchPad is not optimized for your browser (Internet Explorer version 11). Please use Chrome, Safari, or FireFox, for optimal performance.</span>");
			    all_clear = false;
			} else {
				html += line("<b style='color:%color%'>Browser: Not supported</b><br /><span style='font-size:smaller'>LaunchPad is not designed to work with your browser (Internet Explorer version 7 or below). Please upgrade to a more recent version of Internet Explorer, or use Chrome, Safari, or FireFox, for optimal performance.</span>");
				all_clear = false;
			}

		// other browsers: warn
		} else {
			html += line("<b style='color:%color%'>Browser: Not supported</b><br /><span style='font-size:smaller'>LaunchPad is not designed to work with your browser (" + BrowserDetect.browser + " v" + BrowserDetect.version + "). Please use the latest version of Chrome, Safari, FireFox, or Internet Explorer for optimal performance.</span>");
			all_clear = false;
		}
		
		// brightcove (HTML5 or flash)
		if (this.brightcove_required == true) {
			if (html5_video_enabled == true) {
				html += line("Video: Supported (HTML5)");
			
			// Brightcove requires flash 10.2 or above -- http://support.brightcove.com/en/video-cloud/docs/video-cloud-system-requirements
			} else if (flash_installed == true && flash_version > 10.2) {
				html += line("Video: Supported (via Flash)");
			} else {
				html += line("<b style='color:%color%'>Video: Not supported</b><br /><span style='font-size:smaller'>Your browser does not appear to have the capability to play LaunchPad videos. Please install the latest version of <a href='" + flash_href + "' target='_blank'>Adobe Flash Player</a>, or use an HTML5-enabled browser.</span>");
				all_clear = false;
			}
		}
		
		// flash
		if (this.flash_required == true) {
			if (flash_installed == true) {
				if (flash_version > 10.2) {
					html += line("Flash player: Installed (v" + flash_version + ")");
				} else {
					html += line("<b style='color:%color%'>Flash player: Not current (v" + flash_version + ")</b><br /><span style='font-size:smaller'>Some LaunchPad content requires the <a href='" + flash_href + "' target='_blank'>Adobe Flash Player</a> plugin. Please install the most recent version of the Flash Player to ensure optimum performance.</span>");
					all_clear = false;
				}
			} else {
				html += line("<b style='color:%color%'>Flash player: Not installed</b><br /><span style='font-size:smaller'>Some LaunchPad content requires the <a href='" + flash_href + "' target='_blank'>Adobe Flash Player</a> plugin, which you do not appear to have installed.</span>");
				all_clear = false;
			}
		}
		
		// now, if we're OK...
		if (all_clear == true) {
			html = "<div id='pdx_system_check' style='" + div_style + "'>"
				+ "<a style='color:%color%;float:right;font-size:smaller' href='javascript:PDX_System_Check.show_details()' id='pdx_system_check_show_details_link'>show details</a>"
				+ "<b>SYSTEM CHECK: <span style='color:%color%'>OK</span></b><br />"
				+ "<div id='pdx_system_check_details' style='padding-top:3px;display:none'>"
				+ html
				+ "<div style='font-size:smaller; margin-top:5px'><a href='" + sysreq_href + "' target='_blank'>View full system requirements</a></div>"
				+ "</div>"
				+ "</div>"
				;
			
			html = html.replace(/%width%/, "250px");
			html = html.replace(/%color%/g, "#090");
			html = html.replace(/%bgdcolor%/g, "#efe");
		
		// otherwise...
		} else {
			html = "<div id='pdx_system_check' style='" + div_style + "'>"
				+ "<a style='color:%color%;float:right;font-size:smaller' href='javascript:PDX_System_Check.show_details()' id='pdx_system_check_show_details_link'>hide details</a>"
				+ "<b>SYSTEM CHECK: <span style='color:%color%'>See notes below</span></b><br />"
				+ "<div id='pdx_system_check_details' style='padding-top:3px'>"
				+ html
				+ "</div>"
				+ "<div><a href='" + sysreq_href + "' target='_blank'>View full system requirements</a></div>"
				+ "</div>"
				;

			html = html.replace(/%width%/, "400px");
			html = html.replace(/%color%/g, "#900");
			html = html.replace(/%bgdcolor%/g, "#fee");
			setInterval("PDX_System_Check.flash();", 500);
			details_showing = true;
		}
		
		$("body").append(html);
	},
	
	show_details: function() {
		$("#pdx_system_check_details").slideToggle(100);
		details_showing = !details_showing;
		if (!details_showing) {
			$("#pdx_system_check_show_details_link").html("show details");
		} else {
			$("#pdx_system_check_show_details_link").html("hide details");
		}
	},
	
	flash: function() {
		if (flashing == 0) {
			$("#pdx_system_check").css("background-color", "#fee");
			flashing = 1;
		} else {
			$("#pdx_system_check").css("background-color", "#fff");
			flashing = 0;
		}
	},
	
	initialize: function() {
		BrowserDetect.init();

		// detect flash
		flash_installed = FlashDetect.installed;
		if (flash_installed) {
			flash_version = (FlashDetect.major * 1) + (FlashDetect.minor / 10);
		}
		
		// detect html5 video
		// http://diveintohtml5.info/everything.html - "<video> in H.264 format"
		var v = document.createElement('video');
		html5_video_enabled = !!(v.canPlayType && v.canPlayType('video/mp4; codecs="avc1.42E01E, mp4a.40.2"').replace(/no/, ''));

		// debugging -- uncomment below to artificially set things
		//html5_video_enabled = false;
		//flash_installed = false;
		//BrowserDetect.browser = "Explorer";
		//BrowserDetect.version = 8;
			
		PDX_System_Check.report();
		
		// if site is under construction, insert div for that
		if (PDX_System_Check.underconstruction == true) {
			$("body").append(underconstruction_html);
		}
	}

	};
}();

PDX_System_Check.brightcove_required = true;
PDX_System_Check.flash_required = true;
PDX_System_Check.underconstruction = false;

// initialize window on load
//$(window).load(PDX_System_Check.initialize);
