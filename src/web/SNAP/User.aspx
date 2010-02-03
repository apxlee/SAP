<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="SNAP.User" %>
<%@ Register src="Controls/UserManager.ascx" tagname="UserManager" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
   
    <script src="/scripts/jquery-1.3.2.js" type="text/javascript"></script> 
    <% if (false) { %> 
    <script src="/scripts/jquery-1.3.2-vsdoc.js"  type="text/javascript"></script> 
    <% } %>

     
</head>
<body>
    <form id="form1" runat="server">
      <h3>Test!</h3>
      <uc1:UserManager ID="UserManager1" runat="server" />
      
      <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Button" />
      <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
      
    </form>
    
    <script type="text/javascript">
      $(document).ready(DocReady);
    </script>

</body>
</html>
