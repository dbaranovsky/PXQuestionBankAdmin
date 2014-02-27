<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ECommerceInfo>" %>
<div id="PX_HOME_ZONE_3" class="zoneParent">
<div id="ecommerce-action-bar" >
    <% 
       var targetUrl = Url.RequestContext.HttpContext.Request.Url.ToString();
       //only want the substring to the /ecom
       targetUrl = targetUrl.Substring(0, targetUrl.IndexOf("ECommerce") - 1);
       var newTargetUrl = targetUrl.Remove(0, "http://".Count());
       
       var sShoppingCart = String.Format(newTargetUrl + "/{0}/{1}", "Account", "Auth");
       var trialAccess = String.Format(newTargetUrl + "/{0}/{1}", "Account", "Auth");
       
       //this is where the logic will go depending various values in the Model.
     %>
    
    <div class="ecom-side-links">
        <% if (!Model.IsEntitled)
           {
               var accesslink = ViewData["StudentAccessCodeLink"].ToString();
               accesslink = String.Format(accesslink, Url.Action("Auth", "Account", null, "http"));  %>

            <div class="student-links">
            <h2>Students</h2>
            <a class="ecom-btns" href="<%= Url.RouteUrl("Login", new { returnUrl = Request.QueryString["returnUrl"] })%>">Log in<span class="icon"></span></a>

            <% var isValidAccessCodeLink = ViewData["StudentAccessCodeLink"].ToString() != ""; %>
            <% if (isValidAccessCodeLink == true)
               { %>
                <a href="#" style="text-decoration:none;">New user?</a>
                <a class="ecom-btns" href="<%=accesslink %>">Enter Your Student Access Code<span class="icon"></span></a>
                <a class="what-is-link" href="">WHAT IS THIS?</a>
           <% } %>
           
           <%
               var allowPurchase = (bool)ViewData["allowPurchase"];
               var allowSampling = (bool)ViewData["allowSampling"];
               var allowTrialAccess = (bool)ViewData["allowTrialAccess"];                              
               var purchaseLink = ViewData["PurchaseLink"].ToString();
               var trialaccessLink = ViewData["TrialAccessLink"].ToString();
               purchaseLink = String.Format(purchaseLink, HttpUtility.UrlEncode(sShoppingCart));
               trialaccessLink = String.Format(trialaccessLink, HttpUtility.UrlEncode(trialAccess));
               
               var isbn = ViewData["ISBN"].ToString();
               var formdata = "<PurchaseRequest><ISBN>" + isbn + "</ISBN><ReturnURL>" + targetUrl + "</PurchaseRequest></input>";
                        
               if (purchaseLink != "" && isbn != "" && allowPurchase)
               { %>                        
                <a class="ecom-btns" id="purchase-access" href="<%=purchaseLink %>">Purchase Access<span class="icon"></span></a>            
            <% }       
               if (trialaccessLink != "" && isbn != "" && allowTrialAccess)
               { %>                        
                <a class="ecom-btns" id="A2" href="<%=trialaccessLink %>">Temporary Access<span class="icon"></span></a>            
            <% } %>      
         </div>

        <div class="instructor-links">
            <h2>Instructors</h2>
            <% 
                var requestAccessLink = ViewData["ecomRequestAccess"].ToString();
                requestAccessLink = String.Format(requestAccessLink, ViewData["Isbn10"].ToString(), HttpUtility.UrlEncode(targetUrl.Remove(0, "http://".Count())));
                %>
                <a href="#" style="text-decoration:none;">Returning user?</a>
                <a class="ecom-btns" href="<%=Url.RouteUrl("Login", new { returnUrl = Request.QueryString["returnUrl"] }) %>">Log in<span class="icon"></span></a>     
                 
            <% if (isbn != "" && allowSampling)
               {  %>
                    <a href="#" style="text-decoration:none;">New user?</a>
                    <a class="ecom-btns" href="<%=requestAccessLink %>">Request Instructor Access<span class="icon"></span></a>
            <% } %>
        </div>
        <script type="text/javascript">
                PxPage.OnReady(function () {
                    PxPage.Require(['<%= Url.ContentCache("~/Scripts/SystemCheck/SystemCheck.js") %>'], function() {
                        PDX_System_Check.initialize();
                    });
                });
        </script>
       
        <% } %>

    </div>
    <script type="text/javascript" language="javascript">
        $(function () {
            $('#purchase-access').click(function () {
                $('#Productfrm').submit();
            });

        });
    </script>

</div>
</div>