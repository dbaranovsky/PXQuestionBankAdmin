// FNE functionality that is used when a Page Definition is rendering the document
var PageDefinedFNE = function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: "PageDefinedFNE",
        dataKey: "PageDefinedFNE",

        triggers: {
            PRE_LOAD: 'PAGE_DEFINED_FNE_LOADING_PRE_REQUEST',
            POST_LOAD: 'PAGE_DEFINED_FNE_LOADING_POST_REQUEST',
            PRE_CLOSE: 'PAGE_DEFINED_FNE_CLOSE_PRE_REQUEST',
            POST_CLOSE: 'PAGE_DEFINED_FNE_CLOSE_POST_REQUEST',
        },
        events: {
            
        },
        defaults: {
                
        },
        settings: {
            
        },
        sel: {
            FNE_DIV: '#page_defined_fne',
            FNE_CONTENT_CONTAINER: '.content',
            PARENT_TO_FNE: 'body',
            APP_FOOTER: '#footer',
            APP_HEADER: '.homepage-course-info',

            FNE_SIZE_MATCHER: '', // will be seen as window.document if left blank
        },

        /** Private Methods for the Widget **/
        fn: {
            // helper methods

            getFneDiv: function ( ) {
                if (_static.fn.hasFneDivInDom())
                    return $(_static.sel.FNE_DIV);
                else
                    return _static.fn.createFneDivInDom();
            },

            // Determines if the FNE div tag is in the 
            hasFneDivInDom: function ( ) {
                return $(_static.sel.FNE_DIV).length > 0;    
            },

            // creates the div element if it does not already exist
            createFneDivInDom: function ( ) {
                if (_static.fn.hasFneDivInDom())
                    return;

                var id_name = _static.sel.FNE_DIV.substr(1);
                var class_name = _static.sel.FNE_CONTENT_CONTAINER.substr(1);
                var fne_node = $("<div></div>").attr('id', id_name);
                fne_node.append($("<div></div>").addClass(class_name));
                fne_node.hide();
                $(_static.sel.PARENT_TO_FNE).append(fne_node);
                return $(_static.sel.FNE_DIV);
            },

            updateFneSize: function ( ) {
                var fne = $(_static.sel.FNE_DIV);
                var matcher = $(_static.sel.FNE_SIZE_MATCHER);
                
                if (_static.sel.FNE_SIZE_MATCHER == undefined ||
                    _static.sel.FNE_SIZE_MATCHER.length == 0)
                {
                    matcher = $(window.document);
                }
                
                fne.width( matcher.width() );
                fne.height(matcher.height());
            },

            updateFnePosition: function ( ) {
                var fne = $(_static.sel.FNE_DIV);
                
                fne.css({position: 'absolute', top: 0, background: 'white'});
            },

            openFNE: function ( url ) {
                var fne_window = _static.fn.getFneDiv();
                var fne_contents = fne_window.find(_static.sel.FNE_CONTENT_CONTAINER);

                $(PxPage.switchboard).trigger(_static.triggers.PRE_LOAD);

                fne_contents.empty();

                _static.fn.updateFneSize();
                _static.fn.updateFnePosition();

                fne_window.show();
                PxPage.Loading(_static.sel.FNE_DIV.substr(1));
                fne_contents.load(url, function(response, status, jqxhr){
                    PxPage.Loaded(_static.sel.FNE_DIV.substr(1));    
                    $(PxPage.switchboard).trigger(_static.triggers.POST_LOAD);
                });
  
		    },

            closeFNE: function ( ) {
                $(PxPage.switchboard).trigger(_static.triggers.PRE_CLOSE);
                var fne_window = _static.fn.getFneDiv();
                fne_window.hide();
                $(PxPage.switchboard).trigger(_static.triggers.POST_CLOSE);
            },

            pageDefinedFNEOnLoaded: function ( ) {
                alert('loaded');
            },

            init: function ( options ) {
                $.extend(true, _static.settings, _static.defaults, options);
            },  
        },
    };

    /** The public interface for interacting with this plugin **/
    $.fn.PageDefinedFNE = function () {
        return _static.fn.init.apply(this, arguments);
    };

    /** Public Method for the Widget **/
    return {
        OpenFNE: function (url) {
            _static.fn.openFNE(url);
        },
        CloseFNE: function ( ) {
            _static.fn.closeFNE();
        },
        PageDefinedFNEOnLoaded: function ( ) {
            _static.fn.pageDefinedFNEOnLoaded();
        },
        init: function ( ) {
        }
    };

} (jQuery);