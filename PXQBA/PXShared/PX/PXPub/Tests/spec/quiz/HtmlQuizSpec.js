var frameLoaded = false,
    contentFrame = {},
    contentFrameWindow = {},
    quiz = {};

var frameLoad = function() {
    frameLoaded = true;
    contentFrame = $('iframe', quiz.el).eq(0).contents().find('#document-body-iframe');
    contentFrameWindow = contentFrame[0].contentWindow;
};

var newframeLoad = function() {
    frameLoaded = true;
};

describe('HTML Quiz:', function () {
    PxPage = {
        switchboard: $(document),
        log: function() {
        }
    }
    describe('with old player will', function() {        
        beforeEach(function() {
            var model = {
                viewPath: 'HtmlQuiz',
                viewModel: JSON.stringify(HtmlQuiz.withQuestions.withSubmissions),
                viewModelType: 'Bfw.PX.PXPub.Models.HtmlQuiz, Bfw.PX.PXPub.Models'
            };
            var view = PxViewRender.RenderView('PXPub', 'HtmlQuiz', model);
            setFixtures(view);
            var options = {
                displayFooter: true,
                initRpc: false
            }
            quiz = $('.htmlquiz-content').HtmlQuiz(options);

            // mock quiz iframe created through easyXDM so `fnQuizReady` can run without error
            $('<iframe id="htmlquiz-frame" src="http://lcl.whfreeman.com/tests/fixtures/html/htmlquizcontainer.html" onload="frameLoad()"></iframe>').appendTo(quiz.el);
        });

        afterEach(function() {
            frameLoaded = false;
            quiz = {};
            contentFrame = {};
            contentFrameWindow = {};
        });
        
        describe('on load', function () {
            it('set the contentFrame and quizFrame', function() {
                waitsFor(function() {
                    return frameLoaded;
                }, "Frame didn't load", 10000);

                runs(function() {
                    contentFrameWindow.$('div[data-type="section"]').show();
                    $(quiz.el).trigger('htmlquiz-loaded');

                    expect(quiz.quizFrame.length).not.toBe(undefined);
                    expect(quiz.contentFrame.length).not.toBe(undefined);
                });
            });
            
            it('subscribe to digital first event and call `fnQuizDisplay`', function() {

                spyOn(quiz, 'fnQuizDisplay').andCallThrough();

                waitsFor(function() {
                    return frameLoaded;
                }, "Frame didn't load", 10000);

                runs(function() {
                    $(quiz.el).trigger('htmlquiz-loaded');

                    contentFrameWindow.$(contentFrame.contents()).trigger('df-content-rendered');
                    expect(quiz.fnQuizDisplay).toHaveBeenCalled();
                });
            });

            it('call `fnQuizDisplay` without digital first event if already rendered', function() {
                                waitsFor(function() {
                    return frameLoaded;
                }, "Frame didn't load", 10000);

                runs(function() {
                    spyOn(quiz, 'fnQuizDisplay').andCallThrough();
                    contentFrameWindow.$('div[data-type="section"]').show();
                    $(quiz.el).trigger('htmlquiz-loaded');
                    expect(quiz.fnQuizDisplay).toHaveBeenCalled();
                });
            });
        });

        describe('handle a submit button click', function () {
            beforeEach(function() {
                waitsFor(function() {
                    return frameLoaded;
                }, "Frame didn't load", 10000);

                runs(function() {
                    contentFrameWindow.$('div[data-type="section"]').show();
                    $(quiz.el).trigger('htmlquiz-loaded');
                });
            });
            it('and click the submit button inside the iframe', function () {
                var result = false;

                runs(function() {
                    contentFrameWindow.$(contentFrame.contents()).bind('click', function() {
                        result = true;
                    });
                    quiz.fnSubmitQuiz();
                    expect(result).toBe(true);
                });
            });

            it('and display the confirmation modal inside the iframe', function () {
                runs(function() {
                    quiz.fnSubmitQuiz();
                    expect($('#submitModal', contentFrame.contents()).hasClass('in')).toBe(true);
                });
            });
        });

    });

    describe('with brain honey player will', function() {
        beforeEach(function() {
            var model = {
                viewPath: 'HtmlQuiz',
                viewModel: JSON.stringify(NewHtmlQuiz.withQuestions.withSubmissions),
                viewModelType: 'Bfw.PX.PXPub.Models.HtmlQuiz, Bfw.PX.PXPub.Models'
            };
            
            //Not setting the enrollmentid on the model will avoid rendering the bhiframecomponent view which makes this whole thing work so don't set it
            var view = PxViewRender.RenderView('PXPub', 'HtmlQuiz', model);
            setFixtures(view);
            var options = {
                displayFooter: true,
                initRpc: false,
                bhPlayer: true //use brainhoney player
            }
            quiz = $('.htmlquiz-content').HtmlQuiz(options);

            // mock quiz iframe created through easyXDM so `fnQuizReady` can run without error
            $('<div class="bh-component-wrapper"><div class="bh-component"><iframe id="htmlquiz-frame" src="http://lcl.whfreeman.com/tests/fixtures/html/bhhtmlquizcontainer.html" onload="newframeLoad()"></iframe></div></div>').appendTo(quiz.el);
        });

        afterEach(function() {
            frameLoaded = false;
            quiz = {};
        });

        //Test load process for html quiz in brain honey player
        describe('on load', function() {
            it('set the quizFrame, wrapperFrame and contentFrame', function() {
                waitsFor(function () {
                    return frameLoaded;
                }, "Frame didn't load", 10000);

                runs(function() {
                    $(quiz.el).trigger('htmlquiz-loaded');

                    expect(quiz.quizFrame.length).not.toBe(undefined);
                    expect(quiz.wrapperFrame.length).not.toBe(undefined);
                    expect(quiz.contentFrame.length).not.toBe(undefined);
                });
            });
            
            it('subscribe to digital first event and call `fnQuizDisplay`', function () {

                spyOn(quiz, 'fnQuizDisplay').andCallThrough();

                waitsFor(function () {
                    return frameLoaded;
                }, "Frame didn't load", 10000);

                runs(function () {
                    $(quiz.el).trigger('htmlquiz-loaded');
                    quiz.contentFrame[0].contentWindow.$(quiz.contentFrame[0].contentWindow.document).trigger('df-content-rendered');
                    
                    expect(quiz.fnQuizDisplay).toHaveBeenCalled();
                });
            });

            it('set content frame divs to a height of 100% and overflow to visible', function() {
                waitsFor(function () {
                    return frameLoaded;
                }, "Frame didn't load", 10000);

                runs(function () {
                    $(quiz.el).trigger('htmlquiz-loaded');
                    quiz.contentFrame[0].contentWindow.$(quiz.contentFrame.contents()).trigger('df-content-rendered');
                    
                    expect(quiz.contentFrame[0].contentWindow.$('.x-panel-body')[0].style.height).toBe('100%');
                    expect(quiz.contentFrame[0].contentWindow.$('.x-panel-bwrap')[0].style.height).toBe('100%');
                    expect(quiz.contentFrame[0].contentWindow.$('.x-panel-body')[0].style.overflow).toBe('visible');
                    expect(quiz.contentFrame[0].contentWindow.$('.x-panel-bwrap')[0].style.overflow).toBe('visible');
                });
            });
            
            it('set wrapper frame divs overflow to visible', function () {
                waitsFor(function () {
                    return frameLoaded;
                }, "Frame didn't load", 10000);

                runs(function () {
                    $(quiz.el).trigger('htmlquiz-loaded');
                    quiz.contentFrame[0].contentWindow.$(quiz.contentFrame.contents()).trigger('df-content-rendered');
                    
                    var wdiv = quiz.wrapperFrame[0].contentWindow.document.getElementsByClassName('content-iframe-container');
                    for (var w = 0; w < wdiv.length; w++) {
                        expect(wdiv[w].style.overflow).toBe('visible');
                    }
                });
            });
            
            it('set quiz frame content_FrameContent height to 100%', function () {
                waitsFor(function () {
                    return frameLoaded;
                }, "Frame didn't load", 10000);

                runs(function () {
                    $(quiz.el).trigger('htmlquiz-loaded');
                    quiz.contentFrame[0].contentWindow.$(quiz.contentFrame.contents()).trigger('df-content-rendered');
                    
                    var divFrame = quiz.quizFrame[0].contentWindow.document.getElementById('content_FrameContent');
                    expect(divFrame.style.height).toBe('100%');
                });
            });
            
            it('set quiz frame x-panel-body-noheader divs to have height of 100% and overflow of visible', function () {
                waitsFor(function () {
                    return frameLoaded;
                }, "Frame didn't load", 10000);

                runs(function () {
                    $(quiz.el).trigger('htmlquiz-loaded');
                    quiz.contentFrame[0].contentWindow.$(quiz.contentFrame.contents()).trigger('df-content-rendered');
                    
                    var divs = quiz.wrapperFrame[0].contentWindow.document.getElementsByClassName('x-panel-body-noheader');
                    for (var w = 0; w < divs.length; w++) {
                        expect(divs[w].style.height).toBe('100%');
                        expect(divs[w].style.overflow).toBe('visible');
                    }
                });
            });
            
            it('set quiz frame x-panel-bwrap divs overflow to be visible', function () {
                waitsFor(function () {
                    return frameLoaded;
                }, "Frame didn't load", 10000);

                runs(function () {
                    $(quiz.el).trigger('htmlquiz-loaded');
                    quiz.contentFrame[0].contentWindow.$(quiz.contentFrame.contents()).trigger('df-content-rendered');

                    var divs = quiz.wrapperFrame[0].contentWindow.document.getElementsByClassName('x-panel-bwrap');
                    for (var w = 0; w < divs.length; w++) {
                        expect(divs[w].style.overflow).toBe('visible');
                    }
                });
            });
        });
    });
    describe("rendering", function() {
        var ui = {
            history: '#submission-history',
            quiz: '.htmlquiz-content'
        };
        describe("as student", function() {
            it('displays submission history when attempts are present', function() {
                var model = {
                    viewPath: 'HtmlQuiz',
                    viewModel: JSON.stringify(HtmlQuiz.withQuestions.withSubmissions),
                    viewModelType: 'Bfw.PX.PXPub.Models.HtmlQuiz, Bfw.PX.PXPub.Models'
                };
                var view = PxViewRender.RenderView('PXPub', 'HtmlQuiz', model);
                setFixtures(view);

                expect($(ui.history).length).toBe(1);
            });
            it('doesn\'t dislpay submission history when there are no attempts', function() {
                var model = {
                    viewPath: 'HtmlQuiz',
                    viewModel: JSON.stringify(HtmlQuiz.withQuestions.withoutSubmissions),
                    viewModelType: 'Bfw.PX.PXPub.Models.HtmlQuiz, Bfw.PX.PXPub.Models'
                };
                var view = PxViewRender.RenderView('PXPub', 'HtmlQuiz', model);
                setFixtures(view);

                expect($(ui.history).length).toBe(0);
            });
            it('dislpays comment block when comments are allowed', function () {
                var model = {
                    viewPath: 'HtmlQuiz',
                    viewModel: JSON.stringify(HtmlQuiz.withQuestions.withSubmissions),
                    viewModelType: 'Bfw.PX.PXPub.Models.HtmlQuiz, Bfw.PX.PXPub.Models'
                };
                var view = PxViewRender.RenderView('PXPub', 'HtmlQuiz', model);
                setFixtures(view);

                expect($('#highlight-container').length).toBe(1);
                expect($('div.allowComments').length).toBe(1);
            });
            it('doesn\'t dislpays comment block when comments are allowed', function () {
                HtmlQuiz.withQuestions.withSubmissions.AllowComments = false;
                var model = {
                    viewPath: 'HtmlQuiz',
                    viewModel: JSON.stringify(HtmlQuiz.withQuestions.withSubmissions),
                    viewModelType: 'Bfw.PX.PXPub.Models.HtmlQuiz, Bfw.PX.PXPub.Models'
                };
                var view = PxViewRender.RenderView('PXPub', 'HtmlQuiz', model);
                setFixtures(view);

                expect($('#highlight-container').length).toBe(0);
                expect($('div.allowComments').length).toBe(0);
            });
        });
        describe("as instructor", function() {
            beforeEach(function() {
                var viewModel = HtmlQuiz.withQuestions.withSubmissions;
                viewModel.IsInstructor = true;

                var model = {
                    viewPath: 'HtmlQuiz',
                    viewModel: JSON.stringify(viewModel),
                    viewModelType: 'Bfw.PX.PXPub.Models.HtmlQuiz, Bfw.PX.PXPub.Models'
                };
                var view = PxViewRender.RenderView('PXPub', 'HtmlQuiz', model);
                setFixtures(view);
            });
            it('doesn\'t display submission history', function() {
                expect($(ui.history).length).toBe(0);
            })
            it('displays parentid in a hidden input', function() {
                expect($(ui.quiz + ' input#BHParentId').val()).toBe('PX_MANIFEST');
            });
            it('displays isgradable in a hidden input', function () {
                expect($(ui.quiz + ' input#IsGradable').val()).toBe('True');
            });
            it('displays maxpoints in a hidden input', function () {
                expect($(ui.quiz + ' input#MaxPoints').val()).toBe('10');
            });
            it('displays overrideduedatereq in a hidden input', function () {
                expect($(ui.quiz + ' input#OverrideDueDateReq').val()).toBe('True');
            });
        });
    });
});

HtmlQuiz = {
    withQuestions: {
        withSubmissions: {
            Submissions: [{
                Name: 'Sub1',
                QuizId: 'Quiz1',
                QuestionId: 'Question1',
                EnrollmentId: '12345',
                Score: 28.5
            }],
            Questions: [{
                ItemId: 'Question1',
                EnrollmentId: '12345'
            }],
            IsInstructor: false,
            OverrideDueDateReq: true,
            MaxPoints: 10,
            BHParentId: 'PX_MANIFEST',
            IsGradable: true,
            AllowComments: true,
            CourseInfo: {
                CourseType: 15 //XbookV2
            },
            XBookAppParams: {
                ComponentName: 'MainContentArea',
                BrainHoneyUrl: 'http://pxmigration.dev.brainhoney.bfwpub.com/brainhoney',
                AgilixUrl: '/xbookapp'
            }
        },
        withoutSubmissions: {
            Submissions: [],
            Questions: [{
                ItemId: 'Question1',
                EnrollmentId: '12345'
            }],
            IsInstructor: false,
            OverrideDueDateReq: true,
            IsGradable: true,
            MaxPoints: 10,
            BHParentId: 'PX_MANIFEST'
        }
    }
};

NewHtmlQuiz = {
    withQuestions: {
        withSubmissions: {
            Submissions: [{
                Name: 'Sub1',
                QuizId: 'Quiz1',
                QuestionId: 'Question1',
                EnrollmentId: '12345',
                Score: 28.5
            }],
            Questions: [{
                ItemId: 'Question1',
                EnrollmentId: '12345'
            }],
            IsInstructor: false,
            OverrideDueDateReq: true,
            MaxPoints: 10,
            BHParentId: 'PX_MANIFEST',
            IsGradable: true,
            AllowComments: true,
            CourseInfo: {
                CourseType: 15 //XbookV2
            },
            BHHtmlQuiz: true
        },
        withoutSubmissions: {
            Submissions: [],
            Questions: [{
                ItemId: 'Question1',
                EnrollmentId: '12345'
            }],
            IsInstructor: false,
            OverrideDueDateReq: true,
            IsGradable: true,
            MaxPoints: 10,
            BHParentId: 'PX_MANIFEST',
            BHHtmlQuiz: true
        }
    }
};
