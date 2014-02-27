// Although not used directly by any BrainHoney pages, this file implements the 
// Data Exchange Javascript API (DEJS), and can be 
// included by activities hosted on external servers being played within an iFrame in the BrainHoney player.
// This API enables activities to save SCORM data variables, including those that update the BrainHoney gradebook
// for the current activity/current user.
// Additional documentation for the API can be found in BrainHoney Content Developer Guide.

// The DEJS_API class implements the following public methods. Detailed documentation for each
// method is adjacent to the method implementation:
//      initialize
//      ping
//      getData
//      setData
//      getSectionData

// The JSONP class is a supporting class that facilitates cross-site AJAX-type requests without actually
// using AJAX. It uses the JSON with Padding (JSONP) approach described here: http://en.wikipedia.org/wiki/JSON#JSONP
// It also provides some utility functions used by DEJS_API.
window.undefined = window.undefined;
var JSONP = function() {
    var queue = [];
    var queueIndex = 0;
    var defaultTimeout = 30000;
    var useHasOwn=!!{}.hasOwnProperty;
    
    function clearQueueItem(requestId) {
        var result = queue[requestId];
        if (result != null && result.timeout != -1) {
            clearTimeout(result.timeout);
            result.timeout = -1;
        }
        queue[requestId] = null;
        return result;
    }
    
    function onTimeout(requestId) {
        JSONP.callback(requestId, null);
    }
    
    // Generic Utility functions
    function isDate(v) {
        return v && typeof v.getFullYear=='function';
    }
    
    var m = {
        "\b": '\\b',
        "\t": '\\t',
        "\n": '\\n',
        "\f": '\\f',
        "\r": '\\r',
        '"' : '\\"',
        "\\": '\\\\'
    };
    
    function encodeString(s){
        if(/["\\\x00-\x1f]/.test(s)){
            return'"'+s.replace(/([\x00-\x1f\\"])/g, function(a,b){
                var c=m[b];
                if(c){
                    return c;
                }
                c=b.charCodeAt();
                return "\\u00" + Math.floor(c/16).toString(16) + (c%16).toString(16);
            }) + '"';
        }
        return '"' + s + '"';
    }
    
    function encodeArray(o) {
        var a=["["],b,i,l=o.length,v;
        for(i=0;i<l;i+=1){
            v=o[i];
            switch(typeof v){
                case"undefined":
                case"function":
                case"unknown":
                    break;
                default:
                    if(b){
                        a.push(',');
                    }
                    a.push(v===null?"null":JSONP.encode(v));
                    b=true;
            }
        }
        a.push("]");
        return a.join("");
    }
    
    function pad(n){
        return n<10?"0"+n:n;
    }
    function encodeDate(o){
        return'"'+o.getFullYear()+"-"+
            pad(o.getMonth()+1)+"-"+
            pad(o.getDate())+"T"+
            pad(o.getHours())+":"+
            pad(o.getMinutes())+":"+
            pad(o.getSeconds())+'"';
    }
    
    
    return {
        request: function (o) {
            if(JSONP.isEmpty(o) || JSONP.isEmpty(o.url)) {
                return;
            }
            var thisId = queue.length;
            o.params = o.params || {};
            o.params.i = thisId.toString(10);   // index added to each call...to align responses in callback

            var script = document.createElement('script');
	        script.type = 'text/javascript';

            var current = {
                script: script
                , options: o
                , timeout: -1
            };
            queue.push(current);

            // execute the request
            script.src = o.url + '?' + JSONP.urlEncode(o.params);
            current.timeout = setTimeout(function() {onTimeout(thisId); }, o.timeout || defaultTimeout);
            document.getElementsByTagName('head')[0].appendChild(script);
        }
        
        ,callback: function(requestId, json) {
            if (requestId == -1 || queue[requestId] == null) {
                // -1 means caller never specified the id (should never happen)
                // null means this request has already been cancelled due to timeout. In both cases...do nothing
                return;
            }
            // removes it from the queue, stops any timeouts, etc.
            var request = clearQueueItem(requestId);
            if (request != null && !JSONP.isEmpty(request.options.callback)) {
                try {
                    request.options.callback.apply(request.options.scope || window, [request.options, json]);
                } catch (e) {
                    // ignore errors from caller: process next request
                }
                document.getElementsByTagName('head')[0].removeChild(request.script);
            }
        }
        , getUrlLength: function(url, params) {
            params.apiIndex = '99999';   // sufficient padding for lots of requests
    	    var temp = url + '?' + JSONP.urlEncode(params);
    	    return temp.length;
        }
        
        // Generic Utility methods
        , apply: function(o, c, defaults){
            if(defaults){
                apply(o, defaults);
            }
            if(o && c && typeof c == 'object'){
                for(var p in c){
                    o[p] = c[p];
                }
            }
            return o;
        }
        
        , isObject : function(v){
            return v && typeof v == "object";
        }
        , isArray : function(v){
            return v && typeof v.length=='number' && typeof v.splice=='function';
        }
        , isEmpty : function(v) {
            return v===null || v==='' || typeof v == 'undefined';
        }
        , encode : function(o) {
            if(typeof o=="undefined"||o===null){
                return "null";
            } else if (JSONP.isArray(o)){
                return encodeArray(o);
            } else if (isDate(o)){
                return encodeDate(o);
            } else if (typeof o=="string"){
                return encodeString(o);
            } else if (typeof o=="number"){
                return isFinite(o) ? String(o) : "null";
            } else if (typeof o=="boolean"){
                return String(o);
            } else{
                var a=["{"],b,i,v;
                for(i in o){
                    if(!useHasOwn||o.hasOwnProperty(i)){
                        v=o[i];
                        switch(typeof v){
                            case"undefined":
                            case"function":
                            case"unknown":
                                break;
                            default:
                                if(b){
                                    a.push(',');
                                }
                                a.push(JSONP.encode(i),":",v===null?"null":JSONP.encode(v));
                                b=true;
                        }
                    }
                }
                a.push("}");
                return a.join("");
            }
        }
        , urlEncode : function(o){
            if(!o){return"";}
            var buf=[];
            for(var key in o){
                var ov=o[key],k=encodeURIComponent(key);
                var type=typeof ov;
                if(type=='undefined'){
                    buf.push(k,"=&");
                }else if(type!="function" && type!="object"){
                    buf.push(k,"=",encodeURIComponent(ov),"&");
                }else if(isDate(ov)){
                    var s=encode(ov).replace(/"/g,'');
                    buf.push(k,"=",s,"&");
                }else if(JSONP.isArray(ov)){
                    if(ov.length){
                        for(var i=0,len=ov.length;i<len;i++){
                            buf.push(k,"=",encodeURIComponent(ov[i]===undefined?'':ov[i]),"&");
                        }
                    }else{
                        buf.push(k,"=&");
                    }
                }
            }
            buf.pop();
            return buf.join("");
        }
        , urlDecode : function(string,overwrite) {
            if (!string||!string.length){
                return{};
            }
            var obj={};
            var pairs=string.split('&');
            var pair,name,value;
            for(var i=0,len=pairs.length;i<len;i++){
                pair=pairs[i].split('=');
                name=decodeURIComponent(pair[0]);
                value=decodeURIComponent(pair[1]);
                if(overwrite!==true){
                    if(typeof obj[name]=="undefined"){
                        obj[name]=value;
                    } else if(typeof obj[name]=="string"){
                        obj[name]=[obj[name]];
                        obj[name].push(value);
                    } else {
                        obj[name].push(value);
                    }
                } else {
                    obj[name]=value;
                }
            }
            return obj;
        }
        , htmlEncode : function(value){
            return !value ? value : String(value).replace(/&/g,"&amp;").replace(/>/g,"&gt;").replace(/</g,"&lt;").replace(/"/g,'&quot;');
        }
    };
}();

JSONP.apply(Function.prototype, {
    createDelegate : function(obj,args){
        var method=this;
        return function(){
            var callArgs=args||arguments;
            return method.apply(obj||window,callArgs);
        };
    },
    defer : function(millis,obj,args){
        var fn=this.createDelegate(obj,args);
        if(millis){
            return setTimeout(fn,millis);
        }
        fn();
        return 0;
    }
});

var DEJS_API = function(){
    // private variables
    var enrollmentId = null;
    var itemId = null;
    var appRoot = null;
    var debug = false;
    var pingTimer = null;
    var timeToLive = 15;    // DLAP expiration timeout in minutes
    
    // private methods
    
    // returns true if initialize was successfully called; otherwise false
    function isInitialized() {
        return !JSONP.isEmpty(enrollmentId) && !JSONP.isEmpty(appRoot) && !JSONP.isEmpty(itemId);
    }
    
	function pingScorm(options) {
        if (!isInitialized()) {
            return false;
        }
        JSONP.request({
            url: appRoot + '/Learn/ScormData.ashx'
            , params: {
                action: 'ping'
            }
            , callback: pingCallback
            , scope: this
            , timeout: options == null ? null : options.timeout
            , agxOptions: options
        });
        return true;
	}    

    function checkForJSONPError(data) {
	    var result = data || {};
	    if (JSONP.isEmpty(result.success)) {
	        result.success = false;
	    }
	    if (!result.success && JSONP.isEmpty(result.message)) {
            result.message = 'no response';
	    }
	    return result;
	}
	
	var putCount = 0;           // global counter to keep track of last request made
	var putRemainder = null;    // remaining data to put after the current request completes
	function putDataWorker(options, data, add) {
        // Max URL length in IE is 2083 characters (http://support.microsoft.com/kb/208427)
	    // Calculate the largest allowed buffer size given the current param set.
	    var sizeTest = {
            url: appRoot + '/Learn/ScormData.ashx'
            , action: 'putscormdata'
            , enrollmentid: enrollmentId
            , itemid: itemId
            , data: ''
            , add: '1'
            , last: '1'
	    };
	    var maxPutSize = 2083 - (JSONP.getUrlLength(appRoot + '/Learn/ScormData.ashx', sizeTest) + 20); // 20 = extra padding
	    var last = false;
		// If the data is larger than allowed, split it into multiple chunks and make multiple JSONP requests
		if (data.length > maxPutSize) {
			putRemainder = data.substr(maxPutSize);
			data = data.substr(0, maxPutSize);
			
			// Avoid escaped-character boundaries...back up a few characters to get in front of the last escaped character
			if (data.substr(data.length-10).indexOf('%') != -1) {
				var splitAt = data.lastIndexOf('%');
				putRemainder = data.substr(splitAt) + putRemainder;
				data = data.substr(0, splitAt);
			}
        } else {
            putRemainder = null;
            last = true;
        }

        // Build parameter list and make the JSONP request	    
	    putCount++;
	    var params = {
            action: 'putscormdata'
            , enrollmentid: enrollmentId
            , itemid: itemId
            , data: decodeURIComponent(data)    // must decode because JSONP re-encodes it
	    };
	    if (add) {
	        params.add = '1';
	    }
	    if (last) {
	        params.last = '1';
	    }
        JSONP.request({
            url: appRoot + '/Learn/ScormData.ashx'
            , params: params
            , callback: putDataCallback
            , scope: this
	        , timeout: options == null ? null : options.timeout
            , agxOptions: options
            , agxPutCount: putCount
        });
	}
	
	function putDataCallback(options, data) {
	    if (options.agxPutCount != putCount) {
	        // this response is for a cancelled/superceeded request. Ignore the result and just return. No callback
	        // to the caller since they know that they cancelled one before the previous callback was called.
	        return;
	    }
        var result = checkForJSONPError(data);
        if (result.success) {
            if (!JSONP.isEmpty(putRemainder)) {
                // there's more data to put. Immediately call putDataWorker after this callback has returned
                putDataWorker.defer(1, this, [options.agxOptions, putRemainder, true /*add*/]);
                return;
            }
        } else {
            // call failed. No need to keep this around
            putRemainder = null;
        }
        // if we got here, then the put has completed.
        if (!JSONP.isEmpty(options.agxOptions) && !JSONP.isEmpty(options.agxOptions.callback)) {
            options.agxOptions.callback.call(options.agxOptions.scope || window, options.agxOptions, result.success);
        }
	}
    
    function pingCallback(options, data) {
        var result = checkForJSONPError(data);
        if (!JSONP.isEmpty(options.agxOptions) && !JSONP.isEmpty(options.agxOptions.callback)) {
            options.agxOptions.callback.call(options.agxOptions.scope || window, options.agxOptions, result.success);
        }
    }
    
    function getDataCallback(options, data) {
        var result = checkForJSONPError(data);
        var data = [];
        if (result.success) {
            //log("got data: " + JSONP.encode(result));
            // push activity user data into array first
            if (!JSONP.isEmpty(result.scormData)) {
                for (var i = 0, len = result.scormData.length; i < len; i++) {
                    var name = result.scormData[i].name;
                    var value = result.scormData[i].value;
                    if (!JSONP.isEmpty(name)) {
            		    data.push({name:name, value:value});
                    }
                }
            }
            // readonly custom fields next
            if (!JSONP.isEmpty(result.customFields)) {
                for (var i = 0, len = result.customFields.length; i < len; i++) {
                    var name = result.customFields[i][0];
                    var value = result.customFields[i][1];
                    if (!JSONP.isEmpty(name)) {
                        name = 'bh.custom.' + name;
            		    data.push({name:name, value:value});
                    }
                }
            }
            // readonly system vars last
            if (!JSONP.isEmpty(result.bhVars)) {
                for (var i = 0, len = result.bhVars.length; i < len; i++) {
                    var name = result.bhVars[i][0];
                    var value = result.bhVars[i][1];
                    if (!JSONP.isEmpty(name)) {
            		    data.push({name:name, value:value});
                    }
                }
            }
        }
        if (!JSONP.isEmpty(options.agxOptions) && !JSONP.isEmpty(options.agxOptions.callback)) {
            options.agxOptions.callback.call(options.agxOptions.scope || window, options.agxOptions, result.success, data);
        }
    }
    
    function getSectionDataCallback(options, data) {
        var result = checkForJSONPError(data);
        if (result.success) {
            //log("got class data: " + JSONP.encode(result));
        }
        if (!JSONP.isEmpty(options.agxOptions) && !JSONP.isEmpty(options.agxOptions.callback)) {
            options.agxOptions.callback.call(options.agxOptions.scope || window, options.agxOptions, result.success, result.data);
        }
    }
//    function log(message) {
//        if (debug) {
//            alert(message);
//        }
//    }

    // public methods
    return {

        /**    
        * Initializes the DEJS_API object. You must call this method immediately upon page load
        * to ensure the page remains authenticated and that all other calls succeed. This method
        * also starts a timer that regularly pings the server to keep this object authenticated
        * @return {Boolean} Returns true on success; otherwise false.
        */
        initialize: function() {
            if (isInitialized()) {
                return true;
            }
            var queryParams = JSONP.urlDecode(window.location.search.substr(1));
            if(queryParams['enrollmentid']) {
                enrollmentId = queryParams['enrollmentid'];
            }
            if (queryParams['itemid']) {
                itemId = queryParams['itemid']
            }
            if (queryParams['approot']) {
                appRoot = queryParams['approot'];
            }
            // Look for BLTI names
            if(queryParams['ext_enrollmentid']) {
                enrollmentId = queryParams['ext_enrollmentid'];
            }
            if (queryParams['ext_itemid']) {
                itemId = queryParams['ext_itemid']
            }
            if (queryParams['ext_approot']) {
                appRoot = queryParams['ext_approot'];
            }
            if (!isInitialized()) {
                return false;
            }
            // Set the timer to tick just shy of 50% of the timeout, which means we'll get 2 pings in
            // every timeout interval (including the first one), and sometimes 3 (in later ones)
            // Currently timeToLive is 15, so this will ping at 7, 14, 21, 28, etc.
            pingTimer = setInterval(pingScorm, (timeToLive * 1000 * 28));            
            return true;
        }

        /**        
        * Launches an asynchronous ping request. This is typically not necessary to call because
        * the initialize method starts a timer that regularly calls ping. 
        * @param {Object} options You can specify an options object containing these members:
        *      * callback {Function} The callback to call upon completion of the request. 
        *          The callback is called with these parameters:
        *              options {Object} - original options passed to ping
        *              success {Boolean} - true if successful; otherwise false
        *      * scope {Object} (optional) scope to execute the callback request. The this pointer for the callback. The default is window.
        *      * timeout {Number} (optional) milliseconds to wait before timing out. The default is 30000 (30 seconds)
        */
        , ping: function(options) {
            return pingScorm(options);
        }
        
        /**
        * Launches an asynchronous request to get the user's data for this activity
        * @param {Object} options Specify an options object containing these members:
        *      * callback {Function} The callback to call upon completion of the request
        *          The callback is called with these parameters:
        *              options {Object} The original options passed to getData
        *              success {Boolean} True if successful; otherwise false
        *              data {Array} An array of objects with 'name' and 'value' members. For example: [{name:'a', value:'foo'},{name:'b',value:'bar'}]
        *      * scope {Object} (optional) Scope to execute the callback request. The this pointer for the callback. The default is window.
        *      * timeout {Number} (optional) Milliseconds to wait before timing out. The default is 30000 (30 seconds)
        *  @return {Boolean} Returns true on successful launch; otherwise false. Check success status of the request in your callback.
        */
        , getData: function(options) {
	        if (!isInitialized()) {
	            return false;   // initialize never called
	        }
            JSONP.request({
                url: appRoot + '/Learn/ScormData.ashx'
                , params: {
                    action: 'getscormdata'
                    , enrollmentid: enrollmentId
                    , itemid: itemId
                }
                , callback: getDataCallback
                , scope: this
    	        , timeout: options == null ? null : options.timeout
                , agxOptions: options
            });
            return true;
        }
        
        /**
        * Launches an asynchronous request to store the data specified in the options.data field.
        * @param {Object} options An options object containing these members:
        *      * data {Array} An array of objects with 'name' and 'value' members. For example: [{name:'a', value:'foo'},{name:'b',value:'bar'}]
        *      * callback {Function} (optional) The callback to call upon completion of the request
        *          The callback is called with these parameters:
        *              options {Object} The original options passed to putData
        *              success {Boolean} True if successful; otherwise false
        *      * scope {Object} (optional) Scope to execute the callback request. The this pointer for the callback. The default is window.
        *      * timeout {Number} (optional) Milliseconds to wait before timing out. The default is 30000 (30 seconds)
        *  @return {Boolean} Returns true on successful launch; otherwise false. Check success status of the request in your callback.
        */
        , putData: function(options) {
            if (!isInitialized()) {
                return false;
            }
            // Build the xml blob
            var dataXml = '<data>';
            if (JSONP.isArray(options.data)) {
                for (var i = 0, len = options.data.length; i < len; i++) {
                    if (!JSONP.isEmpty(options.data[i]) && !JSONP.isEmpty(options.data[i].name) &&
                        !JSONP.isObject(options.data[i].name) && !JSONP.isArray(options.data[i].name))
                    {
                        dataXml = dataXml.concat('<entry name="' + JSONP.htmlEncode(options.data[i].name) + '" value="' +  JSONP.htmlEncode(options.data[i].value || '') + '"/>');
                    }
                }
            }
            dataXml = dataXml.concat('</data>');
            // Send it to the server
            putDataWorker(options, encodeURIComponent(dataXml), false /*add*/);
            return true;
        }

        /**        
        * Launches an asynchronous request to get all section data for the current item.
        * @param {Object} options An options object containing these members:
        *       * callback {Function} The callback to call upon completion of the request
        *          The callback is called with these parameters:
        *              options {Object} The original options passed to getData
        *              success {Boolean} True if successful; otherwise false
        *              data {Array} An array of objects. Each object corresponds to a single student's data and contains 
        *                  these fields:
        *                  id {String} Student's enrollment ID. If the requester is a student, this property is omitted.
        *                  first {String} Student's first name. If the requester is a student, this property is omitted.
        *                  last {String} Student's last name. If the requester is a student, this property is omitted.
        *                  score {String} A real number (typically between 0 and 1) that is the student's score.
        *                  scorm {Array} Array of objects containing name and value properties for all other scorm variables set
        *                      by the student. The objects are in the form [{'name':'value'},…].
        *                      For example: [{'cmi.interactions.0.id':'q1'},{'cmi.interactions.0.type','other'}]
        *      * scope {Object} (optional) Scope to execute the callback request. The this pointer for the callback. The default is window.
        *      * timeout {Number} (optional) Milliseconds to wait before timing out. The default is 30000 (30 seconds)
        *  @ return {Boolean} True on successful launch; otherwise false. Check success status of the request and get the data in your callback.
        */
        , getSectionData: function(options) {
            if (!isInitialized()) {
                return false;
            }
            JSONP.request({
                url: appRoot + '/Learn/ScormData.ashx'
                , params: {
                    action: 'getsectionsummary'
                    , enrollmentid: enrollmentId
                    , itemid: itemId
                    , allstatus: options == null ? false : (options.allstatus ? '1' : '0')
                }
                , callback: getSectionDataCallback
                , scope: this
    	        , timeout: options == null ? null : options.timeout
                , agxOptions: options
            });
            return true;
        }
    };
}();