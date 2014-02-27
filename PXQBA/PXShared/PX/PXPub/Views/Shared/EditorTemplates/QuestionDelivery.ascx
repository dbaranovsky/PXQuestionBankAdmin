<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.QuestionDelivery>" %>

<% if (ViewData["disabled"] == null)
   { %>
        <div><input type="radio" name="QuestionDelivery" value="All" <%if(Model==QuestionDelivery.All){%><%:Html.Raw("checked=\"checked\"") %><%}%> /> All questions on one screen</div>
        <div><input type="radio" name="QuestionDelivery" value="One" <%if(Model==QuestionDelivery.One){%><%:Html.Raw("checked=\"checked\"") %><%}%> /> One question at a time</div>
        <!--<div><input type="radio" name="QuestionDelivery" value="OneNoBacktrack" <%if(Model==QuestionDelivery.OneNoBacktrack){%><%:Html.Raw("checked=\"checked\"") %><%}%> /> One question at a time, no backtracking</div>-->
<%} %>
<% else { %>
        <div style="color:gray"><input type="radio" name="QuestionDelivery" disabled = <%= ViewData["disabled"] %> /> All questions on one screen</div>
        <div style="color:gray"><input type="radio" name="QuestionDelivery" disabled = <%= ViewData["disabled"] %> /> One question at a time</div>
        <!-- <div style="color:gray"><input type="radio" name="QuestionDelivery" disabled = <%= ViewData["disabled"] %> /> One question at a time, no backtracking</div> -->
<%} %>