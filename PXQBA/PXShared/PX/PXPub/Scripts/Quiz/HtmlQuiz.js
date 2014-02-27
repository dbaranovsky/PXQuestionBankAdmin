(function ($) {
    var defaults = {
        displayFooter: false, //true if we should display the submit/save controls
        initRpc: true,
        bhPlayer: false //true if we are rending using the bh activity player
    };
    var _errorMessage = 'Error';
    var fn = {
        initQuiz: function (options) {
            var quiz = $(this);
            var rpc = {
                destroy: function() {
                }
            };
            if (quiz.length === 0) {
                return;
            }

            PxPage.log('initializing quiz: ' + quiz.find('.htmlquiz').attr('rel'));
            $.extend(defaults, options);
            if (defaults.initRpc && !defaults.bhPlayer) {
                rpc = fn.initRpc(quiz);
            }

            var retval = {
                el: quiz,
                rpc: rpc,
                quizFrame: {},
                wrapperFrame: {},
                contentFrame: {},
                displayFooter: defaults.displayFooter,
                fnHide: function () {
                    this.el.hide();
                    this.fnHideSubmissionFooter();
                },
                fnShow: function () {
                    this.el.show();
                    if (this.displayFooter) {
                        this.fnShowSubmissionFooter();
                    }
                },
                fnShowMask: function () {
                    this.el.block({
                        message: null,
                        blockMsgClass: 'htmlquizBlock',
                        overlayCSS: {
                            opacity: 1.0
                        }
                    });
                },
                fnShowSubmissionFooter: function () {
                    //Because right now we are using the default buttons 
                    if (defaults.bhPlayer) {
                        if (this.contentFrame !== undefined) {
                            var footer = this.contentFrame[0].contentWindow.$('div[id*="assessmentframefooterrow_"]');
                            footer.show();
                        }
                    } else {
                        this.el.find('#submission-controls').show();
                        if (this.quizFrame.length) {
                            var iframe = this.quizFrame.contents()[0];
                            $('#allcontainers', iframe).addClass('footer-visible');
                        }
                    }
                },
                fnHideSubmissionFooter: function () {
                    //Because right now we are using the default buttons 
                    if (defaults.bhPlayer) {
                        if (this.contentFrame !== undefined) {
                            var footer = this.contentFrame[0].contentWindow.$('div[id*="assessmentframefooterrow_"]');
                            footer.hide();
                        }
                    } else {
                        this.el.find('#submission-controls').hide();
                        if (this.quizFrame.length) {
                            var iframe = this.quizFrame.contents()[0];
                            $('#allcontainers', iframe).removeClass('footer-visible');
                        }
                    }
                },
                fnSaveQuiz: function () {
                    var content = this.contentFrame.contents()[0];
                    var saveButton = $('button#htmlquizsavebutton', content)[0];

                    saveButton.click();
                },
                //initiate a click on the submit button within the htmlquiz iframe.
                fnSubmitQuiz: function () {
                    var content = this.contentFrame.contents();

                    // scroll to the top of the page when clicking submit to see modal window
                    $('#content-viewer, html, body, #fne-content').animate({ scrollTop: 0 }, 500);
                    $('#htmlquizbutton', content).click();
                    
                    //We need this for IE10 because for whatever reason the coffee script isn't doing this.
                    $('#submitModal', content).addClass('in').show();
                },
                fnQuizDisplay: function () {
                    var frames = defaults.bhPlayer ? [this.contentFrame, this.wrapperFrame, this.quizFrame] : [this.contentFrame, this.quizFrame]; // innermost frame must be first
                    $.each(frames, function () {
                        this.iframeAutoHeight();
                    });
                    
                    //For now hide the save/submit for bh htmlquiz 
                    if (retval.displayFooter) {
                        retval.fnShowSubmissionFooter();
                    } else {
                        retval.fnHideSubmissionFooter();
                    }
                    
                    this.el.unblock();
                },
                fnQuizReady: function () {
                    //BH play has 3 iframes instead of two, and different frame id's so we need this
                    if (defaults.bhPlayer) {
                        this.quizFrame = $('.bh-component iframe', this.el);
                        this.wrapperFrame = this.quizFrame.eq(0).contents().find('#contentBody_FrameContent');
                        this.contentFrame = this.wrapperFrame.eq(0).contents().find('iframe');

                        //Wait for the rendering to complete, and fix all the css on the BH components
                        this.contentFrame[0].contentWindow.$(this.contentFrame.contents()).bind('df-content-rendered', function (event) {
                            //Content frame style fixes
                            retval.contentFrame[0].contentWindow.$('.x-panel-body, .x-panel-bwrap').height('100%').css('overflow', 'visible');
                            
                            //Wrapper frame style fixes
                            var wdiv = retval.wrapperFrame[0].contentWindow.document.getElementsByClassName('content-iframe-container');
                            for (var w = 0; w < wdiv.length; w++) {
                                wdiv[w].style.overflow = 'visible';
                            }

                            //Quiz frame style fixes
                            //Fix bad heights/overflow
                            var cframe = retval.quizFrame[0].contentWindow.document.getElementById('content_FrameContent');
                            cframe.style.height = '100%';
                            var noheadDivs = retval.quizFrame[0].contentWindow.document.getElementsByClassName('x-panel-body-noheader');
                            for (var i = 0; i < noheadDivs.length; i++) {
                                noheadDivs[i].style.height = '100%';
                                noheadDivs[i].style.overflow = 'visible';
                            }
                            
                            //Fix bad overflow
                            var bwrapDivs = retval.quizFrame[0].contentWindow.document.getElementsByClassName('x-panel-bwrap');
                            for (var j = 0; j < bwrapDivs.length; j++) {
                                bwrapDivs[j].style.overflow = 'visible';
                            }
                            
                            retval.fnQuizDisplay.call(retval);
                       });

                    } else {
                        this.quizFrame = $('#htmlquiz-frame', this.el);
                        this.contentFrame = this.quizFrame.eq(0).contents().find('#document-body-iframe');

                        //If the content is already visible just remove the mask, else wait until rendering is complete.
                        if (this.contentFrame[0].contentWindow.$ === undefined ||
                            this.contentFrame[0].contentWindow.$('div[data-type="section"]:visible').length > 0) {
                            this.fnQuizDisplay();
                        } else {
                            this.contentFrame[0].contentWindow.$(this.contentFrame.contents()).bind('df-content-rendered', { quiz: this }, function (event) {
                                var quiz = event.data.quiz;
                                quiz.fnQuizDisplay.call(quiz);
                            });
                        }
                    }
                },
                fnDestroy: function () {
                    if (this.rpc !== undefined) {
                        this.rpc.destroy();
                    }
                    this.contentFrame = {};
                    this.quizFrame = {};
                    this.el = undefined;
                    $(PxPage.switchboard).unbind('.htmlquiz');
                }
            };

            //Handler for a click of the submission link from the previous attempts view
            quiz.find(".submission-link").click(function () {
                retval.el.trigger("fnedonemode");
            });
            
            //Setup click handlers for save and submit
            quiz.find('#submission-submit').click(function() {
                retval.fnSubmitQuiz();
            });
            quiz.find('#submission-save').click(function() {
                retval.fnSaveQuiz();
            });
            quiz.bind('htmlquiz-submit', function (event, status) {
                $(PxPage.switchboard).trigger('htmlquiz-submit-complete', [status]);
            });
            quiz.bind('htmlquiz-loaded', function() {
                retval.fnQuizReady();
            });
            
            //We are using the bh activity player so we need to bind to the frameAPI events for load, submit and saving
            if (defaults.bhPlayer) {
                $(PxPage.switchboard).bind('bh-componentactive.htmlquiz', function () {
                    if (window.ContentWidget && $(".htmlquiz-content.allowComments").length) {
                        ContentWidget.InitCommenting($('.bh-component iframe', quiz.find('#htmlquiz-frame').contents()[0])[0], '.htmlquiz-content', '.htmlquiz');
                    }
                    retval.el.trigger('htmlquiz-loaded');
                });
                $(PxPage.switchboard).bind('componentsubmit.htmlquiz', function(event, componentType, id, state, frame) {
                    retval.el.trigger('htmlquiz-submit', ['success']);
                });
                
                $(PxPage.switchboard).bind('componentsaved.htmlquiz', function (event, componentType, id, state) {
                    retval.el.trigger('htmlquiz-save', ['success']);
                });
            }

            retval.fnShowMask();
            return retval;
        },
        //Initializes the easyXDM rpc to make calls to the htmlquiz player
        initRpc: function (quiz) {
            return new easyXDM.Rpc({
                remote: quiz.find('.htmlquiz').attr('rel'),
                onReady: function () {
                    PxPage.log('MainContentArea is ready for communication');
                },
                container: quiz.find('.htmlquiz').attr('id'),
                props: {
                    id: "htmlquiz-frame",
                    scrolling: "no"
                }
            }, {
                local: {
                    //For when the frame has competed loading
                    MainContentArea_Loaded: function () {
                        if (window.ContentWidget && $(".htmlquiz-content.allowComments").length) {
                            ContentWidget.InitCommenting($('#document-body-iframe', quiz.find('#htmlquiz-frame').contents()[0])[0], '.htmlquiz-content', '.htmlquiz');
                        }
                        $(quiz).trigger('htmlquiz-loaded');
                    },
                    //Lets us know the status of a save attempt on the HTML Quiz
                    SaveHtmlQuizStatus: function (status) {
                        $(quiz).trigger('htmlquiz-saved', [status]);
                    },

                    SubmitHtmlQuizStatus: function (status) {
                        $(quiz).trigger('htmlquiz-submit', [status]);
                    }

                },
                remote: {
                    UpdateContent: function (itemId, fragmentId) { }
                }
            });
        }
    };

    $.fn.HtmlQuiz = function () {
        return fn.initQuiz.apply(this, arguments);
    };
}(jQuery));