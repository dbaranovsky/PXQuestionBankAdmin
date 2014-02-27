<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% Html.RenderPartial("NumericVariableDialog"); %>
<% Html.RenderPartial("TextVariableDialog"); %>
<% Html.RenderPartial("MathVariableDialog"); %>
<% Html.RenderPartial("NumericArrayDialog"); %>
<% Html.RenderPartial("TextArrayDialog"); %>
<% Html.RenderPartial("MathArrayDialog"); %>

<div class="hts-response-menu" style="display: none;">
                                        <ul>
                                            <li class="edit">Edit</li>
                                            <li class="delete">Delete</li>
                                        </ul>
                                        <input class="selected-response" type="hidden" />
                                    </div>