<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucUserDetails.ascx.cs" Inherits="SNAP.includes.ucUserDetails" %>

<script type="text/javascript" language="javascript">
    function DoIDSearch(txtEmpID, oid, url)
    {
        alert('value: ' + txtEmpID.value);
        alert('value2: ' + oid);
        
        if(txtEmpID.value == ""){
            alert('Please enter an Employee ID to validate');
            return;
        }
        
        var varSortOrder = "";
        var varSrcKey = oid;
        var varSrcParam = txtEmpID.value;
        
        if(url == '' || url == undefined){
            url = "http://localhost:2435/Controls/Search/SearchService.asmx";
        }
        
        //alert('url: ' + url);
        var xmlInput = setSearchParams(varSrcKey, varSrcParam, varSortOrder, "PersonByID");
        callAjaxSoap(url, "Search", convertParamsFromHash($H({strXmlInput:xmlInput})), true, OnComplete_SearchID);
        
    }
    
function OnComplete_SearchID(call, env)
{
    
	var response = getAjaxResponseText(env);
    alert('callback: ' + response);
    if(response == "False"){
        alert('Unassigned Employee ID. This value is acceptable');
    }else if(response == "True"){
        alert('Employee ID is already assigned');
    }
	

}    
</script>


<table border="0">
    <tr>
        <td>&nbsp;<asp:Label ID="lblOID" runat="server" Visible="False"></asp:Label></td>
        <td>First</td>
        <td>&nbsp;</td>
        <td>MI</td>
        <td>Last</td>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td align="right" style="height: 26px">Name:</td>
        <td style="height: 26px"><asp:TextBox ID="txtFName" runat="server" Width="200px" /></td>
        <td style="height: 26px">&nbsp;</td>
        <td style="height: 26px"><asp:TextBox ID="txtMName" runat="server" /></td>
        <td style="height: 26px"><asp:TextBox ID="txtLName" runat="server" Width="200px" /></td>
        <td style="height: 26px">&nbsp;</td>
    </tr>
    <tr>
        <td align="right">Display Name:</td>
        <td><asp:TextBox ID="txtDName" runat="server" Width="200px" /></td>
        <td>&nbsp;</td>
        <td align="right">Preferred First Name:</td>
        <td><asp:TextBox ID="txtPreFName" runat="server" Width="200px" /></td>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="6">&nbsp;</td>
    </tr>
    <tr>
        <td align="right">Last 4 of SSN:</td>
        <td align="left"><asp:Label ID="lblSSN" runat="server" Font-Bold="true" /></td>
        <td>&nbsp;</td>
        <td align="right">Employee ID:</td>
        <td><asp:TextBox ID="txtEmpID" runat="server" Width="200px" /></td>
        <td>
            <asp:Label ID="lblCheckID" runat="server" BackColor="Transparent" BorderColor="DarkGray"
                BorderStyle="Solid" BorderWidth="1px" CssClass="labelbutton" Text="..."></asp:Label></td>
    </tr>
    <tr>
        <td align="right">Employee Type:</td>
        <td align="left"><asp:DropDownList ID="ddlEmpType" runat="server" >
            <asp:ListItem Value="S">S - Staff</asp:ListItem>
            <asp:ListItem Value="C">C - Contractor</asp:ListItem>
        </asp:DropDownList></td>
        <td>&nbsp;</td>
        <td align="right">Employee Status:</td>
        <td align="left"><asp:DropDownList ID="ddlEmpStatus" runat="server" >
            <asp:ListItem Value="A">A - Active</asp:ListItem>
            <asp:ListItem Value="D">D - Deceased</asp:ListItem>
            <asp:ListItem Value="L">L - Leave</asp:ListItem>
            <asp:ListItem Value="T">T - Terminated</asp:ListItem>
        </asp:DropDownList></td>
        <td>&nbsp;</td>        
    </tr>
    <tr>
        <td colspan="2">&nbsp;</td>
        <td align="right" colspan="2">Urgent Access Removal:</td>
        <td align="left">
            <asp:CheckBox ID="chkUrgentRemoval" runat="server" />
        </td>
    </tr>
    <tr>
        <td colspan="6">&nbsp;</td>
    </tr>
    <tr>
        <td align="right">
            <asp:Label ID="lblMangerID" runat="server" Visible="False"></asp:Label>Manager:</td>
        <td><asp:TextBox ID="txtManager" runat="server" ReadOnly="True" Width="200px" /></td>
        <td><asp:Button ID="btnLookManager" runat="server" Text="..." CommandArgument="Manager" CommandName="Search" OnCommand="Search" /></td>
        <td align="right">
            <asp:Label ID="lblJobID" runat="server" Visible="False"></asp:Label>Job:</td>
        <td><asp:TextBox ID="txtJob" runat="server" ReadOnly="True" Width="200px" /></td>
        <td><asp:Button ID="btnLookJob" runat="server" Text="..." CommandArgument="Job" CommandName="Search" OnCommand="Search" /></td>  
    </tr>
    <tr>
        <td align="right" style="height: 26px">
            <asp:Label ID="lblCostCenterID" runat="server" Visible="False"></asp:Label>Cost Center:</td>
        <td style="height: 26px"><asp:TextBox ID="txtCostCenter" runat="server" ReadOnly="True" Width="200px" /></td>
        <td style="height: 26px"><asp:Button ID="btnLookCostCenter" runat="server" Text="..." CommandArgument="CostCenter" CommandName="Search" OnCommand="Search" /></td>
        <td align="right" style="height: 26px">
            <asp:Label ID="lblCampusID" runat="server" Visible="False"></asp:Label>Campus:</td>
        <td style="height: 26px"><asp:TextBox ID="txtCampus" runat="server" ReadOnly="True" Width="200px" /></td>
        <td style="height: 26px"><asp:Button ID="btnLookCampus" runat="server" Text="..." CommandArgument="Location" CommandName="Search" OnCommand="Search" /></td>      
    </tr>
    <tr>
        <td colspan="6">&nbsp;</td>
    </tr>
    <tr>
        <td align="right">Start Date <small>(MM/DD/YYYY)</small>:</td>
        <td><asp:TextBox ID="txtStartDate" runat="server" Width="200px" /></td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>      
    </tr>
    <tr>
        <td align="right">End Date <small>(MM/DD/YYYY)</small>:</td>
        <td><asp:TextBox ID="txtEndDate" runat="server" Width="200px" /></td>
        <td></td>
        <td align="right">Contractor Agency:</td>
        <td><asp:TextBox ID="txtContract" runat="server" Width="200px" /></td>
        <td>&nbsp;</td>      
    </tr>
    <tr>
        <td colspan="6">&nbsp;</td>
    </tr>
    <tr>
        <td align="right">UOP PS Emp ID:</td>
        <td><asp:TextBox ID="txtUOPEmpID" runat="server" Width="200px" /></td>
        <td>&nbsp;</td>
        <td align="right">UOP PS User ID:</td>
        <td><asp:TextBox ID="txtUOPUserID" runat="server" Width="200px" /></td>
        <td>&nbsp;</td>      
    </tr>  
    <tr>
        <td align="right">WIU PS Emp ID:</td>
        <td><asp:TextBox ID="txtWIUEmpID" runat="server" Width="200px" /></td>
        <td>&nbsp;</td>
        <td align="right">WIU PS User ID:</td>
        <td><asp:TextBox ID="txtWIUUserID" runat="server" Width="200px" /></td>
        <td>&nbsp;</td>      
    </tr>
    <tr>
        <td colspan="6">
            <br />
            <hr />
            <asp:Label ID="lblOutput" runat="server" Text="" ForeColor="Red" /><br /><br />
            <div align="center"><asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" /></div>
            <br />
        </td>
    </tr>                 
</table>
