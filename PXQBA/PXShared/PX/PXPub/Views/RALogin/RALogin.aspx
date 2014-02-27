<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ThreeColumn.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.RALogin>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
    <link href ="<%= Url.Content("../../Rag/css/My_Site.css") %>" rel="stylesheet" type="text/css" />
    <link href= "<%= Url.Content("../../Rag/css/RAg.css") %>" rel="stylesheet" type="text/css" />
	
	<script type="text/javascript">
	    var MySite_CtrlWin = window.self;
	    var MySite_PageWin = null;
	    var RA_CtrlWin = window.self;	   
	</script>
	
	<%
        if (ViewData["scripts"] != null)
        {                      
            foreach (string src in (string[])ViewData["scripts"])
            {                         
    %>
                <script type="text/javascript" src="<%=src%>" ></script>
    
	<%
            }
        }   
    %>  

    <link type="text/css" href= "<%= ViewData["RAcss"].ToString()%>"  rel="stylesheet"  />
	<script type="text/javascript">
	   
	    RA_CtrlWin.RA.setOptions({
	        ProxyType: 'ASP.NET'
            , LocalProxyURL: '/RAg/RAgLocal.asmx'
            , LocalRAifURL: '/RAg/RAif.html'
            , UseIFrame: false
	    });
    </script>
    <script type="text/javascript">
       
        var popupsblocked = false;
        var cookiesblocked = !navigator.cookieEnabled;
        if (!cookiesblocked) {
            RA_CtrlWin.RA.RAXS.Check();
        }       
	</script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/Rag/RAg.js") %>"></script>
	
	<script type="text/javascript" >
	    jQuery(document).ready(function() {
	        MySite_init();
	    });
    </script>
    
 </asp:Content>
 <asp:Content ID="Content5" ContentPlaceHolderID="CenterContent" runat="server">

    <div id="MySite_page_grayed" style="display:none;">
    </div>   
    
    <form id="LoginForm" action="JavaScript:MySite_RA_doLogin();">
        <div id="MySite_UserInfo_AnonInner">
            <fieldset>
                <legend>Account Information</legend>
                
                <div >
                    <%= Html.LabelFor(m => m.RA_Email) %>
                </div>
                <div>
                    <%= Html.TextBoxFor(m => m.RA_Email) %>
                    <%= Html.ValidationMessageFor(m => m.RA_Email) %>
                </div>
                
                <div >
                    <%= Html.LabelFor(m => m.RA_Password) %>
                </div>
                <div >
                    <%= Html.PasswordFor(m => m.RA_Password)%>
                    <%= Html.ValidationMessageFor(m => m.RA_Password)%>
                </div>
                <p>
                    <input type="submit" value="Log in" />
                </p>
            </fieldset>
        </div>
    </form>    
        
    <div id="RAif_div" style="margin-top:10px;"></div>
      <%--<div id="MySite_UserInfo_LoggedIn" class="MySite_module">
            <p><b>RA Logged-in user info and options</b></p>
            <div id="MySite_RA_UserName"></div>
            <div id="MySite_RA_UserAccess"></div>
            <div id="MySite_UserInfo_LoggedInInner"></div>
            <p>
                <a href="JavaScript:MySite_RAif_init('dologout')">log out</a>
            </p>
            
            <p>
            Do you want to continue as a <%=Html.ActionLink("Student", "RAStudent")%> or an <%=Html.ActionLink("Instructor", "RAInstructor")%>?
            </p>                
        </div>--%>
       
      <div ID="RAServerOutput" runat="Server" style="display:none;"></div>
   
</asp:Content>
