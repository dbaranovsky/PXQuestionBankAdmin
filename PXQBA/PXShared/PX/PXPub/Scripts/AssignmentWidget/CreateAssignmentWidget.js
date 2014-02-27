//Fudge
var PxCreateNewAssignment = function ($) {
    //static class properties
    var _static = {
        initialized: false,
        defaults: {
            dlapPatentId: 'PX_MANIFEST',
            parentid: 'PX_TOC'
        },
        events: {
            assignExisting: 'assignexisting',
            folderSelected: 'folderselected',
            cancelContentCreation: 'cancelcontent'
        },
        sel: {
            assignButton: '#newAssignment',
            assigncontrol: '#assignContent',
            dropdown: '#assignContent select',
            dropdownandlabel: '#assignContent select, #assignContent .lblDialog',
            nextbutton: '#assignContent #btnNext',
            cancelbutton: '#assignContent #btnCancel',
            radiochoices: '#assignContent input',
            radioselected: '#assignContent input:checked',
            radioassign: '#assignContent input[value="assign"]',
            radiocreate: '#assignContent input[value="create"]'
        },
        fn: {
            //Initialize the dialog.
            init: function () {

                //Listen to a click on the "Create Assignment" button and set the hash to its "ref" attribute
                $(_static.sel.assignButton + ' *[ref]').bind('click', _static.fn.setHash);

                //Initializes hash routes for this control
                if (!_static.initialized) {
                    PxRoutes.AddComponentRoute('content', null, _static.fn.handleRoute);
                    _static.initialized = true;
                }
            },
            //Sets the hash for controls setup with the "ref" attribute
            setHash: function (event) {
                var ref = $(event.target).closest('*[ref]').attr('ref');
                HashHistory.SetHash(ref);
            },
            //Handles any routes that would lead to an external control, like viewAssignment, and fires an appropriate event for a product to handle
            handleRoute: function (state, func, args) {
                if (CreateAssignmentRoutes[func] !== undefined) {
                    CreateAssignmentRoutes[func].apply(this, [args]);
                }
            },
            //Fires an event to set the assigned folder to an empty string
            //TODO: Find better way to clear the product listening to the folder assign event
            cancelContentCreate: function () {
                $(PxPage.switchboard).trigger(_static.events.folderSelected, ['']);
                $(PxPage.switchboard).trigger(_static.events.cancelContentCreation);
            },
            //I only care for this event if it was initiated by this dialog.  this is ugly and awful
            handelCancelFromSwitchboard: function () {
                _static.fn.cancelContentCreate();
                $(PxPage.switchboard).unbind('removenewnode', _static.fn.handelCancelFromSwitchboard);
            },
            //Displays the dialog that presents a radio button for assigning existing content or creating a new
            //piece of content
            displayAssignContentDialog: function() {
                $.ajax({
                    url: PxPage.Routes.create_assignment,
                    type: "POST",
                    data: {
                    },
                    success: function (response) {
                        $(response).dialog({
                            width: 400,
                            height: 200,
                            minWidth: 400,
                            minHeight: 200,
                            modal: true,
                            draggable: false,
                            closeOnEscape: true,
                            dialogClass: 'hide-titlebar',
                            open: function (event, ui) {
                                //Handle the change of the radio buttons
                                $(_static.sel.radiochoices).bind('change', _static.fn.onRadioChange);
                                $(_static.sel.dropdown).bind('change', _static.fn.onDropDownChange);

                                //Listens to a click of a control within the dialog that has a 'ref' attribute and set the hash to that value
                                $(_static.sel.assigncontrol + ' *[ref]').bind('click', _static.fn.setHash);

                                $(_static.sel.radioselected).prop('checked', false);
                                $(_static.sel.dropdownandlabel).hide();
                                $(_static.sel.nextbutton).attr('disabled', '');
                            }
                        });
                    }
                });
            },
            displayCreateContentDialog: function (parentid, folderid) {
                PxContentTemplates.ContentCreationTemplates(PxPage.Context.course_type, function () {
                    PxPage.Loaded();

                    $('.nonmodal-unblock-action').click(function () {
                        _static.fn.cancelContentCreate();
                        //Can't set hash using the "rel" attribute because these dialogs are using in other applications
                        HashHistory.SetHash('#state/');
                    });


                    // Add alternating class to templateItems
                    $('.templateLineItem').filter(function () {
                        return $(this).css('display') != 'none';
                    }).each(function (index) {
                        $(this).addClass("row" + (index % 2 == 0).toString().replace("t", "T").replace("f", "F"));
                    });
                    // Add class to keep create new modals consistent across products
                    $('.nonmodal-window').addClass('ac-create-content');

                    //Listen to the click of a template type in the list.  Again, can't use hash because this is used
                    //in other applications
                    $('.templateLineItem').bind('click', function (event) {
                        var item = event.target;
                        if (parentid !== undefined && parentid !== '') {
                            HashHistory.SetHash('#state/content/save?templateid=' + $(item).attr('itemid') + '&parentid=' + parentid + '&folderid=' + folderid);
                        } else {
                            HashHistory.SetHash('#state/content/save?templateid=' + $(item).attr('itemid') + '&parentid=' + _static.defaults.parentid + '&folderid=' + folderid);
                        }
                    });
                }, null, true, 800);
            },
            //Displays a new px content type that has been created based on the item selected by the user
            displaySelectedTemplate: function (templateid, parentid, folderid) {

                PxPage.Loading("nonmodal-window");

                //Check to make sure a valid content template was selected.
                if (!templateid) {
                    alert('Invalid Template Selection');
                    PxPage.Loaded("nonmodal-window");
                    return false;
                }

                PxContentTemplates.CreateItemFromTemplate(templateid, function (item) {
                    var itemUrl = PxPage.Routes.display_content + "/" + item.id + "?mode=Edit&includeNavigation=false&isBeingEdited=true";

                    $.get(itemUrl, null, function (response) {
                        $(response).PxNonModal({
                            title: 'Create New',
                            widthOverride: 710,
                            heightOverride: 440,
                            topOverride: 250,
                            onCompleted: function () {
                                //Some styling
                                $('#nonmodal-content').css('overflow','hidden');
                                $('#nonmodal-content').find('.content-title').hide();
                                $('#nonmodal-content').find('.doc-collection-content-view').hide();
                                $('#nonmodal-content').find('.sub-title').parent("li").hide();
                                $('#nonmodal-content').find('.sub-title').hide();
                                $('#nonmodal-window').find('#assign').css('margin-left', '15px');
                                $('#nonmodal-window').find('#title-text').css('margin-bottom', 0);
                                $('#nonmodal-window').find('#title-text').css('margin-bottom', 0);
                                $('#nonmodal-content').find('#Content_Title').css({
                                    'padding':'5px',
                                    'border' : '1px solid #ccc'
                                });
                                $('#contentwrapper').find('#form').css({
                                    'padding':0,
                                    'margin':'8px 20px 0',
                                    'overflow':'hidden'
                                });
                                $('#nonmodal-content').find('#editForm').css({
                                    'padding':0,
                                    'margin':'0 0 10px 0'
                                });
                                $('#nonmodal-content').find('.create-new-btn-wrapper').css({
                                    'background' : 'none',
                                    'border': 0,
                                    'margin' : 0,
                                    'padding' : '20px 0 0 4px'
                                });
                                $('.create-new-btn-wrapper').find('input').addClass("savebtn").css('margin', 0);
                                if($('#nonmodal-content').find('#Content_Title').val() == 'Untitled Document Collection'){
                                    $('#nonmodal-content').find('.create-new-btn-wrapper').css({
                                        'padding':0
                                    });
                                }

                                // change the parent id from PX_TEMP parent id, so that when clicking
                                // save button we can create a new item with this parent id.
                                $('#nonmodal-content').find('#Content_DefaultCategoryParentId').val(parentid);
                                $('#nonmodal-content').find('#Content_ParentId').val(_static.defaults.dlapPatentId);
                                //$('#nonmodal-content').find('#Content_Title').val($('#nonmodal-content').find('#Content_Title').val());
                                
                                $('#nonmodal-content').find('#Content_Title').attr("placeholder", $('#nonmodal-content').find('#Content_Title').val());
                                $('#nonmodal-content').find('#Content_Title').val('');

                                $('#nonmodal-content').find('#Content_SyllabusFilter').val(parentid);

                                $('#Content_SourceTemplateId').val(templateid);
                                //TODO: Find better way to clear the product listening to the folder assign event
                                $(PxPage.switchboard).bind('removenewnode', _static.fn.handelCancelFromSwitchboard);
                                $('.nonmodal-unblock-action').click(function () {
                                    _static.fn.cancelContentCreate();

                                    $.ajax({
                                        type: 'POST',
                                        url: PxPage.Routes.delete_item,
                                        data: {
                                            id: item.id,
                                            parentid: parentid
                                        },
                                        success: function () {
                                            PxPage.log('Cancel successful');
                                            HashHistory.SetHash('#state/');
                                        },
                                        error: function (message) {
                                            PxPage.log('Cancel failed: ' + message);
                                            HashHistory.SetHash('#state/');
                                        }
                                    });
                                });

                                //So whoever is handling adding the item to an assignment knows what folder to add it to.
                                $(PxPage.switchboard).trigger(_static.events.folderSelected, [folderid]);
                                PxPage.Loaded("nonmodal-window");
                            }
                        });
                        PxPage.Update();
                    });
                }, parentid);
            },
            //Handles changes to the radio by hiding/showing the dropdown and updating the ref hash url attribute on the "Next" button
            onRadioChange: function () {
                $(_static.sel.nextbutton).removeAttr('disabled');
                $(_static.sel.radioselected).each(function () {
                    if ($(this).attr('value') === 'create') {
                        $(_static.sel.dropdownandlabel).show();
                        $(_static.sel.nextbutton).attr('ref', 'state/content/create?folderid=' + $(_static.sel.dropdown + ':visible').val());
                    } else {
                        $(_static.sel.dropdownandlabel).hide();
                        $(_static.sel.nextbutton).attr('ref', 'state/toc/displayContentItem?itemid=&displayAssign=true');
                    }
                });
            },
            //Updates the ?folder= query parameter on the hash url on the "Next" button
            onDropDownChange: function () {
                $(_static.sel.nextbutton).attr('ref', 'state/content/create?folderid=' + $(_static.sel.dropdown + ':visible').val());
            }
        }
    };
    return {
        //To be used by a product to get the assignment folder set on a new piece of content
        FolderLocation: '',
        Init: function () {
            _static.fn.init();
        },
        AssignContentDialog: function() {
            _static.fn.displayAssignContentDialog();
        },
        CreateContentDialog: function (parentid, folderid) {
            _static.fn.displayCreateContentDialog(parentid, folderid);
        },
        SaveContentDialog: function (templateid, parentid, folderid) {
            _static.fn.displaySelectedTemplate(templateid, parentid, folderid);
        },
        //Cleans up any dialogs that may be open
        CleanupDialogs: function () {
            //Assign dialog
            $(_static.sel.assigncontrol).dialog('destroy');
            $(_static.sel.assigncontrol).remove();
        }
    };
} (jQuery);