<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DSMLSample._Default" ValidateRequest="false" EnableViewState="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>DSML Test</title>
    <meta http-equiv="Pragma" content="no-cache" />
    <meta name="X-UA-Compatible" content="">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.6.2/jquery.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function() {
            $("#sRequestSample").bind("change", function() {
                $.ajax({
                    url: '/DSMLSample/SampleXML/' + $(this).val(),
                    cache: false,
                    method: "GET",
                    success: function (data) {
                        $('#dvRequest textarea').text(data);
                    },
                    error: function (textStatus) {
                        alert(textStatus);
                    }
                });
            });
        });
        
    </script>
    <style type="text/css">
        body
        {
            white-space:nowrap;
        }
        #taRequest {
            height: 396px;
            width: 422px;
        }
        #taResponse 
        {
            height: 396px;
            width: 422px;
        }
        pre
        {
            font-family:Verdana;
            font-size: 10px;
         }
         #dvRequestLeft
         {
                width:450px;
                float:left;
                padding-right:5px;
         }
         #dvRequestRight
         {
             width:300px;
             float:left;
             padding-left:5px;
         }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dvHeader">
        <h3>DSML Test Page</h3> <br />
        <h6>Target URL: <asp:label id="lblTarget" runat="server" />  </h6>
    </div>
    <div id="dvRequest">
            <div id="dvRequestLeft">
            <pre>Test Request:</pre>
            <textarea id="taRequest" runat="Server" cols="5"></textarea> 
            <br />
            <asp:Button ID="btnGo" runat="Server" Text="Go" onclick="btnGo_Click" />
            </div>
            <div id="dvRequestRight">
                <select id="sRequestSample">
                    <option selected="selected" title="Test Connection" value="TestConnection.xml">Test Connection</option>
                    <option title="Add Request" value="AddRequest.xml">Add Request</option>
                    <option title="Delete Request" value="DelRequest.xml">Delete Request</option>
                    <option title="ModDnRequest" value="ModDnRequest.xml">ModDn Request</option>
                    <option title="ModifyRequest" value="ModifyRequest.xml">Modify Request</option>
                    <option title="SearchRequest" value="SearchRequest.xml">Search Request</option>
                </select>
            </div>
    </div>
    <br />
    <div style="clear:both">
    </div>
    <div id="dvResponse">
            <pre>Response:</pre>
            <textarea id="taResponse" runat="Server" cols="5"></textarea>
            <br />
    </div>
    </form>
</body>
</html>
