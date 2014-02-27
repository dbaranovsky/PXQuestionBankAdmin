<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IDictionary<string,string>>" %>
<div class="modal_dialog_title" id="moreResourcesTitleName">
    Resources</div>
<h2>
    Content and assignments</h2>
<ul>
    <li>
        <%=Ajax.ActionLink("Content by type", "FacePlateBrowseResourcesFacets", "BrowseMoreResourcesWidget",
                           new RouteValueDictionary(new { fieldNames =  "meta-content-type_dlap_e", title = "Content by type"  }),
                           new AjaxOptions() { UpdateTargetId = "browseResults", LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500 })%>
    </li>
    <li>
        <%--  <%  var rv = new RouteValueDictionary();
            var strings = new string[] {"meta-topic_dlap_e", "meta-topics/meta-topic_dlap_e"};
            for (int i = 0; i < strings.Length; ++i)
            {
                rv["fieldNames[" + i + "]"] = strings[i];
            }
            rv["title"] = "Content by chapter";
         %>--%>
        <%=Ajax.ActionLink("Content by chapter", "FacePlateBrowseResourcesFacets", "BrowseMoreResourcesWidget", 
                           new RouteValueDictionary(new { fieldNames =  "meta-topics/meta-topic_dlap_e", title = "Content by chapter"  }),
                           new AjaxOptions() { UpdateTargetId = "browseResults", LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500 })%>
    </li>
    <% bool isInstructor = ((Bfw.PX.Biz.ServiceContracts.AccessLevel)ViewData["AccessLevel"]) ==
                           Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor;
       if (isInstructor)
       {%>
        <li>
            <%=Ajax.ActionLink("Content I've created", "FacePlateBrowseMyResources", "BrowseMoreResourcesWidget",
                               new AjaxOptions() { UpdateTargetId = "browseResults", LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500 })%>
        </li>
        <li>
            <%=Ajax.ActionLink("Removed content", "FacePlateBrowseRemoved", "BrowseMoreResourcesWidget",
                               new AjaxOptions() { UpdateTargetId = "browseResults", LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500 })%>
        </li>
    <% }%>
    <%--<% RSS FEED LIST
       foreach (var feedName in Model.Keys)
       {%>
       <li>
        <%=Ajax.ActionLink(Model[feedName], "FacePlateBrowseResourcesTypes", "BrowseMoreResourcesWidget", new AjaxOptions() { UpdateTargetId = "browseResults" })%>
    </li>
    <option value='<%= Model[feedName] %>'>
        <%= feedName %></option>
    <% }%>--%>
</ul>
 <% 
    if (isInstructor)
    { %>
     <h2>
         Questions</h2>
     <ul>
         <li>
               <%=Ajax.ActionLink("Questions by chapter", "ResourceQuestionBanks", "BrowseMoreResourcesWidget",new RouteValueDictionary(new {collectionType = "ByPublisher"}),
                               new AjaxOptions() { UpdateTargetId = "browseResults", LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500 })%>
         </li>
         <li>
              <%=Ajax.ActionLink("Questions by assessment", "ResourceQuestionBanks", "BrowseMoreResourcesWidget", new RouteValueDictionary(new { collectionType = "InExistingAssessment" }),
                               new AjaxOptions() { UpdateTargetId = "browseResults", LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500 })%>
         </li>
         <li>
              <%=Ajax.ActionLink("Questions I've created or edited", "ResourceQuestionBanks", "BrowseMoreResourcesWidget", new RouteValueDictionary(new { collectionType = "ByMe" }),
             new AjaxOptions() { UpdateTargetId = "browseResults", LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500 })%>
         </li>
     </ul>
    <% } %>