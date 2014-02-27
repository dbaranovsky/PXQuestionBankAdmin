var setFixtureBasedOnMode = function (html, mode, target) {
    switch (mode) {
        case 'set':
            setFixtures(html);
            break;
        case 'append':
            appendSetFixtures(html);
            break;
        case 'target':
            $(target).html(html);
            break;
        default:
            setFixtures(html);
            break;
    }
};

var TestQuizHelper = function ($) {
    return {
        Cached: {},
        setAllCheckTarget: '',
        Model: {
            generatePresetQuestion_NonEmptyQuestionPool: function () {
                var questions = [];

                questions.push({ Id: 'QuestionPool0', Type: 'BANK', Text: 'Question POOL 0', Questions: [{ Id: 'QuestionPool0Question1', Type: 'MC', Text: 'Question 1 in Question Pool 0' }] });
                questions.push({ Id: 'QuestionMC1', Type: 'MC', Text: 'Question MC 1' });

                return questions;

            },
            generatePresetQuestion_WithSearchableMetaData: function () {
                var questions = [];

                questions.push({ Id: 'QuestionMC1', Type: 'MC', Text: 'Question MC 1', SearchableMetaData: { "title": "123123", "exercisenumber": "2", "usedin": "LOR_econportal__stoneecon2__master_QUIZ_B02E4372ECAB4AB029F8760E21020008", "questionstatus": "1", "suggesteduse": "", "learningobjectives": "" } });
                questions.push({ Id: 'QuestionMC2', Type: 'ESSAY', Text: 'Question MC 1', SearchableMetaData: { "difficulty": "tobereplaced", "cognitivelevel": "Higher-order cognition", "bloomdomain": "Analysis", "coreconcept": "CC4.1 Proteins are linear polymers.", "relatedcontent": "Chapter1 Progressions", "guidance": "This question reinforc.", "freeresponsequestion": "This meta data has been ad.", "suggesteduse": "", "title": "6. (EOC) I", "exercisenumber": "6", "usedin": "LOR_econportal__stoneecon2__master_QUIZ_B02E4372ECAB4AB029F8760E21020008", "questionstatus": "1", "learningobjectives": "" } });
                return questions;
            },
            generateRandomQuestionArray: function (numberOfQuestions, allowType) {
                var questions = [];
                allowType = !allowType || !allowType.length ? ["MC"] : allowType;

                for (var i = 0; i < numberOfQuestions; i++) {
                    var type = allowType[i % allowType.length];
                    questions.push({ Id: i, Type: type, Text: 'Question ' + type + ' ' + i });
                }

                return questions;
            },
            generateQuizModel: function (questions) {
                if (!questions)
                    questions = TestQuizHelper.Model.generateRandomQuestionArray(2, ["Bank", "MC"]);

                var model = {
                    path: "EditQuiz",
                    AllowResubmission: false,
                    AllowSaveAndContinue: true,
                    AttemptLimit: 3,
                    Display: 'Instructor',
                    IsLC: false,
                    Questions: questions,
                    CourseInfo: { QuestionFilter: {} }
                    ,
                    type: "Bfw.PX.PXPub.Models.Quiz"
                };
                return model;

            },
            generateAdvancedSearchResultsModel: function (start, result) {
                start = !start ? 0 : start;
                result = !result ? 10 : result;
                var model = {
                    Results: {
                        numFound: result
                    },
                    Query: {
                        Start: start
                    }
                };
                return model;
            },
            generateQuizSearchResultsModel: function (quiz, result) {
                quiz = !quiz ? TestQuizHelper.Model.generateQuizModel() : quiz;
                result = !result ? TestQuizHelper.Model.generateAdvancedSearchResultsModel() : result;
                return {
                    Results: result,
                    Quiz: quiz,
                    type: 'Bfw.PX.PXPub.Models.QuizSearchResults',
                    path: 'SearchQuiz'
                };
            }
        },
        generateViewModel: function (model) {
            var viewModel = {
                viewPath: model.path,
                viewModel: JSON.stringify(model),
                viewModelType: model.type
            };
            return viewModel;
        },
        setFixtureFromView: function (model, mode, target, callback) {
            mode = !mode ? 'set' : mode;
            folder = 'QuizPartials';
            var html = PxViewRender.RenderView('PXPub', folder, model);
            setFixtureBasedOnMode(html, mode, target);
            if (callback)
                callback(html);

        },
        setFixtureFromCache: function (model, mode, target) {
            if (!model.viewPath || !model.viewModelType)
                TestQuizHelper.setFixtureFromView(model, mode, target);
            var key = model.viewPath + '|' + model.viewModelType;
            if (TestQuizHelper.Cached[key]) {
                var view = TestQuizHelper.Cached[key];
                setFixtureBasedOnMode(view, mode, target);
            } else {
                TestQuizHelper.setFixtureFromView(model, mode, target, function (html) {
                    TestQuizHelper.Cached[key] = html;
                });
            }

        },
        defaultSpyOn: function () {
            TestQuizHelper.spyOnPxPage(true);
            TestQuizHelper.spyOnPxQuiz(true);
        },
        spyOnPxPage: function (callFake) {
            PxPage = jasmine.createSpyObj('PxPage', ['Loading', 'Loaded', 'Routes', 'log', 'switchboard', 'FneInitHooks', 'FneResizeHooks']);
            if (callFake) {
                PxPage.Toasts = jasmine.createSpyObj('PxPage.Toasts', ['Error', 'Success']);
                PxPage.Toasts.Error.andCallFake(function (message) {
                    $('#pxpage-toasts-error').html(message);
                });
                PxPage.Toasts.Success.andCallFake(function (message) {
                    $('#pxpage-toasts-success').html(message);
                });
            }
        },
        spyOnPxQuiz: function (callFake) {

            PxQuiz = jasmine.createSpyObj('PxQuiz',
                ['isProcessing', 'UpdateAvailableQuestionsMenu', 'UpdateAddedQuestions', 'UpdateQuestionList', 'cfind', 'SetAllChecks', 'CloseDialog', 'LoadDefaultBreadcrumbTrail', 'FindDefaultRoot',
                    'UpdateChildList', 'PostChildLoadedProcessing']);
            if (callFake) {
                PxQuiz.cfind.andCallFake(function (target, selector) {
                    if (selector == 'test')
                        return $('<div id="test"></div>');
                    var lookingAt = $(target);
                    if (lookingAt != null && lookingAt.length) {
                        var found = $(lookingAt).find(selector);
                        if (found.length) {
                            return found;
                        } else {
                            lookingAt = lookingAt.parents(".question-pool, .selected-questions, .available-questions, .quiz-overview");
                            return lookingAt.find(selector);
                        }
                    }
                    return lookingAt;
                });

                PxQuiz.SetAllChecks.andCallFake(function (target, value) {
                    target = $(target);
                    var classes = '.selected-questions, .available-questions, .quiz-overview, .question-pool';
                    if (!target.is(classes))
                        target = $(TestQuizHelper.setAllCheckTarget);
                    if (target.length > 0) {
                        target.find("ul.questions:visible li.question [name=selected], li.quiz-item:visible [name=selected]").prop('checked', value);
                        target.find("ul.questions:visible li.question").removeClass("selected-question-in-quiz-editor");
                        target.find("ul.questions:visible li.question .preview-available-question").removeClass("displayquestionmenu");
                        target.find("ul.questions:visible li.question .add-to-pool-available-question").removeClass("displayquestionmenu");
                        $('.available-questions-header .questions-actions .add-quiestion-btn-wrapper').hide();
                        $('.available-questions-header .questions-actions .add-to-pool-available-quiestion-btn-wrapper').hide();

                        target.find("ul.questions li.question").removeClass('active');
                        if (value) {
                            target.find("ul.questions li.question").addClass('active');
                        }
                    }

                });
            }
        },
        spyOnActionWidget: function () {
            $.fn.ActionWidget = jasmine.createSpy('ActionWidget').andCallFake(function (settings) {
                this.each(function () {
                    var container = $(this);
                    container.html('<div class="gearbox" style="display: none;"></div><ul id="' + settings.menu.id + '" class="contextMenu"></ul>');
                    $(settings.menu.options).each(function () {
                        container.find('ul').append($('<li class="' + this.name + '"><a  action_href="#' + this.name + '"></a></li>'));

                    });
                    container.find('li').unbind('click');

                    container.find('li').click(function (event) {
                        var actionName = $(this).find("a").attr("action_href");
                        actionName = actionName.substring(actionName.lastIndexOf("#") + 1);
                        settings.action.apply(this, [event, actionName, container]);
                    });
                });
            });
        },
        spyOnPxBreadcrumb: function () {
            PxBreadcrumb = jasmine.createSpyObj('PxBreadcrumb', ['RunResizers']);

        },
        spyOnContentWidget: function () {
            ContentWidget = jasmine.createSpyObj('ContentWidget', ['BreadcrumbChanged']);

        },
        spyOnEasyXdm: function () {
            window.easyXDM = jasmine.createSpyObj('easyXDM', ['Rpc']);

        },
        clearCache: function () {
            TestQuizHelper.Cached = {};
        }
    };
}(jQuery);
