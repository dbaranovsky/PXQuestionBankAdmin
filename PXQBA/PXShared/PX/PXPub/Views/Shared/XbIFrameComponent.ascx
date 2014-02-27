<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.BhComponent>" %>
<%     
    // this is a hack, this shouldn't be done anywhere else. we are doing this because it
    // is expedient. it would be better to use an action filter to populate the viewdata with the
    // domain name. 
    var locator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;
    var ctx = locator.GetInstance<Bfw.PX.Biz.ServiceContracts.IBusinessContext>();
    Model.DomainUserSpace = ctx.Domain.Userspace;
    string url = string.Empty;
    var component = Model.ComponentName;
    var id = Model.Id;
    var query = Model.GetQueryString();
    var subQuery = string.IsNullOrEmpty(query) ? "showBeforeUnloadPrompts=false" : query.ToLower().Contains("showbeforeunloadprompts") ? "" : "&showBeforeUnloadPrompts=false";
    query = string.IsNullOrEmpty(query) ? string.Format("?{0}", subQuery) : string.Format("?{0}{1}", query, subQuery);

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

    var urlBase = ConfigurationManager.AppSettings["XBookComponentURL"];
    url = String.Format("{0}/Components/{1}{2}", urlBase, component, query); 
    var guid = Guid.NewGuid().ToString(); 
%>
<div id="<%= id%>" class="xb-component" rel="<%= url %>">
</div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            var initWidget = function () {
                var id = '<%=id %>';

                //In case the control is being opened in an fne, we need to give it a different name.  This is a bit hacky, ideally when the control is requested
                //it would know it is coming from an fne window.
                if ($('#fne-window #' + id).length > 0) {
                    $('#fne-window #' + id).attr('id', 'fne-' + id);
                    id = 'fne-' + id;
                }
                $('#' + id).initXbookWidget();
                $(PxPage.switchboard).trigger('XBook_IFrame_Loading', ["<%=component %>"]);
            };
            if (typeof PxXbookWidget !== 'undefined') {
                initWidget();
            } else {
                PxPage.Require(['<%= Url.ContentCache("~/Scripts/Xbook/XbookWidget.js") %>'], function () {
                    initWidget();
                });
            }
        });
    } (jQuery));
</script>
