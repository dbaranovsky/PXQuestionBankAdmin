<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.LearningCurveActivity>" %>
<%@ Import Namespace = "Bfw.PX.PXPub.Controllers.Helpers" %>

<%  
    var rand = new Random();
    var frameHostId = string.Format("lc-viewer-frame-host-{0}", rand.Next());    
    var mode = Context.Request.QueryString["mode"];
    //http://dev-learningcurve.bfwpub.com/index.php?st=200&enrollmentid=&itemid=bsi__47ADE45B__FFD6__4E6F__A8A0__30370FEC32DB&approot=http://bfwusers.bhdev.bedfordstmartins.com/brainhoney&disable_links=true&platform=px&reportingMode=arga&test_mode=true&st=20
    var proxyUrl = LearningCurveHelper.GetPlayerUrl(Model, mode);
%> 

<div id="<%= frameHostId %>" class="lc_iframe-host" rel="<%= proxyUrl %>" style="height:600px"></div>
    
    
