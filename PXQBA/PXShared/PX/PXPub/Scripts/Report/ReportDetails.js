var PxReportDetails = function ($) {
    var _static = {
        pluginName: "ReportDetails",
        dataKey: "ReportDetails",
        bindSuffix: ".ReportDetails",
        dataAttrPrefix: "data-rd-",
        defaults: {},
        settings: {},
        modes: {},
        // commonly used CSS classes
        css: {},
        // commonly used jQuery selectors
        sel: {
            reportContent: '.detail.reports',
            reportTitle: '.rpt-title',
            //rubric Performance
            optCriteria: '#optCriteria',
            optType: '#optType',
            optPerformance: '#optPerformance',
            chkPerformance: '.chkPerformance',
            rptResult: '#rpt-perf-results',
            rubric_performance_report_section: '.rubric-performance-report',
            //
            //LO Performance
            optType2: '#optType2',
            optPerformance2: '#optPerformance2',
            chkPerformance2: '.chkLOPerformance',
            rptResult2: '#rpt-perf-results2',
            lo_performance_report_section: '.lo-performance-report',
            //           
            optStudent: '.optStudent',
            selectedFolder: '.rpt-folder-item.selected',
            chkFilter: '.chkFilter',
            performance: '#performance',
            detailData: '.detail',
            reporttitle: '.assessment-container span',
            printMenu: '.rpt-detail-menu',
            printContainer: '.printable',
            performanceTable: '#rpt-perf-table',
            performanceTable2: '#rpt-perf-table2'
        },
        fn: {
            criteriaChecked: function () {
                if (this.checked) {
                    var folderId = $(_static.sel.selectedFolder).attr('id');
                    _static.fn.loadPerformance(folderId);
                }
                else {
                    $(_static.sel.optType)[0].selectedIndex = 0;
                    $(_static.sel.optPerformance)[0].selectedIndex = 0;
                    $(_static.sel.optCriteria)[0].selectedIndex = 0;
                    $(_static.sel.performanceTable).empty();
                }
            },
            criteriaChecked2: function () {
                if (this.checked) {
                    var folderId = $(_static.sel.selectedFolder).attr('id');
                    _static.fn.loadPerformance2(folderId);
                }
                else {
                    $(_static.sel.optType2)[0].selectedIndex = 0;
                    $(_static.sel.optPerformance2)[0].selectedIndex = 0;
                    $(_static.sel.performanceTable2).empty();
                }
            },
            YellowFade: function (selector) {
                $(selector)
                  .css('opacity', 0)
                  .animate({ backgroundColor: '#DAA520', opacity: 1.0 }, 1200)
                  .animate({ backgroundColor: '#ffffff' }, 350, function () {
                      $(this).removeAttr('filter');
                  });
            },
            loadPerformance: function (folderId, objReport) {
                var type = $(_static.sel.optType).val(),
                performance = $(_static.sel.optPerformance).val(),
                criteria = $(_static.sel.optCriteria).val(),
                checkedCriteria = $(_static.sel.chkPerformance).is(':checked'),
                checkedFilter = $(_static.sel.chkFilter).is(':checked');

                if (checkedCriteria && criteria != "default") {
                    PxPage.Loading("rubric-performance-report", true);

                    //ajax call to performance data
                    var bodyPostContent = $.ajax({
                        type: 'POST',
                        url: PxPage.Routes.rpt_performance_view,
                        data: { "Id": folderId, "sType": type, "sPerformance": performance, "sCriteria": criteria, "oReport": objReport, "bFilterChecked": checkedFilter },
                        success: function (response) {
                            PxPage.Loaded("rubric-performance-report", true);
                            var responseObj = $(response);
                            var selector = _static.sel.rubric_performance_report_section;
                            if (responseObj.closest(selector).length > 0) {
                                var replacementData = $(selector).html(responseObj.closest(selector));
                                _static.fn.YellowFade(_static.sel.performanceTable);
                            }
                        },
                        error: function (req, status, error) {
                            PxPage.Loaded("rubric-performance-report", true);
                            PxPage.log('Error displaying performance detail');
                        }
                    });
                }
            },
            loadPerformance2: function (folderId, objReport) {
                var type = $(_static.sel.optType2).val(),
                performance = $(_static.sel.optPerformance2).val(),
                checkedCriteria = $(_static.sel.chkPerformance2).is(':checked'),
                checkedFilter = $(_static.sel.chkFilter).is(':checked');

                if (checkedCriteria) {
                    PxPage.Loading("lo-performance-report ", true);

                    //ajax call to performance data
                    var bodyPostContent = $.ajax({
                        type: 'POST',
                        url: PxPage.Routes.rpt_performance_view2,
                        data: { "Id": folderId, "sType": type, "sPerformance": performance, "oReport": objReport, "bFilterChecked": checkedFilter },
                        success: function (response) {
                            PxPage.Loaded("lo-performance-report ", true);
                            var responseObj = $(response);
                            var selector = _static.sel.lo_performance_report_section;
                            if (responseObj.closest(selector).length > 0) {
                                var replacementData = $(selector).html(responseObj.closest(selector));
                                _static.fn.YellowFade(_static.sel.performanceTable2);
                            }
                        },
                        error: function (req, status, error) {
                            PxPage.Loaded("lo-performance-report", true);
                            PxPage.log('Error displaying performance detail');
                        }
                    });
                }
            },
            selectStudent: function (withOption) {
                _static.fn.reload(false, false);
            },

            reload: function (filterCheck, print) {
                var studentId = $(_static.sel.optStudent).val(),
                    student = $(_static.sel.optStudent).find('option:selected').text(),
                    folderId = $(_static.sel.selectedFolder).attr('id'),
                    folder = $(_static.sel.selectedFolder).text(),
                    filterChecked = $(_static.sel.chkFilter).is(':checked'),
                    reportTitle = $(_static.sel.reporttitle).html();

                if (filterCheck) { studentId = ""; student = ""; }

                PxPage.Loading();

                //ajax call to performance data
                var bodyPostContent = $.ajax({
                    type: 'POST',
                    url: PxPage.Routes.rpt_detail_view,
                    data: { "folderItemId": folderId, "Item": folder, "StudentId": studentId, "bFilterChecked": filterChecked, "Title": reportTitle, "student": student, "print": print },
                    success: function (response) {
                        if (print) {
                            $(_static.sel.printContainer).html(response);
                            $(_static.sel.printContainer).print();
                            PxPage.Loaded();
                            //return false;
                        }
                        else {
                            $(_static.sel.detailData).html(response);
                            PxPage.Loaded();
                        }

                    },
                    error: function (req, status, error) {
                        PxPage.Loaded();
                        alert('Error displaying performance detail');
                    }
                });
            },
            filterChecked: function (withOption) {
                _static.fn.reload(true, false);
            },
            printClicked: function (reportName) {
                var reportTitle = $(_static.sel.reportTitle).html().trim() + " Performance Report";
                $(_static.sel.reportContent).printElement({ printMode: 'popup',
                    pageTitle: reportTitle,
                    overrideElementCSS: [PxPage.CssRoutes.gradeReports_css]
                });
            }
        }
    };

    return {
        init: function () {
            $(_static.sel.chkPerformance).die();
            $(_static.sel.chkPerformance2).die();
            $(_static.sel.chkFilter).die();
            $(_static.sel.printMenu).die();

            $(_static.sel.chkPerformance).live('click', _static.fn.criteriaChecked);
            $(_static.sel.chkPerformance2).live('click', _static.fn.criteriaChecked2);
            $(_static.sel.chkFilter).live('click', _static.fn.filterChecked);
            $(_static.sel.printMenu).live('click', _static.fn.printClicked);
        },
        selectStudent: function (option) {
            _static.fn.selectStudent(option);
        },
        criteriaChecked: function () {
            _static.fn.criteriaChecked();
        },
        criteriaChecked2: function () {
            _static.fn.criteriaChecked2();
        },
        loadPerformance: function (rubricId, RptObject) {
            _static.fn.loadPerformance(rubricId, RptObject);
        },
        loadPerformance2: function (rubricId, RptObject) {
            _static.fn.loadPerformance2(rubricId, RptObject);
        }
    }
} (jQuery);