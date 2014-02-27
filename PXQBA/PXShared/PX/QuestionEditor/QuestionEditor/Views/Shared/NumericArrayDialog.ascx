<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="hts-numeric-array-dialog hts-side-dialog">
<form action="">
    <div class="header">
        <span>Editing Numeric Array Variable</span></div>
    <div class="form">
    <div class="errors"></div>
        <ul>
            <li>
                <label>
                    Name</label>
                <div class="helper-button">
                </div>
                <input type="text" class="hts-variable-array-name required" name="hts-variable-array-name" />
                <div class="clearer">
                </div>
            </li>
            <li>
                <label>
                    Decimal Places</label>
                <%--<div class="helper-button">
                </div>--%>
                <input name="hts-variable-decimal-places" type="text" class="hts-variable-decimal-places required digits" value="0" />
                <div class="clearer">
                </div>
            </li>
        </ul>
<%--        <div class="clearer">
        </div>--%>
    </div>
    <div class="body">
    <table class="hts-variable-list">
    <tr class="hts-variable-row-template hts-variable-row">
    <td class="hts-variable-index"></td>
    <td class="hts-variable-value">
    <span></span>
                <input class="hts-variable-value-input required" name="hts-variable-value-input" type="text" style="display:none;" />
                <div class="hts-variable-actions-menu">                    
                    <div class="action hts-variable-edit">
                    </div>
                    <div class="action hts-variable-delete">
                    </div>
                    <div class="action hts-variable-save">
                    </div>
                    <div class="action hts-variable-cancel">
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
