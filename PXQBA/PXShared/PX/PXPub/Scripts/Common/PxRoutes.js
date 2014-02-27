// PxRoutes:
// Description: Wrapper for the crossroads plugin.  Does things like setup routes for product/component and handle whatever piping we need to setup.
// See http://millermedeiros.github.com/crossroads.js for crossroads explaination
var PxRoutes = function ($) {
    var _static = {
        defaults: {
            redirectPriority: 100,
            debug: true,
            component_route: "{func}:?args:" //default component route. used if null is passed in.  
        },
        redirectSyntax: 'state/:component:/:func*::?args:',
        redirectRoute: {},
        redirectHandler: {},
        componentRouter: {}, //handles the routes for components
        productRoutes: null,
        fn: {
            addProductRoute: function (route, handler, priority, rules) {
                var newroute = crossroads.addRoute(route, handler, priority);
                if (rules !== undefined) {
                    newroute.rules = rules;
                }
                _static.productRoutes = $(_static.productRoutes).add(newroute);
            },
            //Adds a component level route (ie. gradebook, fne?).  Sets all previously added product level routes to pipe to this route
            addComponentRoute: function (component, croute, handler, priority) {
                var croute = croute || _static.defaults.component_route;
                var route = '{state*}/' + component + '/' + croute;

                if (_static.componentRouter.addRoute !== undefined) {
                    var compiledroute = _static.componentRouter.addRoute(route, handler, priority);
                    compiledroute.rules = {
                        "state*": function(value, request, valuesObj) {
                            if (!isNaN(value) || value == "state" || value == "/state" || value == "#state") {
                                return false;
                            }
                            return true;
                        }
                    };
                } else {
                    PxPage.log('PxRoutes has not been initialized.  Component route not added.');
                }

            },
            debugBypass: function (request) {
                PxPage.log('product route bypassed: ' + request);
            },
            componentBypassed: function (request) {
                PxPage.log('component route bypassed: ' + request);
            }
        }
    };

    return {
        //Initializes the redirect route and sets the redirect handler. 
        Init: function (redirectHandler, options) {
            if (crossroads === undefined) {
                PxPage.log('crossroads plugin not loaded.');
                return;
            }

            $.extend(true, _static.defaults, options);

            _static.componentRouter = crossroads.create();
            _static.componentRouter.ignoreState = true; //components need to ignore state because not every route will be matched here
            if (_static.defaults.debug) {
                crossroads.bypassed.add(_static.fn.debugBypass);
                _static.componentRouter.bypassed.add(_static.fn.componentBypassed);
            }

            //Once the crossroads router has finished parsing, pass onto the component router.
            crossroads.pipe(_static.componentRouter);
            //Setup the redirect route for components
            _static.redirectRoute = crossroads.addRoute(_static.redirectSyntax, redirectHandler, _static.defaults.redirectPriority);
            _static.redirectHandler = redirectHandler;

        },
        //Adds a product level route (ie. xbook, portal, etc.). Pipes all hash's from this route to previously added component routes
        AddProductRoute: function (route, handler, priority, rules) {
            _static.fn.addProductRoute(route, handler, priority, rules);
        },
        //Adds a component level route (ie. gradebook, fne?).  Sets all previously added product level routes to pipe to this route
        AddComponentRoute: function (component, route, handler) {
            _static.fn.addComponentRoute(component, route, handler);
        },

        GetProductRoutes: function () {
            return _static.productRoutes;
        },
        ForceRouteParse: function () {
            crossroads.resetState();
            crossroads.parse(hasher.getHash());
        }
    };
} (jQuery);