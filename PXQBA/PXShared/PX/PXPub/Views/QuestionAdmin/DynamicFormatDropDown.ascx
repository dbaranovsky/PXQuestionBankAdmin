<%@ Control Language="C#"   %>
<script type="text/javascript" >

	var DynamicFormatDropDown = {

		initDropDownResponseFormat: function () {
			//alert('initDropDownResponseFormat');

			$(document).on("click", function (e) {
				var $clicked = $(e.target);
				if (!$clicked.parents().hasClass("cssResponseFormat"))
					$(".cssResponseFormat dd ul").hide();
			});

			$(document).off('click',".cssResponseFormat dd ul li a").on('click', function () {
				var text = $(this).html();
				$(".cssResponseFormat dt a").html(text);
				$(".cssResponseFormat dd ul").hide();

				var source = $("#FormatList");
				source.val($(this).find("span.value").html());
				$("#cmdSearch").click();
			});


			$(document).off('click',".cssResponseFormat dt a").on('click', ".cssResponseFormat dt a", function () {
				$(".cssResponseFormat dd ul").toggle();
			});

		},

		createDropDownResponseFormat: function () {

			//alert('createDropDownResponseFormat');

			var source = $("#FormatList");
			var selected; //= source.find("option[selected]");
			var options = $("option", source);

			$("#tdResponseFormat").append("<dl id='targetResponseFormat' class='cssResponseFormat'></dl>");


			if (selected!=undefined && selected.val() != undefined) {
				$("#targetResponseFormat").append('<dt><a href="#">' + selected.text() + '<span class="value">' + selected.val() + '</span></a></dt>');
			} else {
				$("#targetResponseFormat").append("<dt><a href='#'>Type <span class='value'>1</span></a></dt>");
			}

			$("#targetResponseFormat").append('<dd><ul></ul></dd>');

			options.each(function () {
				if ($(this).val() != 0) {
					$("#targetResponseFormat dd ul").append("<li><a href='#'>" + $(this).text() + "<span class='value'>" + $(this).val() + "</span></a></li>");
				}
			});
		}
	};
</script>
	