var PxRubricBuilder = function ($) {
    var deleteButton = '<div class="rubric-delete-button"></div><div class="hoverdelete">Delete</div>',
        descriptionDefaultText = 'Enter performance description',
        criterionDefaultText = 'Enter criteria',
        maxScoreDefaultText = 'Enter Max Score',
        scoreDefaultText = 'Enter Score';
    var rubricTable = $('.rubric-grid').first();
    return {
        BindControls: function () {
            $('.add-rubric-row').unbind().bind('click', PxRubricBuilder.AddRow);
            $('.add-rubric-column').unbind().bind('click', PxRubricBuilder.AddColumn);
            $('.rubric-delete-column .rubric-delete-button').die().live('click', PxRubricBuilder.DeleteColumn);
            $('.rubric-delete-row-button').die().live('click', PxRubricBuilder.DeleteRow);
            $('.rubric-editor .rubric-row span.rubric-description').die().live('click', PxRubricBuilder.OnColumnClick);
            $('.rubric-row .rubric-score-column').die().live('click', PxRubricBuilder.OnScoreColumnClick);
            $('.rubric-row .rubric-score-column input').die().live('keyup', PxRubricBuilder.OnScoreColumnKeyUp);

            $('.rubric-column').live('hover', PxRubricBuilder.OnHoverDescriptionColumn);
            $('.rubric-grid').live('mouseleave', PxRubricBuilder.OnRubricGridLeave);
            $('.rubric-save-text').die().live('click', PxRubricBuilder.OnSaveTextClick);
            $('.rubric-cancel-text').die().live('click', PxRubricBuilder.OnCancelTextClick);

            $('.rubric-delete-button').die('mouseenter', PxRubricBuilder.OnHoverDeleteButton);
            $('.rubric-delete-button').live('mouseenter', PxRubricBuilder.OnHoverDeleteButton);

            $('.rubric-delete-button').die('mouseleave', PxRubricBuilder.OnMouseLeaveDeleteButton);
            $('.rubric-delete-button').live('mouseleave', PxRubricBuilder.OnMouseLeaveDeleteButton);
        },
        Build: function (data) {
            if ($('.rubric-editor .rubric-grid').length) {
                rubricTable = $('.rubric-grid').first();

                rubricTable.empty();
                PxRubricBuilder.BindControls();
                data = $.parseJSON(data);

                if (data.Rows.length < 1) {
                    PxRubricBuilder.AddRow();
                }

                if (data == null || data.Rows == null || data.Rows.length < 1) {
                    return;
                }
                rubricTable.append(PxRubricBuilder.GetDeleteRow(data.ColumnTitles, data.MaxScore));
                rubricTable.append(PxRubricBuilder.GetHeaderRow(data.ColumnTitles, data.MaxScore));
                for (var i = 0; i < data.Rows.length; i++) {
                    var row = PxRubricBuilder.GetRow(data.Rows[i]);
                    rubricTable.append(row);
                }

                PxRubricBuilder.SetDefaultValue();
                PxRubricBuilder.ReCalcTotalPoints();
            }
        },
        GetDeleteRow: function (columnTitle, maxScore) {
            var titleRow = $('<tr class="rubric-delete-row"></tr>');
            titleRow.append('<td class="rubric-row-delete-column rubric-column"></td><td class="rubric-info-container rubric-column"><span class="rubric-info-cell"></span></td>');

            if (columnTitle == null || columnTitle.length < 1) {
                maxScore = '';
                titleRow.append(PxRubricBuilder.GetDeleteColumn(''));
            }
            else {
                for (var i = 0; i < columnTitle.length; i++) {
                    var delColumn = PxRubricBuilder.GetDeleteColumn('');
                    titleRow.append(delColumn);
                }
            }

            titleRow.append('<td class="rubric-info-container rubric-column"><span class="rubric-info-cell"></span></td>');

            return titleRow;
        },

        ReCalcTotalPoints: function () {
            var totalPointsLabel = $(rubricTable).find('.rubric-score-column div.rubric-info-cell'),
                pointsPerRow = $(rubricTable).find('.rubric-score-column input');

            // if no rows that could have points exist, nothing to count
            if (pointsPerRow.length == 0) {
                totalPointsLabel.text = 'Total Score TBD';
                return;
            }

            // count each row, blank entried will be treated like '0'
            var pointsCount = 0;
            $(pointsPerRow).each(function (idx) {
                points = $(pointsPerRow[idx]).val();
                if (points.length > 0) {
                    pointsCount += points * 1;
                }
            });

            totalPointsLabel.text(pointsCount);
        },

        AdjustHeaderRowScores: function () {
            var columnLength = $(rubricTable).find('.rubric-header-row .rubric-heading-column').length;
            var perColumn = Math.floor(100 / (columnLength - 1));
            $(rubricTable).find('.rubric-header-row .rubric-heading-column').each(function (idx) {
                var columnIndex = idx,
                    columnScore = 0;
                if (columnIndex == 0) columnScore = 100
                else {
                    columnScore = ((columnLength - 1) - columnIndex) * perColumn;
                }
                $(this).find('span.score').text(columnScore.toFixed(0) + '%');
            });
        },

        GetHeaderRow: function (columnTitle, maxScore) {
            var titleRow = $('<tr class="rubric-header-row"></tr>');
            titleRow.append('<td class="rubric-row-delete-column rubric-column"></td><td class="rubric-info-container rubric-column"><span class="rubric-info-cell">' + $('#rubric-info').val() + '</span></td>');

            if (columnTitle == null || columnTitle.length < 1) {
                maxScore = '';
                titleRow.append(PxRubricBuilder.GetHeadingColumn(''));
            }
            else {
                for (var i = 0; i < columnTitle.length; i++) {
                    var colTitle = PxRubricBuilder.GetHeadingColumn(columnTitle[i].Title);
                    titleRow.append(colTitle);
                }
            }

            titleRow.append('<td class="rubric-info-container rubric-score-column rubric-column"><span class="rubric-info-cell">Points Possible</span><div class="rubric-info-cell">Total Score TBD</div></td>');
            return titleRow;
        },
        GetRow: function (row) {
            var rowElement = $('<tr class="rubric-row"></tr>');
            var score = '';
            if (row == null || row == undefined) {
                rowElement.append(PxRubricBuilder.GetRowDeleteColumn());
                rowElement.append(PxRubricBuilder.GetRowTitleColumn(''));
                rowElement.append(PxRubricBuilder.GetColumn(''));
            }
            else {
                score = row.Score;
                rowElement.append(PxRubricBuilder.GetRowDeleteColumn());
                rowElement.append(PxRubricBuilder.GetRowTitleColumn(row.Title || ''));
                for (var i = 0; i < row.Columns.length; i++) {
                    rowElement.append(PxRubricBuilder.GetColumn(row.Columns[i].Description));
                }
            }
            rowElement.append(PxRubricBuilder.GetScoreColumn(score));
            return rowElement;
        },
        GetDeleteColumn: function (columnText) {
            var deleteButtonDiv = $(deleteButton);
            var columnElement = $('<td></td>').append(deleteButtonDiv).addClass('rubric-column').addClass('rubric-delete-column');
            return columnElement;
        },
        GetRowDeleteColumn: function (columnText) {
            var deleteButtonDiv = $(deleteButton);
            var columnElement = $('<td></td>').append(deleteButton).addClass('rubric-column').addClass('rubric-delete-column');
            return columnElement;
        },
        GetHeadingColumn: function (columnText) {
            var textbox = $('<input type="text" name="textbox" id="textbox" value="" >').val(columnText);
            var span = $('<span></span>').addClass('score');
            var columnElement = $('<td></td>').append(textbox).append(span).addClass('rubric-column').addClass('rubric-heading-column');
            return columnElement;
        },
        GetColumn: function (columnText) {
            var span = $('<span></span>').addClass('rubric-description');
            if (columnText) { span.text(columnText); }
            var columnElement = $('<td></td>').append(span).addClass('rubric-column').addClass('rubric-description-column');
            return columnElement;
        },
        GetScoreColumn: function (columnText) {
            var textbox = $('<input type="text" name="textbox" id="textbox" value="" >').val(columnText);
            var columnElement = $('<td></td>').append(textbox).addClass('rubric-column').addClass('rubric-score-column');
            return columnElement;
        },
        GetRowDeleteColumn: function () {
            var rowDeleteContainer = $('<div class="rubric-delete-row-button">' + deleteButton + '</div>');
            var deleteTd = $('<td class="rubric-row-delete-column rubric-column"></td>').append(rowDeleteContainer);
            return deleteTd;
        },
        GetRowTitleColumn: function (title) {
            var span = $('<span></span>').text(title).addClass('rubric-description');
            var titleTd = $('<td></td>').append(span).addClass('rubric-column').addClass('rubric-row-title');
            return titleTd;
        },
        AddRow: function () {
            var existingButtons = $('.rubric-grid .rubricTextButtons');
            if (existingButtons.length) {
                existingButtons.find('.rubric-save-text').click();
            }
            PxRubricBuilder.RemoveActiveEditor();

            var lastTr = rubricTable.find('tr.rubric-row:last');
            var newTr;
            if (lastTr.length) {
                newTr = lastTr.clone();
                newTr.find('span').text('');
                newTr.find('input').val('');
                rubricTable.append(newTr);
            }
            else {
                rubricTable.append(PxRubricBuilder.GetDeleteRow());
                rubricTable.append(PxRubricBuilder.GetHeaderRow());
                newTr = PxRubricBuilder.GetRow();
                rubricTable.append(newTr);
                PxRubricBuilder.AddColumn();
                PxRubricBuilder.AddColumn();
                PxRubricBuilder.AddColumn();
            }
            newTr.find('.rubric-description, .rubric-score-column').addClass('fade-effect');
            PxPage.Fade();
            var scrollheight = $('.rubric-grid-container')[0].scrollHeight + 100;
            $('.rubric-grid-container').animate({ scrollTop: scrollheight }, 'fast');

            PxRubricBuilder.SetDefaultValue();
        },
        AddColumn: function () {
            var existingButtons = $('.rubric-grid .rubricTextButtons');
            if (existingButtons.length) {
                existingButtons.find('.rubric-save-text').click();
            }
            PxRubricBuilder.RemoveActiveEditor();

            var trs = rubricTable.find('tr');
            if (trs.length) {
                trs.each(function () {
                    if ($(this).hasClass('rubric-delete-row')) {
                        var newTd = $(this).find('.rubric-delete-column:last').clone();
                        $(this).find('td:last').before(newTd);
                    }
                    else if ($(this).hasClass('rubric-header-row')) {
                        var newTd = $(this).find('.rubric-heading-column:last').clone();
                        newTd.find('#textbox').val('');
                        $(this).find('td:last').before(newTd);
                    }
                    else {
                        var newTd = $(this).find('.rubric-description-column:last').clone();
                        newTd.find('span').text('');
                        $(this).find('td:last').before(newTd);
                    }
                });

                PxRubricBuilder.SetDefaultValue();
            }
            else {
                PxRubricBuilder.AddRow();
            }
        },
        SetDefaultValue: function () {
            if ($('.rubric-grid').hasClass('rubric-preview') == false) {
                $('.rubric-grid span').each(function () {
                    if ($(this).parents('td').hasClass('rubric-description-column')) {
                        this.defaultValue = descriptionDefaultText;
                        if ($(this).text() == '') {
                            $(this).text(descriptionDefaultText);
                        }
                    }
                    else if ($(this).parents('td').hasClass('rubric-row-title')) {
                        this.defaultValue = criterionDefaultText;
                        if ($(this).text() == '') {
                            $(this).text(criterionDefaultText);
                        }
                    }
                    else if ($(this).parents('td').hasClass('rubric-max-score')) {
                        this.defaultValue = maxScoreDefaultText;
                        if ($(this).text() == '') {
                            $(this).text(maxScoreDefaultText);
                        }
                    }
                    else if ($(this).parents('td').hasClass('rubric-score-column')) {
                        this.defaultValue = scoreDefaultText;
                        if ($(this).text() == '') {
                            $(this).text(scoreDefaultText);
                        }
                    }
                });
            }
            PxRubricBuilder.AdjustHeaderRowScores();
        },
        DeleteRow: function () {
            var existingButtons = $('.rubric-grid .rubricTextButtons');
            if (existingButtons.length) {
                existingButtons.find('.rubric-save-text').click();
            }
            PxRubricBuilder.RemoveActiveEditor();

            if (confirm("Are you sure you want to delete the selected row?")) {
                if (rubricTable.find('tr.rubric-row').length == 1) {
                    rubricTable.empty();
                    return;
                }
                $(this).parents('tr').remove();
            }

            PxRubricBuilder.ReCalcTotalPoints();
        },
        DeleteColumn: function () {
            var existingButtons = $('.rubric-grid .rubricTextButtons');
            if (existingButtons.length) {
                existingButtons.find('.rubric-save-text').click();
            }
            PxRubricBuilder.RemoveActiveEditor();

            if (confirm("Are you sure you want to delete the selected column?")) {

                if ($(this).closest("td").parent('tr').find('td.rubric-delete-column').length == 1) {
                    var index = $(this).closest("td").prevAll("td").length;

                    $(rubricTable).find('.rubric-header-row').find('td:eq(' + index + ') input[type="text"]').val('');
                    rubricTable.find('.rubric-row').find('td:eq(' + index + ') span').html('');

                    PxRubricBuilder.SetDefaultValue();

                    return;
                }
                else {
                    var index = $(this).closest("td").prevAll("td").length;
                    rubricTable.find('tr').each(function () {
                        $(this).find('td:eq(' + index + ')').remove();
                    });

                    PxRubricBuilder.SetDefaultValue();
                }
            }
        },
        OnScoreColumnClick: function (event) {
            var existingButtons = $('.rubric-grid .rubricTextButtons');
            if (existingButtons.length) {
                existingButtons.find('.rubric-save-text').click();
            }

            PxRubricBuilder.RemoveActiveEditor();
        },

        OnScoreColumnKeyUp: function (event) {
            //to allow decimals,use/[^0-9\.]/g 
            var regex = new RegExp(/[^0-9\.]/g);
            var containsNonNumeric = this.value.match(regex);
            if (containsNonNumeric)
                this.value = this.value.replace(regex, '');

            this.value = this.value.replace(/^(.*?\.)(.*)$/, function ($0, $1, $2) {
                return $1 + $2.replace(/\./g, "");
            });
            PxRubricBuilder.ReCalcTotalPoints();
        },

        OnColumnClick: function (event) {
            var $this = $(this),
                textAreaToAppend = $('<textarea style="width: 100%; height: 60px; font-size: 0.9em;" rows="2" name="RubricText"  maxlength="1024" id="RubricText" cols="20" class="rubricTextbox rubricText-editor"></textarea>'),
                buttonsToAppend = $('<div class="rubricTextButtons"><input type="button" id="saveRubricText" value="Save" name="saveRubricText" class="rubric-save-text linkButton" /><input type="button" id="cancelRubricText" value="Cancel" name="cancelRubricText" class="rubric-cancel-text linkButton" /></div>'),
                currentHtml = $this.html(),
                existingButtons = $('.rubric-grid .rubricTextButtons');

            if (existingButtons.length) {
                existingButtons.find('.rubric-save-text').click();
            }
            PxRubricBuilder.RemoveActiveEditor();

            textAreaToAppend.val(currentHtml);

            $this.hide();
            $this.parent().prepend(textAreaToAppend);
            $this.parent().append(buttonsToAppend);

            PxRubricBuilder.AddActiveEditor();
        },
        OnSaveTextClick: function (event) {
            var $this = $(this),
                parentColumn = $(this).closest('.rubric-column'),
                spanBlock = parentColumn.find('span.rubric-description'),
                text = tinyMCE.get('RubricText').getContent();
            PxRubricBuilder.RemoveActiveEditor();
            spanBlock.html(text);
            parentColumn.find('.rubricTextButtons').remove();
            spanBlock.show();
        },
        OnCancelTextClick: function (event) {
            var $this = $(this),
                parentColumn = $(this).closest('.rubric-column'),
                spanBlock = parentColumn.find('span.rubric-description'),
                text = tinyMCE.get('RubricText').getContent();
            PxRubricBuilder.RemoveActiveEditor();
            parentColumn.find('.rubricTextButtons').remove();
            spanBlock.show();
        },

        OnHoverDeleteButton: function () {
            $(this).parents(".rubric-row-delete-column").find(".hoverdelete").text('Delete Row').show();
            $(this).parents(".rubric-column.rubric-delete-column").find(".hoverdelete").text('Delete Column').show();
        },

        OnMouseLeaveDeleteButton: function () {
            $(this).parents(".rubric-row-delete-column").find(".hoverdelete").hide();
            $(this).parents(".rubric-column.rubric-delete-column").find(".hoverdelete").hide();
        },

        OnHoverDescriptionColumn: function (event) {
            var index = $(this).prevAll("td").length;
            $('.rubric-delete-button').hide();
            $('.rubric-delete-row').find('td:eq(' + index + ')').find('.rubric-delete-button').show();
            $(this).closest('tr.rubric-row').find('.rubric-delete-button').show();
        },
        OnRubricGridLeave: function (event) {
            $('.rubric-delete-button').hide();
        },

        AddActiveEditor: function () {
            if (PxPage.Context.course_tinyMCE_options == "") {
                tinyMCE.init(PxPage.rubricText_editor_options);
            }
            else {
                tinyMCE.init(opts[PxPage.Context.course_tinyMCE_options]);
            }

            tinyMCE.execCommand('mceAddControl', false, 'RubricText');
        },

        RemoveActiveEditor: function () {
            try {
                if ($(".rubric-grid").find('textarea[name=RubricText]').length) {
                    $(".rubric-grid").find('textarea[name=RubricText]').remove();
                    try {
                        if (tinyMCE.activeEditor) {
                            tinyMCE.activeEditor.remove();
                        }
                    }
                    catch (e) {
                        //PxPage.log(e);
                    }

                    $('.rubric-grid').find(".mceEditor").remove();
                }
            }
            catch (e) {
                PxPage.log("Unbinding exception thrown for tinymce" + e);
            }
        },

        SaveRubricText: function () {
            var existingButtons = $('.rubric-grid .rubricTextButtons');
            if (existingButtons.length) {
                existingButtons.find('.rubric-save-text').click();
            }
            PxRubricBuilder.RemoveActiveEditor();
        },

        GetRubricData: function () {
            rubricTable = $('.rubric-grid').first();
            var rubricData = {};
            var columnTitles = [];
            var rows = [];  
            var rubricrule = [];
            $(rubricTable).find('.rubric-header-row td.rubric-column input[type="text"]').each(function (idx) {
                var columnValue = $(this).val();
                if (columnValue == descriptionDefaultText) { columnValue = ''; }
                columnTitles.push({ 'Sequence': idx + 1, 'Title': columnValue });
            });
            rubricData.MaxScore = $('.rubric-max-score input[type="text"]').val();
            rubricData.Title = $('.rubric-title').val();
            rubricData.ColumnTitles = columnTitles;

            $(rubricTable).find('.rubric-row').each(function (rowIdx) {
                var rubrictexts = [];

                var row = {};
                var columns = [];
                row.Sequence = rowIdx + 1;
                row.Score = $(this).find('td.rubric-score-column input[type="text"]').val();
                row.Title = $(this).find('td.rubric-row-title').text();
                if (row.Title == criterionDefaultText) { row.Title = ''; }

                var rubrictext = {};
                rubrictext.paragraph = row.Title;
                rubrictexts.push(rubrictext);

                $(this).find('td.rubric-column:not(.rubric-row-title) span.rubric-description').each(function (colIdx) {

                    var columnValue = $(this).text();
                    if (columnValue == descriptionDefaultText) { columnValue = ''; }

                    columns.push({ 'Sequence': colIdx + 1, 'Description': columnValue });

                    var rubrictext = {};
                    rubrictext.paragraph = columnValue;
                    rubrictexts.push(rubrictext);

                });

                row.Columns = columns;
                rows.push(row);

                var rule = {};
                rule.id = row.Sequence;
                rule.MaxScore = row.Score;
                rule.RubricTexts = rubrictexts;
                rubricrule.push(rule);

            });
            rubricData.Rows = rows;
            rubricData.RubricRules = rubricrule;
            return rubricData;
        }
    };
} (jQuery);