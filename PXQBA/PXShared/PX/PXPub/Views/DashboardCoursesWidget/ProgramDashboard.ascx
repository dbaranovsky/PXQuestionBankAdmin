<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DashboardData>" %>
<style type="text/css">
#PX_Program_DashboardWidget #widgetBody{
    border: none;
    -moz-box-shadow: none;
    -webkit-box-shadow: none;
    background-image: none;
    padding-bottom: 30px;
}
#PX_Program_DashboardWidget .widgetHeader{
    border: none;
}
.widgetItem.PX_Program_DashboardWidget
{
    -webkit-box-shadow:none ;
}

#PX_Program_DashboardWidget.widgetItem
{
    
 border: none;
 height:100%;
}

#PX_PROGRAM_DASHBOARD{

width:90%;
margin:10px auto 0px auto;

}

#PX_PROGRAM_DASHBOARD #header #menu {

border-bottom:5px #BCBDC0 solid;
float:left;
width:100%
}
#PX_PROGRAM_DASHBOARD #header #menu li.first-item {
margin: 0px;

}

#PX_PROGRAM_DASHBOARD #header #menu li a{
font-weight:bold;
text-decoration:none;
}
#PX_PROGRAM_DASHBOARD #header #menu .active-menu-item{

float: left;
margin: 0px 0px 0px 5px;
background-color: #BCBDC0;
min-width: 100px;
text-align: center;
padding: 10px;
padding-top: 15px;
}
#PX_PROGRAM_DASHBOARD #header #menu .menu-item{

float: left;
margin: 0px 0px 0px 5px;
background-color: #FFF;
min-width: 100px;
text-align: center;
padding: 10px;

border-top: 5px solid #BCBDC0;
border-right: 5px solid #BCBDC0;
border-left: 5px solid #BCBDC0;
border-bottom: none;
}

#PX_PROGRAM_DASHBOARD #term-selector{

float:left;
margin-top:10px;
width: 100%;
}

#PX_PROGRAM_DASHBOARD #widgetBody{
    border: none;
    -moz-box-shadow: none;
    -webkit-box-shadow: none;
    background-image: none;
    padding-bottom: 30px;
}
.widgetItem.PX_CarouselWidget
{
    -webkit-box-shadow:none ;
}

#PX_PROGRAM_DASHBOARD.widgetItem
{
    
 border: none;
 height:100%;
}


    
    #PX_PROGRAM_DASHBOARD .dashboardgrid
    {
        margin: auto;
        width: 100%;
        border-collapse: collapse;
		float:left;
		margin-top: 15px;
    }
    
    #PX_PROGRAM_DASHBOARD .dashboardgrid tr
    {   padding:0px;
        text-align: left;
    }
    #PX_PROGRAM_DASHBOARD .dashboardgrid th
    {    
    font-weight: normal;
    padding-bottom:5px;
    text-align: left;
    text-transform: uppercase;
        border-bottom: 1px solid #767E68;
    }
    #PX_PROGRAM_DASHBOARD .dashboardgrid td
    {
        border-bottom: 1px dashed #767E68;
        text-align: left;
        min-width: 100px;
        width: auto;
        padding:0px;
        vertical-align: bottom;
    }
    #PX_PROGRAM_DASHBOARD .dashboardgrid .status-cell
    {
   border-left: 1px solid #767E68;
    border-right: 1px solid #767E68;
    min-width: 50px;
    padding: 5px 5px 5px 5px;
    width: 10%;
    }
    #PX_PROGRAM_DASHBOARD .dashboardgrid .header-titles
    {   padding-left:5px;
        min-width: 50px;
    }
    #PX_PROGRAM_DASHBOARD .dashboardgrid .enrollment-count-cell
    {   width: 20%;
        padding: 5px 5px 5px 5px;
    }
    #PX_PROGRAM_DASHBOARD .dashboardgrid .title-cell
    {
    padding: 5px 5px 5px 0px;
    text-decoration: none; 
    }
        #PX_PROGRAM_DASHBOARD .dashboardgrid .title-cell a
    {
    color:#003D8B;
    text-decoration: none;
    float:left
    }
    #PX_PROGRAM_DASHBOARD #widget-title
    {
        color: #58595B;
        float:left;
      
    }
    #PX_PROGRAM_DASHBOARD .my-courses
    {
        border-top: 5px solid #7A8569;
        color: #7A8569;
        padding-bottom: 20px;
        padding-top: 6px;
    }   
    #PX_PROGRAM_DASHBOARD #create-course
    {
        
     float:left;
        
    }
    
    #PX_PROGRAM_DASHBOARD #dashboard-header
    {
        float: left;
width: 100%;
        margin-bottom: 20px;
         margin-top: 30px;
    }
    
    #PX_PROGRAM_DASHBOARD .show-creation-button{

        background-color: #E6E7E8;
        color: #414142;
        text-decoration: none;
        font-weight: bold;
        margin: 20px 0px 0px 0px;
        padding: 5px;
        font-size: 12px;
        float: left;
        border: 1px solid #58595B;
    }
   #PX_PROGRAM_DASHBOARD .dashboardgrid .enrollment-count-cell .show-url-delete
   {
           display:none;   
    float:right;
       text-decoration:none;
       color: #003D8B;
   }
    
   #PX_PROGRAM_DASHBOARD .dashboardgrid .title-cell .show-url-hover
   {
    display:none;   
    float:right;
   }

   #PX_PROGRAM_DASHBOARD .dashboardgrid .title-cell .show-url
   {
    display:none;   
    float:left;
    padding-top: 5px;
   }
   
    #PX_PROGRAM_DASHBOARD .dashboardgrid .title-cell .show-url
   {
    display:none;   
    float:left;
   }






</style>

<div id="PX_PROGRAM_DASHBOARD">

	<div id="header">

		<div id="menu">
		<ul>
		<li class="first-item menu-item"><a>Courses</a></li>
		<li class="active-menu-item"><a>Templates</a></li>
		<li class="menu-item"><a>Assesment</a></li>
		</ul>
		</div>



	</div>
	
<div id="term-selector">
    <span><b>Instructor e-Portfolios</b></span>
    <select id="academicTerms">
    
        <option value="2349a155-7a5c-4788-880f-f0b6fcb1ff8f" selected="selected">
            Current Academic Term
        </option>
    
        <option value="5349a155-7a5c-4788-880f-f0b6fcb1ff8f">
            Winter 2013
        </option>
    
    </select>
</div>

<table class="dashboardgrid">
    <tbody>

        <tr>
            <th>
                Course Name
            </th>
            <th class="header-titles">
                Owner
            </th>
			   <th class="header-titles">
                
                Status
            </th>
            <th class="header-titles">
                Students
            </th>
        </tr>         
        <%foreach (DashboardItem i in Model.InstructorCourses)
          { %>
        <tr class="entityidofcourse" data-dw-id="gff" >
            <td class="title-cell">
               <a href="<%= Model.CourseUrl %><%=i.CourseId %>"><%= i.CourseTitle%></a>
               
                <a href="JavaScript:void(0);" class="show-url-hover">Show URL</a><br />
                <a href="" class="show-url course-url" target="_blank"></a>
            </td>
			 <td class="status-cell">
               <%= i.OwnerName %>
            </td>
            <td class="status-cell">
               <%= i.Status %>
            </td>
            <td class="enrollment-count-cell">
                <%= i.Count %>
                 <a href="JavaScript:void(0);" class="show-url-delete">Delete Course</a><br />
            </td>
        </tr>
        <%} %>
             
    </tbody>
</table>

 <div id="create-course">
                <%
           
                    var lnkHtml = Html.ActionLink("Create", "ShowCreateCourse", "Course", null, new { @class = "show-creation-button fne-link fixed", title = "" });
            %> 
            

        <%=lnkHtml %></div>
</div>
</div>
