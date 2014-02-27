<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ResourceDocument>" %>

<div id="content-item">
    <div id="contentwrapper">
        <%if (Model.isStandAlone)
          { %>
        <div id="content-nav" style="padding-bottom: 20px">
            <span id="back">&nbsp;</span> 
            <span id="next">&nbsp;</span> 
            <a style="display: none;" href=""></a>
            <a href="" class="fne-link expand-link fne-local"></a>
        </div>
        
        <div id="content">
            <h2 class="content-title">
                <%= HttpUtility.HtmlDecode(Model.title)%>
            </h2>
            <div style="text-align: left">
                <%=Model.body%>
            </div>
        </div>
        <%} else { %>
            <div style="text-align: left">
                <%=Model.body%>
            </div>
        <%} %>
    </div>
</div>