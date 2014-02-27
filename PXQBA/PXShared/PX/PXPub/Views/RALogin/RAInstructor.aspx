<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ThreeColumn.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.RALogin>" %>


    
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
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
            , LocalProxyURL: '../../../RAg/RAgLocal.asmx'
            , LocalRAifURL: '../../../RAg/RAif.html'
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
	<script type="text/javascript" src="<%= Url.Content("../../Scripts/Rag/RAg.js") %>"></script>
	
	<script type="text/javascript" >
	    $(document).ready(function() {
	        MySite_init();
	    });
    </script>
 </asp:Content> 

<asp:Content ID="Content5" ContentPlaceHolderID="CenterContent" runat="server">
 <div id="Div3" style="display:none;"></div>  
 <div id="Div4" style="margin-top:10px;"></div>
 <div id="Div5" class="MySite_module">
            <div><p>You are logged in as : <div id="MySite_RA_UserName"></div></p></div>
            <br />             
             <p>
<%           if (ViewData["courseid"] != null)
             {
                 if (ViewData["courseid"].ToString() != string.Empty || ViewData["accessType"].ToString() == "20" || ViewData["accessType"].ToString() == "40")
                 {                      
%>
                      <p style='color:Red'><b>You are not authorized to view this course.</b></p>
<%                    
                 }
                 else if (ViewData["courseid"].ToString() == string.Empty)
                 {
%>                     
                   <p>
                        <form id="AdopterForm">
                            <div id="MySite_UserInfo_AnonInner">
                                <fieldset>
                                <p>
                                <input type="submit" value="Upgrade Me to Adopter" />
                                </p>
                                </fieldset>
                            </div>
                        </form>
                        <form id="DemoForm">
                            <div id="Div1">
                                <fieldset>
                                <p>
                                <input type="submit" value="Show Demo" />
                                </p>
                                </fieldset>
                            </div>
                        </form>      
                   </p>
<%                 }
             }
             
%>           
            </p>   
            <br />          
            <p>
                <a href="JavaScript:MySite_RAif_init('dologout')">log out</a>
            </p>
  </div>
  <div id="login-status"></div>
  <div ID="Div7" runat="Server" style="display:none;"></div>
   
</asp:Content>
