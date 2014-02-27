<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.BhComponent>" %>
 <%     
	// this is a hack, this shouldn't be done anywhere else. we are doing this because it
	// is expedient. it would be better to use an action filter to populate the viewdata with the
	// domain name. 
	var locator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;
	var ctx = locator.GetInstance<Bfw.PX.Biz.ServiceContracts.IBusinessContext>();
	Model.DomainUserSpace = ctx.Domain.Userspace;

  var samlSubDomain = ConfigurationManager.AppSettings["SAMLsubDomain"];
	string url = string.Empty;
	var query = Model.GetQueryString();
	var subQuery = string.IsNullOrEmpty(query) ? "showBeforeUnloadPrompts=false" : query.ToLower().Contains("showbeforeunloadprompts") ? "" : "&showBeforeUnloadPrompts=false";
	query = string.IsNullOrEmpty(query) ? string.Format("?{0}",subQuery) : string.Format("?{0}{1}", query, subQuery);

    if (!string.IsNullOrEmpty(samlSubDomain))
    {
        // parse domain name from url
        string host = Request.Url.Host.ToString();
        string LocalDomainName = "";
        if (host.Split('.').Length > 1)
        {
            int last = host.LastIndexOf(".");
            string concatURL = host.Substring(0, last);
            LocalDomainName = host.Substring(concatURL.LastIndexOf(".") + 1, host.Length - (concatURL.LastIndexOf(".") + 1));
        }
        else
        {
            LocalDomainName = host;
        }

        var urlBase = String.Format("{0}.{1}.{2}", Model.DomainUserSpace.ToLower(), ConfigurationManager.AppSettings["SAMLsubDomain"], (host == "localhost") ? "bfwpub.com" : LocalDomainName);
        url = String.Format("http://{0}/brainhoney/component/{1}{2}", urlBase, Model.ComponentName, query);
    }
    else
    {
        url = String.Format("http://{0}/brainhoney/component/{1}{2}", Request.Url.Host, Model.ComponentName, query);
    }
   
	var guid = Guid.NewGuid().ToString(); 
	
	%>
    <div class="bh-component-wrapper" id="bh-component-wrapper-<%= guid %>">
        <div id="bh-component-frame-<%= guid %>" class="bh-component" rel="<%= url %>" component-name="<%=Model.ComponentName %>"></div>
    </div>
<script type="text/javascript">
    PxPage.OnReady(function() {
        PxPage.SetFrameApiHooks();
    });
</script>