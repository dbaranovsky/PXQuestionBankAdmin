<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.MetaDataElement>" %>
<%@ Import Namespace="Bfw.Common.DynamicExtention" %>

<% 
   ElasticObject elasticContorl = Model.ElasticControl;

   if (elasticContorl!=null)
   {  	   
	%>    
    
	<% = elasticContorl.ToXElement() %>

	<%
    }
   else
   {
	   //TODO: work directly with Model.ElasticData
   }
  %>
  


