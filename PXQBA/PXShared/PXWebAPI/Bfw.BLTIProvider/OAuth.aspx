<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OAuth.aspx.cs" Inherits="Bfw.BLTIProvider.OAuth" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
   <form id="form1" runat="server"> 
   <h2>
        Welcome to OAUTH TEST LAUNCH application
    </h2>
    
    
      
    <table width="100%">
        <tr>
            <td style="width: 15%">Launch URL:</td>
            <td style="width: 30%">
                <asp:TextBox ID="txtLaunchURL" Width="100%" runat="server">http://localhost:60078/Content/SearchPxTableofContents/117318?parentid=bsi__1146E2F7__A154__4828__999C__AA77B1599C55&filter=KEYWORD&term=Revolution&userRole=0</asp:TextBox>
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
            <td>oauth_version:</td>
            <td>1.0</td>
        </tr>
         <tr>
            <td>oauth_callback:</td>
            <td>about:blank</td>
              <td>
                <asp:CheckBox ID="chkOAuthCallback" runat="server" />
               </td>
        </tr>
        
         <tr>
            <td>oauth_signature_method:</td>
            <td>HMAC-SHA1</td>
        </tr>
         <tr>
            <td>HTTP method</td>
            <td>
                <asp:TextBox ID="txtHttpMethod" Width="100%"  runat="server">GET</asp:TextBox>
             </td>
              
        </tr>
        <tr>
            <td>Request body for POST</td>
            <td>
                <asp:TextBox ID="txtRequestBody" Width="100%"  runat="server"></asp:TextBox>
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



    
   
    
   
  <asp:Literal runat="Server" id="ResponseResult" />
      </form>
      
</body>
</html>
