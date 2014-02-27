<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<SearchResultDoc>>" %>
<% bool isInstructor = ((Bfw.PX.Biz.ServiceContracts.AccessLevel)ViewData["AccessLevel"]) == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor;
   bool groupBySubtopic = (bool)ViewData["GroupResultsBySubtopic"];
   bool isFromLearningCurve = Convert.ToBoolean(ViewData["isFromLearningCurve"]);
   var title = ViewData["Title"];
   bool showQuestionsTab = (bool)ViewData.GetValue("ShowQuestionsTab", false);
   bool isLinked = title != "Removed Content";
%>
   
<% Func<SearchResultDoc, SearchResultDoc> writeDocument = delegate(SearchResultDoc doc)
       { 
            if (doc.dlap_title == null)
            {
                return null;
            }
            bool hideIfViewIsStudent = doc.Metadata.ContainsKey("meta-content-type_dlap_e") && doc.Metadata["meta-content-type_dlap_e"].ToString().ToLower().StartsWith("instructor_") || doc.Metadata.ContainsKey("meta-content-type") && doc.Metadata["meta-content-type"].ToString().ToLower().StartsWith("instructor_");

            if (!isInstructor && hideIfViewIsStudent)
            {
                return null; 
            }%> 
<div class="moreResourceItem" itemid="<%= doc.itemid %>" parentid="<%= doc.RootParentId %>"
    itemtype="<%= doc.doc_class %>" parentname="<%= doc.RootParentName %>">
    <span class="moreResourcesAction">
		<span class="lblInUse moreResourcesLabel<%= doc.Included ? " lblIncluded" : "" %>" <%= doc.InUse ? "style='display:block'" : "" %>></span>
		<span class="lblAdd moreResourcesLabel"></span>
		<span class="lblAddInUse moreResourcesLabel"></span>
    </span>
    <% var fnelink = String.Empty;

       var dlapTitle = doc.dlap_title.Replace("&amp;rsquo;", "'");
       dlapTitle = dlapTitle.Replace("&#39;", char.ConvertFromUtf32(39));
       dlapTitle = dlapTitle.Replace("&#039;", char.ConvertFromUtf32(39));
       dlapTitle = dlapTitle.Replace("&#097;", char.ConvertFromUtf32(97));
       dlapTitle = dlapTitle.Replace("&#97;", char.ConvertFromUtf32(97));
       dlapTitle = dlapTitle.Replace("&#8220;", char.ConvertFromUtf32(8220));
       dlapTitle = dlapTitle.Replace("&#8221;", char.ConvertFromUtf32(8221));
       dlapTitle = dlapTitle.Replace("&#111;", char.ConvertFromUtf32(111));
       if (doc.doc_class == "feed")
       {
           string item = "<a class='lnkMoreResourceItem fne-link loadFullFne customRssLink lnkMoreResourceItemRSS'" +
                         " title='" + dlapTitle +
                         "'  href='" + Url.Action("DisplayItem", "ContentWidget", new { mode = "ExternalUrl", renderFNE = "True", externalUrl = doc.Url }) + "'>" + doc.dlap_title.Truncate("...", 0, 50) + "</a>";

           item += string.Format(@"<span class='moreResourceRssDueDate'>{0}</span>",
                                 doc.Metadata["PubDate"]);

           fnelink = item;
       }
       else
       {
           fnelink = Url.GetComponentLink(dlapTitle.Truncate("...", 0, 50), "item",doc.itemid,
                                    new
                                        {
                                            mode = ContentViewMode.Preview,
                                            includeDiscussion = false,
                                            renderFNE = true
                                        },
                                    new
                                        {
                                            @class = "lnkMoreResourceItem",
                                            style = isLinked && doc.dlap_itemtype != "Folder" ? string.Empty : "cursor: default; color: black;",
                                            onclick = isLinked && doc.dlap_itemtype != "Folder" ? "" : "return false;",       
                                            qtip_title = HttpUtility.HtmlAttributeEncode(dlapTitle)
                                        });
       }
       if (!isFromLearningCurve)
       {
    %>
    <span class="fptitle">
        <%= fnelink %>
        <span class="bmr-subtitle">
            <%= doc.dlap_subtitle %></span> </span>
    <% }
       else
       { %>
    <span class="fptitle"><a id="<%= doc.itemid %>" class="learning_curve_resource" href="#">
        <%= dlapTitle.Truncate("...", 0, 50) %></a> <span class="bmr-subtitle">
            <%= doc.dlap_subtitle %></span> </span>
    <% } %>
</div>
<%
       return doc;
       }; %>
<% Html.RenderPartial("ResourceBreadcrumb"); %>

<% if (showQuestionsTab && isInstructor)
   { %>
    <ul class="menuResultsModes">
        <li class="resources active"><a href="#" class="modeResources">Content and Assignments</a></li>
        <li class="questions"><a href="#" class="modeQuestions">Questions</a></li>
    </ul>
<% } %>

<div class="sidepanel-body">
    <div id="search-results" class="px-default-text  <%= isInstructor ? "" : "studentView" %>">
        <% if (Model.Count <= 0)
           { %>
        No resources available.
        <% }
           else
           { %>
            <div class="resource-count"><%= Model.Count %> available item<%= (Model.Count > 1 ? "s": "") %></div>
            <%
               Func<string, object> convert = str =>
               {
                   try { return int.Parse(str); }
                   catch { return str; }
               };

               var docGroups = Model.GroupBy(doc =>
                                             !doc.Metadata.ContainsKey("meta-subtopic")
                                                 ? "Unit-wide resources"
                                                 : doc.Metadata["meta-subtopic"])
                                    .OrderBy(group => Regex.Split(((group.Key == "Unit-wide resources") ? "zzz" : group.Key).Replace(" ", ""), "([0-9]+)").Select(convert), new EnumerableComparer<object>());
                                    //.OrderBy(group => (group.Key == "Unit-wide resources") ? "zzz" : group.Key);
               bool hasSubtopics = docGroups.Count() > 1;
               if (hasSubtopics && groupBySubtopic)
               {

                   foreach (var group in docGroups)
                   { %>
                        <h2>
                            <%= group.Key %></h2>
                        <% foreach (var doc in group)
                           {
                               writeDocument(doc);
                           }
                   }
                }
               else
               {
                   foreach (var doc in Model)
                   {
                       writeDocument(doc);
                   }
               }
           } %>
    </div>
</div>

<div style="display:none">
    <div id="faceplate-move-resource-menu" class="faceplate-move-resource-menu"> 
        <div class="spanInUse">Used in:</div>
        <span id='lblInUseDescription'></span>
        <hr/>
        <ul>
            <li><a id='remove' href='#'>Remove from this unit</a></li>
            <li id='li_move'><a id='move' href='#'>Move to current unit</a></li>
            <li id='li_copy'><a id='copy' href='#'>Add copy to current unit</a></li>
        </ul>
    </div>
</div>

<input type="hidden" id="browse-resources-url" value="<%= String.Format("{0}?{1}",  Request.Url.ToString(), Request.Form.ToString()) %>" />
<script type="text/javascript">
    PxPage.OnReady(function () {
        $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesResults");
    });
</script>
