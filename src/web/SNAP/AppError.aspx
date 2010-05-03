<%@ Page Language="C#" MasterPageFile="~/SNAP.Master" AutoEventWireup="true" CodeBehind="AppError.aspx.cs" Inherits="Apollo.AIM.SNAP.Web.AppError" %>

<asp:Content ID="_contentContainer" ContentPlaceHolderID="_contentPlaceHolder" runat="server">
	<!-- BEGIN CONTENT AREA -->
	<div class="csm_container_100 csm_500">
		<div class="snap_container_center_844">
			<div class="csm_container_center_700">
				<h1>Application Error</h1>
				<div class="csm_content_container">
					<p>The application encountered an error.</p>
					<asp:Label ID="_errorMessage" runat="server" CssClass="csm_error_text"></asp:Label>
				</div>
			</div>
		</div>
	</div>
	<div class="csm_clear">&nbsp;</div>
	<!-- END CONTENT AREA -->
	<script type="text/javascript">
		try { pageTracker._trackPageview("/AppError"); }
		catch (err) { alert(err.toString()); }
	</script>	
</asp:Content>		
	
