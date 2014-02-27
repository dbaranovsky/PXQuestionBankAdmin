<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.GroupSet>" %>

<h2><%= Model.Name %></h2>
<% if (!Model.Groups.IsNullOrEmpty())
   {
       foreach (var group in Model.Groups)
       {
           var mailLink = String.Format("mailto:{0}", group.Members.Fold(";", m => m.Email));
           %>
            <div class="group">
                
                        <table class="students">
                            <thead>
                                <tr class="groupname" >
                                    <td colspan="1"> <%= group.Name %> </td>
                                    <td colspan="2">
                                    <% if (!group.Members.IsNullOrEmpty()) { %>
                                        <a class="email-link" href="<%= mailLink %>">E-mail <%= Model.Name == GroupSet.AllStudentsName ? "All" : "Group" %></a>
                                    <% } %>
                                    </td>
                                </tr>
                                <% if (!group.Members.IsNullOrEmpty()) {  %>
                                <tr>
                                    <th class="name">Name:</th>
                                    <th class="email">E-mail:</th>
                                    <th class="login">Last Login:</th>
                                </tr>
                                <% } %>
                            </thead>
                            <tbody>
                            <% if (!group.Members.IsNullOrEmpty())
                                { 
                                 foreach (var member in group.Members)
                                   { %>
                                    <tr>
                                        <td class="name"><%= String.Format("{1}, {0}", member.FirstName, member.LastName)%></td>
                                        <td class="email"><a href="mailto:<%= member.Email %>"><%= member.Email %></a></td>
                                        <td class="login" style="color: #999;"><%= member.LastLoginDescription%></td>
                                    </tr>
                                <% } 
                                }
                                else
                                { %>
                                    <tr>
                                        <td colspan="3"><div class="no-students-msg">This group does not have any students</div></td>
                                    </tr>                                    
                            <% } %>
                            </tbody>
                        </table>
            </div>
    <% }
   }
   else
   { %>
        <span class="no-groups-msg">This group set does not contain any groups.</span>
<% } %>