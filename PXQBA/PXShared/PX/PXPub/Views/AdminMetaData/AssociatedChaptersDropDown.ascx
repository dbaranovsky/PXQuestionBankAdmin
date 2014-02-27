<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<SelectListItem>>" %>

<%= Html.DropDownList(String.Format("dropDownChapters_{0}", ViewData["accociationsType"]), Model, "Select" )%>

												   