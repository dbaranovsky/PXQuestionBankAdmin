<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="WebApplicationTest._default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="smTest" runat="server" />
                    <asp:HiddenField ID="hfSTAttempts" runat="server" Value="0" />
                                    <asp:HiddenField ID="hfDSAttempst" runat="server" Value="0" />
    <div>
        <div style="float: left; width: 250px; padding-right: 10px">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                Number of Requests&nbsp;<asp:TextBox ID="txtNumStreamRequests" runat="server" Width="100px" />
                Number of Attempts:&nbsp;<asp:Label ID="lblAttemptsStream" runat="server" /><br />
                    <asp:Button ID="btnTestStream" runat="server" Text="Begin Stream Test" OnClick="btnTestStream_Click" />
                    <br />
                                        <asp:Label ID="lblStream" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div style="float: left; width: 250px; padding-right: 10px">
            <asp:UpdatePanel ID="upDS" runat="server">
                <ContentTemplate>
                Number of Requests&nbsp;<asp:TextBox ID="txtNumDSRequests" runat="server" Width="100px" />
                Number of Attempts:&nbsp;<asp:Label ID="lblAttemptsDS" runat="server" /><br />
                    <asp:Button ID="btnTestDS" runat="server" Text="Begin XDocument Test" OnClick="btnTestDS_Click" />
                    <br />
                                        <asp:Label ID="lblDS" runat="server" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    </form>
</body>
</html>
