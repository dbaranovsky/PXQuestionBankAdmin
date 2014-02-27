<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.QuestionEditor.Models.HTSModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    HTS Editor
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%

        var s = Model;
        
    %>
    <div class="customquestion-properties-section-hts">
        <a href="#" class="customquestion-properties-hts">Properties</a>
    </div>
    <input id="txtMetaTitle_customComponent" type="text" style="display:none" />
    <div id="customquestion-meta-title-dialog-hts">
        <div class="custom-question-wrap">
            <div class="x-form-item " tabindex="-1">
                <label for="qmeta_title_customeditorcomponent-hts" style="width:49px;" class="x-form-item-label">title:</label>
            
                <input type="text" size="20" autocomplete="off" id="qmeta_title_customeditorcomponent-hts" name="qmeta-title" style="width: 409px;">
            
            </div>
        </div>
    </div>

    <div id="question-editor" hts-data-editorurl='<%= Model.EditorUrl %>'
        hts-data-equationimageurl='<%= Model.EquationImageUrl%>' style="display: none;">
        <div id="text-editor-wrapper" class="defaultSkin">
        </div>
        <div class="content-wrapper">
            <table class="content-wrapper-table">
                <tr>
                    <td class="content-wrapper-table-left">
                        <div class="content-left">
                            <div id="html-view">
                                <div class="pane steps">
                                </div>
                            </div>
                            <div id="xml-view" style="display:none;">
                                <div class="node raw">
                                    <div class="title">
                                        <span>Raw XML Editor</span>
                                    </div>
                                    <div class="node-body">
                                        <textarea rows="3" cols="3" id="Textarea2" spellcheck="false" ></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </td>
                    <td class="content-wrapper-table-right">
                        <div class="content-right">
                            <% Html.RenderPartial("VariablesSidePanel"); %>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <% Html.RenderPartial("EditorBottomMenu"); %>
    </div>
    <% Html.RenderPartial("StepTemplates"); %>
    <% Html.RenderPartial("DialogTemplates"); %>
    <script src="<%: Url.Content("~/Scripts/tiny_mce/plugins/hts/js/helper.js") %>" type="text/javascript"></script>    
    <script type="text/javascript">
        (function ($) {
            $(document).ready(function () {
            var args = '<%=  Model.HTSJsonResult%>';
            var argsCollection = jQuery.parseJSON(args);
            $('#question-editor').QuestionEditor(argsCollection);
                <% 
        if (!Model.InvalidParams)
        {
       
    %>
                $('#question-editor').QuestionEditor("build");
                <% } %>
            });
        } (jQuery))

    </script>
    
</asp:Content>
