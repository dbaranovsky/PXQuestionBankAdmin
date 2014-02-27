<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DashboardItem>" %>


<div id="display-container">
<div id="display-info">
<h2><b>Oh, looks like this is your first branch.</b></h2>


<p>Branching courses can get a little complicated for those unfamiliar with the concept, so here's a quick explanation:</p>
<p>Branching creates a "linked" copy of a course, something we call a <i>branch</i>. Any changes you make to the original course are also made in the Branch. However, changes you make in a branch do not affect the original course or any other branches.</p>

<p>Suggestions:</p>



<p id="instruction-1"><b class="instruction-1"> <i>&#155;</i> Use branching to teach multiple sections of a course in a single semester.</b></p>
<div id="branch-instruction-1" class="display-branch display-branch-none">

<p>For example, if you are planning to teach two sections in a single semester, it would be a good idea to branch your course twice, like this:</p>

<p>This structure allows you to make changes to both sections simultaneously throughout the semester by editing the parent course. You could also make changes specific to each of your sections (for example, due dates) by editing the branches. </p>


</div>
<p id="instruction-2"><b class="instruction-2"> 	&#155; Don't branch old courses to create courses for the current semester.</b></p>



<div id="branch-instruction-2" class="display-branch display-branch-none">

<p>When the next semester begins and you're teaching the same course, you may be inclined to create two more branches off of last semester's parent course: one for each of your new sections. However, this can introduce some issues.</p>
<p>For example, edit to the parent course affect not only your two new sections, but your two old ones from the previous semester! This could potentially impact student grades in the old courses, and the old branches will no longer represent the course as it was taught.</p>
<p>To prevent situations like this, it's a better practice to create another course that is a copy of a course from a previous semester, that to branch the course from a previous semester twice.</p>

</div></div>
</div>