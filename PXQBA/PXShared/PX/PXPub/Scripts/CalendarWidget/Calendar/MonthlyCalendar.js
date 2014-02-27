String.prototype.format = function() {
    var formatted = this;
    for (arg in arguments) {
        formatted = formatted.replace("{" + arg + "}", arguments[arg]);
    }
    return formatted;
};

var PxCalendar = function($) {
    return {
        // Given a date and a day of the week, returns the date
        // that is the closest day of the week less than or equal to date.
        GetPreviousWeekday: function(date, dayOfWeek) {
            var modDate;
            for (modDate = new Date(date); modDate.getDate() > date.getDate() - 10; modDate.setDate(modDate.getDate() - 1)) {
                if (modDate.getDay() === dayOfWeek) {
                    return modDate;
                }
            }
        },

        // Given two date-time objects, return true if they fall on the same day.
        SameDay: function(d1, d2) {
            if (d1 == null || d2 == null)
                return false;
                
            return d1.getFullYear() === d2.getFullYear() &&
                   d1.getMonth() === d2.getMonth() &&
                   d1.getDate() === d2.getDate();
        }
    };
} (jQuery);

(function($) {
    $.fn.PxCalendar = function(options) {
        var id = null;

        var settings = {
            startDate: new Date(),
            rows: 8,
            highlightDates: [],
            canSelect: false,
            selectedDate: new Date('invalid date'),
            altField: '',
            dateFormat: 'D M d, yy',
            pageLength: 0
        };

        var DisplayCalendar = function() {
            var template = "<div class='calendar'>";

            if (+settings.pageLength > 0) {
                template += "<div class='shift-button up-button'>&#x25b2;</div>";
            }
            template += "<table cellspacing='0' cellpadding='0'><thead><tr><th>S</th><th>M</th><th>T</th><th>W</th><th>T</th><th>F</th><th>S</th></tr></thead><tbody>";

            var loopDate,
                css,
                dateId,
                i,
                row = 1,
                monthLabelCss,
                fillCss;
            for (loopDate = new Date(settings.startDate); row <= settings.rows; loopDate.setDate(loopDate.getDate() + 1)) {
                css = GetStyle(loopDate) + " ";

                // If this day is the first of the month, then add a month label row before continuing
                if (loopDate.getDate() == 1) {
                    fillCss = css.indexOf(' out ', 0) > -1 ? css.replace(' out', '') : css + 'out ';
                    fillCss = fillCss.replace('selected', '');
                    for (i = loopDate.getDay(); i < 7; i++) {
                        template += "<td class='" + fillCss + "'></td>";
                    }
                    monthLabelCss = css.replace('today', '').replace('selected', '');
                    template += "</tr><tr><td colspan='7' class='" + monthLabelCss + " month-name'>" + MonthNameForDate(loopDate) + "</td></tr>";
                    row += 2;
                    if (row > settings.rows) {
                        break;
                    }
                    template += "<tr>";
                    for (i = 0; i < loopDate.getDay(); i++) {
                        template += "<td class='" + css.replace('selected', '') + "'></td>";
                    }
                }

                // Start a new row on Sundays
                if (loopDate.getDay() == 0) {
                    template += "<tr>";
                }

                if (settings.canSelect) {
                    css += " selectable";
                }

                dateId = String.format('{0}-day-link-{1}', id, loopDate.getTime());
                template += String.format("<td class='cal-date {0}' id='{1}' title='{2}'>", css, dateId, $.datepicker.formatDate(settings.dateFormat, loopDate));
                if (settings.editable) {
                    template += String.format("<a href='#' class='calendarlink'>{2}</a>", id, loopDate.getDate());
                }
                else {
                    template += loopDate.getDate();
                }
                template += "</td>";

                // Close out the row at the end of Saturday
                if (loopDate.getDay() == 6) {
                    template += "</tr>";
                    row++;
                }
            }

            template += "</tbody></table>";

            if (+settings.pageLength > 0) {
                template += "<div class='shift-button down-button'>&#x25bc;</div>";
            }

            template += "</div>";

            UpdateAltField();

            return template;
        };

        var MonthNameForDate = function(date) {
            var map = {
                0: "January",
                1: "February",
                2: "March",
                3: "April",
                4: "May",
                5: "June",
                6: "July",
                7: "August",
                8: "September",
                9: "October",
                10: "November",
                11: "December"
            };
            return map[date.getMonth()];
        };

        var UpdateAltField = function() {
            if (settings.selectedDate != null && typeof settings.selectedDate == 'object' && settings.selectedDate.constructor == Date) {
                $(settings.altField).val($.datepicker.formatDate(settings.dateFormat, settings.selectedDate));
            }
        };

        var GetStyle = function(loopdate) {
            var css = "";
            var today = new Date();

            // Check if the date needs to be higlighted    
            css = GetDateStyles(loopdate);

            if (css == "") {
                if (PxCalendar.SameDay(today, loopdate)) {
                    css += " today";
                }
                if (loopdate.getMonth() % 2 == 0) {
                    css += " out";
                }
            }

            // Set the style on the selected date
            if (PxCalendar.SameDay(settings.selectedDate, loopdate)) {
                css += " selected";
            }
            return css;
        };

        var GetDateStyles = function(loopdate) {
            var css = "";

            var date;
            for (var i = 0; i < settings.highlightDates.length; i++) {
                date = settings.highlightDates[i].Date;
                if (PxCalendar.SameDay(date, loopdate)) {
                    css = settings.highlightDates[i].CssClass;
                }
            }

            return css;
        };

        if (options) {
            $.extend(settings, options);
        }

        var Render = function($this) {
            var html = DisplayCalendar();
            $this.empty();
            $.tmpl(html, "").appendTo($this);
            if (settings.canSelect) {
                $(document).off('click', '#' + id + ' td.cal-date').on('click', '#' + id + ' td.cal-date', function (event) {
                    settings.selectedDate = new Date(+event.target.id.substring(event.target.id.length - 13));
                    $this.trigger('calendar.dateSelect', [settings.selectedDate]);
                    $this.trigger('calendar.dateShift', [settings.startDate]);
                    Render($this);
                });
            }
            $('#' + id + ' div.up-button, #' + id + ' div.down-button').unbind('click').click(function(event) {
                var direction = $(event.target).hasClass('up-button') ? -1 : 1;
                settings.startDate.setDate(settings.startDate.getDate() + (direction * settings.pageLength * 7));
                $this.trigger('calendar.dateShift', [settings.startDate]);
                Render($this);
            });
        };

        return this.each(function() {
            var $this = $(this);
            id = this.id;
            Render($this);
        });
    };
})(jQuery);