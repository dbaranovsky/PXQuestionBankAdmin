<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.GradebookPreferences>" %>

<table class="gradebookCat-table">
    <thead>
        <tr>
            <td class="sequence">Sequence | Title</td>
            <td>Drop Lowest</td>
            <td>Weight/Points</td>
            <td>% of Total</td>
            <td>&nbsp;</td>
        </tr>
    </thead>
    <tbody>
        <%
            int i = 1;
            List<SelectListItem> categorySequence = new List<SelectListItem>();

            foreach (var category in Model.GradeBookWeights.GradeWeightCategories)
            {
                categorySequence.Add(new SelectListItem() { Text = i.ToString(), Value = category.Id });
                i++;
            }

            i = 1;
            
            foreach (var category in Model.GradeBookWeights.GradeWeightCategories)
            {
                categorySequence.Find(o => o.Value.Equals(category.Id)).Selected = true;                                
        %>
                <tr>
                    <td><%= Html.DropDownList("categorySequence_" + category.Id, categorySequence, new { @class = "categorySequence", sequence = category.Sequence }) + "&nbsp;<span class='" + (category.Text.Equals("Uncategorized") ? string.Empty : "labelName") + "' style='cursor: pointer'>" + category.Text + "</span><input type='text' class='textName' size='100' style='display: none' /><input type='button' class='btnName' value='OK' style='display: none' />"%></td>
                    <td class="drop"><%= "<span class='labelDrop' style='cursor: pointer;'>" + category.DropLowest + "</span><input type='text' class='textDrop' size='5' style='display: none' /><input type='button' class='btnDrop' value='OK' style='display: none' />"%></td>
                    <td class="weight"><%= Model.UseWeightedCategories ? "<span id='labelWeight_" + category.Id + "' class='labelWeight' style='cursor: pointer'>" + category.Weight + "</span><input type='text' class='textWeight' size='5' style='display: none' /><input type='button' class='btnWeight' value='OK' style='display: none' />" : "" + category.ItemWeightTotal %></td>
                    <td class="percent"><%= Math.Round(Double.Parse(category.Percent) * 100, 2).ToString("0.00") %>%</td>
                    <td><%= category.Text.Equals("Uncategorized") ? string.Empty : "<input type=\"button\" value=\"Remove\" class=\"removeCategory\" />" %></td>
                </tr>                
        <%  
                int j = 1;
                List<SelectListItem> itemSequence = new List<SelectListItem>();

                foreach (var item in category.Items)
                {
                    itemSequence.Add(new SelectListItem() { Text = j.ToString(), Value = item.Id });
                    j++;
                }

                j = 1;                
                
                foreach (var item in category.Items)
                {
                    if (item.Weight > 0)
                    {
                        var fnelink = item.level == 1 ? 
                            item.Title.Truncate("...", 0, 100) :
                            Url.GetComponentLink(item.Title.Truncate("...", 0, 100), "item", item.Id, new
                            {
                                mode = ContentViewMode.Assign,
                                includeDiscussion = false,
                                renderFNE = true
                            });

                        itemSequence.Find(o => o.Value.Equals(item.Id)).Selected = true;
        %>                    
                <tr style="display:none" class='category-items <%= j % 2 == 0 ? "even" : "odd" %>'>
                    <td class="sequence"><%= Html.DropDownList("itemSequence_" + item.Id, itemSequence, new { @class = "itemSequence", sequence = item.CategorySequence, category = category.Id }) + "&nbsp;" %><%= fnelink %></td>
                    <td><%= ""%></td>
                    <td class="weight"><%= item.Weight%></td>
                    <td class="percentage"><%= Math.Round(item.Percent * 100, 2).ToString("0.00")%>%</td>
                    <td><%= ""%></td>
                </tr>                
        <%            
                        j++;
                    }
                }                         
            
                i++;     
            }
        %>
    </tbody>
</table>

