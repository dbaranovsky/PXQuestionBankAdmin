<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>

<%
    Html.RenderPartial("BhIFrameComponent", new BhComponent()
                    {
                        ComponentName = "ItemEditor",
                        DomainUserSpace = "Model.DomainUserSpace",
                        Parameters = new
                                        {
                                            EnrollmentId =
                            Model.EnrollmentId,
                                            Id = "itemeditor",
                                            ItemId = Model.Id,
                                            GroupId = "",
                                            ShowBeforeUnloadPrompts = false,
                                            ShowOnlyProperties = true
                                        }
                    }); 
%>
