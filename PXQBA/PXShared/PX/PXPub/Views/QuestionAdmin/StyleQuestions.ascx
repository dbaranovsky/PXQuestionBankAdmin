<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%
    var url = Url.ActionCache("getStyleQuestions", "QuestionAdmin");   
    %>
<script type="text/javascript">

    function getStyleQuestion(id) {

        if (id == 30) return;

        $.ajax({
            type: 'POST',
            url: '<%=url%>',
            data: { id: id },

            traditional: true,
            success: function (data, textStatus, jqXHR) {

                $('#questions').append(data);
                id = id + 1;
                getStyleQuestion(id);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('error ' + textStatus + ': ' + errorThrown);
                PxPage.Loaded();
            }
        });
    }

    getStyleQuestion(0);

</script>

<div id="questions">


</div>