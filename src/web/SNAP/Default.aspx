<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SNAP._Default" %>
<%@ Register TagPrefix="uc1" TagName="ucUserDetails" Src="includes/ucUserDetails.ascx" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">

    <script src="/scripts/jquery-1.3.2.js" type="text/javascript"></script> 
    <% if (false) { %> 
    <script src="/scripts/jquery-1.3.2-vsdoc.js"  type="text/javascript"></script> 
    <% } %>

    <script type="text/javascript" >
        $(document).ready(function() {
        });
        
    </script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        <h3>Hello SNAP!</h3>
        <asp:linkbutton id="LogoutButton" runat="server" onclick="LogoutButton_Click">Logout</asp:linkbutton><br />
        <span>You are:</span><br />
        <asp:TextBox ID="txtUsrInfo" runat="server" Height="112px" TextMode="MultiLine" 
            Width="292px"></asp:TextBox>
         <asp:TextBox ID="txtGroupInfo" runat="server" Height="112px" TextMode="MultiLine" 
            Width="292px"></asp:TextBox>

        <h4>This is just to show how to get user details...some functionilities are not supported</h4>
        <uc1:ucUserDetails id='userDetail' runat='server' />         
        </div>
    </form>
</body>
</html>
