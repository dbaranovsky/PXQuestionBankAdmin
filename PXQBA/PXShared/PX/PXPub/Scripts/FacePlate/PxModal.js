// PxModal
//
// This plugin is responsible for the client-side templates for
// Px Modal dialogs
var PxModal = function ($) {
    // privately scoped data used by the plugin
    var _static = {
        pluginName: "PxModal",
        dataKey: "PxModal",
        bindSuffix: ".PxModal",
        dataAttrPrefix: "data-ud-",
        //private vars
        modal: null,
        //default option values
        defaults: {
            title: '',
            width: 560,
            height: 'auto', //pixels value, "auto"
            content: '',
            classes: '',
            dialogClass: 'px-dialog',
            showButtons: true,
            buttons: {
                Save: {
                    text: "Save",
                    fn: function () { _static.fn.submit(); },
                    show: false,
                    "class": 'button primary large',
                    html: "<input type='button' class='save' value='Save' />"
                },
                Yes: {
                    text: "Yes",
                    fn: function () { _static.fn.submit(); },
                    show: false,
                    "class": 'button primary large',
                    html: "<input type='button' class='yes' value='Yes' />"
                },
                No: {
                    text: "No",
                    fn: function () { _static.fn.cancelButton(); },
                    show: false,
                    "class": 'button primary large',
                    html: "<input type='button' class='no' value='No' />"
                },
                Other : {
                    fn: function () {  },
                    show: false,
                    "class": 'button large',
                    html: "<input type='button' class='other' value='Other' />"
                      },
                Cancel: {
                    text: "Cancel",
                    fn: function () { _static.fn.cancel(); },
                    show: false,
                    "class": 'link',
                    html: "<a href='#' class='dialogClose'>Cancel</a>"
                }
            },
            onOpen: function () {
            },
            onSubmit: function () {

            },
            onCancel: function () {

            },
            onClose: function () {

            },
            onResize: function () {
            }
        },
        //class names used by plugin
        css: {
            categoryTitle: "nav-category-title",
            active: "active",
            itemCount: "nav-category-item-count",
            category: "nav-category",
            fauxTree: "faux-tree"
        },
        // jquery selectors used for commonly accessed elements
        sel: {

        },
        //private functions
        fn: {
            initModal: function (modal) {
                var data = _static.modal.data(_static.dataKey);
                var jqueryDialogOptions = {
                    modal: true,
                    draggable: false,
                    closeOnEscape: true,
                    width: data.settings.width,
                    dialogClass: data.settings.dialogClass,
                    classes: data.settings.classes,
                    height: data.settings.height,
                    showButtons: data.settings.showButtons,
                    resizable: false,
                    autoOpen: false,
                    title: data.settings.title,
                    buttons: data.settings.buttons
                };
                var tag = $("<div></div>");
                $(tag).addClass(data.settings.classes);
                tag.append(data.settings.content);

                // Search the Btns Object, if show is set to true return a new object of buttons
                jqueryDialogOptions.buttons = $();
                $.each(data.settings.buttons, function (index, button) {
                    if (button.show) {
                        var btnFn = this.click;
                        this.click = function () {
                                        if ($.isFunction(btnFn)) {
                                            btnFn();
                                        }
                                        $(this).dialog("close");
                                     };
                        jqueryDialogOptions.buttons = jqueryDialogOptions.buttons.add(this);
                    }
                });
                data.jqueryDialogOptions = jqueryDialogOptions;
                _static.modal.html = tag;
            },
            openModal: function (modal) {
                var data = _static.modal.data(_static.dataKey);
                var options = data.jqueryDialogOptions;
                var html = _static.modal.html;

                $(html).dialog({
                    modal: _static.modal,
                    title: options.title,
                    draggable: options.draggable,
                    closeOnEscape: options.closeOnEscape,
                    dialogClass: options.dialogClass,
                    classes: options.classes,
                    width: options.width,
                    height: options.height,
                    showButtons: options.showButtons,
                    resizable: options.resizable,
                    autoOpen: options.autoOpen,
                    buttons: options.buttons,
                    close: function () {
                        $(this).dialog('destroy').remove();
                    }
                });
                // Find the outer wrapper and add our string of classes
                if (options.classes) {
                    $('.px-dialog').addClass(options.classes);
                }
                $(html).dialog('open');
            },
            closeModal: function () {
            },
            submit: function () {
            },
            cancelButton: function () { },
            cancel: function () { },
            pxAlertModal: function (modal) {
                var initTemplate = {
                    title: "Really do this?",
                    content: "<ul><li>Consequence 1</li><li>Consequence 2</li></ul>",
                    classes: 'pxAlert',
                    height: 'auto',
                    buttons: {
                       Cancel: {
                            show: true
                        },
                       Save: {
                          show:true
                      },
                        Other : {
                          show: true
                      }
                    }
                };
                return initTemplate;
            },
            pxFixedHeightModal: function (modal) {
                var initTemplate = {
                    title: "Default Fixed Height Modal",
                    content: "",
                    classes: 'fixed-height',
                    height: 600,
                    buttons: {
                       Cancel: {
                            show: true
                        },
                       Save: {
                          show:true
                      },
                        Other : {
                          show: true
                      }
                    }
                };
                return initTemplate;
            },
            pxAutoHeightModal: function (modal) {
                var initTemplate = {
                    title: "Default Fixed Height Modal",
                    content: "",
                    classes: 'auto-height',
                    height: "auto"
                };
                return initTemplate;
            },
            pxCenterDialog: function (tag, overrideHeight) {
                tag.dialog("open");
                tag.dialog("widget").css("position", "fixed");
                var height = (overrideHeight) ? overrideHeight : tag.dialog("widget").height();
                tag.dialog("widget").css("top", ($(window).height() / 2 - (height / 2)) - 50);
            },
            pxConfirmation: function (options) {
                _static.defaults.buttons = $();

                var settings = $.extend(true, {}, _static.defaults, options);

                var $this = $('<div id="dialogConfirm"></div>');
                _static.modal = $this;
                _static.modal.data(_static.dataKey, {
                    target: $this,
                    settings: settings
                });
                _static.fn.initModal($this);
                _static.fn.openModal($this);
                return $this;
            }
        }
    },
    api = {
        init: function (options) {
            var settings = $.extend(true, {}, _static.defaults, options),
                $this = this;
            _static.modal = $this;
            _static.modal.data(_static.dataKey, {
                target: $this,
                settings: settings
            });
            _static.fn.initModal($this);
            return $this;
        },
        openModal: function () {
            _static.fn.openModal(this);
        },
        closeModal: function () {

        },
        pxAlertModal: function (options) {
            var settings = _static.fn.pxAlertModal(this);
            settings = $.extend(true, {}, _static.defaults, settings),
            settings = $.extend(true, {}, settings, options);
            var $this = this;
                
            _static.modal = $this;
            _static.modal.data(_static.dataKey, {
                target: $this,
                settings: settings
            });
            _static.fn.initModal($this);
            _static.fn.openModal($this);
            return $this;
        },
        pxFixedHeightModal: function (options) {
            var settings = _static.fn.pxFixedHeightModal(this);
            settings = $.extend(true, {}, _static.defaults, settings),
            settings = $.extend(true, {}, settings, options);
            var $this = this;
                
            _static.modal = $this;
            _static.modal.data(_static.dataKey, {
                target: $this,
                settings: settings
            });
            _static.fn.initModal($this);
            _static.fn.openModal($this);
            return $this;
        },
        pxAutoHeightModal: function (options) {
            var settings = _static.fn.pxAutoHeightModal(this);
            settings = $.extend(true, {}, _static.defaults, settings),
            settings = $.extend(true, {}, settings, options);
            var $this = this;
                
            _static.modal = $this;
            _static.modal.data(_static.dataKey, {
                target: $this,
                settings: settings
            });
            _static.fn.initModal($this);
            _static.fn.openModal($this);
            return $this;
        }
    };

    // Associate the plugin with jQuery
    $.fn.PxModal = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === "object" || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error("Method " + method + " does not exist in PxModal." + _static.pluginName);
            return null;
        }
    };

    return {
        CreateConfirmDialog: _static.fn.pxConfirmation,
        CenterDialog: _static.fn.pxCenterDialog
    };
} (jQuery);