
//Uses an iframe element to asynchronously post a form, providing hooks
//to preprocess form data and handle the response
PxPage.AjaxForm = function ($) {

    //generates an id for the frame
    var genFrameId = function () {
        return 'AjaxFrame_' + Math.floor(Math.random() * 99999);
    };

    //generates a frame with the given callback function
    var genFrame = function (args) {

        var n = genFrameId();
        var d = document.createElement('DIV');


        if ($.browser.safari) {
            d.innerHTML = '<iframe style="display:none" src="about:blank" id="' + n + '" name="' + n + '"></iframe>';
        }
        else {
            d.innerHTML = '<iframe style="display:none" src="about:blank" id="' + n + '" name="' + n + '" onload="PxPage.AjaxForm.loaded(\'' + n + '\')"></iframe>';
        }

        if (args.frameContainer && args.frameContainer != '') {
            //attempted fix for safari.            
            $(d).appendTo(args.frameContainer);
            //alert('appended frame to: ' + args.frameContainer);
        }
        else {
            document.body.appendChild(d);
        }

        var i = document.getElementById(n);
        if ($.browser.safari) {
            $(i).load(function () {
                PxPage.AjaxForm.loaded(n);
            });
        }


        if (args && typeof (args.onComplete) == 'function') {
            PxPage.log('AjaxForm: onComplete registered');
            i.onComplete = args.onComplete;
        }

        return n;
    };

    //configures the form to post to the frame
    var form = function (f, name) {
        if ($(f).length) {
            PxPage.log('AjaxForm: set target attribute');
            $(f).attr('target', name);
        }
        else {
            PxPage.log('AjaxForm: could not find form');
        }
    };

    return {
        //called when the submitted frame loads with the response
        loaded: function (id) {
            PxPage.log("AjaxForm: loaded id = " + id);

            var i = document.getElementById(id);
            if (i.contentDocument) {
                var d = i.contentDocument;
            } else if (i.contentWindow) {
                var d = i.contentWindow.document;
            } else {
                var d = window.frames[id].document;
            }

            try {
                if (d.location.href == "about:blank") {
                    PxPage.log("AjaxFrame: frame location is about:blank");
                    //return;
                }
            }
            catch (err) { }

            if ($.browser.safari && d == null) {
                //attempted fix for safari.
                d = document.defaultView.document;
            }

            if (typeof (i.onComplete) == 'function') {
                PxPage.log("AjaxFrame: onComplete");
                if (d != null) {
                    i.onComplete(d.body.innerHTML);
                }
            }

            return;
        },
        //submits the given form in an iframe.
        //args can have an 'onStart' and 'onComplete' handler.
        //onStart is executed before the call is marshalled.
        //onComplete is executed after the response comes back to the client.
        //args also takes a parameter called 'formContainer' that is a jQuery selector
        //to the element where the iframes should be appended. This value defaults to 
        //document.body.
        submit: function (f, args) {
            PxPage.log("AjaxForm: submit");
            form(f, genFrame(args));
            if (args && typeof (args.onStart) == 'function') {
                return args.onStart();
            } else {
                return true;
            }
        }
    };

} (jQuery);