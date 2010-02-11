<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.User" %>
<!--<%@ Register src="Controls/UserManager.ascx" tagname="UserManager" tagprefix="uc1" %> -->

<%@ Register src="Controls/RequestForm.ascx" tagname="RequestForm" tagprefix="uc2" %>





<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
   
    <script src="/scripts/jquery-1.3.2.js" type="text/javascript"></script> 
    <% if (false) { %> 
    <script src="/scripts/jquery-1.3.2-vsdoc.js"  type="text/javascript"></script> 
    <% } %>

    <!--<script src="Scripts/SNAPDefaultJQuery.js" type="text/javascript"></script> -->
     <script src="Scripts/jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>
     
</head>
<body>
    <form id="form1" runat="server">
      <uc2:RequestForm ID="RequestForm1" runat="server" /> 
      <br />
      <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Button" />
      <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>     
    </form>

    <script type="text/javascript">
        $(document).ready(DocReady);
    </script>
    
    </body>
</html>
