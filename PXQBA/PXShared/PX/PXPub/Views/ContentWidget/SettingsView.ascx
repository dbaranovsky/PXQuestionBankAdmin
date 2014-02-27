<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>




<style type="text/css">
    dd {
        margin-left: 20px;
    }
    #toggle_main {
        cursor: pointer;
        float: right;
    }
    #toggle_date {
        cursor: pointer;
        float: right;
    }
    .toggle_container {
        border-bottom: 1px #F1F2F3 solid;
        padding-top: 10px;
        
    }
    #date_picker {
        display: none;
    }

</style>
<script type="text/javascript">
    $(document).ready(function () {
        var hfs = $('#HiddenFromStudent').is(':checked');
        if (!hfs) {
            $('#byDate').hide();
        }
        var hasdate = $('#HideByDate').is(':checked');
        if (hasdate) {
            $('#date_picker').show();
            var dt = '<%= Model.AvailableDate %>';
            var justDate = dt.split(' ')[0];
            var rawTime = dt.split(' ')[1];
            var justHour = rawTime.split(':')[0];
            var justMinute = rawTime.split(':')[1];
            var justMaridian = dt.split(' ')[2];
            $('#date').val(justDate);
            $('#dueHour').val(justHour);
            $('#dueMinute').val(justMinute);
            $('#dueAmpm').val(justMaridian);

        }
    });
    function SaveSettings() {
        var hft = $('#HiddenFromToc').is(':checked');
        var hfs = $('#HiddenFromStudent').is(':checked');
        var hbd = $('#HideByDate').is(':checked');
        var d = $('#date').val();
        var t = $('#time').val();
        var id = $('#itemId').val();
        $.ajax({
            type: 'POST',
            url: PxPage.Routes.settings_view_save_settings,
            data: { itemId: id, visibleInToc: hft, visibleToStudent: hfs, ByDate: hbd, date: d, time: t },
            success: function (response) {
                $('#response_text').html('<span style="color:red">' + response.reason + '</span>');
                // changing link color in toc and adding tool tip to show that this item is hidden from students
                if (response.hft == true) {
                    $('span:contains("' + response.title + '")').closest("li").remove();
                    return;
                }

                if (response.hfs == true) {
                    $('span:contains("' + response.title + '")').css("color", "#CFCFCF").parent().attr("title", "HIDDEN FROM STUDENTS");
                } else {
                    $('span:contains("' + response.title + '")').css("color", "#00008B").parent().attr("title", "");
                }

            },
            error: function (req, status, error) {
                alert('ERROR_UPDATE_RESOURCE_LIST');
            }
        });
    }
    
    function toggleStudent() {
        var hft = $('#HiddenFromToc').is(':checked');
        if (hft) {
            $('#HiddenFromStudent').prop("checked", true);
            $('#byDate').toggle();
        } else {
            $('#HiddenFromStudent').prop("checked", false);
            $('#byDate').toggle();
            $('#HideByDate').prop("checked", false);
            $('#date').val('');
            $('#time').val('');
            $('#date_picker').toggle();
        }
    }
    
    function showByDate() {
        $('#date_picker').toggle();
    }
</script>
<div class="toggle_container"></div>
<% 
    if (Model != null)
   {%>
       
<dl>
    <dt>
        <% if (Model.Hidden) {%>
        <input type="checkbox" name="HiddenFromToc" id="HiddenFromToc"  value="<%= Model.Hidden %>" onclick="toggleStudent();" />
        <% } else
           { %>
           <input type="checkbox" name="HiddenFromToc" id="HiddenFromToc" checked="checked" value="<%= Model.Hidden %>" onclick="toggleStudent();" />
        <% } %>
        <label>Visible in table of contents</label>
    </dt>
    <dt>
        <% 
            if (Model.HiddenFromStudents) {%>
        <input type="checkbox" name="HiddenFromStudent" id="HiddenFromStudent" value="<%= Model.HiddenFromStudents %>" onclick="javascript:$('#byDate').toggle();" />
        <% } else
           { %>
           <input type="checkbox" name="HiddenFromStudent" id="HiddenFromStudent" checked="checked" value="<%= Model.HiddenFromStudents %>" onclick="javascript:$('#byDate').toggle();" />
           <% } %>
        <label>Visible to students</label>
    </dt>
    <dd id="byDate">
        <% if (Model.AvailableDate.Year != 1) { %>
            <input type="checkbox" name="HideByDate" id="HideByDate" checked="checked" onclick="showByDate();" />
        <% } else
           { %>
            <input type="checkbox" name="HideByDate" id="HideByDate" onclick="showByDate();" />
        <% } %>
        <label>Restrict visibility by date</label>
    </dd>
    <dd>
        
        <div id="date_picker">
            <%
                Html.RenderPartial("DateTimePicker",
                                   new DateTimePicker
                                       {Mode = "single", Calendars = 1, Format = "m-d-Y", Position = "bottom"});%>
        </div>
    </dd>    
</dl>
<input type="button" value="Save" onclick="SaveSettings()"/>
<div class="toggle_container"></div>
<div id="response_text"></div>
 <% }
   %>