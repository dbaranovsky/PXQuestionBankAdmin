<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
            <script type="text/javascript">
                (function ($) {
                    $(function () {
                        $('#date').focus(function () {
                            PxPage.ShowDatePicker({
                                calendarMode: "single",
                                sender: "DateTimePicker",
                                callback: function (d) {
                                    var date = d.StartDate ? (new Date(d.StartDate)).format("mm/dd/yyyy") : "01/01/0001";
                                    $('#date').val(date);
                                }
                            });
                        });

                    });
                } (jQuery));

                function setTime() {
                    var h = $('#dueHour').val();
                    var m = $('#dueMinute').val();
                    var ma = $('#dueAmpm').val();
                    $('#time').val(h + ":" + m + " " + ma);
                }
            </script>
<label>Date</label><input id="date" name="date" type="text"/>&nbsp;<label>Time</label>
<input id="time" type="hidden"/>

                    <select onchange="setTime();" id="dueHour" name="dueHour">
                        <%
                            var t = Model;
                            for ( int i = 1; i <= 12; i++ )
                           {%>
                                <option value="<%= i %>"><%= i %></option>
                        <% } %>
                    </select>
                    <span class="input-label">:</span>
                    <select onchange="setTime();" id="dueMinute" name="dueMinute">
                        <% 
                            for ( int i = 0; i <= 59; i++ )
                            {
                               var v = String.Format("{0:00}", i);
                                %>
                                
                                <option value="<%= v %>"><%= v %></option>
                        <%
                           } %>               
                    </select>
                    <select onchange="setTime();" id="dueAmpm" name="dueAmpm">                     
                        <option value="AM">AM</option>
                        <option value="PM" selected="selected">PM</option>                      
                    </select>