//jQuery plugin for Quiz Preview functionality

(function ($) {
    var _static = {
        pluginName: "quizhomework",
        dataKey: "quizhomework",
        bindSuffix: ".quizhomework",
        dataAttrPrefix: "data-qh-",
        //plugin defaults
        defaults: {
            
        },
        //css settings
        css: {
            
        },
        //selectors for commonly accessed elements
        sel: {
            changeAttemptsLnk: "ul.questions li.question .total-attempts a.link-attempt",
            saveAttempts: "ul.questions li.question .total-attempts .questions-attempts"
        },
        //private functions
        fn: {
            AttemptClick: function (event) {
                $(".attempt-textbox").hide();
                $(".attempt-label").show();

                if ($(event.target).closest(".description, .total-attempts").find(".attempt-label").show()) {
                    $(event.target).closest(".description, .total-attempts").find(".attempt-label").hide();
                    $(event.target).closest(".description, .total-attempts").find(".attempt-textbox").show();
                    $(event.target).closest(".total-attempts").find(".attempt-textbox").find('.questions-attempts').focus().select();
                }

                return false;
            },
            SaveAttempts: function (event) {
                var attempts = $(event.target).siblings('.questions-attempts').val();
                if (attempts == undefined) {
                    attempts = $(event.target).val();
                }

                var questionid = $(event.target).parents('li.question').find('input.question-id').val();
                var entityId = $(event.target).parents('li.question').find('input.question-entity-id').val();

                if (!attempts.match('^(0|[1-9][0-9]*)$')) {
                    PxPage.Toasts.Error("Please enter a numeric value");
                    $(event.target).closest(".total-attempts").find(".attempt-textbox").find('.questions-attempts').focus().select();
                    return;
                }
                $('#' + questionid).block({ message: 'Loading...' });

                $.post(
                    PxPage.Routes.save_attempts, {
                        quizId: $(event.target).closest('.quiz').attr('id'),
                        questionId: questionid,
                        attempts: attempts,
                        entityId: entityId
                    },
                    function (response) {
                        PxPage.log('saved attempts: ' + response);
                        if (Number(attempts) > 0) {
                            $(event.target).closest(".description, .total-attempts").find(".attempt-label").text(attempts + (Number(attempts) > 1 ? " attempts" : " attempt"));
                        }
                        else {
                            $(event.target).closest(".description, .total-attempts").find(".attempt-label").text("Unlimited attempts");
                        }

                        $(event.target).closest(".description, .total-attempts").find(".attempt-label").show();
                        $(event.target).closest(".description, .total-attempts").find(".attempt-textbox").hide();
                        $('.question-list').questionlist('numberQuestions', event.target);
                        if ($('#fne-window').is(':visible')) {
                            $('#fne-window').removeClass('require-confirm');
                        }
                        $('#' + questionid).unblock();
                    }
                );
            },
            
        }//end functions
    };
    
    var api = {
        init: function(options) {
            $(document).off('click', _static.sel.changeAttemptsLnk).on('click', _static.sel.changeAttemptsLnk, _static.fn.AttemptClick);
            $(document).off('focusout', _static.sel.saveAttempts).on('focusout', _static.sel.saveAttempts, _static.fn.SaveAttempts);
            
        },
    };
    // register the plugin with jQuery and provide access to
    // the api.
    $.fn.quizhomework = function (method) {
        if (api[method]) {
            return api[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return api.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.' + _static.pluginName);
        }
    };

} (jQuery));

///DEPRECATED HOMEWORK FUNCTIONS:

//            HintsClick: function (event) {
//                if ($(event.target).closest(".description").find(".hint-label").text() == 'Hints Off') {
//                    $(event.target).closest(".description").find(".hint-label").text('Hints On');
//                }
//                else {
//                    $(event.target).closest(".description").find(".hint-label").text('Hints Off');
//                }

//                var text = $(event.target).closest(".description").find(".hint-label").text();
//                var hints = text.substring(6, text.length);
//                var questionid = $(event.target).parents('li.question').find('input.question-id').val();
//                var questiontype = $(event.target).parents('li.question').find('input.question-type').val();
//                var quizId = $('#content-item-id').text();

//                $.post(
//                    PxPage.Routes.save_hints, {
//                        questionId: questionid,
//                        quizId: quizId,
//                        hints: hints
//                    },
//                    function (response) {
//                        PxPage.log('saved hints: ' + response);
//                    }
//                );
//                return false;
//            },

//            ScrambledClick: function (event) {

//                if ($(event.target).closest(".description").find(".scrambled-label").text() == 'Answers Scrambled') {
//                    $(event.target).closest(".description").find(".scrambled-label").text('Not Scrambled');
//                }
//                else {
//                    $(event.target).closest(".description").find(".scrambled-label").text('Answers Scrambled');
//                }

//                var text = $(event.target).closest(".description").find(".scrambled-label").text();
//                var scrambled = text.substring(0, text.indexOf('Scrambled') - 1);
//                var questionid = $(event.target).parents('li.question').find('input.question-id').val();
//                var questiontype = $(event.target).parents('li.question').find('input.question-type').val();
//                var quizId = $('#content-item-id').text();

//                $.post(
//                    PxPage.Routes.save_scrambled, {
//                        questionId: questionid,
//                        quizId: quizId,
//                        scrambled: scrambled
//                    },
//                    function (response) {
//                        PxPage.log('saved score: ' + response);
//                    }
//                );

//                return false;
//            },
//              TimeLimitClick: function (event) {
//                if ($(event.target).closest(".description").find(".timelimit-label").show()) {
//                    $(event.target).closest(".description").find(".timelimit-label").hide();
//                    $(event.target).closest(".description").find(".timelimit-dropdown").show();
//                }

//                $(event.target).closest(".description").find("#ddlTimelimit").find("option").remove();
//                for (var i = 0; i < 6; i++) {

//                    if (i == 0) {
//                        $(event.target).closest(".description").find("#ddlTimelimit").append($('<option></option>').val(i).html("Unlimited"));
//                    }
//                    else {
//                        $(event.target).closest(".description").find("#ddlTimelimit").append($('<option></option>').val(i).html(i + ":00"));
//                    }
//                }
//                var text = $(event.target).closest(".description").find(".timelimit-label").text();
//                if (text != "Unlimited") {
//                    $(event.target).closest(".description").find("#ddlTimelimit").val(text.substring(0, 1));
//                }

//                return false;
//            },

//        ReviewClick: function (event) {
//            if ($(event.target).closest(".description").find(".review-label").show()) {
//                $(event.target).closest(".description").find(".review-label").hide();
//                $(event.target).closest(".description").find(".review-dropdown").show();

//                var groupdropdown = PxPage.Routes.group_dropdown;
//                var reviewdropdown = groupdropdown.split("|");
//                var arrReview = "";

//                $.each(reviewdropdown, function (index) {
//                    var arrValues = reviewdropdown[index].split(":");
//                    for (var j = 0; j < arrValues.length; j++) {
//                        if (arrValues[j] == "ReviewDropDown") {
//                            arrReview = arrValues[1].split(",");
//                            return false;
//                        }
//                    }
//                });

//                $(event.target).closest(".description").find("#ddlReview").find("option").remove();
//                for (var i = 0; i < arrReview.length; i++) {

//                    var ddValue = arrReview[i].split("-");
//                    $(event.target).closest(".description").find("#ddlReview").append($('<option></option>').val(ddValue[1]).html("Review " + ddValue[0]));
//                }

//                var text = $(event.target).closest(".description").find(".review-label").text();
//                $(event.target).closest(".description").find("#ddlReview").val(text.substring(7, text.length));

//            }
//            return false;
//        },


//        ScoreClick: function (event) {
//            if ($(event.target).closest(".description").find(".score-label").show()) {
//                $(event.target).closest(".description").find(".score-label").hide();
//                $(event.target).closest(".description").find(".score-dropdown").show();

//                var groupdropdown = PxPage.Routes.group_dropdown;
//                var scoredropdown = groupdropdown.split("|");
//                var arrScore = "";

//                $.each(scoredropdown, function (index) {
//                    var arrValues = scoredropdown[index].split(":");
//                    for (var j = 0; j < arrValues.length; j++) {
//                        if (arrValues[j] == "ScoreDropDown") {
//                            arrScore = arrValues[1].split(",");
//                            return false;
//                        }
//                    }
//                });

//                $(event.target).closest(".description").find("#ddlScore").find("option").remove();
//                for (var i = 0; i < arrScore.length; i++) {

//                    $(event.target).closest(".description").find("#ddlScore").append($('<option></option>').val(arrScore[i]).html("Score " + arrScore[i]));
//                }

//                var text = $(event.target).closest(".description").find(".score-label").text();
//                $(event.target).closest(".description").find("#ddlScore").val(text.substring(6, text.length));
//            }
//            return false;
//        },
//  OnChangeReview: function (event) {
//            var review = $(event.target).val();
//            var questionid = $(event.target).parents('li.question').find('input.question-id').val();
//            var questiontype = $(event.target).parents('li.question').find('input.question-type').val();
//            var quizId = $('#content-item-id').text();

//            $.post(
//                PxPage.Routes.save_review, {
//                    questionId: questionid,
//                    quizId: quizId,
//                    review: review
//                },
//                function (response) {
//                    PxPage.log('saved review: ' + response);
//                }
//            );

//            var text = $(event.target).closest(".description").find("#ddlReview option:selected").text();
//            $(event.target).closest(".description").find(".review-label").text(text);
//            $(event.target).closest(".description").find(".review-label").show();
//            $(event.target).closest(".description").find(".review-dropdown").hide();

//            return false;
//        },
//OnChangeScore: function (event) {
//            var score = $(event.target).val();
//            var questionid = $(event.target).parents('li.question').find('input.question-id').val();
//            var questiontype = $(event.target).parents('li.question').find('input.question-type').val();
//            var quizId = $('#content-item-id').text();

//            $.post(
//                PxPage.Routes.save_score, {
//                    questionId: questionid,
//                    quizId: quizId,
//                    score: score
//                },
//                function (response) {
//                    PxPage.log('saved score: ' + response);
//                }
//            );

//            var text = $(event.target).closest(".description").find("#ddlScore option:selected").text();
//            $(event.target).closest(".description").find(".score-label").text(text);
//            $(event.target).closest(".description").find(".score-label").show();
//            $(event.target).closest(".description").find(".score-dropdown").hide();

//            return false;
//        },
//OnChangeTimelimit: function (event) {

//            var timelimit = $(event.target).val();
//            var questionid = $(event.target).parents('li.question').find('input.question-id').val();
//            var questiontype = $(event.target).parents('li.question').find('input.question-type').val();
//            var quizId = $('#content-item-id').text();

//            $.post(
//                PxPage.Routes.save_timelimits, {
//                    questionId: questionid,
//                    quizId: quizId,
//                    timelimits: timelimit
//                },
//                function (response) {
//                    PxPage.log('saved timelimits: ' + response);
//                }
//            );

//            var text = $(event.target).closest(".description").find("#ddlTimelimit option:selected").text();
//            $(event.target).closest(".description").find(".timelimit-label").text(text + " Minutes");
//            $(event.target).closest(".description").find(".timelimit-label").show();
//            $(event.target).closest(".description").find(".timelimit-dropdown").hide();

//            return false;
//        },