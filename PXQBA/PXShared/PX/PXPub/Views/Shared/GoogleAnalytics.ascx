<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.SiteAnalytics>" %>

<script type="text/javascript">

    var _gaq = _gaq || [];
    _gaq.push(['_setAccount', '<%= Model.SiteKey %>']);
    _gaq.push(['_setDomainName', '<%= Model.RequestDomain %>']);
    _gaq.push(['_setCookiePath', '<%= Model.RequestPath %>']);
    
    <% if (!Model.CustomParams.IsNullOrEmpty()) 
       {
           var index = 1;
           foreach (var p in Model.CustomParams)
           { %>
    _gaq.push(['_setCustomVar', <%= index %>, '<%= p.Key %>', '<%= p.Value %>', 3]);               
        <% 
            ++index; 
           }
       } %>
     
    _gaq.push(['_trackPageview']);

    (function () {
        var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
        ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
    })();

</script>