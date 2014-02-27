<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="hts-math-variable-dialog hts-side-dialog">
<form action="">
    <div class="header">
        <span>Editing Math Variable</span></div>
    <div class="form">
    <div class="errors"></div>
        <ul>
            <li>
                <label>
                    Name</label>

                <input type="text" class="hts-variable-name required" name="hts-variable-name" />
                <div class="clearer">
                </div>
            </li>
        </ul>
    </div>
    <div class="body">
        <div class="hts-pool-list">
            <div class="hts-pool-template hts-pool">
                <div class="header">
                    <span>Default Values Pool</span>
                    <div class="delete-button">
                    </div>
              
                </div>
                <div class="body">
                    <ul>
                        <li class="hts-variable-pool-condition-labels">
                            <label>
                                Condition</label>
                        </li>
                        <li class="hts-variable-pool-condition-inputs">
                            <input type="text" class="hts-variable-pool-condition-left-expression required" name="hts-variable-pool-condition-left-expression" />
                            <select name="hts-variable-pool-condition-type" class="hts-variable-pool-condition-type"
                                size="1">
                                <option value="lt"><</option>
                                <option value="le"><=</option>
                                <option value="gt">></option>
                                <option value="ge">>=</option>
                                <option value="eq">=</option>
                                <option value="ne">!=</option>
                            </select>
                            <input type="text" class="hts-variable-pool-condition-right-expression required" name="hts-variable-pool-condition-right-expression" />                            
                        </li>
                        <li class="hts-variable-pool-values-inputs">
                            <table class="hts-math-pool-input">
                                <tr>
                                    <td class="hts-answer">
                                        <img src="<%: Url.Content("~/Content/images/mathinput.gif") %>" alt="math" class="hts-formula-input" name="hts-formula-input" />
                                    </td>
                                    <td class="hts-actions">
                                        <div class="action hts-grid-editrow">
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <div class="clearer"></div>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
    <div class="footer">
        <input type="button" class="button save-variable-button" value="Save" />
        <input type="button" class="button cancel-variable-button" value="Cancel" />
        <input type="button" class="button add-conditional" value="Add Conditional Values Pool" />
    </div>
    </form>
</div>
