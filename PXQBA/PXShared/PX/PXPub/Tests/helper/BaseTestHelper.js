var _static = {
    cached: {},
    fn: {
        setFixtureBasedOnMode: function(html, mode, target) {
            switch (mode) {
            case 'set':
                setFixtures(html);
                break;
            case 'append':
                appendSetFixtures(html);
                break;
            case 'target':
                $(target).html(html);
                break;
            default:
                setFixtures(html);
                break;
            }
        }
    }
};



var BaseTestHelper = function ($) {
    return {
        model: {
            generateViewModel: function(model) {
                var viewModel = {
                    viewPath: model.path,
                    viewModel: JSON.stringify(model),
                    viewModelType: model.type
                };
                return viewModel;
            }
        },
        api: {
            setFixtureFromView: function(model, folder, mode, target, callback) {
                mode = !mode ? 'set' : mode;
                var html = PxViewRender.RenderView('PXPub', folder, model);
                _static.fn.setFixtureBasedOnMode(html, mode, target);
                if (callback)
                    callback(html);

            },
            setFixtureFromCache: function(model, folder, mode, target) {
                if (!model.viewPath || !model.viewModelType)
                    BaseTestHelper.api.setFixtureFromView(model, folder, mode, target);
                var key = model.viewPath + '|' + model.viewModelType;
                if (_static.cached[key]) {
                    var view = _static.cached[key];
                    _static.fn.setFixtureBasedOnMode(view, mode, target);
                } else {
                    BaseTestHelper.api.setFixtureFromView(model, folder, mode, target, function (html) {
                        _static.cached[key] = html;
                    });
                }

            },
            clearCache: function () {
                _static.cached = {};
            }
        },
        spy: {
            
            spyOnFauxtree: function(callFakes) {
                $.fn.fauxtree = jasmine.createSpy('fauxtree').andCallFake(function(api, args) {
                    if (callFakes && callFakes[api] && (typeof callFakes[api] === "function")) {
                        return callFakes[api](args);
                    }
                });
            },
            spyOnPxPage: function (callFake) {
                window.PxPage = jasmine.createSpyObj('PxPage', ['Loading', 'Loaded', 'Routes', 'log', 'switchboard', 'FneInitHooks', 'FneResizeHooks', 'Fade', 'isIE']);
                PxPage.Toasts = jasmine.createSpyObj('PxPage.Toasts', ['Error', 'Success', 'Info']);

                if (callFake) {
                    PxPage.Toasts.Error.andCallFake(function (message) {
                        $('#pxpage-toasts-error').html(message);
                    });
                    PxPage.Toasts.Success.andCallFake(function (message) {
                        $('#pxpage-toasts-success').html(message);
                    });
                }
            },
            spyOnHashHistory: function (callFakes) {
                window.HashHistory = jasmine.createSpyObj('HashHistory', ['InitializeWithArgs']);
                if (callFakes && callFakes[api] && (typeof callFakes[api] === "function")) {
                    return callFakes[api](args);
                }
            }
        }
    };
}(jQuery);