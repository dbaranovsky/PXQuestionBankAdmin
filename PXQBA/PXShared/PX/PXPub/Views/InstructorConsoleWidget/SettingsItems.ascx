<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.Biz.DataContracts.InstructorConsoleSettings>" %>

<%
    if (Model.ShowGeneral)
    {         
%>
    <li class="link"><a href="#state/instructorconsole/general" class="link">General</a></li>
<%        
    }    
%>
<%
    if (Model.ShowNavigation)
    {         
%>
    <li class="link"><a href="#state/instructorconsole/navigation" class="link">Navigation</a></li>
<%        
    }    
%>
<%
    if (Model.ShowLaunchPad)
    {         
%>
    <li class="link"><a href="#state/instructorconsole/launchpad" class="link">Launch Pad</a></li>
<%        
    }    
%>
