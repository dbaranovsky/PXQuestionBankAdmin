var loaded = false;
var dfFrameLoad = function () {
    loaded = true;
    $(PxPage.switchboard).trigger('document-body-iframe-loaded');
};

describe('Content Area Widget (XB Document Viewer):', function () {

    beforeEach(function() {
        //Setup pxpage
        window.PxPage = {
            switchboard: $(document),
            Loading: function (args) { },
            Loaded: function (args) { },
            log: function(args) { },
            OnReady: function (callback) {
                callback();
            },
            Require: function (dependencies, callback) {
                callback();
            },
            Routes: {
                get_contentviewer: 'http://testurl.com'
            }
        };

        spyOn($.fn, "sticky");
        
        window.ContentWidget = {
            ContentLoaded: function() {
                
            }
        }
        
        spyOn(PxContentAreaWidget.PrivateStatic().fn, 'htmlquizCreated').andCallThrough();
        PxContentAreaWidget.Init();
        
        //defaults to 1 second so make little less for testing purposes
        PxContentAreaWidget.PrivateStatic().settings.loadTimeout = 1000;
    });

    afterEach(function() {
        PxContentAreaWidget.PrivateStatic().initialized = false;
    });
    describe('initialization', function () {
        it('sets initialized to true', function() {
            expect(PxContentAreaWidget.PrivateStatic().initialized).toBe(true);
        });
    });

    xdescribe('displaying content', function () {
        //Note: I'm pretty sure this isn't working because of jquery 1.9 so im going to disable for now
        var displayRequest = undefined;
        var successResponse = {
            status: 200,
            responseText: '<div id="woo"> Testing no iframe </div>'
        };
        
        beforeEach(function() {
            jasmine.Ajax.useMock();
            PxContentAreaWidget.PrivateStatic().fn.displayContent('abcd-123');
            displayRequest = mostRecentAjaxRequest();
        });

        describe('without iframes', function() {
            beforeEach(function() {
                spyOn(PxContentAreaWidget.PrivateStatic().fn, 'sizeFrame');
                displayRequest.response(successResponse);
            });

            it('calls sizeframe for content without iframes', function() {
                //We expect sizeframe to be called immediately for content without iframes to remove
                //the load screen
                expect(PxContentAreaWidget.PrivateStatic().fn.sizeFrame).toHaveBeenCalled();
            });
        });
    });

    describe('setting iframe load timeout', function() {
        it('calls sizeFrame after the specified timeout', function() {
            var done = false;
            spyOn(PxContentAreaWidget.PrivateStatic().fn, 'sizeFrame');
            
            PxContentAreaWidget.PrivateStatic().fn.setLoadTimeout();
            setTimeout(function() {
                done = true
            }, PxContentAreaWidget.PrivateStatic().settings.loadTimeout + 1000);
            waitsFor(function() {
                return done;
            }, PxContentAreaWidget.PrivateStatic().settings.loadTimeout + 1000);

            runs(function() {
                expect(PxContentAreaWidget.PrivateStatic().fn.sizeFrame).toHaveBeenCalled();
            });
        });
    });

    describe('handling iframe load events', function() {
        it('calls sizeFrame if it can\'t get the regular content iframe', function() {
            spyOn(PxContentAreaWidget.PrivateStatic().fn, 'sizeFrame');

            PxContentAreaWidget.PrivateStatic().fn.contentFrameLoad();

            expect(PxContentAreaWidget.PrivateStatic().fn.sizeFrame).toHaveBeenCalled();
        });
        it('calls sizeFrame if it can\'t get the htmlquiz iframes', function () {
            spyOn(PxContentAreaWidget.PrivateStatic().fn, 'sizeFrame');
            
            PxContentAreaWidget.PrivateStatic().fn.htmlQuizLoad();

            expect(PxContentAreaWidget.PrivateStatic().fn.sizeFrame).toHaveBeenCalled();
        });
    });

    describe('handling header button clicks', function () {
        var fixture = '';
        beforeEach(function () {
            if (fixture === '') {
                //Fixture stuff
                var viewPath = "ContentArea";
                var viewModelType = "Bfw.PX.PXPub.Models.ContentArea, Bfw.PX.PXPub.Models";

                var item = {
                    Id: 'itemId'
                }
                var viewModel = JSON.stringify({
                    Content: item
                });
                var data = {
                    viewPath: viewPath,
                    viewModel: viewModel,
                    viewModelType: viewModelType
                };

                fixture = PxViewRender.RenderView('PXPub', 'ContentAreaWidget', data);
            }
            
            setFixtures(fixture);
            PxContentAreaWidget.Init();
        });
        afterEach(function () {
            //We can reuse the fixture instead of calling testcontroller multiple times
            fixture = $("#jasmine-fixtures").html();
        });
        it('will trigger content-back event for a back button click on switchboard', function () {
            var result = false;
            $(PxPage.switchboard).bind('content-back', function() {
                result = true;
            });

            $('#content-back').click();
            expect(result).toEqual(true);
        });
        it('will trigger content-fwd event for a forward button click on switchboard', function () {
            var result = false;
            $(PxPage.switchboard).bind('content-fwd', function() {
                result = true;
            });
            $('#content-fwd').click();
            expect(result).toEqual(true);
        });
        it('will trigger content-fullscreen event for a fullscreen button click on switchboard', function () {
            var result = false;
            spyOn(PxContentAreaWidget.PrivateStatic().fn, 'displayFullScreen');
            $(PxPage.switchboard).bind('content-fullscreen', function() {
                result = true;
            });
            $('#content-fullscreen').click();
            expect(result).toEqual(true);
        });
        it('will call displayFullScreen on a fullscreen click', function () {
            spyOn(PxContentAreaWidget.PrivateStatic().fn, 'displayFullScreen');
            $('#content-fullscreen').click();

            expect(PxContentAreaWidget.PrivateStatic().fn.displayFullScreen).toHaveBeenCalled();
        });
        it('will update renderFne to True on a fullscreen click', function () {
            window.location.hash = 'FNE=False';
            $('#content-fullscreen').click();
            expect(window.location.hash.indexOf('FNE=True')).toBeGreaterThan(-1);
        });
    });

    describe('handling non digitial first iframe content', function () {
        beforeEach(function() {
            var model = {
                Content: {
                    Title: 'Test Content',
                    Id: "Id"
                }
            };

            var data = {
                viewPath: 'Summary',
                viewModel: JSON.stringify(model),
                viewModelType: 'Bfw.PX.PXPub.Models.ContentArea'
            };
            var view = PxViewRender.RenderView('PXPub', 'ContentAreaWidget', data);
            setFixtures(view);
            var viewer = $('#content-viewer');
            $('<iframe id="htmlquiz-frame" src="http://lcl.whfreeman.com/tests/fixtures/html/lccontent.html" onload="dfFrameLoad()"></iframe>').appendTo(viewer);
        });
        afterEach(function() {
            loaded = false;
        });
        it('calls size frame without params when not df content', function() {
            spyOn(PxContentAreaWidget.PrivateStatic().fn, 'sizeFrame');
            waitsFor(function() {
                return loaded;
            }, "iFrame load timed out", 18000);

            runs(function() {
                expect(PxContentAreaWidget.PrivateStatic().fn.sizeFrame).toHaveBeenCalledWith();
            });
        });
    });

    describe('handling html quiz loads', function() {
        var quiz = {
            el: $(document),
            displayFooter: false,
            fnDestroy: function() {

            }
        };
        beforeEach(function() {
            spyOn(PxContentAreaWidget.PrivateStatic().fn, 'saveStatus');
            spyOn(PxContentAreaWidget.PrivateStatic().fn, 'submissionStatus');
            spyOn(PxContentAreaWidget.PrivateStatic().fn, 'htmlQuizLoad');
            $(PxPage.switchboard).trigger('htmlquiz-created', [quiz]);
        });
        it('runs htmlquizCreated', function() {
            expect(PxContentAreaWidget.PrivateStatic().fn.htmlquizCreated).toHaveBeenCalled();
        });
        it('binds saveStatus to saved event', function() {
            $(quiz.el).trigger('htmlquiz-saved', ['message']);
            expect(PxContentAreaWidget.PrivateStatic().fn.saveStatus).toHaveBeenCalled();
        });
        it('binds submissionStatus to submitted event', function () {
            $(quiz.el).trigger('htmlquiz-submit', ['message']);
            expect(PxContentAreaWidget.PrivateStatic().fn.submissionStatus).toHaveBeenCalled();
        });
        it('binds htmlQuizLoad to load event', function () {
            $(quiz.el).trigger('htmlquiz-loaded');
            expect(PxContentAreaWidget.PrivateStatic().fn.htmlQuizLoad).toHaveBeenCalled();
        });
    });

    xdescribe('regular iframe content', function () {
        
        
        beforeEach(function () {

            loaded = false;

            //Fixture stuff
            var viewPath = "DocumentViewer";
            var viewModelType = "Bfw.PX.PXPub.Models.DocumentToView";

            //TODO: NEED TO UPDATE TO USE TEST DATA.  Currently using data on dev that could be removed
            var doctoview = {
                AllowComments: 'false',
                DisciplineId: '6698',
                IsProductCourse: 'false',
                IsExternalContent: 'true',
                ItemId: 'bsi__3A996B89__95CA__4018__9C18__7D10B6A1A3F7',
                Url: ''
            };
            
            var viewModel = JSON.stringify(doctoview);
            var shebang = {
                viewPath: viewPath,
                viewModel: viewModel,
                viewModelType: viewModelType
            };

            var doc = pxRenderingHelper.controller.generateWidgetModel('PXPub', '', shebang, { escapeScript: false });
            setFixtures(doc);
            
            //List for the content to be loaded so we can run out test
            $('iframe').bind('load', function () {
                $(PxPage.switchboard).trigger('document-body-iframe-loaded');
                loaded = true;
            });
        });
        
        it('sizes the iframe when loaded', function() {
            spyOn($.fn, 'iframeAutoHeight');
            waitsFor(function () {
                return loaded;
            }, 'content to load', 5000);

            runs(function() {
                expect($.fn.iframeAutoHeight).toHaveBeenCalled();
                expect($.fn.iframeAutoHeight.calls.length).toEqual(1);
            });
        });
    });
    
    xdescribe('htmlquiz', function () {
        //These are integration tests with the xbookapp htmlquiz player
        var loaded = false;
        
        beforeEach(function () {

            loaded = false;
            
            //Fixture stuff
            var viewPath = "HtmlQuiz";
            var viewModelType = "Bfw.PX.PXPub.Models.HtmlQuiz";
            var token = HtmlQuizHelper.GetAuthToken();

            //TODO: NEED TO UPDATE TO USE TEST DATA.  Currently using data on dev that could be removed
            var xbookAppParams = {
                DlapCookie: token,
                ComponentName: 'MainContentArea',
                StudentOverride: 'FALSE',
                BrainHoneyUrl: 'http://pxmigration.dev.brainhoney.bfwpub.com/brainhoney',
                AgilixUrl: '/xbookapp',
                EnrollmentId: '137973',
                ProductCourseId: '122725',
                ItemID: 'bsi__C2B3CE9B__8BA5__47EE__8ADC__03448361E9D0'
            }
            var viewModel = JSON.stringify({
                Id: '',
                XBookAppParams: xbookAppParams
            });
            var shebang = {
                viewPath: viewPath,
                viewModel: viewModel,
                viewModelType: viewModelType
            };

            var doc = pxRenderingHelper.controller.generateWidgetModel('PXPub', '', shebang, { escapeScript: false });
            setFixtures(doc);
            
            //List for the htmlquiz to be loaded so we can run out test
            $(PxPage.switchboard).one('htmlquiz-loaded', function () {
                loaded = true;
            })
        });
        
        it('can retrieve both htmlquiz iframe', function () {
            waitsFor(function() {
                return loaded;
            }, 'htmlquiz to load', 10000);
            
            runs(function() {
                var frames = PxContentAreaWidget.PrivateStatic().fn.getHtmlQuizIFrames();
                expect(frames).not.toBe(undefined);
                expect(frames.contentFrame).not.toBe(undefined);
                expect(frames.parentFrames).not.toBe(undefined);
            })
        });
        
        it('calls autoHeight twice on quiz load', function () {
            spyOn($.fn, 'iframeAutoHeight');
            waitsFor(function () {
                return loaded;
            }, 'htmlquiz to load', 10000);

            runs(function () {
                expect($.fn.iframeAutoHeight).toHaveBeenCalled();
                expect($.fn.iframeAutoHeight.calls.length).toEqual(2);
            })
        });
    });
});
