//PxContentAreaWidget: plugin for the px content area widget
PxContentAreaWidget = function ($) {
    var _static = {
        initialized: false,
        quiz: undefined,
        settings: {
            loadTimeout: 5000,
            useRoute: true,
            dragParentSelector: '#PX_XbookContentWidget_XBook',
	    useSticky: true
        },
        events: {
            contentback: 'content-back',
            contentfwd: 'content-fwd',
            contentfs: 'content-fullscreen'
        },
        sel: {
            widget: '#xb-documentviewer',
            content: '#content-px',
            contentviewoptions: '#content-view-options',
            relatedcontent: '#eLibrary',
            contentarea: '#content-viewer',
            submitbutton: '#submission-submit',
            savebutton: '#submission-save',
            submissionstatus: '#submission-status',
            settingsbutton: '#settings'
        },
        footer: '',
        loadTimeout: {},
        footertypes: ['settings', 'submission'],
        fullscreen: false,
        optionMenuTimeout: 0,
        fn: {
            init: function (options) {
                _static.settings = $.extend({}, _static.settings, options);

                $('#content-back').click( function () {
                    $(PxPage.switchboard).trigger(_static.events.contentback);
                });
                $('#content-fwd').click( function () {
                    $(PxPage.switchboard).trigger(_static.events.contentfwd);
                });
                $('#content-fullscreen').click( function () {
                    $(PxPage.switchboard).trigger(_static.events.contentfs);
                    _static.fn.displayFullScreen();
                });

                //This is to initialize the fixed header functionality in xbook2
                //_static.settings.useSticky will be set by dlap
                if (_static.settings.useSticky === true) {
  		           $("#contentPlayerHeader").sticky({ topSpacing: 0 });
                }
                
                if (!_static.initialized) {
                    //Listen for creation of an html quiz
                    $(PxPage.switchboard).bind('htmlquiz-created', _static.fn.htmlquizCreated);

                    //Handle sizing of document veiwer iframe
                    $(PxPage.switchboard).bind("document-body-iframe-loaded", _static.fn.contentFrameLoad);

                    if (window.PxRelatedContent !== undefined) {
                        PxRelatedContent.Init(_static.settings.dragParentSelector);
                    }
                    
                    _static.initialized = true;
                }
            },
            htmlquizCreated: function (event, quiz) {
                /// <summary>
                /// Handles creating of an html quiz by setting the quiz property and displaying the footer
                /// </summary>

                _static.quiz = quiz;
                $(_static.quiz.el).bind('htmlquiz-saved', _static.fn.saveStatus)
                    .bind('htmlquiz-submit', _static.fn.submissionStatus)
                    .bind("htmlquiz-loaded", _static.fn.htmlQuizLoad);
                
                if (_static.quiz.displayFooter) {
                    _static.fn.viewFooter();
                } else {
                    _static.fn.hideFooter();
                }
            },
            htmlQuizLoad: function () {
                /// <summary>
                /// Handles the load of the html quiz
                /// </summary>

                var frames = _static.fn.getHtmlQuizIFrames();
                
                //if for whatever reason we can't select the html quiz iframes, just switch the buffers
                if (frames === undefined || frames.contentFrame[0].contentWindow.$ === undefined) {
                    PxPage.log('DocumentViewer: Cannot find htmlquiz iframes to size')
                    _static.fn.sizeFrame();
                    return;
                }

                //If the content is already visible, we don't need to wait for the event, otherwise wait for 
                //the digital first event to know when content is done loading/our timeout to expire.
                if (frames.contentFrame[0].contentWindow.$('div[data-type="section"]:visible').length > 0) {
                    _static.fn.sizeFrame([frames.contentFrame, frames.parentFrames]);

                } else {
                    frames.contentFrame[0].contentWindow.$(frames.contentFrame.contents()).bind('df-content-rendered', function () {
                        _static.fn.sizeFrame([frames.contentFrame, frames.parentFrames]);
                    });
                    //Only wait for the dig first event for a specific amount of time
                    _static.fn.setLoadTimeout([frames.contentFrame, frames.parentFrames]);
                }
            },
            contentFrameLoad: function (event, iframe) {
                /// <summary>
                /// Handles the load of digital first content
                /// </summary>
                /// <param name="frame" type="Object">Jquery object that holds the reference to the content iframe</param>

                _static.fn.hideFooter();

                var frame = undefined;
                if (iframe) {
                    frame = $(iframe);
                }

                if (frame !== undefined && frame.parents('#xb-documentviewer').length < 1) { //if the frame doesn't belong to the document viewer 
                    return;
                }
                var hostdiv = undefined;
                if (frame !== undefined) {
                    hostdiv = frame.closest('.document-viewer-frame-host');
                }

                //Under the following situations we just want to remove the load ui
                if (frame === undefined ||  //no iframe
                    hostdiv.attr('rel').indexOf('http') === 0 ||    //not in our domain
                    frame[0].contentWindow.$ === undefined ||   //no jquery
                    frame[0].contentWindow.$('div[data-type="section"]').length < 1) {  //not digital first content
                    _static.fn.sizeFrame();
                } else {
                    if (frame[0].contentWindow.$ === undefined ||  //Frame doesn't use jquery (it should be)
                        frame[0].contentWindow.$('div[data-type="section"]:visible').length > 0) { //frame is digital first but already rendered
                        _static.fn.sizeFrame([frame]);
                    } else {
                        //Digital first and hasn't completed rendering.
                        frame[0].contentWindow.$(frame.contents()).bind('df-content-rendered', function () {
                            _static.fn.sizeFrame([frame]);
                        });

                        //Only wait for the dig first event for a specific amount of time
                        _static.fn.setLoadTimeout([frame]);
                    }
                }
            },
            displayContentInFne: function(itemid, args) {
                // load content in an fne window
                PxPage.log('router : loading content to fne');
                
                //Kill document viewer so it doesn't conflict with any fne window stuff
                $('#xb-documentviewer #content-viewer').html('');
                
                //Remove the fne callback before adding to url params
                var fneOpenCallback = args.fneOpened;
                delete args.fneOpened;
                
                args.url = PxPage.Routes.display_content + '/' + itemid;
                args.url += $.format('?{0}', $.param(args));
                args.loadFullFne = true;
                
                if (PxPage.LargeFNE) {
                    PxPage.LargeFNE.OpenFNELink(args.url, args.title, false, null, true);
                }
            },
            displayContentInViewer: function (itemid) {
                /// <summary>
                /// Makes an ajax's call to get the content and display it in the main content area.
                /// </summary>

                PxPage.Loading(_static.sel.widget);

                //Get the content type
                $.ajax({
                    url: PxPage.Routes.get_contentviewer,
                    data: {
                        contentItemId: itemid
                    },
                    type: "GET",
                    success: function (response) {
                        //Destroy old htmlquiz if there was one
                        if (_static.quiz !== undefined) {
                            _static.quiz.fnDestroy();
                            _static.quiz = undefined;
                        }
                        
                        //To avoid seeing the loading window drop down to a small height, set the height
                        //of the new frame to be the height of the old frame.  It gets sized properly
                        //later.
                        //TODO: Height stuff not working.  Need to revisit
                        //var lastheight = $(_static.sel.widget + ' iframe').height();
                        $(_static.sel.content).html(response);
                        
                        //This should handle the creation of the iframe for non htmlquiz stuff
                        ContentWidget.ContentLoaded();
                        
                        //If non-iframe content, just remove the load screen
                        if ($(_static.sel.widget + ' iframe').length > 0) {
                            //if (lastheight !== undefined) {
                            //    $(_static.sel.widget + ' iframe').height(lastheight);
                            //}
                        } else {
                            _static.fn.sizeFrame();
                        }
                    }
                });
            },
            displayContent: function(itemid, args) {
                //Decide if we should render in the fne window or doc viewer
                if ((args.renderFNE && args.renderFNE.toLowerCase() === 'true') ||
                    (args.renderFne && args.renderFne.toLowerCase() === 'true')) {
                    _static.fn.displayContentInFne(itemid, args);
                } else {
                    _static.fn.displayContentInViewer(itemid);
                }
            },
            displayFullScreen: function() {
                /// <summary>
                /// Takes any content being displayed in the doc viewer and displays it in FNE 
                /// </summary>

                window.location.href = window.location.href.replace('FNE=False', 'FNE=True');
            },
            getHtmlQuizIFrames: function () {
                /// <summary>
                /// Returns the frame of the current HTML quiz
                /// </summary>

                //Don't bother checking for the quiz iframe if we don't have a reference to the quiz
                if (_static.quiz == undefined) {
                    return;
                }

                //We gotta do this nonsense to get to the iframe to levels in.  Also need to check if we are in an FNE window because they can both be open.
                //Level 1
                var htmlQuizPlayerFrame = $('#' + _static.quiz.el.attr('id') + ' .htmlquiz iframe');
                var htmlQuizPlayerContent = htmlQuizPlayerFrame.contents()[0];

                //Level 2
                var contentFrame = $('#document-body-iframe', htmlQuizPlayerContent);
                if (contentFrame.length == 0) {
                    return;
                } else {
                    return {
                        contentFrame: contentFrame,
                        parentFrames: htmlQuizPlayerFrame
                    }
                }
            },
            sizeFrame: function (iframes) {
                /// <summary>
                /// Calls autoheight on all iframes 
                /// </summary>
                /// <param name="iframes" type="Array">Array of jQuery objects that hold the iframes 
                /// to run autoheight against. </param>

                //If we set a timeout on the loading of content, clear it because we made it where we wanted to
                clearTimeout(_static.loadTimeout);

                //Set height on any iframes the content may be viewed in
                if (iframes != undefined) {
                    //Since we set the height of the new frame to be the height of the previous iframe, we need to 0
                    //it out in case the old frame is > than the new frame. (else autoheight won't make it smaller).
                    $(_static.sel.widget + ' iframe').height(0);
                    
                    for (var i = 0; i < iframes.length; i++) {
                        iframes[i].iframeAutoHeight({ heightOffset: 0 });
                    }
                }
                PxPage.Loaded(_static.sel.widget);
            },
            setLoadTimeout: function (frames) {
                /// <summary>
                /// Starts a timer for the content change to happen.
                /// </summary>
                /// <param name="frames" type="Object">The frames we need to set the height on</param>

                _static.loadTimeout = setTimeout(function () {
                    PxPage.log('DocumentViewer: Content load timer expired.')
                    _static.fn.sizeFrame(frames);
                }, _static.settings.loadTimeout);
            },
            saveStatus: function (event, message) {
                //TODO: Figure out how this should work.
                //$(_static.sel.submissionstatus).text(message);
            },
            viewFooter: function () {
                $('.contentwrapper, #content-viewer, #content-px, #content-buffer' + ($('#fne-window').length > 0 ? ', #fne-content, #content.content' : '')).addClass('footer-visible');
            },
            hideFooter: function () {
                $('.contentwrapper, #content-viewer, #content-px, #content-buffer' + ($('#fne-window').length > 0 ? ', #fne-content, #content.content' : '')).removeClass('footer-visible');
            },
            submissionStatus: function (event, submitResponse) {
                if (submitResponse.success) {
                    $(_static.sel.submissionstatus).text("Your responses have been submitted. To view results and feedback, click on the Grades tab and select the activity you just completed.");
                    $(_static.sel.submissionstatus).removeClass('submitfailure').addClass('submitsuccess');
                }
                else {
                    $(_static.sel.submissionstatus).text("Error occurred while submitting.");
                    $(_static.sel.submissionstatus).removeClass('submitsuccess').addClass('submitfailure');
                }
            },

            showOptionsMenu: function ($this) {
                if (!$("#fne-edit-menu").is(":visible")) {
                    $("#fne-edit-menu").fadeIn('fast');
                    $("#fne-edit-menu").position({
                        my: "left top",
                        at: "left bottom",
                        of: $this,
                        collision: "none"
                    });
                }
            },
            hideOptionsMenu: function () {
                _static.optionMenuTimeout = 0;
                $("#fne-edit-menu").fadeOut('fast');
                if ($(this)) {

                }
            }
        }
    };
    return {
        events: {
            ContentBack: _static.events.contentback,
            ContentForward: _static.events.contentfwd,
            ContentFullScreen: _static.events.contentfs
        },
        //Init: Initializes the content areas routes and sets up the triggers for the navigation controls.
        // dragParentSelector: used to set parent of the related content item when it is being dragged ( so that it exists outside its parent div).
        Init: function (options) {
            /// <summary>
            /// Initializes the content areas routes and sets up the triggers for the navigation controls.
            /// </summary>
            /// <param name="options" type="Object">Object that holds configuration options for the widget:
            /// - dragParentSelector:  used to set parent of the related content item when it is being dragged
            /// - loadTimeout: milliseconds to wait for content to complete rendering before switching buffer
            /// - useRoute (bool): set to true to setup the widgets hash route
            /// </param>

            _static.fn.init(options);
        },
        DisplayContent: function (itemid, args) {
            _static.fn.displayContent(itemid, args);
        },
        DisplayFooter: function (footerType) {
            //Set which footer type to display.  Options are ['settings', 'submission', 'none', ''(same as none)]
            _static.footer = footerType || '';
        },
        PrivateStatic: function () {
            /// <summary>For testing purposes only</summary>

            return _static;
        }
    };
}(jQuery);