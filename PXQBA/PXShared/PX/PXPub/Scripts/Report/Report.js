var PxReport = function($) {
    return {
        Init: function() {
            PxReport.BindControls();
            PxReport.SetTitle();
        },

        BindControls: function() {
            $('#btnRun').bind('click', PxReport.GetControlValues);
            $('.report-menu a').bind('click', PxReport.GetReportName);
        },

        GetControlValues: function() {
            var strVariables = '';
            var fromDate = $('#startDate').val();
            var toDate = $('#endDate').val();
            var studentId = $('#selStudent option:selected').val();
            var groupId = $('#selGroup option:selected').val();
            var reportName = $('.report-menu a.selected').attr("id");
            strVariables = "?ReportName=" + reportName + "&FromDate=" + fromDate + "&ToDate=" + toDate + "&StudentId=" + studentId + "&GroupId=" + groupId;
            PxReport.LoadFrame(strVariables);
            PxReport.DateValidation();
        },

        LoadFrame: function(strVariables) {
            var newurl = '';
            var url = $('#iFrameReport').attr("src").split("?");
            if (url[0].length > 1) {
                newurl = url[0] += strVariables;
            }
            $('#iFrameReport').show().attr("src", newurl);
        },

        GetReportName: function(event) {
            $('.report-menu a').removeClass('selected');
            $(this).addClass('selected');
            var strTitle = $('.report-menu a.selected').attr("title");
            $("#report-title").text(strTitle);
        },

        SetTitle: function() {
            $("#startDate").datepicker();
            $("#endDate").datepicker();
        },

        DateValidation: function() {
            var startdatevalue = $('#startDate').val();
            var enddatevalue = $('#endDate').val();
            if (startdatevalue.length == 0) {
                alert('Must select a From Date');
                return true;
            }
            if (enddatevalue.length == 0) {
                alert('Must select a End Date');
                return true;
            }
        }
    }
} (jQuery);




