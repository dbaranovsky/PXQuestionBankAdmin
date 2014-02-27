
var PxQuestionAdmin = function ($) {
	//set up defaults
    var DataSaved = false;
    var HTS_RPC = null;
	var defaults = {
		SearchBoxID: '#txtSearchKeyword',
		SearchText: 'enter search keywords',
		ClearSearchLink: '#lnkClearSearch',
		SearchForm: '#frmSearch',

		ChapterList_new: '#ChapterList_new',
		divQuizSelectList_new: '#divQuizSelectList_new',
		QuizList_new: "#QuizList",

		ChapterList: '#ChapterList_new',
		divQuizSelectList: '#divQuizSelectList',
		QuizList: "#QuizList",

		FormatList: "#FormatList",
		StatusList: "#StatusList",
		cmdSearch: "#cmdSearch",
		QuestionModifiedAction: "",
		QBAContainer: "#QBA",
		SearchResultId: "SearchResult",
		QuestionListTable: "#QBA-QuestionList-Table",
		EditorLink: ".EditorLink",
		alert_header: "Please Note",
		unSavedMsg: "There is unsaved data are you sure you want to Done Editing",
		DataSavedMsg: "Changes have been saved"

	},
	QuestionEditorSelector = {
		TabSelector: '.RouteTab',
		TabContainer: '#TabContainer',
		TabContainerBody: '#QBA-Editor-Container',
		SelectedTabCss: "selected",
		DoneButton: "#QBA-DoneEditing",
		MainTab: ".maintab"
	},
	NoteTabSelector = {
		Form: "#frmAddNote",
		AddNoteContainer: "#QBA-AddNote-Add",
		AddNoteFormContainer: "#QBA-AddNote-Form",
		AddNoteButton: "#QBA-AddNote-Add-Button",
		CancelButton: "#QBA-AddNote-Cancel-Button"
	},
	PreviewTab = {
		Container: "#QBA-Flag-Container",
		FlagButton: "#QBA-Flag-Button",
		FlagForm: "#frmAddFlag",
		AddConfirmation: "#QBA-AddFlag-Confirmation",
		CancelButton: "#QBA-AddFlag-Cancel-Button",
		FormContainer: "#QBA-AddFlag-Form"
	},
 MetadataTab = {
	 MetadataButton: "#QBA-Metadata-Button",
	 MetadataForm: "#QBA-Metadata-Fieldset",
	 AddConfirmation: "#QBA-Metadata-Confirmation",
	 QuestionTitle: "#QBA-Metadata-Title",
	 QuestionId: "#QBA-Metadata-Id",
	 QuestionEntityId: "#QBA-Metadata-EntityId"
 },

	FNESelector = {
		fnetitle: "#fne-title",
		fneminimized: "#fne-minimized",
		fnesearchminimized: "#fne-search-minimized",
		fnewindow: "#fne-window"
	},
	QuestionEditorTab = {
		SaveButton: "#QBA-Save-Question",
		UndoButton: "#QBA-Undo-Changes",
		Editor: "questioneditor",
		EditorComponent: "quizeditorcomponent"
	};
	return {
		Init: function () {

			//bind events:
		    $(document).on('focus', defaults.SearchBoxID, function () {
				if ($(defaults.SearchBoxID).val() == defaults.SearchText && $(defaults.SearchBoxID).attr("readonly") == "readonly") {

					PxPage.Toasts.Error("Keyword Search is unavailable at this time. Please use the eBook Chapter and Question Bank options to begin a question search.");
					//$(defaults.SearchBoxID).val("");
				}
			});

		    $(document).on('focusout', defaults.SearchBoxID, function () {
				if ($(defaults.SearchBoxID).val() == "") {
					$(defaults.SearchBoxID).val(defaults.SearchText);
				}
			});
			/*
			$(defaults.ChapterList).live('click', function () {
			$.ajax(
			{
			type: "POST",
			url: select_chapter_route,
			data: { data: $(this).serialize() },
			success:
			function (result) {
			if (result != null) {
			$(defaults.divQuizSelectList).html(result);
			}
			}
			});

			PxQuestionAdmin.ResetIndexPage();
			});
			*/

			$(defaults.ChapterList_new).bind("change", function (e) {
				PxPage.Loading($('#SearchPanel'));
				if (e.removed && e.val.length < 1) {
					//alert('all gone');
					$.ajax(
					{
						type: "POST",
						url: select_chapter_route,
						success:
							function (result) {
								if (result != null) {
									$(defaults.divQuizSelectList_new).html(result);
									$(defaults.QuizList).select2({ placeholder: "Search / Browse Question Bank", allowClear: true });
									PxPage.Loaded($('#SearchPanel'));
								}
							}
					});
				}
				else
					return false;
			});



			$(document).on('click', defaults.ChapterList_new, function () {

				var d = $(this).val(); //JSON.stringify(d)
				if (d != null) {

					var dataStr = "";
					for (var i = 0; i < d.length; i++) {
						if (i == 0)
							dataStr += "ChapterSelectedValues=" + d[i];
						else
							dataStr += "&ChapterSelectedValues=" + d[i];
					}
					PxPage.Loading('SearchPanel');
					$.ajax(
					{
						type: "POST",
						url: select_chapter_route,
						data: { data: dataStr },
						success: function (result) {
							if (result != null) {
								var QuestionBankReturnedIDs = $(result).find('option').each(function () { return this.value; })
								var previousQBA_Ids = $(defaults.QuizList).select2("val");

								$(defaults.divQuizSelectList_new).html(result);
								$(defaults.QuizList).select2({ placeholder: "All Quizzes", allowClear: true });

								if (previousQBA_Ids.length > 0) {
									var allIDs = [];
									for (var i = 0; i < QuestionBankReturnedIDs.length; i++) {
										for (var k = 0; k < previousQBA_Ids.length; k++) {
											if (QuestionBankReturnedIDs[i].value == previousQBA_Ids[k]) {
												allIDs.push(previousQBA_Ids[k]);
											}
										}
									}
									//$(defaults.QuizList).select2("val", previousQBA_Ids[k]);
									if (allIDs.length > 0)
										$(defaults.QuizList).val(allIDs).trigger('change');
								}
							}
							PxPage.Loaded($('#SearchPanel'));
						}
					});

					PxQuestionAdmin.ResetIndexPage();
				}
				PxPage.Loaded('SearchPanel');
			});

			$(document).on('click', defaults.QuizList, function () {
				PxQuestionAdmin.ResetIndexPage();
			});

			$(document).on('mouseenter', "#QBA-QuestionList-Table tr", function () {
			    PxQuestionAdmin.ShowUrlDuplicateButton(this);
			});
			$(document).on('mouseleave', "#QBA-QuestionList-Table tr", function () {
			    PxQuestionAdmin.HideDuplicateButton(this)
			});

			$(document).on('mouseenter', ".duplicate-button-container", function () {
			    PPxQuestionAdmin.ShowGearMenu(this);
			});
			$(document).on('mouseleave', ".duplicate-button-container", function () {
			    PxQuestionAdmin.HideGearMenu(this);
			});

			$(document).on('click', defaults.FormatList, function () {
				PxQuestionAdmin.ResetIndexPage();
			});

			$(document).on('click', "#duplicate-action", function () {
				var currentTr = $(this).closest("tr");
				PxQuestionAdmin.DuplicateQuestion(currentTr);
			});

			$(document).on('click', defaults.StatusList, function () {
				PxQuestionAdmin.ResetIndexPage();
			});

			$(document).on('click', defaults.ClearSearchLink, function () {
				PxQuestionAdmin.ClearSearchForm();
			});


			$(document).on('click', ".edit-button", function () {

				PxQuestionAdmin.TextEdit($(this));
			});

			$(document).on('click', ".free-text-view-all-button", function () {
				PxQuestionAdmin.ViewAllQBA($(this));
			});

			$(FNESelector.fnetitle).hide();
			$(FNESelector.fneminimized).remove();
			$(FNESelector.fnesearchminimized).remove();

			$(defaults.StatusList).val("0");
			$(defaults.FormatList).val("0");
			$(defaults.ChapterList).val("0");
			$(defaults.QuizList).val("0");
			DataSaved = false;

			$(PxPage.switchboard).unbind("clickcontent", PxQuestionAdmin.SetHtsRpcHooks);
			$(PxPage.switchboard).bind("htsEditorLoaded", PxQuestionAdmin.SetHtsRpcHooks);


		},


		//////////// List all functions below /////////////////////////////


		ResetIndexPage: function () {
			//PxQuestionAdmin.ResetSearchResultPanel();
			PxQuestionAdmin.ResetPagination();
		},


		ResetSearchResultPanel: function () {

			if ($("#TotalCount").val != "") {
				$.ajax(
					{
						type: "GET",
						url: reset_search_result_route,
						success:
							function (result) {
								if (result != null) {
									$("#tdSearchResultPanel").html(result);
								}
							}
					});
			}
		},

		ResetPagination: function () {
			$("#CurrentPage").val = 1;
			$("#TotalItems").val = 0;
			$("#paging-info.NextPageStartSearchQuestion").val = 1;
			$("#paging-info.NextPageStartSearchQuiz").val = 1;
		},
		DuplicateQuestion: function (currentTr) {

			var questionId = currentTr.attr("data-qba-question-id"), entityid = currentTr.attr("data-qba-entityid"), quizid = currentTr.attr("data-qba-quiz-id");
			var clonedQuestion = currentTr.clone(true).removeClass().addClass("InProgress");

			PxPage.Loading($('#SearchResult'));
			$.ajax(
					{
						type: "POST",
						url: PxPage.Routes.copy_question,
						data: { entityid: entityid, questionId: questionId, quizid: quizid },
						success: function (response) {

							clonedQuestion.find(".EditorLink span b").text(response.questionToCopy.Title)
							clonedQuestion.find(".Qstatus").text("In Progress");
							clonedQuestion.find(".duplicate-button-container")
							currentTr.after(clonedQuestion.attr("data-qba-question-id", response.questionToCopy.Id));
							PxPage.Loaded($('#SearchResult'));
							PxPage.doBGFade($(clonedQuestion), [245, 255, 159], [255, 255, 255], '', 75, 20, 4);

							var total = parseInt($("#TotalCount").val()) + 1;
							var currentPg = parseInt($("#SearchResultCurrentPage").val());
							var start = (currentPg * 25) - 24;
							var end =  currentPg * 25;
    						var newPagingText = "Displaying items " + start + " - " + end + "  of  " + total;
							if ((total) <= 25) 
							    newPagingText = "Displaying items " + start + " - " + total + "  of  " + total;
							$("#QBA-Result-Table #paging-info span").text(newPagingText);
						}

					});


		},
		ClearSearchForm: function () {
			PxPage.Loading($('#SearchPanel'));
			//PxPage.Loading('.question-admin');
			$(defaults.SearchForm).clearForm();
			$(defaults.StatusList).val("0");
			$(defaults.FormatList).val("0");
			$(defaults.ChapterList_new).select2("val", "");
			$(defaults.QuizList).select2("val", "");
			$(defaults.SearchBoxID).val(defaults.SearchText);
			PxQuestionAdmin.ResetSearchResultPanel();
			PxQuestionAdmin.ResetPagination();
			$("#QBA-Search-Table #CurrentPage").val(1);
			$(":checkbox").prop("checked", false);
			$('#cmdSearch').removeAttr('disabled').val('Search');
			$.ajax(
					{
						type: "POST",
						url: select_chapter_route,
						success:
							function (result) {
								if (result != null) {
									$(defaults.divQuizSelectList_new).html(result);
									//$(defaults.QuizList).val("0");
									$(defaults.QuizList).select2({ placeholder: "Search / Browse Question Bank", allowClear: true });
								}
							}
					});

			//PxPage.Loaded('.question-admin'); 
			PxPage.Loaded($('#SearchPanel'));
		},

		ValidateInput: function () {
			var totalItems = $('#SearchResultTotalItems').val();
			$('#SearchPanelTotalItems').attr("value", totalItems);
			var QuestionFormat = $("input:checked").length;

			if ($(defaults.ChapterList_new).val() == null && $(defaults.QuizList).val() == null) {
				PxPage.Toasts.Error("Please select question search criteria in the left column and then click Search. At this time search has to include an eBook Chapter, and/or Question Bank.");
				return false;
			}


			if ($(defaults.StatusList).val() == "0" && QuestionFormat == 0 && $(defaults.ChapterList_new).val() == null && $(defaults.QuizList).val() == null && $(defaults.SearchBoxID).val() == defaults.SearchText) {
				PxPage.Toasts.Error("Please select question search criteria in the left column and then click Search.");
				return false;
			}
			else {
				PxQuestionAdmin.BlockUI();
				return true;
			}
		},
		callbackSelectChapter: function (result) {
			if (result != null) {
				$('#divQuizSelectList').update(result);
			}
		},
		submitPagination: function (pageNumber) {
			$("#SearchPanelNextPageStartSearchQuestion").attr("value", $("#SearchResultNextPageStartSearchQuestion").val());
			$("#SearchPanelNextPageStartSearchQuiz").attr("value", $("#SearchResultNextPageStartSearchQuiz").val());

			$("#CurrentPage").attr("value", pageNumber);
			$("#SearchPanelCurrentPage").attr("value", pageNumber);

			var totalItems = $('#SearchResultTotalItems').val();

			$('#SearchPanelTotalItems').attr("value", totalItems);

			$(defaults.cmdSearch).click();

		},

		// Init Event for QuestionEditor page
		InitQuestionEditorPage: function () {
			$(QuestionEditorSelector.TabSelector).die().live("click", function () {
				$(QuestionEditorSelector.TabSelector).removeClass(QuestionEditorSelector.SelectedTabCss);
				PxQuestionAdmin.OnTabClick(this);
			});
			$(QuestionEditorSelector.MainTab).click();
		},
		// Event to load new Tab content
		OnTabClick: function (selector) {
			var routeURL = $(selector).attr("route");
			if (routeURL !== null) {
			    
				if (routeURL.indexOf("PreviewTab") > 0) {
				    var questionId = $('#SelectedQuestionId').val();
				    var questionType = $('#SelectedQuestionType').val();
				    var questionUrl = $('#SelectedQuestionUrl').val();

				    if (questionType == 'custom') {
				        PxQuestionAdmin.PreviewCustomQuestion(questionId, questionUrl);
				    }
				    else {
				        PxQuestionAdmin.PreviewQBAQuestion(selector, routeURL);
				    }

				}
				else {
					var arr = ["MetaDataTab", "QuestionEditorTab"];
					for (var i = 0; i < arr.length; ++i) {
						if (routeURL.indexOf(arr[i]) != -1) {
							DataSaved = true;
							break;
						}
						else
							DataSaved = false;
					}
					PxQuestionAdmin.GetTabContent(selector, routeURL);
				}
			}
		},

		CalculateNewValue: function (original) {
			return original * (94 / 100); // 90% 
		},
		DisplayCustomPreview: function (response) {

		    var showQuiz = function () {
		        $.ajax({
		            url: QBA_show_quiz_route, // "/launchpad/bps6e/71836/QuestionAdmin/ShowQuiz",
		            headers: {"QBA": "true"},
		            cache: false,
		            success: function (xml) {
		                PxQuestionAdmin.DisplayPreview(xml);
		            },
		            data: { Id: response, QuestionId: $('#QuestionEditorTab').attr('data-qba-questionid')},
		            type: "POST"
		        });
		    };

		    showQuiz();
		    return false;
		},
		DisplayPreview: function (response) {
		    var _width = PxQuestionAdmin.CalculateNewValue($(window).width()); //$(window).width() - 70;
		    var _height = PxQuestionAdmin.CalculateNewValue($(window).height()); //$(window).height() - 70;
		    var previewDiv = $('<div></div>');
		    previewDiv.addClass('preview-question-dialog');
		    previewDiv.dialog({
		        width: _width,
		        height: _height,
		        minWidth: 700,
		        minHeight: 300,
		        modal: true,
		        draggable: false,
		        resizeable: true,
		        closeOnEscape: true,
		        //title: 'Question Preview',
		        close: function () {
		            $(this).dialog('destroy').empty().remove();

		        }
		    }); //dialog end
		    previewDiv.html(response);
		    $(".preview-question-dialog").closest(".ui-dialog").attr('id', "PreviewDialog");

		    $('#PreviewTab .DonePreview').bind("click", function () {
		        $(".preview-question-dialog").dialog('destroy').remove();
		    });
		},

		PreviewCustomQuestion: function (questionId, questionUrl) {
		    var questionPreviewEntityId = $('#PreviewEntityId').val();
		    if (questionUrl == 'HTS') {
		        PxQuiz.HTS_RPC.previewQuestion(questionPreviewEntityId, PxQuestionAdmin.DisplayCustomPreview);
		    }
		    if (questionUrl == 'FMA_GRAPH') {
		        /*var disciplineCourseId = $('#QuestionEditorTab').attr('data-qba-entityid');
		        PxQuiz.PreviewCustomQuestion(questionId, questionUrl, 'qba', disciplineCourseId);*/
		        PxQuizHts.StoreAndPreviewGraphQuestion(questionUrl);
		    }
		},

		PreviewQBAQuestion: function (selector, routeURL) {
			var bodyPostContent = $.ajax({
				type: "POST",
				url: routeURL, /* URL */
				data: {},
				beforeSend: function (jqXHR, settings) {
					//PxPage.Loading($(QuestionEditorSelector.TabContainer).attr("id")); /* Blocks the UI on Ajax call */
				},
				success: function (data) {
					if (data.Status != undefined && data.Status == "error")
						$(QuestionEditorSelector.TabContainerBody).html('Unexpected error occurred while loading this tab.');
					else {

					    PxQuestionAdmin.DisplayPreview(data);
					}

				},
				complete: function (jqXHR, textStatus) {
					PxPage.Loaded();
				}
			});
		},

		SetHtsRpcHooks: function () {
		    if ($('#custom-hts-editor').length) {
		        var remoteUrl = $('#custom-hts-editor').attr('rel');

		        var consumer = new easyXDM.Rpc({
		            remote: remoteUrl,
		            container: 'custom-hts-editor'
		        },
                {
                    local: {
                        //    questionSaved: function (questionId, success, error) {
                        //        PxPage.Loading('fne-window');
                        //        $.post(
                        //    PxPage.Routes.remove_question_from_cache, {
                        //        questionId: questionId
                        //    },
                        //    function (response) {
                        //        PxQuiz.UpdateQuizEditorQuestions(null, function () { });
                        //        PxPage.log('remove_question_from_cache : ' + response);
                        //        if (success) {
                        //            PxPage.Loaded('fne-window');
                        //            success();
                        //        }
                        //    }
                        //); PxPage.Toasts.Success('Question Saved.');

                        //    }
                    },
                    remote: {
                        saveQuestion: {},
                        previewQuestion: {},
                        isDirty: {}
                    }
                });
		        PxQuiz.HTS_RPC = consumer;
		    }
		},

		/* Ajax Call to load tabs */
		GetTabContent: function (selector, routeURL) {
			var bodyPostContent = $.ajax({
				type: "POST",
				url: routeURL, /* URL */
				data: {},
				beforeSend: function (jqXHR, settings) {
					PxPage.Loading($(QuestionEditorSelector.TabContainer).attr("id")); /* Blocks the UI on Ajax call */
				},
				success: function (data) {
					if (data.Status != undefined && data.Status == "error")
						$(QuestionEditorSelector.TabContainerBody).html('Unexpected error occurred while loading this tab.');
					else
						$(QuestionEditorSelector.TabContainerBody).html(data);
					$(selector).addClass(QuestionEditorSelector.SelectedTabCss);
				},
				complete: function (jqXHR, textStatus) {
					PxPage.Loaded($(QuestionEditorSelector.TabContainer).attr("id")); /* Unblocks the UI after Ajax call */
				}
			});
		},
		// Events realted to Adding note to question //
		AddNoteEvent: function () {
			$(NoteTabSelector.Form).validate();
			$(NoteTabSelector.AddNoteContainer).show();
			$(NoteTabSelector.AddNoteFormContainer).hide();
			$(NoteTabSelector.AddNoteButton).bind("click", function () {
				$(NoteTabSelector.AddNoteContainer).hide();
				$(NoteTabSelector.AddNoteFormContainer).show("slide", { direction: "up" }, 500);
			});
			$(NoteTabSelector.CancelButton).bind("click", function () {
				$('#AddNote_txt').val('');
				$(NoteTabSelector.AddNoteFormContainer).hide("slide", { direction: "up" }, 200);
				$(NoteTabSelector.AddNoteContainer).show();
			});
		},

		GetDataValue: function () {
			return DataSaved;
		},

		ShowUrlDuplicateButton: function (currentRow) {
			$(currentRow).find(".duplicate-button-container").show();
		},



		HideDuplicateButton: function (currentRow) {
			$(currentRow).find(".duplicate-button-container").hide();
		},

		ShowGearMenu: function (currentRow) {
			$(currentRow).parent().find("#menu-container").show();
		},

		HideGearMenu: function (currentRow) {

			$(currentRow).parent().find("#menu-container").hide();

		},

		HideButtons: function (currentRow) {

			$(currentRow).parent().find("#menu-container").hide();

		},

		MetadataEvent: function () {
			/*
			/////////////////////////////////////// to detect changes on the Form PX-3279  ////////////////////////////////
			$('#QBA-Metadata-Table .dirtyField').each(function () {
			$(this).bind("change", function () { DataSaved = true; });
			});
			$("#free-edit-textarea").live('change', function () {
			DataSaved = true;
			});
			*/

			$("#QBA-Metadata-ebook-section").bind("click", function () {
				PxPage.Loading("question-admin", true);
				var quizId = $(this).serialize();
				var idSerialized = quizId.replace("QBA-Metadata-ebook-section=", "");
				$.ajax(
					{
						type: "POST",
						url: select_chapter_route,
						data: { data: idSerialized },
						success:
							function (result) {
								if (result != null) {
									var selectBox = $(result).html();
									$("#QBA-Metadata-question-bank").empty();
									$("#QBA-Metadata-question-bank").html(selectBox);
									PxPage.Loaded("question-admin", true);
								}
							}
					});

			});

			var similarQuestion = $('#SimilarQuestions');
			similarQuestion.die().live('click', function (event) {
			    var quizid = similarQuestion.attr("data-qba-quiz-id");
			    if (quizid && quizid.length > 0) {
			        PxQuestionAdmin.ClearSearchForm();
			        $(defaults.QuizList).select2("val", quizid);
			        PxQuestionAdmin.ReturnToSearchPage();
			    }
			});

			$(MetadataTab.MetadataButton).bind("click", function () {
				PxPage.Loading("QBA-Metadata-Table"); /* Blocks the UI on Ajax call */
				var title = $("#QBA-Metadata-Title").val(),
				exerciseNumber = $("#QBA-Metadata-exercise-number").val(),
				guidance = $("#QBA-Metadata-guidance").attr("data-qba-full-text"),
				freeResponseQuestion = $("#QBA-Metadata-free-response").attr("data-qba-full-text"),
				questionBank = $("#QBA-Metadata-question-bank").val(),
				questionBankText = $("#QBA-Metadata-question-bank").children("option").filter(":selected").text(),
				eBookSection = $("#QBA-Metadata-ebook-section").val(),
				ebookSectionText = $("#QBA-Metadata-ebook-section").children("option").filter(":selected").text()
				difficulty = $("#QBA-Metadata-difficulty").val(),
				cognititveLevel = $("#QBA-Metadata-cognitivie-level").val(),
				bloomsDomain = $("#QBA-Metadata-blooms-domain").val(),
				entityid = $("#meta-data-container").attr("data-qba-entityid"),
				suggestedUse = $("#QBA-Metadata-Suggested-use-results").select2("data"),
				learningObjective = $("#QBA-Metadata-learning-objective-results").select2("data"),
				questionId = $("#meta-data-container").attr("data-qba-questionId"),
				oldQuizIdstring = $("#oldQuizId").val(),
				QuestionBank_Text = $("#QBA-Metadata-question-bank option[value='" + questionBank + "']\"").text(),
				eBookChapter_Text = $("#QBA-Metadata-ebook-section option[value='" + eBookSection + "']\"").text(),
				questionstatus = $("#QBA-QuestionStatus").val();
                
				for (var i = 0; i <= suggestedUse.length; i++) {

					if (typeof (suggestedUse[i]) == "object") {

						suggestedUse[i].element = "";
						try {
							delete suggestedUse[i].element;
						} catch (e) { }

					}

				}
				suggestedUse = PxQuestionAdmin.CreateFakeSelectListToSendToServer(suggestedUse);
				learningObjective = PxQuestionAdmin.CreateFakeSelectListToSendToServer(learningObjective);
				if (title == "") {
					PxPage.Loaded("QBA-Metadata-Table");
					PxPage.Toasts.Success("Please enter a title");
					return;
				}
				/*
				if (eBookSection == "0") {
				PxPage.Loaded("QBA-Metadata-Table");
				alert("Please Choose a Exercise number");
				return;

				}

				if (questionBank == "0") {
				PxPage.Loaded("QBA-Metadata-Table");
				alert("Please Choose a Question bank");
				return;
				}
				*/
				var questionMetadata = {
					QuestionId: questionId,
					Title: title,
					EntityId: entityid,
					ExcerciseNo: exerciseNumber,
					QuestionBank: questionBank,
					eBookChapter: eBookSection,
					Difficulty: difficulty,
					CongnitiveLevel: cognititveLevel,
					BloomsDomain: bloomsDomain,
					Guidance: guidance,
					FreeResponseQuestion: freeResponseQuestion,
					SuggestedUse: suggestedUse,
					LearningObjectives: learningObjective,
					QuestionBankText: questionBankText,
					eBookChapterText: ebookSectionText,
					OldQuizId: oldQuizIdstring,
					QuestionStatus: questionstatus
				};

				var request = $.ajax({
					type: "POST",
					url: update_question_metadata, /* URL */
					contentType: "application/json; charset=utf-8",
					data: JSON.stringify(questionMetadata),
					beforeSend: function (jqXHR, settings) {
						//PxPage.Loading(); /* Blocks the UI on Ajax call */
					},
					success: function (data) {
						if (data.Status != undefined && data.Status == "error") {
							//$(DialogID).html('Unexpected error occurred while adding new question.');
						}
						else {
							//alert('Question title updated successfully.');
							//window.helper.confirmMessageDialog("Changes have been saved.");
							PxPage.Toasts.Success(defaults.DataSavedMsg);
							DataSaved = false;
						}
						PxPage.Loaded("QBA-Metadata-Table");
					},
					complete: function (jqXHR, textStatus) {
						PxPage.Loaded("QBA-Metadata-Table");
					}
				});
			});

		},

		CreateFakeSelectListToSendToServer: function (thelist) {

			var theReturnList = [];
			$.each(thelist, function (d, g) {

				var values = {};
				values.Selected = false;
				values.Value = g.id;
				values.Text = g.text;
				theReturnList.push(values);
			});
			return theReturnList;
		},




		TurnDropDownToSelect2: function () {
			$("select.select2").each(function (index) {
				var currentSelect = $(this);
				var multiple = $(currentSelect).hasClass("multiple");

				if (multiple == true) {
					PxQuestionAdmin.TurnDropDownToMulitpleSelect2(currentSelect);
				} else {
					$(currentSelect).select2();

				}
			});
		},
		TruncateGuidanceFreeResponse: function () {

			var guidance = $("QBA-Metadata-guidance");
			var freeresponse = $("QBA-Metadata-free-response");

			var guidancetruncated = "";
			var freeresponsetruncated = "";
			if (freeresponse != undefined) {

				guidancetruncated = PxCommon.TruncateText(freeresponse, 300);

			}

			if (guidance != undefined) {

				guidancetruncated = PxCommon.TruncateText(guidance, 300);

			}

		},


		ViewAllQBA: function (currentItem) {

			var thetext = $(currentItem).parent().find("p").attr("data-qba-full-text");
			currentItem.parent().find("p").text(thetext);

			if (thetext.length > 300) {
				$(currentItem).addClass("hide-view-all");
			}

		},

		TurnDropDownToMulitpleSelect2: function (currentDropDown) {

			$(currentDropDown[0]).attr("data-qa-dropdown-results", currentDropDown[0].id + "-results").removeAttr("multiple");
			var resultsDropdown = currentDropDown.parent().find(".hidden-selected-results option");
			var dropdownResultId = $(currentDropDown[0]).attr("data-qa-dropdown-results");
			$(currentDropDown[0]).before(new Option("", "0"));
			$(currentDropDown).select2({ placeholder: "Search / Browse Chapters", allowClear: true });


			var options = $("#" + currentDropDown[0].id + " option").clone();
			var resultDropdown = $(document.createElement("select")).attr("id", dropdownResultId).attr("multiple", "multiple");

			$(resultDropdown).append(new Option("", "0"));
			$(resultDropdown).append(options);
			$(resultDropdown).children().removeAttr("selected");
			$(currentDropDown[0]).after(resultDropdown);
			$(resultDropdown).select2({ placeholder: "", allowClear: false });

			if (resultsDropdown.length > 0) {
				var dropdownData = []
				$.each(resultsDropdown, function () {
					var values = {};
					values.id = $(this).val();
					values.text = $(this).text();
					dropdownData.push(values);
				});
				$(resultDropdown).select2("data", dropdownData);
			}




			$(currentDropDown[0]).click(function () {
				var result = $(resultDropdown).select2("val");
				var k = $(currentDropDown[0]).select2("val");
				result.push(k);
				$(resultDropdown).val(result).trigger("change");
				$(currentDropDown[0]).val(" Please Select ").trigger("change");
			});
			/// **************** outside click event ********************** ///
			$(resultDropdown).bind("open", function () {
				$(resultDropdown).select2("close");
			});
			/*
			$(resultDropdown).bind("change", function (e) {
			DataSaved = true;
			});
			*/

		},

		TextEdit: function (currentItem) {
			if ($(".free-edit").length > 0) {

				$(".free-edit").remove();

			}
			var item = currentItem;
			var updateTo = item.parent().find("p").attr("id");
			var newDiv = $(document.createElement('div')).attr("id", "free-edit").attr("class", "free-edit").attr("update-to", updateTo);
			var textareaEdit = $(document.createElement('textarea')).attr("id", "free-edit-textarea");
			$(textareaEdit).append(currentItem.parent().find("p").attr("data-qba-full-text"))
			newDiv.append(textareaEdit);
			newDiv.dialog({
				modal: true,
				width: 600,
				height: 300,
				dialogClass: "free-edit-container",
				buttons: [{
					text: "ok",
					click: function () {
						var dialog = $(this);
						var itemToUpdate = "#" + $(this).attr("update-to");
						var updatedText = $(this).find("#free-edit-textarea").val();
						var updatedTextTruncated = $(this).find("#free-edit-textarea").val();
						if (updatedText.length > 300) {

							updatedTextTruncated = updatedText.substring(0, 300)
							$(itemToUpdate).parent().find(".free-text-view-all-button").removeClass("hide-view-all");

						}
						$(itemToUpdate).text(updatedTextTruncated);
						$(itemToUpdate).attr("data-qba-full-text", updatedText);

						$(this).dialog("close");
					}
				}, {
					text: "cancel",
					click: function () {
						$(this).dialog("close");
						DataSaved = false;
					}
				}]

			});


		},
		// Events related to Flagging question from Preview tab
		FlagEvent: function () {
		    $(PreviewTab.Container).hide();
			$(PreviewTab.FlagButton).bind("click", function () {
			    if ($('#questionContents.show-quiz').length > 0) {
			        $("#QuestionFlagContainer").addClass("CustomFlag");
			        $('#QuestionId').val($('#SelectedQuestionId').val());
			    }
			    $("#questionContents").animate({ 'width': '80%' });
				
			    $(PreviewTab.Container).show();
				$(PreviewTab.FlagButton).hide();
			});
			$(".footer .preview-button-bar").show();
			if ($('#maintable .unflagtab').is(':visible'))
			    $(PreviewTab.FlagButton).hide();
		},
		AddFlagEvent: function () {
			$(PreviewTab.FlagForm).validate();
			$(PreviewTab.AddConfirmation).hide();
			$(PreviewTab.CancelButton).bind("click", function () {
			    //$(PreviewTab.Container).hide("slide", { direction: "right" });

			    if ($('#questionContents.show-quiz').length > 0)    
			        $("#QuestionFlagContainer").removeClass("CustomFlag");

				$(PreviewTab.Container).hide();
				$("#questionContents").animate({ 'width': '100%' });
				$(PreviewTab.FlagButton).show();
			});
		},
		AddFlagCompletedEvent: function () {

			$(PreviewTab.FormContainer).hide();
			$(PreviewTab.AddConfirmation).show();
			$("#QBAEditor").find(".unflagtab").show();

			// partial working //
			//$(PreviewTab.Container).hide("slide", { direction: "right" }, 9000);
			//$("#questionContents").animate({ 'width': '100%' });


			//   testing
			//$("#questionContents").animate({ 'width': '100%' }).delay(1000);
			//$(PreviewTab.Container).delay(1000).hide();
			//$(PreviewTab.Container).animate({ 'width': '0'}).delay(2000).hide();


		},
		RemoveFlagCompletedEvent: function () {
			$("#QBAEditor").find(".unflagtab").hide();
			if ($(PreviewTab.FlagButton).length > 0) {
				$(PreviewTab.FlagButton).show();
				$(PreviewTab.FormContainer).show();
				$(PreviewTab.AddConfirmation).hide();
				$("#AddFlag_txt").val("");
			}
			PxPage.Toasts.Success("This question flag has been removed.");
		},
		HideUnflagTab: function () {
			$("#QBAEditor").find(".unflagtab").hide();
		},
		SetQuestionModifiedAction: function (url) {
			defaults.QuestionModifiedAction = url;
		},
		// Ajax call to update question log
		QuestionModifiedLog: function () {
			if (defaults.QuestionModifiedAction != null) {
				var Log = $.ajax({
					type: "POST",
					url: defaults.QuestionModifiedAction /* URL */
				});
			}
		},
		GetDataForGraphQuestion: function (questionId) {
			var xml = "";
			try {
				//Run some code here
				xml = document.getElementById("flash").getXML();
			}
			catch (err) {
				//Handle errors here
				PxPage.log("Could not retrieve XML for custom question:" + questionId);
			}
			return xml;
		},
		BlockUI: function () {
			PxPage.Loading(defaults.SearchResultId); /* Blocks the UI on Ajax call */
			$('#cmdSearch').val('Searching...');
			$('#cmdSearch').attr('disabled', 'disabled');

		},
		UnblockUI: function () {
			PxPage.Loaded(defaults.SearchResultId); /* Unblocks the UI after Ajax call */
			$('#cmdSearch').removeAttr('disabled');
			$('#cmdSearch').val('Search');
			var pageNumber = $('#SearchResultCurrentPage').val();

			$("#CurrentPage").attr("value", pageNumber);

			var totalItems = $('#SearchResultTotalItems').val();

			$('#SearchPanelTotalItems').attr("value", totalItems);

		},
		EditorBlockUI: function () {
			PxPage.Loading($(QuestionEditorSelector.TabContainer).attr("id")); /* Blocks the UI on Ajax call */
		},
		EditorUnblockUI: function () {
			PxPage.Loaded($(QuestionEditorSelector.TabContainer).attr("id")); /* Blocks the UI on Ajax call */
		},
		// Cleans up the Question Result list 
		CleanUpSearchResult: function () {
			$(defaults.QuestionListTable).find(defaults.EditorLink).die().live("click", PxQuestionAdmin.LaunchQuestionEditor);
			$('.truncated').find('img').remove();
		},
		// OnClick event to launch question Editor in FNE
		LaunchQuestionEditor: function (event) {
			var url = $(this).attr("ref");
			if (url != null)
				PxQuestionAdmin.OpenFNE(url);
		},
		// function to open FNE window it calls PxPage FNE
		OpenFNE: function (url) {
			var fneArgs = {
				url: url,
				useLocal: false,
				fixed: false,
				requireConfirm: false,
				autoSubmit: false,
				hasChanges: false,
				minimize: false,
				onFneLoaded: PxQuestionAdmin.FNEOnLoaded
			};
			PxPage.OpenFNE(fneArgs);
			$(FNESelector.fnewindow).show();
			$(defaults.QBAContainer).hide();
		},
		// FNE window loaded event callback
		FNEOnLoaded: function (event) {
			$(QuestionEditorSelector.DoneButton).bind("click", function () {
				PxQuestionAdmin.CheckUnSavedMetaData();
			});
		},
		CheckUnSavedMetaData: function () {
			var result = PxQuestionAdmin.GetDataValue();
			if (result == false) {
				$("#cmdSearch").click();
				$(defaults.QBAContainer).show();
				$(FNESelector.fnewindow).hide();
			}
			else {
				//"Alert Box","There is unsaved data are you sure you want to Done Editing",
				r = window.helper.confirmDialog(
					defaults.alert_header, defaults.unSavedMsg,
					"Close Window", "Don't Close",   //button text
					function () { PxQuestionAdmin.ReturnToSearchPage() }, // ok callBack
					function () { } // cancel callback
					);
			}
		},
		ReturnToSearchPage: function () {
			$("#cmdSearch").click();
			$(defaults.QBAContainer).show();
			$(FNESelector.fnewindow).hide();
			DataSaved = false;
		},
		// Display Quiz Selection Model to list Quizzes dropdown
		LaunchQuizSelector: function (event) {
			var questionType = $(this).attr("type");

			var newQuestionUrl = event.data.url;
			//            if($("#QuizList :selected").length == 1 && $("#QuizList").find(":selected").val() != 0)
			//            {
			//                var quizId =  $("#QuizList").find(":selected").val(); 
			//                if(quizId != "0")
			//                {
			//                     var url = newQuestionUrl + "/" + questionType + "/" + quizId;         
			//                     //$(location).attr('href', url);   
			//                     PxQuestionAdmin.CreateNewQuestion(quizId,url,defaults.SearchResultId);
			//                }
			//            }
			//            else
			//            {
			var DialogID = "QBA-QuizSelection-Dialog";
			if ($("#" + DialogID).length == 0)
				$("body").prepend("<div id=" + DialogID + "></div>");

			$("#" + DialogID).dialog({
				resizable: false,
				width: 600,
				height: 300,
				minWidth: 600,
				minHeight: 500,
				modal: true,
				title: "Please select quiz"
			});

			var QuizList = $("#QuizList").clone();
			$(QuizList).removeClass('select2-offscreen');
			$(QuizList).find("option[value='0']").remove();
			$(QuizList).removeAttr("multiple");
			$(QuizList).attr("size", 10);
			$(QuizList).attr("id", "QuizSelection");
			$(QuizList).find("option:first-child").attr("selected", "selected");

			$("#" + DialogID).html($(QuizList));
			$("#" + DialogID).dialog("option", "buttons", [
					{
						text: "Add Question",
						click: function () {
							var quizId = $("#QuizSelection").find(":selected").val();
							var url = newQuestionUrl + "/" + questionType + "/" + quizId;
							//$(location).attr('href', url);                            
							$(this).dialog("close");

							PxQuestionAdmin.CreateNewQuestion(quizId, url, defaults.SearchResultId);
						}
					},
					{
						text: "Cancel",
						click: function () {
							$(this).dialog("close");

						}
					}
				]);
			//}
		},
		// Makes AJAX call to create new Question
		CreateNewQuestion: function (quizId, routeURL, containerId) {
			var request = $.ajax({
				type: "POST",
				url: routeURL, /* URL */
				data: {},
				beforeSend: function (jqXHR, settings) {
					PxPage.Loading(containerId); /* Blocks the UI on Ajax call */
				},
				success: function (data) {
					if (data.Status != undefined && data.Status == "error") {
						//$(DialogID).html('Unexpected error occurred while adding new question.');
					}
					else {
						if (data.Url != null || data.Url != "")
							PxQuestionAdmin.OpenFNE(data.Url);
					}
				},
				complete: function (jqXHR, textStatus) {
					PxPage.Loaded(containerId); /* Unblocks the UI after Ajax call */
				}
			});
		},
		// Init function for Question Editor Tab
		QuestionEditorTab: function (question_modified_action) {
		    PxQuestionAdmin.SetQuestionModifiedAction(question_modified_action);
		    var isCustomQuestion = $('.custom-question-component').is(":visible");
			$(QuestionEditorTab.SaveButton).die().live('click', function () {
				//var isCustomQuestion = $('.custom-question-component').is(":visible");
				var questionId = $("#QuestionEditorTab").attr("data-qba-questionId");
				var disciplineCourseId = $('#QuestionEditorTab').attr('data-qba-entityid');

				if (isCustomQuestion) { /*This is for Graph Question*/
					var xml = PxQuestionAdmin.GetDataForGraphQuestion(questionId);
					customQuestionXML = $('<div/>').text(xml).html();
					var updateCallback = function () {
					    PxQuiz.UpdatePreviousQuestionOnServer(questionId, customQuestionXML, null, 'qba', disciplineCourseId);
					    PxPage.Toasts.Success('Question updated Successfully');
					};

					PxQuiz.RemoveQuestionFromCacheOnServer(questionId, updateCallback, 'qba', disciplineCourseId);
					
				}
				else {

					PxPage.FrameAPI.saveComponent(QuestionEditorTab.Editor, QuestionEditorTab.EditorComponent);
					PxQuestionAdmin.QuestionModifiedLog();
				}
			});
			$(QuestionEditorTab.UndoButton).die().live('click', function () {
				$(QuestionEditorSelector.MainTab).click();
			});
			if (PxPage.FrameAPI) {
				PxPage.FrameAPI.setShowBeforeUnloadPrompts(false);
			}
		},
		SubmitBackToIndexPage: function (quizId) {
			$("#QBA-DoneEditing").click();
			PxQuestionAdmin.ClearSearchForm();
			$("#QuizList").val(quizId);
			$("#cmdSearch").click();
			$("#QuizList").val(quizId);
		},


		BulkEditInitialize: function () {

		    // highlight first step
		    PxQuestionAdmin.HighlightRadioButton(1);
		    PxQuestionAdmin.HideConfirmButton();
		    PxPage.FneCloseHooks['BulkEditorWindow'] = PxQuestionAdmin.BulkEditorClose;

		    PxQuestionAdmin.InitializeBulkOperationParameters();

		    $("#btn_BOW_Close").click(function () {
		        PxPage.CloseFne();
		    });


		    $(document).off('click', "#chkAll_Questions").on('click', "#chkAll_Questions", function () {
		        $(".bowQBA").prop('checked', this.checked);
		        $(".bowQBA").toggleClass('chkSelected');
		    });

		    $(document).off('click', ".bowQBA").on('click', ".bowQBA", function () {
		        $(this).toggleClass('chkSelected');
		    });

		    $(document).off('change', "#bow-QuestionStatus").on('change', "#bow-QuestionStatus", function () {
		        $("#bow_confirmation_Result").html($("#bow-QuestionStatus option:selected").text());
		    });

		    $(".BulkWizardConfirm").click(function () {
		        PxQuestionAdmin.ApplyBulkOperation();
		    });

		    //change dropDown to Select2
		    $("#bow-QuestionStatus").select2({
		        placeholder: "Select a Status"                
		    });

		    //Next Button click
		    $(".BulkWizardNext").click(function () {
		        var currentStep = parseInt($(".wizardState").val());
		        if (currentStep == 5) {
		            currentStep = 1;
		            $(".bowRadio").prop('checked', false).next().removeClass("SelectedRadioButton");
		        }

		        if (currentStep == 3) {
		            if (!$(".bowChooseOperation").is(":checked"))
		            { PxPage.Toasts.Error("Please make a selection"); return; }
		        }
		        if (currentStep == 4) {
		            if (!$(".bowOperationStatus").is(":checked"))
		            { PxPage.Toasts.Error("Please make a selection"); return; }
					
					
		            // validate if Question-Status is set 
		            if ($("#bow-QuestionStatus option:selected").val() < 0)
		            { PxPage.Toasts.Error('Please select a valid status option '); return; }

		        }
		        PxQuestionAdmin.HighlightRadioButton(currentStep);
		    });

		    /*
			$(".BulkWizardCancel").click(function () {
				var currentStep = parseInt($(".wizardState").val());
				if (currentStep == 5)
					PxQuestionAdmin.HideConfirmButton();


				if (currentStep > 2) {
					currentStep -= 1;
					var currentRadio = $("input[type='radio'][value='" + currentStep + "']").filter('.bowRadio');
					var step = currentStep - 1;
					var previousRadio = $("input[type='radio'][value='" + step + "']").filter('.bowRadio');
					currentRadio.prop('checked', false).next().removeClass("SelectedRadioButton visitedBulkEditRadio");
					previousRadio.prop('checked', true).next().removeClass("visitedBulkEditRadio").addClass("SelectedRadioButton");
					$(".wizardState").val(currentStep.toString());
					PxQuestionAdmin.HideDiv(step);
				}
				else
					if (currentStep == 2) {
						currentStep -= 1;
						$("input[type='radio'][value='" + currentStep + "']").filter('.bowRadio').prop('checked', true).next().addClass("SelectedRadioButton");
						PxQuestionAdmin.HideDiv(currentStep);
					}
			});
            */
		    $(".BulkWizardCancel").click(function () {
		        PxQuestionAdmin.BulkEditorClose();
		    });
			

		    $(".bowRadio").click(function () {
		        var $this = $(this);
		        var radioID = $this.prop("value");
		        var currentStep = parseInt($(".wizardState").val());
		        if (radioID >= currentStep) return false;
		        if ($(".BulkWizardConfirm").is(":visible"))
		            PxQuestionAdmin.HideConfirmButton();
		        
                var step = parseInt(radioID) + 1;
		        if (radioID > 1)
		        {
		            
		            $('.bowRadio').prop('checked', false).each(function () { $(this).prop('checked', false).next().removeClass("visitedBulkEditRadio SelectedRadioButton"); });
		            $this.prop('checked', true).next().addClass("SelectedRadioButton");
		            $('.bowRadio').each(function () {
		                var $obj = $(this);
		                if (parseInt($obj.prop("value")) < parseInt(radioID))
		                    $obj.next().addClass("visitedBulkEditRadio");
		            }); 
		            if (currentStep > 2) {
		                currentStep -= 2;
		                PxQuestionAdmin.HideDiv(currentStep);
		                currentStep = parseInt(radioID) + 1;
		                if (currentStep > 4) currentStep = 4;
		                $(".wizardState").val(currentStep.toString());
		            }
		        }
		        else // first step
		        {
		            $('.bowRadio').prop('checked', false).each(function () { $(this).next().removeClass("visitedBulkEditRadio SelectedRadioButton") });
		            $("input[type='radio'][value='1']").filter('.bowRadio').prop('checked', true).next().addClass("SelectedRadioButton");
		            currentStep = 2;
		            $(".wizardState").val(currentStep.toString());
		            PxQuestionAdmin.HideDiv(1);
		        }
		    });

		},

		BulkEditorClose: function()
		{
			$("#BulkEditor").remove();
			$(defaults.QBAContainer).show();
			$(FNESelector.fnewindow).hide();
		},

		HideDiv: function (divID) {
			var currentDiv = "#pv_" + divID;
			$(".bowpartials").hide().filter(currentDiv).show();
		},
		HighlightRadioButton: function (step) {

			if (step == 4) {
				// change button text to Confirm button
				PxQuestionAdmin.ShowConfirmButton();
				// set confirm button click event to request to server code
			}
			var currentRadio = $("input[type='radio'][value='" + step + "']").filter('.bowRadio');
			currentRadio.prop('checked', true);
			currentRadio.next().addClass("SelectedRadioButton");
			var previousRadio = $("input[type='radio'][value='" + (step - 1) + "']").filter('.bowRadio').prop('checked', false);
			previousRadio.next().toggleClass("SelectedRadioButton").addClass("visitedBulkEditRadio");
			// hide other divs
			PxQuestionAdmin.HideDiv(step);
			step += 1;
			if (step > 4) step = 4;

			$(".wizardState").val(step.toString());

		},
		HideConfirmButton: function () {
			$(".BulkWizardNext").show();
			//$("#BulkWizardCancel").hide();
			$(".BulkWizardConfirm").hide();
		},
		ShowConfirmButton: function () {
			$(".BulkWizardNext").hide();
			$(".BulkWizardConfirm").show();
		},

		InitializeBulkOperationParameters: function () {

			$('.cHeader , .cFooter').hide();

			var responseFormat = [], questionStatuslist = [];
			$("input[name='StatusSelectedValues']:checked").each(function () {
				questionStatuslist.push($(this).val());
			});
			$("input[name='FormatSelectedValues']:checked").each(function () {
				responseFormat.push($(this).val());
			});
			var quizSelected = "0";
			if ($(defaults.QuizList).val() != null)
				quizSelected = $(defaults.QuizList).val(); 

			var currentPage =  $("#SearchResultCurrentPage").val(), 
		        rowCount =25, 
		        skipCount = (currentPage -1) * rowCount;

           var objPagination = {
			    TotalItems:  $("#SearchResultTotalItems").val(),
			    CurrentPage: currentPage, 
			    CurrentPageByButton: currentPage, 
			    Take: rowCount, 
			    Skip: skipCount
			}

			var questionSearchModel = {
				SearchKeyword: $(defaults.SearchBoxID).val(),
				FormatSelectedValues: responseFormat,
				StatusSelectedValues: questionStatuslist,
				ChapterSelectList: $(defaults.ChapterList_new).val(),
				ChapterSelectedValues: $(defaults.ChapterList_new).val(),
				QuizSelectedValues: quizSelected,
				FlagSelectedValues: $("input[name='FlagSelectedValues']:checked").val(),
				BulkEdit: true,
				Pagination: objPagination
			}

			var objJson = JSON.stringify(questionSearchModel);
			var request = $.ajax({
				type: "POST",
				url: QBA_search_route,
				contentType: "application/json; charset=utf-8",
				data: objJson,
				beforeSend: function (jqXHR, settings) {
					PxPage.Loading("fne-content"); /* Blocks the UI on Ajax call */
				},
				success: function (data) {
					if (data.Status != undefined && data.Status == "error") {
						PxPage.Toasts.Error('Error Loading Bulk Edit Question List');
					}
					else {
						$('.cHeader , .cFooter').show();
						var bulkEditMarkup = $("#pv_1").html(data);
						bulkEditMarkup.find("#QBA-QuestionList-Table #main-action-container").remove();
						bulkEditMarkup.find("#QBA-QuestionList-Table .QuestionTitle a").removeClass("EditorLink");
					}
					PxPage.Loaded("fne-content");
				},
				complete: function (jqXHR, textStatus) {
					PxPage.Loaded("fne-content");
				}
			});
		},

		ApplyBulkOperation: function () {

			
			var questionlist = [], questionIDs = [];
			$(".bowQBA.chkSelected").each(function () {
				var $this = $(this);
				questionlist.push($this.prop('name'));
				questionIDs.push($this.val());
			});
			if (questionlist.length < 1 || questionIDs.length < 1)
			{ PxPage.Toasts.Error('No Questions selected for Bulk edit'); return; }


			var BulkEditModel = {
				ChapterSelectedValues: $(defaults.ChapterList_new).val(), //$($(".bowQBA.chkSelected").get(0)).prop('name'),
				QuizSelectedValues: questionlist,
				BulkEditSelectedQuestions: questionIDs,
				BulkEdit: true,
				BulkEditStatus: $("#bow-QuestionStatus option:selected").val()
			}

			var objJson = JSON.stringify(BulkEditModel);

			var request = $.ajax({
				type: "POST",
				url: QBA_BulkEdit,
				contentType: "application/json; charset=utf-8",
				data: objJson,
				beforeSend: function (jqXHR, settings) {
					PxPage.Loading("fne-content"); /* Blocks the UI on Ajax call */
				},
				success: function (data) {
					if (data.Status != undefined && data.Status == "error") {
						PxPage.Toasts.Error("bulk operation failed on server");
					}
					else {
						// on success of search result
						PxPage.CloseFne();
						$(defaults.cmdSearch).click();
						PxPage.Toasts.Success("Bulk operation completed successfully");
					}
					PxPage.Loaded("fne-content");
				},
				complete: function (jqXHR, textStatus) {
					PxPage.Loaded("fne-content");
				}
			});
		}

	};
} (jQuery);