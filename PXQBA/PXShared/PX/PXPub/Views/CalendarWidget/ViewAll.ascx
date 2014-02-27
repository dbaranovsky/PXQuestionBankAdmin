<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignmentWidget>" %>

<div id="agenda-content-wrapper">
    <% Html.RenderAction(ViewData["type"].ToString() == "agenda" ? "AgendaFullView" : "MonthFullView", "CalendarWidget"); %>
</div>

<div id="dialog-confirm" title="Also remove this item from the gradebook?" style="display: none;">    
    <ol>
        <li>Any existing student grades for this item will be lost.</li>
        <li>You can re-add this item from "Resources" later.</li>
    </ol>
</div>

<script id="tip-template" type="text/x-handlebars-template">
    <div class="tip-top">

        <strong class="tip-title">
            {{description}}
        </strong>

        <p>

            {{#if groups}}
                <em class="tip-groups">
                    {{groups}}
                </em>
                <br />
            {{/if}}

            <span class="tip-points">
                {{points}} points
            </span>

            {{#if exception.has}}
                <span class="tip-date-exception">
                Exception Due Date:
                    <br />
                    {{exception.date}} at {{exception.time}}
                </span>
            {{/if}}

            <span class="tip-date-original">
                {{#if exception.has}}
                    {{original.label}}
                {{/if}}
                Due Date:
                <br />
                {{original.date}} at {{original.time}}
            </span>

        </p>

    </div>

    {{{link.edit}}}
    {{{link.open}}}
    {{{link.clear}}}

</script>