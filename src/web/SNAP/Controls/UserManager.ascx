<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserManager.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.UserManager" %>

<!--<script src='/scripts/user.js' type="text/javascript" ></script>-->


<script type="text/javascript">
    if (typeof userManager == 'undefined') {
        document.write("<script src=\"/scripts/user.js\" type=\"text/javascript\"></" + "script>");
    }
</script>

  <table>
      <tr>
          <td>
              Name:
          </td>
          <td>
              <asp:TextBox ID="userName" runat="server"></asp:TextBox>
              <button type='button' id='userCheck'>Check</button>
          </td>
          <td>
              <div id="notification" style="display: none;"><img src="/images/ajax_indicator.gif" width="16" height="16" />&nbsp;Working...</div>
          </td>
          <td>
              <asp:TextBox ID="userLoginId" runat="server" style="display:none" ></asp:TextBox>
          </td>
      </tr>
      <tr>
          <td>
              &nbsp;</td>
          <td>
              <select size="3" name="nameSelection" id="nameSelection"></select></td>
          <td>
              &nbsp;</td>
      </tr>
      <tr>
          <td>
              Manager:
          </td>
          <td>
              <asp:TextBox ID="mgrName" runat="server"></asp:TextBox>
              <button type='button' id='mgrCheck'>Change</button>
              
          </td>
          <td><button type='button' id='mgrEdit'>Edit</button></td>
          <td>
              <asp:TextBox ID="mgrLoginId" runat="server" style="display:none"></asp:TextBox>
          </td>
      </tr>
      <tr>
          <td>
              &nbsp;</td>
          <td>
              <select size="3" name="mgrSelection" id="mgrSelection"></select></td>
          <td>
              &nbsp;</td>
      </tr>
  </table>

