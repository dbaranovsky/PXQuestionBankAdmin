var PxAccountWidget = function ($) {
    return {
        Init: function () {
            PxAccountWidget.InitAccountList();
            PxAccountWidget.BindControls();
        },

        BindControls: function () {
            $('#accountActionsList').bind('change', PxAccountWidget.OnAccountListChange);
        },

        InitAccountList: function () {
            $('#accountActionsList option').each(function () {
                if ($(this).val() == 'disabled') {
                    $(this).attr('disabled', 'disabled');
                }
                else if ($(this).text().length > 26) {
                    var text = $(this).text();
                    $(this).attr('title', text);
                    text = text.substr(0, 26) + "...";
                    $(this).text(text);
                }
            });
        },

        OnAccountListChange: function (event) {
            var value = $(this).val();
            var selectedIndex = $("#accountActionsList option:selected").index();
            var selectedText = $("#accountActionsList option:selected").text();

            if (selectedText == 'Join a different course') {
                PxPage.OpenEnrollmentSwitchPopup(null, value);
                $('#accountActionsList option:first-child').attr("selected", "selected");
            }
            else {
                if (value == "CreateCourseDashboard" && $("#createCourseLink").length > 0) {
                    $(PxPage.switchboard).trigger("PxDashboard.CreationDialog");
                } else if (value == "CreateCourse" && $("#createCourseLink").length > 0) {
                    if ($("#createCourseLink").hasClass("fne-link")) {
                        $("#createCourseLink").click();
                        $('#accountActionsList').prop('selectedIndex', 0);
                    } else {
                        window.location = $("#createCourseLink").attr("href");
                    }
                }
                else if (value == "CourseList" && $("#courseListLink").length > 0) {
                    if ($("#courseListLink").hasClass("fne-link")) {
                        $("#courseListLink").click();
                        $('#accountActionsList').prop('selectedIndex', 0);
                    }
                    else {
                        window.location = $("#courseListLink").attr("href");
                    }
                }
                else if (value == 'profile') {
                    PxPage.TriggerClick($("#marsProfileLink"));
                    $("#accountActionsList").val('user');
                }
                else if (value == 'switchenrollment') {
                    window.location = location.protocol + '//' + location.host + PxPage.Routes.ECommerceEnroll;
                }
                else if (value != 'user') {
                    window.location = value;
                }
            }
        }
    }
} (jQuery);