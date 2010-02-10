<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestFormSection.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.RequestFormSection" %>
<tr>
    <td colspan="2" class="csm_input_spanning_column">
	    <div class="csm_input_form_label_container">
		    <label id="_labelContainer" runat="server">
		        <asp:Label ID="_outerLabel" runat="server"/>
		    </label>
		    <asp:Label ID="_outerDescription" runat="server"/>	
		    <asp:Label Visible="false" ID="_parentId" runat="server"/>				
	    </div>
	    <div class="csm_input_form_control_container">
	        <asp:PlaceHolder ID="_requestFormField" runat="server" ></asp:PlaceHolder>
	    </div>						
    </td>
</tr>