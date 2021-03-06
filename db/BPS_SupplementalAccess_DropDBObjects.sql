USE [BPS_SupplementalAccess]
GO
/****** Object:  ForeignKey [FK_Access_User_Text_Access_Details_Form]    Script Date: 04/29/2010 12:15:18 ******/
ALTER TABLE [dbo].[SNAP_Access_User_Text] DROP CONSTRAINT [FK_Access_User_Text_Access_Details_Form]
GO
/****** Object:  ForeignKey [FK_Access_User_Text_Request]    Script Date: 04/29/2010 12:15:18 ******/
ALTER TABLE [dbo].[SNAP_Access_User_Text] DROP CONSTRAINT [FK_Access_User_Text_Request]
GO
/****** Object:  ForeignKey [FK_Actors_Actor_Group]    Script Date: 04/29/2010 12:15:18 ******/
ALTER TABLE [dbo].[SNAP_Actors] DROP CONSTRAINT [FK_Actors_Actor_Group]
GO
/****** Object:  ForeignKey [FK_SNAP_Request_Comments_SNAP_Requests]    Script Date: 04/29/2010 12:15:18 ******/
ALTER TABLE [dbo].[SNAP_Request_Comments] DROP CONSTRAINT [FK_SNAP_Request_Comments_SNAP_Requests]
GO
/****** Object:  ForeignKey [FK_Workflow_Actors]    Script Date: 04/29/2010 12:15:18 ******/
ALTER TABLE [dbo].[SNAP_Workflow] DROP CONSTRAINT [FK_Workflow_Actors]
GO
/****** Object:  ForeignKey [FK_Workflow_Request]    Script Date: 04/29/2010 12:15:18 ******/
ALTER TABLE [dbo].[SNAP_Workflow] DROP CONSTRAINT [FK_Workflow_Request]
GO
/****** Object:  ForeignKey [FK_Comments_Workflow]    Script Date: 04/29/2010 12:15:18 ******/
ALTER TABLE [dbo].[SNAP_Workflow_Comments] DROP CONSTRAINT [FK_Comments_Workflow]
GO
/****** Object:  ForeignKey [FK_Workflow_State_Workflow]    Script Date: 04/29/2010 12:15:18 ******/
ALTER TABLE [dbo].[SNAP_Workflow_State] DROP CONSTRAINT [FK_Workflow_State_Workflow]
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_approval_status]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_open_my_approval_status]
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_workflow_comments]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_open_my_request_workflow_comments]
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_workflow_xml]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_insert_workflow_xml]
GO
/****** Object:  StoredProcedure [dbo].[usp_open_request_tab]    Script Date: 04/29/2010 12:15:18 ******/
DROP PROCEDURE [dbo].[usp_open_request_tab]
GO
/****** Object:  StoredProcedure [dbo].[usp_open_request_details]    Script Date: 04/29/2010 12:15:18 ******/
DROP PROCEDURE [dbo].[usp_open_request_details]
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_details]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_open_my_request_details]
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_comments]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_open_my_request_comments]
GO
/****** Object:  StoredProcedure [dbo].[usp_open_access_team_details_old]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_open_access_team_details_old]
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_workflow_details]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_open_my_request_workflow_details]
GO
/****** Object:  StoredProcedure [dbo].[usp_search_requests]    Script Date: 04/29/2010 12:15:18 ******/
DROP PROCEDURE [dbo].[usp_search_requests]
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_workflow_comments]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_insert_workflow_comments]
GO
/****** Object:  StoredProcedure [dbo].[usp_request_details]    Script Date: 04/29/2010 12:15:18 ******/
DROP PROCEDURE [dbo].[usp_request_details]
GO
/****** Object:  StoredProcedure [dbo].[usp_delete_workflow]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_delete_workflow]
GO
/****** Object:  StoredProcedure [dbo].[usp_create_weekends_and_holidays_table]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_create_weekends_and_holidays_table]
GO
/****** Object:  UserDefinedFunction [dbo].[udf_get_next_business_day]    Script Date: 04/29/2010 12:15:18 ******/
DROP FUNCTION [dbo].[udf_get_next_business_day]
GO
/****** Object:  Table [dbo].[SNAP_Comments_Type]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Comments_Type]
GO
/****** Object:  StoredProcedure [dbo].[usp_build_request_form]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_build_request_form]
GO
/****** Object:  StoredProcedure [dbo].[usp_build_status_tracking]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_build_status_tracking]
GO
/****** Object:  Table [dbo].[SNAP_Request_State_Type]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Request_State_Type]
GO
/****** Object:  StoredProcedure [dbo].[usp_requests]    Script Date: 04/29/2010 12:15:18 ******/
DROP PROCEDURE [dbo].[usp_requests]
GO
/****** Object:  StoredProcedure [dbo].[usp_active_manager_check]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_active_manager_check]
GO
/****** Object:  Table [dbo].[SNAP_Actor_Group_Type]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Actor_Group_Type]
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_request_xml]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_insert_request_xml]
GO
/****** Object:  Table [dbo].[SNAP_Access_Details_Form]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Access_Details_Form]
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_workflow_state]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_insert_workflow_state]
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_texts]    Script Date: 04/29/2010 12:15:18 ******/
DROP PROCEDURE [dbo].[usp_search_request_texts]
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_comments]    Script Date: 04/29/2010 12:15:18 ******/
DROP PROCEDURE [dbo].[usp_search_request_comments]
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_workflow_comments]    Script Date: 04/29/2010 12:15:18 ******/
DROP PROCEDURE [dbo].[usp_search_request_workflow_comments]
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_workflow_details]    Script Date: 04/29/2010 12:15:18 ******/
DROP PROCEDURE [dbo].[usp_search_request_workflow_details]
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_details]    Script Date: 04/29/2010 12:15:18 ******/
DROP PROCEDURE [dbo].[usp_search_request_details]
GO
/****** Object:  Table [dbo].[SNAP_Actor_Group]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Actor_Group]
GO
/****** Object:  Table [dbo].[SNAP_Weekends_and_Holidays]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Weekends_and_Holidays]
GO
/****** Object:  Table [dbo].[SNAP_Workflow_State_Type]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Workflow_State_Type]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_tab]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_request_tab]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_tab]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_approval_tab]
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_tab]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_access_team_tab]
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_actor]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_insert_actor]
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_access_user_text]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_insert_access_user_text]
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_texts]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_access_team_texts]
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_details]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_access_team_details]
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_comments]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_access_team_comments]
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_workflow_comments]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_access_team_workflow_comments]
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_workflow_details]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_access_team_workflow_details]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_details]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_request_details]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_texts]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_request_texts]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_comments]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_request_comments]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_workflow_comments]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_request_workflow_comments]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_workflow_details]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_request_workflow_details]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_workflow_details]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_approval_workflow_details]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_workflow_comments]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_approval_workflow_comments]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_comments]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_approval_comments]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_details]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_approval_details]
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_texts]    Script Date: 04/29/2010 12:15:17 ******/
DROP PROCEDURE [dbo].[usp_my_approval_texts]
GO
/****** Object:  Table [dbo].[SNAP_Actors]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Actors]
GO
/****** Object:  Table [dbo].[SNAP_Requests]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Requests]
GO
/****** Object:  Table [dbo].[SNAP_Access_User_Text]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Access_User_Text]
GO
/****** Object:  Table [dbo].[SNAP_Request_Comments]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Request_Comments]
GO
/****** Object:  Table [dbo].[SNAP_Workflow_State]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Workflow_State]
GO
/****** Object:  Table [dbo].[SNAP_Workflow]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Workflow]
GO
/****** Object:  Table [dbo].[SNAP_Workflow_Comments]    Script Date: 04/29/2010 12:15:18 ******/
DROP TABLE [dbo].[SNAP_Workflow_Comments]
GO
