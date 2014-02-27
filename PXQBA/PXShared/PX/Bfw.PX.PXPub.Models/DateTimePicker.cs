using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Bfw.PX.PXPub.Models
{
    public class DateTimePicker : ContentView
    {
        /// <summary>
        /// The desired event to trigger the date picker. Default: 'click'
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// The selected date(s) as string (will be converted to Date object based on teh format suplied) 
        /// and Date object for single selection, as Array of strings or Date objects
        /// </summary>
        public DateTime[] Date { get; set; }

        /// <summary>
        /// Whatever if the date picker is appended to the element or triggered by an event. Default false
        /// </summary>
        public bool Flat { get; set; }

        /// <summary>
        /// The day week start. Default 1 (monday)
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// HTML inserted to previous links. Default '◀' (UNICODE black left arrow)
        /// </summary>
        public string Prev { get; set; }

        /// <summary>
        /// HTML inserted to next links. Default '▶' (UNICODE black right arrow)
        /// </summary>
        public string Next { get; set; }

        /// <summary>
        /// Date selection mode ['single'|'multiple'|'range']. Default 'single'
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Start view mode. Default 'days' ['days'|'months'|'years']
        /// </summary>
        public string View { get; set; }

        /// <summary>
        /// Number of calendars to render inside the date picker. Default 1
        /// </summary>
        public int Calendars { get; set; }

        /// <summary>
        /// Date format. Default 'Y-m-d'
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Date picker's position relative to the trigegr element (non flat mode only) ['top'|'left'|'right'|'bottom']. Default 'bottom'
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Location: provide a hash with keys 'days', 'daysShort', 'daysMin', 'months', 'monthsShort', 'week'. Default english
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Callback function triggered when the date picker is shown
        /// </summary>
        public string OnShow { get; set; }

        /// <summary>
        /// Callback function triggered before the date picker is shown
        /// </summary>
        public string OnBeforeShow { get; set; }

        /// <summary>
        /// Callback function triggered when the date picker is hidden
        /// </summary>
        public string OnHide { get; set; }

        /// <summary>
        /// Callback function triggered when the date is changed
        /// </summary>
        public string OnChange { get; set; }

        /// <summary>
        /// Callback function triggered when the date is rendered inside a calendar. 
        /// It should return and hash with keys: 'selected' to select the date, 
        /// 'disabled' to disable the date, 'className' for additional CSS class
        /// </summary>
        public string OnRender { get; set; }

        /// <summary>
        /// Do you want to show a time input
        /// </summary>
        public bool ShowTime { get; set; }
    }
}
