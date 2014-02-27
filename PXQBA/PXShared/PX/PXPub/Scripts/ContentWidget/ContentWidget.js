$.ui.plugin.add("resizable", "iframeFix", {
    start: function (event, ui) {
        var o = $(this).data('resizable').options;
        $(o.iframeFix === true ? "iframe" : o.iframeFix).each(function () {
            $('<div class="ui-resizable-iframeFix" style="background: #fff;"></div>').css({
                width: this.offsetWidth + "px",
                height: this.offsetHeight + "px",
                position: "absolute",
                opacity: "0.001",
                zIndex: 1000
            }).css($(this).offset()).appendTo("body");
        });
    },
    stop: function (event, ui) {
        $("div.ui-resizable-iframeFix").each(function () {
            this.parentNode.removeChild(this);
        }); //Remove frame helpers
    }
});

$.ui.plugin.add("resizable", "alsoResizeReverse", {

    start: function (event, ui) {

        var self = $(this).data("resizable"),
            o = self.options;

        var _store = function (exp) {
            $(exp).each(function () {
                $(this).data("resizable-alsoresize-reverse", {
                    width: parseInt($(this).width(), 10),
                    height: parseInt($(this).height(), 10),
                    left: parseInt($(this).css('left'), 10),
                    top: parseInt($(this).css('top'), 10)
                });
            });
        };

        if (typeof (o.alsoResizeReverse) == 'object' && !o.alsoResizeReverse.parentNode) {
            if (o.alsoResizeReverse.length) {
                o.alsoResize = o.alsoResizeReverse[0];
                _store(o.alsoResizeReverse);
            } else {
                $.each(o.alsoResizeReverse, function (exp, c) {
                    _store(exp);
                });
            }
        } else {
            _store(o.alsoResizeReverse);
        }
    },

    resize: function (event, ui) {
        var self = $(this).data("resizable"),
            o = self.options,
            os = self.originalSize,
            op = self.originalPosition;

        var delta = {
            height: (self.size.height - os.height) || 0,
            width: (self.size.width - os.width) || 0,
            top: (self.position.top - op.top) || 0,
            left: (self.position.left - op.left) || 0
        },

            _alsoResizeReverse = function (exp, c) {
                $(exp).each(function () {
                    var el = $(this),
                        start = $(this).data("resizable-alsoresize-reverse"),
                        style = {},
                        css = c && c.length ? c : ['width', 'height', 'top', 'left'];

                    $.each(css || ['width', 'height', 'top', 'left'], function (i, prop) {
                        var sum = (start[prop] || 0) - (delta[prop] || 0); // subtracting instead of adding
                        if (sum && sum >= 0) style[prop] = sum || null;
                    });

                    //Opera fixing relative position
                    if (/relative/.test(el.css('position')) && $.browser.opera) {
                        self._revertToRelativePosition = true;
                        el.css({
                            position: 'absolute',
                            top: 'auto',
                            left: 'auto'
                        });
                    }

                    el.css(style);
                });
            };

        if (typeof (o.alsoResizeReverse) == 'object' && !o.alsoResizeReverse.nodeType) {
            $.each(o.alsoResizeReverse, function (exp, c) {
                _alsoResizeReverse(exp, c);
            });
        } else {
            _alsoResizeReverse(o.alsoResizeReverse);
        }
    },

    stop: function (event, ui) {
        var self = $(this).data("resizable");

        //Opera fixing relative position
        if (self._revertToRelativePosition && $.browser.opera) {
            self._revertToRelativePosition = false;
            el.css({
                position: 'relative'
            });
        }

        $(this).removeData("resizable-alsoresize-reverse");
    }
});


//Set of helper static methods for applying client-side behaviors to the ContentWidget
var ContentWidget = function ($) {

    var _modes = {
        "None": 0,
        "Add": 2,
        "View": 4,
        "Edit": 8,
        "Settings": 16,
        "Results": 32,
        "Discussion": 64,
        "Assign": 128,
        "Rubrics": 256,
        "Rubrics & Learning Objectives": 256,
        "ReadOnly": 512
    };
    
    var _isInitialized = false;
    var _treeThemeUrl = false;
    var _childrenIds;
    var _pxapi = [];

    //This function sets up the PxAPI integration that allows content to communicate with 
    //PX cross domain
    var _initPxAPI = function () {
        var target = $(".document-viewer-frame-host");
        PxPage.log("initPxAPI");
        if (target.length <= 0) {
            PxPage.log("no PxAPI targets");
            return;
        }

        target.each(function (i, e) {
            PxPage.log("PxAPI target: " + $(e).attr("rel"));
            var frame = $(e).find("#document-body-iframe");

            if (frame.length === 0) {
                PxPage.log("PxAPI no iframe exists, creating new iframe");

                var rpcFunctions = {
                    local: {
                        openContent: function (args) {
                            PxPage.log(JSON.stringify(args));
                            PxPage.openContent(args);
                        },
                        argacomplete: function () {
                            ArgaApi.argacomplete();
                        },
                        contentResize: function (args) {
                            var width = args.width;
                            var height = args.height;

                            $(PxPage.switchboard).trigger("contentResize", [{ width: width, height: height }]);
                        },
                        updateTargetScoreInItem: function (entityId, itemId, targetScore) {
                            //identify to use common ajax object
                            $.post(PxPage.Routes.LearningCurve_UpdateTargetScoreInItem, { entityId: entityId, itemId: itemId, targetScore: targetScore },
                                function (response) {
                                    if (response.Status === 'Error')
                                        PxPage.log('update score error: ' + response.Message);
                                });
                        }
                    }
                };
                var isLcRpc = false;
                if ($.extend && $.fn.LearningCurve) {
                    var lcRpcFunctions = $.fn.LearningCurve('getLcRpcFunctions');
                    if (lcRpcFunctions && typeof (lcRpcFunctions) === 'object') {
                        rpcFunctions = $.extend(true, {}, lcRpcFunctions, rpcFunctions);
                        isLcRpc = true;
                    }
                }

                var rpc = new easyXDM.Rpc({
                    remote: $(e).attr("rel"),
                    container: $(e).attr("id"),
                    //Updated easyXDM moved this function to the config object to avoid double call to onload.
                    onLoad: function (event) {
                        $(PxPage.switchboard).trigger('document-body-iframe-loaded', [event.target]);
                        if ($(".document-viewer.allowComments").length) {
                            var iframe = $(e).find("#document-body-iframe");
                            ContentWidget.InitCommenting(iframe);
                        }
                    },
                    props: {
                        id: "document-body-iframe"
                    }
                }, rpcFunctions);


                _pxapi.push(rpc);

                if (isLcRpc)
                    $.fn.LearningCurve('setLcRpc', rpc);

                frame = $(e).find("#document-body-iframe");
                if (frame.length == 0) {
                    //failsave in case frame moves
                    frame = $("#document-body-iframe");
                }

                if (frame.length > 0) {
                    //commenting is disabled, autoheight won't work, set height to 100%
                    frame.attr("style", "height:100%; width:100%;");
                    frame.attr("class", "static-height");
                } else {
                    frame.attr("style", "min-height:100%;");
                    frame.attr("class", "autoHeight proxyFrame");
                }

            } else {
                PxPage.log("PxAPI already initialized");
            }
        });
    };

    //given itemid, loads up a list of children ids for infinite scroll
    var getChildrenIds = function (tocItemId, callback) {
        $.ajax({
            url: PxPage.Routes.list_children_ids,
            data: {
                'id': tocItemId
            },
            type: "POST",
            success: function (response) {
                _childrenIds = response;
                callback(response);
            }
        });
    };


    //starts at root and recursively descends the tree until the
    //last child is found
    var loadLastChild = function (root) {
        $(root).find('.children').first().load($(root).children('a.expand').attr('href'), function (response) {
            if ($(root).find('.children .section').length) {
                $(root).find('.children').show();

                var nr = $(root).find('.children .section').last();
                loadLastChild(nr);
            } else {
                var stopToggle = false;
                var disp = $(root).children('.children:visible').length;
                if (disp) stopToggle = true;

                $(root).children('a.expand').trigger('click', [stopToggle]);
            }
        });
    };

    var isProcessing = false;

    //given the section to load, the children of that section are loaded into the
    //TOC
    var expandSection = function (root) {
        var url = $(root).find('a.expand').first().attr('href');
        var children = $(root).find('.children').first();
        $(children).load(url, function (response) {
            $(children).show();
            if ($(root).find('.section-controls.allowed').length) {
                $(root).find('.section-controls.allowed').first().show();
            }
            $(root).find('ins.section-icon').first().removeClass('loading');

            if ($(root).find('.children').first().children().length == 0) {
                $(root).find('.children').first().append('<li class="section" style="width:100%;"><a class="expand" style="display:block;width: 100%;">&nbsp;</a></li>');
            }

            $('#toc > ul > .section').filter('.ui-sortable').sortable('refresh');
        });
    };

    var getActiveCategory = function () {
        var category = '';
        if ($("#toc-filters select option:selected").length) {
            category = $("#toc-filters select option:selected").attr("id");
        }

        return category;
    };

    //start the content creation process based on the dropped item
    var createContent = function (node, parentNode, prevNode, nextNode, source) {
        var type = node.type;
        var minSequence = '';
        var maxSequence = '';
        var parentId = parentNode != null ? parentNode.id : '';
        var category = getActiveCategory();

        if (prevNode != null) minSequence = prevNode.sequence;

        if (nextNode != null) maxSequence = nextNode.sequence;

        var args = {
            parentId: parentId,
            type: type,
            minSequence: minSequence,
            maxSequence: maxSequence,
            category: category
        };

        if ($(source).hasClass('add-eportfolio-folder') == false) {
            PxPage.Loading();
            $.post(PxPage.Routes.create_content, args, function (response) {
                $('#content-item').html(response);
                PxPage.Update();
                PxPage.Loaded();
                $(node).addClass("fade-effect");
                PxPage.Fade();
            });
        }
    };

    //saves the reorder operation on the given items
    var reorderToc = function (node, parentNode, prevNode, nextNode) {
        PxPage.log("reorderToc");
        var minSequence = '';
        var maxSequence = '';
        var parentId = parentNode != null ? parentNode.id : '';

        if (prevNode != null) minSequence = prevNode.sequence;

        if (nextNode != null) maxSequence = nextNode.sequence;

        var args = {
            id: node.id,
            parentId: parentId,
            minSequence: minSequence,
            maxSequence: maxSequence,
            category: getActiveCategory()
        };

        $.post(PxPage.Routes.reorder_content, args, function (response) {
            node.sequence = response;
            node.save();
            node.fade();
        });
    };

    var associateTocItemToLesson = function (parentLessonId, tocItemId, text) {
        if (tocItemId == null) return;

        var lessonWrapper = parentLessonId.parents("span").attr('id');
        var filterId = parentLessonId.closest('.filtersection').attr('id');

        if (lessonWrapper == null || lessonWrapper.indexOf('__lesson_wrapper_') == -1) {

            var lesson_parent = $('#' + filterId).find('#__lesson_parent');
            var adjust = 0;

            if (lesson_parent.find('#__lesson_parent_empty').length > 0) {
                lesson_parent.find('#__lesson_parent_empty').detach();
                adjust = -1;
            }

            lesson_parent.first().append('<span id="span_parent_' + tocItemId + '" class="syllabus_item ui-droppable newtocitem"><div class="new" style="font-size:small;">' + text + '</div></span>');
            PxSyllabusCategory.SaveContentToSyllabus(tocItemId, filterId, '', 'student|instructor', true, true, null, null, filterId);
        } else {
            var lessonId = lessonWrapper.replace("__lesson_wrapper_", "");

            var lessonBodyKey = '#__lesson_body_';
            var lessonBodyItemKey = '#__lesson_body_item_';

            if ($(lessonBodyKey + lessonId).find(lessonBodyItemKey + tocItemId).length > 0) {
                return;
            }

            var args = {
                lessonId: lessonId,
                tocItemId: tocItemId,
                type: '',
                behavior: 'add'
            };

            $(lessonBodyKey + lessonId).prepend('<li id="__lesson_body_item_' + tocItemId + '">' + text + '</li>');
            $("#__lesson_body_item_empty" + lessonId).hide();


            PxSyllabusCategory.SaveContentToSyllabus(tocItemId, lessonId, 'lesson', 'student|instructor', true, true, null, null, filterId);
        }
    };

    var filterToc = function (event) {
        var url = $("#toc-filters select option:selected").val();
        window.location = url;
    };

    var displayItem = function (itemUrl, data, callback, mode) {
        // Add the item url as a hash to the page url
        var urlParts = itemUrl.split("/");
        var itemId = urlParts[urlParts.length - 1].split("?")[0];
        var idStart = itemUrl.indexOf("id=");
        if (idStart >= 0) {
            var idEnd = idStart + "id=".length;
            var idAmp = itemUrl.indexOf("&", idEnd);

            if (idAmp > 0) {
                itemId = itemUrl.substr(idEnd, idAmp - idEnd);
            } else {
                itemId = itemUrl.substr(idEnd);
            }
        }

        PxPage.log("ContentWidget initial url: " + itemUrl);

        var cb = callback;
        var activeMode = $("#content-modes li.active a");
        if (mode) {
            activeMode = $("<span />").text(mode);
        }
        if (activeMode.length) {
            var m = activeMode.text();
            if (m != undefined) {
                if (_modes[m] == undefined) {
                    m = "View";
                }
                itemUrl = itemUrl.replace("mode=Preview", "mode=" + _modes[m]);
                if (_modes[m] == 128) {
                    cb = function (response, status, request) {
                        if (typeof (callback) == "function") {
                            callback(response, status, request);
                        }
                    };
                }
            }
        }

        var cb2 = function (response, status, request) {
            if ($(response).attr('id') == 'content-item') {
                $('#content-item').replaceWith(response);
            } else {
                $('#content-item').html(response);
            }

            if ($('#fne-content-ignore').length == 0) {
                if ($('#fne-content:visible').length) {
                    $('#fne-content').empty();
                    $('#fne-content').append(response);
                }
            }

            var active = $('#content-modes .active a');
            if (active && _modes[m] == 256) {
                //Rubrics = 256,
                active.click();
            }

            if (typeof (cb) == "function") cb(response, status, request);
        };

        PxPage.log("ContentWidget loading url: " + itemUrl);

        $.get(itemUrl, data, cb2);
    };
    
    //updates the action menu if one exists
    var updateActionMenu = function (nodeData) {
        PxPage.log("ContentWidget: updateActionMenu");
        var actions = $('#content_widget_action_menu, .eportfolio-student-tools .eportfolio_menu');
        var show = {
            name: "show",
            text: "Show To Students"
        };
        var hide = {
            name: "hide",
            text: "Hide From Students"
        };
        var toggle = show;

        if (!nodeData.isHidden()) {
            toggle = hide;
        }

        if (actions.length) {
            var menu = {
                id: "tocactions",
                options: []
            };

            var callback = function (event, action) {
                event.preventDefault();
                switch (action) {
                    case "show":
                        nodeData.show();
                        nodeData.hideUrl = nodeData.showUrl;
                        nodeData.showUrl = "";
                        nodeData.hidden = false;
                        updateActionMenu(nodeData);
                        break;

                    case "hide":
                        if (nodeData.isRootLevel) {
                            PxPage.Toasts.Warning("Top level items can not be hidden");
                        } else {
                            nodeData.hide();
                            nodeData.showUrl = nodeData.hideUrl
                            nodeData.hideUrl = "";
                            updateActionMenu(nodeData);
                        }
                        break;

                    case "remove":
                        nodeData.remove();
                        break;

                    case "context-menu-section":
                        event.stopImmediatePropagation();
                        break;

                    case "open-templates":
                        PxPage.CloseNonModal(); // Close Nonmodal if exisits
                        PxContentTemplates.ShowMoreDialog();
                        break;
                    case "autofocus":
                        if (TocAutoFocus.IsEnabled('#toc')) {
                            TocAutoFocus.Disable();
                            actions.find('.autofocus a').text('Enable Autofocus');
                        } else {
                            TocAutoFocus.Enable();
                            actions.find('.autofocus a').text('Disable Autofocus');
                        }
                        break;
                    default:
                        PxPage.CloseNonModal(); // Close Nonmodal if exisits
                        var templateItemId = action;
                        PxContentTemplates.CreateItemFromTemplate(templateItemId, ContentWidget.AddItemToActiveNode);
                        break;
                }
            };

            menu.options.push(toggle);
            if (!nodeData.isRootLevel) {
                menu.options.push({
                    name: "remove",
                    text: "Remove from TOC"
                });
            }

            menu.options.push({
                name: "open-templates",
                text: "Create New..."
            });

            menu.options.push({
                name: "autofocus",
                text: TocAutoFocus.IsEnabled('#toc') ? "Disable Auto Focus" : "Enable Auto Focus"
            });
            actions.ActionWidget({
                menu: menu,
                action: callback
            });
        }
    };

    _onHashChange = function () {
        ContentWidget.HashHasChanged();
    };
    return {
        Init: function () {
            //window.onhashchange = _onHashChange;
            PxPage.FneInitHooks['content'] = ContentWidget.OnContentFneLoaded;

            if ($('#toc').length) {
                PxPage.log("ContentWidget Binding TocWidget");

                $(PxPage.switchboard).bind("tocItemHovered", ContentWidget.TocItemHovered);
                $(PxPage.switchboard).bind("tocItemUnhovered", ContentWidget.TocItemUnhovered);

                $(PxPage.switchboard).bind("contentclicked", function (event, nodeData) {
                    var doNotLoadContent = $('#ignoreShowContent').length > 0 || nodeData.type == 'cinon_selectable_toc_content';
                    if (!doNotLoadContent) {
                        PxPage.Loading();
                        window.onhashchange = null;
                        PxPage.log("ContentWidget contentclicked: " + nodeData.id);
                        displayItem(nodeData.viewUrl, null, function () {
                            ContentWidget.ContentLoaded();

                            $('html, body').animate({
                                scrollTop: 0
                            }, 'fast');

                            //IE 7 fix
                            var contentwrapperHeight = $('#right #contentwrapper').height();
                            if (contentwrapperHeight == null || contentwrapperHeight == 0) {
                                var mainHeight = $('#main').height();
                                $('#right #contentwrapper').height(mainHeight - 32);
                            }

                            //PxComments.BindFrameControls(); 
                        });
                        //window.onhashchange = _onHashChange;
                    }

                    //updates the instructor reviewd items
                    var item = $("#" + nodeData.id);
                    if (item.hasClass('ucurrent')) {
                        item.removeClass('ucurrent');
                        item.children("a.ucurrent").removeClass("ucurrent");
                        var studentListItem = $("#ddlContext option[selected='selected']");
                        var pattern = /\((\d)\)/;
                        var listcount = parseInt(pattern.exec(studentListItem.text())[1]) - 1;
                        studentListItem.text(studentListItem.text().replace(pattern, '(' + listcount + ')'));
                        if (listcount == 0) {
                            studentListItem.removeClass('intructor-notreviewed');
                        }
                        if (!item.hasClass('uchild')) {
                            item.addClass('unone');
                            item.children("a").addClass("unone");
                        }
                        item.parents("li.uchild").each(function () {
                            if ($(this).find("li.ucurrent").length == 0 && $(this).find("li.uchild").not(':has(ul)').length == 0) {
                                if (!$(this).hasClass('ucurrent')) {
                                    $(this).addClass("unone");
                                    $(this).children("a.uchild").addClass("unone");
                                }
                                $(this).removeClass("uchild");
                                $(this).children("a.uchild").removeClass("uchild");
                            }
                        });
                    }
                });

                $(PxPage.switchboard).bind("contentloaded", function (event, nodeData) {
                    PxPage.Loaded();
                    PxPage.equalizeWidth("#left", "#right", -15);

                });

                $(PxPage.switchboard).bind("tocmoved", function (event, node, parent, prev, next) {
                    if (node.type.match("^ci") == "ci") reorderToc(node, parent, prev, next);
                });

                $(PxPage.switchboard).bind("createcontent", function (event, node, parent, prev, next, source) {
                    createContent(node, parent, prev, next, source);
                });

                // This binding helps draggin toc to accordians in Assignment center.
                $(document).off('tocDrop', PxPage.switchboard).on("tocDrop", PxPage.switchboard, function (event, itemId, target, text) {
                    if ($(target).parents("#accordioncontent").length && itemId != null) {
                        associateTocItemToLesson(target, itemId, text);
                    }
                });

                $(PxPage.switchboard).bind("tocactivatenode", function (event, nodeData) {
                    updateActionMenu(nodeData);
                });
            }

            $(PxPage.switchboard).bind("componentcancelled", function (event, componentType, componentId, componentState, frame) {
                PxPage.log("got component cancelled");
                if (componentType == "rubriceditor") {
                    PxPage.Loading();
                    if ($("a#view").length) $("a#view").click();
                }
                var isFacePlate = $('.product-type-faceplate').length > 0;
                if (!isFacePlate) {
                    if (componentType == "assessment" || componentType == "homework" || componentType == "activityplayer" || componentType == "submissiondetails") {
                        PxPage.CloseFne();
                    }
                }
            });

            if ($("#toc-filters").length) {
                $("#toc-filters select").change(filterToc);
            }

            $(document).off('click',"#activities.types ul li span").on("click", "#activities.types ul li span", function (event) {
                $(PxPage.switchboard).trigger("newnode", [$(this), "active"]);
            });

            $(document).off('click', '#toc a.delete').on('click', '#toc a.delete', function (event, stopToggle) {
                event.preventDefault();
                alert($(this).html());
                $(this).parents('.section').first().find('ins.section-icon').first().addClass('section-hover');
                var answer = confirm('Are you sure you want to delete this item?');
                if (answer) {

                    $('#toc > ul').load($(this).attr('href'), function (response) {
                        $('#toc a.expand.active').parent().find('a.display').first().click();
                    });
                } else {
                    $(this).parents('.section').first().find('ins.section-icon').first().removeClass('section-hover');
                }
            });

            $(document).off('click', '#toc a.hide').on('click', '#toc a.hide', function (event, stopToggle) {
                event.preventDefault();
                var answer = confirm('Are you sure you want to hide this item from students?');
                if (answer) {
                    var control = $(this);
                    $(control).parents('.section').first().find('ins.section-icon').first().addClass('section-hover');
                    $('#toc > ul').load($(control).attr('href'), function (response) {
                        //$(control).parent().find('a.display').first().click();
                    });
                } else { }
            });

            $(document).off('click', '#toc a.show').on('click', '#toc a.show', function (event, stopToggle) {
                event.preventDefault();
                var answer = confirm('Are you sure you want to show this item to students?');
                if (answer) {
                    var control = $(this);
                    $(control).parents('.section').first().find('ins.section-icon').first().addClass('section-hover');
                    $('#toc > ul').load($(control).attr('href'), function (response) {
                        //$(control).parent().find('a.display').first().click();
                    });
                } else { }
            });            

            //select all checkboxes in the document list when selectAll is clicked
            $(document).off('click', 'fieldset ul.document-list-controls li input[name="selectall"]').on('click', 'fieldset ul.document-list-controls li input[name="selectall"]', function () {
                $('fieldset .document-list input[type="checkbox"]').prop('checked', true);
            });

            $(document).off('click', 'fieldset ul.document-list-controls li input[name="clearall"]').on('click', 'fieldset ul.document-list-controls li input[name="clearall"]', function () {
                $('fieldset .document-list input[type="checkbox"]').prop('checked', false);
            });

            //configure event handlers for next and back
            $(document).off('click', '#nav-container #next').on('click', '#nav-container #next', ContentWidget.NextItem);
            $(document).off('click', '#nav-container #back').on('click', '#nav-container #back', ContentWidget.BackItem);

            // empty breadcrumb when closing fne window
            $('#fne-unblock-action').unbind('click', ContentWidget.RemoveBreadcrumb).bind('click', ContentWidget.RemoveBreadcrumb);

            // Respond to folder child item clicks
            $(document).off('click', '.folder-child-link').on('click', '.folder-child-link', function (event) {
                var nodeId = $(event.target).attr('rel');
                $('li#' + nodeId + ' > a.expand').trigger('dblclick');
            });

            //Create an allowed content template from the Assign Tab
            $(document).off('click', '.relatedTemplate').on('click', '.relatedTemplate', function () {
                var templateID = $(this).attr('templateid');
                ContentWidget.AddContentFromAssignTab(templateID);
            });

            // When content is loaded, update the breadcrumb for the FNE window
            $(PxPage.switchboard).bind("contentloaded", function () {
                ContentWidget.MakeBreadcrumb(ContentWidget.MoveBreadCrumbToFneTitleBar);
            });

            ContentWidget.ContentLoaded();

            // If we can find an item ID in the URL, then show that piece of content in the content widget
            var itemId = ContentWidget.GetParameterByName('assignmentID');

            if (itemId) {
                PxPage.Loading();
                var viewModeName = ContentWidget.GetParameterByName('mode');
                if (_modes[viewModeName] == undefined) {
                    viewModeName = "View";
                }
                var viewModeKey = _modes[viewModeName];
                ContentWidget.ShowContentItem(itemId, viewModeKey, function () { });
                setTimeout(function () {
                    $('#right').find("a:contains('" + viewModeName + "')").click();
                    PxPage.Loaded();
                }, 5000);
            }
        },

        InitCommenting: function (contentFrame, viewerElement, bodyElement) {
            // Setting the property centerContentItem to false for faceplate specifically
            if (window.PxComments !== undefined) {
                var commentsSettings = { contentItemWidthAuto: false, contentFrame: contentFrame, viewerElement: viewerElement, bodyElement: bodyElement };
                PxComments.Init(commentsSettings);
            } else {
                PxPage.log("Cannot initialize commenting.  PxComments not loaded.")
            }
        },

        MoveBreadCrumbToFneTitleBar: function () {
            var breadcrumb = $('#fne-content .breadcrumb');
            if (breadcrumb.length) {
                if (breadcrumb.html().length < 2) {
                    return; //there is no breadcrumb to move, keep existing
                }
                if (breadcrumb.length > 1) {
                    breadcrumb = breadcrumb.eq(1);
                }
                $('#fne-title-breadcrumb .breadcrumb').remove();
                breadcrumb.appendTo('#fne-title-breadcrumb');
                $('#fne-content .breadcrumb').hide();
                PxBreadcrumb.RunResizers();
                PxPage.UpdateFneSize();
            }
        },

        AddResourceDraggable: function () {
            if (PxPage.Context.IsProductCourse == "false") {
                $(".more-resources li").liveDraggable({
                    revert: false,
                    connectToSortable: '#toc > ul > .section',
                    cursor: "move",
                    cursorAt: {
                        top: -12,
                        left: -20
                    },
                    helper: "clone"
                });
            }
        },

        OnContentFneLoaded: function () {
            ContentWidget.ContentLoaded();
        },

        MakeBreadcrumb: function (callback) {
            if ($('#fne-window .breadcrumb').length == 0) //no breadcrumb in this FNE
                return;

            var contentReadOnly = $('#content-item .content-item-readonly').length && $('#content-item .content-item-readonly').val().toLowerCase() == 'true' ? true : false;
            if ($('#fne-content .assignment-viewer').length) {
                return;
            }

            var itemId = $('#fne-content #content-item-id').text();
            if (!itemId) {
                return;
            }
            var courseType = 1;

            if (($('.product-type-lms-faceplate').length > 0 || $('.product-type-faceplate').length > 0)) {
                courseType = 5;
            }

            if ($('.product-type-eportfolio').length > 0) {
                courseType = 2;
                return;
            }


            var categoryId;
            if ($('#view_mode').length == 0) {
                categoryId = "";
            } else {
                categoryId = $("#view_mode").val();
            }
            PxBreadcrumb.LoadItem($('#fne-window .breadcrumb'), itemId, ["PX_TOC", "PX_MULTIPART_LESSONS"], categoryId, courseType, callback);
            // Respond when the breadcrum selection changes
            $(PxPage.switchboard).unbind("breadcrumb.selectionChanged", ContentWidget.BreadcrumbChanged)
                .bind("breadcrumb.selectionChanged", ContentWidget.BreadcrumbChanged)
                .unbind("breadcrumb.onQtipResize", ContentWidget.BreadCrumbResize)
                .bind("breadcrumb.onQtipResize", ContentWidget.BreadCrumbResize);
        },

        BreadcrumbChanged: function (event, itemId) {
            var breadcrumbMarkup = $('#fne-content .breadcrumb').html();
            $('li#' + itemId + ' > a.expand').trigger('dblclick');
            return;
        },

        BreadCrumbResize: function (event, breadContent) {
            var breadHeight = $('#fne-content').height() - $('#fne-header').height() - $('.fne-edit-tabs').height();
            $(breadContent).children('.bread-crumb-siblings-list').css({
                'overflow-y': 'auto',
                'max-height': breadHeight + 'px'
            });
        },

        RemoveBreadcrumb: function () {
            $('#content-item .breadcrumb').empty();
        },

        AddItemToActiveNode: function (item, activeNode, position) {

            var source = $("<span />").text(item.title);

            if (!activeNode) {
                activeNode = "active";
            }

            $(PxPage.switchboard).trigger("newnode", [source, item.id, activeNode, null, position]);
            var itemUrl = PxPage.Routes.display_content + "/" + item.id + "?mode=Edit&includeNavigation=false";

            var completed = function (nodeId, position) {
                return function () {
                    var parentIdSel = "#toc li.section.active";
                    var nodeTypeSel = "#toc a.expand.active";
                    if (nodeId != "active") {
                        parentIdSel = "#toc li#" + nodeId;
                        nodeTypeSel = parentIdSel + " a.expand";
                    }
                    var parentId = ((nodeId === "active" || position === "inside") ? $(parentIdSel) : $(parentIdSel).parents("li").first()).attr("id");
                    var nodeType = $(nodeTypeSel).parent().attr("rel");
                    if (nodeId === "active" && (nodeType != "cifolder")) {
                        var activeNode = $("#toc li.section.active");
                        var parentNode = activeNode.parents("li").first();
                        parentId = parentNode.attr("id");
                    }
                    // change the parent id from PX_TEMP parent id, so that when clicking
                    // save button we can create a new item with this parent id.
                    $('#nonmodal-content').find('#Content_DefaultCategoryParentId').val(parentId);
                    $('#nonmodal-content').find('#Content_ParentId').val(parentId);
                    var prev = $("li:has(>a.new)").prev().find(".section-sequence").first().text();
                    var sequence = prev + "a";
                    $('#nonmodal-content').find('#Content_Sequence').val(sequence);
                    $('#nonmodal-content').find('#Content_Title').val('Untitled ' + $('#nonmodal-content').find('#Content_Title').val());


                    if (tinyMCE.activeEditor) {
                        try {
                            if (tinyMCE.activeEditor) {
                                tinyMCE.activeEditor.remove();
                            }
                        } catch (e) {
                            //PxPage.log(e);
                        }
                    }

                    if ($('.nonmodal-window')) {
                        ContentWidget.InitAssign('nonmodal-window');
                    }

                    PxPage.Loaded();
                };
            }(activeNode, position);

            $.get(itemUrl, null, function (response) {
                $(response).PxNonModal({
                    title: 'Create New',
                    onCompleted: completed
                });
                //PxPage.Update();
            });
        },

        AddResourceItemToActiveNode: function (item, activeNode, position) {
            PxPage.Loading();
            var source = $("<span />").text(item.title);

            if (!activeNode) {
                activeNode = "active";
            }

            $(PxPage.switchboard).trigger("newnode", [source, item.id, activeNode, null, position]);
            var itemUrl = PxPage.Routes.display_content + "/" + item.id + "?mode=Edit&includeNavigation=false";

            var parentIdSel = "#toc li.section.active";
            var nodeTypeSel = "#toc a.expand.active";

            if (activeNode != "active") {
                parentIdSel = "#toc li#" + activeNode;
                nodeTypeSel = parentIdSel + " a.expand";
            }

            var parentId = ((activeNode === "active" || position === "inside") ? $(parentIdSel) : $(parentIdSel).parents("li").first()).attr("id");
            var nodeType = $(nodeTypeSel).parent().attr("rel");

            if (activeNode === "active" && (nodeType != "cifolder")) {
                var activeNode = $("#toc li.section.active");
                var parentNode = activeNode.parents("li").first();
                parentId = parentNode.attr("id");
            }

            var prev = $("li:has(>a.new)").prev().find(".section-sequence").first().text();
            var sequence = prev + "a";

            var args = {
                itemId: item.id,
                parentId: parentId,
                sequence: sequence,
                category: getActiveCategory()
            };

            $.post(PxPage.Routes.save_resource_to_toc, args, function (response) {
                activeNode.sequence = response;
                //$(PxPage.switchboard).trigger("refreshtoc");
                $("#toc ul").load(PxPage.Routes.expand_section, {
                    id: "PX_TOC"
                }, function () {
                    PxPage.Update();
                    //ContentWidget.InitToc();
                    PxPage.Loaded();
                });
            });
        },
        ShowContentItem: function (itemId, mode, callback, getChildrenGrades) {
            if (window.History != null && window.History.enabled) {
                window.History.replaceState({ plugin: "ContentWidget", func: "ShowContentItemAction", args: [itemId, mode, callback.toString(), getChildrenGrades] }, "Home", "?item=" + itemId);
            } else {

                ContentWidget.ShowContentItemAction(itemId, mode, callback.toString(), getChildrenGrades);
            }
        },
        ShowContentItemAction: function (itemId, mode, callback, getChildrenGrades) {
            var url = PxPage.Routes.display_content + "?id=" + itemId;

            if (mode) {
                url += "&mode=" + mode;
            } else if (url.indexOf("&mode=") < 0) {
                url += "&mode=Preview";
            }

            if (getChildrenGrades) {
                url += "&getChildrenGrades=true";
            }

            if (!callback) {
                callback = function () { };
            }
            else {
                callback = eval("(" + callback + ")"); //TODO: is this IE-compatible?
            }
            PxPage.Loading();
            displayItem(url, '', function () {
                PxPage.Loaded();
                callback();


                //IE 7 fix
                var contentwrapperHeight = $('#right #contentwrapper').height();
                if (contentwrapperHeight == null || contentwrapperHeight == 0) {
                    var mainHeight = $('#main').height();
                    $('#right #contentwrapper').height(mainHeight - 32);
                }
            });
        },

        RefreshContentItem: function (itemId, mode, parentId, callback) {
            var itemUrl = PxPage.Routes.display_content + "?id=" + itemId + "&mode=" + mode;
            var data = "id=" + itemId + "&mode=" + mode;
            $('#' + parentId + ' #content-item').load(itemUrl, data, callback);
        },

        TocItemUnhovered: function (event, nodeData) {
            clearTimeout(ContentWidget.ItemHoverTimer);
            ContentWidget.ItemHoverTimer = null;
        },

        TocItemHovered: function (event, nodeData) {
            ContentWidget.ShowItemHover = true;
            ContentWidget.ItemHoverTimer = setTimeout(function () {
                if (!ContentWidget.ItemHoverTimer) {
                    return;
                }
                clearTimeout(ContentWidget.ItemHoverTimer);
                ContentWidget.ItemHoverTimer = null;
                //$('.qtip-content').hide();
                var tooltip = $("#" + nodeData.id + " > .tooltip");
                var tocItem = tooltip.siblings('#tocItemId').val();
                var isHiddenOnly = false;
                if (!tooltip.parent().hasClass('assigned')) {

                    if ($("#" + nodeData.id).hasClass("hidden")) {
                        isHiddenOnly = true;
                    } else {
                        $('.qtip-content').hide();
                        return;
                    }
                }
                var $item = $("#" + nodeData.id);
                if (tooltip.length) {
                    $('.qtip-content').show()
                    var self = $("#" + nodeData.id + " .expand").first();
                    var itemId = nodeData.id;
                    self.qtip({
                        content: {
                            text: 'Loading'
                        },
                        show: {
                            delay: 2000,
                            ready: true
                        },
                        hide: {
                            when: {
                                event: 'mouseout'
                            }
                        },
                        position: {
                            corner: {
                                tooltip: "topLeft",
                                target: "topRight"
                            }
                        },
                        style: {
                            padding: 0,
                            width: {
                                min: 178,
                                max: 300
                            }
                        },
                        api: {
                            onRender: function () {
                                var current = this;
                                if (isHiddenOnly) {
                                    current.updateContent('Hidden from students', false);
                                } else {
                                    jQuery.post(
                                        PxPage.Routes.show_tocitem_tooltip, {
                                            'assignmentId': itemId,
                                            'isAssignmentCenter': false
                                        }, function (theResponse) { // Callback after the ajax request is complete
                                            if (jQuery.trim(theResponse) == '') {
                                                current.updateContent('no tool tip', false);
                                            } else {
                                                current.updateContent(theResponse, false); // Replace current content
                                            }
                                            return true;
                                        });
                                }
                                PxPage.log('render tooltip');
                            },
                            onShow: function () {
                                $('.qtip-content').show()
                                PxPage.log('removing timer id ' + ContentWidget.ItemHoverTimer);
                                clearTimeout(ContentWidget.ItemHoverTimer);
                                ContentWidget.ItemHoverTimer = null;
                            }
                        }
                    }).qtip("show");
                }
            }, 3000); // this has to be greater than the delay for qtip
        },

        AddGBBcategory: function (itemid, contentWrapper, isIncludeAddNewOption) {

            var options = $.extend({
                modal: true,
                title: "Create New Category",
                draggable: true,
                closeOnEscape: false,
                width: '400px;',
                height: '300px',
                resizable: false,
                autoOpen: false,
                dialogClass: 'px-dialog-notitle'
            });


            var tag = $("<div id='px-dialog'></div>"); //This tag will the hold the dialog content.
            var args = {
                contentItemId: itemid,
                isIncludeAddNewOption: isIncludeAddNewOption
            };


            tag.dialog({
                modal: options.modal,
                title: options.title,
                draggable: options.draggable,
                closeOnEscape: options.closeOnEscape,
                width: options.width,
                resizable: options.resizable,
                autoOpen: options.autoOpen,
                close: function () {
                    $(this).dialog('destroy').empty().detach();
                }
            }).dialog('open');

            tag.load(PxPage.Routes.contentwidget_show_gradebookcategory, args, function (data, textStatus, XMLHttpRequest) { });
        },

        closeForm: function () {
            $('#selgradebookweightsHidden').val('');

            $("#txtaddGBBcategory").val('');
            $('#ggbcategorydialog input[type="text"]').val('');
            $("#ggbcategorydialog").hide();
            //$('#ggbcategorydialog').dialog('close');
            $("#ggbcategorydialog").remove();
        },

        cancelCategoryAdd: function () {
            $('#selgradebookweightsHidden').val('');

            $('#ggbcategorydialog input[type="text"]').val('');
            $(".ui-icon-closethick").click();
        },

        addGBBcategory: function (itemid, isIncludeAddNewOption) {
            $('#spnLinkError').text('');
            $('#spnLinkError').hide();


            if ($.trim($('#ggbcategorydialog input[type="text"]').val()) == '') {
                $('#spnLinkError').addClass('field-validation-error').text('Field cannot be empty');
                $('#spnLinkError').show();
                $('#txtaddGBBcategory').focus();
                return;
            }

            //Validate if input text is matching any options list value
            $("#selgradebookweights option[value='']").remove();
            $('#ggbcategorydialog input[type="text"]').val();

            var txtNotExist = true;
            $("#selgradebookweights option").each(function () {
                //$(this).text();
                if ($.trim($(this).text()).toLowerCase() == $.trim($('#ggbcategorydialog input[type="text"]').val()).toLowerCase()) {
                    txtNotExist = false;
                    $(this).text();
                }
            });

            if (txtNotExist == false) {
                //throw error message
                $('#spnLinkError').addClass('field-validation-error').text('This category exists. please change a category name.');
                $('#spnLinkError').show();
                $('#txtaddGBBcategory').focus();
            } else {
                //var counter = $("#selgradebookweights option").each(function () { counter++; }).length;
                //mishka fixed count max option value
                var x = "";
                $("#selgradebookweights option[value]").each(function () {
                    if ($(this).val() != "createNewCat" && $(this).val() != "NaN") {
                        x += $(this).val() + ',';
                    }
                });
                
                var item = {
                    id: itemid,
                    name: $.trim($('#ggbcategorydialog input[type="text"]').val())
                };
                
                ContentWidget.CreateNewGradebookCategory(item, function (categoryId) {
                    $(".selgradebookweights").append('<option value="' + categoryId + '" selected="selected">' + $.trim($('#ggbcategorydialog input[type="text"]').val()) + '</option>');
                    
                    $(".ui-icon-closethick").click();
                    ContentWidget.closeForm();
                });
            }
        },
        
        CreateNewGradebookCategory: function (item, callback) {
            var url = PxPage.Routes.addGradebookCategory_link;
            
            $.post(url, {
                id: item.id,
                'newCategory': item.name
            }, function (result) {
                if (result) {
                    var categoryId = result.gradebookId;
                    if (typeof(callback) === "function") {
                        callback(categoryId, item.id);
                    }

                    PxPage.log("Gradebook category for " + item.id + " is successfully created with name " + item.name);
                } else {
                    PxPage.Toasts.Error('Error when adding new category.');
                    
                    PxPage.log("Creating gradebook category for " + item.id + " failed.");
                }
            });
        },
        
        AddGradebookCategoryToUnit: function (categoryId, unitId, callback)
        {
            var url = PxPage.Routes.addGradebookCategoryToUnit;

            $.post(url, {
                unitId: unitId,
                categoryId: categoryId
            }, function (result) {
                if (result == "success") {
                    if (typeof(callback) === "function") {
                        callback();
                    }
                    
                    return true;
                } else {
                    PxPage.Toasts.Error('Error when adding new category to unit.');

                    PxPage.log("Creating gradebook category for " + unitId + " failed.");

                    return false;
                }
            });
        },

        AssignmentDateSelected: function (formatted, dates) {
            if (formatted == '') {
                $('#facePlateAssignDueDate').val('');
                $('.invaliddate').show();
                return;
            }

            $('.invaliddate').hide();
            $('.invalidtime').hide();
            $('.placeholderWrap').addClass('placeholder-changed');

            var contentWrapper = $(this).closest('.contentwrapper');
            if (contentWrapper.length < 1) {
                contentWrapper = $('.FacePlateAsssign.contentwrapper'); //find faceplate contentwrapper
            }
            if (contentWrapper.length < 1) {
                contentWrapper = $('#cal-box #assignment-calendar').closest('.contentwrapper');
            }

            var startDate = new Date();
            var endDate = new Date();
            if ($.isArray(dates)) {
                startDate = new Date(dates[0]);
                endDate = new Date(dates[1]);
            } else {
                startDate = new Date(formatted);
                endDate = new Date(formatted);
            }

            contentWrapper.find('#StartDate').val(startDate.format('mm/dd/yyyy'));
            contentWrapper.find('#DueDate').val(endDate.format('mm/dd/yyyy'));

            if ($("#facePlateAssignDueDate").length > 0) {
                $("#facePlateAssignDueDate").val(endDate.format('mm/dd/yyyy')).change();

                if ($("#facePlateAssignTime").length > 0) {
                    if ($("#facePlateAssignTime").val() == ""
                        || $("#facePlateAssignTime").val() == "—"
                        || !$("#facePlateAssignTime").val().match(/[0-1]\d:[0-5]\d [aApP][mM]/)) {
                        $("#facePlateAssignTime").val('11:59 PM').change();
                        contentWrapper.find('#dueHour').val("11");
                        contentWrapper.find('#dueMinute').val("59");
                        contentWrapper.find('#dueAmpm').val("PM");
                    }

                    endDate = new Date(formatted + " " + $("#facePlateAssignTime").val());
                } else {
                    endDate = new Date(formatted + " 11:59 PM");
                }
            }
            var settingsAssignDueDate = contentWrapper.find('#settingsAssignDueDate');
            if (settingsAssignDueDate.length > 0) {
                settingsAssignDueDate.val(endDate.format('mm/dd/yyyy')).change();

                ContentWidget.UpdateAssignedItems(endDate.getFullYear(), endDate.getMonth(), endDate.getDate());

                var settingsAssignTime = contentWrapper.find('#settingsAssignTime');
                if (settingsAssignTime.length > 0) {
                    if (settingsAssignTime.val() == ""
                        || !settingsAssignTime.val().match(/[0-1]\d:[0-5]\d [aApP][mM]/)) {
                        settingsAssignTime.val('11:59 PM');
                        contentWrapper.find('#dueHour').val("11");
                        contentWrapper.find('#dueMinute').val("59");
                        contentWrapper.find('#dueAmpm').val("PM");
                    }

                    endDate = new Date(formatted + " " + settingsAssignTime.val());
                } else {
                    endDate = new Date(formatted + " 11:59 PM");
                }
            }
            var dueDate = contentWrapper.find('.DueDate');
            if (dueDate.length > 0) {
                dueDate.removeClass("readonly");
                dueDate.removeAttr("readonly");
                dueDate.val(endDate.format('mm/dd/yyyy'));
                $(".hdnFullDueDate").val(endDate.format('mm/dd/yyyy'));
            }
            var dueHour = contentWrapper.find('#dueHour');
            if (dueHour.length > 0) {
                dueHour.val(ContentWidget.getHours(endDate));
            }
            var dueMinute = contentWrapper.find('#dueMinute');
            if (dueMinute.length > 0) {
                dueMinute.val(ContentWidget.getMinutes(endDate));
            }
            var dueAmPm = contentWrapper.find('#dueAmpm');
            if (dueAmPm.length > 0) {
                dueAmPm.val(ContentWidget.getAMorPM(endDate));
            }

            var id = contentWrapper.find('#content-item-id').text();

            //if Date is selected then show the date and time picker
            contentWrapper.find('.chkDueDate').prop('checked', true);
            contentWrapper.find('#fsDateTime').show();

            ContentWidget.IsFormChanged(contentWrapper);

        },

        UpdateAssignedItems: function (year, month, day) {
            if (!$('#other-assignments').is('div')) {
                return;
            }

            var selectedGroupId = $("#SettingsEntityId").val();
            var id = $('#content-item-id').text();

            if (isNaN(year)) {
                $('#other-assignments').html('');
            } else {
                var args = {
                    year: year,
                    month: month,
                    day: day,
                    itemId: id,
                    groupId: selectedGroupId
                };

                $.get(PxPage.Routes.assigned_items_date, args, function (response) {
                    $('#other-assignments').html(response);
                });
            }
        },

        OnRenderMonthFacePlateAssignment: function () {
            //alert('hello!');
        },

        getTime: function (d) {
            var input = dateFormat(d, "mm/dd/yyyy hh:MM TT");
            if (/^\d+$/.test(input)) {
                input = input.toNumber();
            }
            var text, dt = Date.parse(input);
            if (dt == null) {
                text = 'Invalid time.';
            } else {
                text = dt.format('hh:MM TT');
            }
            return text;
        },

        getAMorPM: function (d) {
            var input = dateFormat(d, "mm/dd/yyyy hh:MM TT");
            if (/^\d+$/.test(input)) {
                input = input.toNumber();
            }
            var text, dt = Date.parse(input);
            if (dt == null) {
                text = 'PM';
            } else {
                text = dt.format('TT');
            }
            return text;
        },

        getHours: function (d) {
            var input = dateFormat(d, "mm/dd/yyyy hh:MM TT");
            if (/^\d+$/.test(input)) {
                input = input.toNumber();
            }
            var text, dt = Date.parse(input);
            if (dt == null) {
                text = '11';
            } else {
                text = dt.format('hh');
            }
            return text;
        },

        getMinutes: function (d) {
            var input = dateFormat(d, "mm/dd/yyyy hh:MM TT");
            if (/^\d+$/.test(input)) {
                input = input.toNumber();
            }
            var text, dt = Date.parse(input);
            if (dt == null) {
                text = '59';
            } else {
                text = dt.format('MM');
            }
            return text;
        },
        FacePlateAssignmentDateSelected: function (formatted, dates) {
            $(this).parents('#assignment-calendar').find('#facePlateAssignDueDate').val(formatted);
            $(this).parents('#assignment-calendar').find('#faceplate-assignment-time-field').val("11:59 pm");
        },

        //Initializes behaviors for the Create and Assign view of the widget
        InitAssign: function (assignmentSettings, isResize, getSubmissionStatus) {
            $(document).ready(function () {
                PxPage.log("ContentWidget InitAssign");
                //this is to avoid conflict if multiple instance of assign view is available
                //ex: create new folder when current tab is assign tab

                if (isResize == undefined || isResize == null)
                    isResize == true;

                if (getSubmissionStatus === undefined || getSubmissionStatus === null)
                    getSubmissionStatus = true;

                var assignmentSettingsClass;
                if (assignmentSettings != null) {
                    assignmentSettingsClass = $('.' + assignmentSettings);
                } else {
                    assignmentSettingsClass = $('#assignment-settings');
                }

                //assignment date
                var startDate = new Date(assignmentSettingsClass.find('#StartDate').val());
                var tempDate = assignmentSettingsClass.find('#DueDate').val();

                if (tempDate)
                    tempDate = tempDate.replace('1/1/0001', '');

                var dueDate = new Date(tempDate);
                if (isNaN(dueDate.getTime())) {
                    dueDate = null;

                    // turn validation messages on
                    assignmentSettingsClass.find('.invaliddate').show();
                    assignmentSettingsClass.find('.invalidtime').show();
                }

                var dateFrom = startDate == null ? '' : startDate;
                var dateTo = dueDate == null ? '' : dueDate;
                var dateCurrent = dueDate == null ? new Date() : dueDate;

                if (assignmentSettingsClass.find('#content-item.module').length) {
                    assignmentSettingsClass.find('#startDateField').show();

                    assignmentSettingsClass.find('#cal-box #assignment-calendar').DatePicker({
                        flat: true,
                        date: [dateFrom, dateTo],
                        current: dateCurrent,
                        calendars: 1,
                        mode: 'range',
                        starts: 0,
                        onChange: ContentWidget.AssignmentDateSelected
                    });
                } else {

                    assignmentSettingsClass.find('#cal-box #assignment-calendar').DatePicker({
                        flat: true,
                        date: dateTo,
                        current: dateCurrent,
                        calendars: 1,
                        mode: 'single',
                        starts: 0,
                        onChange: ContentWidget.AssignmentDateSelected
                    });
                    if (dateTo != '' && dateTo != null) {
                        assignmentSettingsClass.find('#cal-box #assignment-calendar').DatePickerSetDate(dateTo, true);
                        ContentWidget.UpdateAssignedItems(dueDate.getFullYear(), dueDate.getMonth(), dueDate.getDate());
                    }
                    //if (assignmentSettingsClass.parent().is('form')) {
                    //    assignmentSettingsClass.find('#cal-box #assignment-calendar').DatePicker({
                    //        onChange: ContentWidget.AssignmentDateSelected
                    //    });
                    //}
                }

                //rebrics selection
                assignmentSettingsClass.find('#fsRubric .fsEnabler').click(function (event) {
                    if (event.target.checked == true) {
                        assignmentSettingsClass.find('#selRubric').removeAttr("disabled");
                    } else {
                        assignmentSettingsClass.find('#selRubric').attr("disabled", "disabled");
                    }
                });

                assignmentSettingsClass.find('#fsRubric .view-rubric').click(function (event) {

                    var objValue = assignmentSettingsClass.find('option:selected', '#selRubric').val();
                    if (objValue != '') {
                        var data = {
                            url: PxPage.Routes.select_rubric + "?id=" + objValue,
                            title: 'Rubric Selector',
                            minimize: false
                        };
                        PxPage.openContent(data);
                    }
                });

                //Email reminder
                assignmentSettingsClass.find('#chkScheduleEmailReminder').rebind('click', function (event) {
                    ContentWidget.EmailReminderClicked(event.target.checked, assignmentSettingsClass);
                });

                //Late Submission
                assignmentSettingsClass.find('#chkAllowLateSubmissions').click(function (event) {
                    if (event.target.checked == true) {
                        assignmentSettingsClass.find('#lateSubmissionDeatils').show();
                    } else {
                        assignmentSettingsClass.find('#lateSubmissionDeatils').hide();
                    }

                    if (event.target.checked == true) {
                        assignmentSettingsClass.find('#gracePeriodDetails').show();
                        assignmentSettingsClass.find('#LateGracePeriodDuration').removeAttr("disabled");
                        assignmentSettingsClass.find('#LateGracePeriodDurationType').removeAttr("disabled");
                    } else {
                        assignmentSettingsClass.find('#gracePeriodDetails').hide();
                        assignmentSettingsClass.find('#LateGracePeriodDuration').attr("disabled", "disabled");
                        assignmentSettingsClass.find('#LateGracePeriodDurationType').attr("disabled", "disabled");
                    }
                });

                if (tinyMCE) {
                    $(PxPage.switchboard).unbind("editor_value_changed");
                    $(PxPage.switchboard).bind("editor_value_changed", function (event) {
                        assignmentSettingsClass.find('#hdnReminderBody').val('false');
                        ContentWidget.IsFormChanged(assignmentSettingsClass);
                    });
                }

                //create rubric
                assignmentSettingsClass.find('#fsRubric .create-rubric').click(function (event) {
                    var data = {
                        url: PxPage.Routes.edit_rubric,
                        title: 'Create Rubric',
                        minimize: false
                    };
                    PxPage.openContent(data);
                });

                //select rubric
                assignmentSettingsClass.find('#fsRubric #selRubric').change(function () {
                    var objValue = assignmentSettingsClass.find('option:selected', '#selRubric').val();
                    if (objValue != '') {
                        assignmentSettingsClass.find('#fsRubric .view-rubric').show();
                    } else {
                        assignmentSettingsClass.find('#fsRubric .view-rubric').hide();
                    }
                });


                //assignment gradable checkbox
                assignmentSettingsClass.find('.chkAssignmentGradeable').change(function () {
                    ContentWidget.AssignmentGradeable(assignmentSettingsClass);
                });

                //mark as complete checkbox
                assignmentSettingsClass.find('.chkMarkAsComplete').change(function () {
                    ContentWidget.MarkAsCompleted(assignmentSettingsClass);
                });

                //due date checkbox
                assignmentSettingsClass.find('.chkDueDate').change(function () {
                    var chkDueDate = $(this);
                    ContentWidget.DueDateChanged(assignmentSettingsClass, chkDueDate);
                });
            });

            if (getSubmissionStatus) {
                // finding out how many students have submitted this assignment
                var args = {
                    assignmentId: $("#content-item #assignment-settings #AssignmentTabItemId").val()
                };

                $.post(PxPage.Routes.get_submission_status_management_card, args, function (response) {
                    $(".faceplate-student-completion-stats").html(response);
                });
            }

            if (assignmentSettings === "contentcreate" && isResize == true) {
                PxPage.UpdateFneSize();
                $(window).trigger('resize');
            }

            $('#fne-window').removeClass('require-confirm');
        },

        AddNewCategoryClick: function (ev) {
            // changed the target.event variable cause IE8 doesn't pass an instance of the event object to the handler
            ev = ev || window.event;
            var target = ev.target || ev.srcElement;

            var contentWrapper = $(target).closest('.contentwrapper');

            var itemId = "";
            if (contentWrapper.length > 0) {
                itemId = contentWrapper.find("#form #Id").val();
                ContentWidget.AddGBBcategory(itemId, contentWrapper, true);
            } else {
                ContentWidget.AddGBBcategory(itemId, assignmentSettingsClass);
            }

        },

        GetAssignedTime: function (contentWrapper) {
            var dueDate = contentWrapper.find('#DueDate:not(.hdnFullDueDate)').val();

            var time = contentWrapper.find('#settingsAssignTime').val();
            if (time == null || time == "") {
                time = contentWrapper.find("#dueHour").val() + ":" + contentWrapper.find("#dueMinute").val() + ":" + contentWrapper.find("#dueAmpm").val();
            }
            else {
                var localDateTime = dueDate + " " + $.trim(time);

                if ($("#dueHour").length > 0) {
                    $("#dueHour").val(ContentWidget.getHours(localDateTime));
                }

                if ($("#dueMinute").length > 0) {
                    $("#dueMinute").val(ContentWidget.getMinutes(localDateTime));
                }

                if ($("#dueAmpm").length > 0) {
                    $("#dueAmpm").val(ContentWidget.getAMorPM(localDateTime));
                }
            }
            return time;
        },
        ///Rest the value of the item when the assign / usnassign action is successful
        OnAssignSuccess: function (response, contentWrapper) {
            if (contentWrapper.find('#assignment-settings #isError').val() != 'true') {
                var itemId = contentWrapper.find('#assignment-settings #Id').val();
                var node = $("#toc #" + itemId);
                var assigned = contentWrapper.find('#assignment-settings #requestType').val() == "assign" ? true : false;
                if (node.length) {
                    if (response.behavior == "assign") {
                        node.addClass("assigned");
                    } else {
                        node.removeClass("assigned");
                    }
                }
            }
            var date = new Date(contentWrapper.find("#DueDate").val());
            ContentWidget.UpdateAssignedItems(date.getFullYear(), date.getMonth(), date.getDate());

            var courseType = contentWrapper.find('#courseType').val();

            if (response.behavior == "assign") {
                contentWrapper.find('.btnAssign').hide();
                $('.btnSaveChanges').show();
                $('.btnSaveChanges').attr('disabled', 'disabled');
                contentWrapper.find('#btnUnassign').show();
                contentWrapper.find('#hdnIsAssigned').val('true');
                //enable settings dropdown for assigned items
                if (contentWrapper.find("#ddlSettingsList").length) {
                    contentWrapper.find("#ddlSettingsList").prop('disabled', false);
                }
                
            }

            if (response.behavior == "unassign") {
                contentWrapper.find("button.unassign").hide();
                contentWrapper.find('#cal-box #assignment-calendar').html("");

                contentWrapper.find("#DueDate").val("");
                contentWrapper.find("#dueHour").val("0");
                contentWrapper.find("#dueMinute").val("0");
                contentWrapper.find("#dueAmpm").val("am");
                contentWrapper.find('.invaliddate').show();
                contentWrapper.find('.invalidtime').show();

                contentWrapper.find('#chkAssignmentGradeable').prop('checked', false);
                contentWrapper.find("#chkMarkAsComplete").prop('checked', false);

                contentWrapper.find('#chkAllowLateSubmissions').prop('checked', false);
                contentWrapper.find('#chkScheduleEmailReminder').prop('checked', false);
                contentWrapper.find('.btnAssign').show();
                contentWrapper.find('.btnAssign').prop('disabled', true);
                contentWrapper.find('.btnSaveChanges').hide();
                contentWrapper.find('#btnUnassign').hide();

                //Email Reminder
                contentWrapper.find('#ReminderBeforeCount').val('0');
                contentWrapper.find('#ReminderBeforeType').val('day');
                contentWrapper.find('#ReminderSubject').val('');
                if (tinyMCE.activeEditor != null) {
                    try {
                        tinyMCE.activeEditor.setContent('');
                    } catch (e) { }
                }
                contentWrapper.find('#ReminderBeforeCount').attr("disabled", "disabled");
                contentWrapper.find('#ReminderBeforeType').attr("disabled", "disabled");
                contentWrapper.find('#ReminderSubject').attr("disabled", "disabled");
                contentWrapper.find('#reminderEmailDeatils').hide();

                //Late Submission
                contentWrapper.find('#LateGracePeriodDuration').val('0');
                contentWrapper.find('#LateGracePeriodDurationType').val('minute');
                contentWrapper.find('#lateSubmissionDeatils').hide();
                contentWrapper.find('#gracePeriodDetails').hide();
                //disable settings dropdown when item is unassigned
                if (contentWrapper.find("#ddlSettingsList").length) {
                    contentWrapper.find("#ddlSettingsList").prop("disabled", true);
                }
                
                ContentWidget.InitAssign();
            } else {
                contentWrapper.find("button.unassign").show();
            }

            $('#fne-window').removeClass('require-confirm');
            PxPage.Toasts.Success("Item Assignment Saved");
        },

        InitContentCreation: function () {
            PxPage.UpdateFneSize();
        },

        UpdateToc: function () {
            if ($('#right > #content-item #toc > ul').length) {
                $('#toc > ul').replaceWith($('#right > #content-item #toc > ul').clone().show());
                //ContentWidget.InitToc();
            }
        },

        ToggleSection: function () {
            if (!$('#toc a.expand.active').parent().hasClass('noToggle')) {
                var children = $('#toc a.expand.active').parent().children('.children');
                if ($(children).css('display') == 'none') {
                    $(children).show();
                } else {
                    $(children).hide();
                }
            }

            $('a.expand').prev('.section-icon.section-hover').removeClass('section-hover');
            $('#toc > ul .section:has(a.expand.active)').find('a.expand').droppable({
                hoverClass: 'section-hover',
                accept: '.section, li.ui-draggable',
                tolerance: 'pointer'
            });
        },

        ReloadMultiplePartPage: function (cl) {
            window.location = '?cl=' + cl;
            return true;
        },

        ContentLoaded: function () {

            _initPxAPI();

            $(PxPage.switchboard).bind("htmlquiz-loaded", function (e) {

                ////var frame = $("#htmlquiz-frame");
                var frame = $("#htmlquiz-frame")

                if (frame.length > 0) {
                    frame.attr("class", "autoHeight ");
                }
            });

            $(document).off('click', '.requireConfirmation').on('click', '.requireConfirmation', function (event) {
                //Check if the button clicked was the left mouse button
                if (event.which == 1) {
                    var valid = ContentWidget.ValidateNavigateAway(event.target, event.srcElement);
                    if (!valid) {
                        event.stopImmediatePropagation();
                        return false;
                    }
                }
            });
            $('.addActive').bind('click', function (event) {
                var target = event.target || event.srcElement;
                PxPage.Loading("#content-item");
                if (event != null && target != null) {
                    //set calling tab to active
                    $(target).parent("li").addClass("active");
                    $(target).parent("li").siblings().removeClass("active");
                }
            });

            if ($('h2.content-title').text() != null && $('h2.content-title').text().length > 0) {
                document.title = $('h2.content-title').text();
            }

            $('#right #content-item .discussion-widget').remove();

            PxPage.SetFneLinks();
            PxPage.UpdateFneSize();
            if (window.PxAssignment != undefined) PxAssignment.Init();
            if (window.PxArga != undefined) PxArga.Init();
            if (window.PxRssFeed != undefined) PxRssFeed.Init();
            if (window.PxDiscussion != undefined) PxDiscussion.Init();
            if (window.PxWiki != undefined) PxWiki.Init();
            if (window.PxRubric != undefined) PxRubric.Init();
            //PLATX - in IE 7 below code was causing script timeout error so commented the line. PLATX -- 4805
            //if (window.PxAssignmentCenter != undefined) PxAssignmentCenter.Init();
            if (window.PxSettingsTab != undefined) {
                if ($('#content-modes.link-list li.active a').text() == 'Settings') {
                    PxSettingsTab.Init();
                }
            }
            $(PxPage.switchboard).trigger("contentloaded");
            if ($('#fne-window').is(':visible')) {
                ContentWidget.MakeBreadcrumb(ContentWidget.MoveBreadCrumbToFneTitleBar);
                PxPage.SetFneNavigations();
            } else {
                $('#content-nav').show();
                var itemid = $("#content-item-id").text();

            }
        },
        ValidateNavigateAway: function (target, srcElement, callback) {
            target = target || srcElement;

            if ($('#fne-window').hasClass('require-confirm')) {
                if ($('#fne-content').find("#dialog-confirm").length == 0) {
                    $("<div id='dialog-confirm' title='Editing'>Are you sure you want to leave this tab?<br/>All your changes will be lost!</div>").appendTo("#fne-content");
                }

                var navigateTo = $(target).attr('id');

                ContentWidget.ValidateConfirmation(function () {
                    if (callback != null) {
                        callback();
                    }
                    PxPage.Loading("#content-item");

                    //set calling tab to active
                    if (srcElement != null) {
                        $(srcElement).parent("li").addClass("active");
                        $(srcElement).parent("li").siblings().removeClass("active");
                    }
                    else if (target != null) {
                        $(target).parent("li").addClass("active");
                        $(target).parent("li").siblings().removeClass("active");
                    }
                }, navigateTo);

                return false;
            }
            else if ($('#fne-window').hasClass('require-confirm-custom')) {
                $(PxPage.switchboard).trigger("validateNavigateAway", [target, srcElement, callback]);
            }
            else {
                if (callback != null) {
                    callback();
                }
                PxPage.Loading("#content-item");

                //set calling tab to active
                if (srcElement != null) {
                    $(srcElement).parent("li").addClass("active");
                    $(srcElement).parent("li").siblings().removeClass("active");
                }
                else if (target != null) {
                    $(target).parent("li").addClass("active");
                    $(target).parent("li").siblings().removeClass("active");
                }

                return true;
            }

        },
        ContentCreatedAndOpen: function (response, parent, item, mode, context, callback) {
            var callbackOpen = function () {
                if (callback != null) {
                    callback();
                }
                var contentItemId = $("#content-item-id").text();
                if (contentItemId == "") {
                    contentItemId = $(response).find(".item-id").val();
                }
                $(PxPage.switchboard).trigger("opencontent", contentItemId);
            };
            ContentWidget.ContentCreated(response, parent, item, mode, context, callbackOpen);
        },

        ContentCreated: function (response, parent, item, mode, context, callback) {
            PxPage.log("ContentCreated");
            //Since the find method looks at descendants  only added the new check.

            //Since xbook has the document viewer displayed at the same time as a dialog, we need to make sure we are grabbing the
            //right content 
            var createdId = $(response).find('.item-id').val();
            var createdContentItem = $('#content-item .item-id[value="' + createdId + '"]').parent();
            
            if (context == undefined) {
                if ($('.product-type-faceplate').length > 0) {
                    context = "FacePlate";
                }
            }

            var fireTrigger = true;
            if ((response != null && response == "cancel") || createdContentItem.length == 0) {
                //do nothing
                fireTrigger = false;
            } else if ($("#content-item:has(#content-item)").length) {
                    PxPage.log("ContentCreated error out");
                    var appendMe = createdContentItem.children("#content-item");
                    createdContentItem.first().replaceWith(appendMe);
            }

            if (fireTrigger) {
                var contentItemId = $.trim(createdContentItem.find("#content-item-id").text());
                var contentTitle = $.trim($(response).find(".content-item-title").val());
                var contentTOC = 'syllabusfilter';

                if (createdContentItem.find('.toc').length > 0 && createdContentItem.find('.toc').val() !== '') {
                    contentTOC = createdContentItem.find('.toc').val();
                }

                var contentType = '';
                if (createdContentItem.find('#Content_Type').length > 0) {
                    contentType = createdContentItem.find('#Content_Type').val();
                }

                var unitType = createdContentItem.attr("bfwtype");

                if ((context == "FacePlate" && contentItemId == "") || contentItemId == "") {
                    contentItemId = $(response).find(".item-id").val();
                }
                if (parent == null) {
                    parent = $(response).find("#content-item-parentid").val();

                    if (parent == null) {
                        parent = $(response).find("#ParentId").val();
                    }
                }

                PxPage.log("ContentCreated content-item-id: " + contentItemId);

                var content = {
                    id: contentItemId,
                    name: contentTitle,
                    parentId: parent,
                    toc: contentTOC,
                    contentType: contentType,
                    unitType: unitType
                };

                $(PxPage.switchboard).trigger("contentcreated", [content, mode, parent, callback]);
            }

            ContentWidget.ContentLoaded();

            var isCloseNonModal = mode;

            if (!isCloseNonModal) {
                isCloseNonModal = PxContentTemplates.GetTemplateReloadMode();
            }

            var assignTabContentCreate = false;
            if ($('#hdnAssignTabContentCreate').length > 0) {
                assignTabContentCreate = $.parseJSON($('#hdnAssignTabContentCreate').val());
                $('#hdnAssignTabContentCreate').val('false');
            }

            if (assignTabContentCreate) {
                isCloseNonModal = 'normal';
            }

            if (isCloseNonModal == null || isCloseNonModal == 'normal' || isCloseNonModal == 'modal') {
                PxPage.CloseNonModal();
            } else {
                PxPage.Loading("nonmodal");
                PxContentTemplates.ShowMoreDialog(context, callback);
            }

            var contentwrapperHeight = $('#right #contentwrapper').height();
            if (contentwrapperHeight == null || contentwrapperHeight == 0) {
                var mainHeight = $('#main').height();
                $('#right #contentwrapper').height(mainHeight - 32);
            }

            PxPage.Update();
            if ($("#fne-window.fne-edit").length != 0) {
                $('#fne-window').removeClass('require-confirm');
                if ($('#saveItem').find('form').length == 0) {
                    PxPage.RestoreDocForm();

                    PxPage.Toasts.Success("Item was saved");
                    PxPage.Loaded("#content-item");
                }
                ContentWidget.NavigateAway();
            }

        },

        ValidateDateOnAssign: function (contentWrapper) {
            var dueDateParts = contentWrapper.find("#DueDate").val().split("/");
            if (dueDateParts.length >= 2) {
                var dueYear = dueDateParts[2];
                var dueMonth = dueDateParts[0] - 1;
                var dueDay = dueDateParts[1];
                var dueHour = parseInt(contentWrapper.find('#dueHour').val()) % 12;
                var dueMinute = contentWrapper.find('#dueMinute').val();
                var dueAmpm = contentWrapper.find('#dueAmpm').val();

                if (dueAmpm == 'pm') {
                    dueHour = dueHour + 12;
                }

                var submitDate = new Date(dueYear, dueMonth, dueDay, dueHour, dueMinute, 0, 0);
                var today = new Date();
                //PRODUCTION CHANGE - allow due dates in the past
                //                if (submitDate < today) {
                //                    alert('Due date/time cannot be lesser than current date/time.');
                //                    return false;
                //                }
            }

            return true;
        },

        IsAssignmentDateSelected: function (contentWrapper) {
            var dueDateParts = contentWrapper.find("#DueDate").val().split("/");
            if (dueDateParts.length < 3) {
                return false;
            }
            return true;
        },

        ContentAssignedAssignmentCenter: function (itemId, unit, dueDateParts, possibleScore, gradebookCategory, onSuccess, onFailure) {
            var points = possibleScore;

            $(PxPage.switchboard).trigger("contentassigned", [{
                    id: itemId,
                    date: dueDateParts,
                    startdate: dueDateParts,
                    points: points,
                    category: "",
                    gradebookcategory: gradebookCategory,
                    unit: unit
                }]);

            if (onSuccess) {
                onSuccess();
            }
        },

        // assignment units
        CreateAssignmentUnit: function (templateId, toc) {
            var parentId = "PX_MULTIPART_LESSONS"; //TODO: this is where we can set the default parent ID for newly created units
            PxPage.Loading();
            PxContentTemplates.CreateItemFromTemplate(templateId, function (item) {
                var itemUrl = PxPage.Routes.display_content + "/" + item.id + "?mode=Edit&includeNavigation=false&toc=" + toc;

                // TODO: change it to use $.ajax so that display_content need not pass many params
                $.get(itemUrl, null, function (response) {
                    $(response).PxNonModal({
                        title: 'Create New Assignment',
                        widthOverride: 800,
                        centerFaceplate: true,
                        onCompleted: function () {
                            PxPage.Loaded();
                            $('#nonmodal-content').find('#Content_DefaultCategoryParentId').val(parentId);
                            $('#nonmodal-content').find('#Content_ParentId').val(parentId);
                            $('#nonmodal-content').find('#Content_Title').val("Untitled Assignment");
                            $('#nonmodal-content').find('#content-item').attr("bfwtype", "AssignmentUnit");

                            if (tinyMCE && tinyMCE.activeEditor) {
                                tinyMCE.activeEditor.setContent('');
                            }

                            $('#Content_SourceTemplateId').val(templateId);
                        }
                    });
                    PxPage.Update();
                });

                if (tinyMCE && tinyMCE.activeEditor) {
                    tinyMCE.activeEditor.setContent('');
                }

            }, parentId, "Unit");
        },

        CreateAndAssign: function () {
            var contentWrapper = $('.contentcreate');

            if (contentWrapper.length == 0) return;

            var dueDateParts = contentWrapper.find("#DueDate").val().split("/");
            if (dueDateParts.length < 3) {
                return;
            }

            var chkAssignmentGradebale = true;
            if (contentWrapper.find('#chkAssignmentGradeable').length > 0) {
                chkAssignmentGradebale = contentWrapper.find('#chkAssignmentGradeable').is(':checked');
            }


            var chkIsMarkAsComplete = true;
            if (contentWrapper.find("#chkMarkAsComplete").length > 0) {
                chkIsMarkAsComplete = contentWrapper.find("#chkMarkAsComplete").is(':checked');
            }

            var chkImportantAssignment = contentWrapper.find('#chkImportantAssignment').is(':checked');
            var courseType = contentWrapper.find('#courseType').val();

            var points = '0';
            if (contentWrapper.find('#Score_Possible').length > 0) {
                points = contentWrapper.find('#Score_Possible').val();
            } else if (contentWrapper.find('#txtGradePoints').length > 0) {
                points = contentWrapper.find('#txtGradePoints').val();
            }

            var chkAllowLateSubmissions = contentWrapper.find('#chkAllowLateSubmissions').is(':checked');
            var chkScheduleEmailReminder = contentWrapper.find('#chkScheduleEmailReminder').is(':checked');


            var dueDate = contentWrapper.find('#DueDate').val();
            // var dueTime = contentWrapper.find('#settingsAssignTime').val();
            //TODO :
            var dueTime = ContentWidget.GetAssignedTime(contentWrapper);
            
            //Email Reminder            
            var durationBefore = 0;
            var durationType = "day";
            var emailSubject = "";
            var emailBody = "";
            if (chkScheduleEmailReminder) {
                durationBefore = contentWrapper.find('#ReminderBeforeCount').val();
                durationType = contentWrapper.find('#ReminderBeforeType option:selected').val();
                emailSubject = contentWrapper.find('#ReminderSubject').val();
                emailBody = contentWrapper.find('.html-editor').val();
            }

            //Late Submission
            var gracePeriodDuration = 0;
            var gracePeriodDurationType = "minute";
            if (chkAllowLateSubmissions) {
                gracePeriodDuration = contentWrapper.find('#LateGracePeriodDuration').val();
                gracePeriodDurationType = contentWrapper.find('#LateGracePeriodDurationType option:selected').val();
            }

            //gbb trigger
            var includeGbbScoreTrigger = contentWrapper.find('#selIncludeGbbScoreTrigger').is(':checked') ? 2 : 1;

            var completiontrigger = '';
            if (contentWrapper.find('#selCompletionTrigger').length > 0) {
                completiontrigger = contentWrapper.find('#selCompletionTrigger option:selected').val();
            }
            var gradeBookCategory = '';
            if (contentWrapper.find('#selgradebookweights').length > 0) {
                gradeBookCategory = contentWrapper.find('#selgradebookweights option:selected').val();
            }
            var calculationTypeTrigger = '';
            if (contentWrapper.find('#selCalculationTypeTrigger').length > 0) {
                calculationTypeTrigger = contentWrapper.find('#selCalculationTypeTrigger option:selected').val();
            }
            var syllabusFilter = '';
            if (contentWrapper.find('#selSyllabusFilter').length > 0) {
                syllabusFilter = contentWrapper.find('#selSyllabusFilter option:selected').val();
            }
            var rubricId = '';
            if (contentWrapper.find('#rubricId').length > 0) {
                rubricId = contentWrapper.find('#rubricId').val();
            }

            return "DueDate=" + dueDate + " " + dueTime + "&Behavior=assign&IsImportant=" + chkImportantAssignment + "&CompletionTrigger=" + completiontrigger + "&IncludeGbbScoreTrigger=" + includeGbbScoreTrigger + "&GradebookCategory=" + gradeBookCategory + "&CalculationTypeTrigger=" + calculationTypeTrigger + "&SyllabusFilter=" + syllabusFilter + "&Points=" + points + "&RubricId=" + rubricId + "&IsGradeable=" + chkAssignmentGradebale + "&IsMarkAsCompleteChecked=" + chkIsMarkAsComplete + "&IsAllowLateSubmission=" + chkAllowLateSubmissions + "&IsSendReminder=" + chkScheduleEmailReminder + "&ReminderDurationCount=" + durationBefore + "&ReminderDurationType=" + durationType + "&ReminderSubject=" + emailSubject + "&ReminderBody=" + emailBody + "&LateGraceDuration=" + gracePeriodDuration + "&LateGraceDurationType=" + gracePeriodDurationType;
        },

        IsAssignDateValid: function (assignedDate, callback) {
            var args = {
                assignedDate: assignedDate
            };

            $.post(PxPage.Routes.contentwidget_isassigndatevalid, args, function (response) {
                if (response == "true") {
                    callback();
                } else {
                    if (confirm("The due date/time is less than current date/time. Are you sure you want to set the due date in the past?")) {
                        callback();
                    }
                }
            });
        },

        ContentAssigned: function (behavior, itemId, callback, contentWrapper, areaToBlock, showAlert) {
            if (areaToBlock) {
                PxPage.Loading(areaToBlock);
            } else {
                PxPage.Loading();
            }

            var currentPoints;
            if (contentWrapper.find('#Score_Possible').length > 0) {
                currentPoints = parseInt(contentWrapper.find('#Score_Possible').val());
            } else if (contentWrapper.find('#txtGradePoints').length > 0) {
                currentPoints = parseInt(contentWrapper.find('#txtGradePoints').val());
            }
            currentPoints = isNaN(currentPoints) ? 0 : currentPoints;

            if (currentPoints <= 0) {
                var completedStudents = $(".faceplate-student-completion-stats").html()[0];
                var msg = "";

                if (completedStudents > 0) {
                    msg = "Setting the points to zero removes the gradebook entry for this item. Existing gradebook contains a student grade.";
                }
                else {
                    msg = "You have chosen to assign a due date without putting this item in the gradebook. Are you sure this is what you want to do? To have the item appear in the gradebook, enter gradebook points and choose a gradebook category.";
                }

                if (!confirm(msg)) {
                    PxPage.Loaded();
                    PxPage.Loaded("content");

                    return false;
                }
            }

            //Cancel out of save if points haven't been set.
            //TODO: Check UC to make sure this is an actual requirement.
            if (currentPoints == 0 && showAlert) {
                if (areaToBlock) {
                    PxPage.Loaded(areaToBlock);
                } else {
                    PxPage.Loaded();
                }
                PxPage.Toasts.Error("Point value hasn't been set");
                return;
            }

            var dueDateParts = $.trim(contentWrapper.find("#DueDate:not(.hdnFullDueDate)").val()).split("/");
            //Why set the date to 1/1/0001.  No due date is an allowed option.
            if (dueDateParts == "") {
                var tempDate = "1/1/0001";
                dueDateParts = tempDate.split("/");;
            }

            if (!itemId) {
                itemId = $('.item-id:last').val();
            }

            var chkAssignmentGradebale = true;
            if (contentWrapper.find('#chkAssignmentGradeable').length > 0) {
                chkAssignmentGradebale = contentWrapper.find('#chkAssignmentGradeable').is(':checked');
            }


            var chkIsMarkAsComplete = true;
            if (contentWrapper.find("#chkMarkAsComplete").length > 0) {
                chkIsMarkAsComplete = contentWrapper.find("#chkMarkAsComplete").is(':checked');
            }

            var chkImportantAssignment = contentWrapper.find('#chkImportantAssignment').is(':checked');
            var courseType = contentWrapper.find('#courseType').val();

            var chkAllowLateSubmissions = contentWrapper.find('#chkAllowLateSubmissions').is(':checked');
            var chkScheduleEmailReminder = contentWrapper.find('#chkScheduleEmailReminder').is(':checked');

            var dueDate = contentWrapper.find('#DueDate:not(.hdnFullDueDate)').val();
            var dueYear = (dueDate === "") ? 0 : new Date(dueDate).getFullYear();

            var dueTime = ContentWidget.GetAssignedTime(contentWrapper);

            var dueHour = contentWrapper.find('#dueHour').val();
            if (dueHour == "") dueHour = "11";

            var dueMinute = contentWrapper.find('#dueMinute').val();
            if (dueMinute == "") dueMinute = "59";

            var syllabusFilter = '';
            if (contentWrapper.find('#selSyllabusFilter').length > 0) {
                syllabusFilter = contentWrapper.find('#selSyllabusFilter option:selected').val();
            } else {
                syllabusFilter = $('#hdnSyllabusCategory').val();
            }

            var dueAmpm = contentWrapper.find('#dueAmpm').val();
            var selCalculationType = contentWrapper.find('#selCalculationTypeTrigger').val();
            var selCompletionTrigger = contentWrapper.find('#selCompletionTrigger').val();

            var timeToComplete = parseInt(contentWrapper.find('#timeToComplete').val());
            if (selCompletionTrigger == "0" && (isNaN(timeToComplete) || timeToComplete <= 0)) {
                if (areaToBlock) {
                    PxPage.Loaded(areaToBlock);
                } else {
                    PxPage.Loaded();
                }
                PxPage.Toasts.Error("Please provide valid time to complete");
                return;
            }

            var passingScore = parseFloat(contentWrapper.find('#passingScore').val());
            if (selCompletionTrigger == "2" && (isNaN(passingScore) || passingScore <= 0 || passingScore > 100)) {
                if (areaToBlock) {
                    PxPage.Loaded(areaToBlock);
                } else {
                    PxPage.Loaded();
                }
                PxPage.Toasts.Error("Please provide valid passing score");
                return;
            }

            var selgradebookweights = contentWrapper.find('#selgradebookweights').val();
            var selSyllabusFilter = contentWrapper.find('#selSyllabusFilter').val();
            var selCalculationTypeTrigger = contentWrapper.find('#selCalculationTypeTrigger').val();


            //Email Reminder            
            var durationBefore = 0;
            var durationType = "day";
            var emailSubject = "";
            var emailBody = "";
            if (chkScheduleEmailReminder) {
                if (tinyMCE) tinyMCE.triggerSave();
                durationBefore = contentWrapper.find('#ReminderBeforeCount').val();
                durationType = contentWrapper.find('#ReminderBeforeType option:selected').val();
                emailSubject = contentWrapper.find('#ReminderSubject').val();
                emailBody = contentWrapper.find('#ReminderEmail_Body').val();
            }

            //Late Submission
            var highlightLatSubmission = false;
            var allowGracePeriodLate = false;
            var gracePeriodDuration = 0;
            var gracePeriodDurationType = "minute";
            if (chkAllowLateSubmissions) {
                allowGracePeriodLate = true;
                gracePeriodDuration = contentWrapper.find('#LateGracePeriodDuration').val();
                gracePeriodDurationType = contentWrapper.find('#LateGracePeriodDurationType option:selected').val();

            }

            //gbb trigger
            var includeGbbScoreTrigger = contentWrapper.find('#selIncludeGbbScoreTrigger').is(':checked') ? 2 : 1;

            var settingsEntityId = "";

            if ($("#SettingsEntityId").length > 0) settingsEntityId = $("#SettingsEntityId").val();

            var isAllowExtraCredit = false;

            if ($("#isAllowExtraCredit").is(':checked')) {
                isAllowExtraCredit = true;
            }

            var gradebookCategory = contentWrapper.find('#selgradebookweights option:selected').val();

            $.ajax({
                url: PxPage.Routes.assign_item,
                data: {
                    itemId: itemId,
                    dueYear: dueYear,
                    dueMonth: dueDateParts[0],
                    dueDay: dueDateParts[1],
                    dueHour: dueHour,
                    dueMinute: dueMinute,
                    dueAmpm: dueAmpm,
                    behavior: behavior,
                    isImportant: chkImportantAssignment,
                    completionTrigger: contentWrapper.find('#hdnCompletionTrigger').val(),
                    IncludeGbbScoreTrigger: includeGbbScoreTrigger,
                    gradebookCategory: gradebookCategory,
                    CalculationTypeTrigger: contentWrapper.find('#selCalculationTypeTrigger option:selected').val(),
                    syllabusFilter: syllabusFilter,
                    points: currentPoints,
                    rubricId: contentWrapper.find('#rubricId').val(),
                    isGradeable: chkAssignmentGradebale,
                    IsMarkAsCompleteChecked: chkIsMarkAsComplete,
                    isAllowLateSubmission: chkAllowLateSubmissions,
                    isSendReminder: chkScheduleEmailReminder,
                    reminderDurationCount: durationBefore,
                    reminderDurationType: durationType,
                    reminderSubject: emailSubject,
                    reminderBody: emailBody,
                    isHighlightLateSubmission: highlightLatSubmission,
                    isAllowLateGracePeriod: allowGracePeriodLate,
                    lateGraceDuration: gracePeriodDuration,
                    lateGraceDurationType: gracePeriodDurationType,
                    isAllowExtraCredit: isAllowExtraCredit,
                    groupId: settingsEntityId,
                    timeToComplete: timeToComplete,
                    passingScore: passingScore
                },
                type: "POST",
                success: function (response) {
                    PxPage.Loaded();
                    if (($('.product-type-lms-faceplate').length > 0 || $('.product-type-faceplate').length > 0)) {
                        PxPage.Loaded("content");
                    }

                    if (response.error) {
                        if (areaToBlock) {
                            $("#" + areaToBlock).removeAttr("style");
                        }
                        PxPage.Toasts.Error(response.error);
                    } else {
                        //update the hidden fields to track the changes to the form
                        contentWrapper.find('#hdnDueDate').val(dueDate);
                        contentWrapper.find('#hdnDueHour').val(dueHour);
                        contentWrapper.find('#hdnDueMinute').val(dueMinute);
                        contentWrapper.find('#hdnDueAmpm').val(dueAmpm);
                        contentWrapper.find('#hdnGradebookCategory').val(selgradebookweights);
                        contentWrapper.find('#hdnSyllabusCategory').val(selSyllabusFilter);
                        contentWrapper.find('#hdnGradable').val(chkAssignmentGradebale);
                        contentWrapper.find("#hdnIsMarkAsComplete").val(chkIsMarkAsComplete);
                        contentWrapper.find('#hdnGradePoints').val(currentPoints);
                        contentWrapper.find('#hdnLateSubmission').val(chkAllowLateSubmissions);
                        contentWrapper.find('#hdnSendReminder').val(chkScheduleEmailReminder);
                        contentWrapper.find('#hdnIncludeGbbScoreTrigger').val(includeGbbScoreTrigger);
                        if (selCompletionTrigger != null && (selCompletionTrigger.length > 0)) {
                            contentWrapper.find('#hdnCompletionTrigger').val(selCompletionTrigger);
                        }
                        if (selCalculationType) {
                            contentWrapper.find('#hdnCalculationTypeTrigger').val(selCalculationType);
                        }
                        //update reminder email hidden fields
                        if (chkScheduleEmailReminder) {
                            contentWrapper.find('#hdnReminderBeforeCount').val(durationBefore);
                            contentWrapper.find('#hdnReminderBeforeType').val(durationType);
                            contentWrapper.find('#hdnReminderSubject').val(emailSubject);
                            contentWrapper.find('#hdnReminderBody').val('true');
                        }
                        //update late submission hidden fields
                        if (chkAllowLateSubmissions) {
                            contentWrapper.find('#hdnAllowLateUntilGracePeriod').val(allowGracePeriodLate);
                            contentWrapper.find('#hdnLateGracePeriodDuration').val(gracePeriodDuration);
                            contentWrapper.find('#hdnLateGracePeriodDurationType').val(gracePeriodDurationType);
                        }

                        
                        ContentWidget.OnAssignSuccess(response, contentWrapper);
                        if (callback) {

                            var data = contentWrapper.find("#DueDate").val() + "|" + contentWrapper.find("#txtGradePoints").val();
                            callback(data);
                        }

                        var localTime = contentWrapper.find("#DueDate").val() + " " + contentWrapper.find('#settingsAssignTime').val()

                        var courseId = $("#CourseId").val();
                        var entityId = (settingsEntityId == courseId || settingsEntityId == "") ? "" : settingsEntityId;

                        if (behavior == "unassign") {
                            $(PxPage.switchboard).trigger("contentunassigned", [{
                                id: itemId,
                                date: '',
                                startdate: '',
                                points: '',
                                keepInGradebook: false,
                                category: "",
                                gradebookcategory: ''
                            }]);

                            if ($("#settingsAssignDueDate").length) {
                                $("#settingsAssignDueDate").val('');
                                $('#settingsAssignTime').val('');
                            }

                            contentWrapper.find('#hdnDueDate').val('');
                            contentWrapper.find('#hdnDueHour').val('0');
                            contentWrapper.find('#hdnDueMinute').val('0');
                            contentWrapper.find('#hdnDueAmpm').val('am');

                            contentWrapper.find('#hdnIsAssigned').val("false");

                            $(".view-tab a").click();
                        }
                        else {
                            $(PxPage.switchboard).trigger("contentassigned", [{
                                id: itemId,
                                date: localTime,
                                category: syllabusFilter,
                                points: currentPoints,
                                gradebookcategory: gradebookCategory,
                                entityId: entityId
                            }]);

                            $(".view-tab a").click();
                        }
                    }

                    ContentWidget.NavigateAway();
                }
            });
        },

        InitSettingsTab: function () {
            PxPage.UpdateFneSize();
            PxSettingsTab.Init();
            PxPage.SetFrameApiHooks();

            $('#fne-window').removeClass('require-confirm');
        },

        ValidateConfirmation: function (callBack, navigateTo) {
            $("#dialog-confirm").dialog({
                resizable: false,
                height: 150,
                width: 500,
                modal: true,
                dialogClass: '',
                buttons:
                [{
                    text: "Save",
                    click: function () {
                        $('#fne-window').removeClass('require-confirm');
                        $(this).dialog("close");

                        var link = $(".submit-action").not(':disabled').first();

                        if (link.length == 0) {
                            link = $('.savebtn');
                        }

                        if (link != undefined && link.length) {
                            $('#fne-window').attr('navigateTo', navigateTo);

                            if (link.attr('onclick')) {
                                try {
                                    if (!eval(link.attr('onclick'))) {
                                        return false;
                                    }
                                    callBack();
                                }
                                catch (ex) {
                                }
                            }
                            else {
                                link.trigger('click');
                                callBack();
                            }
                        }
                    }
                },
                {
                    text: "Cancel",
                    click: function () {
                        $(this).dialog("close");
                        return false;
                    }
                },
                {
                    text: "Don't Save",
                    click: function () {
                        $('#fne-window').removeClass('require-confirm');
                        callBack();
                        $(this).dialog("close");

                        PxPage.TriggerClick($('#' + navigateTo)); //force click the node in order to trigger hash change
                    }
                }]
            });
        },

        InitResultsTab: function () {
            PxPage.UpdateFneSize();
            PxPage.SetFrameApiHooks();
            $(window).trigger('resize');
        },

        InitMoreResourcesTab: function () {
            PxPage.SetFneLinks();
            PxPage.UpdateFneSize();
            ContentWidget.AddResourceDraggable();
            $(window).trigger('resize');
        },

        InitMoreResourcesStaticTab: function () {
            PxPage.SetFneLinks();
            PxPage.UpdateFneSize();
            ContentWidget.AddResourceDraggable();
            $(window).trigger('resize');
        },

        //This is added for handling validations when submitted from modal windows (e.g. Create Folder in Bookmark etc.). Added as part of PLATX-3757
        ValidateContentCreated: function (ac) {
            //ac -- ajaxContext Object
            if (ac) {
                var responseText = ac.get_response().get_responseData();
                if (responseText != null && responseText.indexOf('field-validation-error') != -1) {
                    $(responseText).PxNonModal({
                        title: 'Create New',
                        onCompleted: ContentWidget.AddItemToActiveNode.completed
                    });
                    PxPage.Update();
                    $('#content-item').hide();
                } else {
                    // no validation messages
                    ContentWidget.ContentCreated(null);
                    $('#content-item').show();
                }
            }
            PxPage.Loaded();
        },
        //This is to retrict alphabets and special charecters during the keypress event
        AllowNumbersOnly: function (event) {

            // Backspace, tab, enter, end, home, left, right
            // We don't support the del key in Opera because del == . == 46.
            var controlKeys = [8, 9, 13, 35, 36, 37, 39];
            // IE doesn't support indexOf
            var isControlKey = controlKeys.join(",").match(new RegExp(event.which));
            // Some browsers just don't raise events for control keys. Easy.
            // e.g. Safari backspace.
            if (!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
      (event.which >= 48 && event.which <= 57) || isControlKey

                //      || // Always 1 through 9
                //       (event.which == 46 && $(event.target).val().indexOf(".") == -1)

       ) { // Opera assigns values for control keys.
                return true;
            } else {
                event.preventDefault();
                return false;
            }
        },
        BlockContent: function (elm) {
            if (elm == '') $('#right').block({
                message: null
            });
            else $(elm).block({
                message: null
            });
        },

        UnblockContent: function (elm) {
            if (elm == '') $('#right').unblock();
            else $(elm).unblock();
        },

        ClearHtmlEditor: function () {
            tinyMCE.activeEditor.setContent('');
        },

        NextItem: function (event) {
            $(PxPage.switchboard).trigger("tocnext");
        },

        BackItem: function (event) {
            $(PxPage.switchboard).trigger("tocprev");
        },

        SetSyllabusFilter: function () {
            var selectedItem = $("#Syllabus_ChildrenFilterSections :selected").val();
            $('#SyllabusFilter').val(selectedItem);
            return true;
        },

        ValidatePoints: function (ob, maxScore) {
            var floatRegex = /^(0|[1-9][0-9]*|[1-9][0-9]{0,2}(,[0-9]{3,3})*)$/;
            var assignedScore = '-1';

            assignedScore = $('#Score_Possible').val();
            assignedScore = jQuery.trim(assignedScore);
            if (assignedScore == '') {
                assignedScore = -1;
            }
            if (!isNaN(assignedScore)) {
                assignedScore = assignedScore * 1;
            } else {
                assignedScore = -1;
            }

            if (assignedScore < 0 || assignedScore > maxScore) {
                PxPage.Toasts.Warning("Please enter a Points between 0 and " + maxScore);
                setTimeout('document.getElementById(\'Score_Possible\').focus();document.getElementById(\'Score_Possible\').select();', 0);
                return false;
            }

            return true;
        },

        OnUploadComplete: function (response) {
            PxPage.Loading();

            var docContentView = $("#fne-window .doc-collection-content-view, #nonmodal .doc-collection-content-view");
            var isDocumentCollection = docContentView.length > 0;
            var isFacePlate = $('.product-type-faceplate').length > 0;
            var isXbook = $('.product-type-xbook').length > 0;
            var contentItem = $("#content-item");

            if (isDocumentCollection) {
                contentItem = $("#documentList");
            }
            else {
                if (!contentItem.is(":visible")) {
                    contentItem = $('#nonmodal-content').find("#content-item");
                }
            }

            contentItem.html(response);

            if (!isFacePlate && !isXbook) {
                ContentWidget.ContentCreated(null);
            }

            PxPage.Loaded();

            if (isDocumentCollection) {
                if (PxUpload) {
                    PxUpload.CloseDialog();
                }
            }
        },

        GetParameterByName: function (name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regexS = "[\\?&]" + name + "=([^&#]*)";
            var regex = new RegExp(regexS);
            var results = regex.exec(window.location.href);
            return (results == null) ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        },

        // if Assignment Gradable checkbox is checked then show the Grade Points textbox
        AssignmentGradeable: function (contentWrapper) {
            var checked = contentWrapper.find('#chkAssignmentGradeable').is(':checked');
            if (checked) {
                contentWrapper.find('#divGradePoints').show();
                contentWrapper.find('#divGradebookCategory').show();
                contentWrapper.find('#divIncludeGbbScore').show();
                contentWrapper.find('#divCalculationType').show();
            } else {
                contentWrapper.find('#divGradePoints').hide();
                contentWrapper.find('#txtGradePoints').val('0');
                contentWrapper.find('#divGradebookCategory').hide();
                contentWrapper.find('#divGradebookCategory').val(0);
                contentWrapper.find('#divIncludeGbbScore').hide();
                contentWrapper.find('#divIncludeGbbScore').val(0);
                contentWrapper.find('#divCalculationType').hide();
            }

            ContentWidget.IsFormChanged(contentWrapper);
        },

        // if Mark as completed checked - show dropdown
        MarkAsCompleted: function (contentWrapper) {
            var markAsCompletedChecked = contentWrapper.find("#chkMarkAsComplete").is(':checked');
            if (markAsCompletedChecked) {
                contentWrapper.find("#divShowMarkAsComplete").show();
            } else {
                contentWrapper.find("#divShowMarkAsComplete").hide();
            }

            ContentWidget.IsFormChanged(contentWrapper);
        },


        //if 'Due date and time' checkbox is checked
        DueDateChanged: function (contentWrapper, chkDueDate) {
            var isChecked = chkDueDate.is(':checked');
            if (isChecked) {
                contentWrapper.find('#fsDateTime').show();
            } else {
                contentWrapper.find('#fsDateTime').hide();
            }

        },

        showGradeBookCategory: function () {
            if ($('#txtGradePoints').length > 0) {
                var txtGradePoints = $('#txtGradePoints').val();

                if (txtGradePoints == "0" || txtGradePoints == "") {
                    txtGradePoints = "";
                    $("#trGradeBookCategory").hide();
                }
                else {
                    $("#trGradeBookCategory").show();
                }
            }

            return true;
        },

        ShowAddNewGradeBookScreen: function () {
            $('#ggbcategorydialog_link').click();
        },

        OnGradeBookCategoryChange: function () {

            if ($(this).val() == "createNewCat") {
                $('#ggbcategorydialog_link').click();

            }

        },

        EmailReminderClicked: function (isChecked, assignmentSettingsClass) {
            if (assignmentSettingsClass == null) {
                assignmentSettingsClass = $("#assignment-settings");
            }

            if (isChecked) {
                assignmentSettingsClass.find('#reminderEmailDeatils').show();
                assignmentSettingsClass.find('#ReminderBeforeCount').removeAttr("disabled");
                assignmentSettingsClass.find('#ReminderBeforeType').removeAttr("disabled");
                assignmentSettingsClass.find('#ReminderSubject').removeAttr("disabled");
            } else {
                assignmentSettingsClass.find('#reminderEmailDeatils').hide();
                assignmentSettingsClass.find('#ReminderBeforeCount').attr("disabled", "disabled");
                assignmentSettingsClass.find('#ReminderBeforeType').attr("disabled", "disabled");
                assignmentSettingsClass.find('#ReminderSubject').attr("disabled", "disabled");
            }
        },

        //Detects any changes to the form fields after a submission for an eportfolio course type
        IsFormChanged: function (contentWrapper) {

            $(document).off('change', "#selgradebookweights").on("change", "#selgradebookweights", ContentWidget.OnGradeBookCategoryChange);

            if (contentWrapper == null || $(contentWrapper).length < 1 || $(contentWrapper).is(':visible') == false) {
                contentWrapper = $(this).closest('.contentwrapper');
                if ($(contentWrapper).length < 1 && $('.contentcreate:visible').length > 0) {
                    contentWrapper = $('.contentcreate');
                } else {
                    contentWrapper = $('.assigntab');
                }
            }

            //if the item is not assigned, the save changes button is not shown

            var isAssigned = 'false';
            if (contentWrapper.find('#hdnIsAssigned').length > 0) {
                isAssigned = contentWrapper.find('#hdnIsAssigned').val().toLowerCase();
            }

            var dueDate = contentWrapper.find('#DueDate').val();
            var hdnDueDate = contentWrapper.find('#hdnDueDate').val();

            var dueHour = contentWrapper.find('#dueHour').val();
            var hdnDueHour = contentWrapper.find('#hdnDueHour').val();

            var dueMinute = contentWrapper.find('#dueMinute').val();
            var hdnDueMinute = contentWrapper.find('#hdnDueMinute').val();

            var dueAmpm = contentWrapper.find('#dueAmpm').val();
            var hdnDueAmpm = contentWrapper.find('#hdnDueAmpm').val();

            var blnScorePossible = false;
            if (contentWrapper.find('#Score_Possible').length > 0) {
                var scorePossible = contentWrapper.find('#Score_Possible').val();
                var hdnGradePoints = contentWrapper.find('#hdnGradePoints').val();
                blnScorePossible = (scorePossible != hdnGradePoints);
            }

            var blnCompletionTrigger = false;
            if (contentWrapper.find('#selCompletionTrigger').length > 0) {
                var selCompletionTrigger = contentWrapper.find('#selCompletionTrigger').val();
                var hdnCompletionTrigger = contentWrapper.find('#hdnCompletionTrigger').val();
                blnCompletionTrigger = (selCompletionTrigger != hdnCompletionTrigger);

                if (selCompletionTrigger == "0") {
                    contentWrapper.find('#timeToCompleteArea').show();
                }
                else {
                    contentWrapper.find('#timeToCompleteArea').hide();
                }

                if (selCompletionTrigger == "2") {
                    contentWrapper.find('#passingScoreArea').show();
                }
                else {
                    contentWrapper.find('#passingScoreArea').hide();
                }
            }

            var blnIncludeGbbScoreTrigger = false;
            if (contentWrapper.find('#selIncludeGbbScoreTrigger').length > 0) {
                var selIncludeGbbScoreTrigger = contentWrapper.find('#selIncludeGbbScoreTrigger').is(':checked') ? 2 : 1;
                var hdnIncludeGbbScoreTrigger = contentWrapper.find('#hdnIncludeGbbScoreTrigger').val();
                blnIncludeGbbScoreTrigger = (selIncludeGbbScoreTrigger != hdnIncludeGbbScoreTrigger);
            }

            var blnCalculationTypeTrigger = false;
            if (contentWrapper.find('#selCalculationTypeTrigger').length > 0) {
                var selCalculationTypeTrigger = contentWrapper.find('#selCalculationTypeTrigger').val();
                var hdnCalculationTypeTrigger = contentWrapper.find('#hdnCalculationTypeTrigger').val();
                blnCalculationTypeTrigger = (selCalculationTypeTrigger != hdnCalculationTypeTrigger);
            }

            var blnGradebookCategory = false;
            if (contentWrapper.find('#selgradebookweights').length > 0) {
                var selgradebookweights = contentWrapper.find('#selgradebookweights').val();
                var hdnGradebookCategory = contentWrapper.find('#hdnGradebookCategory').val();
                blnGradebookCategory = (selgradebookweights != hdnGradebookCategory);
            }

            var blnSyllabusCategory = false;
            if (contentWrapper.find('#selSyllabusFilter').length > 0) {
                var selSyllabusFilter = contentWrapper.find('#selSyllabusFilter').val();
                var hdnSyllabusCategory = contentWrapper.find('#hdnSyllabusCategory').val();
                blnSyllabusCategory = (selSyllabusFilter != hdnSyllabusCategory);
            }

            var blnAssignmentGradable = false;
            var chkAssignmentGradeable;
            var hdnGradable;
            if (contentWrapper.find('#chkAssignmentGradeable').length > 0) {
                chkAssignmentGradeable = contentWrapper.find('#chkAssignmentGradeable').is(':checked').toString();
                hdnGradable = contentWrapper.find('#hdnGradable').val().toLowerCase();
                blnAssignmentGradable = (chkAssignmentGradeable != hdnGradable);
            }

            var blnIsMarkAsCompleted = false;
            if (contentWrapper.find('#chkMarkAsComplete').length > 0) {
                var chkMarkAsComplete = contentWrapper.find('#chkMarkAsComplete').is(':checked').toString();
                var hdnMarkAsComplete = contentWrapper.find('#hdnIsMarkAsComplete').val().toLowerCase();
                blnIsMarkAsCompleted = (chkMarkAsComplete != hdnMarkAsComplete);
            }


            var blnGradePoints = false;
            var txtGradePoints;
            var hdnGradePoints;
            if (contentWrapper.find('#txtGradePoints').length > 0) {
                txtGradePoints = contentWrapper.find('#txtGradePoints').val();
                hdnGradePoints = contentWrapper.find('#hdnGradePoints').val();

                if (hdnGradePoints == "0") {
                    hdnGradePoints = "";
                }

                if (txtGradePoints == "0" || txtGradePoints == "") {
                    txtGradePoints = "";
                    $("#trGradeBookCategory").hide();
                }
                else {
                    $("#trGradeBookCategory").show();
                }

                blnGradePoints = (txtGradePoints != hdnGradePoints);
            }

            var blnAllowLateSubmissions = false;
            if (contentWrapper.find('#chkAllowLateSubmissions').length) {
                var chkAllowLateSubmissions = contentWrapper.find('#chkAllowLateSubmissions').is(':checked').toString();
                var hdnLateSubmission = contentWrapper.find('#hdnLateSubmission').val().toLowerCase();
                blnAllowLateSubmissions = (chkAllowLateSubmissions != hdnLateSubmission);

                var allowLateGracePeriod = contentWrapper.find('#ckhAllowLateUntilGracePeriod').is(':checked').toString();
                var hdnAllowLateGracePeriod = contentWrapper.find('#hdnAllowLateUntilGracePeriod').val().toLowerCase();

                var lateGraceDuration = contentWrapper.find('#LateGracePeriodDuration').val();
                var hdnLateGraceDuration = contentWrapper.find('#hdnLateGracePeriodDuration').val();

                var lateGraceDurationType = contentWrapper.find('#LateGracePeriodDurationType').val();
                var hdnLateGraceDurationType = contentWrapper.find('#hdnLateGracePeriodDurationType').val();

                if (lateGraceDurationType === 'Infinite')
                    contentWrapper.find('#LateGracePeriodDuration').prop('disabled', true);
                else
                    contentWrapper.find('#LateGracePeriodDuration').prop('disabled', false);

                blnAllowLateSubmissions = blnAllowLateSubmissions || (chkAllowLateSubmissions == 'true' && (allowLateGracePeriod != hdnAllowLateGracePeriod || lateGraceDuration != hdnLateGraceDuration || lateGraceDurationType != hdnLateGraceDurationType))
            }

            var blnSendReminder = false;


            if (contentWrapper.find('#chkScheduleEmailReminder').length > 0) {
                var chkScheduleEmailReminder = contentWrapper.find('#chkScheduleEmailReminder').is(':checked').toString();
                var hdnSendReminder = contentWrapper.find('#hdnSendReminder').val().toLowerCase();
                blnSendReminder = (chkScheduleEmailReminder != hdnSendReminder);

                var durCount = contentWrapper.find('#ReminderBeforeCount').val();
                var hdnDurCount = contentWrapper.find('#hdnReminderBeforeCount').val();

                var durType = contentWrapper.find('#ReminderBeforeType').val();
                var hdnDurType = contentWrapper.find('#hdnReminderBeforeType').val();

                var mailSubject = contentWrapper.find('#ReminderSubject').val();
                var hdnMailSubject = contentWrapper.find('#hdnReminderSubject').val();

                var hdnReminderBody = contentWrapper.find('#hdnReminderBody').val() == 'false';


                blnSendReminder = blnSendReminder || ((chkScheduleEmailReminder == 'true') && ((tinyMCE.activeEditor != null && tinyMCE.activeEditor.isDirty() && hdnReminderBody) || (durCount != hdnDurCount) || (durType != hdnDurType) || (mailSubject != hdnMailSubject)));
            }

            var isChanged = false;
            if (isAssigned == 'true') {
                isChanged = (dueDate != hdnDueDate || dueHour != hdnDueHour || dueMinute != hdnDueMinute || dueAmpm.toLowerCase() != hdnDueAmpm.toLowerCase() || blnCompletionTrigger || blnCalculationTypeTrigger || blnGradebookCategory || blnSyllabusCategory || blnScorePossible || blnAssignmentGradable || blnGradePoints || blnIsMarkAsCompleted || blnAllowLateSubmissions || blnSendReminder);

                if (isChanged) {
                    $('.btnSaveChanges').removeAttr('disabled');

                    var fneVisible = $("#fne-window").attr("style").indexOf("display: none");
                    if (fneVisible < 0) {
                        fneVisible = $("#fne-window").attr("style").indexOf("display:none");
                    }

                    var isFNEHidden = (parseInt(fneVisible) > -1);
                    if (isFNEHidden == true) {
                        $('#fne-window').removeClass('require-confirm');
                    }
                } else {
                    $('.btnSaveChanges').attr('disabled', 'disabled');
                }

                var currentFields = {
                    fields: {
                        dueDate: dueDate,
                        dueHour: dueHour,
                        dueMinute: dueMinute,
                        dueAmpm: dueAmpm,
                        gradable: chkAssignmentGradeable,
                        gradePoints: txtGradePoints
                    }
                };
                var prevFields = {
                    fields: {
                        dueDate: hdnDueDate,
                        dueHour: hdnDueHour,
                        dueMinute: hdnDueMinute,
                        dueAmpm: hdnDueAmpm,
                        gradable: hdnGradable,
                        gradePoints: hdnGradePoints
                    }
                };

                ContentWidget.Fade(currentFields, prevFields, contentWrapper);
            } else {

                // the "Assign" button will be enabled if the due date is set.
                isChanged = (dueDate != hdnDueDate || dueHour != hdnDueHour || dueMinute != hdnDueMinute || dueAmpm != hdnDueAmpm);

                if (isChanged) {
                    $('.btnAssign').removeAttr('disabled');
                    $('.placeholderWrap').addClass('placeholder-changed');
                    var fneVisible = $("#fne-window").attr("style").indexOf("display: none");
                    if (fneVisible < 0) {
                        fneVisible = $("#fne-window").attr("style").indexOf("display:none");
                    }

                    var isFNEHidden = (parseInt(fneVisible) > -1);
                    if (isFNEHidden == true) {
                        $('#fne-window').removeClass('require-confirm');
                    }

                } else {
                    $('.btnAssign').attr('disabled', 'disabled');
                }
            }

        },

        Fade: function (currentFields, prevFields, contentWrapper) {
            var dueDateField = contentWrapper.find('#liDueDate #DueDate');
            var dueHourTime = contentWrapper.find('#liDateTime #dueHour');
            var dueMinuteTime = contentWrapper.find('#liDateTime #dueMinute');
            var dueMeridian = contentWrapper.find('#liDateTime #dueAmpm');
            var assignGradable = contentWrapper.find('#fsGradebook #liAssignmentGradable');
            var gradePoints = contentWrapper.find('#fsGradebook #txtGradePoints');

            if (currentFields.fields.dueDate != prevFields.fields.dueDate) {
                dueDateField.addClass("fade-effect");
                PxPage.Fade();
            }
            if (currentFields.fields.dueHour != prevFields.fields.dueHour) {
                dueHourTime.addClass("fade-effect");
                PxPage.Fade();
            }
            if (currentFields.fields.dueMinute != prevFields.fields.dueMinute) {
                dueMinuteTime.addClass("fade-effect");
                PxPage.Fade();
            }
            if (currentFields.fields.dueAmpm != prevFields.fields.dueAmpm) {
                dueMeridian.addClass("fade-effect");
                PxPage.Fade();
            }
            if (currentFields.fields.gradable != prevFields.fields.gradable) {
                assignGradable.addClass("fade-effect");
                PxPage.Fade();
            }
            if (currentFields.fields.gradePoints != prevFields.fields.gradePoints) {
                gradePoints.addClass("fade-effect");
                PxPage.Fade();
            }
        },

        HashHasChanged: function () {
            var hash = window.location.hash;
            hash = hash.substring(hash.lastIndexOf('#') + 1);
            var itemUrl = PxPage.Routes.display_content + "?id=" + hash + "&mode=Preview&includeToc=False&includeDiscussion=True&isStudentUpdated=False";
            displayItem(itemUrl);
            return;
        },

        AddContentFromAssignTab: function (templateId) {
            PxPage.Loading();
            //set the flag to indicate that the content template creation is happening from Assign Tab
            $('#hdnAssignTabContentCreate').val('true');
            $(PxPage.switchboard).trigger("openactivenode", function () {
                PxContentTemplates.CreateItemFromTemplate(templateId, function (item) {
                    ContentWidget.AddItemToActiveNode(item);
                });
            });
        },

        LoadContentModes: function (renderContentModesInFne) {
            if (renderContentModesInFne || renderContentModesInFne == null) {
                PxPage.Loaded();
                PxPage.Loaded("#content-item");
            }
            else {
                PxPage.TriggerFNELoaded();
                PxPage.Loaded("#content-item");
            }
        },

        ShowSaveMessage: function () {
            if ($('#isShowOnSuccessMessage').val() == 'true') {
                PxPage.Toasts.Success('Your changes have been saved');
            }

            $(PxPage.switchboard).trigger("contentSettingsSaved");
            $('#fne-window').removeClass('require-confirm');

            ContentWidget.NavigateAway();
        },

        NavigateAway: function () {
            if ($('#fne-window').attr('navigateTo') != undefined && $('#fne-window').attr('navigateTo').length > 0) {
                var navigateTo = $('#fne-window').attr('navigateTo');
                $('#fne-window').removeAttr('navigateTo');

                PxPage.TriggerClick($('#' + navigateTo)); //force click the node in order to trigger hash change
            }
        },

        ShowSearchRoster: function () {

            var tag = $("<div id='px-dialog'></div>"); //This tag will the hold the dialog content.

            var options = {
                modal: true,
                draggable: false,
                closeOnEscape: true,
                width: '1000px',
                height: '900px',
                resizable: false,
                autoOpen: false,
                title: 'Search Class Roster'
            };

            tag.dialog({
                modal: options.modal,
                title: options.title,
                draggable: options.draggable,
                closeOnEscape: options.closeOnEscape,
                width: options.width,
                resizable: options.resizable,
                autoOpen: options.autoOpen,
                close: function () {
                    $(this).dialog('destroy').remove();
                }
            }).dialog('open');

            tag.load(PxPage.Routes.assignmentsettings_search_class_roster, null, function (data, textStatus, XMLHttpRequest) {
                $(".divAddIndividual").show();
                $(".field-validation-error").hide();

                tag.dialog("option", "position", "center");

                tag.dialog("widget").css("position", "fixed");
                tag.dialog("widget").css("top", ($(window).height() / 2 - (tag.dialog("widget").height() / 2)));

                PxSettingsTab.AutoComplete();
            });
        },

        BindRequireConfirmControls: function () {
            if (!$('.quiz-editor').length) { // disable 'require-confirm' functionality for question editor
                $('input').
                    not('#txtSearchQuiz, #uploadTitle, #uploadFile').
                    bind('keypress', function () {
                        if ($('#fne-window').is(':visible')) {
                            $('#fne-window').addClass('require-confirm');
                        }
                    });

                $('select,input').
                not('#txtSearchQuiz, #uploadTitle, #uploadFile, #ddlSettingsList, .highlight-public-private, #more-resources-search-box').
                    bind('keyup', function () {
                        if ($('#fne-window').is(':visible')) {
                            $('#fne-window').addClass('require-confirm');
                        }
                    });
            }

            $('.mceIframeContainer').find('iframe').contents().find('body').bind('keyup', function () {
                $('#fne-window').addClass('require-confirm');
            });
        },
        
        MakeContentItemGradable: function (itemId, isSubmitted) {
            /// <summary>
            /// Makes a content item able to appear in the brain honey gradebook.
            /// </summary>
            /// <param name="itemId" type="String">The id of the content item to make gradable</param>
            /// <param name="isSubmitted" type="Boolean">If true will only make item gradable if the item 
            /// has a submission associated with it</param>
            
            $.ajax({
                url: isSubmitted ? PxPage.Routes.make_quiz_gradable_if_submitted : PxPage.Routes.make_quiz_gradable,
                type: 'POST',
                data: {
                    itemId: itemId
                },
                success: function (response) {
                    if (response === true) {
                        PxPage.log('Item ' + itemId + ' has been made gradable');
                        $(PxPage.switchboard).trigger('contentgradable', [itemId]);
                    } else {
                        PxPage.log('Item ' + itemId + ' was not made gradable');
                    }
                },
                error: function () {
                    PxPage.log('Error making item ' + itemId + ' gradable');
                }
            });
        }
    };
}(jQuery);