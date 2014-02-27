


// This plugin provides the functionalty to manage UI for widgets, like add, remove, move (in same zone or to a different zone)
// options:
//pageName:                 name of the page on which this plugin is being used, so that it can be passes in ajax calls to server
//ajaxUrlAddWidget:         url for ajax call to server to add widget
//ajaxUrlRemoveWidget:      url for ajax call to server to remove widget
//ajaxUrlMoveWidget:        url for ajax call to server to MOVE widget
//ajaxUrlSetWidgetDisplay:  url for ajax call to server to set widget display
//zoneParentClass:          class that  wraps a zone. zonePrant has UI for adding widgt and a class for actual zone
//zoneClass:                class that is applied to all zones, used to find zones in DOM
//widgetClass:              class that is applied to all widgets, used to find widgets in DOM
//addWidgetClass:           class to find the div, which has to be clicked in order to add a new widget
//chooseWidgetClass:        div which display list of widgets that can be added
//widgetDisplayItemClass:   class around each element of list of widgets, which can be added.
//removeWidgetClass:        to find remove buttons in zones and attach events
//widgetToolbarClass:       class to find the widget toolbar so that it can be used to bind drag and drop functionality
//toggleDisplayClass:       class to find control which will expand and collapse widget
//widgetBodyClass:          class to get the body of widget so that it can be hidden while collapsing
//widgetCollapseClass:      class uses for toggle button when in expand mode
//widgetExpandClass:        class uses for toggle button when in collapse  mode
(function ($) {
    // privately scoped data used by the plugin
    var _private = {
        pluginName: "PageLayout",
        dataKeyPlugin: "Zones_Widgets",
        dataKeyZone: "Zone",
        bindSuffix: ".PageLayout",
        itemId: "itemId",
        allowedWidgets: "allowedWidgets",
        sequence: "sequence",
        widgetTemplateId: "templateId",
        allowMultiple: "isMultipleAllowed",
        widgetSilentCreation: "silentCreation",
        dialogWidth: "dialogWidth",
        dialogHeight: "dialogHeight",
        dialogMinWidth: "dialogMinWidth",
        dialogMinHeight: "dialogMinHeight",
        dialogTitle: "dialogTitle",
        defaults: {
            pageName: '',
            ajaxUrlAddWidget: '',
            ajaxUrlEditWidget: '',
            ajaxUrlGetWidgetTemplate: '',
            ajaxUrlRemoveWidget: '',
            ajaxUrlMoveWidget: '',
            ajaxUrlSetWidgetDisplay: '',
            ajaxUrlSaveCustomWidget: '',
            zoneParentClass: 'zoneParent',
            zoneClass: 'zone',
            widgetClass: 'widgetItem',
            addWidgetClass: 'addWidget', // to find the div which will be clicked to show list of widgets which can be added
            chooseWidgetClass: 'chooseWidget', // div which display list of widgets that can be added
            widgetDisplayItemClass: 'widgetDisplayItem', // class around each element of list of widgets, which can be added.
            removeWidgetClass: '', // to find remove buttons in zones and attach events
            widgetToolbarClass: 'widgetToolbar', // class to find the widget toolbar so that it can be used to bind drag and drop functionality
            toggleDisplayClass: 'widgetToggleDisplay', // class to find control which will expand and collapse widget
            widgetBodyClass: 'widgetBody', // class to get the body of widget so that it can be hidden while collapsing
            widgetCollapseClass: 'widgetCollapse', // class uses for toggle button when in expand mode
            widgetExpandClass: 'widgetExpand', // class uses for toggle button when in collapse  mode
            customEditWidgetClass: 'customEditWidget', // class uses for custom Widget (To get dialog properties)
            BeforeAddWidgetForm: 'BeforeAddWidgetForm', // Form uses for Custom Widet AJAX Form
            hiddenWidgetZoneID: 'WidgetZoneID', // Hidden Field to store Widget Zone Id for further use (in case of Custom Widgets where we require extra input before add) 
            hiddenWidgetTemplateID: 'WidgetTemplateID', // Hidden Field to store Widget Template Id for further use (in case of Custom Widgets where we require extra input before add) 
            hiddenWidgetID: 'WidgetID', // Hidden Field to store Widget Template Id for further use (in case of Custom Widgets where we require extra input before add) 
            Mode: 'View', // possible values are 'Edit' or 'View' 
            WidgetHeaderTextClass: "widgetHeaderText" // Class uses for Widget Header Text
        },
        // following 3 objects zone, zone settings and widgtes are listed here ONLY to provide the schema of data structure used to keep information in this plugin
        zones: [], // the data of this plugin will have an array called zones and each element in this array will be of type zoneSettings
        zoneSettings: { // data structure to store zone information
            itemId: '',
            allowedWidgets: '',  // comma separated list of template ids of widgets which are allowed in the zone
            widgets: ''          // array of objects of type widgetSettings to keep track of all widgets in the zone
        },
        widgetSettings: { // data structure to store widget information
            itemId: '',
            sequence: '',
            templateId: '',
            allowMultiple: false
        }
    };



    // Gets zone (of type zoneSettings). Looks up the array of zones (stored in the  data of plugin)
    // zoneId : id of the zone which we are looking for.
    //instance: JQuery object for instance on which the plugin is called, used to get settings from data of plugin
    //Reurens: zone (of type zoneSettings) which has the zoneId
    _getZone = function (zoneId, instance) {
        if (!zoneId) {
            return null;
        }

        var $this = instance,
        zones = $this.data(_private.dataKeyZone);

        var len = zones.length;
        for (var i = 0; i < len; i++) {
            if (zones[i].itemId == zoneId) {
                return zones[i];
            }
        }
    }

    // this functiion looks at all widgets in the zone  and gets a widget which has given templateId
    // templateId: templateId of the widget
    // zone: is of type zoneSettings, representing a zone
    //Returns: widget (of type widgetSettings) which has given templateId
    _getWidgetByTemplateId = function (templateId, zone) {
        if (!templateId || !zone) {
            return null;
        }

        var zoneCount = zone.widgets.length;
        for (var i = 0; i < zoneCount; i++) {
            if (zone.widgets[i].templateId == templateId) {
                return zone.widgets[i];
            }
        }
    };


    // Adds widget to zone, in the data of plugin to keep the memory state in sych with the UI
    // widget: is the  object of type widgetsettings 
    // zone: is of type zoneSettings 
    //Reurns: Void
    _addWidgetToZone = function (widget, zone) {
        if (!zone.widgets) {
            zone.widgets = [];
        }
        zone.widgets[zone.widgets.length] = widget;
    }

    // Removes widget from the zone.  in the data of plugin to keep the memory state in sych with the UI
    // widgetId: id of the widget to be removed
    // zone: zone from where the widget is to be removed, if passed null, zone will be determined by looking up all zones and finding one which has this widget
    //instance: JQuery object for instance on which the plugin is called, used to get settings from data of plugin
    //Reurns: Void
    _removeWidgetFromZone = function (widgetId, zone, instance) {
        if (!widgetId) {
            return;
        }


        if (!zone) {
            zone = _getZoneForWidget(widgetId, instance);
        }

        if (zone) {
            var len = zone.widgets.length;
            for (var i = 0; i < len; i++) {
                if (zone.widgets[i].itemId == widgetId) {
                    zone.widgets.splice(i, 1);
                    break;
                }
            }
        }
    }

    // Goes through all zones in the data of plugin and finds the zone using widgetId
    //widgetId: id of the widget (of type widgetsettings)
    //instance: JQuery object for instance on which the plugin is called, used to get settings from data of plugin
    //Returns: zone (of type zoneSettings) which has a widget (with itemId = widgetId )
    _getZoneForWidget = function (widgetId, instance) {
        if (!widgetId) {
            return;
        }

        var $this = instance,
        zones = $this.data(_private.dataKeyZone),
        zoneCount = zones.length,
        zone = null,
        widgetCount = 0;

        for (var i = 0; i < zoneCount; i++) {
            zone = zones[i];
            if (zone.widgets) {
                widgetCount = zone.widgets.length;
                for (var j = 0; j < widgetCount; j++) {
                    if (zone.widgets[j].itemId == widgetId)
                        return zone;
                }
            }
        }
    }

    //Zone ids are in format PX_[pageName]_[ZoneId], this funtion does string manuplation and returns the last part which is [ZoneId]
    //pageName: (type of string) name of the current page (this was passed to plugin while initializing it)
    //zoneId: id of the zone from which PX_[pageName] has to be replaced. 
    //Reurens: a string
    _getZoneName = function (pageName, zoneId) {
        if (!pageName || !zoneId) {
            return;
        }

        var toReplace = "PX_" + pageName.toUpperCase() + "_";
        var id = zoneId.replace(toReplace, "");

        return id;
    }

    //this funtion is called from the event of Jquery sortable, so that a widget can be dragged from one place to another
    //widgetUI: ui argument from the events of Jquery sortable
    //targetZoneId: item id of the target zone (of type zoneSettings)
    //Returns: boolean -- true for success and false for failure
    _moveWidget = function (widgetUI, targetZoneId) {
        if (!widgetUI || !targetZoneId) {
            return false;
        }

        var $this = $(this),
        widgetItemId = widgetUI.item.attr(_private.itemId),
        widgetSequence = widgetUI.item.attr(_private.sequence),
        widgetTemplateId = widgetUI.item.attr(_private.widgetTemplateId),
        widgetAllowMultiple = widgetUI.item.attr(_private.allowMultiple),
        widgetSettings = { itemId: widgetItemId, sequence: widgetSequence, templateId: widgetTemplateId, allowMultiple: widgetAllowMultiple },
        sourceZone = _getZoneForWidget(widgetItemId, $this),
        targetZone = _getZone(targetZoneId, $this),
        settings = $this.data(_private.dataKeyPlugin);
        isSameZone = true;

        if (!sourceZone || !targetZone) {
            return false;
        }

        if (sourceZone.itemId != targetZone.itemId) {
            isSameZone = false;
        }

        if (!targetZone.allowedWidgets || targetZone.allowedWidgets.indexOf(widgetTemplateId) == -1) {
            //  alert("widget is not allowed in zone");
            return false;
        }

        if (!isSameZone) {
            var alreadyExistingWidget = _getWidgetByTemplateId(widgetTemplateId, targetZone);
            if (alreadyExistingWidget) {
                // if widget was found in zone, the we have to check whether it can be added more than once or not.
                if (alreadyExistingWidget.allowMultiple.toUpperCase() == "FALSE") {
                    alert("this widget is already in this zone, and cannot be added more than once");
                    return false;
                }
            }
            _removeWidgetFromZone(widgetItemId, sourceZone, $this);
            _addWidgetToZone(widgetSettings, targetZone);
        }


        // code for ajax call to update agilix        

        var settings = $this.data(_private.dataKeyPlugin),
        currentPageName = settings.pageName,
        targetId = _getZoneName(currentPageName, targetZone.itemId);

        var prevWidgetSequence = widgetUI.item.prevAll("." + settings.widgetClass).attr("sequence"),
        nextWidgetSequence = widgetUI.item.nextAll("." + settings.widgetClass).attr("sequence");
        if (prevWidgetSequence == 'undefined') {
            prevWidgetSequence = '';
        }
        if (nextWidgetSequence == 'undefined') {
            nextWidgetSequence = '';
        }


        var bodyPostContent = $.ajax({
            type: "POST",
            url: settings.ajaxUrlMoveWidget,
            data: { pageName: currentPageName, zoneName: targetId, widgetId: widgetItemId, minSequence: prevWidgetSequence, maxSequence: nextWidgetSequence },
            success: function (data) {
                widgetUI.item.attr("sequence", data.Sequence);
                //                alert(data.Sequence);
            },
            error: function (req, status, error) {
                // alert("ERROR_POST_MOVE");
            }
        });

        return true;
    }

    // expands and collapses widget UI
    //toggleButton: jQuery object representing the button which is clicked to expand or collapse widget 
    // settings: setting data of the plugin
    _toggleWidgetDisplay = function (toggleButton, settings) {
        if (toggleButton.hasClass(settings.widgetCollapseClass)) {
            toggleButton.removeClass(settings.widgetCollapseClass);
            toggleButton.addClass(settings.widgetExpandClass);
            toggleButton.parents("." + settings.widgetClass).find("." + settings.widgetBodyClass).toggle("slow");
            toggleButton.parents("." + settings.widgetClass).attr("style", "height:50px;");
        }
        else {
            toggleButton.removeClass(settings.widgetExpandClass);
            toggleButton.addClass(settings.widgetCollapseClass);
            toggleButton.parents("." + settings.widgetClass).find("." + settings.widgetBodyClass).toggle("slow");
            toggleButton.parents("." + settings.widgetClass).removeAttr("style");
        }
    }


    // this function displays the edit widget UI in a dialog
    //editWidgetUI: jQuery object representing the UI for edit widget
    _displayEditWidgetDialog = function (editWidgetHTML, zoneId, widgetTemplateId, widgetId) {

        var editWidgetUI = $(editWidgetHTML);
        var $this = $(this);

        var settings = $(this).data(_private.dataKeyPlugin),
        dialogWidth = editWidgetUI.find("." + settings.customEditWidgetClass).attr(_private.dialogWidth),
        dialogHeight = editWidgetUI.find("." + settings.customEditWidgetClass).attr(_private.dialogHeight),
        dialogMinWidth = editWidgetUI.find("." + settings.customEditWidgetClass).attr(_private.dialogMinWidth),
        dialogMinHeight = editWidgetUI.find("." + settings.customEditWidgetClass).attr(_private.dialogMinHeight),
        dialogTitle = editWidgetUI.find("." + settings.customEditWidgetClass).attr(_private.dialogTitle);

        var customWidgetModal = $('.customWidgetModal').first();
        $(customWidgetModal).html('<div style="padding-top:10px;padding-bottom:10px;">Loading Widget Dialog..</div>')
        $(customWidgetModal).dialog({ dialogClass: 'dialog1', width: dialogWidth, height: dialogHeight, minWidth: dialogMinWidth, minHeight: dialogMinHeight, modal: true, draggable: false, title: dialogTitle,
            open: function (event, ui) {
                $('<a />', {
                    text: ' | '
                })
                    .appendTo($(".ui-dialog-buttonset"));

                $('<a />', {
                    'class': 'linkCancelClass', // TO DO : Define
                    text: 'Cancel',
                    href: '#'
                })
                    .appendTo($(".ui-dialog-buttonset"))
                    .click(function () {
                        $(event.target).dialog("destroy");
                    });

                $(".ui-dialog-titlebar").attr("style", "font-size:14px;");
                $(".ui-dialog-buttonpane").attr("style", "text-align:center; font-size:12px;");
                $(".ui-dialog-content").attr("style", "font-size:12px;");
                $(".ui-dialog-buttonset").attr("style", "float:left;");
            },
            buttons: {
                Save: function (evt) {
                    PxPage.Loading(zoneId); //Load the spinner
                    var buttonDomElement = evt.target;
                    $(buttonDomElement).attr('disabled', true);

                    var bodyPostContent = $.ajax({
                        type: "POST",
                        url: settings.ajaxUrlGetWidgetTemplate,
                        data: { templateID: widgetTemplateId },
                        success: function (data) {
                            if (data.Result == 'SUCCESS') {
                                var widgetSaveUrl = settings.ajaxUrlSaveCustomWidget;
                                widgetSaveUrl = widgetSaveUrl.replace("DYNAMIC_CONTROLLER", data.WidgetSaveController);
                                widgetSaveUrl = widgetSaveUrl.replace("DYNAMIC_ACTION", data.WidgetSaveAction);
                                var inputValues = $(customWidgetModal).find(".InputForControllerAction"); // Need to extend this, in case of different kind of Custom Widgets.
                                var inputVal = '';
                                jQuery.each(inputValues, function () {
                                    inputVal = inputVal + $(this).attr("name");
                                    inputVal = inputVal + '#';
                                    inputVal = inputVal + $(this).val();
                                });
                                var bodyPostContent = $.ajax({
                                    type: "POST",
                                    url: widgetSaveUrl,
                                    data: { CustomValues: inputVal, WidgetZoneID: zoneId, WidgetTemplateID: widgetTemplateId, WidgetID: widgetId },
                                    success: function (data) {
                                        if (data.Result == "Success") {
                                            if (data.Mode == "ADD") {
                                                intrface.addWidget.apply($this, [data.WidgetTemplateID, data.ZoneId, data.WidgetId]);
                                            }
                                            else if (data.Mode == "EDIT") {
                                                intrface.removeAndAddWidget.apply($this, [data.OldWidgetID, data.WidgetTemplateID, data.ZoneId, data.WidgetId]);
                                            }
                                            $(customWidgetModal).dialog("destroy");
                                        }
                                        else if (data.Result == "Fail") {
                                            $(buttonDomElement).attr('disabled', false);
                                            $(customWidgetModal).find('#ErrorText').text(data.ErrorMes);
                                            $(customWidgetModal).find('#ErrorText').removeClass("ErrorTextNotVisible");
                                            $(customWidgetModal).find('#ErrorText').addClass("ErrorTextVisible");
                                        }
                                    },
                                    error: function (req, status, error) {
                                        alert("ERROR_SAVE_WIDGET --> GET WIDGET TEMPLATE");
                                    }
                                });
                            }
                            else if (data.Result == 'FAIL') {
                                alert(data.StatusMessage);
                            }
                        },
                        error: function (req, status, error) {
                            alert("ERROR_SAVE_WIDGET --> GET WIDGET TEMPLATE");
                        }
                    });
                }
            }
        });
        $(customWidgetModal).html(editWidgetUI.find(".customwidgetBody"));
    }

    var intrface = {
        // initializes the plugin
        init: function (options) {
            var settings = $.extend(true, {}, _private.defaults, options);
            var $this = $(this),
                data = $this.data(_private.dataKeyPlugin);

            if (!data) {
                $this.data(_private.dataKeyPlugin, settings);

                //alert($this.find("." + settings.zoneClass).length);

                zones = [];
                $this.find("." + settings.zoneClass).each(function (index, element) { // go through all zones in DOM and make object of type zoneSetting add them to array zones
                    var zone = $(this);
                    var allowlist = zone.attr(_private.allowedWidgets),
                    zoneId = zone.attr(_private.itemId),
                    zoneSettings = { itemId: zoneId, allowedWidgets: allowlist, widgets: [] };

                    //  alert("allowlist = " + allowlist);

                    zone.find("." + settings.widgetClass).each(function (index, element) { // go through all widgets in DOM and make object of tyoe widgetSettings and to zone
                        var widget = $(this),
                        widgetItemId = widget.attr(_private.itemId),
                        widgetSequence = widget.attr(_private.sequence),
                        widgetTemplateId = widget.attr(_private.widgetTemplateId),
                        widgetAllowMultiple = widget.attr(_private.allowMultiple),
                        widgetSettings = { itemId: widgetItemId, sequence: widgetSequence, templateId: widgetTemplateId, allowMultiple: widgetAllowMultiple };
                        if (widget.find(".customValues").length > 0) {
                            widget.find("." + settings.WidgetHeaderTextClass).text(widget.find(".customValues").val());
                        }
                        _addWidgetToZone(widgetSettings, zoneSettings);
                    });
                    zones[index] = zoneSettings;
                });
                // alert(zones.length);
                $this.data(_private.dataKeyZone, zones); //add the array zones (which has elements of zoneSettings) to data so that we can load it in other part of plugin

                //this call will make ui non editable
                intrface.disableEditing.apply(this);

                // bind event handler for remove widget button
                $("." + settings.removeWidgetClass).live('click', function (event) {
                    var widget = $(this).closest("." + settings.widgetClass);
                    var widgetID = widget.attr(_private.itemId);
                    intrface.removeWidget.apply($this, [widgetID]);
                });

                // bind event handler to expand and collapse widget
                $("." + settings.toggleDisplayClass).live('click', function (event) {
                    _toggleWidgetDisplay($(this), settings);
                });

                // bind event to show list of widgets which can be added to zone            
                $("." + settings.addWidgetClass).click(function () {
                    $(this).hide();
                    var chooseWidget = $(this).siblings("." + settings.chooseWidgetClass);
                    chooseWidget.show();
                });

                // bind event to hide the div (which displays list of widgets) on mouse out
                $("." + settings.chooseWidgetClass).mouseleave(function () {
                    $(this).hide();
                    $(this).siblings("." + settings.addWidgetClass).show();
                });


                // bind event,  to add the chosen widget 
                $("." + settings.widgetDisplayItemClass).live("click", function () {
                    var widgetType = $(this).attr("widgetType"),
                    zone = $(this).closest("." + settings.zoneParentClass).find("." + settings.zoneClass),
                    zoneId = zone.attr(_private.itemId),
                    widgetInsertSlot = $(this).closest("." + settings.chooseWidgetClass);
                    PxPage.Loading(zoneId); //Load the spinner
                    intrface.addWidget.apply($this, [widgetType, zoneId, "NotKnownYet"]);
                    widgetInsertSlot.hide();
                    widgetInsertSlot.siblings("." + settings.addWidgetClass).show();
                });

                var zoneSelector = "." + settings.zoneClass;

                // implement drag and drop on zones  
                $(zoneSelector).sortable({
                    connectWith: '.zoneParent', //zoneSelector,
                    handle: "." + settings.widgetToolbarClass,
                    placeholder: 'placeholder',
                    forcePlaceholderSize: true,
                    stop: function (event, ui) {
                        var result = _moveWidget.apply($this, [ui, ui.item.parent().attr(_private.itemId)]);
                        if (result == false) {
                            $(this).sortable('cancel');
                        }
                    }
                });


                // bind event to show edit widget screen
                $("." + "blockWidgetUI").live("click", function () {
                    //alert ('widget edit');
                    //_private.defaults.zoneClass

                    var blockDiv = $(this);
                    blockDiv.removeClass("blockWidgetUI_ON");
                    blockDiv.addClass("widgetBlocker_OFF");

                    widget = blockDiv.closest("." + settings.widgetClass),
                    wigetId = widget.attr(_private.itemId),
                    wTemplateId = widget.attr(_private.widgetTemplateId),
                    zoneId = _getZoneForWidget(wigetId, $this).itemId;
                    PxPage.Loading(zoneId); //Load the spinner
                    var bodyPostContent = $.ajax({
                        type: "POST",
                        url: settings.ajaxUrlEditWidget,
                        data: { widgetId: wigetId },
                        success: function (widgetEditHtml) {
                            _displayEditWidgetDialog.apply($this, [widgetEditHtml, zoneId, wTemplateId, wigetId]);
                        }

                    });

                });

                // bind event to show the prompt "this widget cannot be edited", also if widget is editable, just block ui 
                $("." + "widgetContents").live("mouseenter", function () {
                    if (settings.Mode != 'Edit') {
                        return;
                    }
                    var wContents = $(this),
                    widget = wContents.closest("." + settings.widgetClass);
                    attrEditable = widget.attr("iseditable");

                    var blockerDiv = null;

                    if (attrEditable == 'True') {
                        // alert("it is editable");
                        blockerDiv = wContents.find(".blockWidgetUI");
                        blockerDiv.addClass("blockWidgetUI_ON");
                        blockerDiv.removeClass("widgetBlocker_OFF");
                    }
                    else {
                        blockerDiv = wContents.find(".widgetBlocker");
                        blockerDiv.addClass("widgetBlocker_ON");
                        blockerDiv.removeClass("widgetBlocker_OFF");
                    }

                    var offSetCoordibates = wContents.offset()
                    hght = wContents.height(),
                    wdth = wContents.width();

                    blockerDiv.offset(offSetCoordibates);
                    blockerDiv.height(hght);
                    blockerDiv.width(wdth);

                    var divBlockMessage = blockerDiv.find("." + "widgetBlockMessage");
                    if (divBlockMessage) {
                        divBlockMessage.offset({ top: offSetCoordibates.top + hght / 2, left: offSetCoordibates.left + (wdth - divBlockMessage.width()) / 2 });
                    }
                });

                // bind event to remove the prompt "this widget cannot be edited"
                $("." + "widgetContents").live("mouseleave", function () {
                    if (settings.Mode != 'Edit') {
                        return;
                    }
                    var wContents = $(this),
                    blockerDiv = wContents.find(".widgetBlocker");
                    blockerDiv.addClass("widgetBlocker_OFF");
                    blockerDiv.removeClass("widgetBlocker_ON");

                    blockerDiv = wContents.find(".blockWidgetUI");
                    blockerDiv.addClass("widgetBlocker_OFF");
                    blockerDiv.removeClass("blockWidgetUI_ON");
                });
            }
            return $this;
        },

        //  testing function to make sure that initialize has set all the data properly
        print: function (zoneId) {
            var $this = $(this),
            zoneSettings = _getZone(zoneId, $this);

            if (zoneSettings) {
                var details = "<br /> zone id = " + zoneSettings.itemId + "<br />";
                details += " allowed widgets = " + zoneSettings.allowedWidgets;

                var len = zoneSettings.widgets.length;
                for (var i = 0; i < len; i++) {
                    var widgetDetails = (i + 1) + " widget itemId = " + zoneSettings.widgets[i].itemId;
                    widgetDetails += " sequence = " + zoneSettings.widgets[i].sequence;
                    widgetDetails += " templateId = " + zoneSettings.widgets[i].templateId;
                    widgetDetails += " allowMultiple = " + zoneSettings.widgets[i].allowMultiple;

                    details = details + "<br />" + widgetDetails;
                }

                return details;
            }
        },


        // adds widget to the zone
        // widgetTemplateId: template id of the widget that has to be added
        // zoneId: id of the zone where widget has to be added
        // widgetID: New widget Id, this will only come in case of custom widgets.
        // prevWidgetId: Previous widget Id. This will only come in case of Edit for custom widgets.
        // nextWidgetId: Next widget Id. This will only come in case of Edit for custom widgets.
        addWidget: function (widgetTemplateId, zoneId, widgetID, prevWidgetId, nextWidgetId) {
            //alert("widgetTemplateId:" + widgetTemplateId + " zoneId: " + zoneId + "widgetID :" + widgetID);
            var $this = $(this),
            settings = $this.data(_private.dataKeyPlugin),
            zoneSettings = _getZone(zoneId, $this);


            if (!zoneSettings.allowedWidgets || zoneSettings.allowedWidgets.indexOf(widgetTemplateId) == -1) {
                alert("widget is not allowed in zone");
                return;
            }


            // since this widget is allowed in zone we can proceed

            // check to see if widget is allowed more than once or not, and if the same widget is in the zone already.            
            var widget = _getWidgetByTemplateId(widgetTemplateId, zoneSettings);
            if (widget) {
                // if widget was found in zone, the we have to check whether it can be added more than once or not.
                if (widget.allowMultiple.toUpperCase() == "FALSE") {
                    alert("this widget is already in this zone, and cannot be added more than once");
                    return;
                }
            }

            var currentPageName = settings.pageName,
            targetId = _getZoneName(currentPageName, zoneSettings.itemId),
            widgetController = '',
            widgetActions = '',
            prevSeq = '',
            nextSeq = '';


            if (!prevWidgetId) {
                prevSeq = '';
            }
            else {
                prevSeq = $("#" + zoneId).find("." + settings.zoneClass).find("." + settings.widgetClass + "[" + _private.itemId + "='" + prevWidgetId + "']").attr("sequence");
            }

            if (!nextWidgetId) {
                nextSeq = $("#" + zoneId).find("." + settings.zoneClass).find("." + settings.widgetClass).first().attr("sequence");
                if (!nextSeq) {
                    nextSeq = '';
                }
            }
            else {
                nextSeq = $("#" + zoneId).find("." + settings.widgetClass + "[" + _private.itemId + "='" + nextWidgetId + "']").attr("sequence");
            }

            // ajax call to get widget from server and add it to DOM
            var bodyPostContent = $.ajax({
                type: "POST",
                url: settings.ajaxUrlAddWidget,
                data: { pageName: currentPageName, zoneID: targetId, templateID: widgetTemplateId, prevSequence: prevSeq, nextSequence: nextSeq, newWidgetID: widgetID },
                success: function (widgetHtml) {

                    // add widget to the zone array
                    var widget = $(widgetHtml),
                    widgetItemId = widget.attr(_private.itemId),
                    widgetSequence = widget.attr(_private.sequence),
                    widgetAllowMultiple = widget.attr(_private.allowMultiple),
                    widgetSilentCreation = widget.attr(_private.widgetSilentCreation),
                    dialogWidth = widget.find("." + settings.customEditWidgetClass).attr(_private.dialogWidth),
                    dialogHeight = widget.find("." + settings.customEditWidgetClass).attr(_private.dialogHeight),
                    dialogMinWidth = widget.find("." + settings.customEditWidgetClass).attr(_private.dialogMinWidth),
                    dialogMinHeight = widget.find("." + settings.customEditWidgetClass).attr(_private.dialogMinHeight),
                    dialogTitle = widget.find("." + settings.customEditWidgetClass).attr(_private.dialogTitle);

                    if (widgetSilentCreation == "true") { // Example: Announcement Widget
                        var widgetSettings = { itemId: widgetItemId, sequence: widgetSequence, templateId: widgetTemplateId, allowMultiple: widgetAllowMultiple };
                        _addWidgetToZone(widgetSettings, zoneSettings);
                        if ((!prevWidgetId) && (!nextWidgetId)) {
                            $("#" + zoneId).find("." + settings.zoneClass).prepend(widgetHtml);
                            $("#" + zoneId).find("." + settings.zoneClass).find("." + settings.widgetClass).first().find("." + settings.WidgetHeaderTextClass).text(widget.find(".customValues").val());
                        }
                        else if ((prevWidgetId) && (!nextWidgetId)) {
                            $("#" + zoneId).find("." + settings.zoneClass).find("." + settings.widgetClass + "[" + _private.itemId + "='" + prevWidgetId + "']").after(widgetHtml);
                            $("#" + zoneId).find("." + settings.zoneClass).find("." + settings.widgetClass + "[" + _private.itemId + "='" + prevWidgetId + "']").next().find("." + settings.WidgetHeaderTextClass).text(widget.find(".customValues").val());
                        }
                        else if ((!prevWidgetId) && (nextWidgetId)) {
                            $("#" + zoneId).find("." + settings.zoneClass).find("." + settings.widgetClass + "[" + _private.itemId + "='" + nextWidgetId + "']").before(widgetHtml);
                            $("#" + zoneId).find("." + settings.zoneClass).find("." + settings.widgetClass + "[" + _private.itemId + "='" + nextWidgetId + "']").prev().find("." + settings.WidgetHeaderTextClass).text(widget.find(".customValues").val());
                        }
                        else if ((prevWidgetId) && (nextWidgetId)) {
                            $("#" + zoneId).find("." + settings.zoneClass).find("." + settings.widgetClass + "[" + _private.itemId + "='" + nextWidgetId + "']").before(widgetHtml);
                            $("#" + zoneId).find("." + settings.zoneClass).find("." + settings.widgetClass + "[" + _private.itemId + "='" + nextWidgetId + "']").prev().find("." + settings.WidgetHeaderTextClass).text(widget.find(".customValues").val());
                        }
                        PxPage.Loaded(zoneId);
                    }
                    else {          // Example : Custom Rss Feed Widget
                        _displayEditWidgetDialog.apply($this, [widget, zoneId, widgetTemplateId]);
                        PxPage.Loaded(zoneId);
                    }
                },
                error: function (req, status, error) {
                    alert("ERROR_POST");
                }
            });
        },

        removeAndAddWidget: function (oldWidgetId, widgetTemplateId, zoneId, widgetID) {
            var $this = $(this);
            var settings = $this.data(_private.dataKeyPlugin);
            var prevWidgetId = $("." + settings.widgetClass + "[" + _private.itemId + "='" + oldWidgetId + "']").prevAll("." + settings.widgetClass).attr("itemid"),
            nextWidgetId = $("." + settings.widgetClass + "[" + _private.itemId + "='" + oldWidgetId + "']").nextAll("." + settings.widgetClass).attr("itemid");
            intrface.removeWidget.apply($this, [oldWidgetId]);
            intrface.addWidget.apply($this, [widgetTemplateId, zoneId, widgetID, prevWidgetId, nextWidgetId]);
        },

        removeWidget: function (widgetId) {
            var $this = $(this);

            var settings = $this.data(_private.dataKeyPlugin);
            var bodyPostContent = $.ajax({
                type: "POST",
                url: settings.ajaxUrlRemoveWidget,
                data: { widgetId: widgetId, pageName: settings.pageName },
                success: function (msg) {
                    $("." + settings.widgetClass + "[" + _private.itemId + "='" + widgetId + "']").remove();
                },
                error: function (req, status, error) {
                    alert("ERROR_POST_DELETE");
                }
            });
            _removeWidgetFromZone(widgetId, null, $this);
        },

        enableEditing: function () {
            var $this = $(this),
            settings = $this.data(_private.dataKeyPlugin);
            $("." + settings.addWidgetClass).show();
            $("." + settings.widgetToolbarClass).show();

            settings.Mode = 'Edit';
            $this.data(_private.dataKeyPlugin, settings);
        },

        disableEditing: function () {
            var $this = $(this);
            var settings = $this.data(_private.dataKeyPlugin);
            $("." + settings.addWidgetClass).hide();
            $("." + settings.widgetToolbarClass).hide();

            settings.Mode = 'View';
            $this.data(_private.dataKeyPlugin, settings);
        },

        // destroys the plugin instances
        destroy: function () {
            return this.each(function () {
                $(this).unbind(bindSuffix);
            });
        }
    };

    // registers the plugin
    $.fn.PageLayout = function (method) {
        if (intrface[method]) {
            return intrface[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return intrface.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _private.pluginName);
        }
    };
} (jQuery))