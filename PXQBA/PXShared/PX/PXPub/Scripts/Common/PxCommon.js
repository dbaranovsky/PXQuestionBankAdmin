//jQuery.noConflict();
//Provides a simple Page helper for the application
var PxPage = function ($) {
    var opts = {};
    var sel =
    {
        AssignDialog: "#assigndialog"

    };
    var edit = {
        Editor: "#editor"
    };
    var fade = {
        fadeOptions: {
            highlightNewItems: true,
            highlightUpdatedItems: true,
            highlightColor: "yellow",
            highlightTime: "3000"
        }
    };
    var defaults = {
        internalForm: ''
    };

    //reset blockUI defaults since they are not applicable to FNE
    $.blockUI.defaults.css = {};

    var loadingTimer = [];
    var _fneLoaded = function () {
        PxPage.Loaded();
        _fneBlockUI();
        PxPage.Update();

        var fneClass;

        for (fneClass in PxPage.FneInitHooks) {
            if ($('#fne-content .' + fneClass).length > 0) {
                if (PxPage.FneInitHooks[fneClass] != null) {
                    PxPage.FneInitHooks[fneClass]();
                }
            }
        }
        for (fneClass in PxPage.FneResizeHooks) {
            if ($('#fne-content .' + fneClass).length > 0) {
                PxPage.ResizeHooksInUse[fneClass] = PxPage.FneResizeHooks[fneClass];
                PxPage.FneResizeHooks[fneClass]();
            }
        }
        PxPage.SetFneNavigations();
        PxPage.UpdateFneSize();
        PxPage.SetFrameApiHooks();

        PxPage.log("_fneLoaded complete");

        for (fneClass in PxPage.FneLoadedHooks) {
            if ($('#fne-content .' + fneClass).length > 0) {
                try {
                    PxPage.FneLoadedHooks[fneClass]();
                }
                catch (ex) {
                    PxPage.log("Couldn't load fneLoadedHooks ");
                }
            }
        }
        $(PxPage.switchboard).trigger("fneloaded");
    };
    var _fneBlockUI = function () {
        var fneVisible = $('#fne-window').is(':visible');
        if (!fneVisible) {
            var blockUIOpts;

            if (PxPage.fneOpts) {
                blockUIOpts = PxPage.fneOpts;
                blockUIOpts.message = $('#fne-window');
                blockUIOpts.fadeIn = 500;
            } else {
                blockUIOpts = {
                    message: $('#fne-window')
                };
            }
            $("body .single-column").block(blockUIOpts);
        }
        //$.blockUI(opts);
    };

    var _refreshView = function (args) {
        if (args.url != null) {
            args.url = args.url.replace('Shortcut__1__', '');
        }
        var documentViewer = $('#right #contentwrapper #content #document-viewer');
        documentViewer.empty();
        documentViewer.append('<iframe src="' + args.url + '" />');
        $('#right #contentwrapper #content #document-viewer iframe').load(function () {
            var contentheight = $('#right #contentwrapper #content #document-viewer iframe').contents().height();
            $('#right #contentwrapper #content #document-viewer iframe').css({ 'width': '100%', 'height': contentheight });
        });
    };
    var _loadFneIframe = function (args) {

        PxPage.log("_openFNE: useIFrameForExternal");
        if ($('#fne-window #fne-content #document-viewer').length > 0) {
            var documentViewer = $('#fne-window #fne-content #document-viewer');
            documentViewer.empty();
            documentViewer.append('<iframe src="' + args.url + '" />');
            $('#fne-window #fne-content #document-viewer iframe').load(function () {
                var contentheight = $('#fne-window #fne-content #document-viewer iframe').contents().height();
                $('#fne-window #fne-content #document-viewer iframe').css({ 'width': '100%', 'height': contentheight });
            });
        }
        else {
            $('#fne-window #fne-content').empty();
            $('#fne-window #fne-content').append('<div id="contentwrapper" class="contentwrapper"><iframe src="' + args.url + '" /></div>');
            $('#fne-window #fne-content iframe').load(function () {
                var contentheight = $('#fne-window #fne-content iframe').contents().height();
                $('#fne-window #fne-content iframe').css({ 'width': '100%', 'height': contentheight });
            });
            _fneLoaded();
            if (args.onFneLoaded) args.onFneLoaded();
        }
    }
    ///arguments: 
    ///title: title of the FNE window
    ///fixed: set the window to a fixed size
    ///requireConfirm: require confirmation on close
    ///autoSubmit: automatically submit?
    ///hasChanges: dirty buffer tracker
    ///minimize: allow minimize
    ///useLocal: FNE takes already existing content in DOM
    ///useIframe: use iframe to load args.url
    ///url: url to load in iframe
    ///onFneLoaded: callback to run after FNE is loaded
    ///useIFrameForExternal: use ducumentViewer to load FNE
    ///loadFullFne: loads the FNE view from the server
    var _openFNEAction = function (args) {
        var title = args.title;
        PxPage.SetFneTitle(title);

        // A quick work-around to load some contents' data.
        // Currently the shortcut links are not properly referencing the actual content.

        if (args.url != null) {
            args.url = args.url.replace('Shortcut__1__', '');
        }

        if (args.fixed) {
            $('#fne-window').addClass('fixed');
        }

        if (args.requireConfirm) {
            //$('#fne-window').addClass('require-confirm');
        }

        if (args.requireConfirmCustom)
            $('#fne-window').addClass('require-confirm-custom');
        
        if (args.autoSubmit) {
            $('#fne-window').addClass('auto-submit');
        }

        if (args.hasChanges) {
            $('#fne-window').addClass('has-changes');
        }

        if (args.minimize) {
            $('#fne-window').addClass('fne-minimize');
            $('#fne-window #fne-minimize-action').show();
        }

        $(PxPage.switchboard).trigger("fneprep");

        _fneBlockUI();
        //PxPage.Loading();
        if (args.useLocal) {
            PxPage.log("_openFNE: useLocal");
            //assume content is already in DOM
            $('#fne-window #fne-content').empty();
            $('#fne-window #fne-content').append($('#content-item'));
            $('#fne-window #fne-content #content-item .content').slice(1).remove();
            $('#fne-unblock-action').addClass('fne-local');
            //hide the extra navigation arrows in eportfolio
            _fneLoaded();

            if (args.onFneLoaded) args.onFneLoaded();
        }
        else if (args.loadFullFne) {
            PxPage.log("_openFNE: URL, full FNE");
            //load via URL
            PxPage.Loading("#fne-window");
            $.get(args.url, function (response) {
                var fneVisible = $('#fne-window').is(':visible');
                if (!fneVisible) {
                    //if the FNE window is no longer visible after we've retrieved the info from the server, the user has cancelled the request
                    //we don't need to do anything
                    return;
                }
                $("#fne-window").replaceWith($(response));
                if (fneVisible) {
                    $("#fne-window").show();
                }

                PxPage.Loaded("#fne-window");

                _fneLoaded();
                if (args.onFneLoaded) {
                    args.onFneLoaded();
                }
            });
        }
        else if (args.useIFrame) {
            PxPage.log("_openFNE: useIFrame");
            $('#fne-window #fne-content').empty();
            $('#fne-window #fne-content').append('<iframe src="' + args.url + '" />');
            PxPage.Loaded("#fne-window");
            _fneLoaded();
            if (args.onFneLoaded) args.onFneLoaded();
        }
        else if (args.useIFrameForExternal) {
            PxPage.Loaded("#fne-window");
            _loadFneIframe(args);
        }

        else {
            PxPage.log("_openFNE: URL");
            //load via URL
            $('#fne-window #fne-content').empty();
            $('#fne-window #fne-content').append("<div>Loading...</div>");
            $('#fne-unblock-action').removeClass('fne-local');

            $.ajax({
                // guid parameter added to prevent caching by IE8
                // corresponding server method should have it in case caching issue observed
                url: args.url + (args.url.indexOf('?') === -1 ? '?' : "&") + "guid=" + Math.random(),
                cache: false,
                dataType: "html",
                success: function (data) {
                    $('#fne-window #fne-content').html(data);

                    PxPage.Loaded("#fne-window");
                    _fneLoaded();

                    if (args.onFneLoaded) {
                        args.onFneLoaded();
                    }
                }
            });
        }

        // fixes top for the student view (caused by the student view mode)
        $('#fne-content').css('top', 0);
    };

    var _openFNE = function (args) {
        //log FNE action in history
        try {
            if (window.History != null && window.History.enabled) {
                var windowUrl = window.location.toString().split('?')[0];
                var userUrl = args.url.replace(windowUrl, "");
                var pageName = window.location.pathname.substring(window.location.pathname.lastIndexOf('/') + 1);
                var foundItemString = userUrl.indexOf("?item=");
                if (foundItemString == -1) {
                    if (window.History.pushState)
                        var success = window.History.pushState({ plugin: "PxPage", func: "OpenFNE", args: [args] }, args.title, pageName + "?item=" + userUrl);
                    else
                        _openFNEAction(args);
                }
            } else {
                _openFNEAction(args);
            }
        } catch (ex) {
            return false;
        }
        return false;
    };
    //fills the rest of the screen with rightCol
    var _equalizeWidth = function (leftCol, rightCol, customFix) {
        PxPage.log("equalizeWidth(" + leftCol + "," + rightCol + ")");
        //Sets the Height on the Content Browsers Main div for IE HACKS :( 
        if ($.browser.msie && $.browser.version < 8) {
            var mainHeight = $('#main').height();
            $('#left').height(mainHeight);
            $('#right #contentwrapper').height(mainHeight - 32);
            $('#assignmentViewContent').height(mainHeight + 150);
        }
        var w_width = $('#main').width();
        var l_owidth = $(leftCol).outerWidth(true);
        var r_width = w_width - l_owidth - 3;
        $(rightCol).width(r_width);
    };

    var _serializeMap = function (map) {
        var data = '';
        for (var key in map) {
            if (map.hasOwnProperty(key)) {
                var val = map[key];
                if (data != '')
                    data += '&';

                data += key + "=" + val;
            }
        }
        return data;
    };

    //Generic function to handle global ajax error cleanup
    var handleAjaxErrors = function (event, jqxhr, settings, exception, errorFrame) {
        
        if (jqxhr.statusText === "timeout") {
            jqxhr.status = 408;
        }
        
        //error stack trace container in the popup contents
        //we grab the stack trace so we can send that along with the logging details to the server side
        var errorStacktrace = errorFrame.contents().find(".errorMessage2").html();
        
        //There can be times when the errormessage container is not available, hence the reason for this check.
        if (errorStacktrace === null) {
            errorStacktrace = "";
        }

        PxPage.log("this is the error status upon entry into handleAjaxErrors - " + jqxhr.status);

        var errorData = {            
            errorName: "Error Status: " + jqxhr.status,
            errorMessage: ""
        };

        //jqxhr object contains the status code reference object to capture errors based upon code being thrown
        //server side currently does not specifiy a 503 error although it can be specified by the local error handling
        jqxhr.statusCode({
            500: function() {
                PxPage.log("The system has entered into the 500 error block");
                errorData.errorMessage = "\nURL: " + settings.url + "\nException: " + exception + "\nThis is the error - Stacktrace:  " + errorStacktrace.toString();
            },

            503: function() {
                PxPage.log("The system has entered into the 503 error block");
                errorData.errorMessage = "\nURL: " + settings.url + "\nException: " + exception + "\nThis is the error - Stacktrace:  " + errorStacktrace.toString();
            },

            //method for handling client side timeout issues
            408: function() {
                PxPage.log("The system has entered into the 408 error block");
                errorData.errorMessage = "\nURL: " + settings.url + "\nException: " + exception;
            }
            
        });
        
        handleAjaxLogging(errorData, errorFrame);
    };
    
    var handleAjaxLogging = function (errorData, errorFrame) {
        
        //placeholder for ajax logging API method until the functionality is put into place
        $.ajax({
            type: 'POST',
            data: errorData,
            url: PxPage.Routes.log_javascript_errors,
            success: function (response) {
                PxPage.log("the response from the logging request: " + response);
            },
            
            error: function (req, status, error) {
                PxPage.log("An Ajax error has occurred in the logging");
                if (errorFrame.contents().find("title").text() == "Maintenance Message") {
                    location.reload();
                }
            }
            
        });
    };

    //This function renders the html for the 408 timeout error
    var render408Html = function () {
        var stylesAdded = "body{font-family: arial, sans-serif;background-color:#f2f2f2;}" +
            "#errorpage-container{border: 4px solid #D4484B;height: 300px;padding: 50px 25px 0;width: 550px;margin: 10% auto;background-color: #fff;}" +
            "/* error image is embedded so we dont trigger an error cascade */#errorpage-container .icon{display:block;height:90px;width:96px;float:left;}" +
            "#errorpage-container .message {float: left;padding: 25px 0 0 25px;width: 420px;color:#333333;}" +
            "#errorpage-container .sorry{font-size:30px;font-weight:bold;padding-bottom:20px;border-bottom:3px solid #333;}" +
            "#errorpage-container .sorry span{font-weight:normal;color:#D4484B;}" +
            "#errorpage-container .page-exist{font-weight:bold;font-size:20px;padding-top:5px;}" +
            "#errorpage-container .url-check{border-top:1px dotted  #b3b3b3;border-bottom:1px dotted  #b3b3b3;padding: 8px 70px 8px 0;font-family:georgia;font-size:15px;}" +
            "#errorpage-container .url-check a{color: #D4484B;text-decoration:none;}" +
            "#errorpage-container .showdetailslink{font-size:10px;text-decoration:underline;cursor:pointer;}" +
            "#detail-msg-container .errorMessage1{font-size:16px;font-weight:bold;padding-bottom:5px;}" +
            "#detail-msg-container .errorMessage2{font-size:14px;}";

        var pageContent = "<div id='errorpage-container'><div class='icon'></div><div class='message'><div class='sorry'>We’re sorry&mdash;The request you have made has timed out.</div><p class='url-check'>Please <a href='http://support.bfwpub.com/supportform/form.php?View=contact'>click here</a> to report this issue to the Technical Support team.</p></div></div>";
        
        var htmlcode = "<html><head><title>Page Timed Out 408</title></head><body><style>" + stylesAdded + "</style>" + pageContent + "</body></html>";
        return htmlcode;
    };

    return {
        Context: {},
        Routes: {},
        CssRoutes: {},
        AssignmentDates: {},
        AssignmentDatesCallback: {},
        launchpad_editor_options: {},
        html_editor_options: {},
        editableNote_editor_options: {},
        rubricText_editor_options: {},
        //called to initialize a page. should only be called once
        Init: function (options) {
            opts = options;

            if (window.History && window.History.enabled) {
                //set unblock as initial state
                var pageName = window.location.pathname.substring(window.location.pathname.lastIndexOf('/') + 1);
                window.History.pushState({ plugin: "PxPage", func: "UnBlockAction", args: [null] }, "Home", pageName);
            }
            
            if(PxPage.TouchEnabled()){
                $("html").addClass("touch");
            } else {
                $("html").addClass("no-touch");
            }

            $.ajaxSetup({
                cache: false
            });
            
            $(document).ajaxSend(function(event, jqxhr, ajaxOptions) {
                ajaxOptions.timeout = 600000;
                
            });
            
            $(document).ajaxStop(function () {
                PxPage.Update();
            });

            $(document).ajaxError(function (event, jqxhr, settings, exception) {
                if (settings.url == PxPage.Routes.log_javascript_errors) {
                    return;
                }

                if (exception.length == 0) {
                    //error is due to browser redirect, ignore
                    PxPage.Loaded();
                    return;
                }
                
                PxPage.log("Error requesting pge " + settings.url + "\n Error: " + exception);
                PxPage.Loaded();

                var errorContainer = $('<div id="errorContainer"></div>').appendTo('#main');
                var errorFrame = $('<iframe id="errorFrame"/>').appendTo(errorContainer);

                event.preventDefault();
                //errorContainer.append(errorFrame);
                if ($(errorContainer).dialog)
                    $(errorContainer).dialog({
                        title: "A problem has occurred",
                        resizable: false,
                        height: 630,
                        width: 700,
                        modal: true,
                        buttons: {
                            "Ok": function () {
                                $(this).dialog("close");
                                $("#errorFrame").remove();
                                $("#errorContainer").remove();
                                //statement to unblock the UI in the event of an error
                                $("#loadingBlock").parents().unblock();
                            }
                        },
                        dialogClass: "error"
                    });
                // put in a setTimeout method here to give Firefox and IE time to receive the populated jqxhr object
                // also set the url target for the macmillan support form link to "_blank" since when the form opens up in the dialog,
                //it is very difficult to see. - this Fixes jira item PX-6522
                setTimeout(function () {
                    if (jqxhr.statusText === "timeout") {
                        jqxhr.responseText = render408Html();
                    }
                    errorFrame.contents().find("body").html(jqxhr.responseText);
                    errorFrame.contents().find("head").append("<base target='_blank'>");
                    //Added handleAjaxErrors method here to capture and log client side errors
                    handleAjaxErrors(event, jqxhr, settings, exception, errorFrame);
                }, 200);

            });
            var ctx = { TocDisabled: "false" };
            $.extend(ctx, opts.context);

            PxPage.Routes = opts.routes;
            PxPage.CssRoutes = opts.css_routes;
            PxPage.Context = ctx;
            PxPage.QuestionFilter = opts.questionFilter;
            PxPage.launchpad_editor_options = opts.launchpad_editor_options;
            PxPage.html_editor_options = opts.html_editor_options;
            PxPage.editableNote_editor_options = opts.editableNote_editor_options;
            PxPage.rubricText_editor_options = opts.rubricText_editor_options;

            PxPage.ExternalUrlPrefix = opts.externalurlprefix;


            //turn off autocomplete for input fields in firefox
            $('input,textarea').attr('autocomplete', 'off');


            if (typeof tinyMCE != "undefined") {
                if (PxPage.Context.course_tinyMCE_options == "") {
                    tinyMCE.init(opts.html_editor_options);
                }
                else {
                    tinyMCE.init(opts[PxPage.Context.course_tinyMCE_options]);
                }
            }


            this.Update();
            this.SetFneLinks();
            this.SetNonModalLinks();
            this.SetActivationLinks();
            this.SetWindowResizeHandler();
            this.SetSyllabusUpload();

            $(document).off('click','#fne-unblock-action').on("click", '#fne-unblock-action', PxPage.UnBlock);

            $(document).off('click', '.ui-widget-overlay').on('click', '.ui-widget-overlay', function (event) {
                event.stopImmediatePropagation();
                $(sel.AssignDialog).dialog("close");
            });

            $(document).off('click', '#PXAssignWithoutDueDate').on('click', '#PXAssignWithoutDueDate', function (event) {
                event.stopImmediatePropagation();
                PxPage.AssignmentDatesCallback.CustomValues.StartDate = "01/01/0001"; // Need to decide, which date we need to use in case of "Assign without due date"
                PxPage.AssignmentDatesCallback.Callback(PxPage.AssignmentDates, PxPage.AssignmentDatesCallback.CustomValues);
                $(sel.AssignDialog).remove();
                PxPage.AssignmentDatesCallback = { Callback: null, CustomValues: null };
                PxPage.AssignmentDates = { StartDate: null, DueDate: null, Counter: 0 };
            });

            $('#fne-minimize-action').click(function () {

                $('#fne-window #fne-minimize-action').hide();

                var title = $('#fne-window #fne-title #title-text').text();
                $('#fne-minimized #fne-minimized-title').empty().prepend(title);

                $('#fne-minimized #fne-minimized-content').empty().append($('#fne-window #fne-content').html());

                //$('#fne-minimized').show();

                $('#footer').block({
                    message: $('#fne-minimized'),
                    css: {
                        padding: 0,
                        margin: 0,
                        top: '0%',
                        left: '0%'
                    }
                });


                // do something
                //$('#main #right').append($('#content-item'));

                var fneClass;
                for (fneClass in PxPage.FneMinimizeHooks) {
                    if ($('.' + fneClass).length > 0) {
                        PxPage.log("firing FNE minimize for " + fneClass);
                        PxPage.FneMinimizeHooks[fneClass]();
                    }
                    else {
                        PxPage.log("FAILED FNE minimize for " + fneClass);
                    }
                }

                $.unblockUI();
                PxPage.ResizeHooksInUse = {};

                
                return false;
            });

            $('#fne-minimized').click(function () {

                $('#fne-minimized-close').click();
                $('#fne-window #fne-content').empty().html($('#fne-minimized #fne-minimized-content').html());
                var title = $('#fne-minimized #fne-minimized-title').text();
                PxPage.SetFneTitle(title);

                _fneLoaded();

                $('#fne-minimized #fne-minimized-content').empty();
                $('#fne-window #fne-minimize-action').show();

                return false;
            });

            $('#fne-minimized-close').click(function () {
                $('#footer').unblock();
                $('#fne-minimized').hide();
                return false;
            });


            $(document).off('click','.toc .level a.expand').on('click', '.toc .level a.expand', function (event) {
                $('.toc .level a.expand.active').removeClass('active');
                $(this).toggleClass('active folderopen');
            });

            if (window.PxAccountWidget) {
                PxAccountWidget.Init();
            }

            //top.bookId = "just about anything";

            $(document).off('keypress', '.alpha-numeric').on('keypress', '.alpha-numeric', function (evt) {
                if (evt.which == 0) return true;
                var key = String.fromCharCode(evt.which);
                var regex = /^[a-zA-Z0-9]+$/i;
                if (!regex.test(key)) {
                    evt.returnValue = false;
                    if (evt.preventDefault) evt.preventDefault();
                }
            });

            $(document).off('keypress','.title').on('keypress', '.title', function (e) {
                if (e.which === 13)
                    return false;
            });

            if (window.PxUpload) {
                PxUpload.Init();
            }
            
            PxPage.Loader.Init();

            PxPage.IsReady = true;
            $(PxPage.switchboard).trigger("PxPageReady");
            $(PxPage.switchboard).trigger("magic");

            if (PxPage.FrameAPI) PxPage.FrameAPI.fireEvent("nodescription");
        },

        UnBlock: function (event) {
            var isFneLocal = $(this).hasClass('fne-local');
            if (window.History != null && window.History.enabled) {

                var pageName = window.location.pathname.substring(window.location.pathname.lastIndexOf('/') + 1);

                window.History.pushState({
                    plugin: "PxPage",
                    func: "UnBlockAction",
                    args: [null, isFneLocal]
                }, "Home", pageName);
            } else if (HashHistory && HashHistory.IsInit()) {
                //if hash history is enabled, the home button just brings us to the default "state" of the page
                window.location.hash = "#state";
            } else {
                PxPage.UnBlockAction(event, isFneLocal);
            }

            return false;
        },
        ValidateFneClosing: function () {
            var closingValid = $(PxPage.switchboard).trigger("ValidateFneClosing");
            if (!closingValid) {
                PxPage.Toasts.Error("Unable to close FNE, validation failed");
                return false;
            }
            if (PxPage.FrameAPI) {
                if ($('#fne-window .show-quiz').length) {
                    if ($('#fne-window').hasClass('auto-submit')) {
                        if (PxPage.autoSubmitting) {
                            PxPage.autoSubmitting = false;
                            return true;
                        }
                        else if (!confirm("Are you sure you want to close? Doing so will automatically submit the assignment.")) {
                            return false;
                        }
                        else {
                            PxPage.FrameAPI.fireEvent('submitrequested');
                            PxPage.autoSubmitting = true;
                            $('#fne-window').addClass('quiz-complete');                            
                            PxPage.Toasts.Success("Your work has been submitted.");
                            return false;
                        }
                    }
                    else if (!$('#fne-window').hasClass('quiz-complete') && !$('#fne-window .submission-history').length > 0) {
                        if (!confirm("Navigating away will save your progress. You will be able to resume later. Press OK to continue, or Cancel to stay in the current page.")) {
                            return false;
                        } else {
                            PxPage.Toasts.Info('Your work has been saved. Click Resume to continue your progress.');
                        }
                    }
                }

               
                // fix to Safari
                // throws an exception after going into question player
                try {
                    PxPage.FrameAPI.fireEvent("hostclosing");
                }
                catch (ex) {
                }
            }

            if ($('#fne-window').hasClass('require-confirm')) {
                if (!confirm("Are you sure you want to cancel? All your changes will be lost.")) {
                    return false;
                }
            }
            return true;
        },
        UnBlockPrep: function (event, isFneLocal) {
            // disable blocking the enter key if set
            if (window.inFneDisableEnter !== undefined) {
                window.inFneDisableEnter = undefined;
            }


            if (!PxPage.ValidateFneClosing()) {
                return false;
            }
            $(PxPage.switchboard).trigger("fneclosing");

            if ($('#fne-window').hasClass('has-changes')) {
                window.location.href += "?mode=unlock1";
                window.location.reload();
            }
            //need better way to do this
            //            if ($('#fne-window').hasClass('activate-course-fne')) {
            //                $("html").css('overflow', 'auto');
            //            }

            if (isFneLocal) {
                // Do something
                $('#main #right').append($('#fne-content #content-item'));
            }

            var fneClass;
            for (fneClass in PxPage.FneCloseHooks) {
                if (fneClass.length < 1 || $('#fne-content .' + fneClass).length > 0) {
                    PxPage.log("firing FNE close for " + fneClass);

                    PxPage.FneCloseHooks[fneClass]();
                }
                else {
                    PxPage.log("FAILED FNE close for " + fneClass);
                }
            }

            PxPage.ResetNavigations();
            PxPage.ResizeHooksInUse = {};

            $('#fne-window').removeClass('require-confirm');
            $('#fne-window').removeClass('auto-submit');
            return true;
        },
        UnBlockAction: function (event, isFneLocal) {

            if (!PxPage.UnBlockPrep(event, isFneLocal)) {
                return false;
            }


            $(PxPage.switchboard).trigger("fneclosing-beforeunblock");
            $('#fne-window #fne-minimize-action').hide();
            var unblockOpts = {
                onUnblock: function () {
                    $(PxPage.switchboard).trigger("fneclosed");
                }
            };
            $("body .single-column").unblock(unblockOpts);


            return false;
        },
        CanEditToc: function () {
            return (PxPage.Context.IsInstructor === "true") && !(PxPage.Context.IsProductCourse === "true");
        },

        OnReady: function (callback) {
            if (PxPage.IsReady) {
                callback();
            }
            else {
                $(PxPage.switchboard).bind("PxPageReady", callback);
            }
        },
        OnProductLoaded: function (callback) {
            if (PxPage.ProductLoaded) {
                callback();
            }
            else {
                $(PxPage.switchboard).bind("productloaded", callback);

            }
        },

        FitText: function (container, textSel) {
            // If the full text for the breadcrumbs not known, make a note of it.  Also, set the breadcrumbs to using the full text.
            container.find(textSel).each(function (i, v) {
                v = $(v);
                if (!v.attr('fullText')) {
                    vText = v.text().replace("&rsquo;", "\'");
                    v.attr('fullText', vText);
                }
                else {
                    v.text(v.attr('fullText'));
                }
            });

            // Set some styles we need for functionality
            container.css({ 'white-space': 'nowrap' });

            // Get the height of one line (since we have no-wrap set, it is only one line), then remove the nowrap
            var lineHeight = container.innerHeight();

            container.css({ 'white-space': 'normal' });

            // Get the length, l, of the longest text.
            var l = 0;
            container.find(textSel).each(function (i, v) {
                l = Math.max(l, $(v).text().length);
            });

            // While the container has wrapped to two lines (remember we removed the no-wrap), make the thing smaller, by setting all items 
            // to max l-1 characters.  If the breadcrumb trail is still wrapping, set all items to max l-2.  Continue until the 
            // breadcrumb trail is only one line in height.
            var exit = false;
            var iterations = 0; // Keep track of iterations to prevent possible infinite loop
            while (!exit) {
                var links = container.find(textSel);
                exit |= !links.length;
                links.each(function (i, v) {
                    exit = (iterations > 10000) || (container.innerHeight() <= lineHeight);
                    if (exit) {
                        return false;
                    }
                    var text = $(v).text();
                    if (text.length - 3 > l) {
                        var noElipsis = text.substring(0, text.length - 3);
                        $(v).text(noElipsis.substring(0, l) + '...');
                    }
                    iterations++;
                });
                l--;
            }
        },

        ToggleSection: function () {
            $('.toc .level a.expand.active').parent().children('span.children').toggle();
        },

        //called to perform any repetitive updates
        Update: function () {

            opts.html_editor_options.oninit = function () {
                PxPage.HideTinyMceCustomCtrls();
            };

            opts.html_editor_options.setup = function (ed) {
                ed.onPostRender.add(function (ed, cm) {
                    $(PxPage.switchboard).trigger("mceLoaded");
                });
            };

            opts.editableNote_editor_options.setup = function (ed) {
                ed.onKeyPress.add(function (ed, e) {
                    var keyCode = e.keyCode || e.which || e.charCode;
                    //if (keyCode == 13) {
                    //    PxComments.SaveCurrentNote(ed);
                    //}
                });
            };

            var forms = $("form.validate-form");
            if (forms.length > 0) {
                forms.validate();
            }

            var editors = $('textarea.html-editor:not(.wysiwyg)');

            //To fix  repeated appearance of tinyMCE on Xbook Quiz, following line has been added.
            if (editors.length > 0) PxPage.RemoveTinyMCE();

            //code to remove the tinymce instance if the editor is already there.
            editors.each(function (i, e) {
                var name = $(e).attr("name");
                if (tinyMCE && tinyMCE.getInstanceById(name) != null) {
                    PxPage.log("DUPLICATE EDITOR FOUND: " + name);
                    //$("<span>Duplicate Editor Found for : "+name+"</span>").insertBefore(e);
                    $(e).removeClass('html-editor');
                }
            });

            if (editors.length > 0) {
                if (jQuery.browser.msie) {
                    tinyMCE.relaxedDomain = "";
                }

                try {
                    if (tinyMCE.activeEditor) tinyMCE.activeEditor.remove();
                } catch (ex) { }

                if (PxPage.Context.course_tinyMCE_options == "") {
                    PxPage.html_editor_options.forced_root_block = "p";
                    tinyMCE.init(PxPage.html_editor_options);
                }
                else {
                    tinyMCE.init(opts[PxPage.Context.course_tinyMCE_options]);
                }
            }

            editors.each(function (i, e) {
                var id = $(e).attr("id");
                var name = $(e).attr("name");
                PxPage.log("initializing editor for: " + name);
                $(this).before('<div id="addLinkDialog"></div>');
                $(e).addClass("wysiwyg");
            });

            PxPage.SetFrameApiHooks();
            if (!window.addEventListener) {
                window.attachEvent("onload", PxPage.RemoveConfirmPageEvent);
            }
            else {
                window.addEventListener("load", PxPage.RemoveConfirmPageEvent, false);
            }
        },

        mceAddLink: function (selectedHTML) {

            //remove the HTML from the selected content
            var contentDiv = document.createElement("DIV");
            $(contentDiv).html(selectedHTML);
            var selectedText = $.trim($(contentDiv).text());
            var modalTitle = 'Create my own link'; //the title of the add link dialog

            var args = {};

            $.get(PxPage.Routes.mceAddLink, args, function (response) {

                $('#addLinkDialog').dialog({ width: 1000, height: 600, minWidth: 1000, minHeight: 600, modal: true, draggable: false, title: modalTitle, resizable: false
                    , create: function (event, ui) {
                        $('#addLinkDialog').html(response);
                    }
                    , close: function (event, ui) {
                    }
                });

                if (selectedText != '') {
                    $('#addLinkDialog #Title').val(selectedText);
                    $('#addLinkDialog #hdnSelectedText').val(selectedText);
                    $('#addLinkDialog #hdnSelectedHTML').val(selectedHTML);
                    $('#addLinkDialog #hdnIsSelectedText').val('true');
                }
            });
        },        

        //creates the quick link listbox in tinyMCE and populates with the dynamic data
        CreateQuickLinks: function (result) {

            //var result = [{ "LinkTitle": "Science Chapter 1", "LinkedItemId": "Google", "LinkUrl": "http://www.google.com" }, { "LinkTitle": "Science Chapter 2", "LinkedItemId": "Yahoo", "LinkUrl": "http://www.yahoo.com"}]

            tinymce.create('tinymce.plugins.testbox', {

                createControl: function (n, cm) {
                    switch (n) {
                        case 'quickLinkList':
                            var quickLink = cm.createListBox('quickLinkList', {
                                title: 'e-Book Quicklinks',
                                onselect: function (v) {
                                    var vals = v.split("|");
                                    var link = '<a target="_blank" href="' + vals[1] + '">' + vals[0] + '</a> ';
                                    tinyMCE.activeEditor.execCommand('insertHTML', false, link);
                                }
                            });

                            var item;
                            for (item in result) {
                                var lnkValue = result[item].LinkTitle + '|' + result[item].LinkUrl;
                                quickLink.add(result[item].LinkTitle, lnkValue);
                            }

                            // Return the new listbox instance
                            return quickLink;
                    }

                    return null;
                },

                getInfo: function () {
                    return { longname: "testbox plugin",
                        author: "Moxiecode Systems AB",
                        authorurl: "http://tinymce.moxiecode.com",
                        infourl: "http://wiki.moxiecode.com/index.php/TinyMCE:Plugins/dropdown",
                        version: tinymce.majorVersion + "." + tinymce.minorVersion
                    }
                }

            });

            // Register plugin with a short name
            tinymce.PluginManager.add('testbox', tinymce.plugins.testbox);
        },

        GetCommandNameForEditor: function () {
            if (jQuery.browser.msie) return "mceInsertRawHTML";
            else if (jQuery.browser.mozilla) return "insertHtml";
            else return "insertHtml";
        },

        HideTinyMceCustomCtrls: function () {
            $('.mceNativeListBox').hide();
        },

        TriggerAssignmentHtmlSave: function () {
            if (tinyMCE) {
                tinyMCE.triggerSave();

                /*$('textarea.html-editor').each(function (i, e) {
                var id = $(e).attr("id");
                PxPage.log("removing editor for: " + id);
                tinyMCE.execCommand('mceRemoveControl', true, id);
                });*/
            }
        },


        TriggerHtmlSave: function () {
            if (tinyMCE != null) {
                try {
                    if (tinyMCE) tinyMCE.triggerSave();
                } catch (ex) {
                }

                PxPage.RemoveTinyMCE();
            }
        },

        RemoveTinyMCE: function () {
            if (tinyMCE) {
                $('textarea.html-editor').each(function (i, e) {
                    var id = $(e).attr("id");
                    PxPage.log("removing editor for: " + id);
                    $(this).removeClass("wysiwyg");

                    try {
                        tinyMCE.execCommand('mceRemoveControl', false, e.id);
                    } catch (ex) {
                    }
                });
            }
        },

        SetSyllabusUpload: function () {
            $(document).off('click', '#syl-unblock-action, #syllabusForm input[name="cancel"]').on('click', '#syl-unblock-action, #syllabusForm input[name="cancel"]', function () { $.unblockUI(); return false; });
            $(document).off('click', '#upload-syllabus').on('click', '#upload-syllabus', function (event) {
                $.blockUI({
                    message: $('#syl-window'),
                    css: {
                        padding: 0,
                        margin: 0,
                        width: '30%',
                        top: '40%',
                        left: '35%',
                        textAlign: 'center',
                        cursor: 'wait'
                    }
                });
                event.preventDefault();
            });
        },

        ///arguments
        ///this.href = url to open
        ///attributes: title, 
        ///classes: fne-local, fixed, require-confirm, auto-submit, has changes, fne-minimize,  useIFrameForExternal, 
        ///         fixedHeader, creation-button
        OpenFneLink: function (event) {
            var url = ((this.href !== undefined) ? this : event).href;
            event.preventDefault();
            var title = event.title || $(event.target).attr("title");
            var contentClicked = $(this);
            var useLocal = contentClicked.hasClass('fne-local');
            var useIFrameForExternal = false;
            var loadFullFne = false;
            var fixed = contentClicked.hasClass('fixed');

            var requireConfirm = contentClicked.hasClass('require-confirm');
            var requireConfirmCustom = contentClicked.hasClass('require-confirm-custom');
            var autoSubmit = contentClicked.hasClass('auto-submit');
            var hasChanges = contentClicked.hasClass('has-changes');
            var minimize = contentClicked.hasClass('fne-minimize');

            if ($(this).hasClass('useIFrameForExternal')) {
                useIFrameForExternal = true;

            }

            if ($(this).hasClass('loadFullFne')) {
                loadFullFne = true;
            }
            var onFneLoaded = null;

            var fneArgs = {
                url: url,
                title: title,
                useLocal: useLocal,
                fixed: fixed,
                requireConfirm: requireConfirm,
                requireConfirmCustom: requireConfirmCustom,
                autoSubmit: autoSubmit,
                hasChanges: hasChanges,
                minimize: minimize,
                onFneLoaded: onFneLoaded,
                useIFrameForExternal: useIFrameForExternal,
                loadFullFne: loadFullFne
            };
            $(PxPage.switchboard).trigger("clickFneLink", contentClicked);
            _openFNE(fneArgs);


            return false;
        },

        OpenNonModalLink: function (event) {
            var url = ((this.href !== undefined) ? this : event).href;
            event.preventDefault();
            var title = $(event.target).attr("title");
            $.fn.PxNonModal.OpenNonModalFromLink({ url: url, title: title });
        },

        OpenNonModalLinkCentered: function (event) {
            var url = ((this.href !== undefined) ? this : event).href;
            event.preventDefault();
            var title = $(event.target).attr("title");
            $.fn.PxNonModal.OpenNonModalFromLink({ url: url, title: title, center: true });
        },

        OpenEnrollmentSwitchPopup: function (event, url) {
            var options = $.extend({ modal: true, draggable: true, closeOnEscape: false, width: 1024, height: 750, resizable: false, autoOpen: false, dialogClass: 'notitle' }, options || {});
            var tag = $("<div id='px-dialog'><div id='loadingBlock' style='padding-top:10px;padding-bottom:10px;'>Loading..</div></div>");

            var courseid = url.substring();

            tag.load(url, function (data, textStatus, XMLHttpRequest) {
                tag.dialog({ modal: options.modal, title: options.title, dialogClass: options.dialogClass, draggable: options.draggable, closeOnEscape: options.closeOnEscape, width: options.width, height: options.height, resizable: options.resizable, autoOpen: options.autoOpen, close: function () {
                    if ($(this).hasClass('ui-dialog-content')) {
                        $(this).dialog('destroy').empty().remove();
                    }
                    
                }
                }).dialog('open');
            });
            $(document).off('click', '#px-dialog .find-course-continue').on('click', '#px-dialog .find-course-continue', function (event) {
                var selectedCourse = $('#px-dialog #courses').val();
                if (selectedCourse == null || selectedCourse == "") {
                    return false;
                }
                var url = PxPage.Routes.GetCoursesDetails + "?courses=" + selectedCourse + "&switchEnrollFromCourse=" + $('#switchFormCourseId').val();
                tag.load(url, function (data, textStatus, XMLHttpRequest) { });
                event.preventDefault();
            });

            $(document).off('click', '#px-dialog #generic-course-id').on('click', '#px-dialog #generic-course-id', function (event) {
                var url = this.href;
                tag.load(url, function (data, textStatus, XMLHttpRequest) { });
                event.preventDefault();
            });

            $(document).off('click', '#px-dialog #join-course-confirmation').on('click', '#px-dialog #join-course-confirmation', function (event) {
                var selectedCourse = $('#px-dialog #courses').val();
                var url = PxPage.Routes.ShowEnrollmentConfirmation;  //+"?courses=" + selectedCourse + "&InsertionMode=Replace&LoadingElementDuration=0";
                var _domain = $('#px-dialog #domain').val();
                var _instructorId = $('#px-dialog #instructors').val();
                var _switchEnrollFromCourse = $('#px-dialog #switchEnrollFromCourse').val();
                tag.load(url, { courses: selectedCourse, domain: _domain, instructors: _instructorId, switchEnrollFromCourse: _switchEnrollFromCourse },
                     function (data) {
                         $('.ui-dialog-titlebar').hide()
                     });
                event.preventDefault();
            });

            $(document).off('click', '#px-dialog #wrong-course-link').on('click', '#px-dialog #wrong-course-link', function (event) {
                var url = this.href;
                tag.load(url);
                event.preventDefault();
            });

        },


        SetActivationLinks: function () {
            var activationLinks = $('.activation-link');
            var activationEnabled = PxPage.Context.CourseActivationEnabled == 'true' ? true : false;
            if (activationEnabled) {
                activationLinks.attr('href', PxPage.Routes.show_course_activation);
                $(document).off('click', '.activation-link').on('click', '.activation-link', PxPage.OpenFneLink);
                activationLinks.show();
            }
            else
                activationLinks.hide();

        },
        
        SetFneLinks: function () {
            $(document).off('click','.fne-link').on('click', '.fne-link', PxPage.OpenFneLink);
            $(document).off('click', '.fne-done-link').on('click', '.fne-done-link', function () {
                $(PxPage.switchboard).trigger("fne-done-link");
                if ($(this).hasClass("fne-done-link")) { //if this link still has the fne-done-link class, do not fall though and execute
                    return false;
                } else {
                    return true;
                }
            });
        },

        SetNonModalLinks: function () {
            $(document).off('click', '.nonmodal-link').on('click', '.nonmodal-link', PxPage.OpenNonModalLink);
            $(document).off('click','.nonmodalcenter-link').on('click', '.nonmodalcenter-link', PxPage.OpenNonModalLinkCentered);
        },

        TruncateText: function (stringToTruncate, maxSymbols) {
            var result = stringToTruncate;

            if (stringToTruncate.length > maxSymbols) {
                var lastIndex = stringToTruncate.indexOf(" ");

                stringToTruncate = stringToTruncate.substring(0, maxSymbols);

                if (lastIndex == -1) {
                    result = stringToTruncate;
                }
                else {
                    lastIndex = stringToTruncate.lastIndexOf(" ", maxSymbols - 1);

                    if (lastIndex != -1) {
                        stringToTruncate = stringToTruncate.substring(0, lastIndex);
                    }

                    result = stringToTruncate;
                }

                result = result + "...";
            }

            return result;
        },

        SetWindowResizeHandler: function () {
            PxPage.UpdateFneSize();
            $(window).resize(function () {
                PxPage.UpdateFneSize();
            });
        },

        SetFneTitle: function (title) {
            if (title && title != "") {
                if ($("#fne-window #fne-title-action-left").length > 0) {
                    $('#fne-window #fne-title-action-left #title-text').remove();
                    $('#fne-window #fne-title-action-left #title-original-text').remove();
                    $('#fne-window #fne-title-action-left')
                        .append('<span id="title-text">' + title + '</span>')
                        .append('<input type="hidden" id="title-original-text" value="' + title + '" />');
                }
                else if ($("#fne-window #fne-header #fne-title").length > 0) {
                    $("#fne-window #fne-header #fne-title").text(title);
                }
            } else {
                $('#fne-window #fne-title-action-left #title-text').remove();
            }

        },

        SetFneNavigations: function () {

            if ($('#content-nav').length) {
                //$('#content-nav').hide();
                $('#fne-title-right-nav #content_widget_highlight_menu').remove();
                $('#fne-title-right-nav').prepend($('#fne-content #content-nav #content_widget_highlight_menu').clone());
                $('#fne-title-right-nav .highlight-count').remove();
                $('#fne-title-right-nav #content_widget_highlight_menu').after($('#content-nav .highlight-count').clone());
                $('#fne-title-action-left #nav-container').remove();

                $('#fne-title-action-left').prepend($('#nav-container').clone());

            }
            
            if ($('.product-type-faceplate').length > 0 && $('#fne-content h2.content-title:first').text() == "") {
                $('#fne-content h2.content-title:first').hide();
            }
        },

        GetRandomId: function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        },

        GetUniqueId: function () {
            return (PxPage.GetRandomId() + PxPage.GetRandomId() + "-" + PxPage.GetRandomId()
                + "-" + PxPage.GetRandomId() + "-" + PxPage.GetRandomId() + "-" + PxPage.GetRandomId()
                + PxPage.GetRandomId() + PxPage.GetRandomId());

        },



        GetTitleFromContent: function () {
            var titleElement = $('#fne-content h2.content-title:first').clone();
            titleElement.children().remove();
            return $.trim(titleElement.text());
        },

        ResetNavigations: function () {
            $('#content-nav').show();
            $('h2.content-title:first').show();
            $('#fne-window #fne-title-left').css({ 'width': '100%', 'border-top-right-radius': '5px' });
            $('#fne-window #fne-title-right').width(0).hide();
            $('#fne-title-breadcrumb .breadcrumb').remove();
            $('#fne-title-right-nav #content_widget_highlight_menu').remove();
            $('#fne-title-right-nav .highlight-count').remove();            
            $('#fne-title-action-left #nav-container').remove();
        },

        RemoveConfirmPageEvent: function () {
            try {
                var u = "beforeunload";
                var v = 'unsafeWindow';
                if (v._eventTypes && v._eventTypes[u]) {
                    var r = v._eventTypes[u];
                    for (var s = 0; s < r.length; s++) {
                        v.removeEventListener(u, r[s], false);
                    }
                    v._eventTypes[u] = [];
                }
            }
            catch (e) {
            }
        },

        SetFrameApiHooks: function () {
            //PxPage.log("Setting FrameAPI hooks");
            var frame = $(".bh-component");

            if (frame.length > 0 && (typeof easyXDM !== 'undefined')) {
                frame.each(function (i, v) {
                    if (($(v).find("iframe").length == 0) && ($(v).attr("rpcinit") != "true")) {
                        $(v).attr("rpcinit", "true");
                        var rpc = new easyXDM.Rpc({
                            remote: $(v).attr("rel"),
                            container: $(v).attr("id")
                        },
                        {
                            local: {
                                init: function (success, error) {
                                    PxPage.log("BH component init");
                                    $(v).parent(".bh-component-wrapper").attr("isloaded", "true");
                                    PxPage.Loaded("#" + $(v).parent(".bh-component-wrapper").attr('id'));
                                    rpc.addListeners("componentsaved|componentcancelled|advancededitclicked|closegroupsetupwindow|description|submit|reviewstate|jswindowopening|editoropening|vieweropening|save|componentactive");
                                    if (success) {
                                        success();
                                    }
                                },
                                onEvent: function (event) {
                                    switch (event) {
                                        case "componentsaved":
                                            PxPage.log("ComponentSaved");
                                            if (TemplateSaveAs == true) {
                                                var oldItemId = $(".template-management-list li.selected").attr('itemid');
                                                var args = { itemId: oldItemId };
                                                TemplateSaveAs = false;
                                                $.post(PxPage.Routes.create_template, args, function (NewId) {
                                                    TemplateResponseIdNew = NewId;
                                                    PxSettingsTab.SaveAsTemplatePostSave();
                                                });
                                            } else if (TemplateProgateUpdates == true) {
                                                var itemId = $(".template-management-list li.selected").attr('itemid');
                                                TemplateProgateUpdates = false;
                                                var args = { itemId: itemId };
                                                //propagate changes
                                                $.post(PxPage.Routes.save_template_update, args, function (response) { });
                                            } else if (TemplateRegularSave == true) {
                                                var itemId = $(".template-management-list li.selected").attr('itemid');
                                                TemplateRegularSave = false;
                                                var args = { itemId: itemId };
                                                //propagate changes
                                                $.post(PxPage.Routes.save_template, args, function (response) { });
                                            }

                                            PxPage.ComponentSaved(arguments[1], arguments[2], arguments[3]);
                                            break;
                                        case "componentcancelled":
                                            PxPage.log("ComponentCancelled");

                                            if (arguments[1] === "homework" || arguments[1] === "submissiondetails") {
                                                $('#fne-window').addClass('quiz-complete');
                                                $('#fne-window').removeClass('require-confirm-custom');
                                            }

                                            PxPage.ComponentCancelled(arguments[1], arguments[2], arguments[3], v);
                                            break;
                                        case "advancededitclicked":
                                            PxPage.log("advancededitclicked");
                                            if (confirm('Are you sure you want to open this question in the HTS editor? The question will be reformatted to HTS.')) {
                                                var questionXml = arguments[2].questionXml;
                                                questionXml = questionXml.replace(/\"/g, '\'');
                                                PxQuiz.ConvertToHtsQuestion(questionXml);
                                            }
                                            break;
                                        case "closegroupsetupwindow":
                                            $(PxPage.switchboard).trigger("OpenManageGroupsWindow", [true]);
                                            break;
                                        case "description":
                                            PxQuiz && PxQuiz.ShowQuizDirections();
                                            break;
                                        case "submit":
                                            $('#fne-window').removeClass('auto-submit');
                                            $('#fne-window').addClass('quiz-complete');
                                            PxPage.autoSubmitting = false;
                                            PxPage.Toasts.Success("Your work has been submitted.");
                                            var showReview = $('.quiz-show-review');
                                            if (showReview.length && showReview.val() === 'False') {
                                                $('#fne-window').addClass('quiz-complete');
                                                $('#fne-window').removeClass('require-confirm-custom');
                                                PxPage.ComponentCancelled('submissiondetails');
                                            }
                                            break;
                                        case "save":
                                            // Go to the submission history for this assessment when an assessment 
                                            // is being saved.
                                            if (!$('#fne-window').hasClass('quiz-complete'))                                                
                                                PxPage.Toasts.Info("Your work has been saved. Click Resume to continue your progress.");

                                            PxPage.TriggerClick($(".save-link"));
                                            
                                            break;
                                        case "reviewstate":
                                            if (PxPage.autoSubmitting) {
                                                $('#fne-unblock-action').click();
                                                $(PxPage.switchboard).trigger("fnedonemode-exit");
                                            };
                                            PxPage.ComponentSubmit(arguments[1], arguments[2], arguments[3], v);
                                            break;
                                        case 'componentactive':
                                            $(PxPage.switchboard).trigger("bh-componentactive", [arguments[2]]);
                                            break;
                                    }
                                }
                            },
                            remote: {
                                addListeners: {},
                                fireEvent: {},
                                saveComponent: {},
                                callComponentMethod: {},
                                navigate: {},
                                getProperties: {},
                                getComponentState: {},
                                hasRight: {},
                                setShowBeforeUnloadPrompts: {}
                            }
                        });
                        PxPage.FrameAPI = rpc;

                    }
                    else {
                        //PxPage.log("FrameApi hooks already set up");
                    }

                    if (window.location.host.substring(0, 9) != 'localhost') {
                        if ($(v).find("iframe:visible").length > 0 && $(v).parent(".bh-component-wrapper").attr("isloaded") != "true") {
                            PxPage.Loading("#" + $(v).parent(".bh-component-wrapper").attr('id'));
                        }
                    }


                });
            }
        },
        ComponentSubmit: function (componentType, id, state, frame) {
            $(PxPage.switchboard).trigger("componentsubmit", [componentType, id, state, frame]);
        },

        ComponentCancelled: function (componentType, id, state, frame) {
            $(PxPage.switchboard).trigger("componentcancelled", [componentType, id, state, frame]);
        },

        ComponentSaved: function (componentType, id, state) {
            $(PxPage.switchboard).trigger("componentsaved", [componentType, id, state]);
        },

        UpdateFneSize: function () {
            // Want to update the height of fne-content given the height of the fne-window and the
            // height of the title bar.

            var windowHeight = $(window).height();
            var windowWidth = $(window).width();

            var studentViewBanner = $('.has-banner');
            $('.has-banner').parent().addClass("has-student-banner");
            var studentViewBannerHeight = !studentViewBanner.length ? 0 : studentViewBanner.height();
            windowHeight = windowHeight - $('#fne-window #fne-footer').height() - studentViewBannerHeight;

            $(PxPage.switchboard).trigger("updatefnesize");
            //$('#fne-window').height(windowHeight);

            var headerHeight = $('#fne-window #fne-header').height() + $('.fne-edit-tabs').height();
            if ($('#fne-window #fne-title').length > 0) {
                //var titleHeight = $('#fne-window #fne-title').height() + ($('#fne-window #fne-title').offset().top - $(window).scrollTop()) * 2;
                var titleHeight = $('#fne-window #fne-title').height() + ($('#fne-window #fne-title').position().top - $(window).scrollTop()) * 2;
                if (titleHeight > headerHeight)
                    headerHeight = titleHeight;
            }

            // If fne-link has class fixed, then set fne-window size to same as actual content.
            if ($('#fne-window').hasClass('fixed')) {
                var contentWidth = $('#fne-window #fne-content :first').outerWidth(true);
                var fneWidth = windowWidth - contentWidth;
                $('#fne-window').parent().css({ 'left': fneWidth / 2, 'right': fneWidth / 2 });
                $('#fne-window #fne-content').css('overflow', 'auto');
            } else {
                var contentItems = $('#fne-content').add($('#fne-content').find('#content'));
                if (contentItems.length > 0) {
                    if ($('#fne-window').hasClass('full-content')) {
                        contentItems.height($('#fne-window').height() - $('#fne-window #fne-header').height()).css('width', '100%');
                    } else {
                        var footerheight = 0;
                        if (contentItems.hasClass('footer-visible')) {
                            footerheight = 65;
                        }
                        contentItems.height(windowHeight - headerHeight).css('width', '100%');

                        $('#fne-content').find('#content').height(windowHeight - headerHeight - footerheight).css('width', '100%');
                    }
                }
            }

            // iframe height is content height - (content nav height + content title height)
            var contentHeight = $('#fne-content').find('#content').outerHeight(true);
            var contentNavHeight = $('#fne-content #content-item #content-nav').outerHeight(true);
            var contentTitleHeight = $('#fne-content #content-item .content-title').outerHeight(true);
            var iframe = $('#fne-content').find('#content');
            if (iframe.length > 0) {
                iframe.height(contentHeight - (contentNavHeight + contentTitleHeight));

                // If we're showing the discussion widget, want change the width of
                // the content iframe to accommodate it.
                var discussionWidget = $(".discussion-widget");
                if (discussionWidget.length > 0) {
                    var contentWidth = $('#fne-content').find('#content').outerWidth(true);
                    var discussionWidth = discussionWidget.outerWidth(true);
                    iframe.width(contentWidth - discussionWidth);
                    discussionWidget.height(iframe.height());
                }
            }

            PxPage.AutoFitText($('#title-text'), $('#title-original-text').val());
            if ($('#fne-title-breadcrumb .breadcrumb').length) {
                var topPadding = $('#fne-title-breadcrumb').height() + 7;
                $('#fne-title-right-nav').css('padding-top', topPadding + 'px');
            }
            else {
                $('#fne-title-right-nav').css('padding-top', '7px');
            }

            // resizing the i-frame if it has a static-height
            if ($("#document-body-iframe").hasClass("static-height")) {
                    $("#document-body-iframe").css("height", $("#fne-content").height() + "px");
            }

            // Run resize hooks for anything that has included itself, if we're showing that function
            var fneClass;
            for (fneClass in PxPage.ResizeHooksInUse) {
                PxPage.ResizeHooksInUse[fneClass]();
            }

            // Remove horisontal scrollbar from the iframe
            $("#document-body-iframe").contents().find("body").css('overflow-x', 'hidden');
        },

        AutoFitText: function (element, originalText) {
            if (originalText == '') return;
            element.text(originalText);
            element.css('white-space', 'nowrap');
            var lineHeight = element.innerHeight();
            element.css('white-space', 'normal');
            var isMultipleLines = false;
            var iterations = 0;
            var exit;
            setTimeout(function () {
                while (true) {
                    isMultipleLines = element.innerHeight() > lineHeight; // test to see if text is wrapped
                    exit = (iterations++ > 10000) || !isMultipleLines;
                    if (exit) {
                        break;
                    }
                    element.text(element.text().substr(0, element.text().length - 5) + '...');
                }
            }, 100);
        },

     
        
        Logout: function () {
            MySite_RAif_init('dologout');
        },

        //Restore Default Instructions button
        RestoreDefault: function (itemId, defaultCategoryParentId) {
            var confirmReset = confirm("Are you sure you want to clear Title and Instructions field?");

            if (confirmReset) {
                $.ajax({
                    type: 'POST',
                    url: PxPage.Routes.refresh_default_item_description,
                    data: {
                        itemId: itemId
                    },
                    success: function (response) {                     
                        $("#Title").val(response.title);
                        $("#Content_Title").val(response.title);
                        tinyMCE.activeEditor.setContent(response.description);
                        if (tinyMCE.activeEditor.getContent().length <= 0) {
                            $("#Description-word-count").text("0");

                        }
                    },
                    error: function (req, status, error) {
                        PxPage.Toasts.Error(error);
                    }
                });
            } else {
                return;

            }


        },

        TriggerClick: function (control) {
            if (control instanceof jQuery) {
                control = $(control)[0];
            }
            if (control && control.click) {
                control.click();
            } else {
                var evObj = document.createEvent('MouseEvents');
                evObj.initMouseEvent('click', true, true, window,
                    0, 0, 0, 0, 0, false, false, false, false, 0, null);
                control.dispatchEvent(evObj);
            }
        },

        Loading: function (controlId, isClass, maskParent, message, optionsOverride) {
            /// <summary>
            /// Displays a blocking UI over the dom elements whos id is controlId
            /// </summary>
            /// <param name="controlId" type="String">The id of the element to block.  
            /// Doesn't matter if the # is present or not. If not defined will default to 'main'. </param>
            /// <param name="isClass" type="Boolean">If true, appends a '.' before controlId in the jQuery selector 
            /// to get the element. </param>
            /// <param name="maskParent" type="Boolean">Changes the blocking target to the controlId elements parent</param>
            /// <param name="message" type="String">Changes the blocking target to the controlId elements parent</param>
            /// <param name="optionsOverride" type="Object">Ability to override the blockUI options</param>

            if ($.blockUI == null) {
                return;
            }
            var target = "";
            if (message == null) {
                messsage = "";
            }
            if (controlId == undefined || controlId == "" || controlId == null) {
                controlId = 'main';
            }
            if (controlId.nodeType || controlId instanceof $) {
                target = $(controlId);
            } else {
                if (controlId.indexOf('#') == 0) {
                    controlId = controlId.slice(1, controlId.length);
                }
                else if (controlId.indexOf('.') == 0) {
                    controlId = controlId.slice(1, controlId.length);
                    isClass = true;
                }

                if (isClass != null && isClass == true) {
                    target = $('.' + controlId);
                } else {
                    target = $('#' + controlId);
                }
            }
            if (maskParent != null && maskParent == true) {
                target = target.parent();
            }
            var blockFunc = function () {

                var blockOpts = {
                    message: '<div id="loadingBlock">' + message + '</div>',
                    css: {
                        padding: 0,
                        margin: '0px 0px 0 0',
                        top: '50%',
                        left: 0
                    },
                    fadeIn: 400
                };

                $.extend(true, blockOpts, optionsOverride);
                $(target).block(blockOpts);
            };
            //To avoid flicker of the block ui, display on a delay. A call
            //to Loading with the same controlId before the timer expires will
            //cancel the display of the block UI.
            if (!controlId.nodeType) {
                if (loadingTimer[controlId] != null) {
                    clearTimeout(loadingTimer[controlId]);
                }
                loadingTimer[controlId] = setTimeout(blockFunc, 350);
            } else {
                blockFunc();
            }
        },

        Loaded: function (controlId, isClass, maskParent) {
            if (controlId == undefined || controlId == "" || controlId == null) {
                controlId = 'main';
            }
            var target = "";
            if (controlId.nodeType || controlId instanceof $) {
                target = $(controlId);
            } else {
                if (controlId.indexOf('#') == 0) {
                    controlId = controlId.slice(1, controlId.length);
                }
                else if (controlId.indexOf('.') == 0) {
                    controlId = controlId.slice(1, controlId.length);
                    isClass = true;
                }

                if (isClass != null && isClass == true) {
                    target = $('.' + controlId);
                } else {
                    target = $('#' + controlId);
                }
            }
            if (maskParent != null && maskParent == true) {
                target = target.parent();
            }
            if ($(target).length > 0) {
                if (!controlId.nodeType) {
                    clearTimeout(loadingTimer[controlId]);
                }

                $(target).unblock();

            }
        },

        ShowCustomEditor: function (args) {
            newDiv = $(sel.AssignDialog);
            editorDiv = $(edit.Editor);
            callback = args.callback;
            restore = args.restore;
            sender = args.sender;

            var editorName = "showcustomeditor";

            if (newDiv.length > 0) {
                newDiv.empty();
            }
            else {
                newDiv = $(document.createElement('div')).attr("id", editorName).attr("class", editorName);
            }

            var restorediv = $(document.createElement('div')).attr("id", "restorediv").attr("class", "showcustomeditor-restorediv");
            var restoreicon = $(document.createElement('i')).attr("class", "icon-reply stand-alone black");
            var restorelabel = $(document.createElement('a')).attr("id", "restorelabel").attr("href", "#").attr("class", "showcustomeditor-restorelabel");
            restorelabel.html('Restore Publisher-Supplier ' + args.title);
            if (restore) {
                restorelabel.bind('click', function () {
                    PxPage.Loading(editorName);
                    $.ajax({
                        url: restore,
                        data: {
                            id: args.id
                        },
                        type: "POST",
                        success: function (data) {
                            if (data.Result) {
                                tinyMCE.activeEditor.setContent(data.Result, { format: 'raw' });
                                PxPage.Loaded(editorName);
                            }
                        }
                    });
                });
            }

            var m = $(document.createElement('textarea')).attr("name", "customeditor").attr("id", "customeditor").attr("class", "showcustomeditor-textarea html-editor");

            m.html($(sender).html());
            newDiv.append(m);
            restorediv.append(restoreicon);
            restorediv.append(restorelabel);
            newDiv.append(restorediv);

            newDiv.dialog({
                autoOpen: true,
                height: 450,
                width: 700,
                resizable: false,
                dialogClass: 'showcustomeditor-dialog',
                title: 'Edit ' + args.title,
                modal: true,
                buttons: {
                    "Save": function () {
                        var v = tinyMCE.activeEditor.getContent();
                        if (callback) {
                            callback(args.id, sender, v);
                        } else {
                            $(sender).html(v);
                        }

                        $(this).dialog("close");
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                },
                close: function () {
                    tinyMCE.execCommand('mceRemoveControl', true, 'customeditor');

                    try {
                        if (tinyMCE.activeEditor) tinyMCE.activeEditor.remove();
                    }
                    catch (ex) {
                    }

                    $(this).remove();
                }
            });

            newDiv.dialog("widget")
            .find(".ui-dialog-buttonpane button span")
            .eq(0).addClass("showcustomeditor-dialog-save").end()
            .eq(1).addClass("showcustomeditor-dialog-cancel").end();
            PxPage.Update();
        },

        ShowDatePicker: function (args) {

            var callback = args.callback,
                closeCallback = args.onCloseCallBack,
                sender = args.sender,
                customTitle = args.customTitle,
                calendarMode = args.calendarMode,
                customValues = args.customValues,
                oldStartDate = args.oldStartDate,
                oldDueDate = args.oldDueDate,
                dialogClass = args.dialogClass,
                instructions = args.instructions,
                itemAssigned = args.itemAssigned,
                assignWithOutDueDate = args.assignWithOutDueDate,
                newDiv = $(sel.AssignDialog),
                calenderdiv = $('#PXAssignCalendar'),
                unassignButtonDivLC = $('#unassignButtonLC'),
                assignwithoutDueDatediv = $(document.createElement('div')).attr("id", "assignWithoutDueDate"),
                checkboxLabel = $(document.createElement('p')).attr("id", "checkboxLabel"),
                calInstructions = $(document.createElement("i")).text("Click on a date to set a due date.");

            var startDate = new Date(oldStartDate);
            var dueDate = new Date(oldDueDate);



            checkboxLabel.text("No Due Date");
            if (!calendarMode) {
                calendarMode = 'single';
            }

            PxPage.AssignmentDatesCallback = { Callback: callback, CustomValues: customValues, OnCloseCallback: closeCallback };
            PxPage.AssignmentDatesCallback.CustomValues.AutoClose = args.autoCloseAfterAssign;
            if (isNaN(dueDate) != true && customValues.LC != undefined && customValues.LC != null && customValues.LC == true) {
                PxPage.AssignmentDateSelected(dueDate)
            }
            /* Creates the wrapper div for the calendar */

            if (newDiv.length > 0) {
                newDiv.empty();
            }
            else {
                newDiv = $(document.createElement('div')).attr("id", "assigndialog");
            }

            /* Creates the div for the calendar */
            if (calenderdiv.length > 0) {
                calenderdiv.empty();
            }
            else {
                calenderdiv = $(document.createElement('div')).attr("id", 'PXAssignCalendar');
            }

            var assignwithoutDueDateInput = $('#PXAssignWithoutDueDateCheckBox');
            assignwithoutDueDateInput = $(document.createElement('input')).attr("id", 'PXAssignWithoutDueDateCheckBox');
            assignwithoutDueDateInput.attr('type', 'checkbox');

            if (customValues.AssignedWithnoDueDate != undefined && customValues.AssignedWithnoDueDate == true) {
                assignwithoutDueDateInput.prop('checked', true);
            }
            calInstructions.attr("id", "calInstructions");
            if (instructions == true) {
                calenderdiv.append(calInstructions);
            }
            newDiv.append(calenderdiv);

            /* Instructions */

            if (instructions == true) {

                var assignmentInstructions = $(document.createElement('div')).attr("id", 'assignmentInstructions');

                var instructions = document.createElement('p');
                instructions.innerHTML = "Instructions:";
                var textareaMCE = $(document.createElement('textarea')).attr("id", 'zen-editor'); //.attr("id", 'html-editor')
                textareaMCE.attr("class", "zen-editor instructions");
                textareaMCE.attr("name", "instructions");
                if (customValues.Instructions != undefined) {

                    textareaMCE.text(customValues.Instructions);
                }

                assignmentInstructions.append(instructions);
                assignmentInstructions.append(textareaMCE);
                newDiv.append(assignmentInstructions);


            }


            if (customValues.LC == undefined) {

                customValues.LC = false;
            }

            if (customValues.LC) {


                assignwithoutDueDateanchor = $(document.createElement('a')).attr("id", 'PXAssignWithoutDueDate').text('Assign without a due date');
                assignwithoutDueDateanchor.attr('href', '#');
                assignwithoutDueDateanchor.attr("style", "text-align: center;font-size: 12px;color: blue;clear: left;display: block;padding-top:10px;width:217px;");

                assignwithoutDueDatediv.append(assignwithoutDueDateanchor);
                if (customValues != null && customValues.unassignLC) {


                    unassignButtonDivLC = $(document.createElement('div')).attr("id", 'unassignButtonLC');

                    var unassignButtonLC = document.createElement('input');
                    unassignButtonLC.setAttribute('type', 'submit');
                    unassignButtonLC.setAttribute('name', 'unassignButton');
                    unassignButtonLC.setAttribute('class', 'unassignButtonLC');
                    unassignButtonLC.setAttribute('value', 'Unassign');
                    unassignButtonLC.setAttribute('id', customValues.id);

                    unassignButtonDivLC.append(unassignButtonLC);

                    assignwithoutDueDatediv.append(unassignButtonDivLC);

                }

            }
            if (assignWithOutDueDate == undefined) {
                args.assignWithOutDueDate = false;
            }

            if (assignWithOutDueDate == true) {
                assignwithoutDueDatediv.append(assignwithoutDueDateInput);
                assignwithoutDueDatediv.append(checkboxLabel);
            }


            //If due date is equal to max year, set it to null
            if (isNaN(dueDate.getTime()) || dueDate.getYear() === 8099) {
                dueDate = null;
            }

            var dateFrom = startDate == null ? '' : startDate;
            var dateTo = dueDate == null ? '' : dueDate;
            var dateCurrent = dueDate == null ? new Date() : dueDate;

            var buttonsDiv = $(document.createElement('div')).attr("id", 'buttonscontainer');
            if (PxPage.AssignmentDatesCallback.CustomValues.AutoClose == false) {
                var assignButton = $(document.createElement('input'));
                var unAssignButton = $(document.createElement('input'));
                var cancelButton = $(document.createElement('input'));
                unAssignButton.attr("type", "button");
                assignButton.attr("type", "button");
                cancelButton.attr("type", "button");

                if (itemAssigned) {
                    assignButton.attr("value", "Assign");
                    assignButton.attr("class", 'primary assignButton ');
                    unAssignButton.attr("value", "Unassign");
                    unAssignButton.attr("class", 'UnassignButton primary');
                    buttonsDiv.append(unAssignButton);
                } else {
                    assignButton.attr("value", "Assign");
                    assignButton.attr("class", 'primary assignButton ');
                }
                cancelButton.attr("class", 'cancelbutton secondary ');
                cancelButton.attr("value", "Cancel");

                newDiv.append(buttonsDiv);

                buttonsDiv.append(assignButton);
                buttonsDiv.append(cancelButton);

                $(document).off("click", '.assignButton').on("click", '.assignButton', function () {

                    var i = $(".instructions").val();

                    if (i == null) {
                        i = "";
                    }
                    PxPage.AssignmentDatesCallback.CustomValues.Instructions = i;
                    PxPage.OnAssignmentClicked();

                });

                $(document).off("click", '#PXAssignWithoutDueDateCheckBox').on("click", '#PXAssignWithoutDueDateCheckBox', function () {

                    if ($("#PXAssignWithoutDueDateCheckBox").is(":checked")) {

                        PxPage.AssignmentDatesCallback.CustomValues.StartDate = "01/01/0001";
                        PxPage.AssignmentDatesCallback.CustomValues.EndDate = "01/01/0001";
                    }
                    $('#PXAssignCalendar').find('.datepickerSelected').removeClass('datepickerSelected');

                });
            }

            newDiv.dialog({ width: 'auto', height: 'auto', modal: true, draggable: false, resizable: false, title: customTitle, dialogClass: dialogClass, closeOnEscape: true,
                open: function (event, ui) {
                    event.stopImmediatePropagation();
                    var aDates = { StartDate: '', DueDate: '', Counter: 0 };
                    PxPage.AssignmentDates = aDates;
                    newDiv.attr("mode", "modal");

                    if (calendarMode == 'single') {

                        $('#PXAssignCalendar').DatePicker({
                            flat: true,
                            date: dateTo,
                            current: dateCurrent,
                            calendars: 1,
                            mode: calendarMode,
                            starts: 0,
                            duration: "fast",
                            changeMonth: true,
                            changeYear: true,
                            gotoCurrent: true,
                            onChange: PxPage.AssignmentDateSelected
                        });

                        calenderdiv.append(assignwithoutDueDatediv);
                    }
                    else if (calendarMode == 'range') {
                        $('#PXAssignCalendar').DatePicker({
                            flat: true,
                            date: [dateFrom, dateTo],
                            current: dateCurrent,
                            calendars: 1,
                            mode: calendarMode,
                            starts: 0,
                            duration: "fast",
                            changeMonth: true,
                            changeYear: true,
                            gotoCurrent: true,
                            onChange: function (formatted, dates) { PxPage.AssignmentDateSelected(formatted, dates, calendarMode, dateFrom, dateTo, dateCurrent); },
                            onClick: PxPage.OnAssignmentClicked
                        });

                    }


                },

                beforeClose: function (event) {

                    if (typeof (args.onCloseCallBack) == 'function' || $(event.target).hasClass(".ui-dialog-content")) {
                        var confirmAnswer = confirm('Are you sure you want to close?  Your assignment will not be saved until you click the “Assign” button');

                        if (confirmAnswer == true) {
                            $(sel.AssignDialog).remove();
                            if ($(this).hasClass('ui-dialog-content')) {
                                $(this).dialog('destroy');
                            }

                        } else {

                            return false;
                        }

                    }
                    return true;

                },
                close: function (event, ui) {

                    if (calendarMode == 'range') {
                        PxPage.AssignmentStartDateSelected();
                    }

                    $(sel.AssignDialog).remove();
                    $(this).dialog("destroy");
                    tinyMCE.execCommand('mceRemoveControl', true, 'zen-editor');
                }



            });

            //newDiv.position({ my: "center", at: "center", of: window });
        },

        AssignmentDateSelected: function (formatted, dates, mode, dateFrom, dateTo, dateCurrent, autoClose) {

            var startDate = new Date();
            var endDate = new Date();
            var now = new Date();
            if (formatted[0] == formatted[1] && mode === "range" && PxPage.AssignmentDatesCallback.CustomValues.Counter == 0) {
                startDate = new Date(formatted[0]);
                if ((startDate < now) && (startDate.format('mm/dd/yyyy') != now.format('mm/dd/yyyy'))) {
                    PxPage.Toasts.Error("The selected date has already past. Please specify a start date that occurs in the future.");
                    $('#PXAssignCalendar').empty();   // temp code, trying to reset calendar
                    $('#PXAssignCalendar').DatePicker({
                        flat: true,
                        date: [startDate, ''],
                        current: dateCurrent,
                        calendars: 1,
                        mode: mode,
                        starts: 0,
                        duration: "fast",
                        changeMonth: true,
                        changeYear: true,
                        gotoCurrent: true,
                        onChange: function (formatted, dates) { PxPage.AssignmentDateSelected(formatted, dates, mode, startDate, '', dateCurrent); }
                    });
                    return;
                }
                var aDates = { StartDate: startDate.format('mm/dd/yyyy') + ' 11:59 AM', DueDate: startDate.format('mm/dd/yyyy') + ' 11:59 PM', Counter: PxPage.AssignmentDates.Counter + 1 };
                PxPage.AssignmentDates = aDates;
            }
            else {
                if ($.isArray(dates)) {
                    startDate = new Date(dates[0]);
                    endDate = new Date(dates[1]);
                } else {
                    startDate = new Date(formatted);
                    endDate = new Date(formatted);
                }

                $("#PXAssignWithoutDueDateCheckBox").prop('checked', false);

                var aDates = { StartDate: startDate.format('mm/dd/yyyy') + ' 12:00 AM', DueDate: endDate.format('mm/dd/yyyy') + ' 11:59 PM' };
                PxPage.AssignmentDates = aDates;

                PxPage.AssignmentDatesCallback.CustomValues.StartDate = startDate.format('mm/dd/yyyy');
                PxPage.AssignmentDatesCallback.CustomValues.EndDate = endDate.format('mm/dd/yyyy');
                PxPage.AssignmentDatesCallback.CustomValues.Counter = 0;

                if (PxPage.AssignmentDatesCallback.CustomValues.AutoClose == undefined || PxPage.AssignmentDatesCallback.CustomValues.AutoClose == null) {
                    PxPage.AssignmentDatesCallback.CustomValues.AutoClose = true;
                }

                if (PxPage.AssignmentDatesCallback.CustomValues.AutoClose == true) {

                    //if date is not valid, that may mean user selects something else, ie: changing month.
                    //then do not call close the dialog, let user procede.
                    if (!isNaN(dates.valueOf())) {
                        PxPage.OnAssignmentClicked();
                        $(sel.AssignDialog).remove();
                        if ($(this).hasClass('ui-dialog-content')) {
                            $(this).dialog("destroy");
                        }
                    }
                }

            }
        },

        OnAssignmentClicked: function () {

            PxPage.AssignmentDatesCallback.Callback(PxPage.AssignmentDates, PxPage.AssignmentDatesCallback.CustomValues);

        },
        AssignmentStartDateSelected: function () {

            var now = new Date();
            var startDate = new Date(PxPage.AssignmentDates.StartDate);

            if (PxPage.AssignmentDates.StartDate.length != 0) {
                if ((startDate >= now) || (startDate.format('mm/dd/yyyy') == now.format('mm/dd/yyyy'))) {
                    PxPage.AssignmentDatesCallback.Callback(PxPage.AssignmentDates, PxPage.AssignmentDatesCallback.CustomValues);
                }
            }
            PxPage.AssignmentDatesCallback = { Callback: null, CustomValues: null };
            PxPage.AssignmentDates = { StartDate: null, DueDate: null, Counter: 0 };
        },

        ScrollIntoView: function (element, container) {
            var containerTop = $(container).scrollTop();
            var containerBottom = containerTop + $(container).height();
            var elemTop = $(element).offset().top;
            var elemBottom = elemTop + $(element).outerHeight();
            if (elemTop < containerTop) {
                $(container).scrollTop(elemTop);
            } else if (elemBottom > containerBottom) {
                var delta = elemBottom - $(container).height();
                if (delta < $(element).outerHeight())
                    delta = $(element).outerHeight();
                $(container).scrollTop(delta);
            }
            PxPage.log('scrollTop: ' + containerTop + ' contHeight: ' + $(container).height() + ' contBottom: ' + containerBottom + ' elmTop: ' + elemTop + ' elmHeight: ' + $(element).outerHeight() + ' elmBottom: ' + elemBottom);
        },

        ValidateDeleteDocResource: function (event) {
            if (confirm("Do you want to delete the item?")) {
                $(event.target).parent().find('.ajaxRemove').click();
            }

            return false;
        },

        ValidateDeleteResources: function (container, element) {
            var item = '#' + container + ' #' + element;
            //if (!$(item).is(':checked')) {
            //fix for PLATX-4909
            if (!$('input:checked').val()) {
                PxPage.Toasts.Error("Please select atleast one resource to delete");
                return false;
            }
            else {
                if (confirm("Do you want to delete the selected item(s)?")) {
                    PxPage.OnFormSubmit(null, true);
                    return true;
                }
            }
            return false;
        },

        ValidateDeleteLinkResource: function (event) {
            if (confirm("Do you want to delete the selected item(s)?")) {
                $(event.target).parent().find('.ajaxRemove').click();
            }

            return false;
        },

        ValidateDeleteResourcesAlt: function (container, element) {
            var item = '#' + container + ' #' + element;
            //if (!$(item).is(':checked')) {
            //fix for PLATX-4909
            if (!$('input:checked').val()) {
                PxPage.Toasts.Error("Please select atleast one resource to delete");
                return false;
            }
            else {
                if (confirm("Do you want to delete the selected item(s)?")) {
                    PxPage.OnFormSubmit(null, true);
                    $("#deleteSelectedLink").click();
                    $('#fne-window').removeClass('require-confirm');
                    return true;
                }
            }
            return false;
        },

        // close - optional parameter object with following properties
        //      reason: "cancel", or null
        //      id: id of the item being viewed when cancellation happend
        CloseNonModal: function () {
            $.fn.PxNonModal.Close();
            $(PxPage.switchboard).trigger("closenonmodal", [close]);

            PxPage.Loaded();
            return false;
        },

        CloseFne: function () {
            if ($('#nonmodal-unblock-action').length > 0) {
                $('#nonmodal-unblock-action').click();
            }

            if ($('#fne-unblock-action').length > 0) {
                $('#fne-unblock-action').click();
            }

            if ($(".ui-icon-closethick").length > 0) {
                $(".ui-icon-closethick").click();
            }

        },

        CancelForm: function (itemId) {
            ContentWidget.ShowContentItem(itemId, "Preview");
        },

        ChangeReport: function () {
            var dropdown = $("#ReportSelector").val();
            if (dropdown == 'ItemAnalysis') {
                $('#ItemDetailsReport').hide();
                $('#ItemSummaryReport').hide();
                $('#ItemAnalysisReport').show();
            } else if (dropdown == 'ItemSummary') {
                $('#ItemDetailsReport').hide();
                $('#ItemAnalysisReport').hide();
                $('#ItemSummaryReport').show();
            } else if (dropdown == 'ItemDetails') {
                $('#ItemSummaryReport').hide();
                $('#ItemAnalysisReport').hide();
                $('#ItemDetailsReport').show();
            }
        },
        
        ShowDataSets: function (title, id, url) {
            $('body').remove('#dataset');

            // add a '/' to end of url if needed
            if (url.charAt(url.length - 1) != "/") {
                url = url + "/";
            }

            var file_name = title;
            var ti_calc_ext = "8Xm"; // default file extension for TI-Calc
            // if the id passed in has an extension, then we need to grab the file name and ext
            if (id.match(/(.*)\.(.*)/)) {
                ti_calc_ext = RegExp.$2;
            }

            var ds = '<ul>';
            ds += '<li><b>File Format</b></li>';
            ds += '<li><a href="' + url + 'PC_Text/' + file_name + '.txt" target="_blank">PC-Text</a></li>';
            ds += '<li><a href="' + url + 'Mac_Text/' + file_name + '.txt" target="_blank">Mac-Text</a></li>';
            ds += '<li><a href="' + url + 'Excel/' + file_name + '.xls" target="_blank">Excel</a></li>';
            ds += '<li><a href="' + url + 'Minitab/' + file_name + '.mtp" target="_blank">Minitab</a></li>';
            ds += '<li><a href="' + url + 'SPSS/' + file_name + '.por" target="_blank">SPSS</a></li>';
            ds += '<li><a href="' + url + 'TI_Calc/' + file_name + '.' + ti_calc_ext + '" target="_blank">TI-Calc</a></li>';
            ds += '<li><a href="' + url + 'JMP/' + file_name + '.jmp" target="_blank">JMP</a></li>';
            ds += '</ul>';

            var div = '<div id="dataset" class="dataset" title="' + title + '">' + ds + '</div>';

            $('body').append(div);

            $("#dataset").dialog();

            return false;
        },

        OpenSupp: function (type, course, file) {
            if (type == 'crunchit') {
                var version = 3;
                var url = "http://" + window.location.host + PxPage.Routes.CrunchitBridgePage + "?version=" + version + "&bcourse=" + course + "&file=" + file;

                var logWin = window.open('', type);
                logWin.location = url;
            }
        },

        SetCourseType: function () {
            var selected = $("#selectCourseType option:selected").val();
            $('#CourseType').val(selected);
            return true;
        },
        ContentCreated: function (response) {
            $(PxPage.switchboard).trigger("contentcreated", [response]);
        },
        ContentOpen: function (response) {
            $(PxPage.switchboard).trigger("contentopenCloseCreateNewScreen", [response]);
        },

        PrepareDocForm: function () {
            if ($('#saveItem').find('form').length > 0) {
                defaults.internalForm = $('#saveItem').find('form');
                $('#saveItem').find('form').replaceWith("<iframe id='internalForm'>" + defaults.internalForm + "</iframe>");
            }
        },
        RestoreDocForm: function () {
            if (defaults.internalForm != '') {
                $('#saveItem').find('#internalForm').replaceWith(defaults.internalForm);
            }
        },

        OnFormSubmit: function (message, isShowSubmit, validation, callbackFunc, isShowPageLoading) {
            if (validation != null) {
                var contentWrapper = $(validation.form).closest('.contentwrapper');

                //if assignment of a content item is possible from the Content Creation window
                var isContentCreateAssign = false;
                if (contentWrapper.find('#hdnIsContentCreateAssign').length > 0) {
                    isContentCreateAssign = $.parseJSON(contentWrapper.find('#hdnIsContentCreateAssign').val());
                }
            }

            if (validation != null) {
                if (validation.rules) {
                    $(validation.form).validate({ rules: validation.rules });
                }

                var isValid = $(validation.form).valid();

                if (validation.validationOverride != null) {
                    isValid = true;
                }

                if (isContentCreateAssign) {
                    isValid = ContentWidget.ValidateDateOnAssign(contentWrapper);
                }

                if (!isValid) {
                    PxPage.Loaded();
                    return false;
                }
                else {
                    PxPage.TriggerHtmlSave();

                        var data = $(validation.form).serialize();
                        if (data.indexOf("&Content.Description=&") != -1) {
                            data = data.replace("&Content.Description=", "&Content.Description=+");
                        } else if (data.match("Content.Description=$")) {
                            data = data.replace("Content.Description=", "Content.Description=+");
                    }

                    if (validation.data != null) {
                        if (data != '')
                            data += '&';

                        data += _serializeMap(validation.data);

                        if (validation.externalData != null) {
                            if (data != '')
                                data += '&';
                            data += validation.externalData();
                        }
                    }

                    if ((isShowSubmit == null) || (isShowSubmit == false)) {
                        $('input[type="submit"]').hide();
                    }
                    if (isShowPageLoading != false) {
                        if (validation != null && validation.form != null) {
                            PxPage.Loading(validation.form);
                        } else {
                            PxPage.Loading();
                        }
                    }
                    if (validation.iframe != undefined && validation.iframe != null) {
                        PxPage.log("OnFormSubmit: using iframe");
                        
                        //Wrap the validation success call in another function to make sure 
                        //that we close the block ui using the same id
                        var completed = function (response) {
                            if (validation && validation.form) {
                                validation.success(response);
                            }
                            if (isShowPageLoading != false) {
                                if (validation != null && validation.form != null) {
                                    PxPage.Loaded(validation.form);
                                } else {
                                    PxPage.Loaded();
                                }
                            }
                        }

                        PxPage.AjaxForm.submit(validation.form, { onComplete:
                            completed,
                            frameContainer: validation.iframe.frameContainer
                        });

                        $(validation.form).submit();

                        if (callbackFunc != undefined) {
                            callbackFunc();
                        }

                        PxPage.RemoveTinyMCE();

                        return true;
                    }
                    else {
                        var url = $(validation.form).attr("action");

                        PxPage.log("OnFormSubmit: { action: " + url + ", data: " + data + " }");

                        //if content can be created from the AssignTab
                        var assignTabContentCreate = false;
                        if ($('#hdnAssignTabContentCreate').length > 0) {
                            assignTabContentCreate = $.parseJSON($('#hdnAssignTabContentCreate').val());
                        }

                        $.post(url, data, function (response) {
                            //if there is an assign tab in content create window
                            if (isContentCreateAssign) {
                                var itemId = contentWrapper.find("#content-item-id").text();
                                var callback = null;

                                if (ContentWidget.IsAssignmentDateSelected(contentWrapper)) {
                                    ContentWidget.ContentAssigned('assign', itemId, callback, contentWrapper);
                                }
                            }

                            if (validation.success != null && typeof (validation.success) == "function") {
                                PxPage.RemoveTinyMCE();
                                validation.success(response);

                                if (callbackFunc != undefined) {
                                    callbackFunc();
                                }
                            }

                            if (isShowPageLoading != false) {
                                if (validation != null && validation.form != null) {
                                    PxPage.Loaded(validation.form);
                                } else {
                                    PxPage.Loaded();
                                }
                            }
                        });

                        return false;
                    }
                }
            }

            if ((isShowSubmit == null) || (isShowSubmit == false)) {
                $('input[type="submit"]').hide();
                $('.submit').hide();
            }

            if (isShowPageLoading) PxPage.Loading();
        },

        CloseCreateNewScreen: function (close) {
            if ($(".product-type-lms-faceplate").length) {
                $("#nonmodal .nonmodal-unblock-action").click();
                return false;
            }
            if ($(".product-type-faceplate, .product-type-xbook").length) {
                $("#nonmodal .nonmodal-unblock-action").click();
                return false;
            }
            $(PxPage.switchboard).trigger("removenewnode", close);
            PxPage.CloseNonModal();
            PxPage.TriggerHtmlSave();
            // as per new requirenment, we are not re-opening the create new screen after cancel.
            //$('li.open-templates').click();
            return false;
        },

        FneInitHooks: {},

        FneResizeHooks: {},

        FneCloseHooks: {},

        FneLoadedHooks: {}, // this is to add functionality when the fnewindow load is complete

        FneMinimizeHooks: {},

        ResizeHooksInUse: {},

        switchboard: '#main',

        log: function (message) {
            try {
                if (window.console && window.console.log) {
                    window.console.log(message);
                }
            }
            catch (ex) {
            }
        },

        // Returns word count of currently active mce editor.
        GetTinyMceWordCount: function () {
            var tx = tinyMCE.activeEditor.getContent({ format: 'raw' });
            var wordCount = 0;
            if (tx) {
                tx = tx.replace(/<.[^<>]*?>/g, ' ').replace(/&nbsp;|&#160;/gi, ' '); // remove html tags and space chars
                tx = tx.replace(/[0-9.(),;:!?%#$¿'"_+=\\\/-]*/g, ''); // remove numbers and punctuation
                tx.replace(/\S\s+/g, function () { wordCount++; }); // count the words
            }
            return wordCount;
        },

        openContent: function (args) {
            var url = args.url;
            var useIFrame = false;
            var useIFrameForExternal = false;
            var title = '';
            var minimize = false;
            var redirect = false;
            var useFne = false;

            if (args.title != undefined)
                title = args.title;

            if (args.useIFrame != undefined)
                useIFrame = args.useIFrame;

            if (args.minimize != undefined)
                minimize = args.minimize;
            if (args.id != undefined) {
                PxPage.log("loading fne via id");
                // adding 3 parameters for a new fne
                if (typeof HashHistory === undefined) {
                    url = PxPage.Routes.display_content + "?id=" + args.id + "&mode=Preview" + "&renderFne=true" + "&includeNavigation=true" + "&isBeingEdited=false";
                    args.loadFullFne = true;
                }
                else {
                    window.location.hash = 'state/item/' + args.id + "?mode=Preview" + "&renderFne=true" + "&includeNavigation=true" + "&isBeingEdited=false";
                }
            }
            else if (args.xUrl != undefined) {

                // hack to translate xUrls into agilix brainhoney resources.
                var disciplineId = $("#DisciplineId").val(),
                url = "/BrainHoney/Resource/" + disciplineId + "/" + args.xUrl;

                var urlTemp = url;

                $.ajax({
                    url: PxPage.Routes.get_mediaVaultUrl,
                    data: { urlToChange: urlTemp },
                    success: function (UrlWithMediaVaultHash) { url = UrlWithMediaVaultHash },
                    async: false
                });

                var showInFne = $('#fne-window').is(':visible');
                if (showInFne) {
                    PxPage.log("loading fne via xUrl");
                    useFne = true;
                }

                if (url != undefined) {
                    figureClicked = (url.indexOf('figures') > -1 || url.indexOf('tables') > -1);
                }

                if (!figureClicked) {
                    $(PxPage.switchboard).trigger("fnedonemode");
                }
            }
            else {
                PxPage.log("loading fne via url");
            }
            if (args.readonly != undefined && args.readonly == true) {
                url = url + (url.indexOf('?') == -1 ? '?' : '&') + 'readOnly=true';
            }
            if (args.includenavigation != undefined && args.includenavigation == false) {
                url = url + (url.indexOf('?') == -1 ? '?' : '&') + 'includeNavigation=false';
            }
            if (args.redirect != undefined)
                redirect = args.redirect;

            if (!redirect) {
                var figureClicked = false;
                var activitiesClicked = false;
                if (url != undefined) {
                    figureClicked = (url.indexOf('figures') > -1 || url.indexOf('tables') > -1);
                }
                if (url != undefined) {
                    activitiesClicked = (url.indexOf('/Activities/'));
                }
                if (useFne || args.useFne == true) {
                    if (args.xUrl != undefined) {
                        useIFrameForExternal = true;
                    }
                    PxPage.log("fne url: " + url);
                    if (figureClicked) {
                        window.open(url, '_newtab');
                    } else {
                        PxPage.UnBlockPrep();
                        _openFNE({ url: url, title: title, useLocal: args.useLocal, minimize: minimize, useIFrame: useIFrame, useIFrameForExternal: useIFrameForExternal, onFneLoaded: args.onContentLoaded });
                    }
                }
                else if (args.xUrl != undefined) {
                    _refreshView({ url: url, title: title, useLocal: args.useLocal, minimize: minimize, useIFrame: useIFrame, onFneLoaded: args.onContentLoaded });
                }
                else {
                    //_openFNEAction({ url: url, title: title, useLocal: args.useLocal, minimize: minimize, useIFrame: useIFrame, useIFrameForExternal: useIFrameForExternal, onFneLoaded: args.onContentLoaded });
                    PxPage.UnBlockPrep();
                    _openFNE({ url: url, title: title, useLocal: args.useLocal, minimize: minimize, useIFrame: useIFrame, useIFrameForExternal: useIFrameForExternal, onFneLoaded: args.onContentLoaded, loadFullFne: args.loadFullFne });
                }
            } else {
                url = 'AssignmentCenter?item=' + args.id;
                window.location = url;
            }
        },
        OpenFNE: function (args) {
            _openFNEAction(args);
        },
        OpenFNEWithHistory: function (args) {
            _openFNE(args);
        },
        TriggerFNELoaded: function (args) {
            _fneLoaded();
        },
        SetFneCreateCourseTitle: function () {
            var title = $('#courseFneTitle').val();
            PxPage.SetFneTitle(title);
        },

        SearchTermOperation: function (args) {
            $.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', null, 'search');
            
            $('.faceplate-browse-resources').find('#more-resources-search-box').val(args.searchString);
            $('.faceplate-browse-resources').find('#more-resources-search-box').parent().addClass('placeholder-changed');

            $.fn.FacePlateBrowseMoreResources('showMoreResourcesSearch', args.mode);
        },

        ValidateTitle: function (validateBlank, titleSelector) {
            var sel = 'input.title';
            if (titleSelector)
                sel = titleSelector;


            var value = $(sel).val();
            var title = jQuery.trim(value);
            if (title == '' || title == null) {
                PxPage.Toasts.Error('Title cannot be blank.');
                $(sel).focus();
                return false;
            }
            if (value.indexOf('<') != -1 ||
                    value.indexOf('>') != -1) {
                if (typeof (html_sanitize) !== 'undefined') {
                    try {
                        html_sanitize(value);
                    } catch (e) {
                        PxPage.Toasts.Error(e.message? e.message: 'Title cannot contain unsupported html');
                        return false;
                        
                    }
                    return true;


                }
                PxPage.Toasts.Error('Title cannot contain html tags.');
                $(sel).focus();
                return false;
            }

            var validTitleRegexp = /^[A-Za-z0-9:;? \-,.!@#$\~%^=_+&*()\[\]'"()]+$/;
            if (!validTitleRegexp.test($(sel).val())) {
                PxPage.Toasts.Error('Title cannot contain special characters.');
                $(sel).focus();
                return false;
            }
            return true;
        },

        ValidateUrl: function (sel) {
            var link = $(sel).val();

            if ($(sel).is(':visible')) {
                if (link == '') {
                    PxPage.Toasts.Error('Url cannot be blank.');
                    $(sel).focus();
                    return false;
                }

                var RegExp = /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&amp;'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i;
                if (!RegExp.test($(sel).val())) {
                    PxPage.Toasts.Error('Url provided is not valid.');
                    $(sel).focus();
                    return false;
                }
            }

            return true;
        },
        

        SetActiveHeaderMenu: function () {
            // binds last viewed item to content tab

            $('#PX_MENU_ITEM_CONTENT').click(function () {
                var lastItem = get_cookie('LastItem');

                if (lastItem.length > 0) {
                    var firstCut = lastItem.indexOf("/") + 1;

                    var lastItemArray = lastItem.substring(firstCut);
                    var redirPathTemp = '' + window.location.href;
                    var redirPath = redirPathTemp.split("/lms/");

                    if (redirPath.length == 2) {
                        var redirPathArray = redirPath[1].split("/");
                        if (lastItemArray[0] == redirPathArray[0]) { // ensures courseid matches
                            $('#PX_MENU_ITEM_CONTENT A').attr('href', redirPath[0] + "/lms" + lastItem);

                        }
                    }
                }
            });
        },

        PageReload: function () {
            var startLocation = window.location.pathname;
            var startExtract = startLocation.length;
            var lastItem = get_cookie('LastItem');
            var extractOldLocation = startLocation.substring(startLocation.lastIndexOf('/'), startExtract);
            var finalLocation = startLocation.substring(0, startLocation.indexOf(extractOldLocation)) + '/' + lastItem;
            window.location.replace(finalLocation);
        },

        TouchEnabled: function() {
            var device = navigator.userAgent.toLowerCase();
            var isMobileOs = device.match(/(iphone|ipod|ipad|android|webos|blackberry)/);
            return (Modernizr.touch && isMobileOs);
        },

        isIE: function () {
            // http://msdn.microsoft.com/en-us/library/ie/cc196988(v=vs.85).aspx
            return 'documentMode' in document;
        },
        
        equalizeWidth: _equalizeWidth,

        ScriptsLoaded: {},
        

        Require: function (dependencies, callback, dependenciesLoaded) {
            var afterLoadCallback = function () {
                for (dep in dependencies) {
                    PxPage.ScriptsLoaded[dependencies[dep]] = true;
                }
                if (callback) {
                    callback();
                }
            };
            for (dep in dependencies) {
                if (PxPage.ScriptsLoaded[dependencies[dep]] != true) {
                    $LAB.queueScript(dependencies[dep]);
                }
            }
            for (dep in dependenciesLoaded) {
                PxPage.ScriptsLoaded[dependenciesLoaded[dep]] = true;
            }
            
            try {
                $LAB.runQueue().wait(afterLoadCallback);
            }
            catch (err) {
                //added this error statement to deal with IE issues
                afterLoadCallback();
            }
        },

        SetBanners: function () {
            var hasBanner = false;
            $("div.banner").each(function (i, v) {
                //add the banner wrapper at the top of the body
                var bannerWrapper = "<div class='has-banner'></div>";
                $(bannerWrapper).prependTo("body");
                //append the banner content to the banner wrapper
                $('.has-banner').first().append($(v).show());
            });
        },

        InjectZoneHide: function () {
            var toggleLink = "<span class=\"hidetips\" style=\"float:right;\"><a class=\"InjectZoneHide showState\" onclick=\"return PxPage.ToggleTips();\" href=\"#\">HIDE SETUP TIPS</a><span class=\"arrow\"></span></span>";
            $("#PX_PRIMARY_EPORTFOLIO").append('<li style="float:right;">' + toggleLink + '<li>');
        },

        ToggleTips: function () {
            var toggleLink = $('.InjectZoneHide').html();

            if ($('.InjectZoneHide').hasClass('showState')) {
                $('.InjectZoneHide').removeClass('hiddenState');
                $('.InjectZoneHide').removeClass('showState');
                $('.InjectZoneHide').addClass('hiddenState');
                $('.InjectZoneHide').html('SETUP TIPS');
            }
            else {
                $('.InjectZoneHide').removeClass('hiddenState');
                $('.InjectZoneHide').removeClass('hiddenState');
                $('.InjectZoneHide').removeClass('showState');
                $('.InjectZoneHide').addClass('showState');
                $('.InjectZoneHide').html('HIDE SETUP TIPS');
            }

            $('.zoneIsSupportHide').toggle();
            return false;
        },

        ShowDeleteDashboardMessage: function (obj) {
            if ($(obj).prop("checked")) {
                PxPage.Toasts.Error("Please be advised that checking this option will delete existing data");
                // $("#DeleteExistingEportfolioCourse").val(true);
            }
            else {
                // $("#DeleteExistingEportfolioCourse").val(false);
            }
        },

        OnCancelFolderCreation: function () {
            var showModal = $('.showModal');
            if (confirm("Are you sure you want to exit without saving?")) {
                $(showModal).dialog('close');
                PxPage.CloseNonModal();
            }
        },
        doBGFade: function (elem, startRGB, endRGB, finalColor, steps, intervals, powr) {

            var easeInOut = function (minValue, maxValue, totalSteps, actualStep, powr) {
                var delta = maxValue - minValue;
                var stepp = minValue + (Math.pow(((1 / totalSteps) * actualStep), powr) * delta);
                return Math.ceil(stepp);
            };

            if (elem.bgFadeInt) window.clearInterval(elem.bgFadeInt);
            var actStep = 0;
            elem.bgFadeInt = window.setInterval(
                function () {
                    elem.css("backgroundColor", "rgb(" +
                        easeInOut(startRGB[0], endRGB[0], steps, actStep, powr) + "," +
                        easeInOut(startRGB[1], endRGB[1], steps, actStep, powr) + "," +
                        easeInOut(startRGB[2], endRGB[2], steps, actStep, powr) + ")"
                    );
                    actStep++;
                    if (actStep > steps) {
                        elem.css("backgroundColor", finalColor);
                        window.clearInterval(elem.bgFadeInt);
                    }
                }, intervals);

        },
        Fade: function () {
            var fadeItems = $(".fade-effect");
            var fadeTime = parseInt(fade.fadeOptions.highlightTime, 10);
            //$(fadeItems).effect("highlight", { color: fade.fadeOptions.highlightColor }, fadeTime);
            PxPage.doBGFade($(fadeItems), [245, 255, 159], [255, 255, 255], '', 75, 20, 4);
            $(fadeItems).removeClass("fade-effect");
        },
        ToggleContentSection: function (id, doc) {
            var section = jQuery(doc).find(id);
            if ($(section).is(":visible")) {
                newHeight = $("#document-body-iframe").height() + ($(section).height() + 10);
                $("#document-body-iframe").css("height", newHeight + "px");
            } else {
                newHeight = $("#document-body-iframe").height() - ($(section).height() + 10);
                $("#document-body-iframe").css("height", newHeight + "px");
            }
        },

        ResizeDialogBox: function (editorName, widthPercent, heightPercent) {
            var top = Math.max($(window).height() - editorName.outerHeight(), 0) / 2;
            var left = Math.max($(window).width() - editorName.outerWidth(), 0) / 2;

            editorName.css({
                top: top,
                left: left + $(window).scrollLeft()
            });

            if (widthPercent != undefined) {
                var newWidth = $(window).width() * (widthPercent / 100);
                editorName.css({
                    width: newWidth
                });
            }

            if (heightPercent != undefined) {
                var newHeight = $(window).width() * (heightPercent / 100);
                editorName.css({
                    height: newHeight
                });
            }
        }
    };
} (jQuery);

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
