<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<!-- START OF SmartSource Data Collector TAG -->
<!-- Copyright (c) 1996-2011 Webtrends Inc.  All rights reserved. -->
<!-- Version: 9.4.0 -->
<!-- Tag Builder Version: 3.2  -->
<!-- Created: 5/16/2011 3:43:30 PM -->
<script type="text/javascript" language="javascript" src="<%= Url.Content("~/Scripts/WebTrends/webtrends.js") %>"></script>
<!-- ----------------------------------------------------------------------------------- -->
<!-- Warning: The two script blocks below must remain inline. Moving them to an external -->
<!-- JavaScript include file can cause serious problems with cross-domain tracking.      -->
<!-- ----------------------------------------------------------------------------------- -->
<% 
  string webTrendsKey = string.Empty;

    string strDomain = Context.Request.Url.Host;
    if (strDomain.StartsWith("www.")) strDomain = strDomain.Remove(0, 4);
    if (strDomain.StartsWith("dev.")) strDomain = strDomain.Remove(0, 4);
    if (strDomain.StartsWith("qa.")) strDomain = strDomain.Remove(0, 3);

    webTrendsKey = String.Format("WebTrends_{0}", strDomain);
%>
<script type="text/javascript">
    //<![CDATA[

    var _domain = '<%= Context.Request.Url.Host%>';
    var _key = '<%= ConfigurationManager.AppSettings[webTrendsKey]%>';
    
    
    var _tag = new WebTrends(_domain, _key);
    _tag.dcsGetId();
    //]]>
</script>
<script type="text/javascript">
    //<![CDATA[
    _tag.dcsCustom = function() {
        // Add custom parameters here.
        //_tag.DCSext.param_name=param_value;
}
var meta = document.createElement('meta');
meta.name = 'DCS.dcssip';
meta.content = '<%= Context.Request.Url.Host%>';
document.getElementsByTagName('head')[0].appendChild(meta);

    _tag.dcsCollect();
    //]]>
</script> 

<noscript>
<%     
    string url = string.Empty;
  

    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[webTrendsKey]))
    {
        url = String.Format("//statse.webtrendslive.com/{0}/njs.gif?dcsuri=/nojavascript&amp;WT.js=No&amp;WT.tv=9.4.0", ConfigurationManager.AppSettings[webTrendsKey]);
    
%>

<div><img alt="DCSIMG" id="DCSIMG" width="1" height="1" src="<%= url %>"/></div>
<%} %>
</noscript>
<!-- END OF SmartSource Data Collector TAG -->