<%@ Control Language="C#"  Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.Note>>" %>


<script type="text/javascript" language="javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/jquery/jquery.validate.js") %>'], function () {                
            });
        });
    } (jQuery));
</script>

    <% if (Model.Count() > 0)
       {    
           foreach (var item in Model)
           {           
            %>
            <li class="noteli">   
                <span class="noteTitle">
                  <span class="noteicon"></span>
                    <%=Html.Encode(item.Title)%> :
                </span>
                <span class="noteTitleContent"> 
                    <%=Html.Encode(item.ShortText)%>
                </span>
                <a href="#" class="useText linkButton">USE</a>
                <div class="editableForm">      
                    <div class="formElements">                        
                            <%= Html.HiddenFor(m => item.NoteId)%>
                            <%= Html.HiddenFor(m => item.EntityId)%>
                            <%= Html.HiddenFor(m => item.Sequence)%>
                            <%= Html.HiddenFor(m => item.Text)%>
                            <%= Html.Hidden("shortTitle",item.Title)%>
                            

                            <form class="validationForm" method="post" action="">
                                <label for="noteTitle">
                                    Title:</label>
                                <input type="text" name="Title"  value="<%=Html.Encode(item.Title) %>" style="width: 98%" />                               
                                
                                <label for="noteText">
                                    Note:
                                </label>
                                <textarea style="width: 98%" name="Text" id="Text"><%=Html.Encode(item.Text)%></textarea>                                     
                                <div class="clear" />
                                <div class="saveAndSubmit">
                                    <input id="btnSubmit" type="submit" value="Save" />
                                    <input id="btnCancel" class="btnCancelClass" type="button" value="Cancel" />
                                </div>
                            </form>
                            <div class="clear" />                            
                    </div>
                </div>
            </li>  
            <%
           }
        }
        else
        {  %>
                <label for="noNotes">
                    Create a note by selecting 'Create New Note' in the gearbox.
                </label>
        <%} %>