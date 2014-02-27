/* ****************************************************************************
*
* Copyright (c) Francesco Abbruzzese. All rights reserved.
* francesco@dotnet-programming.com
* http://www.dotnet-programming.com/
* 
* This software is subject to the the license at http://mvccontrolstoolkit.codeplex.com/license  
* and included in the license.txt file of this distribution.
* 
* You must not remove this notice, or any other, from this software.
*
* ***************************************************************************/
//utilities
Array.prototype.remove = function (from, to) {
    var rest = this.slice((to || from) + 1 || this.length);
    this.length = from < 0 ? this.length + from : from;
    return this.push.apply(this, rest);
};
function MvcControlsToolkit_Trim(stringToTrim) {
    return stringToTrim.replace(/^\s+|\s+$/g, "");
}
function MvcControlsToolkit_DateTimeToDate(date) {
    if (!date) return date;
    if (date.getTime() > new Date(date.getFullYear(), date.getMonth(), date.getDate(), 11, 59, 59, 999).getTime()) date.setDate(date.getDate() + 1);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    date.setMilliseconds(0);
    return date;
}
function GlobalEvalScriptInElementId(element) {
    var allScriptText = "";
    if (element.tagName == "SCRIPT") {
        allScriptText = element.text;
    }
    else {
        var scripts = $(element).find("script");
        for (var i = 0; i < scripts.length; i++) {
            allScriptText += scripts[i].text;
        }
    }
    jQuery.globalEval(allScriptText);

}
function GlobalEvalScriptAndDestroy(element) {
    var allScriptText = CollectScriptAndDestroy(element);
    jQuery.globalEval(allScriptText);

}
function CollectScriptAndDestroy(element) {
    var allScriptText = "";
    if (element.tagName == "SCRIPT") {
        allScriptText = element.text;
        $(element).remove();
    }
    else {
        var scripts = $(element).find("script");
        for (var i = 0; i < scripts.length; i++) {
            allScriptText += scripts[i].text;
        }
        scripts.remove();
    }
    return allScriptText;

}

function GlobalEvalScriptInElementIdById(id) {
    var scripts = $("#" + id).find("script");
    var allScriptText = "";
    for (var i = 0; i < scripts.length; i++) {
        allScriptText += scripts[i].text;
    }
    jQuery.globalEval(allScriptText);
}

function CollectAllScriptsInelement(id) {
    var scripts = $("#" + id).find("script");
    var allScriptText = "";
    for (var i = 0; i < scripts.length; i++) {
        allScriptText += scripts[i].text;
    }
    return allScriptText;
}
///////////////////Json serialization/////////////////////////////
(function ($) {

    // JSON RegExp
    var rvalidchars = /^[\],:{}\s]*$/;
    var rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g;
    var rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g;
    var rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g;
    var dateISO = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:[.,]\d+)?(Z|(?:[-+]\d+)?)/i;
    var dateNet = /\/Date\((\d+)(?:[-+]\d+)?\)\//i;

    // replacer RegExp
    var replaceISO = /"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})(?:[.,](\d+))?(Z|(?:[-+]\d+)?)"/i;
    var replaceNet = /"\\\/Date\((\d+)(?:[-+]\d+)?\)\\\/"/i;

    // determine JSON native support
    var nativeJSON = (window.JSON && window.JSON.parse) ? true : false;
    var extendedJSON = nativeJSON && window.JSON.parse('{"x":9}', function (k, v) { return "Y"; }) === "Y";

    var jsonDateConverter = function (key, value) {
        if (typeof (value) === "string") {
            if (dateISO.test(value)) {
                var res = new Date(value);
                if (isNaN(res)) 
                {
                    if (value.charAt(value.length-1) == 'Z')
                        return new Date(value.substring(0, value.length-1));
                    else
                        return new Date(value+'Z');
                }
                else return res;
            }
            if (dateNet.test(value)) {
                return new Date(parseInt(dateNet.exec(value)[1], 10));
            }
        }
        return value;
    };

    $.extend({
        parseJSON: function (data) {
            /// <summary>Takes a well-formed JSON string and returns the resulting JavaScript object.</summary>
            /// <param name="data" type="String">The JSON string to parse.</param>
            /// <param name="convertDates" optional="true" type="Boolean">Set to true when you want ISO/Asp.net dates to be auto-converted to dates.</param>
            var convertDates = true;
            if (typeof data !== "string" || !data) {
                return null;
            }

            // Make sure leading/trailing whitespace is removed (IE can't handle it)
            data = $.trim(data);

            // Make sure the incoming data is actual JSON
            // Logic borrowed from http://json.org/json2.js
            
            if (rvalidchars.test(data
                .replace(rvalidescape, "@")
                .replace(rvalidtokens, "]")
                .replace(rvalidbraces, ""))) {
                // Try to use the native JSON parser
                if (extendedJSON || (nativeJSON && convertDates !== true)) {
                    return window.JSON.parse(data, convertDates === true ? jsonDateConverter : undefined);
                }
                else {
                    data = convertDates === true ?
                        data.replace(replaceISO, "new Date(parseInt('$1',10),parseInt('$2',10)-1,parseInt('$3',10),parseInt('$4',10),parseInt('$5',10),parseInt('$6',10),(function(s){return parseInt(s,10)||0;})('$7'))")
                            .replace(replaceNet, "new Date($1)") :
                        data;
                    return (new Function("return " + data))();
                }
            } else {
                $.error("Invalid JSON: " + data);
            }
        }
    });
})(jQuery);

//////Refreshing //////////////////////////////////
function MvcControlsToolkit_RefreshWidget(element) {
    jElement = $(element);
    var dataRefresh = jElement.attr("data-refresh") || "";
    if (dataRefresh == '') return;
    try{
        $(element)[dataRefresh]("refresh");
    }
    catch(e){
    }
}

//Generic validation functions
var ValidationType_StandardClient = "StandardClient";
var ValidationType_UnobtrusiveClient = "UnobtrusiveClient";
var ValidationType_Server = "Server";

//validate single field return value of true if valid
function MvcControlsToolkit_Validate(fieldName, validationType) {
    if (validationType == ValidationType_StandardClient) {
        if (typeof document.getElementById(fieldName)[MvcControlsToolkit_FieldContext_Tag] === 'undefined') return true;
        var errorMessages = null;
        try{
            errorMessages=document.getElementById(fieldName)[MvcControlsToolkit_FieldContext_Tag].validate('blur');
        }
        catch(e){
        }
        if (errorMessages && errorMessages.length) {
            return false;
        }
        else {
            return true;
        }
    }
    else if (validationType == ValidationType_UnobtrusiveClient) {
        var selector = '#' + fieldName;
        var res = true;
        try{
            res=$(selector).parents('form').validate().element(selector);
        }
        catch(e){
        }
        return  res;
    }
    return true;
}
//validate whole form with return value of true if valid
function MvcControlsToolkit_FormIsValid(elementField, validationType)
{
    if (validationType == ValidationType_StandardClient) {
            var formContext = null;
            $('#' + elementField).parents('form').each(function (i) { formContext = this[MvcControlsToolkit_FieldContext_formValidationTag]; })
            if (formContext == null) return true;
            var errorMessages = formContext.validate('submit');
            if (errorMessages && errorMessages.length) {
                return false;
            }
            else {
                return true;
            }
        }
        else if (validationType == ValidationType_UnobtrusiveClient) {
        return $('#' + elementField).parents('form').validate().form();
        }
        else {
            return true;
        }
    }
 ////////////////////////////STANDARD VALIDATION///////////////////////////////////////////////////////////

// Constants used in validation
var MvcControlsToolkit_FieldContext_hasValidationFiredTag = '__MVC_HasValidationFired';
var MvcControlsToolkit_FieldContext_formValidationTag = '__MVC_FormValidation';
var MvcControlsToolkit_FieldContext_Tag = '__MVC_FieldContext';
var MvcControlsToolkit_SpecialFormName = '_Template_Data_';



// Validation Patch, to avoid that deleted input fields be validated
function MvcControlsToolkit_FormContext$_isElementInHierarchy(parent, child) {
    if (child == null) return false;
    while (child) {
        if (parent === child) {
            return true;
        }
        child = child.parentNode;
    }
    return false;
}
function MvcControlsToolkit_FieldContext$validate(eventName){
if (typeof Sys === 'undefined' || Sys === null || typeof Sys.Mvc === 'undefined' ||  Sys.Mvc === null || typeof Sys.Mvc.FormContext === 'undefined' || Sys.Mvc.FormContext === null) return;
    for (var j = 0; j < this.elements.length; j++) {
        if (!MvcControlsToolkit_FormContext$_isElementInHierarchy(document.body, this.elements[j])) {
            this.clearErrors();
            return [];
        }
    }
    return this.baseValidate(eventName);
}

function MvcControlsToolkit_FieldContext$enableDynamicValidation() {
    for (var i = 0; i < this.elements.length; i++) {
            var element = this.elements[i];
            element[MvcControlsToolkit_FieldContext_Tag]=this;
    }
    this.baseEnableDynamicValidation();
}

if (typeof Sys !== 'undefined' && Sys !== null && typeof Sys.Mvc !== 'undefined' && Sys.Mvc !== null && typeof Sys.Mvc.FormContext !== 'undefined' && Sys.Mvc.FormContext !== null) {

    Sys.Mvc.FieldContext.prototype.baseValidate = Sys.Mvc.FieldContext.prototype.validate;
    Sys.Mvc.FieldContext.prototype.validate = MvcControlsToolkit_FieldContext$validate;

    Sys.Mvc.FieldContext.prototype.baseEnableDynamicValidation= Sys.Mvc.FieldContext.prototype.enableDynamicValidation;
    Sys.Mvc.FieldContext.prototype.enableDynamicValidation = MvcControlsToolkit_FieldContext$enableDynamicValidation;

    Sys.Mvc.FieldContext.prototype._dependsOn = new Array();

    Sys.Mvc.FieldContext.prototype._dependencyOnBlur = function (e) {
        this.validate('blur');
    };

    Sys.Mvc.FieldContext.prototype.takeDynamicValue = function (fieldName) {
        var fieldToVerify = null;
        if (this.elements.length > 0) fieldToVerify = this.elements[0];
        if (fieldToVerify == null) return null;
        var name = fieldToVerify.name;
        var index = name.lastIndexOf('.');
        if (index >= 0) {
            var toCut = name.substring(index + 1);
            var thisId = fieldToVerify.id;
            thisId = thisId.substring(0, thisId.lastIndexOf(toCut));
            fieldName = thisId + fieldName;
        }
        var element = document.getElementById(fieldName);
        if (element == null) return null;
        var toInsert = false;
        if (this._dependsOn == null) {
            this._dependsOn = new Array();
            this._dependsOn[fieldName] = element;
            toInsert = true;
        }
        else {
            if (typeof this._dependsOn[fieldName] === 'undefined') {
                this._dependsOn[fieldName] = element;
                toInsert = true;
            }
        }
        if (toInsert) {
            var jField = $(fieldToVerify);
            var callback = function () {
                if (jField.attr('data-elementispart')) {
                    var jCompanion = $('#' + fieldToVerify.id + '_hidden');
                    jField.trigger('pfocus');
                    jField.trigger('pblur');
                    jCompanion.trigger('pfocus');
                    jCompanion.trigger('pblur');
                }
                else
                    this.validate('blur');
            };
            Sys.UI.DomEvent.addHandler(element, 'blur', Function.createDelegate(this, callback));
            $(element).bind('vblur', callback);
        }
        return element.value;
    };

     
    /////Multy field rules handling

    ///rule registration for standard validation//////

    ///Dynamic Range rule////
    Sys.Mvc.ValidatorRegistry.validators["dynamicrange"] = function (rule) {
        // initialization code can go here.

        var minValue = rule.ValidationParameters["min"];
        var maxValue = rule.ValidationParameters["max"];

        // we return the function that actually does the validation 
        return function (value, context) {
            if (!value || !value.length) return true; /*success*/
            var convertedValue = Number.parseLocale(value);
            if (!isNaN(convertedValue) &&
                (minValue == null || minValue <= convertedValue) &&
                (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return rule.ErrorMessage;
        };
    };

    ///Client Dynamic Range rule////
    Sys.Mvc.ValidatorRegistry.validators["clientdynamirange"] = function (rule) {
        // initialization code can go here.
        var nminValue = rule.ValidationParameters["min"];
        var nmaxValue = rule.ValidationParameters["max"];

        var minDelay = rule.ValidationParameters["mindelay"];
        var maxDelay = rule.ValidationParameters["maxdelay"];


        // we return the function that actually does the validation 
        return function (value, context) {
            if (!value || !value.length) return true; /*success*/
            var convertedValue = Number.parseLocale(value);
            var minValue = null;
            var maxValue = null;

            if (nminValue != null) {
                var sminValue = context.fieldContext.takeDynamicValue(nminValue);
                if (sminValue != null) {
                    minValue = Number.parseLocale(sminValue);
                    if (isNaN(minValue)) {
                        minValue = null;
                    }
                    else if (minDelay != null) {
                        minValue = minValue+minDelay;
                    }
                }

            }

            if (nmaxValue != null) {
                var smaxValue = context.fieldContext.takeDynamicValue(nmaxValue);
                if (smaxValue != null) {
                    maxValue = Number.parseLocale(smaxValue);
                    if (isNaN(maxValue)) {
                        maxValue = null;
                    }
                    else if (maxDelay != null) {
                        maxValue = maxValue+maxDelay;
                    }
                }

            }

            if (!isNaN(convertedValue) &&
                (minValue == null || minValue <= convertedValue) &&
                (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return rule.ErrorMessage;
        };
    };
    ///////////globalized date///////////////
    Sys.Mvc.ValidatorRegistry.validators["globalizeddate"] = function (rule) {
        return function (value, context) {
            if (!value || !value.length) return true; /*success*/
            var convertedValue = Date.parseLocale(value);
            if (convertedValue != null &&
                !isNaN(convertedValue)) {
                return true; /* success */
            }
            return rule.ErrorMessage;
        };
    }
    ///Date Range rule////
    Sys.Mvc.ValidatorRegistry.validators["daterange"] = function (rule) {
        // initialization code can go here.
        var sminValue = rule.ValidationParameters["min"];
        var smaxValue = rule.ValidationParameters["max"];
        var minValue = null;
        var maxValue = null;
        if (sminValue != null) {
            sminValue = "new "+sminValue.substring(1, sminValue.length - 1);
            minValue = eval(sminValue);
        }
        if (smaxValue != null) {
            smaxValue = "new "+smaxValue.substring(1, smaxValue.length - 1);
            maxValue = eval(smaxValue);
        }

        // we return the function that actually does the validation 
        return function (value, context) {
            if (!value || !value.length) return true; /*success*/
            var convertedValue = Date.parseLocale(value);
            if (convertedValue != null && 
                (minValue == null || minValue <= convertedValue) &&
                (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return rule.ErrorMessage;
        };
    };

    ///Client Dynamic Date Range rule////
    Sys.Mvc.ValidatorRegistry.validators["clientdynamicdaterange"] = function (rule) {
        // initialization code can go here.
        var nminValue = rule.ValidationParameters["min"];
        var nmaxValue = rule.ValidationParameters["max"];

        var minDelay = rule.ValidationParameters["mindelay"];
        var maxDelay = rule.ValidationParameters["maxdelay"];
        var minValue = null;
        var maxValue = null;
        


        // we return the function that actually does the validation 
        return function (value, context) {
            if (!value || !value.length) return true; /*success*/
            var convertedValue = Date.parseLocale(value);

            if (nminValue != null) {
                var sminValue = context.fieldContext.takeDynamicValue(nminValue);
                if (sminValue != null) {
                    minValue = Date.parseLocale(sminValue);
                    
                    if (minDelay != null) {
                        minValue = new Date(minValue.getTime() + minDelay);
                    }
                }

            }

            if (nmaxValue != null) {
                var smaxValue = context.fieldContext.takeDynamicValue(nmaxValue);
                if (smaxValue != null) {
                    maxValue = Date.parseLocale(smaxValue);
                    
                    if (maxDelay != null) {
                        maxValue = new Date(maxValue.getTime() + maxDelay);
                    }
                }

            }

            if (convertedValue != null &&
                (minValue == null || minValue <= convertedValue) &&
                (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return rule.ErrorMessage;
        };
    };

}
////////////////END STANDARD VALIDATION/////////////////////////////////////////////////

///////// UNOBTRUSIVE VALIDATION//////////////////////////////////



if (typeof $ !== 'undefined' && $ !== null && typeof $.validator !== 'undefined' && $.validator !== null && typeof $.validator.unobtrusive !== 'undefined' && $.validator.unobtrusive !== null) {

    
    (function ($) {
        ///////Support function for rules involving other fields(I call them client dynamic/////////////////////
        $.validator.takeDynamicValue = function (fieldToVerify, fieldName) {

            if (fieldToVerify == null) return null;

            var name = fieldToVerify.name;
            var index = name.lastIndexOf('.');
            if (index >= 0) {
                var toCut = name.substring(index + 1);
                var thisId = fieldToVerify.id;
                thisId = thisId.substring(0, thisId.lastIndexOf(toCut));
                fieldName = thisId + fieldName;
            }
            var element = document.getElementById(fieldName);
            if (element == null) return null;
            var toInsert = false;
            var _dependsOn = jQuery.data(fieldToVerify, "_dependsOn");
            if (typeof _dependsOn == 'undefined' || _dependsOn == null) {
                _dependsOn = new Array();
                _dependsOn[fieldName] = element;
                jQuery.data(fieldToVerify, "_dependsOn", _dependsOn);
                toInsert = true;
            }
            else {
                if (typeof _dependsOn[fieldName] === 'undefined') {
                    _dependsOn[fieldName] = element;
                    toInsert = true;
                }
            }
            if (toInsert) {
                var jField = $(fieldToVerify);
                var callback = function () {
                    if (jField.attr('data-elementispart')) {
                        var jCompanion = $('#' + fieldToVerify.id + '_hidden');
                        jField.trigger('pfocus');
                        jField.trigger('pblur');
                        jCompanion.trigger('pfocus');
                        jCompanion.trigger('pblur');
                    }
                    else
                        jField.parents('form').first().validate().element(fieldToVerify);
                };
                $(element).blur(callback);
                $(element).bind('vblur', callback);
            }
            return element.value;
        };
        //////////////parsing input elements parsing functions ///////////////////////////
        $.validator.unobtrusive.clearAndParse = function (selector) {
            var form = $(selector).parents("form");
            if (form.length != 0) {
                form.removeData("validator");
            }
            else {
                $(selector).removeData("validator");
            }
            $.validator.unobtrusive.parse(selector);
        }
        $.validator.unobtrusive.reParse = function (selector) {
            $.validator.unobtrusive.clearAndParse(selector);
            MvcControlsToolkit_ParseRegister.parse(selector);
        }
        $.validator.unobtrusive.parseExt = function (selector) {

            $.validator.unobtrusive.parse(selector);


            var form = $(selector).first().closest('form');


            var unobtrusiveValidation = form.data('unobtrusiveValidation');
            var validator = form.validate();

            $.each(unobtrusiveValidation.options.rules, function (elname, elrules) {
                if (validator.settings.rules[elname] == undefined) {
                    var args = {};
                    $.extend(args, elrules);
                    args.messages = unobtrusiveValidation.options.messages[elname];
                    $('[name= "' + elname + '"]').rules("add", args);
                } else {
                    $.each(elrules, function (rulename, data) {
                        if (validator.settings.rules[elname][rulename] == undefined) {
                            var args = {};
                            args[rulename] = data;
                            args.messages = unobtrusiveValidation.options.messages[elname][rulename];
                            $('[name= "' + elname + '"]').rules("add", args);
                        }
                    });
                }
            });
            MvcControlsToolkit_ParseRegister.parse(selector);
        }
    })($);

    /// mandatory attribute
    $.validator.unobtrusive.adapters.addBool("mandatory", "required");
    /////Multy field rules handling

    ///Dynamic Range rule////
    $.validator.addMethod(
        "dynamicrange",
        function (value, element, param) {

            var minValue = param[0]; if (minValue == '') minValue = null;
            var maxValue = param[1]; if (maxValue == '') maxValue = null;


            if ((!value || !value.length) && this.optional(element)) return true; /*success*/
            var convertedValue = null;
            if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                convertedValue = jQuery.global.parseFloat(value);
            }
            else {
                convertedValue = parseFloat(value);
            }
            if (!isNaN(convertedValue) &&
            (minValue == null || minValue <= convertedValue) &&
            (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return false;
        },
    "value is not in the required range");
        jQuery.validator.unobtrusive.adapters.add("dynamicrange", ["min", "max"], function (options) {
            var min = options.params.min,
                max = options.params.max;

            options.rules["dynamicrange"] = [min, max];
            if (options.message) {
                options.messages["dynamicrange"] = options.message;
            }
        });
  ///Client Dynamic Range rule////
        $.validator.addMethod(
        "clientdynamirange",
        function (value, element, param) {
            var nminValue = param[0]; if (nminValue == '') nminValue = null;
            var nmaxValue = param[2]; if (nmaxValue == '') nmaxValue = null;

            var minDelay = param[1]; if (minDelay == '') minDelay = null;
            var maxDelay = param[3]; if (maxDelay == '') maxDelay = null;
            var minValue = null;
            var maxValue = null;
            if (minDelay != null) minDelay = parseFloat(minDelay);
            if (maxDelay != null) maxDelay = parseFloat(maxDelay);


            if ((!value || !value.length) && this.optional(element)) return true; /*success*/
            var convertedValue = null;
            if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                convertedValue = jQuery.global.parseFloat(value);
            }
            else {
                convertedValue = parseFloat(value);
            }
            

            if (nminValue != null) {
                var sminValue = $.validator.takeDynamicValue(element, nminValue);
                if (sminValue != null) {
                    minValue = null;
                    if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                        minValue = jQuery.global.parseFloat(sminValue);
                    }
                    else {
                        minValue = parseFloat(sminValue);
                    }
                    if (isNaN(minValue)) {
                        minValue = null;
                    }
                    else if (minDelay != null) {
                        minValue = minValue + minDelay;
                    }
                }

            }

            if (nmaxValue != null) {
                var smaxValue = $.validator.takeDynamicValue(element, nmaxValue);
                if (smaxValue != null) {
                    maxValue = null;
                    if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                        maxValue = jQuery.global.parseFloat(smaxValue);
                    }
                    else {
                        maxValue = parseFloat(smaxValue);
                    }
                    if (isNaN(maxValue)) {
                        maxValue = null;
                    }
                    else if (maxDelay != null) {
                        maxValue = maxValue + maxDelay;
                    }
                }

            }

            if (!isNaN(convertedValue) &&
            (minValue == null || minValue <= convertedValue) &&
            (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return false;

        },
        "value is not in the required range");
        jQuery.validator.unobtrusive.adapters.add("clientdynamirange", ["min", "mindelay", "max", "maxdelay"], function (options) {
            var min = options.params.min,
                mindelay = options.params.mindelay;
                max = options.params.max,
                maxdelay = options.params.maxdelay;

                options.rules["clientdynamirange"] = [min, mindelay, max, maxdelay];
            if (options.message) {
                options.messages["clientdynamirange"] = options.message;
            }
        });


        //////Date validation///////////////
        $.validator.addMethod(
            "globalizeddate",
            function (value, element, param) {
                if ((!value || !value.length) && this.optional(element)) return true; /*success*/
                var convertedValue = null;
                if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                    convertedValue = new Date(value);
                    convertedValue = jQuery.global.parseDate(value);
                }
                else {
                    convertedValue = new Date(value);
                }
                return !isNaN(convertedValue) && convertedValue != null;
            },
            "field must be a date/time"
        );
            $.validator.addMethod(
        "daterange",
        function (value, element, param) {

            var minValue = param[0]; if (minValue == '') minValue = null;
            var maxValue = param[1]; if (maxValue == '') maxValue = null;

            if (minValue != null) minValue = (minValue + '').charAt(0) == "/" ? eval("new " + minValue.substring(1, minValue.length - 1)) : new Date(minValue);
            if (maxValue != null) maxValue = (maxValue + '').charAt(0) == "/" ? eval("new " + maxValue.substring(1, maxValue.length - 1)) : new Date(maxValue);

            if ((!value || !value.length) && this.optional(element)) return true; /*success*/
            var convertedValue = null;
            if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                convertedValue = new Date(value);
                convertedValue = jQuery.global.parseDate(value);
            }
            else {
                convertedValue = new Date(value);
            }
            if (!isNaN(convertedValue) &&
            (minValue == null || minValue <= convertedValue) &&
            (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return false;
        },
    "date is not in the required range");
            jQuery.validator.unobtrusive.adapters.add("daterange", ["min", "max"], function (options) {
                var min = options.params.min,
                max = options.params.max;
                options.rules["daterange"] = [min, max];
                if (options.message) {
                    options.messages["daterange"] = options.message;
                }
            });

        $.validator.addMethod(
        "clientdynamicdaterange",
        function (value, element, param) {
            var nminValue = param[0]; if (nminValue == '') nminValue = null;
            var nmaxValue = param[2]; if (nmaxValue == '') nmaxValue = null;

            var minDelay = param[1]; if (minDelay == '') minDelay = null;
            var maxDelay = param[3]; if (maxDelay == '') maxDelay = null;
            var minValue = null;
            var maxValue = null;
            if (minDelay != null) minDelay = parseInt(minDelay);
            if (maxDelay != null) maxDelay = parseInt(maxDelay);


            if ((!value || !value.length) && this.optional(element)) return true; /*success*/
            var convertedValue = null;
            if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                convertedValue = jQuery.global.parseDate(value);
            }
            else {
                convertedValue = parseDate(value);
            }


            if (nminValue != null) {
                var sminValue = $.validator.takeDynamicValue(element, nminValue);
                if (sminValue != null) {
                    minValue = null;
                    if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                        minValue = jQuery.global.parseDate(sminValue);
                    }
                    else {
                        minValue = parseDate(sminValue);
                    }
                    if (isNaN(minValue)) {
                        minValue = null;
                    }
                    else if (minDelay != null) {
                        minValue = new Date(minValue.getTime() + minDelay);
                    }
                }

            }

            if (nmaxValue != null) {
                var smaxValue = $.validator.takeDynamicValue(element, nmaxValue);
                if (smaxValue != null) {
                    maxValue = null;
                    if (typeof jQuery.global !== 'undefined' && typeof jQuery.global.parseFloat === 'function') {
                        maxValue = jQuery.global.parseDate(smaxValue);
                    }
                    else {
                        maxValue = parseDate(smaxValue);
                    }
                    if (isNaN(maxValue)) {
                        maxValue = null;
                    }
                    else if (maxDelay != null) {
                        maxValue = new Date(maxValue.getTime() + maxDelay);
                    }
                }

            }

            if (!isNaN(convertedValue) &&
            (minValue == null || minValue <= convertedValue) &&
            (maxValue == null || convertedValue <= maxValue)) {
                return true; /* success */
            }
            return false;

        },
        "date is not in the required range");

        jQuery.validator.unobtrusive.adapters.add("clientdynamicdaterange", ["min", "mindelay", "max", "maxdelay"], function (options) {
            var min = options.params.min,
                mindelay = options.params.mindelay;
            max = options.params.max,
                maxdelay = options.params.maxdelay;

            options.rules["clientdynamicdaterange"] = [min, mindelay, max, maxdelay];
            if (options.message) {
                options.messages["clientdynamicdaterange"] = options.message;
            }
        });

        jQuery.validator.unobtrusive.adapters.add("globalizeddate", [], function (options) {
            options.rules["globalizeddate"] = [];
            if (options.message) {
                options.messages["globalizeddate"] = options.message;
            }
        });
    
}

////////// END UNOBTRUSIVE VALIDATION //////////////////

// DUAL SELECT

    var DualSelect_Separator = ";;;";
    var DualSelect_SelectAvial = "_AvialSelect";
    var DualSelect_SelectSelected = "_SelSelect";
    var DualSelect_HiddenSelectedItemsVal = "";
    var DualSelect_Postfix = "___PackedValue";
    var DualSelect_AvailableFilter = "_AvailableFilter";
    var DualSelect_SelectedFilter = "_SelectedFilter";

    var DualSelect_TempObjSource, DualSelect_TempObjDestination;

    function MvcControlsToolkit_SubstringRating(toSearch, destination) {
        toSearch = toSearch.toLowerCase();
        destination = destination.toLowerCase();
        var firstMatch = -true ;
        var penalties = 0;
        var matchCount = 0;
        for (var j = 0; j < toSearch.length; j++) {
            if (destination == '') return [matchCount, penalties];
            var currChar = toSearch.charAt(j);
            var index = destination.indexOf(currChar);
            if (index == -1) continue;
            matchCount++;
            if (firstMatch) {
                penalties += index;
                firstMatch = false;
            }
            else penalties += index * 1000;
            
            if (index + 1 < destination.length)
                destination = destination.substr(index + 1)
            else
                destination = '';
        }
        return [matchCount, penalties];
    }

    function DualSelect_FilterInit(controlId, selected) {
        $('#' + controlId + (selected ? DualSelect_SelectedFilter : DualSelect_AvailableFilter)).keydown(function () {
            var self = this;
            var prev = MvcControlsToolkit_Trim($(self).val());
            setTimeout(function () {
                var target = document.getElementById(controlId + (selected ? DualSelect_SelectSelected : DualSelect_SelectAvial));
                var searchTerm = MvcControlsToolkit_Trim($(self).val());
                if (prev == searchTerm) return;
                prev = searchTerm;
                if (searchTerm == '') return;
                var toOrder = [];

                for (var i = 0; i < target.length; i++) {
                    toOrder.push({
                        rating:MvcControlsToolkit_SubstringRating(searchTerm, $(target[i]).text()),
                        item: target[i]
                    });
                }
                toOrder.sort(function (a, b) {
                    if (a.rating[0] > b.rating[0]) return -1;
                    if (b.rating[0] > a.rating[0]) return 1;
                    if (a.rating[1] < b.rating[1]) return -1;
                    if (b.rating[1] < a.rating[1]) return 1;
                    return 0;
                });
                target.options.length = 0;
                for (var i = 0; i < toOrder.length; i++) {
                    target.options.add(toOrder[i].item);
                }
                if (selected) DualSelect_SaveSelection(controlId);
            });
        });
    }

    function DualSelect_SetObjects(dualSelectId, bDoSelected) {
        if (bDoSelected) {
            DualSelect_TempObjSource =
			document.getElementById(dualSelectId + DualSelect_SelectAvial);
            DualSelect_TempObjDestination =
			document.getElementById(dualSelectId + DualSelect_SelectSelected);
        }
        else {
            DualSelect_TempObjSource =
			document.getElementById(dualSelectId + DualSelect_SelectSelected);
            DualSelect_TempObjDestination =
			document.getElementById(dualSelectId + DualSelect_SelectAvial);
        }
    }



    function DualSelect_GetIndexForInsert(oSelect, oNode) {
        if (oSelect.autosort == "false")
            return oSelect.length + 1;

        if (oSelect.length == 0) return 0;

        for (var i = 0; i < oSelect.length; i++)
            if (oSelect[i].text > oNode.text)
                return i;
        return oSelect.length;
    }



    function DualSelect_MoveElement(dualSelectId, bDoSelected) {
        DualSelect_SetObjects(dualSelectId, bDoSelected);
        var rootElement = $('#' + dualSelectId.substring(0, dualSelectId.lastIndexOf("___")));

        if (DualSelect_TempObjSource.length == 0) return;

        iLast = 0;

        for (var i = 0; i < DualSelect_TempObjSource.length; i++) {
            if (DualSelect_TempObjSource[i].selected) {
                iLast = i;
                var oNode = DualSelect_TempObjSource[i];
                var changeData = new MvcControlsToolkit_changeData(oNode, bDoSelected ? 'ItemCreating' : 'ItemDeleting', 0);
                rootElement.trigger('itemChange', changeData);
                if (changeData.Cancel == true) continue;
                DualSelect_TempObjSource.remove(i);
                nPos = (DualSelect_TempObjDestination.length + 1);
                DualSelect_TempObjDestination.options.add(oNode,
				DualSelect_GetIndexForInsert(DualSelect_TempObjDestination, oNode));
                changeData = new MvcControlsToolkit_changeData(oNode, bDoSelected ? 'ItemCreated' : 'ItemDeleted', 0);
                rootElement.trigger('itemChange', changeData);
                i--;
            }
        }

        DualSelect_SaveSelection(dualSelectId);

        if (DualSelect_TempObjSource.length > 0 && iLast == 0)
            DualSelect_TempObjSource.selectedIndex = 0;
        else if (DualSelect_TempObjSource.length - 1 >= iLast)
            DualSelect_TempObjSource.selectedIndex = iLast;
        else if (DualSelect_TempObjSource.length >= 1)
            DualSelect_TempObjSource.selectedIndex = iLast - 1;

        DualSelect_ClearSelection(DualSelect_TempObjSource);
        DualSelect_TempObjSource.focus;
        rootElement.trigger('DualSelect_Changed');
    }

    function DualSelect_Move_Up(dualSelectId, bDoSelected) {
        DualSelect_SetObjects(dualSelectId, bDoSelected);
        var rootElement = $('#' + dualSelectId.substring(0, dualSelectId.lastIndexOf("___")));
        if (DualSelect_TempObjSource.length == 0) return;
        if (DualSelect_TempObjSource[0].selected) return;
        for (var i = 1; i < DualSelect_TempObjSource.length; i++) {
            if (DualSelect_TempObjSource[i].selected) {
                var tempValue = DualSelect_TempObjSource[i];
                var changeData = new MvcControlsToolkit_changeData(tempValue, 'ItemMoving', 1);
                rootElement.trigger('itemChange', changeData);
                if (changeData.Cancel == true) continue;
                var tempValue1 = DualSelect_TempObjSource[i - 1];
                DualSelect_TempObjSource.remove(i);
                DualSelect_TempObjSource.remove(i - 1);
                DualSelect_TempObjSource.options.add(tempValue, i - 1);
                DualSelect_TempObjSource.options.add(tempValue1, i);
                changeData = new MvcControlsToolkit_changeData(tempValue, 'ItemMoved', 1);
                rootElement.trigger('itemChange', changeData);
                i--;
            }
        }

        DualSelect_SaveSelection(dualSelectId);
        rootElement.trigger('DualSelect_Changed');
    }

    function DualSelect_Move_Down(dualSelectId, bDoSelected) {
        DualSelect_SetObjects(dualSelectId, bDoSelected);
        var rootElement = $('#' + dualSelectId.substring(0, dualSelectId.lastIndexOf("___")));
        if (DualSelect_TempObjSource.length == 0) return;
        if (DualSelect_TempObjSource[DualSelect_TempObjSource.length - 1].selected) return;
        for (var i = DualSelect_TempObjSource.length - 2; i > -1; i--) {
            if (DualSelect_TempObjSource[i].selected) {
                var tempValue = DualSelect_TempObjSource[i];
                var changeData = new MvcControlsToolkit_changeData(tempValue, 'ItemMoving', -1);
                rootElement.trigger('itemChange', changeData);
                if (changeData.Cancel == true) continue;
                var tempValue1 = DualSelect_TempObjSource[i + 1];
                DualSelect_TempObjSource.remove(i + 1);
                DualSelect_TempObjSource.remove(i);
                DualSelect_TempObjSource.options.add(tempValue1, i);
                DualSelect_TempObjSource.options.add(tempValue, i + 1);
                changeData = new MvcControlsToolkit_changeData(tempValue, 'ItemMoved', -1);
                rootElement.trigger('itemChange', changeData);
                i++;
            }
        }

        DualSelect_SaveSelection(dualSelectId);
        rootElement.trigger('DualSelect_Changed');
    }

    function DualSelect_MoveAll(dualSelectId, bDoSelected, noEvents) {
        DualSelect_SetObjects(dualSelectId, bDoSelected);
        var rootElement = $('#' + dualSelectId.substring(0, dualSelectId.lastIndexOf("___")));
        var first = 0;
        while (DualSelect_TempObjSource.length > first) {
            oNode = DualSelect_TempObjSource[first];
            if (noEvents != true) {
                var changeData = new MvcControlsToolkit_changeData(oNode, bDoSelected ? 'ItemCreating' : 'ItemDeleting', 0);
                rootElement.trigger('itemChange', changeData);
                if (changeData.Cancel == true) { first++; continue; }
            }
            DualSelect_TempObjSource.remove(oNode);
            DualSelect_TempObjDestination.options.add(oNode,
			DualSelect_GetIndexForInsert(DualSelect_TempObjDestination, oNode));
            if (noEvents != true) {
                var changeData = new MvcControlsToolkit_changeData(oNode, bDoSelected ? 'ItemCreated' : 'ItemDeleted', 0);
                rootElement.trigger('itemChange', changeData);
            }
        }

        DualSelect_SaveSelection(dualSelectId);
        if (noEvents != true) rootElement.trigger('DualSelect_Changed');
    }



    function DualSelect_ClearSelection(oSelect) {
        for (var i = 0; i < oSelect.length; i++)
            oSelect[i].selected = false;
    }



    function DualSelect_SaveSelection(dualSelectId) {
        var oSelect = document.getElementById(
		dualSelectId + DualSelect_SelectSelected);
        var sValues = "";
        var sTexts = "";

        for (var i = 0; i < oSelect.length; i++) {
            sValues += oSelect[i].value + DualSelect_Separator;

        }

        document.getElementById(
		dualSelectId + DualSelect_HiddenSelectedItemsVal).value = sValues;

    }

    function MvcControlsToolkit_DualSelect_Set(element, newValue, formatString, valueType) {
        var node = document.getElementById(element.id + DualSelect_Postfix + DualSelect_SelectAvial);
        var dnode = document.getElementById(element.id + DualSelect_Postfix + DualSelect_SelectSelected);
        if (node == null || dnode == null) {
            setTimeout(function () { MvcControlsToolkit_DualSelect_Set(element, newValue, formatString, valueType); }, 0);
            return;
        }
        DualSelect_MoveAll(element.id + DualSelect_Postfix, false, true);
        if (newValue instanceof Array && node.options.length > 0) {
            var typedArray = new Array();
            for (var i = 0, j = node.length; i < j; i++) {
                typedArray.push(MvcControlsToolkit_Parse(node.options[i].value, valueType));
                node.options[i].selected = false;
            }
            for (var i = 0, n = newValue.length; i < n; i++) {

                if (valueType == MvcControlsToolkit_DataType_Float) {
                    var selectedIndex = 0;
                    var bestError = Math.abs(newValue[i] - typedArray[0]);
                    for (var j = 1, l = typedArray.length; j < l; j++) {
                        if (Math.abs(typedArray[j] - newValue[i]) < bestError) {
                            selectedIndex = j;
                            bestError = Math.abs(typedArray[j] - newValue[i]);
                        }
                    }
                    if (selectedIndex >= 0) {
                        var onode = node.options[selectedIndex];
                        node.options.remove(selectedIndex);
                        typedArray.remove(selectedIndex);
                        dnode.options.add(onode);
                    }
                }
                else {
                    for (var j = 0, l = typedArray.length; j < l; j++) {
                        if (typedArray[j] === newValue[i]) {
                            var onode = node.options[j];
                            node.options.remove(j);
                            typedArray.remove(j);
                            dnode.options.add(onode);
                            break;
                        }
                    }
                }
            }
            DualSelect_SaveSelection(element.id + DualSelect_Postfix);

        }
    }
    function MvcControlsToolkit_DualSelect_SetString(element, newValue) {
        MvcControlsToolkit_DualSelect_Set(element, newValue, null, MvcControlsToolkit_DataType_String);
    }
    function MvcControlsToolkit_DualSelect_Get(element, valueType) {
        var node = document.getElementById(element.id + DualSelect_Postfix + DualSelect_SelectSelected);
        if (node == null) {
            setTimeout(function () { MvcControlsToolkit_DualSelect_Get(element, valueType); }, 0);
            return;
        }
        var typedArray = new Array();
        for (var i = 0, j = node.length; i < j; i++) {
            typedArray.push(MvcControlsToolkit_Parse(node.options[i].value, valueType));
        }
        return typedArray;
    }
    function MvcControlsToolkit_DualSelect_GetString(cnode) {
        return MvcControlsToolkit_DualSelect_Get(cnode, MvcControlsToolkit_DataType_String);
    }

// DATE AND TIME FUNCTIONS

var defaultYear = 1970 + 0;
var defaultMonth = 0+0;
var defaultDay = 1+0;
var defaultHour = 0 + 0;
var defaultMinute = 0 + 0;
var defaultSecond = 0 + 0;


function DateInput_Initialize(id) 
{
    if (document.getElementById(id + "_Year") != null) 
    {
        document.getElementById(id + "_Year").onkeypress = DateInputYearKeyVerify;
        document.getElementById(id + "_Year").onpaste = DateInputYearHandlePaste;
        document.getElementById(id + "_Year").ondrop = DateInputYearHandlePaste;
        document.getElementById(id + "_Year").onchange = DateInputChanged;
    }
    if (document.getElementById(id + "_Month") != null)
        document.getElementById(id + "_Month").onchange = DateInputChanged;

    if (document.getElementById(id + "_Day") != null)
        document.getElementById(id + "_Day").onchange = DateInputChanged;

    if (document.getElementById(id + "_Hours") != null)
        document.getElementById(id + "_Hours").onchange = DateInputChanged;

    if (document.getElementById(id + "_Minutes") != null)
        document.getElementById(id + "_Minutes").onchange = DateInputChanged;

    if (document.getElementById(id + "_Seconds") != null)
        document.getElementById(id + "_Seconds").onchange = DateInputChanged;

    if (eval(id + "_DateInCalendar")) {
        var options = eval(id + "_CalendarOptions");
        
        $('#' + id + '_Calendar').datepicker(options);
        
        
        
    }
    DateInputChanged(null, id, true, null, true);
    $('#' + id + '_Hidden').data('ready', true);

}

function DateInputGetNumDays(M, curYear) {
    M = M + 1;
    if (curYear % 4 == 0) {
        return (M == 9 || M == 4 || M == 6 || M == 11) ? 30 : (M == 2) ? 29 : 31;
    } else {
        return (M == 9 || M == 4 || M == 6 || M == 11) ? 30 : (M == 2) ? 28 : 31;
    }
}

function DateTimeAdjustYears(cmbInput, min, max) {
    if (cmbInput == null || cmbInput.tagName != 'SELECT') return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    var oldVar = cmbInput.value;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        if (i < 10)
            cmbInput.options[j] = new Option("   " + i, i);
        else if (i < 100)
            cmbInput.options[j] = new Option("  " + i, i);
        else if (i < 1000)
            cmbInput.options[j] = new Option(" " + i, i);
        else
            cmbInput.options[j] = new Option("" + i, i);
        j++;
    }
    MvcControlsToolKit_SetDateElement(cmbInput.id, oldVar);
}

function DateTimeAdjustMonthes(cmbInput, min, max) {
    if (cmbInput == null) return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    var oldVar = cmbInput.value;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        cmbInput.options[j] = new Option(DateTimeMonthes[i], i + 1);
        j++;
    }
    MvcControlsToolKit_SetDateElement(cmbInput.id, oldVar);
}

function DateTimeAdjustDays(cmbInput, min, max) {
    if (cmbInput == null) return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    var oldVar = cmbInput.value;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        if (i < 10)
            cmbInput.options[j] = new Option(" " + i, i);
        else
            cmbInput.options[j] = new Option("" + i, i);
        j++;
    }
    MvcControlsToolKit_SetDateElement(cmbInput.id, oldVar);
}
function DateTimeAdjustTimeElement(cmbInput, min, max) {
    if (cmbInput == null) return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    var oldVar = cmbInput.value;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        if (i < 10)
            cmbInput.options[j] = new Option("0" + i, i);
        else
            cmbInput.options[j] = new Option("" + i, i);
        j++;
    }
    MvcControlsToolKit_SetDateElement(cmbInput.id, oldVar);
}

function DateInputYearHandlePaste(evt) {

    evt = (evt) ? (evt) : ((window.event) ? (window.event) : null);
    if (evt == null) return true;

    var sorg = (evt.target) ? (evt.target) : ((event.srcElement) ? (event.srcElement) : null);
    if (sorg == null) return true;

    var val;
    if (evt.type == "paste")
        val = window.clipboardData.getData("Text");
    else if (evt.type == "drop")
        val = evt.dataTransfer.getData("Text");
    else
        return true;


    for (i = 0; i < val.length; i++) {
        keyCode = val.charCodeAt(i);

        if (keyCode == 13 || keyCode == 8)
            continue;

        if ((keyCode >= 48) && (keyCode <= 57))
            continue;
        else
            return false;

    }
    sorg.value = val;
    return false;
}

function DateInputYearKeyVerify(evt) {
    evt = (evt) ? (evt) : ((window.event) ? (window.event) : null);
    if (evt == null) return true;

    var sorg = (evt.target) ? (evt.target) : ((event.srcElement) ? (event.srcElement) : null);
    if (sorg == null) return true;

    var keyCode = ((evt.charCode || evt.initEvent) ? evt.charCode : ((evt.which) ? evt.which : evt.keyCode));


    if (keyCode == 0 || keyCode == 13 || keyCode == 8)
        return true;
    if ((keyCode >= 48) && (keyCode <= 57))
        return true;
    return false;
    var val = sorg.value;
}

function DateTimeInput_UpdateCalendar(clientId) {
    Nanno = document.getElementById(clientId + "_Year").value;
    Nmese = document.getElementById(clientId + "_Month").value;
    Ngiorno = document.getElementById(clientId + "_Day").value;

    var newDate = new Date(Nanno, Nmese - 1, Ngiorno);
    var dateHost = $('#' + clientId + "_Calendar");
    var format = dateHost.datepicker("option", "dateFormat");
    if (format == null) format = 'mm/dd/yy';
    
    dateHost.datepicker( "setDate" , $.datepicker.formatDate(format, newDate));

}

function DateTimeInput_UpdateFromCalendar(stringDate, clientId) {
    var dateHost = $('#' + clientId + "_Calendar");

    
    if (stringDate == null) return;

    var format = dateHost.datepicker("option", "dateFormat");
    if (format == null) format = 'mm/dd/yy';
    var date = null;
    try{
        date = $.datepicker.parseDate(format, stringDate);
    }
    catch (e){
        date = new Date();
    }

    var stringDateMin = dateHost.datepicker("option", "minDate");
    var stringDateMax = dateHost.datepicker("option", "maxDate");

    var dateMin = null;
    var dateMax = null;

    if (stringDateMin != null) dateMin = $.datepicker.parseDate(format, stringDateMin);
    if (stringDateMax != null) dateMax = $.datepicker.parseDate(format, stringDateMax);
    
    if (dateMin != null && date < dateMin) {
        date = dateMin;
    }
    if (dateMax != null && date > dateMax) {
        date = dateMax;
    }
    

    document.getElementById(clientId + "_Year").value = date.getFullYear();
    document.getElementById(clientId + "_Month").value = date.getMonth() + 1;
    document.getElementById(clientId + "_Day").value = date.getDate();

    DateInputChanged(null, clientId, true);
    


}

function DateTimeInput_UpdateCalendarMinMax(clientId, minDate, maxDate) {
    var dateHost = $('#' + clientId + "_Calendar");
    var format = dateHost.datepicker("option", "dateFormat");
    if (format == null) format = 'mm/dd/yy';

    if (minDate != null) {
        dateHost.datepicker("option", "minDate", $.datepicker.formatDate(format, minDate));
    }
    else {
        dateHost.datepicker("option", "minDate", null);
    }
    if (maxDate != null) {
        dateHost.datepicker("option", "maxDate", $.datepicker.formatDate(format, maxDate));
    }
    else {
        dateHost.datepicker("option", "maxDate", null);
    }
}

function DateInputChanged(evt, cid, update, force, init) {

    var clientID;
    if (cid == null) {


        evt = (evt) ? (evt) : ((window.event) ? (window.event) : null);
        if (evt == null) {

            return false;
        }

        var sorg = (evt.target) ? (evt.target) : ((event.srcElement) ? (event.srcElement) : null);
        if (sorg == null) {

            return false;
        }
        clientID = sorg.id.substring(0, sorg.id.lastIndexOf("_"));
    }
    else {
        clientID = cid;
    }
    if (eval(clientID + "Recursive") == true && force == null) return;
    eval(clientID + "Recursive = true;");


    var Nanno;
    var Nmese;
    var Ngiorno;
    var NHours;
    var NMinutes;
    var NSeconds;
    var CurrDate = eval(clientID + "_Curr");
    var CurrDay = CurrDate.getDate();
    var CurrMonth = CurrDate.getMonth();
    var CurrYear = CurrDate.getFullYear();
    var CurrHours = CurrDate.getHours();
    var CurrMinutes = CurrDate.getMinutes();
    var CurrSeconds = CurrDate.getSeconds();

    var currMin = eval(clientID + "_MinDate");
    var currMax = eval(clientID + "_MaxDate");

    var dynamicMin = null;
    if (eval("(typeof " + clientID + "_ClientDynamicMin !== 'undefined') && (" + clientID + "_ClientDynamicMin != null)") == true) dynamicMin = eval(clientID + "_ClientDynamicMin()");

    var dynamicMax = null;
    if (eval("(typeof " + clientID + "_ClientDynamicMax !== 'undefined') && (" + clientID + "_ClientDynamicMax != null)") == true) dynamicMax = eval(clientID + "_ClientDynamicMax()");

    if (dynamicMin != null && (currMin == null || dynamicMin > currMin)) {
        if (currMax != null && dynamicMin > currMax)
            currMin = currMax;
        else
            currMin = dynamicMin;
    }
    if (dynamicMax != null && (currMax == null || dynamicMax < currMax)) {
        if (currMin != null && dynamicMax < currMin)
            currMax = currMin;
        else
            currMax = dynamicMax;
    }

    if (document.getElementById(clientID + "_Year") != null) {
        Nanno = document.getElementById(clientID + "_Year").value;
    }
    else {
        Nanno = CurrYear;
    }

    if (document.getElementById(clientID + "_Month") != null)
        Nmese = document.getElementById(clientID + "_Month").value;
    else
        Nmese = CurrMonth;

    if (document.getElementById(clientID + "_Day") != null)
        Ngiorno = document.getElementById(clientID + "_Day").value;
    else
        Ngiorno = CurrDay;

    if (document.getElementById(clientID + "_Hours") != null)
        NHours = document.getElementById(clientID + "_Hours").value;
    else
        NHours = CurrHours;

    if (document.getElementById(clientID + "_Minutes") != null)
        NMinutes = document.getElementById(clientID + "_Minutes").value;
    else
        NMinutes = CurrMinutes;

    if (document.getElementById(clientID + "_Seconds") != null)
        NSeconds = document.getElementById(clientID + "_Seconds").value;
    else
        NSeconds = CurrSeconds;

    var TempNewDate = new Date(Nanno, Nmese-1, Ngiorno, NHours, NMinutes, NSeconds);

    if (currMax != null && currMax < TempNewDate) TempNewDate = currMax;
    if (currMin != null && currMin > TempNewDate) TempNewDate = currMin;

    Nanno = TempNewDate.getFullYear()+"";
    Nmese = (TempNewDate.getMonth()+1)+"";
    Ngiorno = TempNewDate.getDate()+"";
    NHours = TempNewDate.getHours()+"";
    NMinutes = TempNewDate.getMinutes()+"";
    NSeconds = TempNewDate.getSeconds()+"";

    var NewYear;
    var NewMonth;
    var NewDay;
    var NewHours;
    var NewMinutes;
    var NewSeconds;
    var MaxYear;
    var MinYear;
    var MaxMonth;
    var MinMonth;
    var MinDay;
    var MaxDay;
    var MinHours;
    var MaxHours;
    var MinMinutes;
    var MaxMinutes;
    var MinSeconds;
    var MaxSeconds;
    eval(clientID + "_Valid = true");

    
    //year processing
    NewYear = parseInt(Nanno);
    if (!isNaN(NewYear)) {
        if  (currMax == null) {
            MaxYear = null;
        }
        else {
            MaxYear = currMax.getFullYear();
        }
        if (currMin == null) {
            MinYear = null;
        }
        else {
            MinYear = currMin.getFullYear();
        }
        if (MaxYear != null && MaxYear < NewYear) NewYear = MaxYear;
        if (MinYear != null && MinYear > NewYear) NewYear = MinYear;
        if (document.getElementById(clientID + "_Year") != null && !eval(clientID + "_DateHidden") && !eval(clientID + "_DateInCalendar"))
            DateTimeAdjustYears(document.getElementById(clientID + "_Year"), MinYear, MaxYear);
        
        if ((MaxYear == null || MaxYear >= NewYear) && (MinYear == null || MinYear <= NewYear)) {

            //Month Processing
            MaxMonth = 11;
            MinMonth = 0;
            if (MaxYear == NewYear) {
                MaxMonth = currMax.getMonth();
            }
            if (MinYear == NewYear) {
                MinMonth = currMin.getMonth();
            }
            NewMonth = parseInt(Nmese);
            if (!isNaN(NewMonth)) {
                NewMonth = NewMonth - 1;
                if (MinMonth > NewMonth) {
                    NewMonth = MinMonth;
                }
                if (MaxMonth < NewMonth) {
                    NewMonth = MaxMonth;
                }
                if (init || CurrYear == MinYear || CurrYear == MaxYear || NewYear == MinYear || NewYear == MaxYear)
                    if (document.getElementById(clientID + "_Month") != null && !eval(clientID + "_DateHidden") && !eval(clientID + "_DateInCalendar"))
                        DateTimeAdjustMonthes(document.getElementById(clientID + "_Month"), MinMonth, MaxMonth);
                //day processing
                MinDay = 1;
                MaxDay = DateInputGetNumDays(NewMonth, NewYear);
                if (MaxYear == NewYear && MaxMonth == NewMonth) {
                    MaxDay = currMax.getDate();

                }
                if (MinYear == NewYear && MinMonth == NewMonth) {
                    MinDay = currMin.getDate();
                }
                NewDay = parseInt(Ngiorno);
                if (!isNaN(NewDay)) {
                    if (MinDay > NewDay) {
                        NewDay = MinDay;
                    }
                    if (MaxDay < NewDay) {
                        NewDay = MaxDay;
                    }
                    if (document.getElementById(clientID + "_Day") != null && !eval(clientID + "_DateHidden") && !eval(clientID + "_DateInCalendar"))
                        DateTimeAdjustDays(document.getElementById(clientID + "_Day"), MinDay, MaxDay);
                    // Hours Processing
                    MinHours = 0;
                    MaxHours = 23;
                    if (MaxYear == NewYear && MaxMonth == NewMonth && NewDay == MaxDay) {
                        MaxHours = currMax.getHours();
                    }
                    if (MinYear == NewYear && MinMonth == NewMonth && NewDay == MinDay) {
                        MinHours = currMin.getHours();
                    }
                    NewHours = parseInt(NHours);
                    if (!isNaN(NewHours)) {
                        if (MaxHours < NewHours) NewHours = MaxHours;
                        if (NewHours < MinHours) NewHours = MinHours;
                        if (document.getElementById(clientID + "_Hours") != null)
                            DateTimeAdjustTimeElement(document.getElementById(clientID + "_Hours"), MinHours, MaxHours);
                        // Minutes Processing
                        MinMinutes = 0;
                        MaxMinutes = 59;
                        if (MaxYear == NewYear && MaxMonth == NewMonth && NewDay == MaxDay && MaxHours == NewHours)
                            MaxMinutes = currMax.getMinutes();
                        if (MinYear == NewYear && MinMonth == NewMonth && NewDay == MinDay && MinHours == NewHours)
                            MinMinutes = currMin.getMinutes();
                        NewMinutes = parseInt(NMinutes);
                        if (!isNaN(NewMinutes)) {
                            if (MaxMinutes < NewMinutes) NewMinutes = MaxMinutes;
                            if (MinMinutes > NewMinutes) NewMinutes = MinMinutes;
                            if (document.getElementById(clientID + "_Minutes") != null)
                                DateTimeAdjustTimeElement(document.getElementById(clientID + "_Minutes"), MinMinutes, MaxMinutes);
                            // Seconds Processing
                            MinSeconds = 0;
                            MaxSeconds = 59;
                            if (MaxYear == NewYear && MaxMonth == NewMonth && NewDay == MaxDay && MaxHours == NewHours && MaxMinutes == NewMinutes)
                                MaxSeconds = currMax.getSeconds();
                            if (MinYear == NewYear && MinMonth == NewMonth && NewDay == MinDay && MinHours == NewHours && MinMinutes == NewMinutes)
                                MinSeconds = currMin.getSeconds();
                            NewSeconds = parseInt(NSeconds);
                            if (!isNaN(NewSeconds)) {
                                if (MaxSeconds < NewSeconds) NewSeconds = MaxSeconds;
                                if (NewSeconds < MinSeconds) NewSeconds = MinSeconds;
                                if (document.getElementById(clientID + "_Seconds") != null)
                                    DateTimeAdjustTimeElement(document.getElementById(clientID + "_Seconds"), MinSeconds, MaxSeconds);
                            }
                            else {
                                eval(clientID + "_Valid = false");
                            }
                        }

                        else {
                            eval(clientID + "_Valid = false");
                        }
                    }
                    else {
                        eval(clientID + "_Valid = false");
                    }
                }
                else {
                    eval(clientID + "_Valid = false");
                }
            }
            else {
                eval(clientID + "_Valid = false");
            }
        }
    }
    else {
        eval(clientID + "_Valid = false");
    }
    if (eval(clientID + "_DateInCalendar")) {
        DateTimeInput_UpdateCalendarMinMax(
        clientID,
        currMin,
        currMax);
    }
    var AChange = false;
    if (eval(clientID + "_Valid")) {
        if (update == true || (cid == null  && 
            (CurrYear != NewYear || CurrMonth != NewMonth || CurrDay != NewDay ||
             CurrHours != NewHours || CurrMinutes != NewMinutes || CurrSeconds != NewSeconds))) 
               AChange = true;
        CurrYear = NewYear;
        CurrMonth = NewMonth;
        CurrDay = NewDay;
        CurrHours = NewHours;
        CurrMinutes = NewMinutes;
        CurrSeconds = NewSeconds;
    }
    if (!AChange) {
        eval(clientID + "Recursive = false;")
        return true;
    }

    eval(clientID + "_Curr = new Date(CurrYear, CurrMonth, CurrDay, CurrHours, CurrMinutes, CurrSeconds)");
    
    if (document.getElementById(clientID + "_Year") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Year", CurrYear);
        
    }
    if (document.getElementById(clientID + "_Month") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Month", CurrMonth + 1);
    }
    if (document.getElementById(clientID + "_Day") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Day", CurrDay);
    }
    if (eval(clientID + "_DateInCalendar")) {
        DateTimeInput_UpdateCalendar(clientID);
    }
    if (document.getElementById(clientID + "_Hours") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Hours", CurrHours);
    }
    if (document.getElementById(clientID + "_Minutes") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Minutes", CurrMinutes);
    }
    if (document.getElementById(clientID + "_Seconds") != null) {
        MvcControlsToolKit_SetDateElement(clientID + "_Seconds", CurrSeconds);
    }
    
    var currDate = eval(clientID + "_Curr");
    RefreshDependencies(clientID);
    eval(clientID + "_ClientDateChanged(" + currDate.getTime() + ")");
    $("#"+clientID + "_Hidden").trigger("DateTimeInput_Changed");
    eval(clientID + "Recursive = false;");
    return true;
}
function MvcControlsToolKit_SetDateElement(id, value) {
    var element = document.getElementById(id);
    if (element.tagName == 'SELECT') {
        value = parseInt(value);
        for (var i = element.options.length - 1; i >= 0; i--) {
            if (parseInt(element.options[i].value) <= value) {
                element.selectedIndex = i;
                return;
            }
        }
        element.selectedIndex = 0;
    } else {
        if ((value === null) || (value === undefined))
            value = "";
        element.value = value;
    }
}
function SetDateInput(id, value, cType) {
    if (eval("typeof " + id + "_Curr === 'undefined'") == true) return;
    var currDate = eval(id + "_Curr");
    
    if (currDate == null) return;
    var currDateInMilliseconds = currDate.getTime();

    if (cType == 1 && value >= currDateInMilliseconds) 
    {
        return;
    }
    if (cType == 2 && value <= currDateInMilliseconds) {
       return;
    }
   var DateInFormat = new Date(value);
   var currMin = eval(id + "_MinDate");
   var currMax = eval(id + "_MaxDate");
   if (currMin != null && DateInFormat < currMin) DateInFormat = currMin;
   if (currMax != null && DateInFormat > currMax) DateInFormat = currMax;
   
    if (document.getElementById(id + "_Hours") != null) {
        if (document.getElementById(id + "_Year") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Year", DateInFormat.getFullYear());
            DateInputChanged(null, id, false, true);
        }
        if (document.getElementById(id + "_Month") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Month", DateInFormat.getMonth() + 1);
            DateInputChanged(null, id, false, true);
        }
        if (document.getElementById(id + "_Day") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Day", DateInFormat.getDate());
            DateInputChanged(null, id, false, true);
        }
        if (document.getElementById(id + "_Hours") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Hours", DateInFormat.getHours());
            DateInputChanged(null, id, false, true);
        }
        if (document.getElementById(id + "_Minutes") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Minutes", DateInFormat.getMinutes());
            DateInputChanged(null, id, false, true);
        }
        if (document.getElementById(id + "_Seconds") != null) {
            MvcControlsToolKit_SetDateElement(id + "_Seconds", DateInFormat.getSeconds());
            DateInputChanged(null, id, true, true);
        }
       }
    else {
            if (document.getElementById(id + "_Year") != null) {
                MvcControlsToolKit_SetDateElement(id + "_Year", DateInFormat.getFullYear());
                DateInputChanged(null, id, false, true);
            }
            if (document.getElementById(id + "_Month") != null) {
                MvcControlsToolKit_SetDateElement(id + "_Month", DateInFormat.getMonth() + 1);
                DateInputChanged(null, id, false, true);
            }
            if (document.getElementById(id + "_Day") != null) {
                MvcControlsToolKit_SetDateElement(id + "_Day", DateInFormat.getDate());
                DateInputChanged(null, id, true, true);
            }
    }
    if (eval(id + "_DateInCalendar")) {
        DateTimeInput_UpdateCalendar(id);
    }
    
}

function GetDateInput(id) {
    return eval(id + "_Curr");
}

function MvcControlsToolkit_DateTimeInput_SetString(sorg, value) {
    clientID = sorg.id.substring(0, sorg.id.lastIndexOf("_"));
    var ob = null;
    if (value == null || value == "") {
        ob = new Date();
    }
    else {
        ob = MvcControlsToolkit_Parse(value, MvcControlsToolkit_DataType_DateTime);
    }
    SetDateInput(clientID, ob.getTime(), 3);
}

function MvcControlsToolkit_DateTimeInput_Set(sorg, value, format, valueType) {
    clientID = sorg.id.substring(0, sorg.id.lastIndexOf("_"));
    if ($('#' + sorg.id).length == 0 || eval("typeof " + clientID + "_Curr") === "undefined" || (!($('#' + clientID + '_Hidden').data('ready') || false))) {
        var retry = function () { MvcControlsToolkit_DateTimeInput_Set(sorg, value, format, valueType); };
        setTimeout(retry, 0);
        return;
    }
    if (value == null || value == "") value = new Date();
    SetDateInput(clientID, value.getTime(), 3);
}
function MvcControlsToolkit_DateTimeInput_SetById(id, value, format, valueType) {
    if (value == null || value == "") value = new Date();
    SetDateInput(id+'__', value.getTime(), 3);
}
function MvcControlsToolkit_DateTimeInput_Get(sorg, valueType) {
    clientID = sorg.id.substring(0, sorg.id.lastIndexOf("_"));
    if (eval("typeof " + clientID + "_Curr") === "undefined") return null;
    if (!($('#' + clientID + '_Hidden').data('ready') || false)) return null;
    return eval(clientID + "_Curr");
}
function MvcControlsToolkit_DateTimeInput_GetById(id, valueType) {
    return eval(id + "___Curr");
}
function MvcControlsToolkit_DateTimeInput_BindChange(id, handler) {
    $("#" + id + "___Hidden").bind("DateTimeInput_Changed", handler);
}
function MvcControlsToolkit_DateTimeInput_UnbindChange(id, handler) {
    $("#" + id + "___Hidden").unbind("DateTimeInput_Changed", handler);
}
function MvcControlsToolkit_DateTimeInput_GetString(sorg) {
    clientID = sorg.id.substring(0, sorg.id.lastIndexOf("_"));
    return MvcControlsToolkit_Format(GetDateInput(clientID), 's', MvcControlsToolkit_DataType_DateTime, '', '');
}

function AddToUpdateList(id, toAdd) 
{
    if (id == null || toAdd == null) return;
    
    var currIndex = eval(id+"_UpdateListIndex");
    eval(id + "_UpdateList[" + currIndex + "] = '" + toAdd + "';");
    currIndex++;
    eval(id + "_UpdateListIndex = "+currIndex+";");
}

function RefreshDependencies(id) 
{
    if (eval("typeof " + id + "_UpdateListIndex === 'undefined'") == true) return;
    var length = eval(id + "_UpdateListIndex");
    if (length == null) return;
    for (var i = 0; i < length; i++) {
        DateInputChanged(null, eval(id + "_UpdateList[" + i + "]"), true);
    }
}
/////////////////////////Change Handlers/////////////////////////////////////////
function MvcControlsToolkit_changeData(itemChanged, changeType, data) {
    this.ItemChanged = itemChanged;
    this.ChangeType = changeType;
    this.Data = data;
}
MvcControlsToolkit_changeData.prototype = {
    ItemChanged: null,
    ChangeType: null,
    Data: null,
    Cancel: false
}

//////////////////////// DATAGRID /////////////////////////////

var DataButtonCancel = "DataButtonCancel";
var DataButtonDelete = "DataButtonDelete";
var DataButtonUndelete = "DataButtonUndelete";
var DataButtonEdit = "DataButtonEdit";
var DataButtonInsert = "DataButtonInsert";
var DataButtonResetRow = "DataButtonResetRow";
var DisplayPostfix ="_Display";
var EditPostfix = "_Edit";
var OldEditPostfix = "_OldEdit";
var UndeletePostfix = "_Undelete";
var ChangedExternallyPostfix = "_ChangedExternally";
var SavePostFix = "_Save";
var SavePostFixCurr = "_SaveCurr";
var DatagridFielsdPostfix = "_Datagrid_Fields";
var SavePostFixD = "_SaveD";
var SavePostFixU = "_SaveU";
var SavePostFixC = "_SaveC";
var DeletedPostFix = "_Deleted";
var ContainerPostFix = "_Container";
var VarPostfix = "_Var";
var AllNormalPostfix = "_AllNormal";
var ChandedPostfix = "_Changed";

var TemplateVarsPostfix = '_templateVars';
var TemplatePreparePostfix = '_templatePrepare';
var PlaceHolderPostfix = '_placeHolder';
var ChangedHiddenPostfix = '_changedHidden';
var TemplateSymbolPostfix='_templateSymbol';
var LastIndexPostfix='_lastIndex';
var DataGrid_ValidationTypePostfix = '_validationType';
var MinLastIndexPostfix = '_minLastIndex';
var LastVisibleIndexPostfix = '_lastVisibleIndex';
var TemplateEditHtmlPostfix='_editHtml';
var TemplateDisplayHtmlPostfix = '_displayHtml';
var TemplateAllJavascriptPostfix = '_allJavascript';
var MvcControlsToolkit_DatagridCssPostfix = '_Css';
var MvcControlsToolkit_DatagridAltCssPostfix = '_AltCss';
var MvcControlsToolkit_DatagridFatherItemsPostfix = '_FatherItems';

function DataGrid_Field(original, current, validationType) {
    this.Original = original;
    this.Current = current;
    this.ValidationType = validationType;
    if (this.Original != null) {
        var attr = $(this.Original).attr('data-element-type');
        if (attr != undefined && attr != null) {
            this.ControlType = attr;
            var cnode = this.Original;
            if (attr == 'DateTimeInput') {
                this.Original = eval(' MvcControlsToolkit_' + attr + '_Get(cnode, null)');
            }
            else {
                this.Original = eval(' MvcControlsToolkit_' + attr + '_GetString(cnode)');
            }
        }
    }
}

DataGrid_Field.prototype = {
    Original: null,
    Current: null,
    ControlType: null,
    ValidationType: null,
    Reset: function () {
        if (this.Original == null) return;
        if (this.Current == null) return;
        if (this.ControlType != null) {
            var cnode = this.Current;
            var attr = this.ControlType;
            var cvalue = this.Original;
            if (attr == 'DateTimeInput') {
                eval(' MvcControlsToolkit_' + attr + '_Set(cnode, cvalue, null, null);');
            }
            else {
                eval(' MvcControlsToolkit_' + attr + '_SetString(cnode, cvalue);');
            }
            return;
        }
        if (this.Original.nodeName.toLowerCase() == "input") {
            var iType = this.Original.getAttribute('type').toLowerCase();
            if (iType == 'checkbox' ||
                iType == 'radio') {
                //var unchanged = this.Current.checked == this.Original.checked;
                this.Current.checked = this.Original.checked;
                //if (!unchanged)
                // $(this.Current).trigger('change');
            }
            else if (iType != 'file' && iType != 'button' && iType != 'reset' && iType != 'submit' && iType != 'image') {
                try {
                    //if (iType != 'hidden') $(this.Current).trigger('focus');
                    this.Current.value = this.Original.value;
                    //if (iType != 'hidden') $(this.Current).trigger('blur');
                }
                catch (e) {
                }
            }
        }
        else if (this.Original.nodeName.toLowerCase() == "textarea") {
            //$(this.Current).trigger('focus');
            this.Current.value = this.Original.value;
            //$(this.Current).trigger('blur');
        }
        else if (this.Original.nodeName.toLowerCase() == "select") {
            $(this.Current).html($(this.Original).html());
            //$(this.Current).trigger('change');
        }
        
        MvcControlsToolkit_RefreshWidget(this.Current);
        
        // if (this.Current.id != '') MvcControlsToolkit_Validate(this.Current.id, this.ValidationType);
    }
}

function DataGrid_EditRowFields(validationType) {
    this.Names = new Array();
    this.Dictionary = new Array();
    this.ValidationType=validationType;
}

DataGrid_EditRowFields.prototype = {
    Names: null,
    Dictionary: null,
    ValidationType: null,
    AddOriginal: function (fieldName, field) {
        if (typeof this.Dictionary[fieldName] === 'undefined') {
            this.Names.push(fieldName);
            this.Dictionary[fieldName] = new DataGrid_Field(field, null, this.ValidationType);
        }
        else {
            this.Dictionary[fieldName].Original = field;
        }
    },
    AddCurrent: function (fieldName, field) {
        if (typeof this.Dictionary[fieldName] === 'undefined') {
            this.Names.push(fieldName);
            this.Dictionary[fieldName] = new DataGrid_Field(null, field, this.ValidationType);
        }
        else {
            this.Dictionary[fieldName].Current = field;
        }
    },
    Reset: function(){
        var fieldName=null;
        for(var i=0; i < this.Names.length; i++){
            fieldName=this.Names[i];
            if (typeof this.Dictionary[fieldName] !== 'undefined') {
                this.Dictionary[fieldName].Reset();
            }
        }
    }
}

function DataGrid_ResetRow(itemRoot) {
    var fields = null;
    var temp = null;
    var validationType=null;
    var root = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    root = root.substring(0, root.lastIndexOf('___'));
    validationType = eval(root+DataGrid_ValidationTypePostfix);
    fields = eval(itemRoot + DatagridFielsdPostfix);
    if (fields == null) {
        fields = new DataGrid_EditRowFields(validationType);
        eval(itemRoot + DatagridFielsdPostfix + " = fields;");
        
        temp = eval(itemRoot + SavePostFix);
        temp.find('input:not([data-elementispart])').each(function (i) {
            fields.AddOriginal(this.id, this);
        });
        temp.find('textarea:not([data-elementispart])').each(function (i) {
            fields.AddOriginal(this.id, this);
        });
        temp.find('select:not([data-elementispart])').each(function (i) {
            fields.AddOriginal(this.id, this);
        });
        temp.find('[data-element-type]').each(function (i) {
            fields.AddOriginal(this.id, this);
        });
        temp = eval(itemRoot + SavePostFixCurr);
        temp.find('input:not([data-elementispart])').each(function (i) {
            fields.AddCurrent(this.id, this);
        });
        temp.find('textarea:not([data-elementispart])').each(function (i) {
            fields.AddCurrent(this.id, this);
        });
        temp.find('select:not([data-elementispart])').each(function (i) {
            fields.AddCurrent(this.id, this);
        });
        temp.find('[data-element-type]').each(function (i) {
            fields.AddCurrent(this.id, this);
        });
        eval(itemRoot + SavePostFix+ " = null;");
    }
    fields.Reset();
    var currRow = eval(itemRoot + SavePostFixCurr);
    currRow.find('.input-validation-error').removeClass('input-validation-error');
    currRow.find('.field-validation-error')
        .removeClass('field-validation-error')
        .addClass('field-validation-valid');
}
function DataGrid_Remove_Edit_Item(itemRoot) {
    var temp = null;
    temp = $('#' + itemRoot + EditPostfix + ContainerPostFix).detach();
    if (temp.length != 0) {
        eval(itemRoot + SavePostFixCurr + " = temp;");
    }
}

function DataGrid_Display_Edit_Item(itemRoot) {
    var temp = null;
    temp =  eval(itemRoot + SavePostFixCurr);   
    $('#' + itemRoot + DisplayPostfix + ContainerPostFix).before(temp);
    DataGrid_ResetRow(itemRoot);
}

function DataGrid_ItemRoot_AtIndex(itemRoot, index) {
    var right = itemRoot.substring(itemRoot.lastIndexOf('___'));
    var left = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    left = left.substring(0, left.lastIndexOf('_') + 1);
    return left + index + right;
}
function DataGrid_ItemRoot_Index(itemRoot) {
    itemRoot = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    itemRoot = itemRoot.substring(itemRoot.lastIndexOf('_') + 1);
    return parseInt(itemRoot);
}

function DataGrid_ChecKInsertNewItem(itemRoot) {
    var root = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    root = root.substring(0, root.lastIndexOf('___'));
    if (eval("typeof " + root + LastIndexPostfix + " === 'undefined'")) return;
    var index = DataGrid_ItemRoot_Index(itemRoot);
    var visibleIndex = eval(root + LastVisibleIndexPostfix);
    if (index != visibleIndex) return;

    var lastIndex = eval(root + LastIndexPostfix);
    
    if (lastIndex == index) {
        index++;
        var indexStr = index + '';
        var templateSymbol = eval(root + TemplateSymbolPostfix);
        var displayTemplate = eval(root + TemplateDisplayHtmlPostfix).replace(templateSymbol, indexStr);
        var editTemplate = eval(root + TemplateEditHtmlPostfix).replace(templateSymbol, indexStr);
        var changed = $('<div>').append($('#' + eval(root + ChangedHiddenPostfix)).clone()).remove().html().replace(templateSymbol, indexStr);
        var placeHolderName = eval(root + PlaceHolderPostfix);
        var placeHolder = $('<div>').append($('#' + placeHolderName).clone()).remove().html().replace(templateSymbol, indexStr);

        var tableRoot = $('#' + itemRoot + EditPostfix + ContainerPostFix).parent();
        $(displayTemplate).appendTo(tableRoot);
        $(editTemplate).appendTo(tableRoot);
        
        
        

        jQuery.globalEval(eval(root + TemplateAllJavascriptPostfix).replace(templateSymbol, indexStr));

        var hiddenElementsFather = $('#' + placeHolderName).parent();

        hiddenElementsFather.append(placeHolder);
        hiddenElementsFather.append(changed);
        
        var newItemName = DataGrid_ItemRoot_AtIndex(itemRoot, index) + EditPostfix + ContainerPostFix;
        if (typeof $ !== 'undefined' && $ !== null && typeof $.validator !== 'undefined' && $.validator !== null && typeof $.validator.unobtrusive !== 'undefined' && $.validator.unobtrusive !== null) {

            jQuery.validator.unobtrusive.parseExt('#' + newItemName);
        }

        var initVars = eval(root + TemplateVarsPostfix).replace(templateSymbol, indexStr);
        jQuery.globalEval(initVars);
        
        var changeData = new MvcControlsToolkit_changeData($('#' + newItemName), 'NewHtmlCreated', 0);
        tableRoot.trigger('itemChange', changeData);
        
        var initCall = eval(root + TemplatePreparePostfix).replace(templateSymbol, indexStr);
        jQuery.globalEval(initCall);

        

        visibleIndex++;
        lastIndex++;
        eval(root + LastVisibleIndexPostfix + ' = visibleIndex;');
        eval(root + LastIndexPostfix + ' = lastIndex;');
        
        
    }
    else {
        index++;
        var nextItem = DataGrid_ItemRoot_AtIndex(itemRoot, index);
        var temp = eval(nextItem + SavePostFixD);
        $('#' + nextItem + DisplayPostfix + ContainerPostFix).replaceWith(temp.clone(true));
        eval(root + LastVisibleIndexPostfix + ' = index;');
    }

}

function DataGrid_ChecKDisappearItem(itemRoot) {
    var root = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    root = root.substring(0, root.lastIndexOf('___'));
    if (eval("typeof  " + root + LastIndexPostfix + " === 'undefined'")) return;

    var index = DataGrid_ItemRoot_Index(itemRoot);
    var visibleIndex = eval(root + LastVisibleIndexPostfix);
    var minIndex = eval(root + MinLastIndexPostfix);

    if (index < minIndex) return;
    if (index != visibleIndex) {
        $('#' + itemRoot + DisplayPostfix + ContainerPostFix).css('display', 'none'); ;
        if (index == visibleIndex - 1) {
            index++;
            itemRoot = DataGrid_ItemRoot_AtIndex(itemRoot, index);
            DataGrid_ChecKDisappearItem(itemRoot);
            return;
            
        }
        else {
            return;
        }
    }
    var display = $('#' + itemRoot + DisplayPostfix + ContainerPostFix);
    if (display.length == 0) return;
    if (index <= minIndex) {
        display.css('display', '');
        return;
    }      
    var prevItem = DataGrid_ItemRoot_AtIndex(itemRoot, index - 1);
    if ($('#' + prevItem + DisplayPostfix + ContainerPostFix).length == 0) return;
    display.css('display', 'none');
    visibleIndex--;
    eval(root + LastVisibleIndexPostfix+' = visibleIndex');
    index--;
    var prevtItem = DataGrid_ItemRoot_AtIndex(itemRoot, index);
    DataGrid_ChecKDisappearItem(prevItem, root);

}

function MvcControlsToolkit_Grid_ItemName(item){
    var itemRoot = item.id;
    var place = itemRoot.lastIndexOf("_");
    if (place < 0) return null;
    itemRoot = itemRoot.substring(0, place);
    place = itemRoot.lastIndexOf("_");
    if (place < 0) return null;
    return itemRoot.substring(0, place) + '_Container';

}
function MvcControlsToolkit_DataButton_Click(itemRoot, dataButtonType) {
    if (typeof (itemRoot) != 'string') itemRoot = MvcControlsToolkit_Grid_ItemName(itemRoot);
    var place = itemRoot.lastIndexOf("_");
    if (place < 0) return null;
    itemRoot = itemRoot.substring(0, place);
    place = itemRoot.lastIndexOf("_");
    if (place < 0) return null;
    var itemChanged = itemRoot.substring(0, place)+'_Changed';
    DataButton_Click(itemRoot, itemChanged, dataButtonType);
}
function DataButton_Click(itemRoot, itemChanged, dataButtonType) {
    if (dataButtonType == DataButtonDelete) {
        var undel = eval(itemRoot + SavePostFixU);
        var jItem = $('#' + itemRoot + DisplayPostfix + ContainerPostFix);
        if (jItem.length == 0) return;
        var jParent = jItem.parent();
        var changeData = new MvcControlsToolkit_changeData(jItem, 'ItemDeleting', 0);
        jParent.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        if (undel != null) {
            jItem.before(undel.clone(true));
            jItem.remove();
        }
        else {
            jItem.css('display', 'none');
        }
        DataGrid_Remove_Edit_Item(itemRoot);

        $('#' + itemChanged).val('True');
        eval(itemRoot + DeletedPostFix + " = true;");
        changeData = new MvcControlsToolkit_changeData(jItem, 'ItemDeleted', 0);
        MvcControlsToolkit_DataGridApplyStylesItem(itemRoot);
        jParent.trigger('itemChange', changeData);
    }
    else if (dataButtonType == DataButtonEdit) {
        var jItem = $('#' + itemRoot + DisplayPostfix + ContainerPostFix);
        if (jItem.length == 0) return;
        var jParent = jItem.parent();
        var changeData = new MvcControlsToolkit_changeData(jItem, 'ItemGoingEdit', 0);
        jParent.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        DataGrid_Display_Edit_Item(itemRoot);
        jItem.remove();
        $('#' + itemChanged).val('True');
        MvcControlsToolkit_DataGridApplyStylesItem(itemRoot);
        changeData = new MvcControlsToolkit_changeData($('#' + itemRoot + EditPostfix + ContainerPostFix), 'ItemGoneEdit', 0);
        jParent.trigger('itemChange', changeData);
    }
    else if (dataButtonType == DataButtonInsert) {
        var jItem = $('#' + itemRoot + DisplayPostfix + ContainerPostFix);
        if (jItem.length == 0) return;
        var jParent = jItem.parent();
        var changeData = new MvcControlsToolkit_changeData(null, 'ItemCreating', 0);
        jParent.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        DataGrid_Display_Edit_Item(itemRoot);
        jItem.remove();
        $('#' + itemChanged).val('True');
        DataGrid_ChecKInsertNewItem(itemRoot);
        MvcControlsToolkit_DataGridApplyStylesItem(itemRoot);
        changeData = new MvcControlsToolkit_changeData($('#' + itemRoot + EditPostfix + ContainerPostFix), 'ItemCreated', 0);
        jParent.trigger('itemChange', changeData);
    }
    else if (dataButtonType == DataButtonCancel) {
        var jItem = $('#' + itemRoot + EditPostfix + ContainerPostFix);
        if (jItem.length == 0) return;
        var jParent = jItem.parent();
        var changeData = new MvcControlsToolkit_changeData(jItem, 'ItemUndoing', 0);
        jParent.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        var temp = eval(itemRoot + SavePostFixD);
        jItem.before(temp.clone(true));
        DataGrid_Remove_Edit_Item(itemRoot);
        $('#' + itemChanged).val('False');
        DataGrid_ChecKDisappearItem(itemRoot);
        MvcControlsToolkit_DataGridApplyStylesItem(itemRoot);
        changeData = new MvcControlsToolkit_changeData($('#' + itemRoot + DisplayPostfix + ContainerPostFix), 'ItemUndone', 0);
        jParent.trigger('itemChange', changeData);
    }
    else if (dataButtonType == DataButtonResetRow) {
        var jItem = $('#' + itemRoot + EditPostfix + ContainerPostFix);
        if (jItem.length == 0) return;
        var jParent = jItem.parent();
        var changeData = new MvcControlsToolkit_changeData(jItem, 'ItemResetting', 0);
        jParent.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        var temp = eval(itemRoot + SavePostFixD);
        $('#' + itemRoot + EditPostfix + ContainerPostFix).before(temp.clone(true));
        DataGrid_Remove_Edit_Item(itemRoot);
        DataGrid_Display_Edit_Item(itemRoot);
        $('#' + itemRoot + DisplayPostfix + ContainerPostFix).remove();
        MvcControlsToolkit_DataGridApplyStylesItem(itemRoot);
        changeData = new MvcControlsToolkit_changeData(jItem, 'ItemReset', 0);
        jParent.trigger('itemChange', changeData);
    }
    else if (dataButtonType == DataButtonUndelete) {
        var undel = eval(itemRoot + SavePostFixU);
        var jItem = null;
        if (undel != null) {
            jItem=$('#' + itemRoot + UndeletePostfix + ContainerPostFix);
        }
        else {
            jItem = $('#' + itemRoot + DisplayPostfix + ContainerPostFix);
        }
        if (jItem.length == 0) return;
        var jParent = jItem.parent();
        var changeData = new MvcControlsToolkit_changeData(null, 'ItemUndeleting', 0);
        jParent.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        
        var temp = eval(itemRoot + SavePostFixD);
        if (undel != null) {

            jItem.before(temp.clone(true));
            jItem.remove();
        }
        else {
            jItem.replaceWith(temp.clone(true));
        }
        eval(itemRoot + DeletedPostFix + " = false;");
        $('#' + itemChanged).val('False');
        MvcControlsToolkit_DataGridApplyStylesItem(itemRoot);
        changeData = new MvcControlsToolkit_changeData($('#' + itemRoot + DisplayPostfix + ContainerPostFix), 'ItemUndeleted', 0);
        jParent.trigger('itemChange', changeData);
    }
    

}



function DataGrid_Prepare_Template(itemRoot, itemChanged, deleted, root) 
    {
        var allJavascript = CollectAllScriptsInelement(itemRoot + EditPostfix + ContainerPostFix) + CollectAllScriptsInelement(itemRoot + DisplayPostfix + ContainerPostFix);
        eval(root + TemplateAllJavascriptPostfix + ' = allJavascript;');
        var editTemplateElement=$('#' + itemRoot + EditPostfix + ContainerPostFix);
        var displayTemplateElement = $('#' + itemRoot + DisplayPostfix + ContainerPostFix);
        editTemplateElement.find('script').remove();
        displayTemplateElement.find('script').remove();

        var temp = null;
        if (editTemplateElement.hasClass("MVCCT_EncodedTemplate")) {
            temp = editTemplateElement.text();
        }
        else {
            temp = $('<div>').append(editTemplateElement.clone()).remove().html();
        }
        eval(root + TemplateEditHtmlPostfix + ' = temp;');
        if (displayTemplateElement.hasClass("MVCCT_EncodedTemplate")) {
            temp = displayTemplateElement.text();
        }
        else {
            temp = $('<div>').append(displayTemplateElement.clone()).remove().html();

        } 
        eval(root + TemplateDisplayHtmlPostfix + ' = temp;');



        editTemplateElement.remove();
        displayTemplateElement.remove();
    }

function MvcControlsToolkit_DataGridApplyStylesItem(itemRoot){
    var root = itemRoot.substring(0, itemRoot.lastIndexOf('___'));
    root = root.substring(0, root.lastIndexOf('___'));
    MvcControlsToolkit_DataGridApplyStyles(root);
}

function MvcControlsToolkit_DataGridApplyStyles(rootName){
    var root = eval(rootName+MvcControlsToolkit_DatagridFatherItemsPostfix);
    if (root == null) return;
    var css = eval (rootName + MvcControlsToolkit_DatagridCssPostfix);
    var altCss = eval(rootName + MvcControlsToolkit_DatagridAltCssPostfix);
    var alt = false;
     for (i = 0; i < root.childNodes.length; i++) {
         var nodeId = root.childNodes[i].id;
         if (nodeId == null) continue;
        var end_prefix = nodeId.lastIndexOf("_");
        if (end_prefix < 0) continue;
        var ending = nodeId.substring(end_prefix );
        if (ending != ContainerPostFix) continue;
        if ($(root.childNodes[i]).css('display') == 'none') continue;
        if (alt) {
            if (css != '') $(root.childNodes[i]).removeClass(css);
            if (altCss != '') $(root.childNodes[i]).addClass(altCss);
        }
        else {
            if (altCss != '') $(root.childNodes[i]).removeClass(altCss);
            if (css != '') $(root.childNodes[i]).addClass(css);
        }
        alt=!alt;
    }
}

function DataGrid_Prepare_Item(itemRoot, itemChanged, deleted, root) {
    var temp = eval(root + AllNormalPostfix);
    if (temp == null) {
        temp = new Array();
    }
    temp.push(itemRoot);
    eval(root + AllNormalPostfix + " = temp");
    $('#' + itemRoot + OldEditPostfix + ContainerPostFix).find('script').remove();
    temp = $('#' + itemRoot + OldEditPostfix + ContainerPostFix).detach();
    
    $('#' + itemRoot + EditPostfix + ContainerPostFix).find('script').remove();
    if (temp.length == 0)
        temp = $('#' + itemRoot + EditPostfix + ContainerPostFix).clone(true);
    temp.css('display', '');

    eval(itemRoot + SavePostFix + " = temp;");

    temp = $('#' + itemRoot + DisplayPostfix + ContainerPostFix);
    temp.find('script').remove();
    var parent = temp.parent();
    if (parent.length > 0) {
        parent=parent[0];
        eval(root+MvcControlsToolkit_DatagridFatherItemsPostfix+" = parent;")
    }
    temp=temp.clone(true);
    eval(itemRoot + SavePostFixD + " = temp;");
    temp.css('display', '');

    temp = $('#' + itemRoot + ChangedExternallyPostfix + ContainerPostFix);
    temp.find('script').remove();
    temp=temp.detach();
    temp.css('display', '');
    if (temp != null && temp.size() == 0)
        temp = null;

    eval(itemRoot + SavePostFixC + " = temp;");

    var deletedItem = $('#' + itemRoot + UndeletePostfix + ContainerPostFix);
    deletedItem.find('script').remove();
    temp = deletedItem;

    if (temp != null && temp.size() == 0)
        temp = null;

    eval(itemRoot + SavePostFixU + " = temp;");

    if (deleted) {
        var editItem = $('#' + itemRoot + EditPostfix + ContainerPostFix);
        DataGrid_Remove_Edit_Item(itemRoot);
        editItem.css('display', '');
        if (temp == null)
            $('#' + itemRoot + DisplayPostfix + ContainerPostFix).css('display', 'none');
        else {
            $('#' + itemRoot + DisplayPostfix + ContainerPostFix).remove();
            deletedItem.css('display', '');
        }
        eval(itemRoot + DeletedPostFix + " = true;");
    }
    else {
        deletedItem = deletedItem.detach();
        deletedItem.css('display', '');
        var editItem = $('#' + itemRoot + EditPostfix + ContainerPostFix);
        if (eval(itemChanged + VarPostfix)) {

            DataGrid_Remove_Edit_Item(itemRoot);
            $('#' + itemRoot + DisplayPostfix + ContainerPostFix).css('display', '');
        }
        else {
            $('#' + itemRoot + DisplayPostfix + ContainerPostFix).remove();
        }
        editItem.css('display', '');
        eval(itemRoot + DeletedPostFix + " = false;");
    }
}
////////////////////////////////////SORTABLE and PERMUTATIONS////////////////////////////////
var SortableList_PermutationUpdateRootPrefix = '_PermutationUpdateRoot';
var SortableList_CanSortPrefix = '_CanSort';
var SortableList_ElementsNumberPrefix = '_ElementsNumber';
var SortableList_TemplateSymbolPrefix = '_TemplateSymbol';
var SortableList_TemplateSriptPrefix = '_TemplateSript';
var SortableList_TemplateHtmlPrefix = '_TemplateHtml';
var SortableList_PermutationPrefix = '_Permutation';
var SortableList_PermutationNamePrefix = '.Permutation';
var SortableList_UpdateModelPrefix = '___';
var SortableList_UpdateModelNamePrefix = '.$$';
var SortableList_UpdateModelFieldsPrefix = '_f_ields';
var SortableList_UpdateModelFieldsNamePrefix = '.f$ields';
var SortableList_ItemsContainerPrefix = '_ItemsContainer';
var SortableList_OriginalElementsNumber = '_OriginalElementsNumber';
var SortableList_TemplateHiddenPrefix = '_TemplateHidden';
var SortableList_TemplateHiddenHtmlPrefix = '_TemplateHiddenHtml';
var SortableList_NamePrefixPrefix = '_NamePrefix';
var SortableList_CssPostFix = "_Css";
var SortableList_AltCssPostFix = "_AltCss";

function MvcControlsToolkit_SortableList_ItemName(item) {
   return item.id;

}


function MvcControlsToolkit_SortableList_PrepareTemplate(root, templateId) {
    MvcControlsToolkit_SortableList_PrepareTemplates(root, [templateId]);
}
function MvcControlsToolkit_SortableList_PrepareTemplates(root, templatesId) {
    eval(root + SortableList_TemplateSriptPrefix + ' = new Array();');
    eval(root + SortableList_TemplateHtmlPrefix + ' = new Array();');
    eval(root + SortableList_TemplateHiddenHtmlPrefix + ' = new Array();');
    for (var i = 0; i < templatesId.length; i++) {
        var templateId = templatesId[i];
        var templateElement = $('#' + templateId);
        
        var allJavascript = CollectAllScriptsInelement(templateId);
        eval(root + SortableList_TemplateSriptPrefix + '[i] = allJavascript;');

        $('#' + templateId).find('script').remove();
        var temp=null;
        if (templateElement.hasClass("MVCCT_EncodedTemplate")) 
        {
            temp = templateElement.text();
        }
        else{
            temp = $('<div>').append(templateElement.clone()).remove().html();
        }
        eval(root + SortableList_TemplateHtmlPrefix + '[i] = temp;');

        var hidden = eval(root + SortableList_TemplateHiddenPrefix);
        if (hidden.constructor == Array) {
            hidden = eval(root + SortableList_TemplateHiddenPrefix + '[i]');
        }
        
        temp = $('<div>').append($('#' + hidden).clone()).remove().html();
        eval(root + SortableList_TemplateHiddenHtmlPrefix + '[i] = temp;');

        $('#' + templateId).remove();
    }

    var canSort = eval(root + SortableList_CanSortPrefix);
    if (canSort){
        var elementNumber = eval(root + SortableList_ElementsNumberPrefix);

        var updateModel = document.getElementById(root + SortableList_UpdateModelPrefix + elementNumber );
        updateModel.setAttribute('id', updateModel.id+'_');


        var updateModelFields = document.getElementById(root + SortableList_UpdateModelPrefix + elementNumber  + SortableList_UpdateModelFieldsPrefix);
        updateModelFields.setAttribute('id', updateModelFields.id+'_'); 
    }
   
}
function MvcControlsToolkit_SortableList_AddNew(root, item, after) {
    if (typeof (root) != 'string') {
        root = root.id;
        var end_prefix = root.lastIndexOf("_");
        root = root.substring(0, end_prefix);
    }
    MvcControlsToolkit_SortableList_AddNewChoice(root, 0, item, after);
}
function MvcControlsToolkit_SortableList_AddNewChoice(root, choice, item, after, replace) {
    if (eval("typeof  " + root + SortableList_ElementsNumberPrefix + " === 'undefined'")) return;
    var rootElement = $('#' + root + SortableList_ItemsContainerPrefix);
    var changeData = new MvcControlsToolkit_changeData(null, 'ItemCreating', choice);
    rootElement.trigger('itemChange', changeData);
    if (changeData.Cancel == true) return;
    var elementNumber = eval(root + SortableList_ElementsNumberPrefix);
    var originalElementNumber = eval(root + SortableList_OriginalElementsNumber);
    var templateSymbol = eval(root + SortableList_TemplateSymbolPrefix);
    if (templateSymbol.constructor == Array) {
        templateSymbol = eval(root + SortableList_TemplateSymbolPrefix + '[choice]');
    }
    var hidden = eval(root + SortableList_TemplateHiddenPrefix);
    if (hidden.constructor == Array) {
        hidden = eval(root + SortableList_TemplateHiddenPrefix + '[choice]');
    }
    var allJavascript = eval(root + SortableList_TemplateSriptPrefix + '[choice]').replace(templateSymbol, elementNumber + '');
    var allHtml = eval(root + SortableList_TemplateHtmlPrefix + '[choice]').replace(templateSymbol, elementNumber + '');
    var hiddenElementFather = $('#' + hidden).parent();
    var hiddenElement = eval(root + SortableList_TemplateHiddenHtmlPrefix + '[choice]').replace(templateSymbol, elementNumber + '');
    var canSort = eval(root + SortableList_CanSortPrefix);
    var namePrefix = eval(root + SortableList_NamePrefixPrefix);

    elementNumber++;
    eval(root + SortableList_ElementsNumberPrefix + ' = elementNumber;');

    if (canSort) {
        var permutation = document.getElementById(root + SortableList_PermutationPrefix);
        permutation.setAttribute('name', namePrefix + SortableList_UpdateModelNamePrefix + elementNumber + ".$" + SortableList_PermutationNamePrefix);

        var updateModel = document.getElementById(root + SortableList_UpdateModelPrefix + originalElementNumber + '_');
        updateModel.setAttribute('name', namePrefix + SortableList_UpdateModelNamePrefix + elementNumber);


        var updateModelFields = document.getElementById(root + SortableList_UpdateModelPrefix + originalElementNumber + SortableList_UpdateModelFieldsPrefix + '_');
        updateModelFields.setAttribute('name', namePrefix + SortableList_UpdateModelNamePrefix + elementNumber + SortableList_UpdateModelFieldsNamePrefix);
    }

    hiddenElementFather.append(hiddenElement);
    var result;
    if (item == null) {
        var hasFooter = false;
        var lastChild = $('#' + root + "_Footer");
        if (lastChild.length > 0) {
            hasFooter = true;
        }
        if (hasFooter) {
            result = $(allHtml).insertBefore(lastChild);
        }
        else {
            result = $(allHtml).appendTo(rootElement);
        }
    }
    else {
        if (after != true) result = $(allHtml).insertBefore(item);
        else result = $(allHtml).insertAfter(item);
        if (replace) {
            MvcControlsToolkit_Bind(item, result[0]);
            $(item).remove();
        }
    }

    if (typeof $ !== 'undefined' && $ !== null && typeof $.validator !== 'undefined' && $.validator !== null && typeof $.validator.unobtrusive !== 'undefined' && $.validator.unobtrusive !== null) {
        jQuery.validator.unobtrusive.parseExt('#' + result[0].id)
    }

    jQuery.globalEval(allJavascript);
    result.data("ScriptsRemoved", true);
    Update_Permutations_Root(root);
    if (canSort) {

        $('#' + root + SortableList_ItemsContainerPrefix).sortable("refresh");
    }
    changeData = new MvcControlsToolkit_changeData(result, 'ItemCreated', choice);
    rootElement.trigger('itemChange', changeData);
    return result;
}

function MvcControlsToolkit_SortableList_ComputeRoot(itemName) {
    var place = itemName.lastIndexOf("___");
    if (place < 0) return null;
    var rootName = itemName.substring(0, place);
    place = rootName.lastIndexOf("___");
    rootName = rootName.substring(0, place);
    if (place < 0) return null;
    return rootName;
}
function MvcControlsToolkit_SortableListUpdate(item, senderId) {
    if (senderId != item.parent().attr('id')) return;
    var nodeName = item.attr('id');
    if (nodeName == null) return;
    var rootName = MvcControlsToolkit_SortableList_ComputeRoot(nodeName);
    Update_Permutations_Root(rootName);
    var rootElement = $('#' + rootName + '_ItemsContainer');
    var changeData = new MvcControlsToolkit_changeData(item, 'ItemMoved', 0);
    rootElement.trigger('itemChange', changeData);
}
function Update_Permutations(itemName) {

    return Update_Permutations_Root(MvcControlsToolkit_SortableList_ComputeRoot(itemName));
}

function Update_Permutations_Root(rootName) {
    if (rootName == null) return;
    var root = document.getElementById(rootName + '_ItemsContainer');
    if (root == null) return;
    if (root.childNodes.length > 0 &&
        root.childNodes[root.childNodes.length - 1].nodeType == 1 &&
        root.childNodes[root.childNodes.length - 1].outerHTML != "" &&
        root.childNodes[root.childNodes.length - 1].id == "") {
        root = root.childNodes[root.childNodes.length - 1];
    }
    var field = document.getElementById(rootName + '_Permutation');
    var alt = false;
    var css = eval(rootName + SortableList_CssPostFix);
    var altCss = eval(rootName + SortableList_AltCssPostFix);
    for (i = 0; i < root.childNodes.length; i++) {
        var currNode = root.childNodes[i];
        var jCurrNode = $(currNode);
        var prova = jCurrNode.data("cavolo") || null;
        if (jCurrNode.data("ScriptsRemoved") !== true) {
            CollectScriptAndDestroy(currNode);
            jCurrNode.data("ScriptsRemoved", true);
        }
        var nodeId = root.childNodes[i].getAttribute('id');
        if (nodeId == null) continue;
        var end_prefix = nodeId.lastIndexOf("_");
        if (end_prefix < 0) continue;
        var ending = nodeId.substring(end_prefix + 1);
        if (ending == 'Header' || ending == 'Footer') continue;
        if (alt) {
            if (css != '') $(root.childNodes[i]).removeClass(css);
            if (altCss != '') $(root.childNodes[i]).addClass(altCss);
        }
        else {
            if (altCss != '') $(root.childNodes[i]).removeClass(altCss);
            if (css != '') $(root.childNodes[i]).addClass(css);
        }
        alt = !alt;
    }
    if (field == null) {

        return;
    }

    var res = '';
    for (i = 0; i < root.childNodes.length; i++) {

        var nodeId = root.childNodes[i].getAttribute('id');
        var end_prefix = nodeId.lastIndexOf("_");
        if (end_prefix < 0) continue;
        var deleteName = nodeId.substring(0, end_prefix + 1) + "Deleted";
        var deletedHidden = document.getElementById(deleteName);
        if (deletedHidden != null && deletedHidden.value == "True") continue;
        var end = nodeId.lastIndexOf("___");
        var order = nodeId.substring(0, end);
        var start = order.lastIndexOf("_") + 1;
        order = order.substring(start);
        if (i > 0) res = res + ',';
        res = res + order;
    }
    field.value = res;
}
function MvcControlsToolkit_SortableList_Click(target, dataButtonType, noEvents) {
    if (typeof (target) != 'string') target = target.id;
    if (dataButtonType == ManipulationButtonCustom) {
        eval(target);
        return;
    }
    var end_prefix = target.lastIndexOf("_");
    var deleteName = target.substring(0, end_prefix);
    end_prefix = deleteName.lastIndexOf("_");
    deleteName = deleteName.substring(0, end_prefix + 1) + "Deleted";
    var deletedHidden = document.getElementById(deleteName);
    var rootName = MvcControlsToolkit_SortableList_ComputeRoot(target);
    var rootElement = $('#' + rootName + SortableList_ItemsContainerPrefix);
    if (dataButtonType == ManipulationButtonRemove) {
        var item = $('#' + target);
        var changeData = new MvcControlsToolkit_changeData(item, 'ItemDeleting', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        item.remove();
        Update_Permutations_Root(rootName);
        changeData = new MvcControlsToolkit_changeData(item, 'ItemDeleted', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
    }
    else if (dataButtonType == ManipulationButtonHide) {
        var item = $('#' + target);
        var changeData = new MvcControlsToolkit_changeData(item, 'ItemDeleting', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        var placeHolder = "<span style='display:none' id = '" + target + "_ph' />";
        $(placeHolder).insertBefore('#' + target);
        jQuery.globalEval("var " + deleteName + " = $('#' + '" + target + "').detach();");
        if (deletedHidden != null) deletedHidden.value = "True";
        Update_Permutations_Root(rootName);
        changeData = new MvcControlsToolkit_changeData(item, 'ItemDeleted', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
    }
    else if (dataButtonType == ManipulationButtonShow) {
        var changeData = new MvcControlsToolkit_changeData(null, 'ItemCreating', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        var toAdd = eval(deleteName);
        toAdd.find('script').remove();
        $(toAdd).insertBefore('#' + target + '_ph');
        $('#' + target + '_ph').remove();
        if (deletedHidden != null) deletedHidden.value = "False";
        Update_Permutations_Root(rootName);
        changeData = new MvcControlsToolkit_changeData(toAdd, 'ItemCreated', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
    }


}
function MvcControlsToolkit_SortableList_Move(item, target, after) {
    if (after != true) $(item).insertBefore(target);
    else $(item).insertAfter(target);
    var rootName = MvcControlsToolkit_SortableList_ComputeRoot(item.id);
    Update_Permutations_Root(rootName);
    var rootElement = $('#' + rootName + '_ItemsContainer');
    var changeData = new MvcControlsToolkit_changeData(item, 'ItemMoved', 0);
    rootElement.trigger('itemChange', changeData);
}
///////////////////////////////////MANIPULATION BUTTONS/////////////////////////

var ManipulationButtonRemove = "ManipulationButtonRemove";
var ManipulationButtonHide = "ManipulationButtonHide";
var ManipulationButtonShow = "ManipulationButtonShow";
var ManipulationButtonResetGrid = "ManipulationButtonResetGrid";
var ManipulationButtonCustom = "ManipulationButtonCustom";

function ManipulationButton_Click(target, dataButtonType) {
    if (dataButtonType == ManipulationButtonCustom) {
        if (typeof target === "string") eval(target);
        else target();
        return;
    }
    
    if (dataButtonType == ManipulationButtonRemove) {
        $('#' + target).remove();
    }
    else if (dataButtonType == ManipulationButtonHide) {

        $('#' + target).css('visibility', 'hidden');

    }
    else if (dataButtonType == ManipulationButtonShow) {

        $('#' + target).css('visibility', 'visible');
    }
    else if (dataButtonType == ManipulationButtonResetGrid) {
        var toUndo = eval(target + AllNormalPostfix);
        if (toUndo != null) {
            for (var i = 0; i < toUndo.length; i++) {
                var vChanged = toUndo[i].substring(0, toUndo[i].lastIndexOf("_")) + ChandedPostfix;
                var deleted = eval(toUndo[i] + DeletedPostFix);
                if (deleted != null && deleted == true)
                    DataButton_Click(toUndo[i], vChanged, DataButtonUndelete);
                else
                    DataButton_Click(toUndo[i], vChanged, DataButtonCancel);
            }
        }

    }
    

}
function MvcControlsToolkit_Button_AdjustText(buttonName, newText) {
    var button = document.getElementById(buttonName);

    var nodeTag = button.nodeName.toLowerCase();

    if (nodeTag == 'input') button.value = newText;
    else if (nodeTag == 'img') button.setAttribute('src', newText);
    else if (nodeTag == 'a') button.firstChild.nodeValue = newText;

}


///////////////////////////////PAGER///////////////////////////////////



function PageButton_Click(pageField, pageValue, pageUrl, targetId, validationType) {
    if (pageUrl == '') {
        if (!MvcControlsToolkit_FormIsValid(pageField, validationType)) return;

        var field = document.getElementById(pageField);
        field.value = pageValue;
        $('#' + pageField).parents('form').submit();
    }
    else if (targetId != '') {
        $.ajax({
            type: 'GET',
            url: pageUrl,
            success: function (data) {
                $('#' + targetId).html(data);
            }
        });
    }
    else {
        window.location.href = pageUrl;
    }
}
function MvcControlsToolkit_RefreshPager(pagerId) {
    var enabled = function (jButton, yes) {
        if (!yes || yes == 'curr') {
            if (yes == "curr") {
                jButton.attr("data-selected-page", "selected");
            }
            else {
                jButton.attr("disabled", "disabled");
                if (jButton[0].tagName.toLowerCase() == 'img') {
                    var enabledUrl = jButton.attr('data-enabled-src') || '';
                    if (enabledUrl != '') jButton.attr('src') = enabledUrl;
                }
            }
            jButton.attr('data-no-click', true);
        }
        else {
            jButton.removeAttr("disabled");
            jButton.removeAttr("data-selected-page");
            if (jButton[0].tagName.toLowerCase() == 'img') {
                var disabledUrl = jButton.attr('data-disabled-src') || '';
                if (disabledUrl != '') jButton.attr('src') = disabledUrl;
            }
            jButton.removeAttr('data-no-click');
        }
    };
    var pager = $("#" + pagerId);
    var currPage = parseInt(pager.val());
    var totPages = pager.attr("data-total-pages") || '';
    if (totPages != '') totPages = parseInt(totPages);
    if (currPage < 1) currPage = 1;
    var prefix = pager.attr('data-page-prefix') || '';
    var postfix = pager.attr('data-page-postfix') || '';
    MvcControlsToolkit_TypedTextBox_SetById(pagerId + "_goto", currPage, null, 1);
    $("." + pagerId + "_class").each(function () {
        var jThis = $(this);
        var type = jThis.attr("data-pager-button");
        if (type == "PageButtonFirst" || type == "PageButtonPrevious") {
            if (currPage == 1) {
                enabled(jThis, false);
            }
            else enabled(jThis, true);
        }
        else if (type == "PageButtonNext") {
            if (totPages != '' && currPage >= totPages) enabled(jThis, false);
            else enabled(jThis, true);
        }
        else if (type == "PageButtonLast") {
            if (totPages == '' || currPage >= totPages) enabled(jThis, false);
            else enabled(jThis, true);
        }
        else if (type == "PageButtonPage") {
            var index = parseInt(jThis.attr("data-pager-index"));
            if (currPage + index < 1 || (totPages != '' && currPage + index > totPages)) jThis.parent().hide();
            else {
                jThis.text(prefix + (currPage + index) + postfix);
                jThis.parent().show();
                if (index == 0) enabled(jThis, 'curr');
                else enabled(jThis, true);
            }
        }
    });
}

function MvcControlsToolkit_InitJsonPager(pagerId) {
    $(document).ready(function () {
        var pager = $("#" + pagerId);
        var causeSubmit = (pager.attr('data-cause-submit') || '') != '';
        var validationType = pager.attr('data-validation-type') || '';
        $("." + pagerId + "_class").click(function () {
            var jThis = $(this);
            if ((jThis.attr('data-no-click') || false)) return false;
            var type = jThis.attr("data-pager-button");
            var prevPage = pager.val();
            if (!causeSubmit || MvcControlsToolkit_FormIsValid(pagerId, validationType))
                if (type == "PageButtonFirst") {
                    pager.val(1);
                    pager.trigger("ClientPager_Changed");
                }
                else if (type == "PageButtonPrevious") {
                    var currPage = parseInt(pager.val());
                    pager.val(currPage - 1);
                    pager.trigger("ClientPager_Changed");
                }
                else if (type == "PageButtonNext") {
                    var currPage = parseInt(pager.val());
                    pager.val(currPage + 1);
                    pager.trigger("ClientPager_Changed");
                }
                else if (type == "PageButtonLast") {
                    var totPages = pager.attr("data-total-pages") || '';
                    if (totPages != '') {
                        pager.val(parseInt(totPages));
                        pager.trigger("ClientPager_Changed");
                    }
                }
                else if (type == "PageButtonPage") {
                    var currPage = parseInt(pager.val());
                    var index = parseInt(jThis.attr("data-pager-index"));
                    pager.val(currPage + index);
                    pager.trigger("ClientPager_Changed");
                }
                else if (type == "PageButtonGoTo") {
                    var totPages = pager.attr("data-total-pages") || '';
                    if (totPages != '') totPages = parseInt(totPages);
                    var targetPage = MvcControlsToolkit_TypedTextBox_GetById(pagerId + "_goto", 1);
                    if (targetPage < 1) targetPage = 1;
                    else if (totPages != '' && targetPage > totPages) targetPage = totPages;
                    pager.val(targetPage);
                    pager.trigger("ClientPager_Changed");
                }
            MvcControlsToolkit_RefreshPager(pagerId);
            var totPages = pager.attr("data-total-pages") || '';
            if (totPages != '') totPages = parseInt(totPages);
            var data = {
                type: "page",
                submit: causeSubmit,
                page: parseInt(pager.val()),
                previousPage: parseInt(prevPage),
                totalPages: totPages
            };
            pager.trigger("queryChanged", data);
            if (data.submit) pager.parents('form').first().submit();
        });
        var clientModel = pager.attr('data-client-model') || '';
        var totalPageName = pager.attr('data-total-pages-name') || '';
        if (clientModel != '' && totalPageName != '') {
            var dependentObservable = new ko.dependentObservable(
            function () {
                var pages = ko.utils.unwrapObservable(eval(clientModel + "." + totalPageName));
                pager.attr('data-total-pages', pages + '');
                MvcControlsToolkit_RefreshPager(pagerId);
            },
            null,
            { 'disposeWhenNodeIsRemoved': pager[0] });
            pager.data("__mvcct_totalpagesDependency__", dependentObservable);
        }
        MvcControlsToolkit_RefreshPager(pagerId);
    });
}

function MvcControlsToolkit_NewPage(pagerId, newPage, newTotalPages) {
    var pager = $('#' + pagerId);
    pager.val(newPage);
    pager.attr("data-total-pages", newTotalPages);
    MvcControlsToolkit_RefreshPager(pagerId);
}

function MvcControlsToolkit_ClientPager_Get(source, valueType) {
    return parseInt($(source).val());
}
function MvcControlsToolkit_ClientPager_GetString(source) {
    return $(source).val();
}
function MvcControlsToolkit_ClientPager_GetById(id, valueType) {
    return parseInt($('#' + id).val());
}
function MvcControlsToolkit_ClientPager_Set(source, value, format, valueType) {
    $(source).val(value);
    MvcControlsToolkit_RefreshPager(source.id);
}
function MvcControlsToolkit_ClientPager_SetById(id, value, format, valueType) {
    $('#' + id).val(value);
    MvcControlsToolkit_RefreshPager(id);
}
function MvcControlsToolkit_ClientPager_SetString(source, value) {
    $(source).val(value);
    MvcControlsToolkit_RefreshPager(source.id);
}
function MvcControlsToolkit_ClientPager_BindChange(id, handler) {
    $("#" + id).bind("ClientPager_Changed", handler);
}
function MvcControlsToolkit_ClientPager_UnbindChange(id, handler) {
    $("#" + id).unbind("ClientPager_Changed", handler);
}

///////////////////////////////SORTING///////////////////////////////////

function _inner_Sort_Handler(field, buttonName, initialize, causePostback, clientOrderChanged, sortField, pageField, cssNoSort, cssAscending, cssDescending, validationType, oneColumnSorting) {
    if (!initialize && causePostback && !MvcControlsToolkit_FormIsValid(sortField, validationType)) return;
    var order = $('#' + sortField).val();
    var hasAscending = order.indexOf(' ' + field + '#+;');
    var hasDescending = order.indexOf(' ' + field + '#-;');
    var prevOrder = '';
    var actualOrder = '';
    if (!initialize) {
        if (hasDescending >= 0) {
            if (oneColumnSorting)
                order = order.replace(' ' + field + '#-;', ' ' + field + '#+;');
            else
                order = order.replace(' ' + field + '#-;', '');
            $('#' + sortField).val(order);
            $('#' + buttonName).removeClass(cssDescending);
            if (oneColumnSorting)
                $('#' + buttonName).addClass(cssAscending);
            else
                $('#' + buttonName).addClass(cssNoSort);
            prevOrder = '-';
        }
        else if (hasAscending >= 0) {
            order = order.replace(' ' + field + '#+;', ' ' + field + '#-;');
            $('#' + sortField).val(order);
            $('#' + buttonName).removeClass(cssAscending);
            $('#' + buttonName).addClass(cssDescending);
            prevOrder = '+';
            actualOrder = '-';
        }
        else {
            if (oneColumnSorting) order = ' ' + field + '#+;';
            else order = order + ' ' + field + '#+;';
            $('#' + sortField).val(order);
            $('#' + buttonName).removeClass(cssNoSort);
            $('#' + buttonName).addClass(cssAscending);
            actualOrder = '+';
            if (oneColumnSorting) {
                var prevButton = eval(sortField);
                if (prevButton != '' && prevButton != buttonName) {
                    $('#' + prevButton).removeClass(cssDescending).removeClass(cssAscending).addClass(cssNoSort);
                }
                eval(sortField + ' = buttonName;');
            }
        }
        if (pageField != null) $('#' + pageField).val('1');
        if (clientOrderChanged != null) eval(clientOrderChanged + "('" + field + "', '" + prevOrder + "', '" + actualOrder + "')");
        if (causePostback) $('#' + sortField).parents('form').submit();
    }
    else {
        if (hasDescending >= 0) {
            $('#' + buttonName).addClass(cssDescending);
            if (oneColumnSorting) eval(sortField + ' = buttonName;');
        }
        else if (hasAscending >= 0) {
            $('#' + buttonName).addClass(cssAscending);
            if (oneColumnSorting) eval(sortField + ' = buttonName;');
        }
        else {
            $('#' + buttonName).addClass(cssNoSort);
        }
    }
}
function Sort_Handler(field, buttonName, initialize, causePostback, clientOrderChanged, sortField, pageField, cssNoSort, cssAscending, cssDescending, validationType, oneColumnSorting) {
    var sortFieldOb = $('#' + sortField);
    if (!initialize && causePostback && !MvcControlsToolkit_FormIsValid(sortField, validationType)) return;
    _inner_Sort_Handler(field, buttonName, initialize, false, clientOrderChanged, sortField, pageField, cssNoSort, cssAscending, cssDescending, validationType, oneColumnSorting);
    if (!initialize) {

        var data = {
            type: "sort",
            sortString: sortFieldOb.val(),
            submit: causePostback,
            page: 1

        };
        if (pageField != null) {
            var pager = $('#' + pageField);
            MvcControlsToolkit_RefreshPager(pageField);
            pager.trigger("ClientPager_Changed");
            var totPages = pager.attr("data-total-pages") || '';
            if (totPages != '') totPages = parseInt(totPages);
            data["pageSize"] = totPages;
        }
        sortFieldOb.trigger("queryChanged", data);
        if (data.submit) sortFieldOb.parents('form').first().submit();
    }


};
////////////////////////////////DETAIL FORM///////////////////////////////////////////////


function Setup_Ajax_ClientValidation(formId, validationType) {
    if (validationType == ValidationType_StandardClient) {
        var allFormOptions = window.mvcClientValidationMetadata;
        if (allFormOptions) {
            for (var i = 0; i < allFormOptions.length; i++) {
                var thisFormOptions = allFormOptions[i];
                thisFormOptions.FormId = formId;
            }
        }
        Sys.Mvc.FormContext._Application_Load();
    }
    else {
        if (typeof $ !== 'undefined' && $ !== null && typeof $.validator !== 'undefined' && $.validator !== null && typeof $.validator.unobtrusive !== 'undefined' && $.validator.unobtrusive !== null) {

            $.validator.unobtrusive.clearAndParse('#' + formId);
        }
    }
    MvcControlsToolkit_ParseRegister.parse('#' + formId);
}

function OnBeginDetailForm(baseName, detailType, rowId, loadingElementId, validationType) {
    detailBusy = eval(baseName + '_DetailBusy');
    if (detailBusy) return false;
    eval(baseName + '_DetailBusy = true;');
    eval(baseName + "_TypeDetail = '" + detailType + "';");
    if (rowId != null) {//rowId null means re-posting on the same column
        if (detailType == 'Insert') {
            eval(baseName + "_CurrentRow = null;"); // grid need not be updated on success
        }
        else {
            eval(baseName + "_CurrentRow = '" + rowId + "';");
        }
    }
    else {
        var res = MvcControlsToolkit_FormIsValid(loadingElementId, validationType);
        return res;
    }
    return true;
}

function OnFailureDetailForm(baseName, displayExecute, editExecute) {
    var detailType = eval(baseName + '_TypeDetail');
    if (detailType == 'Display') {
        if (displayExecute != null) (displayExecute + '();');
    }
    else {
        if (editExecute != null) (editExecute + '();');
    }
    eval(baseName + '_DetailBusy = false;');
}

function TrueValue(value, fieldName) {
    var trueValue = value;
    try {
        trueValue = eval(fieldName + "_True");
    }
    catch (e) {

    }
    return trueValue;
}

function FormattedValue(value, fieldName) {
    var fValue = value;
    try {
        fValue = eval(fieldName + "_Format");
    }
    catch (e) {

    }
    if (fValue != null && fValue.length > 0 && fValue.substring(0, 1) == '<') {
        fValue = $('#' + fieldName + '_Format').text();
        $('#' + fieldName + '_Format').remove();
    }
    var companionField = document.getElementById(fieldName + "_hidden");
    if (companionField != null) {
        fValue = companionField.value; 
    }
    return fValue;
}

function UrlValue(fieldName) {
    var urlValue = null;
    try {
        urlValue = eval(fieldName + "_Url");
    }
    catch (e) {

    }
    return urlValue;
}

function OnSuccessDetailForm(baseName, displayExecute, editExecute, ajaxContext, formId, validationType, unobtrusiveAjaxOn, updateTargetId) {
    var detailType = eval(baseName + '_TypeDetail');
    if (!unobtrusiveAjaxOn) GlobalEvalScriptAndDestroy(ajaxContext.get_updateTarget());
    else if (detailType == 'Edit') $('#' + updateTargetId).html(ajaxContext);
    if (validationType != ValidationType_Server) Setup_Ajax_ClientValidation(formId, validationType);
    var detailType = eval(baseName + '_TypeDetail');
    var changedFieldCss = eval(baseName + '_ChangedFieldCss');
    var deletedRecordCss = eval(baseName + '_DeletedRecordCss');
    var isValid = false;
    var hiddenIsValid = document.getElementById("IsValid");
    if (hiddenIsValid != null && hiddenIsValid.value == "True") isValid = true;
    var itemRoot = eval(baseName + "_CurrentRow");
    if (isValid && (detailType == 'FirstEdit' || detailType == 'Edit' || detailType == 'Display')) {       
       var fieldsToUpdate = eval(baseName + "_FieldsToUpdate").split(",");
        var detailRoot = eval(baseName + "_DetailPrefix");
        if (itemRoot != null) {
            var oldItemRoot = itemRoot.substring(0, itemRoot.lastIndexOf('Value')) + 'OldValue';
            var isInsert = true;
            var isExternalDelete = true;
            var changedDisplay = eval(itemRoot + SavePostFixC);
            var changedDisplayAvailable = false;
            if (changedDisplay == null) changedDisplay = eval(itemRoot + SavePostFixD);
            else changedDisplayAvailable = true;
            for (var i = 0; i < fieldsToUpdate.length; i++) {
                var oField = document.getElementById(oldItemRoot + "_" + fieldsToUpdate[i]);
                var oFields = document.getElementById(oldItemRoot + '_' + fieldsToUpdate[i] + '___JSonModel');
                if (oField == null) 
                    oField = oFields;
                if (oField != null) isInsert = false;
                var newFieldName = null;
                if (detailRoot.length == 0) {
                    newFieldName = fieldsToUpdate[i];
                }
                else {
                    newFieldName = oldItemRoot + "_" + fieldsToUpdate[i];
                }
                var newField = document.getElementById(newFieldName);
                if (newField != null) isExternalDelete = false;
                if (oField != null) {
                    var newValue = null;
                    var newFormattedValue = null;
                    if (newField != null && newField.getAttribute('type') != null && (newField.getAttribute('type').toLowerCase() == 'checkbox' || newField.getAttribute('type').toLowerCase() == 'radio')) {
                        if (newField.getAttribute('checked') != null && newField.checked == true) newValue = 'True';
                        else newValue = 'False';
                        newFormattedValue = FormattedValue(newValue, newFieldName);
                    }
                    else  if (newField != null && newField.getAttribute('type') != null && newField.getAttribute('type').toLowerCase() == 'file'){
                    }
                    else {
                        if (newField != null) {
                            try{
                                if (newField.nodeName != null && (newField.nodeName.toLowerCase() == "input" || newField.nodeName.toLowerCase() == "textarea")) {
                                    newValue = TrueValue(newField.value, newFieldName);
                                    newFormattedValue = FormattedValue(newField.value, newFieldName);
                                }
                                else if (newField.nodeName != null && newField.nodeName.toLowerCase() == "select") {
                                    var jNewField = $(newField);
                                    newValue = TrueValue(jNewField.val(), newFieldName);
                                    if (newValue == '') newValue = null;
                                    newFormattedValue = FormattedValue(jNewField.find('option:selected').text(), newFieldName);
                                }
                                else {
                                    newValue = TrueValue(newField.childNodes[0].nodeValue, newFieldName);
                                    newFormattedValue = FormattedValue(newField.childNodes[0].nodeValue, newFieldName);
                                }
                            }
                            catch(e){
                            }
                        }
                        else {
                            newValue = TrueValue(null, newFieldName);
                            newFormattedValue = FormattedValue(null, newFieldName);
                        }
                    }
                    var itemToUpdate = changedDisplay.find('#' + itemRoot + '_' + fieldsToUpdate[i]);
                    var originalValue;
                    var jOField = $(oField);
                    if (typeof (jOField.data('originalvalue')) !== 'undefined') {
                        originalValue = jOField.data('originalvalue');
                        if (originalValue == null || originalValue == undefined) {
                            originalValue = oField.value;
                            jOField.data('originalvalue', oField.value);
                        }
                    }
                    else{
                        originalValue = oField.value;
                        jOField.data('originalvalue', oField.value);
                    }
                    if (changedFieldCss != null) {
                        if (newValue != null && originalValue != newValue) {
                            itemToUpdate.removeClass(changedFieldCss);
                            itemToUpdate.addClass(changedFieldCss);

                        }
                        else {
                            itemToUpdate.removeClass(changedFieldCss);
                        }
                    }
                    if (newValue != null && oField.value != newValue) {
                        oField.value = newValue;
                        if (itemToUpdate.length != 0) {
                            var inputType = itemToUpdate.attr('type');
                            if (inputType != null && inputType != undefined && (inputType.toLowerCase() == 'checkbox' || inputType.toLowerCase() == 'radio')) {
                                if (newValue.toLowerCase() == 'true') itemToUpdate.checked = true;
                                else itemToUpdate.checked = false;
                            }
                            else {
                                if (itemToUpdate.filter('img').length > 0) {
                                    var imgUrl = UrlValue(newFieldName);
                                    if (imgUrl != null) {
                                        try {
                                            itemToUpdate.attr('src', imgUrl);
                                            itemToUpdate.attr('alt', newFormattedValue);
                                        }
                                        catch (e) {
                                        }
                                    }
                                }
                                else {
                                    try {
                                        itemToUpdate.html(newFormattedValue);
                                    }
                                    catch (e) {
                                    }
                                }
                            }
                        }
                    }

                }
            }
            if (isExternalDelete) {
                $('input[id^="' + oldItemRoot + '"]').remove();
                if (deletedRecordCss != null) {
                    var newSavePostFixD = eval(itemRoot + SavePostFixD);
                    newSavePostFixD.addClass(deletedRecordCss);
                    $('#' + itemRoot + DisplayPostfix + ContainerPostFix).replaceWith(newSavePostFixD.clone(true));
                }
            }
            else if (!isInsert) {
                if (changedDisplayAvailable)
                    eval(itemRoot + SavePostFixD + " = " + itemRoot + SavePostFixC + ";");
                var newSavePostFixD = eval(itemRoot + SavePostFixD);
                $('#' + itemRoot + DisplayPostfix + ContainerPostFix).replaceWith(newSavePostFixD.clone(true));
            }
            if (itemRoot != null) MvcControlsToolkit_DataGridApplyStylesItem(itemRoot);
        }
    }

    if (detailType == 'Display') {
        if (displayExecute != null) {
            try {
                eval(displayExecute + '(itemRoot, isValid, detailType);');
            }
            catch (e) {
            }
        }
    }
    else {
        if (editExecute != null) {
            try {
                eval(editExecute + '(itemRoot, isValid, detailType);');
            }
            catch (e) {
            }
        }
    }
    
    eval(baseName + '_DetailBusy = false;');
}

////////////////////////////////ViewList and ViewsOnOff///////////////////////////////////////////////

function ViewList_Client(groupName, hiddenField, cssSelected, prefix) {
    this.CssSelected = cssSelected;
    this.GroupName = groupName;
    this.HiddenField = hiddenField;
    this.Prefix = prefix;
    var allViews = $('.' + groupName);
    allViews.each(function (i) {
        var name = this.id + "_placeholder";
        var thisId = this.id;
        $('#' + thisId).before("<span style='display:none;' id='" + name + "'></span>");

        $('.' + thisId + "_checkbox").click(function () {
            if ($("." + thisId + "_checkbox")[0].checked)
                eval(groupName + "_ViewList").Select(thisId, true);
            else
                eval(groupName + "_ViewList").Select('', true);
        });

    });
    this.SelectionSet = allViews.detach();
    this.SelectionSet.find('script').remove();
}

ViewList_Client.prototype = {
    HiddenField: null,
    GroupName: null,
    CssSelected: null,
    SelectionSet: null,
    Prefix: null,
    Select: function (target, internal) {
        $('.' + this.GroupName + '_button').removeClass(this.CssSelected);
        $('.' + this.GroupName + '_checkbox').each(function (i) { this.checked = false });
        $('.' + this.GroupName).detach();

        if (target == '') {
            document.getElementById(this.HiddenField).value = '';
            return;
        }
        document.getElementById(this.HiddenField).value = target;
        if (internal == null) target = this.Prefix + target;
        this.SelectionSet.filter('#' + target).insertBefore('#' + target + '_placeholder');

        $('.' + target + '_button').addClass(this.CssSelected);

        $('.' + target + '_checkbox').each(function (i) { this.checked = true });
    }
}

function ViewsOnOff_Client_Switch(groupName, on, hidden) {
    if (on) {
        $('.' + groupName + '_checkbox').each(function (i) { this.checked = true });
        var toAttach = eval(groupName + "_ViewsOnOff");
        toAttach.each(function (i) {
            var currId = this.id;
            $(this).insertBefore('#' + currId + "_placeholder");
        });
        document.getElementById(hidden).value = "True";
    }
    else {
        $('.' + groupName + '_checkbox').each(function (i) { this.checked = false });
        $('.' + groupName).detach();
        document.getElementById(hidden).value = "False";
    }
}

function ViewsOnOff_Client_Initialize(groupName, initial_on, hidden) {
    var allViews = $('.' + groupName);
    allViews.each(function (i) {
        var prova = this.id;
        if (this.id == null || this.id == '') this.id = groupName + "_el" + i;
        var name = this.id + "_placeholder";
        var thisId = this.id;
        $('#' + this.id).before("<span style='display:none;' id='" + name + "'></span>");

        $('.' + groupName + "_checkbox").click(function (event) {
            ViewsOnOff_Client_Switch(groupName, event.target.checked, hidden);
        });
    });
    var selectionSet = allViews.detach();
    selectionSet.find('script').remove();
    eval(groupName + "_ViewsOnOff = selectionSet;");
    if (initial_on) {
        $('.' + groupName + '_checkbox').each(function (i) { this.checked = true });
        selectionSet.each(function (i) {
            var currId = this.id;
            $(this).insertBefore('#' + currId + "_placeholder");
        });
    }
    else {
        $('.' + groupName + '_checkbox').each(function (i) { this.checked = false });
    }

}
////////////Typed TextBox ////////////////
var MvcControlsToolkit_DataType_String = 0;
var MvcControlsToolkit_DataType_UInt = 1;
var MvcControlsToolkit_DataType_Int = 2;
var MvcControlsToolkit_DataType_Float = 3;
var MvcControlsToolkit_DataType_DateTime = 4;

function MvcControlsToolkit_Format(value, format, dataType, prefix, postfix) {
    if (dataType<0)
        return value;
    if (value == null || value === undefined) return '';
    if (!prefix) prefix = '';
    if (!postfix) postfix = '';
    return prefix + MvcControlsToolkit_ToString(value, format, dataType) + postfix;
}
function MvcControlsToolkit_FormatDisplay(value, format, dataType, prefix, postfix, nullString) {
    if (dataType<0) 
        return value;
    if (value == null || value === '' || value === undefined) return nullString;
    return prefix + MvcControlsToolkit_ToString(value, format, dataType) + postfix;
}
function MvcControlsToolkit_ToString(value, format, dataType) {
    if (value === undefined) return undefined;
    if (dataType == MvcControlsToolkit_DataType_String || dataType < 0) {
        if (value === true) return "True";
        if (value === false) return "False";
        return value;
    }
    if (value == null) return '';
    if (format == '') {
        if (dataType == MvcControlsToolkit_DataType_DateTime) {
            format = 'd';
        }
        else if (dataType == MvcControlsToolkit_DataType_Int ||
            dataType == MvcControlsToolkit_DataType_UInt) {
            format = 'd';
        }
        else if (dataType == MvcControlsToolkit_DataType_Float) {
            format = 'n';
        }
        else {
            return value;
        }
        
    }
    if ((typeof jQuery !== 'undefined') && (typeof jQuery.global !== 'undefined') && (typeof jQuery.global.parseInt === 'function')) {
        if (dataType == MvcControlsToolkit_DataType_DateTime && format == 's') format = 'S';
        return jQuery.global.format(value, format);
    }
    else if ((typeof Number !== 'undefined') && (typeof Number.parseLocale === 'function')) {
        if (dataType == MvcControlsToolkit_DataType_DateTime && format == 'S') format = 's';
        return value.localeFormat(format);
    }
    else {
        return value + '';
    }
}
function MvcControlsToolkit_Parse(value, dataType) {
    if (dataType == MvcControlsToolkit_DataType_String) return value;
    if (value === undefined) return undefined;
    if (value == '') return null;
    if (dataType == MvcControlsToolkit_DataType_Float) {
        if ((typeof jQuery !== 'undefined') && (typeof jQuery.global !== 'undefined') && (typeof jQuery.global.parseFloat == 'function')) {
            return jQuery.global.parseFloat(value);
        }
        else if ((typeof Number !== 'undefined') && (typeof Number.parseLocale == 'function')) {
            return Number.parseLocale(value);
        }
        else {
            return parseFloat(value);
        }
    }
    else if (dataType == MvcControlsToolkit_DataType_DateTime) {
        if ((typeof jQuery !== 'undefined') && (typeof jQuery.global !== 'undefined') && (typeof jQuery.global.parseDate == 'function')) {
            return jQuery.global.parseDate(value);
        }
        else if ((typeof Date !== 'undefined') && (typeof Date.parseLocale == 'function')) {
            return Date.parseLocale(value);
        }
        else {
            return Date.parse(value);
        }
    }
    else {
        if ((typeof jQuery !== 'undefined') && (typeof jQuery.global !== 'undefined') && (typeof jQuery.parseInt == 'function')) {
            return jQuery.global.parseInt(value);
        }
        else if (typeof Number.parseLocale == 'function') {
            var tFloat = Number.parseLocale(value);
            if (isNaN(tFloat)) return tFloat;
            return parseInt(tFloat + '');
        }
        else {
            return parseInt(value, 10);
        }
    }
}

function MvcControlsToolkit_TypedTextBox_Input(charCode, fieldId, companionId, dataType, decimalSeparator, digitSeparator, plus, minus) {
    if (dataType == MvcControlsToolkit_DataType_String || dataType == MvcControlsToolkit_DataType_DateTime ||
    charCode == 0 || charCode == 13 || charCode == 8 || charCode == digitSeparator.charCodeAt(0)
    || (charCode >= 48 && charCode <= 57)) return true;
    if ((dataType == MvcControlsToolkit_DataType_Int || dataType == MvcControlsToolkit_DataType_Float)
    && (charCode == plus.charCodeAt(0) || charCode == minus.charCodeAt(0))) {
        var value = document.getElementById(fieldId).value;
        return value.indexOf(plus) < 0 && value.indexOf(minus) < 0;
    }
    if (dataType == MvcControlsToolkit_DataType_Float && charCode == decimalSeparator.charCodeAt(0)) {
        var value = document.getElementById(fieldId).value;

        return value.indexOf(decimalSeparator) < 0;
    }
    return false;
}
function MvcControlsToolkit_FocusAtEnd(fieldId) {
    var el = document.getElementById(fieldId);
    if (el.setSelectionRange) /* DOM */
    {

        el.setSelectionRange(el.value.length, el.value.length);

    }
    else if (this.createTextRange) /* IE */
    {
        r = el.createTextRange();
        r.collapse(false);
        r.select();
    } 
}
function MvcControlsToolkit_TypedTextBox_Focus(fieldId, companionId, watermarkCss) {
    document.getElementById(fieldId).value = document.getElementById(companionId).value ||'';
    if (watermarkCss != '') $('#' + fieldId).removeClass(watermarkCss);
}
function MvcControlsToolkit_DisplayEdit_DbClick(fieldId, companionId) {
    $('#' + companionId).hide();
    $('#' + fieldId).show().focus();
}
function MvcControlsToolkit_TypedTextBox_Init(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13) {
    $(document).ready(function () {
        var tTb = $('#' + p0);

        tTb.bind('pblur', function () {
            MvcControlsToolkit_TypedTextBox_Blur(p0, p1, p3, p8, p9, p10, p6, p7, p4, p5,
                           p11, p2, p12); return false;
        });
        tTb.bind('pfocus', function () {
            MvcControlsToolkit_TypedTextBox_Focus(p0, p1, p2);
            return false;
        });

        try {
            tTb.trigger('pfocus');
            MvcControlsToolkit_TypedTextBox_Blur(p0, p1, p3, p8, p9, p10, p6, p7, p4, p5,
                           p11, p2, p12, true);
        }
        catch (ex) { }


        tTb.focus(function () { tTb.trigger('pfocus'); MvcControlsToolkit_FocusAtEnd(p0); return true; });
        if (p13 != null) tTb.datepicker(p13);
        if (p13 != null) tTb.datepicker();
        tTb.blur(function () { tTb.trigger('pblur'); return true; });


        tTb.keypress(function (event) {
            return MvcControlsToolkit_TypedTextBox_Input(event.which, p0, p1, p3, p4, p5, p6, p7);
        });

    });
}
function MvcControlsToolkit_TypedTextBox_Blur(
fieldId, companionId, dataType,
pre, post, format, plus, minus, decimalSeparator, digitSeparator,
watermark, watermarkCss, validationType, jumpValidation) {
    var fieldElement = document.getElementById(fieldId);
    if (fieldElement == null) return;
    var value = fieldElement.value;
    
    var innerValue = value;
    if (dataType != MvcControlsToolkit_DataType_String && dataType != MvcControlsToolkit_DataType_DateTime) {
        value = MvcControlsToolkit_Trim(value);
        innerValue = value;
        var tValue = value;
        tValue = tValue.replace(digitSeparator, '');
        tvalue = tValue.replace(plus, '');
        negative = tValue.indexOf(minus);
        tValue = tValue.replace(minus, '');
        var toBuild = '';
        var charCode = '';
        for (var i = 0; i < tValue.length; i++) {
            charCode = tValue.charCodeAt(i);
            if ((charCode >= 48 && charCode <= 57) || charCode == decimalSeparator.charCodeAt(0)) {
                toBuild = toBuild + tValue.charAt(i);
            }
        }
        tValue = toBuild;
        if (value != '') {
            var nValue = 0;
            try {
                nValue = MvcControlsToolkit_Parse(tValue, dataType);
                if (negative >= 0) nValue = nValue * -1;
                if (!isNaN(nValue)) {
                    value = MvcControlsToolkit_Format(nValue, format, dataType);
                    if (dataType == MvcControlsToolkit_DataType_Float) {
                        if (negative >= 0) innerValue = minus + tValue;
                        else innerValue = tValue;
                    }
                    else
                        innerValue = MvcControlsToolkit_Format(nValue, 'n0', dataType);
                }
            }
            catch (e) {
            }

        }
    }
    if (dataType == MvcControlsToolkit_DataType_DateTime) {
        innerValue = MvcControlsToolkit_Format(MvcControlsToolkit_Parse(innerValue, dataType), format, dataType);
        value = innerValue;
    }
    document.getElementById(companionId).value = innerValue;

    if (jumpValidation == null) {
        MvcControlsToolkit_Validate(companionId, validationType);
        $('#' + companionId).trigger('vblur');
    }

    $('#' + fieldId).removeClass('input-validation-error');
    if ($('#' + companionId).hasClass('input-validation-error')) {
        $('#' + fieldId).addClass('input-validation-error');
    }
    
    if (value == '') {
        if (watermarkCss != '') $('#' + fieldId).addClass(watermarkCss);
        document.getElementById(fieldId).value = watermark;
    }
    else {
        document.getElementById(fieldId).value = pre + value + post;
    }
    
    $('#' + fieldId).trigger('TypedTextBox_Changed');

}
function MvcControlsToolkit_DisplayEdit_Init(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14) {
    $(document).ready(function () {
        var tTb = $('#' + p0);
        if (p13 != null) tTb.datepicker(p13);
        tTb.bind('pblur', function (e, data) {
            var res = MvcControlsToolkit_DisplayEdit_Blur(p0, p1, p2, p7, p8, p9, p5, p6, p3, p4,
                           p10, p12, p14, !p11 && !data);
            if (data) data["goneDisplay"] = res;
            return false;
        });
        try {
            tTb.trigger('pblur');
        }
        catch (ex) { }
        tTb.blur(function () { tTb.trigger('pblur'); return true; });
        if (p11 == 'click') {
            $('#' + p1).click(function () {
                MvcControlsToolkit_DisplayEdit_DbClick(p0, p1);
                return true;
            });
        }
        else if (p11 == 'dblclick') {
            
            $('#' + p1).dblclick(function () {
                MvcControlsToolkit_DisplayEdit_DbClick(p0, p1);
                return true;
            });
        }
        $('#' + p1).bind("pedit", function () {
            MvcControlsToolkit_DisplayEdit_DbClick(p0, p1);
            return true;
        });
        tTb.keypress(function (event) {
            return MvcControlsToolkit_TypedTextBox_Input(event.which, p0, p1, p2, p3, p4, p5, p6);
        });

    });
}
function MvcControlsToolkit_DisplayEdit_Blur(
fieldId, companionId, dataType,
pre, post, format, plus, minus, 
decimalSeparator, digitSeparator, validationType, nullDisplayText, inputType, noBack) {
    var fieldElement = document.getElementById(fieldId);
    if (fieldElement == null) return;
    var value = fieldElement.value;
    if (inputType == "select") {
        value = $(fieldElement).find('option:selected').text();
    }
    var innerValue = value;
    if (dataType != MvcControlsToolkit_DataType_String && dataType != MvcControlsToolkit_DataType_DateTime) {
        value = MvcControlsToolkit_Trim(value);
        innerValue = value;
        var tValue = value;
        tValue = tValue.replace(digitSeparator, '');
        tvalue = tValue.replace(plus, '');
        negative = tValue.indexOf(minus);
        tValue = tValue.replace(minus, '');
        var toBuild = '';
        var charCode = '';
        for (var i = 0; i < tValue.length; i++) {
            charCode = tValue.charCodeAt(i);
            if ((charCode >= 48 && charCode <= 57) || charCode == decimalSeparator.charCodeAt(0)) {
                toBuild = toBuild + tValue.charAt(i);
            }
        }
        tValue = toBuild;
        if (value != '') {
            var nValue = 0;
            try {
                nValue = MvcControlsToolkit_Parse(tValue, dataType);
                if (negative >= 0) nValue = nValue * -1;
                if (!isNaN(nValue)) {
                    value = MvcControlsToolkit_Format(nValue, format, dataType);
                    if (dataType == MvcControlsToolkit_DataType_Float) {
                        if (negative >= 0) innerValue = minus + tValue;
                        else innerValue = tValue;
                    }
                    else
                        innerValue = MvcControlsToolkit_Format(nValue, 'n0', dataType);
                }
            }
            catch (e) {
            }

        }
    }
    if (dataType == MvcControlsToolkit_DataType_DateTime) {
        innerValue = MvcControlsToolkit_Format(MvcControlsToolkit_Parse(innerValue, dataType), format, dataType);
        value = innerValue;
    }
    if (inputType == "select") {
    }
    else {
        fieldElement.value = innerValue;
    }

    if (MvcControlsToolkit_Validate(fieldId, validationType) && !noBack) {
        $('#' + fieldId).hide();
        $('#' + companionId).show();

        if (value == '') {
            if (nullDisplayText == '')
                $('#' + companionId).html('&nbsp;&nbsp;&nbsp;');
            else
                $('#' + companionId).text(nullDisplayText);
        }
        else {
            $('#' + companionId).text(pre + value + post);
        }
        $('#' + fieldId).trigger('TypedEditDisplay_Changed');
        return true;
    }
    else {
        $('#' + fieldId).trigger('TypedEditDisplay_Changed');
        return false;
    }
    
}

function MvcControlsToolkit_EditDisplayButton(idButton, containersSelector, goToDisplayText, goToEditText, changeStateCallback){
    var jMe = $(idButton);
    var state = jMe.data("_editDisplayState_") || { display: false, jElements: jMe.parents(containersSelector).first().find('[data-element-type=TypedEditDisplay]') };
    var me = jMe[0];
    function goToDisplay(elements){
        var ok =true;
        elements.each(function(){
            var res = {goneDisplay: false};
            $(this).trigger('pblur', res);
            if (!res.goneDisplay) ok=false;
        });
        return ok;
    }
    function setButton(bt, txt) {
        
            var tag = bt.tagName.toLowerCase();
            if (tag == 'a') {
                $(bt).text(txt);
            }
            else if (tag == 'img') {
                $(bt).attr('src', txt);
            }
            else {
                $(bt).val(txt);
            }
        
    }
    function goToEdit(elements) {
        elements.each(function () {
            $('#' + $(this).attr('id') + '_display').trigger('pedit');
        });
    }
    function applyCallback(elements, state, callback) {
        if (!callback) return;
        elements.each(function () {
            callback(this, state);
        });
    }
    if (state.display) {
        goToEdit(state.jElements);
        setButton(me, goToDisplayText);
        state.display = false;
        applyCallback(state.jElements, 'goneEdit', changeStateCallback);
    }
    else {
        res = goToDisplay(state.jElements);
        if (res) {
            setButton(me, goToEditText);
            state.display = true;
            applyCallback(state.jElements, 'goneDisplay', changeStateCallback);
        }
        else {
            goToEdit(state.jElements);
            applyCallback(state.jElements, 'failedGoneEdit', changeStateCallback);
        }
    }
    $(me).data("_editDisplayState_", state);

}

function MvcControlsToolkit_TypedTextBox_BindChange(id, handler) {
    $("#" + id + "_hidden").bind("TypedTextBox_Changed", handler);
}
function MvcControlsToolkit_TypedTextBox_UnbindChange(id, handler) {
    $("#" + id + "_hidden").unbind("TypedTextBox_Changed", handler);
}
function MvcControlsToolkit_TypedEditDisplay_BindChange(id, handler) {
    $("#" + id ).bind("TypedEditDisplay_Changed", handler);
}
function MvcControlsToolkit_TypedEditDisplay_UnbindChange(id, handler) {
    $("#" + id ).unbind("TypedEditDisplay_Changed", handler);
}
function MvcControlsToolkit_TypedTextBox_SetString(field, value)
{
    var field = $(field);
    field.trigger('pfocus');
    field.val(value);
    field.trigger('pblur');
}
function MvcControlsToolkit_TypedInput_Load(value, field) {
    var field = $(field);
    field.trigger('pfocus');
    field.val(value);
    field.trigger('pblur');
}
function MvcControlsToolkit_TypedTextBox_GetString(source) {
    var companionId = source.id.substring(0, source.id.lastIndexOf("_"));
    return $('#' + companionId).val();
}
function MvcControlsToolkit_TypedTextBox_Set(source, value, format, valueType) {
    var field = $(source);
    if (!valueType) valueType = parseInt(field.attr("data-client-type"));
    var companionId = source.id.substring(0, source.id.lastIndexOf("_"));
    var companion = $('#' + companionId);
    if (companion.length == 0) {
        var retry = function () { MvcControlsToolkit_TypedTextBox_Set(source, value, format, valueType); };
        setTimeout(retry, 0);
        return;
    }
    var value = MvcControlsToolkit_Format(value, format, valueType, '', '');
    companion.val(value);
    field.val(value);
    field.trigger('pfocus');
    field.trigger('pblur');

}
function MvcControlsToolkit_TypedTextBox_SetById(id, value, format, valueType) {
    var field = $('#' + id + '_hidden');
    if (!valueType) valueType = parseInt(field.attr("data-client-type"));
    var companion = $('#' + id);
    var value = MvcControlsToolkit_Format(value, format, valueType, '', '');
    companion.val(value);
    field.val(value);
    field.trigger('pfocus');
    field.trigger('pblur');
}
function MvcControlsToolkit_TypedTextBox_Get(source, valueType) {
    var companionId = source.id.substring(0, source.id.lastIndexOf("_"));
    return MvcControlsToolkit_Parse($('#' + companionId).val(), valueType);
}
function MvcControlsToolkit_TypedTextBox_GetById(id, valueType) {
    return MvcControlsToolkit_Parse($('#' + id).val(), valueType);
}
function MvcControlsToolkit_TypedEditDisplay_SetString(field, value) {
    var field = $(field);
    field.val(value);
    field.trigger('pblur');
}
function MvcControlsToolkit_TypedEditDisplay_GetString(source) {
    return $(source).val();
}
function MvcControlsToolkit_TypedEditDisplay_Set(source, value, format, valueType) {
    var field = $(source);
    if (!valueType) valueType = parseInt(field.attr("data-client-type"));
    $(source).val(MvcControlsToolkit_Format(value, format, valueType, '', ''));
    field.trigger('pblur');

}
function MvcControlsToolkit_TypedEditDisplay_SetById(id, value, format, valueType) {
    var field = $('#' + id);
    if (!valueType) valueType = parseInt(field.attr("data-client-type"));
    field.val(MvcControlsToolkit_Format(value, format, valueType, '', ''));
    field.trigger('pblur');

}
function MvcControlsToolkit_TypedEditDisplay_Get(source, valueType) {
    return MvcControlsToolkit_Parse($(source).val(), valueType);
}
function MvcControlsToolkit_TypedEditDisplay_GetById(id, valueType) {
    return MvcControlsToolkit_Parse($('#'+id).val(), valueType);
}
//////////////////////////Display Field ////////////
function MvcControlsToolkit_DisplayField_SetString(field, value) {
    return;   
}
function MvcControlsToolkit_DisplayField_GetString(field) {
    return eval(field.id + "_True");
}

//////////////////////////Timer/////////////////////
function MvcControlsToolkit_AjaxLink(url, targetId, mode, afterSuccess) {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (data) {
            if (mode == 0) $('#' + targetId).html(data);
            else if (mode == -1) $('#' + targetId).before(data);
            else if (mode == 1) $('#' + targetId).after(data);
            if (afterSuccess) afterSuccess($('#' + targetId));
        }
    });
}
function MvcControlsToolkit_AjaxSubmit(validationType, elementId) {
    if (!MvcControlsToolkit_FormIsValid(elementId, validationType)) return;

    $('#' + elementId).parents('form').submit();
}
/////////////////////////DropDowns/////////////////////////////
function MvcControlsToolkit_UpdateDropDownOptions(url, jTarget, prompt, optionsCss, optGroupsCss, callBack) {
    $.getJSON(url, function (data) {
        var items = [];
        var createOption = function (val, text) {
            var currCss = null;
            if (typeof (optionsCss) == "function") currCss = optionsCss(val);
            else currCss = optionsCss;
            items.push("<option value ='" + val + "' " + (currCss == null ? "" : "class = '" + currCss + "' ") + ">" + text + "</option>");
        };
        var createOptgroup = function (label, options, val) {
            var currOptCss = null;
            if (typeof (optGroupsCss) == "function") currOptCss = optGroupsCss(val);
            else currOptCss = optGroupsCss;
            items.push("<optgroup label ='" + label + "' " + (currOptCss == null ? "" : "class = '" + currOptCss + "' ") + ">");
            for (var i = 0; i < options.length; i++) {
                createOption(options[i].Value, options[i].Text);
            }
            items.push("</optgroup>");
        };
        if (prompt != null) createOption('', prompt);
        $.each(data, function (index, item) {
            if ("Group" in item) {
                if ("Value" in item) createOptgroup(item.Text, item.Group, item.Value);
                else createOptgroup(item.Text, item.Group, item.Text);
            }
            else createOption(item.Value, item.Text);
        });
        jTarget.html(items.join(''));
        if (callBack != null && typeof (callBack) == "function") callBack(jTarget);
    });
}


/////////////////////////Hover menu/////////////////////////////
$(document).ready(function () {
    $('.MvcControlsToolkit-Hover').live('mouseover', function (e) {
        $(this).addClass('MvcControlsToolkit-Hover-On');
    });
    $('.MvcControlsToolkit-Hover').live('mouseleave', function (e) {
        // Do not close if going over to a select element
        if (e.relatedTarget.tagName.toLowerCase() == 'select') return;
        $(this).removeClass('MvcControlsToolkit-Hover-On');
        $(this).find('select').trigger('blur');
    });

});
//////////////Unobtrusive Parsing///////////////////////////
var MvcControlsToolkit_ParseRegister = (function () {
    var allParsers = [];
    var needInit = false;
    return {
        add: function (parser, initialize) {
            allParsers.push({ f: parser, i: initialize });
            if (initialize) needInit = true;
        },
        parse: function (selector) {
            for (i = 0; i < allParsers.length; i++) {
                allParsers[i].f(selector);
            }
        },
        init: function () {
            if (needInit) {
                for (i = 0; i < allParsers.length; i++) {
                    if (allParsers[i].i) allParsers[i].f();
                }
            }
        }
    };

})();
$(document).ready(function () {
    MvcControlsToolkit_ParseRegister.init();
});
////////////////////////////////////////////////////////////
