USE [Apollo.AIM.SNAP]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Actors_Actor_Group]') AND parent_object_id = OBJECT_ID(N'[dbo].[SNAP_Actors]'))
ALTER TABLE [dbo].[SNAP_Actors] DROP CONSTRAINT [FK_Actors_Actor_Group]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Workflow_State_Workflow]') AND parent_object_id = OBJECT_ID(N'[dbo].[SNAP_Workflow_State]'))
ALTER TABLE [dbo].[SNAP_Workflow_State] DROP CONSTRAINT [FK_Workflow_State_Workflow]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Comments_Workflow]') AND parent_object_id = OBJECT_ID(N'[dbo].[SNAP_Workflow_Comments]'))
ALTER TABLE [dbo].[SNAP_Workflow_Comments] DROP CONSTRAINT [FK_Comments_Workflow]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Workflow_Actors]') AND parent_object_id = OBJECT_ID(N'[dbo].[SNAP_Workflow]'))
ALTER TABLE [dbo].[SNAP_Workflow] DROP CONSTRAINT [FK_Workflow_Actors]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Workflow_Request]') AND parent_object_id = OBJECT_ID(N'[dbo].[SNAP_Workflow]'))
ALTER TABLE [dbo].[SNAP_Workflow] DROP CONSTRAINT [FK_Workflow_Request]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SNAP_Request_Comments_SNAP_Requests]') AND parent_object_id = OBJECT_ID(N'[dbo].[SNAP_Request_Comments]'))
ALTER TABLE [dbo].[SNAP_Request_Comments] DROP CONSTRAINT [FK_SNAP_Request_Comments_SNAP_Requests]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Access_User_Text_Access_Details_Form]') AND parent_object_id = OBJECT_ID(N'[dbo].[SNAP_Access_User_Text]'))
ALTER TABLE [dbo].[SNAP_Access_User_Text] DROP CONSTRAINT [FK_Access_User_Text_Access_Details_Form]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Access_User_Text_Request]') AND parent_object_id = OBJECT_ID(N'[dbo].[SNAP_Access_User_Text]'))
ALTER TABLE [dbo].[SNAP_Access_User_Text] DROP CONSTRAINT [FK_Access_User_Text_Request]
GO
USE [Apollo.AIM.SNAP]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_request_tab]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_request_tab]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_user_view_tab]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_user_view_tab]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_approval_tab]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_approval_tab]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_access_team_tab]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_access_team_tab]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_approval_status]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_approval_status]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_insert_workflow_xml]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_insert_workflow_xml]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_insert_workflow_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_insert_workflow_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_delete_workflow]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_delete_workflow]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_access_team_details_old]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_access_team_details_old]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_access_team_status_old]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_access_team_status_old]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udf_get_next_business_day]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[udf_get_next_business_day]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_active_manager_check]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_active_manager_check]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Actor_Group_Type]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Actor_Group_Type]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_request_tab]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_request_tab]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Request_State_Type]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Request_State_Type]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_requests]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_requests]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Comments_Type]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Comments_Type]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_build_request_form]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_build_request_form]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_build_status_tracking]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_build_status_tracking]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_insert_request_xml]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_insert_request_xml]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_request_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_request_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_user_view_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_user_view_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_user_view_status]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_user_view_status]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_approval_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_approval_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_approval_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_approval_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_approval_text]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_approval_text]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_approval_workflow_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_approval_workflow_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_approval_workflow_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_approval_workflow_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_access_team_workflow_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_access_team_workflow_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_access_team_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_access_team_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_access_team_workflow_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_access_team_workflow_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_access_team_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_access_team_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_access_team_text]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_access_team_text]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_insert_workflow_state]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_insert_workflow_state]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Actor_Group]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Actor_Group]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Weekends_and_Holidays]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Weekends_and_Holidays]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_request_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_request_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_request_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_request_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_request_workflow_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_request_workflow_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_request_workflow_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_request_workflow_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_open_my_request_text]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_open_my_request_text]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Workflow_State_Type]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Workflow_State_Type]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_access_team_tab]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_access_team_tab]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_request_tab]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_request_tab]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_approval_tab]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_approval_tab]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_insert_access_user_text]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_insert_access_user_text]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_access_team_texts]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_access_team_texts]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_access_team_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_access_team_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_access_team_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_access_team_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_access_team_workflow_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_access_team_workflow_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_access_team_workflow_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_access_team_workflow_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Access_Details_Form]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Access_Details_Form]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_request_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_request_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_request_texts]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_request_texts]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_request_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_request_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_request_workflow_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_request_workflow_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_request_workflow_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_request_workflow_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_approval_workflow_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_approval_workflow_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_approval_workflow_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_approval_workflow_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_approval_comments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_approval_comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_approval_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_approval_details]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_my_approval_texts]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_my_approval_texts]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Actors]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Actors]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Workflow_State]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Workflow_State]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Requests]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Requests]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Workflow_Comments]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Workflow_Comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Workflow]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Workflow]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Request_Comments]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Request_Comments]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SNAP_Access_User_Text]') AND type in (N'U'))
DROP TABLE [dbo].[SNAP_Access_User_Text]
