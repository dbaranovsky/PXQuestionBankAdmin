<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.MenuItem>" %>

<li class="activeMenuItems" id="<%=Model.Id%>" style="float:left;" >
<%  var callBack = Model.Callbacks.First();
    var parameters = new RouteValueDictionary();
    var dictParam = new Dictionary<string, object>();        

    foreach (var param in callBack.Value.Parameters)
    {
        if (!param.Value.Contains("{") && !param.Value.Contains("}"))
        {
            parameters.Add(param.Key, param.Value);
        }
    }

    // Look for overrides and apply them if found
    if (!callBack.Value.InstructorOverride.IsNullOrEmpty() && ViewData["AccessLevel"].ToString() == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor.ToString())
    {
        try
        {
            string[] overrideVals = callBack.Value.InstructorOverride.Split('|');
            callBack.Value.Controller = overrideVals[0];
            callBack.Value.Action = overrideVals[1];
            callBack.Value.LinkType = overrideVals[2];
        }
        catch { }
    }
    if (!callBack.Value.StudentOverride.IsNullOrEmpty() && ViewData["AccessLevel"].ToString() == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student.ToString())
    {        
        try
        {
            string[] overrideVals = callBack.Value.StudentOverride.Split('|');
            callBack.Value.Controller = overrideVals[0];
            callBack.Value.Action = overrideVals[1];
            callBack.Value.LinkType = overrideVals[2];
        }
        catch { }
    }
    
    // handles the css class
    string menuCss = "";
    if (Model.IsActive)
    {
        menuCss += "active";
    }
    if (callBack.Value.LinkType.ToLowerInvariant() == "fne")
    {
        if (menuCss.Length > 0)
        {
            menuCss = menuCss + " ";
        }
        menuCss += "fne-link";
    }
    dictParam.Add("class", menuCss);

    if (!callBack.Value.Url.IsNullOrEmpty())
    {%>
        <a href="<%=callBack.Value.Url%>"><span><%=Model.Title%></span></a>
    <%} else if (callBack.Value.RouteName.IsNullOrEmpty()) {

          var callbackUrl = Url.Action(callBack.Value.Action, callBack.Value.Controller);
          %>

          <a href="<%=callbackUrl%>" class="<%=menuCss %>"><span><%=Model.Title%></span></a>
    <%} else {
        var url = Url.RouteUrl(callBack.Value.RouteName, parameters); %>
        <a href="<%=url%>" class="<%=menuCss %>"><span><%=Model.Title%></span></a>
    <%} %>
                    
    <input type="hidden" id="Sequence" value="<%=Model.Sequence%>" />   
    <input type="hidden" id="BfwMenuCreatedby" value="<%=Model.BfwMenuCreatedby%>" /> 
    <%=Html.Hidden("VisibleByInstructor", Model.VisibleByInstructor, new { id = "VisibleByInstructor" })%>    
    <%=Html.Hidden("VisibleByStudent", Model.VisibleByStudent, new { id = "VisibleByStudent" })%>    
   <span class="removeMenuItem" style="position:relative;top:-5px;display:none;"></span>
</li>