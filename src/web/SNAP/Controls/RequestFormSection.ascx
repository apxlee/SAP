<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestFormSection.ascx.cs" Inherits="Apollo.AIM.SNAP.Web.Controls.RequestFormSection" %>
<tr>
    <td colspan="2" class="csm_input_spanning_column">
	    <div class="csm_input_form_label_container">
		    <label id="LabelContainer" runat="server">
		        <asp:Label ID="OuterLabel" runat="server"/>
		    </label>
		    <asp:Label ID="OuterDescription" runat="server"/>	
		    <asp:Label Visible="false" ID="ParentId" runat="server"/>				
	    </div>
	    <div class="csm_input_form_control_container">
	        <asp:PlaceHolder ID="phrRequestFormField" runat="server" ></asp:PlaceHolder>
	    </div>						
    </td>
</tr>