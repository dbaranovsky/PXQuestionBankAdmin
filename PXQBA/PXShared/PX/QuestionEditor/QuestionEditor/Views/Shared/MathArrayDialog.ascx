<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="hts-math-array-dialog hts-side-dialog">
<form action="">
    <div class="header">
        <span>Editing Math Array Variable</span></div>
    <div class="form">
    <div class="errors"></div>
        <ul>
            <li>
                <label>
                    Name</label>
                <div class="helper-button">
                </div>
                <input type="text" class="hts-variable-array-name required" name="hts-variable-array-name" value="<string>" />
                <div class="clearer pad5">
                </div>
                <label>Values</label>
                <div class="helper-button" />
                <div class="clearer" />
            </li>
        </ul>
<%--        <div class="clearer">
        </div>--%>
    </div>
    <div class="body">
        <table class="hts-variable-list">
            <tr class="hts-variable-row-template hts-variable-row">
                <td class="hts-variable-index">
                </td>
                <td class="hts-variable-answer">
                    <img src="<%: Url.Content("~/Content/images/mathinput.gif") %>" alt="math" class="hts-formula-input" />
                </td>
                <td class="hts-variable-actions">
                    <div class="hts-variable-actions-menu">
                        <%--<div class="helper-button">
                        </div>--%>
                        <div class="action hts-variable-edit">
                        </div>
                        <div class="action hts-variable-delete">
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div class="footer">
        <input type="button" class="button save-array-button" value="Save" />
        <input type="button" class="button cancel-array-button" value="Cancel" />
        <input type="button" class="button add-array-value-button" value="Add Array Value" />
    </div>
    </form>
</div>
