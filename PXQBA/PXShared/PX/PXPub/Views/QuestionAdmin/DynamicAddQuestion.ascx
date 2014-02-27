<%@ Control Language="C#"   %>
<script type="text/javascript" >

    var DynamicAddQuestionDropDown = {

        initDropDownAddQuestion: function () {

            $(document).on("click", function (e) {
                var $clicked = $(e.target);
                if (!$clicked.parents().hasClass("cssAddQuestion"))
                    $(".cssAddQuestion dd ul").hide();
            });

            $(document).off('click', ".cssAddQuestion dd ul li a").on('click', ".cssAddQuestion dd ul li a", function () {
                //var text = $(this).html();
                var text = '<button>Add Question</button>';
                $(".cssAddQuestion dt a").html(text);
                $(".cssAddQuestion dd ul").hide();

                //var source = $("#FormatList");
                //source.val($(this).find("span.value").html());
            });


            $(document).off('click', ".cssAddQuestion dt a").on('click', ".cssAddQuestion dt a", function () {
                $(".cssAddQuestion dd ul").toggle();
            });

        },

        createDropDownAddQuestion: function (newQuestionUrl) {
            //alert('createDropDownAddQuestion');
            $("#tdAddQuestion").empty();
            var source = $("#FormatList");
            var selected; // = source.find("option[selected]");

            //$('input:checkbox')
            $("input[name='FormatSelectedValues']").each(function(){
                var $label = $(this).next('span').text();
                if (this.value == "HTS"){// || this.value == "FMA_GRAPH")
                    source += "<option value=\"" + "custom" + "\">" + $label + "</option>"
                }
                else if (this.value == "FMA_GRAPH") {
                    source += "<option value=\"" + "graph" + "\">" + $label + "</option>"
                }
                else
                    source += "<option value=\"" + this.value + "\">" + $label + "</option>"
            }
                );

            //var options = $("option", source);
            var options = $(source);

            $("#tdAddQuestion").append("<dl id='targetAddQuestion' class='cssAddQuestion'></dl>");

            if (selected != undefined && selected.val() != undefined) {
                $("#targetAddQuestion").append('<dt><a href="#">' + selected.text() + '<span class="value">' + selected.val() + '</span></a></dt>');
            } else {
                $("#targetAddQuestion").append("<dt><a href='#'><button>Add Question</button><span class='value'>1</span></a></dt>");

            }
            $("#targetAddQuestion").append('<dd><ul></ul></dd>');

            options.each(function () {
                if ($(this).val() != 0) {

                    //var url = newQuestionUrl + "/" + $(this).val();
                    //alert("this is:" + url);
                    var li = "<li type=" + $(this).val() + "><a href='#'>" + $(this).text() + "<span class='value'>" + $(this).val() + "</span></a></li>";

                    $("#targetAddQuestion dd ul").append(li);
                    //$("#targetAddQuestion dd ul").find("li").bind("click", PxQuestionAdmin.LaunchQuizSelector);

                }
                $("#targetAddQuestion dd ul").find("li").bind("click", { url: newQuestionUrl }, PxQuestionAdmin.LaunchQuizSelector);
            });
        }
    };
</script>