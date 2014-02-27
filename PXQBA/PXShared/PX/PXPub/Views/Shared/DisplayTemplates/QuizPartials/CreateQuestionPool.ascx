<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>
   
    <%= Html.HiddenFor(m => m.Id) %>
    <%= Html.HiddenFor(m => m.ParentId) %>
    <%= Html.HiddenFor(m => m.Type) %>
    <%= Html.HiddenFor(m => m.Url) %>
    <%= Html.HiddenFor(m => m.IsAssignable) %>
    <%= Html.HiddenFor(m => m.Sequence)%>
    <%= Html.HiddenFor(m => m.QuizType) %>
    <div class="pool-title-info">
        <div style="float:left;padding-right:20px;padding-top:5px;">
            <span class="title">Title</span>
        </div>
        <div>
            <input id="pool_title" name="Title"  type="text" style="width: 237px;"/>                
            <span style="display:none" class="field-validation-error pool_title_error px-default-text" >Please enter the title.</span>
        </div>        
    </div>

    <div class="pool-question-info">
        <div style="float:left;padding-right:10px;padding-top:5px;">
            <span class="info">
                Pull how many questions from the pool?
            </span>
        </div>
        <div>
            <input id="pool-count" name="txt-pool-count" type="text" maxlength="3" style="width:40px" />
            <span id="spn-pool-count-error"  style="display:none"  class="field-validation-error px-default-text">Please enter a valid pool count.</span>
            <span id="spn-pool-integer-error"  style="display:none"  class="field-validation-error px-default-text">Please enter a numeric value.</span>        
        </div>
    </div>


