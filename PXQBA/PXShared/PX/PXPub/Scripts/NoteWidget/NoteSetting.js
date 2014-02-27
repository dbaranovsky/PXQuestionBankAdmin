$(function() {
    $("#searchNote").autocomplete({
        source: function(request, response) {
            $.ajax({
                url: PxPage.Routes.get_searchInfo, type: "POST", dataType: "json",
                data: { contactName: request.term, maxresults: 10 },
                success: function(data) {
                    response($.map(data, function(item) {
                        return {
                            value: item.FirstName + " " + item.LastName,
                            id: item.Id
                        }
                    }))
                },
                error: function(XMLHttpRequest, textStatus, errorThrown) {
                    PxPage.Toasts.Error(textStatus);
                }
            });
        },
        select: function(event, ui) {
            if (ui.item) {
                $('#studentId').val(ui.item.id);
                shareWithStudent(ui.item.value);
            }
        },
        minLength: 2,
        mustMatch: true
    });

$("#document-viewer").attr("scrollTop", 0);

$('.remove').die().live('click', function(e) {
    e.stopPropagation();
    PxPage.Loading('noteSetting-Container');
    $(this).parent().removeClass("selectStudent");
    var studentFirstName = $(this).parent().find("input[name='SharedStudentFirstName']").val();
    var studentName = $(this).parent().find("input[name='SharedStudentName']").val();

    var args = {
        stopSharingToId: $(this).parent().find("input[name='SharedStudentId']").val()
    };
    $.post(PxPage.Routes.stop_sharing, args, function(response) {
        $('#shareListContainer').html(response);
        showRemovedMessage(studentName, studentFirstName);
        clearSearchBox();
        PxPage.Update();
        PxPage.Loaded('noteSetting-Container');
    });

    e.stopPropagation();
});

$('#fne-unblock-action').unbind('click', hideNoteSettings).bind('click', hideNoteSettings);

});

$('#retNotes').die().live('click', function(e) {
    $('#noteSetting-Container').remove();
    PxComments.Init();
    e.stopPropagation();
});

function addStudent(studentName) {
    $('#shareListContainer > .shareList').append(function() {
        var contents = "<div class='shared-student'>";
        contents += "<span class='shared-student-name'>" + studentName + "</span>";
        contents += "<span class='remove'></span>";
        contents += "</div>"
        return contents;
    });
}

function shareWithStudent(name) {
    if (confirm("Are you sure you want to share your private notes with " + name + "?")) {
        PxPage.Loading('noteSetting-Container');
        var args = {
            studentFullName: name
        };
        $.post(PxPage.Routes.share_notes, args, function(response) {
            $('#shareListContainer').html(response);
            showAddMessage(name);
            clearSearchBox();
            PxPage.Update();
            PxPage.Loaded('noteSetting-Container');
        });
    }
}

function showAddMessage(studentName) {
    $('#shareListContainer > .added-info-message').html(function() {
        var contents = "<p>You have added " + studentName + " to your share list. ";
        contents += studentName + " may see your private notes. </p>";
        return contents;
    });
}

function showRemovedMessage(studentName, studentFirstName) {
    $('#shareListContainer > .added-info-message').html(function() {
        var contents = "<p>You have removed " + studentName + " from your share list. ";
        contents += studentFirstName + " will not be able to see your private notes. </p>";
        return contents;
    });
}

function clearSearchBox() {
    $('.searchContainer').find("input[name='searchNote']").val('');
}

function hideNoteSettings() {
    $('#highlightList').show();
    $('#noteSetting-Container').remove();
}
 