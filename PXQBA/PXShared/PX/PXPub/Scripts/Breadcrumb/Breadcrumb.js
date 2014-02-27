var PxBreadcrumb = function ($) {
    return {
        Init: function () {

            $('a.path-item').each(function () {
                    $(this).qtip({
                        content: {
                            text: $(this).parent().find('.bread-crumb-siblings-list')
                        },
                        hide: {
                            event: 'unfocus'
                        },
                        show: {
                            ready: false,
                            solo: true
                        },
                        position: {
                            at: 'bottom center'
                        },
                        style: {
                            width: {
                                min: 140,
                                max: 300
                            }
                        },
                        events: {
                            show: function (event) {
                                ContentWidget.BreadCrumbResize(null, $(event.target).find('.qtip-content'))
                            }
                        }
                    }).click(function (event) {
                        event.stopPropagation();
                    });
                }
            );

            $('.path-item, .bread-crumb-siblings-list a.sibling-item').click(function (event) {

                $('.qtip').remove();
                $('.bread-crumb-siblings-list').hide();
                var container = PxBreadcrumb.GetContainer(event.target);
                var id = event.target.id.replace('-bcitem', '');
                var categoryId;
                var courseType = 1;
                var quizContent = ($('#fne-content .quiz-editor').length > 0 && $('#fne-content .quiz-editor').is(':visible'));
                if (($('.product-type-lms-faceplate').length > 0 || $('.product-type-faceplate').length > 0) && !quizContent) {
                    courseType = 5;
                }


                var displayItem = function () {

                    if (courseType == 5) {
                        PxPage.LargeFNE.ShowItemInFNE(id);
                        //$(PxPage.switchboard).trigger("fneclicknode", id);
                    } else {

                        PxBreadcrumb.LoadItem(
                            container,
                            id,
                            [container.find('.root-item').val()],
                            categoryId,
                            courseType,
                            function () {
                                $(PxPage.switchboard).trigger("breadcrumb.selectionChanged", event.target.id.replace('-bcitem', ''));

                            }
                        );
                    }
                };

                if ($('#view_mode').length == 0) {
                    categoryId = "";
                }

                displayItem();
                $(PxPage.switchboard).bind("updateBreadcrumbTitle", function() {
                    var selectedItemTitle = $(".quiz-editor .available-questions .breadcrumb-trail .selected-item-title").val();
                    if (selectedItemTitle != undefined && selectedItemTitle != null) {
                        $(".available-questions-header .quiz-bank-title").text(selectedItemTitle);
                    }
                });


                event.preventDefault();
            });

            // TODO: This should ideally be on the resize event of any given parent container.
            $(window).resize(function () {
                PxBreadcrumb.RunResizers();
            });
        },

        RunResizers: function () {
            $('.breadcrumb').each(function (i, v) {
                PxBreadcrumb.FitText($(v));
                $(PxPage.switchboard).trigger("updateBreadcrumbTitle");
            });
        },

        FitText: function (container) {
            PxPage.FitText(container, 'a.path-item');
        },

        LoadItem: function (container, itemId, rootId, cataegoryId, courseType, callback) {
            //            // not adding breadcrumb if the item is not in the faceplate course
            //            if ($(".faceplate-fne-add-item-bmr-text").text().length && $(".faceplate-fne-add-item-bmr-text").is(":visible")) {
            //                return;
            //            }

            var isQuestionBank = false;
            var currentQuizId = "";
            if ($(".quiz-editor .available-questions .available-questions-header").is(":visible")) {
                isQuestionBank = true;
                currentQuizId = $("#content-item-id").text();
            }

            var data = { "itemId": itemId,
                "faceplateCategory": cataegoryId,
                "courseType": courseType,
                isQuestionBank: isQuestionBank,
                quizId: currentQuizId
            };

            for (var i = 0; i < rootId.length; ++i) {
                data["rootItemIds[" + i + "]"] = rootId[i];
            }

            $.get(PxPage.Routes.render_trail, data, function (response) {
                //container = $('#fne-content .breadcrumb'); //overwrite container to ensure it is not overwridden by content call
                if (container.hasClass('breadcrumb') && $(response).hasClass('breadcrumb')) {
                    container.empty().replaceWith(response);
                }
                else {
                    container.empty().append(response);
                }
                PxBreadcrumb.FitText(container);
                $(PxPage.switchboard).trigger("updateBreadcrumbTitle");
                if (callback) {
                    callback();
                }
            });
        },

        GetSelectedItem: function (container) {
            return container.find('input.selected-item').val();
        },

        GetSelectedItemName: function (container) {
            return $('#' + PxBreadcrumb.GetSelectedItem(container) + '-bcitem').text();
        },

        GetRoot: function (container) {
            return $('.breadcrumb input.root-item').val();
        },

        GetContainer: function (target) {
            // If the target was a breadcrumb link (not one in the menu) then we don't have to do anything fancy
            if ($(target).hasClass('path-item')) {
                return $(target).closest('.breadcrumb');
            }
            var levelId = $(target).closest('.bread-crumb-siblings-list')[0].id + "-level";
            return $('#' + levelId).closest('.breadcrumb');
        }
    };
} (jQuery);
