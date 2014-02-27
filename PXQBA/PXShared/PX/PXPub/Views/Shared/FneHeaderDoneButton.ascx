<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%@ Import Namespace="Bfw.PX.Biz.ServiceContracts" %>
<% 
    
    var doneUrl = Url.GetComponentHash("item", Model.Path,
                                new
                                    {
                                        mode = ContentViewMode.Preview,
                                        includeNavigation = true,
                                        isBeingEdited = false,
                                        renderFne = true,
                                        toc = Model.Toc
                                    }); %>
<!-- Done button -->
<a itemurl="<%=doneUrl %>" href="javascript:" id="fne-done" class="show-faceplate-home-icon faceplate-fne-home-icon fne-done-link">
    <span class="doneEditing-btn-icon"></span>Done</a>