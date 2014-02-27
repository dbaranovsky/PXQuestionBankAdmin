<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Trail>" %>
<% 
    var isQuestionBank = ViewData["isQuestionBank"] == null ? false: (Boolean)ViewData["isQuestionBank"];
    var quizId = ViewData["quizId"] == null ? "" : ViewData["quizId"].ToString();
    var randomNumber = ViewData["randomNumber"] == null ? 0 :(Int32)ViewData["randomNumber"];
    var rootlinkUrl = Url.GetComponentHash("item", quizId, new { mode = ContentViewMode.Questions, renderFne = true, r = randomNumber });
%>
<div class="breadcrumb">
     <span id="question-list-level" style='<%= !isQuestionBank ? "display:none" :""  %>'>
             <a href="<%= rootlinkUrl %>">Question Banks</a>
             <span>&raquo; </span>
            </span>
            
    <% string selectedItem = "";
       string selectedItemTitle = "";
       var levelIndex = 0;
       if (Model != null)
       {
           foreach (var level in Model.Levels)
           {
               if (!level.Items.IsNullOrEmpty())
               {
                   selectedItem = level.Items[level.Selected].Id;
                   selectedItemTitle = level.Items[level.Selected].Display;
                   var levelId = Guid.NewGuid().ToString("N");

                   if (levelIndex++ > 0)
                   { %> &raquo;<% } %>
            <span id="<%= levelId %>-level">
                <a href="javascript:" class="path-item" id="<%= level.Items[level.Selected].Id %>-bcitem"><%= level.Items[level.Selected].Display %></a>
                <div class="bread-crumb-siblings-list" style="display:none;" id="<%= levelId %>">
                    <ul>
                        <% foreach (var item in level.Items)
                           { %>
                            <li><a href="#" class="sibling-item" id="<%= item.Id %>"><%= item.Display %></a></li>
                        <% } %>
                    </ul>
                </div>
            </span>
    <% }
           } %>
    
    

        <input class="selected-item" type="hidden" value="<%= selectedItem %>" />
        <input class="selected-item-title" type="hidden" value="<%= selectedItemTitle %>" />
        <input class="root-item" type="hidden" value="<%= Model.RootItem %>" />
        <script type="text/javascript">
            PxBreadcrumb.Init();
        </script>
    <% } %>
</div>