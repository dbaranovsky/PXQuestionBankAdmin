<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div class="webtour" toolTipId="<%=  (String)ViewData["tooltipId"] %>"></div>

<div id="<%= (String)ViewData["tooltipId"] %>" isToolTipHidden="<%= (bool) ViewData["isToolTipHidden"] %>" 
        qtipTargetPosition="<%= (String)ViewData["qtipTargetPosition"]  %>" qtipTooltipPosition="<%= (String)ViewData["qtipTooltipPosition"] %>" style="display:none" qtipTarget="<%= (String)ViewData["qtipTarget"] %>">
    <div class="persistingtooltip" toolTipId="<%= (String)ViewData["tooltipId"] %>">
        <div class="qtipClose" toolTipId="<%= (String)ViewData["tooltipId"] %>">Close</div>
        <div class="qtipDesc"><%= (String)ViewData["toolTipDescription"] %></div>
    </div>
</div>
