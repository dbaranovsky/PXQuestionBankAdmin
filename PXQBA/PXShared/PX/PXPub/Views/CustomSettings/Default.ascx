<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>

<%  Html.RenderAction("Index", "SettingsView", new { id = Model.Id }); %>
               