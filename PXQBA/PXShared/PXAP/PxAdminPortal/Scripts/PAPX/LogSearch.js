
function LogSearchLoad() {

    $('.txtDate').datepicker({ dateFormat: "mm/dd/yy" });

    // Handle btnSaveSearch
    $("#btnSearch").live("click", function () {

        var form = $(this).closest("form");

        if (form.valid()) {
            BindLogSearchGrid();
        }
    });

    //End Date should be greater than Start Date   
    $.validator.addMethod("startDate", function (value, element) {
        var endDate = $('.endDate').val();

        if (value != '' && endDate != '') {
            return Date.parse(endDate) >= Date.parse(value);
        }
        else {
            return true;
        }
    }, "* End date must be after start date");

    $.validator.addMethod("endDate", function (value, element) {
        var startDate = $('.startDate').val();

        if (value != '' && startDate != '') {
            return Date.parse(startDate) <= Date.parse(value);
        }
        else {
            return true;
        }
    }, "* End date must be after start date");

    //if the request is from the dashboard, load the Log Search grid by default
    var vSev = $("#Severity").val();

    if (vSev.toUpperCase() != "ALL") {
        BindLogSearchGrid();
    }

}


function GetCurrentSourceList(ddList) {
    var sources = "'";
    var selector = ddList + " option";
    var i = 0;
    var sourceList = new Array();

    $(selector).each(function () {
        if ($(this).val() != "All") {
            sourceList[i++] = $(this).val();
        }
    });

    sources = "'" + sourceList.join("','") + "'";

    return sources;
}


function BindLogSearchGrid() {

    var vSeverity = $("#Severity").val();
    var vCategory = $("#Category").val();
    var vStartDate = $("#StartDate").val();
    var vEndDate = $("#EndDate").val();
    var vMessage = $("#Message").val();

    var vSource = $("#Source").val();
    if (vSource == 'All') {
        vSource = GetCurrentSourceList("#Source");
    }
    else {
        vSource = "'" + vSource + "'";
    }

    $('#list').jqGrid('GridUnload');

    $("#list").jqGrid({
        url: window.PXAPRoutes.LogSearchGrid,
        datatype: 'json',
        mtype: 'POST',
        postData: { severity: vSeverity, category: vCategory, startDate: vStartDate, endDate: vEndDate, source: vSource, message: vMessage },
        colNames: ['Id', 'Severity', 'Source', 'Time', 'Category', 'Message', 'Actions'],
        colModel: [
                          { name: 'Id', index: 'Id', width: 100, align: 'left' },
                          { name: 'Severity', index: 'Severity', width: 100, align: 'left' },
                          { name: 'Source', index: 'Source', width: 150, align: 'left' },
                          { name: 'Time', index: 'Time', width: 150, align: 'left' },
                          { name: 'Category', index: 'Category', width: 100, align: 'left' },
                          { name: 'Message', index: 'Message', width: 250, align: 'left' },
                          { name: 'Actions', width: 100, align: 'left' },
                          ],
        pager: jQuery('#pager'),
        rowNum: 20,
        rowList: [20, 40, 50],
        height: 480,
        sortname: 'Time',
        sortorder: "desc",
        viewrecords: true,
        caption: 'Log Search',
        thousandsSeparator: ',',
        jsonReader: {
            root: 'Rows',
            page: 'Page',
            total: 'Total',
            records: 'Records',
            repeatitems: false,
            userdata: 'UserData',
            id: 'Id'
        }
    });
}

function showMessageDdetails(LogID) {
    // call the server to get error message
    $.get(window.PXAPRoutes.LogSearchGetErrorMessage, { logID: LogID }, function (data) {

        $("#tabs").find("#Message").html(data.ErrorMessage);

        $("#tabs").tabs().show();
        $("#tabs").tabs("select", 0);

        $.blockUI();

        $("#dialog").dialog({ width: 650, height: 600, minWidth: 650, minHeight: 600,
            close: function (event, ui) { $.unblockUI(); }
        });

    });
}
