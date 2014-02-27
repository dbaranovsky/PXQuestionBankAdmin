<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="editor-menu">
            <div class="left">
                <div id="view-button">
                    <label>
                        <input type="radio" name="toggle-view" checked="checked" class="toggle-view-button html-view"/><span>Editor</span></label>
                    <label>
                        <input type="radio" name="toggle-view" class="toggle-view-button xml-view"/><span>Raw XML</span></label>
                </div>                
                <input type="button" class="button save-button" value="Save" />
                <input type="button" class="button revert-button" value="Revert to Saved" />
            </div>
            <div class="right">
                <input type="button" class="button add-step-button" value="Add Step" />
                <input type="button" class="button add-solution-button" value="Add Solution" />
                <input type="button" class="button show-variables-button" value="Show Variables" />
                <input type="button" class="button hide-variables-button" style="display: none;"
                    value="Hide Variables" />
            </div>
            <div class="clearer">
            </div>
        </div>