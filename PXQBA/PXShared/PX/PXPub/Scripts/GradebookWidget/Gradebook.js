//Script file for controlling actions associated with the GradebookWidget
var PxGradebook = function ($) {
    var _static = {
        sel: {
            control: 'gbcontrol',
            exportgrid: 'export',
            key: 'key',
            keyContainer: 'gridkey',
            keyClose: 'closeCol',
            nameCol: 'nameCol',
            avgCol: 'avgCol',
            submissionCol: 'submissionCol',
            attemptsCol: 'attemptsCol',
            sortableCols: 'sortableCol',
            fixedCols: 'fixedCol',
            menuHeaderCols: 'menuHeaderCol',
            tooltipTemplate: 'tooltipTemplate',
            tooltipWrapper: 'headerToolTips',
            tooltip: 'menutip',
            ttHeaderSort: '.headerSort',
            ttHeaderAssignment: '.headerAssignment',
            ttHeaderSummary: '.headerSummary',
            content: 'gradebookContent',
            assignedScoresTab: 'assignedScoresTab',
            otherScoresTab: 'otherScoresTab',
            studentDetailsAssignedTab: 'assignedtab',
            studentDetailsOtherTab: 'othertab',
            studentDetailFilterDiv: "filter",
            assignedScoresContent: 'assignedScoresContent',
            otherScoresContent: 'otherScoresContent',
            itemScoresContent: 'itemScoreContent',
            studentDetailContent: 'assignments',
            studentDetailUnassignedContent: 'unassigneditems',
            assignmentSummaryContent: 'summaryContent',
            otherItemGroup: 'itemgroup',
            otherStudentGroup: 'studentgroup',
            otherGroupByItem: 'byItem',
            otherGroupByStudent: 'byStudent',
            studentDetailFilter: 'assignmentFilter',
            itemBack: 'backToScores',
            tableToolsContainer: 'DTTT_container',
            hdnClassAvg: '#gradebookContent #hdnClassAvg',
            classAvgText: '#gbheader .classAvg',
            classAvgPercent: '#gbheader .classAvgPercent',
            assigneditems: '#studentgradebook #divassigneditems',
            unassigneditems: '#studentgradebook #divunassigneditems',
            studentAssignmentTables: '#assignments, #unassigneditems',
            divStudentAssignments: 'studentassignments',
            studentGradeBook: '#studentgradebook',
            studentAvgVal: '.studentAvgVal',
            hdnAssignedAvg: '#studentgradebook #hdnAssignedAvg',
            hdnUnAssignedAvg: '#studentgradebook #hdnUnAssignedAvg',
            studentAssignedTable: '#studentgradebook #assignments',
            studentUnassignedTable: '#studentgradebook #unassigneditems',
            spanGradeRule: '#gradeRule',
            assignmentToggle: '.assignmentToggle',
            toggleClose: 'toggleclose',
            toggleOpen: 'toggleopen',
            studentAssignedExport: '#studentgradebook .assignedExport',
            studentUnAssignedExport: '#studentgradebook .unassignedExport',
            allAssignmentReportBtn: '.gridButtons .allAssignmentReportBtn',
            allAdditionalItemReportBtn: '.gridButtons .allAdditionalItemReportBtn',
            showAllAdditionalExportBtn: '#gradebookContent .showAllAdditionalExportBtn',
            dialogCloser: 'dialogcloser'    //mark this on links that close dialogs
        },
        modes: {
            student: 'student',
            instructor: 'instructor'
        },
        constant: {
            studentSortOrder: 'studentSortOrder'
        },
        state: {
            //Active datatable in the main browser window
            table: '',
            //Active datatable in a dialog, because we need to keep those actions separate from a datatable in main window
            dialogTable: '',
            //Holds col information for the Assigned Scores view
            sortedCol: {
                colId: '',
                direction: ''
            },
            mode: '',
            //If in student mode, the details of the student
            student: {
                userId: '',
                enrollmentId: ''
            },
            initialized: false
        },
        events: {
            viewAssignment: 'viewassignment'
        },
        loadQueue: [],
        fn: {
            init: function (mode) {
                if (_static.state.initialized) {
                    PxPage.log('Tried to reinitialize PxGradebook');
                    return;
                }

                _static.state.mode = mode;

                //Initialize jQuery UI radio buttons
                $('#radio').buttonset();
                $('#' + _static.sel.control + ' *[ref]').live('click', _static.fn.setHash);

                //Setup button click handlers
                $('.gridButtons #' + _static.sel.exportgrid).click(function (event, ui) {
                    $('.' + _static.sel.tableToolsContainer + ' a').trigger('click', [event, ui]);
                });
                $('.gridButtons #' + _static.sel.key).click(function () {
                    $('#' + _static.sel.keyContainer).toggle();
                });

                _static.fn.runQueuedCommands();
                _static.state.initialized = true;
            },
            //Sets the hash for controls setup with the "ref" attribute
            setHash: function (event) {
                var ref = $(event.target).closest('*[ref]').attr('ref');
                HashHistory.SetHash(ref);
            },
            parseQuerystring: function (args) {
                var nvpair = {};
                var qs = args || "";
                var pairs = qs.split('&');
                $.each(pairs, function (i, v) {
                    var pair = v.split('=');
                    nvpair[pair[0]] = pair[1];
                });
                return nvpair;
            },
            //Handles any routes that would lead to an external control, like viewAssignment, and fires an appropriate event for a product to handle
            handleRoute: function (state, func, args) {
                if (GradebookRoutes[func] !== undefined) {
                    GradebookRoutes[func].apply(this, [args]);
                }
            },
            //Runs through commands that have been queued into the gradebook queue
            runQueuedCommands: function () {
                while (_static.loadQueue.length > 0) {
                    _static.loadQueue.shift().apply(this, []);
                }
            },
            initAssignedScores: function () {
                $('.gridButtons #' + _static.sel.key).show();
                $(_static.sel.allAssignmentReportBtn).show();
                $(_static.sel.allAdditionalItemReportBtn).hide();

                var gridDiv = $('#' + _static.sel.assignedScoresContent);
                if (gridDiv.length > 0) {

                    //Populate col's hash.  Need to do this before setting up data table because dup tables are created.
                    var colindex = 0;
                    _static.state.cols = {};
                    $('#' + _static.sel.assignedScoresContent + ' th').each(function () {
                        //Because the FixedColumn plugin creates copies of columns, we want to make sure we dont process
                        //one of the cloned columns twice.
                        if (_static.state.cols[$(this).attr('id')] === undefined && $(this).hasClass(_static.sel.menuHeaderCols)) {
                            _static.state.cols[$(this).attr('id')] = {
                                //Keep track of the columns index and the id of its associated tooltip
                                colIndex: colindex++,
                                colTipId: ''
                            };
                        } else {
                            colindex++;
                        }
                    });

                    var numberOfColumns = gridDiv.find('tr')[0].cells.length;

                    //Setup datatable
                    _static.state.table = gridDiv.dataTable({
                        'sScrollX': '100%',
                        'sScrollY': '200',
                        'bFilter': false,
                        'bPaginate': false,
                        'bLengthChange': false,
                        'bScrollCollapse': true,
                        'sDom': 'T<"clear">lfrtip',
                        'fnRowCallback': function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                            //Sorting recreates the rows (when using the fixed columns) so we need to apply our row changes each time
                            $(nRow).find('td.colName').css('cursor', 'pointer').click(_static.fn.onStudentNameClicked);
                        },
                        'aoColumnDefs': [
                            { 'bSortable': true, 'sWidth': '130px', 'aTargets': [_static.sel.nameCol] },
                            { 'bSortable': true, 'sWidth': '120px', 'aTargets': [_static.sel.avgCol] },
                            { 'bSortable': true, 'sWidth': '120px', 'aTargets': [_static.sel.sortableCols] },
                            { 'bSortable': false, "sSortDataType": "dom-text", 'sWidth': '140px', 'aTargets': [_static.sel.menuHeaderCols] },
                            { 'bSortable': true, "sType": "numeric", 'bVisible': false, 'aTargets': ['headersort'] }
                        ]
                    });

                    //Creates the left two fixed columns on out datatable, if the total number of columns is greater than 2
                    if (numberOfColumns > 2) {
                        new FixedColumns(_static.state.table, {
                            'iLeftColumns': 2,
                            'iLeftWidth': 350
                        });
                    }

                    _static.fn.createToolTips();
                    $(_static.sel.ttHeaderSort).click(_static.fn.onToolTipSort);
                    $(_static.sel.ttHeaderSummary + ', ' + _static.sel.ttHeaderAssignment).click(_static.fn.onToolTipClick);
                    $('.menutip a').click(_static.fn.onToolTipClick);
                    $('.' + _static.sel.tableToolsContainer).hide();
                    $('th#' + _static.sel.keyClose).click(function () {
                        $('#' + _static.sel.keyContainer).toggle();
                    });
                    $('th.sorting').click(_static.fn.onColumnSort);
                    _static.fn.assignClassAverage('Class average for all assignments: ', _static.sel.hdnClassAvg);

                    //Make sure radio button stay's in sync with displayed grid (now that we can move with url's)
                    $('#radio #assignedScoresTab').prop('checked', true);
                    $('#radio #assignedScoresTab').button('refresh');
                    gridDiv.show();
                } else {
                    PxPage.log('Error: Tried to initialize "Assigned Scores" DataTable but id: ' + _static.sel.assignedScoresContent + ' didnt exist');
                }
            },
            initOtherScores: function (tableId) {
                $('.gridButtons #' + _static.sel.key).hide();
                $(_static.sel.allAssignmentReportBtn).hide();

                var showAllAdditionalExportBtn = $.parseJSON($(_static.sel.showAllAdditionalExportBtn).val());
                if (showAllAdditionalExportBtn) {
                    $(_static.sel.allAdditionalItemReportBtn).show();
                }

                //Initialize jQuery UI radio buttons
                $('#groupby').buttonset();
                $('#groupby input').click(_static.fn.onOtherGroupingChange);

                //default to the item group
                var gridDiv = $('#' + tableId);
                if (gridDiv.length > 0) {
                    //Setup datatable
                    _static.state.table = gridDiv.dataTable({
                        'bSort': true,
                        'bAutoWidth': false,
                        'bFilter': false,
                        'bPaginate': false,
                        'bLengthChange': false,
                        'bDestroy': true,
                        'sDom': 'T<"clear">lfrtip',
                        'aoColumnDefs': [
                            { 'bSortable': true, 'sWidth': '350px', 'aTargets': [_static.sel.nameCol] },
                            { 'bSortable': true, 'sWidth': '120px', 'aTargets': [_static.sel.avgCol] },
                            { 'bSortable': true, 'sType': 'date', 'sWidth': '120px', 'aTargets': [_static.sel.submissionCol] }
                        ]
                    });

                    $('tbody tr').css('cursor', 'pointer');
                    _static.fn.assignClassAverage('Class average for below items: ', _static.sel.hdnClassAvg);

                    //UC says default sorting to last submission at top.
                    _static.state.table.fnSort([[2, 'desc']]);
                    $('#radio #otherScoresTab').prop('checked', true);
                    $('#radio #otherScoresTab').button('refresh');
                    gridDiv.show();
                } else {
                    PxPage.log('Error: Tried to initialize "Other Scores" DataTable but id: ' + tableId + ' didnt exist');
                }
            },
            initItemScores: function () {
                $('.gridButtons #' + _static.sel.key).hide();
                $(_static.sel.allAssignmentReportBtn).hide();

                //default to the item group
                var gridDiv = $('#' + _static.sel.itemScoresContent);
                if (gridDiv.length > 0) {
                    //Setup datatable
                    _static.state.table = gridDiv.dataTable({
                        'bSort': true,
                        'bAutoWidth': false,
                        'bFilter': false,
                        'bPaginate': false,
                        'bLengthChange': false,
                        'sDom': 'T<"clear">lfrtip',
                        'aoColumnDefs': [
                            { 'bSortable': true, 'sWidth': '350px', 'aTargets': [_static.sel.nameCol] },
                            { 'bSortable': true, 'sWidth': '120px', 'aTargets': [_static.sel.avgCol] },
                            { 'bSortable': true, 'sWidth': '120px', 'aTargets': [_static.sel.attemptsCol] }
                        ]
                    });

                    $('tbody tr').css('cursor', 'pointer').click(_static.fn.onItemRowClicked);
                    $('#radio #otherScoresTab').prop('checked', true);
                    $('#radio #otherScoresTab').button('refresh');
                    gridDiv.show();
                } else {
                    PxPage.log('Error: Tried to initialize "Item Scores" DataTable but id: ' + _static.sel.itemGroup + ' didnt exist');
                }
            },
            initStudentGradebook: function (divId, isDialog) {
                //Remove all event handelers associated with the student gradebook view
                $(_static.sel.studentGradeBook).find('input, tr, select').unbind('.studentdetail');

                //DataTables.fnDestroy doesn't remove all DataTable elements for whatever reason, including expanded rows.  If the expanded row is in there
                //when you go to create the table, it throws an error.  This removes all expanded rows from the table.
                $('tr td.info_row').parent().remove();
                var gridDiv = $(divId);
                if (gridDiv.length > 0) {
                    //Setup datatable
                    var table = gridDiv.find('table').dataTable({
                        'bSort': true,
                        'bAutoWidth': false,
                        'bFilter': false,
                        'bPaginate': false,
                        'sDefaultContent': '',
                        'bDestroy': true,
                        'bLengthChange': false,
                        'sDom': 'T<"clear">lfrtip',
                        'aoColumnDefs': [
                            { 'bSortable': true, 'sWidth': '350px', 'aTargets': [_static.sel.nameCol] },
                            { 'bSortable': true, 'sWidth': '120px', 'aTargets': [_static.sel.avgCol] },
                            { 'bSortable': true, 'sWidth': '120px', 'aTargets': [_static.sel.attemptsCol] }
                        ]
                    });

                    $(_static.sel.studentGradeBook + ' tbody tr').css('cursor', 'pointer').bind('click.studentdetail', _static.fn.onItemRowClicked);

                    //Check if the student sorder cookie is available. If so use it for sorting of the student table.
                    var studentSortOrder = [2, 'desc'];
                    var sortOrder = get_cookie(_static.constant.studentSortOrder);
                    if (sortOrder != undefined && sortOrder != null && sortOrder != "") {
                        studentSortOrder = sortOrder.split(',');
                    }

                    //We only need to setup the dialog specific radio buttons is being used in the dialog. We use the Gradebook view
                    //dialog button otherwise.
                    if (isDialog) {
                        $('#assignedGrouping').buttonset();
                        $('#assignedGrouping input').bind('click.studentdetail', (_static.fn.onStudentDetailGroupingChange));
                        _static.state.dialogTable = table;
                        _static.state.dialogTable.fnSort([studentSortOrder]);
                    } else {
                        _static.state.table = table;
                        _static.state.table.fnSort([studentSortOrder]);
                    }

                    $('#' + _static.sel.studentDetailFilter).bind('change.studentdetail', _static.fn.onFilterChange);
                    $(table).find('thead tr').live('click', _static.fn.onStudentTableHeaderClicked);

                    gridDiv.show();
                } else {
                    PxPage.log('Error: Tried to initialize "Student Detail" DataTable but id: ' + tableId + ' didnt exist');
                }
            },

            //Save the sort order in a session cookie
            onStudentTableHeaderClicked: function () {
                var dtTable = $(_static.sel.assigneditems).find('table').dataTable(); // this is not dataTable initialization, we are just getting the dataTable object
                set_cookie(_static.constant.studentSortOrder, dtTable.fnSettings().aaSorting[0], '0', '/'); // save the sort order in a cookie
            },

            initAssignmentSummary: function () {
                //default to the item group
                var gridDiv = $('#' + _static.sel.assignmentSummaryContent);
                if (gridDiv.length > 0) {
                    //Setup datatable
                    _static.state.dialogTable = gridDiv.dataTable({
                        'bSort': true,
                        'bAutoWidth': false,
                        'bFilter': false,
                        'bPaginate': false,
                        'bLengthChange': false,
                        'sDom': 'T<"clear">lfrtip',
                        'aoColumnDefs': [
                            { 'bSortable': true, 'sWidth': '350px', 'aTargets': [_static.sel.nameCol] },
                            { 'bSortable': true, 'sWidth': '120px', 'aTargets': [_static.sel.avgCol] },
                            { 'bSortable': true, 'sWidth': '120px', 'aTargets': [_static.sel.attemptsCol] }
                        ]
                    });

                    $('#' + _static.sel.assignmentSummaryContent + ' tbody tr').css('cursor', 'pointer').click(_static.fn.onItemRowClicked);
                    gridDiv.show();
                } else {
                    PxPage.log('Error: Tried to initialize "Assignment Summary" DataTable but id: ' + _static.sel.itemGroup + ' didnt exist');
                }
            },
            //Setup the divs for the header tooltips and the tooltips themselves
            createToolTips: function () {
                //Create tooltip divs.  Set the tooltip id to the col's index + the tooltip wrapper id
                $('.dataTables_scrollHead .' + _static.sel.menuHeaderCols).each(function () {
                    var colId = $(this).attr('id');
                    var headerToolTip = $('#' + colId + _static.sel.tooltipWrapper);
                    _static.state.cols[colId].colTipId = headerToolTip.attr('id');
                    headerToolTip.find(_static.sel.ttHeaderSummary).click(_static.fn.setHash);
                    headerToolTip.find(_static.sel.ttHeaderAssignment).click(_static.fn.setHash);
                });

                //Create the qtip objects
                $('.dataTables_scrollHead .' + _static.sel.menuHeaderCols).each(function () {
                    var id = $(this).attr('id');
                    $(this).qtip({
                        content: {
                            text: $('#' + _static.state.cols[id].colTipId)
                        },
                        position: {
                            my: 'bottom center',
                            at: 'top center'
                        },
                        show: {
                            event: 'click',
                            solo: true
                        },
                        hide: 'unfocus',
                        style: {
                            tip: true
                        }
                    });
                });
            },
            //Displays the view associated with the Assigned Scores radio button in instructor mode
            showAssignedScores: function () {
                if (_static.state.mode == _static.modes.student) {
                    //make sure the data has been loaded
                    if ($(_static.sel.assigneditems).length == 0) {
                        _static.fn.loadStudentGradebook(_static.sel.assigneditems, _static.sel.hdnAssignedAvg);
                    } else {
                        $(_static.sel.unassigneditems).hide();
                        _static.fn.assignClassAverage('Your average for all assignments: ', _static.sel.hdnAssignedAvg);
                        if (_static.state.table) _static.state.table.fnDestroy();
                        _static.fn.initStudentGradebook(_static.sel.assigneditems, false);
                    }
                } else {
                    PxPage.Loading(_static.sel.content);
                    $.get(
                        PxPage.Routes.view_gradebook_assignedscores,
                        null,
                        function (response) {
                            $('#' + _static.sel.content).html(response);
                            _static.fn.initAssignedScores();
                            PxPage.Loaded(_static.sel.content);
                        }
                    );
                }
            },
            //Displays the view associated with the Other Scores radio button in instructor mode
            showOtherScores: function () {
                if (_static.state.mode == _static.modes.student) {
                    //Make sure the data has been loaded
                    if ($(_static.sel.unassigneditems).length == 0) {
                        _static.fn.loadStudentGradebook(_static.sel.unassigneditems, _static.sel.hdnUnAssignedAvg);
                    } else {
                        $(_static.sel.assigneditems).hide();
                        _static.fn.assignClassAverage('Your average for all other items: ', _static.sel.hdnUnAssignedAvg);
                        if (_static.state.table) _static.state.table.fnDestroy();
                        _static.fn.initStudentGradebook(_static.sel.unassigneditems, false);
                    }
                } else {
                    PxPage.Loading(_static.sel.content);
                    $.get(
                        PxPage.Routes.view_gradebook_otherscores,
                        null,
                        function (response) {
                            $('#' + _static.sel.content).html(response);
                            //Default to showing grid grouped by items
                            $('#' + _static.sel.otherStudentGroup).hide();
                            _static.fn.initOtherScores(_static.sel.otherItemGroup);
                            PxPage.Loaded(_static.sel.content);
                        }
                    );
                }
            },
            //Displays the view associated with the Item Scores view in instructor mode
            showItemScores: function (itemid, itemType, username) {
                PxPage.Loading(_static.sel.content);
                $.get(
                    PxPage.Routes.view_gradebook_itemscores,
                    {
                        itemId: itemid,
                        itemType: itemType,
                        userName: username
                    },
                    function (response) {
                        $('#' + _static.sel.content).html(response);
                        _static.fn.initItemScores();
                        PxPage.Loaded(_static.sel.content);
                    }
                );
            },
            //Displays the student detail view associated with both student and instructor mode. 
            showStudentDetail: function (studentUserId, studentEnrollmentId) {
                PxPage.Loading(_static.sel.control);
                $.get(
                    PxPage.Routes.view_gradebook_studentdetail,
                    {
                        studentUserId: studentUserId,
                        studentEnrollmentId: studentEnrollmentId
                    },
                    function (response) {
                        var tab = $('#' + _static.sel.studentDetailUnassignedContent).length == 0 ? _static.sel.assigneditems : _static.sel.unassigneditems;
                        //The same view is used in both Instructor and student mode.  In instructor, its display in a dialog,
                        //For student, it is the main datatable.
                        $('<div class="dialogWrapper"/>').append(response).dialog({
                            width: 900,
                            height: 400,
                            dialogClass: "studentinfo",
                            minWidth: 900,
                            minHeight: 200,
                            modal: true,
                            draggable: false,
                            closeOnEscape: true,
                            open: function (event, ui) {
                                PxPage.Loaded(_static.sel.control);
                                _static.fn.initStudentGradebook(tab, true);
                                _static.fn.onDialogOpen();
                            },
                            close: function (event, ui) {
                                //Should we just do a history.back call here?
                                _static.fn.onDialogClose('state/gradebook/assignedScores');
                            }
                        });
                    }
                );
            },
            //Requests the student gradebook data assicated with student mode
            // tableSel - selector for the html table (assigned score or unassigned scores) to initialize
            // acerageSel - selector for the average to display in the header
            loadStudentGradebook: function (tableSel, averageSel) {
                PxPage.Loading(_static.sel.content);
                $.get(
                    PxPage.Routes.view_gradebook_student,
                    null,
                    function (response) {
                        $('#' + _static.sel.content).html(response);

                        //Hide buttons specific to instructor mode
                        $('.gridButtons #' + _static.sel.key).hide();
                        $('.gridButtons .' + _static.sel.exportgrid).hide();

                        //Hide both grids off the bat because we don't know which one we are initializing
                        $(_static.sel.assigneditems + ', ' + _static.sel.unassigneditems).hide();
                        _static.fn.initStudentGradebook(tableSel, false);
                        _static.fn.assignClassAverage('Your average for all assignments: ', averageSel);
                        PxPage.Loaded(_static.sel.content);
                    }
                );
            },
            //Displays the assignment summary dialog
            showAssignmentSummary: function (assignmentid) {
                PxPage.Loading(_static.sel.control);
                $.get(
                    PxPage.Routes.view_gradebook_assignmentsummary,
                    {
                        assignmentId: assignmentid
                    },
                    function (response) {
                        $('<div class="dialogWrapper"/>').append(response).dialog({
                            width: 650,
                            height: 400,
                            minWidth: 200,
                            dialogClass: "assignmentsummary",
                            minHeight: 200,
                            modal: true,
                            draggable: false,
                            closeOnEscape: true,
                            open: function (event, ui) {
                                PxPage.Loaded(_static.sel.control);
                                _static.fn.initAssignmentSummary();
                                _static.fn.onDialogOpen();
                            },
                            close: function (event, ui) {
                                _static.fn.onDialogClose('state/gradebook/assignedScores');
                            }
                        });
                    }
                );
            },
            //Handles the click on a sortable column thats not an assignment in assigned scores grid.
            onColumnSort: function (event, ui) {
                var colId = $(event.target).closest('th').attr('id');
                _static.state.sortedCol.colId = colId;
            },
            //Handle the tooltip "Sort" click
            onToolTipSort: function (event, ui) {
                var colId = $('#' + $(event.target).parent().attr('id').replace(_static.sel.tooltipWrapper, '')).attr('id');

                //Checks to see if the column has been consecutively sorted.  If not, default to descending ( per UC ).
                if (colId === _static.state.sortedCol.colId) {
                    if (_static.state.sortedCol.direction === 'descending') {
                        _static.state.table.fnSort([[_static.state.cols[colId].colIndex + 1, 'asc']]);
                        _static.state.sortedCol.direction = 'ascending';
                    } else {
                        _static.state.table.fnSort([[_static.state.cols[colId].colIndex + 1, 'desc']]);
                        _static.state.sortedCol.direction = 'descending';
                    }
                } else {
                    _static.state.sortedCol.colId = colId;
                    _static.state.sortedCol.direction = 'descending';
                    _static.state.table.fnSort([[_static.state.cols[colId].colIndex + 1, 'desc']]);
                }
            },
            onToolTipClick: function (event, ui) {
                $(event.target).closest('.qtip').qtip('hide');
            },
            //Handle the grouping change within the "Other Scores" view.
            onOtherGroupingChange: function (event, ui) {
                var btn = $(event.target);

                //Remove DataTable dom markup from currently selected grouping and add to new grouping table
                _static.state.table.hide();
                _static.state.table.fnDestroy();

                if (btn.attr('id') === _static.sel.otherGroupByItem) {
                    _static.fn.initOtherScores(_static.sel.otherItemGroup);
                } else if (btn.attr('id') === _static.sel.otherGroupByStudent) {
                    _static.fn.initOtherScores(_static.sel.otherStudentGroup);
                }
            },
            //Handle the grouping change within the "Student Details" view between Assigned Items and Other Scores
            onStudentDetailGroupingChange: function (event, ui) {
                var btn = $(event.target);

                _static.state.dialogTable.fnDestroy();

                if (btn.attr('id') === _static.sel.studentDetailsAssignedTab) {
                    $(_static.sel.studentUnAssignedExport).hide();
                    $(_static.sel.unassigneditems).hide();
                    _static.fn.initStudentGradebook(_static.sel.assigneditems, true);
                    $(_static.sel.studentAssignedExport).show();
                } else if (btn.attr('id') === _static.sel.studentDetailsOtherTab) {
                    $(_static.sel.studentAssignedExport).hide();
                    $(_static.sel.assigneditems).hide();
                    _static.fn.initStudentGradebook(_static.sel.unassigneditems, true);
                    $(_static.sel.studentUnAssignedExport).show();
                }
            },
            //Handles the click of a row on the "Item Scores" view, "Student Details" view and "Assignment Summary" view. Also the StudentGradebook in student mode
            onItemRowClicked: function (event, ui) {
                var row = this;
                //Check if this is being run on a modal dialog or not
                var table = $('.dialogWrapper').length > 0 ? _static.state.dialogTable : _static.state.table;
                if (table.fnIsOpen(row)) {
                    table.fnClose(row);
                    $(row).find(_static.sel.assignmentToggle).removeClass(_static.sel.toggleOpen).addClass(_static.sel.toggleClose);
                } else {
                    //Only open if we created a div tag for it
                    if ($('#' + $(row).attr('id') + 'rowdetails').length > 0) {
                        table.fnOpen(row, $('#' + $(row).attr('id') + 'rowdetails').clone().contents(), 'info_row');
                        $(row).find(_static.sel.assignmentToggle).removeClass(_static.sel.toggleClose).addClass(_static.sel.toggleOpen);
                    }
                }
            },
            //Handles the change of the filter drop down on the StudentGradebook view in both student and instructor mode.
            onFilterChange: function () {
                var filterid = $('#' + _static.sel.studentDetailFilter + ' option:selected').attr('id');
                if (filterid !== 'all') {
                    $('#' + _static.sel.studentDetailContent + ' tbody tr[folder!="' + filterid + '"]').hide();
                    $('#' + _static.sel.studentDetailContent + ' tbody tr[folder="' + filterid + '"]').show();
                } else {
                    $('#' + _static.sel.studentDetailContent + ' tbody tr').show();
                }
            },
            assignClassAverage: function (text, avgField) {
                $(_static.sel.classAvgText).text(text);
                var classAvg = $(avgField).val();
                $(_static.sel.classAvgPercent).text(classAvg);
            },
            onDialogOpen: function () {
            },
            onDialogClose: function (closeState) {
                HashHistory.SetHash(closeState);
            }
        }
    };

    return {
        events: {
            viewAssignment: _static.events.viewAssignment
        },
        InitRoutes: function () {
            PxRoutes.AddComponentRoute('gradebook', null, _static.fn.handleRoute);
        },
        InitInstructor: function () {
            _static.fn.init(_static.modes.instructor);
        },
        InitStudent: function () {
            _static.fn.init(_static.modes.student);
        },
        ShowAssignedScores: function () {
            if (_static.state.initialized) {
                _static.fn.showAssignedScores();
            } else {
                _static.loadQueue.push(_static.fn.showAssignedScores);
            }
        },
        ShowOtherScores: function () {
            if (_static.state.initialized) {
                _static.fn.showOtherScores();
            } else {
                _static.loadQueue.push(_static.fn.showOtherScores);
            }
        },
        ShowItemScores: function (itemid, itemType, username) {
            if (_static.state.initialized) {
                _static.fn.showItemScores(itemid, itemType, username);
            } else {
                _static.loadQueue.push(function () {
                    _static.fn.showItemScores(itemid, itemType, username);
                });
            }
        },
        ShowStudentDetail: function (studentUserId, studentEnrollmentId) {
            if (_static.state.initialized) {
                _static.fn.showStudentDetail(studentUserId, studentEnrollmentId);
            } else {
                _static.loadQueue.push(function () {
                    _static.fn.showStudentDetail(studentUserId, studentEnrollmentId);
                });
            }
        },
        ShowAssignmentSummary: function (assignmentid) {
            if (_static.state.initialized) {
                _static.fn.showAssignmentSummary(assignmentid);
            } else {
                _static.loadQueue.push(function () {
                    _static.fn.showAssignmentSummary(assignmentid);
                });
            }
        },
        CloseGradebookDialog: function () {
            if ($('.dialogWrapper').length > 0) {
                _static.state.dialogTable.fnDestroy();
                $('.dialogWrapper').dialog('destroy');
                $('.dialogWrapper').remove();
            }
        },
        Reload: function () {
            //Check to see which view is current being used and refresh
            if (_static.state.table !== '') {
                switch ($(_static.state.table[0]).attr('id')) {
                    case _static.sel.assignedScoresContent:
                        _static.fn.showAssignedScores();
                        break;
                    case _static.sel.otherScoresContent:
                        _static.fn.showOtherScores();
                        break;
                    case _static.sel.itemScoresContent:
                        _static.fn.showItemScores();
                        break;
                    default:
                        PxPage.log('Cannot refresh table ' + $(_static.state.table[0]).attr('id'));
                        break;
                }
            }
        }
    };
} (jQuery);