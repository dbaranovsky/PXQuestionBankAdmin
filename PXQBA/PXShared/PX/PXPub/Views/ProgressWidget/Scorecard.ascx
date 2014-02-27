<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ProgressWidget>" %>

<div class="complete row first">
    <h5>Course Completion</h5>
    <div class="row-content">
        <span class="description">You've received grades on <%= String.Format("{0:P0}", Model.PercentGraded) %> of your gradable items.</span>
        <div class="full-bar">
            <div class="part-bar" style="height:100%; width:<%= Model.PercentGraded*100 %>%;"></div>
        </div>
    </div>
</div>
<% if (null != Model.OverallGrade)
   { %>
    <div class="overall row">
        <h5>Overall Grade</h5>
        <div class="row-content">Your current score for this class is <%= Model.OverallGrade %>.</div>
    </div>
<% } %>
<div class="submitted row">
    <h5>Assignments Submitted</h5>
    <div class="row-content">
        <span class="description">You've completed <%= Model.Complete %> out of <%= Model.Due %> assignments that are past due or due today.</span>
        <div class="full-bar">
            <div class="part-bar" style="height:100%; width:<%= Model.Due != 0 ? (((double)Model.Complete)/Model.Due) * 100 : 100 %>%;"></div>
        </div>
    </div>
</div>
<div class="clear"></div>