<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="hts-numeric-variable-dialog hts-side-dialog">
<form action="">
    <div class="header">
        <span>Editing Numeric Variable</span></div>
    <div class="form">
    <div class="errors"></div>
        <ul>
            <li>
                <label>
                    Name</label>
                <div class="helper-button">
                </div>
                <input name="hts-variable-name" type="text" class="hts-variable-name required" />
                <div class="clearer">
                </div>
            </li>
            <li>
                <label>
                    Decimal Places</label>
                <div class="helper-button">
                </div>
                <input name="hts-variable-decimal-places" type="text" class="hts-variable-decimal-places required digits" value="0" />
                <div class="clearer">
                </div>
            </li>
        </ul>
        <div class="clearer">
        </div>
    </div>
    <div class="body">
        <div class="hts-pool-list">
            <div class="hts-pool-template hts-pool">
                <div class="header">
                    <span>Default Values Pool</span>
                    <div class="delete-button">
                    </div>
                    <div class="helper-button">
                    </div>
                </div>
                <div class="body">
                    <ul>
                        <li class="hts-variable-pool-condition-labels">
                            <label>
                                Condition</label><div class="helper-button">
                                </div>
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
                        <li>
                            <input type="radio" value="range" checked="checked" name="hts-variable-pool-type"
                                class="hts-variable-pool-type" />
                            Range
                            <input type="radio" value="list" name="hts-variable-pool-type" class="hts-variable-pool-type" />
                            List
                            <div class="helper-button">
                            </div>
                            <div class="clearer">
                            </div>
                        </li>
                        <li class="hts-variable-pool-range-inputs">
                            <select name="hts-variable-pool-range-left-type" class="hts-variable-pool-range-left-type"
                                size="1">
                                <option value="lt"><</option>
                                <option value="le"><=</option>
                                <option value="gt">></option>
                                <option value="ge">>=</option>
                            </select>
                            <input type="text" class="hts-variable-pool-range-left-expression required" name="hts-variable-pool-range-left-expression" />
                            <select name="hts-variable-pool-range-right-type" class="hts-variable-pool-range-right-type"
                                size="1">
                                <option value="lt"><</option>
                                <option value="le"><=</option>
                                <option value="gt">></option>
                                <option value="ge">>=</option>
                            </select>
                            <input type="text" class="hts-variable-pool-range-right-expression required" name="hts-variable-pool-range-right-expression" />
                            <div class="clearer">
                            </div>
                        </li>
                        <li class="hts-variable-pool-list-inputs">
                            <input type="text" class="hts-variable-pool-inclusion required" name="hts-variable-pool-inclusion" />
                            <div class="clearer">
                            </div>
                        </li>
                        <li class="hts-variable-pool-exclude-inputs">
                            <input type="checkbox" name="hts-variable-pool-exclude" class="hts-variable-pool-exclude" />
                            <label>
                                Exclude</label>
                            <input type="text" class="hts-variable-pool-exclusion required" disabled="disabled" name="hts-variable-pool-exclusion" />
                            <div class="helper-button">
                            </div>
                            <div class="clearer">
                            </div>
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
