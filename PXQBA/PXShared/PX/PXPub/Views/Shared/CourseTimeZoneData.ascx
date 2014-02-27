<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.CourseHeader>" %>


    <%= Html.HiddenFor(m => m.TimeZoneDaylightOffset) %>
    <%= Html.HiddenFor(m => m.TimeZoneStandardOffset) %>
    <%= Html.HiddenFor(m => m.TimeZoneDaylightStartTime) %>
    <%= Html.HiddenFor(m => m.TimeZoneStandardStartTime) %>
    <%= Html.HiddenFor(m => m.TimeZoneDaylightStartTimeNextYear) %>
    <%= Html.HiddenFor(m => m.TimeZoneStandardStartTimeNextYear) %>
    <%= Html.HiddenFor(m => m.TimeZoneAbbreviation) %>