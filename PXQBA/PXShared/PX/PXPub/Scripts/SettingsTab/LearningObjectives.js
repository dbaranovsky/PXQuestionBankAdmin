var PxLearningObjectives = function ($) {
    return {
        BindControls: function () {

            $("#add-objective-link").live("click", PxLearningObjectives.OnAddClicked);
            $("#correlate-objective-button").live("click", PxLearningObjectives.OnCorrelateClicked);
            $("#close-objective-link").live("click", PxLearningObjectives.OnCloseObjectiveClicked);
            $("#delete-objective-button").live("click", PxLearningObjectives.OnDeleteObjectiveClicked);
            $(".remove-objective-link").live("click", PxLearningObjectives.OnRemoveObjectiveClicked);
            $("#show-objective-links").live("click", PxLearningObjectives.OnShowObjectiveClicked);

            $("#add-objective-dialog").dialog({
                title: "Add Learning Objective",
                dialogClass: "objective-modul",
                autoOpen: false,
                modal: true,
                buttons: {
                    "ADD": function () {
                        var numObjectives = $("#objective-list > div").size();
                        var title = jQuery.trim($('.ui-dialog #objectiveTitle').val());
                        if (title == '') {
                            PxPage.Toasts.Warning('Title cannot be blank');
                            $('#objectiveTitle').focus();
                            return false;
                        }

                        $("#add-objective-dialog").dialog("close");
                        PxPage.Loading();

                        if (numObjectives == 0) {
                            $("#no-objective-message").hide();
                            $("#correlate-objective-button").show().css('visibility', 'visible');
                        }

                        $.post(PxPage.Routes.add_new_objective, $(".ui-dialog #add-objective-form").serialize(), function (data) {
                            $("#objective-list").append(data);
                            $("#add-objective-dialog #objectiveTitle").val('');
                            PxPage.Loaded();
                        });
                    }
                }
            });
        },
        OnAddClicked: function (event) {
            event.preventDefault();
            $("#add-objective-dialog").dialog('open');
            return false;
        },
        OnCorrelateClicked: function (event) {
            PxPage.Loading();
            $.post(PxPage.Routes.update_objectives, $("#objective-list-form").serialize(), function (data) {
                $("#objective-tags").html(data);
                PxPage.Loaded();

            });
        },
        OnCloseObjectiveClicked: function (event) {
            event.preventDefault();
            var numChecked = $("#objective-list-form :checked").length;
            var totalCheckboxed = $("#objective-list-form :checkbox").length;
            var serializedForm = $("#objective-list-form").serialize();

            $("#objective-form").hide();
        },
        OnDeleteObjectiveClicked: function (event) {
            event.preventDefault();
            var numChecked = $("#objective-list-form :checked").length;
            var totalCheckboxed = $("#objective-list-form :checkbox").length;
            var serializedForm = $("#objective-list-form").serialize();

            $.post(PxPage.Routes.delete_objectives, serializedForm, function (data) {
                //$("#objective-form").html(data);            
            });
        },
        OnRemoveObjectiveClicked: function (event) {
            $(this).parent('span').remove();
            var id = "#obj_" + $(this).parent('span').attr('id');
            $(id).find('input[type="checkbox"]').prop('checked', false);
        },
        OnShowObjectiveClicked: function (event) {
            $("#objective-form").show();
        },
        Init: function () {
            PxLearningObjectives.BindControls();
        }
    };
} (jQuery);

