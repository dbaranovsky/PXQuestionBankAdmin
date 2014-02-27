// TocNavigationStructure

/*
The primary purpose of this object was a way of using logic to determine the next and previous item that gets displayed. Most other applications use the TOC to store information about the item but in XBOOK we did not control the TOC so another structure was created.
This seems like a cleaner way of storing information about the items in the project. It does not update or save anything back to DLAP by design. 

New next and prev methods can be added to the private section of this object and by using the options next_rule and prev_rule you can set which method actually gets called when calling next or prev (public methods)
Which means other applications can use this functionality with out running into issues with current users of the application

The one task that is known to be outstanding is the ability to reload at specific places in the hierachy, currently it can only be called at the top level item id which loads everything again. 
Another task that is known is it does not currently have the abiltiy to change the next or previous method in the middle. for example if the TOC is filtered to only include an assignments worth of work, then calling next or previous will not be able to filter out the extra calls. The only way this is possible is reinit the object with different options when the filter changes
*/
var PxTocNavStructure = function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: "PxTocNavStructure",
        dataKey: "",
        default_options: {
            reload_callback: function() { },
            next_rule: "next_all_but_excluded",
            prev_rule: "prev_all_but_excluded",
            top_level_item_id: "PX_TOC",
            toc_definition: "bfw_toc_contents",
            excludeSingleItem: function (item) { return false; },
            excludeFolderItem: function (item) { return false; },

            depth: -1,
            includeHref: true,
            includeType: true,
            includeSubtype: true,
            includeHidden: true,
            includeHiddenFromStudents: true,
            includeHiddenFromToc: true,
            includeTitle: true,
            includeBfw_Subtype: true,
        },
        options: {},
        events: {},
        defaults: {},
        state: { 
            // key - value pair where the key is an item id and the value is a hash of data assocaited to the item
            items: {}, 
            // a flatten list of item ids based of the TOC structure so it is easy to determine the next or previous item
            hierarchy: [],
            initialized: false
        },
        sel: {},
        // utility functions
        util: {
            /*
            function: flattenArray
            Description: converts a multi level array structure into a single array and keeping the logical order of the items from lower index to higher index value
            Params: 
                input: array of values or a single value
                output: flattened array
            Returns: 
                returns an array structure
                [1, [2, 3], 4, [5, [6, 7]]] => [1, 2, 3, 4, 5, 6, 7]
            */
            flattenArray: function(input, output){
                if($.isArray(input))
                {
                    for (var i=0;i<input.length;i++)
                    {
                         _static.util.flattenArray(input[i], output);
                    }
                }
                else
                {
                    output.push(input);
                }
            } // END OF flattenArray
        }, // END OF util

        // private functions
        fn: {
            init: function (options) {
                if ((options['navigation_structure'] || '').length == 0) {
                    PxPage.log('URL to connect to PX Server was not supplied ["navigation_structure"] as an option');
                    return false;
                }
                _static.options = $.extend({}, _static.default_options, options);
            }, // END OF init

            getItem: function(itemid) {
                return _static.state.items[itemid];
            }, // END OF getItem

            reload: function (itemid, callback) {
                $.ajax({
                    url: _static.options.navigation_structure,
                    data: {
                        tocDefinition: _static.options.toc_definition,
                        top_item_id: itemid,
                        depth: _static.options.depth,
                        includeHref: _static.options.includeHref,
                        includeType: _static.options.includeType,
                        includeSubtype: _static.options.includeSubtype,
                        includeHidden: _static.options.includeHidden,
                        includeHiddenFromStudents: _static.options.includeHiddenFromStudents,
                        includeHiddenFromToc: _static.options.includeHiddenFromToc,
                        includeTitle: _static.options.includeTitle,
                        includeBfw_Subtype: _static.options.includeBfw_Subtype,
                    },
                    type: "POST",
                    success: function (response, status, obj) {
                        if (status != "success") {
                            PxPage.log('Error making request to reload the PxTocNavStructure');
                            return;
                        }
                        if (response.STATUS != "OK") {
                            PxPage.log('Error making request to "' + _static.options.toc_definition + '"');
                            PxPage.log('Error Message: "' + response.ErrorMessage + '"');
                            return;
                        }
                        _static.state.items = response.Items;
                        _static.fn.generateHierarchy();
                        _static.state.initialized = true;

                        (callback || _static.options.reload_callback).apply(this, []);
                    }
                });
            }, // END OF reload

            /*
            function: isItemInAExcludedFolder
            Description: checks to see if an item being requested is logicly in a folder item that is currently hidden
            Params: 
                itemid: item id from the TOC
            Returns: 
                true: item is in a folder strcture that is hidden
                false: item is not in a folder structure that is hidden
            */
            isItemInAExcludedFolder: function(itemid)
            {
                var parentid = _static.state.items[itemid].TOC_ParentId;

                var isExcluded = false;
                if ($.isFunction(_static.options.excludeFolderItem))
                    isExcluded = _static.options.excludeFolderItem(parentid);
                    
                if (isExcluded)
                    return true;
                
                if (itemid == _static.options.top_level_item_id)
                    return false;
                    
                return _static.fn.isItemInAExcludedFolder(parentid);
            }, // END OF isItemInAExcludedFolder

            // helper method only being used with reload is being called to generate the hierarchy
            getChildHierarchy: function(itemid)
            {
                var number_of_children = _static.state.items[itemid].Descendants.length;

                if (number_of_children == 0)
                    return itemid;
                
                var list = [];
                var i = 0;
                while(i < number_of_children)
                {
                    list.push(_static.fn.getChildHierarchy(_static.state.items[itemid].Descendants[i]));
                    i++;
                }
                return [itemid, list];
            }, // END OF getChildHierarchy

            // helper method only being called to generate the order order of items in the TOC, returns nothing but does set the global hierarchy object which is a flat list of order item ids
            generateHierarchy: function()
            {
                var multi_level_hierarchy = _static.fn.getChildHierarchy(_static.options.top_level_item_id);
                _static.util.flattenArray(multi_level_hierarchy, _static.state.hierarchy);
            }, // END OF generateHierarchy

            current: function(currentItemId) {
                if (_static.state.initialized == false)
                    return currentItemId;

                var curr = _static.state.items[currentItemId];

                var excludeFolder = _static.fn.isItemInAExcludedFolder(currentItemId);
                var excludeItem   = _static.options.excludeSingleItem(curr);

                if (!excludeFolder && !excludeItem)
                    return currentItemId;

                return PxTocNavStructure.next(currentItemId);
            }, // END OF current

            next_all_but_excluded: function (currentItemId) {
                if (_static.state.initialized == false)
                    return currentItemId;

                var curr = _static.state.items[currentItemId];

                var index = _static.state.hierarchy.indexOf(currentItemId);
                var number_of_items = _static.state.hierarchy.length;

                while(index++ < number_of_items)
                {
                    var possible_prev = _static.state.hierarchy[index];
                    var excludeFolder = _static.fn.isItemInAExcludedFolder(possible_prev);
                    var excludeItem   = _static.options.excludeSingleItem(_static.state.items[possible_prev]);

                    if (!excludeFolder && !excludeItem)
                        return possible_prev;

                    if (possible_prev == _static.options.top_level_item_id)
                        continue;
                }

                throw "No Next Found";
            }, // END OF next_all_but_excluded

            prev_all_but_excluded: function (currentItemId) {
                if (_static.state.initialized == false)
                    return currentItemId;

                // when the item does not have children it needs to look for its siblings unless you are the 'top_level_item_id'
                if (currentItemId == _static.options.top_level_item_id)
                    throw "No Prev Item to display";
                
                var curr = _static.state.items[currentItemId];

                var index = _static.state.hierarchy.indexOf(currentItemId);

                while(index-- > 0)
                {
                    var possible_prev = _static.state.hierarchy[index];
                    // item exists within an excluded folder item
                    var excludeFolder = _static.fn.isItemInAExcludedFolder(possible_prev);
                    // the item its self is excluded
                    var excludeItem   = _static.options.excludeSingleItem(_static.state.items[possible_prev]);

                    if (!excludeFolder && !excludeItem)
                        return possible_prev;

                    if (possible_prev == _static.options.top_level_item_id)
                        continue;
                }

                throw "No Previous Found";
            } // END OF prev_all_but_excluded
        } // END OF fn
    }; // END OF _static

    return {
        /*
        function: init
        Description: initiates and loads the data for the first time
        Params: 
            options: helps
            callback: call back is executed after the data has been loaded
        Returns: nothing expected
        */
        init: function (options, callback) {
            _static.fn.init(options);
            _static.fn.reload(_static.options.top_level_item_id, callback);
        }, // END OF init

        /*
        function: reload
        Description: reloads the object with updated information, this will need to be called after an item gets added, deleted or updated so it can be kept up to date
        Params: 
            itemid: 
                undefined, null or "": uses the option "top_level_item_id" as the item to look up
                "[a-zA-Z0-9\-\_]": reloads the tree and only updates the items that get returned from the server request, so the entire tree does not need to be reloaded
            callback: 
                method that gets called when the data has finished loading
        Returns: nothing expected
        KnownIssues:
            TODO: currently only reloads from the highest level PX_TOC, you should be able to pass in an item id and only update only the things that should have changed
        */
        reload: function (itemid, callback) {
            if (itemid == undefined || (itemid || "").trim().length == 0)
                _static.fn.reload(_static.options.top_level_item_id, callback);
            else
                _static.fn.reload(itemid, callback);
        }, // END OF reload

        /*
        function: next
        Description: determines the next item in the TOC given the currently displayed item which was passed in
        Params: 
            currentItemId: item id from the TOC
        Returns: the item id using the next_rule option param to determine what next really means
        */
        next: function (currentItemId) {
            return _static.fn[_static.options.next_rule].apply(this, [currentItemId]);
        }, // END OF next

        /*
        function: prev
        Description: determines the prev item in the TOC given the currently displayed item which was passed in
        Params: 
            currentItemId: item id from the TOC
        Returns: the item id using the prev_rule option param to determine what prev really means
        */
        prev: function (currentItemId) {
            return _static.fn[_static.options.prev_rule].apply(this, [currentItemId]);
        }, // END OF prev

        /*
        function: current
        Description: Checks to see if the item passed is can be displayed and if not it gets the next item that can be displayed. It is a way of checking to see if an item id can be displayed
        Params: 
            currentItemId: item id from the TOC
        Returns: 
            returns the currentItemId if the item has not been excluded from being seen
            If the item is excluded than it will return the next available item id that should be displayed
        */
        current: function(currentItemId) {
            return _static.fn.current(currentItemId);
        }, // END OF current

        /*
        function: getItemProperties
        Description: gets the item data associated to an item id, item data is defined with in the options
        Params: 
            itemid: item id from the TOC
        Returns: returns a hash of values associated to the item
        */
        getItemProperties: function (itemid) {
            return _static.fn.getItem(itemid);
        } // END OF getItemProperties
    };
} (jQuery);
