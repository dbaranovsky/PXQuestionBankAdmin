<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Widget>" %>
<%
    WidgetCallback callback = null;
    if (this.Model.Callbacks.ContainsKey("OnBeforeAdd"))
    {
        callback = this.Model.Callbacks["OnBeforeAdd"];
    }
    else if (Model.Callbacks.ContainsKey("OnBeforeEdit"))
    {
        callback = Model.Callbacks["OnBeforeEdit"];
    }
    if (callback == null)
    {
        return;
    }
%>

<div class="widgetItem" silentCreation="false">
    <div class="customwidgetBody">
        <% Html.RenderAction(callback.Action, callback.Controller, Model);  %>  
            <div id = "ErrorText" class="ErrorTextNotVisible"></div>
            <input type="hidden" name="WidgetPage" id="WidgetPage" />
            <input type="hidden" name="WidgetZoneID" id="WidgetZoneID" value="1111"/>
            <input type="hidden" name="WidgetTemplateID" id="WidgetTemplateID" />
            <input type="hidden" name="WidgetID" id="WidgetID" />
    </div>
</div>
