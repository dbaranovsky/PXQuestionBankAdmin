var PxReportViewer = function ($) {
    var _static = {
        pluginName: "ReportViewer",
        dataKey: "ReportViewer",
        bindSuffix: ".ReportViewer",
        dataAttrPrefix: "data-rv-",
        defaults: {},
        settings: {},
        modes: {},
        // commonly used CSS classes
        css: {},
        // commonly used jQuery selectors
        sel: {
            folder_item: '#report-tree ul li',
            highlightedfolders: '.rpt-folder-item.selected',
            detail: '.detail',
            reporttitle: '.assessment-container span'
        },
        fn: {
            folderClicked: function () {
                var selection = $(this),
                        ItemId = selection.attr('id'),
                        Item = selection.text(),
                        highlightedFolders = $(_static.sel.highlightedfolders),
                        reportTitle=$(_static.sel.reporttitle).html();

                highlightedFolders.removeClass("selected");

                selection.addClass('selected');

                PxPage.Loading("body");

                //ajax call to get the Report details
                var bodyPostContent = $.ajax({
                    type: 'POST',
                    url: PxPage.Routes.rpt_detail_view,
                    data: { "folderItemId": ItemId, "Item": Item, Title: reportTitle, print:false },
                    success: function (response) {
                        $(_static.sel.detail).html(response);
                        PxPage.Loaded("body");
                        PxEportfolioManagement.Active('ReportsItem');
                        PxEportfolioManagement.StartFading();
                    },
                    error: function (req, status, error) {
                        PxPage.Loaded("body");
                        alert('Error displaying report detail');
                    }
                });
            }
        }
    };

    return {
        init: function () {
            $('[title]').removeAttr('title');
            $(_static.sel.folder_item).bind('click', _static.fn.folderClicked);
        }
    }
} (jQuery);