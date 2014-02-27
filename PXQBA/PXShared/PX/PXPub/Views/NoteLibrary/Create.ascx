<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Note>" %>
<div>
    <ul>
        <li>
            <% using (Ajax.BeginForm("SaveNote", "NoteLibrary", new AjaxOptions() { UpdateTargetId = "notelist" }, new { style = "margin:0px" }))
               {%>
            <%= Html.Hidden("Id",Model.NoteId)%>
            <%= Html.Hidden("EntityId",Model.EntityId)%>
            <%= Html.Hidden("Sequence",Model.Sequence)%>
            <div class="formElements">
                <label for="noteTitle">
                    Title:</label>
                <input type="text" name="Title" value="<%=Html.Encode(Model.Title) %>" style="width: 100%" />
                <label for="noteText">
                    Note:
                </label>
                <%=Html.TextArea("Text", Html.Encode(Model.Text), new { style = "width: 100%" })%>
                <div class="clear" />
                <div class="saveAndSubmit">
                    <input id="btnSubmit" type="submit" value="Save" />
                    <input id="btnCancel" name="cancel" class="btnCancelClass" type="button" value="Cancel" />
                </div>
                <div class="clear" />
            </div>
            <%} %>
        </li>
    </ul>
</div>
