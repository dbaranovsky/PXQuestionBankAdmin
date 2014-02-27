
var HelperManagementCard = function($) {
    var _static = {
        defaults: {
            id: "",
            dueDate: new Date(),
            showUnits: false,
        },
        fn: {
            setOptions: function(options) {
                $.extend(_static.defaults, options);
            },
            setFixture: function(options) {
                if (options)
                    _static.fn.setOptions(options);

                var fixture = _static.fn.generateManagementCardView();
                jasmine.Fixtures.prototype.addToContainer_(fixture);
            },
            generateManagementCardView: function() {
                var viewData = {
                    ShowAssignmentUnitWorkflow: {
                        dataType: "System.Boolean",
                        dataValue: _static.defaults.showUnits
                    },
                    isRange: {
                        dataType: "System.Boolean",
                        dataValue: false
                    },
                    HiddenFromStudents: {
                        dataType: "System.Boolean",
                        dataValue: false
                    }
                };

                var data = {
                    viewPath: "ManagementCard",
                    viewModel: JSON.stringify({
                        Id: _static.defaults.id,
                        DueDate: _static.defaults.dueDate,
                        SourceType: "",
                        AssignTabSettings: {
                            ShowMakeGradeable: false,
                            ShowGradebookCategory: true
                        },
                        AvailableSubmissionGradeAction: {                        
                        
                        },
                        Score: {
                            Possible: 0
                        },
                        GradeBookWeights: {
                            GradeWeightCategories: null
                        }
                    }),
                    viewModelType: "Bfw.PX.PXPub.Models.AssignedItem",
                    viewData: JSON.stringify(viewData)
                };

                var view = PxViewRender.RenderView('PXPub', 'LaunchPadTreeWidget', data);
                view += "<div data-ft-id='1' class='faux-tree-node'><div class='faceplate-student-completion-stats'></div></div>";

                return view;
            }
        }
    };
    
    return {
        SetManagementCardViewFixture: _static.fn.setFixture
    };
}(jQuery);