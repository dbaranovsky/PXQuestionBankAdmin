var TemplatePicker = function ($) {

    // static data
    var _static = {

        sel: {
            line: ".templateLineItem",
            title: '.item-title',
            description: '.item-description',
            details: ".details",
            detailsTitle: '.title',
            detailsDescription: '.description',
            createNew: '.create-done-button',
            list: '.template-list'
        },

        // private functions
        fn: {

            // sets right panel content (title + description) based on selected list element
            setDetails: function (event) {
                var details = $(_static.sel.details),
                    listItem = $(event.currentTarget);
                $(_static.sel.list).find('.selected').removeClass('selected');
                listItem.addClass('selected');
                details.find(_static.sel.detailsTitle).text( listItem.text() );
                details.find(_static.sel.detailsDescription).html( listItem.find(_static.sel.description).val() );
            },

            // bound to list item click or create new button
            // AddFromSelectedTemplate creates new content based on the item in list with `selected` class
            createItem: function () {
                if ($(PxPage.switchboard).data('events').AddFromSelectedTemplate) {
                    $(PxPage.switchboard).trigger('AddFromSelectedTemplate');
                }
                else {
                    PxContentTemplates.AddFromSelectedTemplate();                        
                }
            },

            BindControls: function () {

                $(_static.sel.list)
                    .on('touchstart mouseenter', _static.sel.line, _static.fn.setDetails)
                    .on('click', _static.sel.line, _static.fn.createItem);

                if (PxPage.TouchEnabled()) {
                    $(_static.sel.createNew).on('click', _static.fn.createItem).show();
                    $(_static.sel.list).off('click');
                }
                else {
                    $(_static.sel.createNew).hide();
                }

            }
        }
    };

    // static functions
    return {
        Init: function () {
            _static.fn.BindControls();
        },
        privateTest: function () {
            return _static;
        }
    };

} (jQuery)