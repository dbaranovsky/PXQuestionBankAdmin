<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<HighlightModel>>" %>

<div id="topNotesList">
<% foreach (var highlight in Model) {
       if (highlight.NoteType == Bfw.PX.Biz.DataContracts.NoteType.GeneralNote)
       {
           Html.RenderPartial("Show", highlight);
       }
 } %>
</div>

