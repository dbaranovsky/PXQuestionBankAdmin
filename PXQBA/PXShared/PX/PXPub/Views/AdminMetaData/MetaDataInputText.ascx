<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.MetaDataElement>" %>

<input type="text" value="<% =Model.ElasticData.InternalValue %>" id="<% =Model.Name%>"  name="<% =Model.Name%>"/>