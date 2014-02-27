<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignedItem>" %>
<% var isItemLocked = Model.IsItemLocked ? "disabled" : ""; %>

<select <%= isItemLocked %> id="selgradebookweights" name="selgradebookweights" class="selgradebookweights" style="float: none;" onchange="ContentWidget.IsFormChanged()">
    <option value="createNewCat">Create new category</option>

    <% 
        Model.Category = string.IsNullOrEmpty(Model.Category) ? "0" : Model.Category; 
    
        var originalVal = "";
        if (Model.GradeBookWeights.GradeWeightCategories != null)
        {
            foreach (var filter in Model.GradeBookWeights.GradeWeightCategories.OrderBy(i => int.Parse(i.Id)))
            {
                if (filter.Id == Model.Category)
                {
                    originalVal = filter.Text;
    %>

    <option value="<%= filter.Id %>" selected="selected"><%= filter.Text%></option>

    <%
                }
                else
                { 
    %>

    <option value="<%= filter.Id %>"><%= filter.Text%></option>

    <% 
                }
            }
        }
    %>
    
</select>

<input type="hidden" id="selgradebookweightsHidden" value="<%=originalVal %>" class="selgradebookweightsHidden" />
<a href="#" id="ggbcategorydialog_link" class="link-list ggbcategorydialog_link" style="display:none;" onclick="ContentWidget.AddNewCategoryClick(event);">Add New</a>

