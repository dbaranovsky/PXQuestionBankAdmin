<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="pane variables hts-variables-panel">
                                <div class="header">
                                    <div class="left-variable-wrapper">
                                        <span>Variables</span>
                                    </div>
                                    <div class="add-variable-wrapper">
                                        <input type="button" id="add-variable-button" class="button add-variable-button" value="Add Variable" />
                                        <span class="variable-button-icon"></span>
                                    </div>
                                    <div class="clearer">
                                    </div>
                                </div>
                                <div class="body">
                                    <ul class="hts-variables-list">
                                        <li class="hts-variable-item-template" hts-variable-id="">
                                            <div class="icon">
                                            </div>
                                            <div class="name">
                                            </div>
                                            <div class="details">
                                            </div>
                                            <div class="hts-variable-actions-menu">
                                                <div class="action hts-variable-insert">
                                                </div>
                                                <div class="action hts-variable-edit">
                                                </div>
                                                <div class="action hts-variable-delete">
                                                </div>
                                            </div>
                                            <div class="clearer">
                                            </div>
                                        </li>
                                    </ul>
                                    <div class="hts-variables-menu" style="display: none;">
                                        <ul>
                                            <li class="numeric" hts-variable-type="numeric">Numeric</li>
                                            <li class="text" hts-variable-type="text">Text</li>
                                            <li class="math" hts-variable-type="math">Math</li>
                                            <li class="numeric-array" hts-variable-type="numarray">Numeric Array</li>
                                            <li class="text-array" hts-variable-type="textarray">Text Array</li>
                                            <li class="math-array" hts-variable-type="matharray">Math Array</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>