//Represents the wiring up of all client-side behaviors on a page in
//platform-x
var PxPage = function($) {
    //private interface

    //Registers the given component as part of the page.  When the page
    //is initialized the component will be initialized in the appropriate
    //way. The 'component' argument is an object literal used to initialize
    //the component and contains members as follows:
    //name - the name of the component. must be unique across the page. the name 
    //config - an object that contains any configuration that needs to be passed to
    //         the component
    //
    //The 'obj' argument is the object representing the component instance
    var _registerComponent = null;

    //Removes the component along with any listeners it registered
    //target - name of the component to remove
    var _removeComponent = null;

    //Registers a callback function to execute when a specific event is fired. The
    //callback function must conform to the expected signature of the event source.
    //event - name of the event to capture
    //callback - function to execute when the event is fired
    //source - optional component name used to listen to even fired only by that component
    //target - optional component name of the component handling the callback
    //
    //returns -> unique ID of the listener that can be used to unregister it
    var _registerListener = null;

    //Removes the specified event listener
    //listener - if listener is an integer, it is assumed to be the event id returned by
    //           registerListener. if it is string then it is assumed to be a target that was
    //           passed when the listener was registered. In which case, all listeners for that
    //           target will be removed.
    var _removeListener = null;

    //When called by an event source, any registered callbacks will be fired.
    //source - name of the component that is firing the event
    //event - name of the event being fired
    //params - map of parameters to pass to the callback
    var _fireEvent = null;

    //When given the name of a component, an object representing that component instance
    //is returned
    var _getComponent = null;

    //Method that will initialize the page and any registered components
    var _init = null;

    //public interface
    var def = function() {
        return {
            registerComponent: _registerComponent,
            removeComponent: _removeComponent,
            registerListener: _registerListener,
            removeListener: _removeListener,
            fireEvent: _fireEvent,
            getComponent: _getComponent,
            init: _init
        };
    };

    //private data

    //true if the page has been initialized
    var _initialized = false;
    //list of components on the page
    var _components = [];
    //list of listeners that have been registered
    var _listeners = [];
    //stores the next id to be assigned to a registered listener
    var _nextListenerId = 1;

    _getComponent = function(component) {
        var obj = null;

        if (_components[component] !== undefined)
            obj = _components[component].obj;

        return obj;
    };

    _registerComponent = function(component, obj) {
        if (_components[component.name] !== undefined)
            throw "Component with name '" + component.name + "' is already registered";

        _components[component.name] = { name: component.name, config: $.extend(true, {}, {}, component.config), obj: obj, listeners: [] };
    };

    _registerListener = function(event, callback, source, target) {
        var id = -1;
        var index = event;

        if (source != null && source != "") {
            index = source + "." + event;
        }

        if (_listeners[index] === undefined) {
            _listeners[index] = [];
        }

        _listeners[index].push({ cb: callback, t: target });

        id = _nextListenerId;
        //if target is specified, we need to track any listeners added by the target
        if (target != null && target != "" && _components[target] !== undefined) {
            _components[target].listeners.push(id);
        }
        ++_nextListenerId;

        return id;
    };

    _removeListener = function(listener) {
        if (typeof listener === "number") {
            _listeners[listener] = null;
        }
        else if (typeof listener === "string") {
            if (_components[listener] !== undefined) {
                for (var c in _components[listener].listeners) {
                    _listeners[c] = null;
                }
            }
        }
    };

    _removeComponent = function(target) {
        if (_components[target] !== undefined) {
            _removeListener(target);
            _components[target] = null;
        }
    };

    _fireEvent = function(source, event, params) {
        var qualified = source + "." + event;

        //call all listeners to the source specific event
        if (_listeners[qualified] !== undefined) {
            for (var v in _listeners[qualified]) {
                _listeners[qualified][v].cb.apply(_components[_listeners[qualified][v].t].obj, params);
            }
        }

        //call all listeners to the general event
        if (_listeners[event] !== undefined) {
            for (var v in _listeners[event]) {
                _listeners[event][v].cb.apply(_components[_listeners[event][v].t].obj, params);
            }
        }
    };

    _init = function() {
        if (_initialized)
            return;

        $(document).ready(function() {
            for(var c in _components) {
                _components[c].obj.init(_components[c].config);
            }
        });
    };

    return def();

} (jQuery);