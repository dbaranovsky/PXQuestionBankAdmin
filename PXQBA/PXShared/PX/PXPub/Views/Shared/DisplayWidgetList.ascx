<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<WidgetConfigurationCollection>" %>
<div id="<%=Model.ParentId%>_WIDGETLIST" class="widgetcontextMenu" style="position: absolute;">
    <ol>
        <%
        foreach (var node in Model.AllowedWidgetsMasterList.SelectNodes("WidgetList/Widget"))
        {
            var widget = (System.Xml.XmlNode)node;
            var name = widget.Attributes["name"].Value;
            var widgetid = widget.Attributes["id"].Value;

            bool isVis = false;
            foreach (var wgConf in Model.Widgets){
                string strWid = wgConf.Id;
                if (strWid.Equals(widgetid)){
                    isVis = true;// (bool)wgConf.IsVisible;
                    break;
                }
            }

            // if (Model.CurrentWidgetList.IndexOf(widgetid) > 0) strChecked = "type=\"checkbox\"";
            string strChecked = (isVis) ? "checked" : "";
            %>
            <li>
            <input type="checkbox" <%=strChecked%> rel="<%=Model.ParentId%>" id="<%=widgetid%>"  class="widgetcheckbox" value="<%=widgetid%>" title="<%=name%>" /><%=name%></li>
        <%
        }%>
        <li><hr /><div>Create your own</div></li>
    </ol>
</div>