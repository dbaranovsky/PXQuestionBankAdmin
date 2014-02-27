<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ExternalContent>" %>
<% @Import Namespace="System.IO" %>
<h2 class="content-title"></h2>

<div id="document-viewer-wrapper">
  <!-- These fields are needed for taking unassigned quizzes in XB -->
  <%= Html.HiddenFor(m => m.BHParentId) %>
  <%= Html.HiddenFor(m => m.IsGradable) %>
  <%= Html.HiddenFor(m => m.MaxPoints) %>
  <%= Html.HiddenFor(m => m.OverrideDueDateReq) %>
<% 
    var hlDescription = Model.Title;
    var proxyBase = String.IsNullOrEmpty(ConfigurationManager.AppSettings["ExternalResourceBaseUrl"]) ?
                    ConfigurationManager.AppSettings["AppDomainUrl"] :
                    String.Format("{0}/{1}/", ConfigurationManager.AppSettings["ExternalResourceBaseUrl"], Model.DisciplineId);

    string errMessage = string.Empty;

    string externalUrl = "";
    if (!Model.Url.Trim().StartsWith("/"))
    {
        externalUrl = Model.Url.Trim();
        if (!externalUrl.ToLower().StartsWith("http"))
        {
            externalUrl = "http://" + externalUrl;
        }
    }
    
    Uri uri = null;

    try
    {
        uri = new Uri(Model.Url, UriKind.RelativeOrAbsolute);
    }
    catch (Exception ex)
    {
        errMessage = string.Format("Uri is not correct -- uri = {0} <br /> exception:<br />{1} <br />{2}", Model.Url, ex.Message, ex.StackTrace);
    }

    if (!string.IsNullOrEmpty(errMessage))
    {
        %>
          <div id="ErrorMessage" style="visibility:hidden;">  errMessage = <%= errMessage%> </div>
        <%
        return;
    }

    string proxyUrl = Model.Url;
    if (!uri.IsAbsoluteUri && !Model.IsTransformedArgaItem)
    {
        proxyUrl = proxyBase + Model.Url;
    }

    bool isBinary = false;
    try
    {
        string strUrl = (Path.GetExtension(externalUrl.ToLower()));
        if (strUrl == ".pdf" || strUrl == ".doc" || strUrl == ".ppt" || strUrl == ".pptx" || strUrl == ".pptm" || strUrl == ".docx" || strUrl == ".zip" || strUrl == ".xlsm" || strUrl == ".xlsx" || strUrl == ".xls")
        {
            isBinary = true;
        }
    }
    catch { }


    if (Model.FacetMetadata.ContainsKey("meta-content-type") && Model.FacetMetadata["meta-content-type"].Contains("LearningCurve"))
    { //if ExternalContent is a LearningCurve
        const string studentViewFlag = "&view=student";
        const string debugMode = "&test_mode=true";
        
        proxyUrl = proxyUrl + (ResourceEngine.IsDebug() ? debugMode : ""); 
        
        if (Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
        {
            proxyUrl = proxyUrl + studentViewFlag;
        }
    }

    var allowComments = Model.AllowComments;
    
    //Ideally we'd be able to get rid of proxyconfig, but since we can't do that right now,
    //only allow comments if both the proxy and the item allow it (and the course... through the item).
    if (Model.ProxyConfig != null && Model.ProxyConfig.AllowComments != null)
    {
      allowComments = (bool)Model.ProxyConfig.AllowComments && allowComments;
    }
    
    var doc = new DocumentToView()
    {
        ItemId = Model.Id,
        Url = proxyUrl,
        HighlightType = 1,
        HighlightDescription = hlDescription,
        AllowComments = allowComments,
        DisciplineId = Model.DisciplineId,
        NoteId = Model.NoteId,
        ExternalUrl = externalUrl,
        IsBinary = isBinary, 
        IsProductCourse = Model.IsProductCourse,
        IsExernalContent = true
    };
    Html.RenderPartial("DocumentViewer", doc); 
%>

<input type="hidden" id="DisciplineId" value="<%=Model.DisciplineId%>" />
</div>