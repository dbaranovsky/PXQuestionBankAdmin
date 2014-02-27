<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BltiLaunch.aspx.cs" Inherits="Bfw.BLTIProvider.BltiLaunch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<form id="form1" runat="server"> 
   <h2>
        Welcome to BLTI TEST LAUNCH application
    </h2>
    
   
    <table width="100%">
        <tr>
            <td style="width: 15%">Launch URL:</td>
            <td style="width: 30%">
                <asp:TextBox ID="txtLaunchURL" Width="100%" runat="server">http://localhost:4207/universe9e/portal/117644/Home#/launchpad</asp:TextBox>
            </td>
           <td style="width: 55%"></td>
        </tr>
         <tr>
            <td>Key:</td>
            <td>
                <asp:TextBox ID="txtKey" Width="100%" runat="server">key</asp:TextBox>
             </td>
        </tr>
         <tr>
            <td>Secret:</td>
            <td>
                <asp:TextBox ID="txtSecret" Width="100%" runat="server">secret</asp:TextBox>
             </td>
        </tr>
       
         <tr>
            <td>resource_link_id:</td>
            <td>
                <asp:TextBox ID="txtResourceLinkId" Width="100%"  runat="server">120988f929-274612</asp:TextBox>
             </td>
               <td>
                <asp:CheckBox ID="chkResourceLinkId" runat="server" Checked="True"  />
               </td>
        </tr>
         <tr>
            <td>resource_link_title:</td>
            <td>
                <asp:TextBox ID="txtResourceLinkTitle" Width="100%" runat="server"></asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkResournceLinkTitle" runat="server" />
               </td>
        </tr>
         <tr>
            <td>resource_link_description:</td>
            <td>
                <asp:TextBox ID="txtResourceLinkDescription" Width="100%"  runat="server"></asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkResourceLinkDescription" runat="server" />
               </td>
        </tr>
         <tr>
            <td>user_id:</td>
            <td>
                <asp:TextBox ID="txtUserId" Width="100%"  runat="server"></asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkUserId" runat="server" Checked="True" />
               </td>
        </tr>
		
		
		<tr>
            <td>RA_user_id:</td>
            <td>
                <asp:TextBox ID="txtRaUserId" Width="100%"  runat="server">8</asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkRaUserID" runat="server" Checked="True" />
               </td>
        </tr>

		<tr>
            <td>Px_user_id:</td>
            <td>
                <asp:TextBox ID="txtPxUserId" Width="100%"  runat="server">87414</asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkPxUserID" runat="server" Checked="True" />
               </td>
        </tr>


         <tr>
            <td>roles:</td>
            <td>
                <asp:TextBox ID="txtRoles" Width="100%" runat="server"></asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkRoles" runat="server"  />
               </td>
        </tr>
         <tr>
            <td>lis_person_name_full:</td>
            <td>
                <asp:TextBox ID="txtLisPersonNameFull" Width="100%"  runat="server"></asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkLisPersonNameFull" runat="server" />
               </td>
        </tr>
         <tr>
            <td>lis_person_contact_email_primary:</td>
            <td>
                <asp:TextBox ID="txtLisPersonEmailPrimary" Width="100%"  runat="server"></asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkLisPersonEmailPrimary" runat="server"  />
               </td>
        </tr>
         <tr>
            <td>lis_person_sourcedid:</td>
            <td>
                <asp:TextBox ID="txtLisPersonSourceId" Width="100%"  runat="server"></asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkLisPersonSourceId" runat="server"  />
               </td>
        </tr>
         <tr>
            <td>context_id:</td>
            <td>
                <asp:TextBox ID="txtContextId" Width="100%" runat="server">A_16</asp:TextBox>
             </td>
               <td>
                <asp:CheckBox ID="chkContextId" runat="server"  Checked="True"/>
               </td>
        </tr>
         <tr>
            <td>context_title:</td>
            <td>
                <asp:TextBox ID="txtContextTitle" Width="100%" runat="server">lms middleware</asp:TextBox>
             </td>
               <td>
                <asp:CheckBox ID="chkContextTitle" runat="server"  Checked="True"/>
               </td>
        </tr>
         <tr>
            <td>context_label:</td>
            <td>
                <asp:TextBox ID="txtContextLabel" Width="100%" runat="server">lms middleware</asp:TextBox>
             </td>
               <td>
                <asp:CheckBox ID="chkContextLabel" runat="server"  Checked="True"/>
               </td>
        </tr>
         <tr>
            <td>launch_presentation_return_url</td>
            <td>
                <asp:TextBox ID="txtLaunchPresentReturnURL" Width="100%"  runat="server">http://dev-lmslink.bfwpub.com</asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkLaunchPresentReturnURL" runat="server" Checked="True" />
               </td>
        </tr>
         <tr>
            <td>tool_consumer_instance_guid:</td>
            <td>
                <asp:TextBox ID="txtToolConsumerGUID" Width="100%"  runat="server">ebooks</asp:TextBox>
             </td>
               <td>
                <asp:CheckBox ID="chkToolConsumerGUID" runat="server" Checked="True"  />
               </td>
        </tr>
        <tr>
            <td>tool_consumer_instance_name:</td>
            <td>
                <asp:TextBox ID="txtToolConsumerName" Width="100%"  runat="server">ebooks</asp:TextBox>
            </td>
             <td>
                <asp:CheckBox ID="chkToolConsumerName" runat="server" Checked="True" />
               </td>
        </tr>
         <tr>
            <td>tool_consumer_instance_contact_email:</td>
            <td>
                <asp:TextBox ID="txtToolConsumerContactEmail" Width="100%" runat="server">techsupport@macmillan.com</asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkToolConsumerContactEmail" runat="server" Checked="True" />
               </td>
        </tr>
         <tr>
            <td>tool_consumer_instance_description:</td>
            <td>
                <asp:TextBox ID="txtToolConsumerDescription" Width="100%" runat="server"></asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkToolConsumerDescription" runat="server"  />
               </td>
        </tr>
        
         <tr>
            <td>lti_message_type</td>
            <td>basic-lti-launch-request</td>
              <td>
                <asp:CheckBox ID="chkLTIMessage" runat="server" Checked="True"   />
               </td>
        </tr>
         <tr>
            <td>lti_version</td>
            <td>LTI-1p0</td>
               <td>
                <asp:CheckBox ID="chkLTIVersion" runat="server"  Checked="True" />
               </td>
        </tr>
          <tr>
            <td>oauth_version:</td>
            <td>1.0</td>
        </tr>
         <tr>
            <td>oauth_callback:</td>
            <td>about:blank</td>
              <td>
                <asp:CheckBox ID="chkOAuthCallback" runat="server" Checked="True" />
               </td>
        </tr>
        
         <tr>
            <td>oauth_signature_method:</td>
            <td>HMAC-SHA1</td>
        </tr>
					
         <tr>
            <td>bookId:</td>
            <td>
                <asp:TextBox ID="txtBookId" Width="100%" runat="server">henretta7e</asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkBookId" runat="server" Checked="True" />
               </td>
        </tr>
        
         <tr>
            <td>baseURL:</td>
            <td>
                <asp:TextBox ID="txtBaseURL" Width="100%" runat="server">dev-ebooks.bfwpub.com/henretta7e</asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkBaseURL" runat="server" Checked="True"/>
               </td>
        </tr>
         <tr>
            <td>uid:</td>
            <td>
                <asp:TextBox ID="txtUID" Width="100%" runat="server">6668608</asp:TextBox>
             </td>
              <td>
                <asp:CheckBox ID="chkUID" runat="server"  Checked="True"/>
               </td>
        </tr>
    </table>

    <asp:Button ID="Button1" runat="server" Text="Submit" onclick="Button1_Click" />
    <table>
        <tr>
            
            <td><asp:Label ID="lblSignatureGenerated" runat="server" Text=""></asp:Label>
            <br/>
            <asp:Label ID="lblBaseSiganture" runat="server" Text=""></asp:Label>
            </td>
        </tr>

    </table>
   
    
   
  
      </form>
       <asp:Literal runat="Server" id="ResponseResult" />
</body>
</html>
