<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="SNAP.User" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <!-- <script src="http://code.jquery.com/jquery-latest.js"></script> -->
    
    <script src="/scripts/jquery-1.3.2.js" type="text/javascript"></script> 
    <% if (false) { %> 
    <script src="/scripts/jquery-1.3.2-vsdoc.js"  type="text/javascript"></script> 
    <% } %>

    <script src='/scripts/user.js' type="text/javascript" ></script>
    
    
    <script type="text/javascript">
      $(document).ready(DocReady);
    </script>
     
</head>
<body>
    <form id="form1" runat="server">
      <h3>Test!</h3>
      Name: <input type="text" name="userName" id='userName'/>
      <button type='button' id='userCheck'>check</button>
      <br />
      <select size="3" name="nameSelection" id="nameSelection">
      </select>
      <br />
      Manager Name: <input type="text" name="mgrName" id='mgrName'/>
      <button type='button' id='mgrCheck'>check</button>
      <br />
      <select size="3" name="mgrSelection" id="mgrSelection">
      </select>      
      <div id='result'></div>
    </form>
</body>
</html>
