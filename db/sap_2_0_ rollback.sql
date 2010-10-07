USE [BPS_SupplementalAccess]
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_workflow_details]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_request_workflow_details]
	@search nvarchar(100)
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	begin
		select f.requestId, s.pkId as workflowStateId, s.workflowId, s.workflowStatusEnum, s.notifyDate, s.dueDate, s.completedDate, a.pkId as actorId, a.actor_groupId, a.userId, a.displayName, a.emailAddress, a.isGroup, a.isDefault, a.isActive, (select ag.actorGroupType from dbo.SNAP_Actor_Group ag where ag.pkId = a.actor_groupId) as actorGroupType
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
		where (r.userId = @search 
			or r.userDisplayName like('%' + @search + '%')
			or CONVERT(nvarchar(100),r.pkid) =  @search) 
			and r.pkid = f.requestId and f.pkid = s.workflowId
			and f.actorid = a.pkid
		order by r.pkid, s.workflowId, s.pkId
	end

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_workflow_details]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_request_workflow_details]
	@userId nvarchar(128),
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if @status = 'open'
	begin
	
		select f.requestId, s.pkId as workflowStateId, s.workflowId, s.workflowStatusEnum, s.notifyDate, s.dueDate, s.completedDate, a.pkId as actorId, a.actor_groupId, a.userId, a.displayName, a.emailAddress, a.isGroup, a.isDefault, a.isActive, (select ag.actorGroupType from dbo.SNAP_Actor_Group ag where ag.pkId = a.actor_groupId) as actorGroupType
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
		where (r.userId = @userId or  r.submittedBy = @userId) 
			and (r.statusEnum IN(0,1,2)) 
			and r.pkid = f.requestId and f.pkid = s.workflowId
			and f.actorid = a.pkid
		order by r.pkid, s.workflowId, s.pkId
	end
	else if @status = 'close'
	begin
		select f.requestId, s.pkId as workflowStateId, s.workflowId, s.workflowStatusEnum, s.notifyDate, s.dueDate, s.completedDate, a.pkId as actorId, a.actor_groupId, a.userId, a.displayName, a.emailAddress, a.isGroup, a.isDefault, a.isActive, (select ag.actorGroupType from dbo.SNAP_Actor_Group ag where ag.pkId = a.actor_groupId) as actorGroupType
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
		where (r.userId = @userId or  r.submittedBy = @userId) 
			and (r.statusEnum Not IN(0,1,2)) 
			and r.pkid = f.requestId and f.pkid = s.workflowId
			and f.actorid = a.pkid
			and r.lastModifiedDate > getdate()-30
		order by r.pkid, s.workflowId, s.pkId

	end

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_workflow_details]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_approval_workflow_details]
	@userId nvarchar(128),
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if @status = 'open'
	begin
		select f.requestId, s.pkId as workflowStateId, s.workflowId, s.workflowStatusEnum, s.notifyDate, s.dueDate, s.completedDate, a.pkId as actorId, a.actor_groupId, a.userId, a.displayName, a.emailAddress, a.isGroup, a.isDefault, a.isActive, (select ag.actorGroupType from dbo.SNAP_Actor_Group ag where ag.pkId = a.actor_groupId) as actorGroupType
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
		where (r.statusEnum IN(0,1,2))
			and f.requestId IN(select r.pkId from dbo.SNAP_Requests r
			join dbo.SNAP_Workflow sw
			on r.pkId = sw.requestId
			join dbo.SNAP_Actors sa
			on sw.actorId = sa.pkId
			where sa.userId = @userId)
			and r.pkid = f.requestId 
			and f.pkid = s.workflowId
			and f.actorid = a.pkid
		order by r.pkid, s.workflowId, s.pkId

	end
	else if @status = 'close'
	begin
		select f.requestId, s.pkId as workflowStateId, s.workflowId, s.workflowStatusEnum, s.notifyDate, s.dueDate, s.completedDate, a.pkId as actorId, a.actor_groupId, a.userId, a.displayName, a.emailAddress, a.isGroup, a.isDefault, a.isActive, (select ag.actorGroupType from dbo.SNAP_Actor_Group ag where ag.pkId = a.actor_groupId) as actorGroupType
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
		where (r.statusEnum NOT IN(0,1,2))
			and f.requestId IN(select r.pkId from dbo.SNAP_Requests r
			join dbo.SNAP_Workflow sw
			on r.pkId = sw.requestId
			join dbo.SNAP_Actors sa
			on sw.actorId = sa.pkId
			where sa.userId = @userId)
			and r.pkid = f.requestId 
			and f.pkid = s.workflowId
			and f.actorid = a.pkid
			and r.lastModifiedDate > getdate()-30
		order by r.pkid, s.workflowId, s.pkId
	end
END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_workflow_details]    Script Date: 10/07/2010 10:25:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_access_team_workflow_details]
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if @status = 'open'
	begin
		select f.requestId, s.pkId as workflowStateId, s.workflowId, s.workflowStatusEnum, s.notifyDate, s.dueDate, s.completedDate, a.pkId as actorId, a.actor_groupId, a.userId, a.displayName, a.emailAddress, a.isGroup, a.isDefault, a.isActive, (select ag.actorGroupType from dbo.SNAP_Actor_Group ag where ag.pkId = a.actor_groupId) as actorGroupType
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
		where (r.statusEnum IN(0,1,2)) 
			and r.pkid = f.requestId and f.pkid = s.workflowId
			and f.actorid = a.pkid
		order by r.pkid, s.workflowId, s.pkId

	end
	else if @status = 'close'
	begin
		select f.requestId, s.pkId as workflowStateId, s.workflowId, s.workflowStatusEnum, s.notifyDate, s.dueDate, s.completedDate,r.lastModifiedDate, a.pkId as actorId, a.actor_groupId, a.userId, a.displayName, a.emailAddress, a.isGroup, a.isDefault, a.isActive, (select ag.actorGroupType from dbo.SNAP_Actor_Group ag where ag.pkId = a.actor_groupId) as actorGroupType
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
		where (r.statusEnum Not IN(0,1,2)) 
			and r.pkid = f.requestId and f.pkid = s.workflowId
			and f.actorid = a.pkid
			and r.lastModifiedDate > getdate()-0
		order by r.pkid, s.workflowId, s.pkId

	end

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_texts]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_request_texts]
	@search nvarchar(100)
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
	if (object_id('tempdb..#request_temp1') is not null)
		drop table #request_temp1

	select a.requestID, a.access_details_formId, maxDate = max(modifiedDate) into #request_temp1
	from snap_access_user_text a, SNAP_Requests r 
	where (r.userId = @search 
		or r.userDisplayName like('%' + @search + '%')
		or CONVERT(nvarchar(100),r.pkid) =  @search) 
		and a.requestID = r.pkid 
	group by access_details_formId, requestID
	
	select a.* 
	from snap_access_user_text a, #request_temp1 t
	where a.requestId = t.requestId and a.access_details_formId = t.access_details_formId and a.modifiedDate = t.maxDate
	order by a.requestId, a.access_details_formId
	
	drop table #request_temp1	
	
END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_texts]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_approval_texts]
	@userId nvarchar(128),
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	
	if @status = 'open'
	begin	
		if (object_id('tempdb..#approval_temp1') is not null)
			drop table #approval_temp1

		select u.requestID, u.access_details_formId, maxDate = max(modifiedDate) into #approval_temp1
		from snap_access_user_text u, SNAP_Requests r, SNAP_Workflow w, SNAP_Actors a 
		where (r.pkid = w.requestId) 
			and w.actorid = a.pkid
			and (r.statusEnum IN(0,1,2)) 
			and a.userid = @userId 
			and u.requestid = r.pkid
		group by u.access_details_formId, u.requestID
		
		select a.* 
		from snap_access_user_text a, #approval_temp1 t
		where a.requestId = t.requestId and a.access_details_formId = t.access_details_formId and a.modifiedDate = t.maxDate
		order by a.requestId, a.access_details_formId

		drop table #approval_temp1

	end
	
	
	if @status = 'close'
	begin
		if (object_id('tempdb..#approval_temp2') is not null)
			drop table #approval_temp2

		select u.requestID, u.access_details_formId, maxDate = max(modifiedDate) into #approval_temp2
		from snap_access_user_text u, SNAP_Requests r, SNAP_Workflow w, SNAP_Actors a 
		where (r.pkid = w.requestId) 
			and w.actorid = a.pkid
			and (r.statusEnum Not IN(0,1,2)) 
			and a.userid = @userId 
			and u.requestid = r.pkid
			and r.lastModifiedDate > getdate()-30
		group by u.access_details_formId, u.requestID
		
		select a.* 
		from snap_access_user_text a, #approval_temp2 t
		where a.requestId = t.requestId and a.access_details_formId = t.access_details_formId and a.modifiedDate = t.maxDate
		order by a.requestId, a.access_details_formId

		drop table #approval_temp2

	end
	

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_texts]    Script Date: 10/07/2010 10:25:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_access_team_texts]
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if @status = 'open'
	begin
		if (object_id('tempdb..#access_temp1') is not null)
		drop table #request_temp1
	
	
		select a.requestID, a.access_details_formId, maxDate = max(modifiedDate) into #access_temp1
		from snap_access_user_text a, SNAP_Requests r 
		where (r.statusEnum IN(0,1,2)) 
			and r.pkid = a.requestID
		group by access_details_formId, requestID

		select a.* 
		from snap_access_user_text a, #access_temp1 t
		where a.requestId = t.requestId and a.access_details_formId = t.access_details_formId and a.modifiedDate = t.maxDate
		order by a.requestId, a.access_details_formId

		drop table #access_temp1

	
	end
	
	else if @status = 'close'
	begin
		if (object_id('tempdb..#access_temp2') is not null)
			drop table #access_temp2

		select a.requestID, a.access_details_formId, maxDate = max(modifiedDate) into #access_temp2
		from snap_access_user_text a, SNAP_Requests r 
		where (r.statusEnum Not IN(0,1,2))
			and r.pkid = a.requestID
			and r.lastModifiedDate > getdate()-0
		group by access_details_formId, requestID

		select a.* 
		from snap_access_user_text a, #access_temp2 t
		where a.requestId = t.requestId and a.access_details_formId = t.access_details_formId and a.modifiedDate = t.maxDate
		order by a.requestId, a.access_details_formId

		drop table #access_temp2
	end
	

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_texts]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_request_texts]
	@userId nvarchar(128),
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
	if @status = 'open'
	begin
	
		if (object_id('tempdb..#request_temp1') is not null)
			drop table #request_temp1

		select a.requestID, a.access_details_formId, maxDate = max(modifiedDate) into #request_temp1
		from snap_access_user_text a, SNAP_Requests r 
		where (r.userId = @userId or  r.submittedBy = @userId) 
			and (r.statusEnum IN(0,1,2)) 
			and a.requestID = r.pkid 
		group by access_details_formId, requestID
		
		select a.* 
		from snap_access_user_text a, #request_temp1 t
		where a.requestId = t.requestId and a.access_details_formId = t.access_details_formId and a.modifiedDate = t.maxDate
		order by a.requestId, a.access_details_formId
		
		drop table #request_temp1

	end
	
	else if @status = 'close'
	begin
	
		if (object_id('tempdb..#request_temp2') is not null)
			drop table #request_temp2

		select a.requestID, a.access_details_formId, maxDate = max(modifiedDate) into #request_temp2
		from snap_access_user_text a, SNAP_Requests r 
		where (r.userId = @userId or  r.submittedBy = @userId) 
			and (r.statusEnum NOT IN(0,1,2)) 
			and a.requestID = r.pkid 
			and r.lastModifiedDate > getdate()-30
		group by access_details_formId, requestID
		
		select a.* 
		from snap_access_user_text a, #request_temp2 t
		where a.requestId = t.requestId and a.access_details_formId = t.access_details_formId and a.modifiedDate = t.maxDate
		order by a.requestId, a.access_details_formId
		
		drop table #request_temp2


	end
	
	
END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_comments]    Script Date: 10/07/2010 10:25:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_access_team_comments]
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if @status = 'open'
	begin
		select c.* from dbo.SNAP_Requests r 
		JOIN  dbo.SNAP_Request_Comments c on
		r.pkid = c.requestId
		where (r.statusEnum IN(0,1,2)) 

	end
	else if @status = 'close'
	begin
		select c.* from dbo.SNAP_Requests r 
		JOIN  dbo.SNAP_Request_Comments c on
		r.pkid = c.requestId
		where (r.statusEnum Not IN(0,1,2))
		and r.lastModifiedDate > getdate()-0

	end

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_comments]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_request_comments]
	@userId nvarchar(128),
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if @status = 'open'
	begin
		select c.* from dbo.SNAP_Requests r 
		JOIN  dbo.SNAP_Request_Comments c on
		r.pkid = c.requestId
		where (r.userId = @userId or  r.submittedBy = @userId) 
		and (r.statusEnum IN(0,1,2)) 
	end
	else if @status = 'close'
	begin
		select c.* from dbo.SNAP_Requests r 
		JOIN  dbo.SNAP_Request_Comments c on
		r.pkid = c.requestId
		where (r.userId = @userId or  r.submittedBy = @userId) 
		and (r.statusEnum NOT IN(0,1,2))
		and r.lastModifiedDate > getdate()-30		
	end
END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_comments]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_request_comments]
	@search nvarchar(100)
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	begin
		select c.* from dbo.SNAP_Requests r 
		JOIN  dbo.SNAP_Request_Comments c on
		r.pkid = c.requestId
		where (r.userId = @search 
			or r.userDisplayName like('%' + @search + '%')
			or CONVERT(nvarchar(100),r.pkid) =  @search) 
	end
END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_comments]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_open_my_request_comments]
	@userId nvarchar(128)
AS
/*
BEGIN
	SET NOCOUNT ON;
	
	select c.* from dbo.SNAP_Requests r 
	JOIN  dbo.SNAP_Request_Comments c on
	r.pkid = c.requestId
	where (r.userId = @userId or  r.submittedBy = @userId) 
	and (r.statusEnum IN(0,1,2)) 

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_comments]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_approval_comments]
	@userId nvarchar(128),
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if @status = 'open'
	begin
		select c.* 
		from dbo.SNAP_Requests r, 
			dbo.SNAP_workflow w, 
			dbo.SNAP_Actors a, 
			dbo.SNAP_Request_Comments c
		where (r.pkid = w.requestid)
			and (w.actorid = a.pkid)
			and (a.userId = @userid) 
			and (r.statusEnum IN(0,1,2)) 
			and c.RequestId = r.pkid
	end
	else if @status = 'close'
	begin
		select c.* 
		from dbo.SNAP_Requests r, 
			dbo.SNAP_workflow w, 
			dbo.SNAP_Actors a, 
			dbo.SNAP_Request_Comments c
		where (r.pkid = w.requestid)
			and (w.actorid = a.pkid)
			and (a.userId = @userid) 
			and (r.statusEnum Not IN(0,1,2)) 
			and c.RequestId = r.pkid
			and r.lastModifiedDate > getdate()-30

	end

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_workflow_comments]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_open_my_request_workflow_comments]
	@userId nvarchar(128)
AS
/*
BEGIN
	SET NOCOUNT ON;

	select f.requestId, c.*
	from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_Comments c
	where (r.userId = @userId or  r.submittedBy = @userId) 
		and (r.statusEnum IN(0,1,2)) 
		and r.pkid = f.requestId 
		and f.pkid = c.workflowId
	order by r.pkid, c.workflowId

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_workflow_comments]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_request_workflow_comments]
	@search nvarchar(100)
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	begin
		select f.requestId, c.*
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_Comments c
		where (r.userId = @search 
			or r.userDisplayName like('%' + @search + '%')
			or CONVERT(nvarchar(100),r.pkid) =  @search) 
			and r.pkid = f.requestId 
			and f.pkid = c.workflowId
		order by r.pkid, c.workflowId
	end	

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_approval_status]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_open_my_approval_status]
(
	@userId nvarchar(128)
)

AS
/*
BEGIN
SET NOCOUNT ON;

SELECT w.requestId, 
ws.workflowId, 
a.userId, 
a.displayName, 
ws.workflowStatusEnum, 
ws.notifyDate, 
ws.dueDate, 
ws.completedDate,
wc.commentText, 
wc.commentTypeEnum
FROM SNAP_Requests r
JOIN SNAP_workflow w
ON r.pkId = w.requestId
JOIN SNAP_workflow_state ws
ON w.pkId = ws.workflowId
JOIN SNAP_actors a
ON w.actorId = a.pkId
LEFT OUTER JOIN SNAP_Workflow_Comments wc
ON w.pkId = wc.workflowId
WHERE (r.pkId 
IN(SELECT sr.pkId 
FROM SNAP_Requests sr 
JOIN SNAP_workflow sw
ON sr.pkId = sw.requestId
JOIN SNAP_actors sa
ON sw.actorId = sa.pkId
WHERE sa.userId = @userId
AND (r.statusEnum IN(0,1,2))
))

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_workflow_comments]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_request_workflow_comments]
	@userId nvarchar(128),
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if @status = 'open'
	begin
		select f.requestId, c.*
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_Comments c
		where (r.userId = @userId or  r.submittedBy = @userId) 
			and (r.statusEnum IN(0,1,2)) 
			and r.pkid = f.requestId 
			and f.pkid = c.workflowId
		order by r.pkid, c.workflowId
	end
	else if @status = 'close'
	begin
		select f.requestId, c.*
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_Comments c
		where (r.userId = @userId or  r.submittedBy = @userId) 
			and (r.statusEnum Not IN(0,1,2)) 
			and r.pkid = f.requestId 
			and f.pkid = c.workflowId
			and r.lastModifiedDate > getdate()-30
		order by r.pkid, c.workflowId

	end
	

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_workflow_comments]    Script Date: 10/07/2010 10:25:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_access_team_workflow_comments]
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if @status = 'open'
	begin
		select f.requestId, c.*
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_Comments c
		where (r.statusEnum IN(0,1,2)) 
			and r.pkid = f.requestId 
			and f.pkId = c.workflowId
		order by r.pkid, c.workflowId

	end
	else if @status = 'close'
	begin
		select f.requestId, c.*
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_Comments c
		where (r.statusEnum Not IN(0,1,2)) 
			and r.pkid = f.requestId 
			and f.pkId = c.workflowId
			and r.lastModifiedDate > getdate()-0
		order by r.pkid, c.workflowId

	end
	

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_workflow_comments]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_approval_workflow_comments]
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if @status = 'open'
	begin
		select f.requestId, c.*
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Actors a, SNAP_Workflow_Comments c
		where (f.actorid = a.pkid)
			and (r.statusEnum IN(0,1,2)) 
			and r.pkid = f.requestId 
			and f.pkid = c.workflowId
		order by r.pkid

	end
	else if @status = 'close'
	begin
	
		select f.requestId, c.*
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Actors a, SNAP_Workflow_Comments c
		where (f.actorid = a.pkid)
			and (r.statusEnum Not IN(0,1,2)) 
			and r.pkid = f.requestId 
			and f.pkid = c.workflowId
			and r.lastModifiedDate > getdate()-30
		order by r.pkid

	end
	
END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_details]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_approval_details]
	@userId nvarchar(128),
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if @status = 'open'
	begin
	   	select r.* from dbo.SNAP_Requests r , dbo.SNAP_Workflow w, dbo.SNAP_Actors a
		where r.pkid = w.requestId 
			and w.actorid = a.pkid
			and a.userid = @userid
			and (r.statusEnum IN(0,1,2)) 
		group by r.pkId, r.submittedBy, r.userId, r.userDisplayName, r.userTitle, r.managerUserId, r.managerDisplayName, r.ticketNumber, r.isChanged, r.statusEnum, r.createdDate, r.lastModifiedDate
		order by r.lastModifiedDate desc

	end
	else if @status = 'close'
	begin
	   	select r.* from dbo.SNAP_Requests r , dbo.SNAP_Workflow w, dbo.SNAP_Actors a
		where r.pkid = w.requestId 
			and w.actorid = a.pkid
			and a.userid = @userid
			and (r.statusEnum Not IN(0,1,2))
			and r.lastModifiedDate > getdate()-30
		group by r.pkId, r.submittedBy, r.userId, r.userDisplayName, r.userTitle, r.managerUserId, r.managerDisplayName, r.ticketNumber, r.isChanged, r.statusEnum, r.createdDate, r.lastModifiedDate
		order by r.lastModifiedDate desc

	end

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_details]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_open_my_request_details]
	@userId nvarchar(128)
AS
/*
BEGIN
	SET NOCOUNT ON;
	
	select * from dbo.SNAP_Requests r 
	where (r.userId = @userId or r.submittedBy = @userId) 
		and (r.statusEnum IN(0,1,2)) 
	order by r.pkid

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_details]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_request_details]
	@search nvarchar(100)
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


   		select * from dbo.SNAP_Requests r 
		where (r.userId = @search 
			or r.userDisplayName like('%' + @search + '%')
			or CONVERT(nvarchar(100),r.pkid) =  @search) 
		order by r.lastModifiedDate desc
	

	
END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_details]    Script Date: 10/07/2010 10:25:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_access_team_details]
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if @status = 'open'
	begin
		select * from dbo.SNAP_Requests r 
		where (r.statusEnum IN(0,1,2)) 
		order by r.lastModifiedDate desc

	end
	else if @status = 'close'
	begin
		select * from dbo.SNAP_Requests r 
		where (r.statusEnum Not IN(0,1,2))
		and r.lastModifiedDate > getdate()-0
		order by r.lastModifiedDate desc

	end

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_details]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_request_details]
	@userId nvarchar(128),
	@status nvarchar(10) = 'open'
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if @status = 'open'
   		select * from dbo.SNAP_Requests r 
		where (r.userId = @userId or r.submittedBy = @userId) 
			and (r.statusEnum IN(0,1,2)) 
		order by r.lastModifiedDate desc
	else if @status = 'close'
   		select * from dbo.SNAP_Requests r 
		where (r.userId = @userId or r.submittedBy = @userId) 
			and (r.statusEnum NOT IN(0,1,2)) 
			and r.lastModifiedDate > getdate()-30
		order by r.lastModifiedDate desc
	

	
END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_search_requests]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_search_requests]
	@search nvarchar(100)
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
	EXEC [dbo].[usp_search_request_details] @search
	EXEC [dbo].[usp_search_request_texts] @search
	EXEC [dbo].[usp_search_request_comments] @search
	EXEC [dbo].[usp_search_request_workflow_details] @search
	EXEC [dbo].[usp_search_request_workflow_comments] @search


	
END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_tab]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_request_tab] 
	@userId nvarchar(128)
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	EXEC [dbo].[usp_my_request_details] @userId
	EXEC [dbo].[usp_my_request_texts] @userId
	EXEC [dbo].[usp_my_request_comments] @userId
	EXEC [dbo].[usp_my_request_workflow_details] @userId
	EXEC [dbo].[usp_my_request_workflow_comments] @userId


	EXEC [dbo].[usp_my_request_details] @userId, 'close'
	EXEC [dbo].[usp_my_request_texts] @userId, 'close'
	EXEC [dbo].[usp_my_request_comments] @userId, 'close'
	EXEC [dbo].[usp_my_request_workflow_details] @userId, 'close'
	EXEC [dbo].[usp_my_request_workflow_comments] @userId, 'close'

END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_tab]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_my_approval_tab]
	@userId nvarchar(128)
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	EXEC [dbo].[usp_my_approval_details] @userId
	EXEC [dbo].[usp_my_approval_texts] @userId
	EXEC [dbo].[usp_my_approval_comments] @userId
	EXEC [dbo].[usp_my_approval_workflow_details] @userId
	EXEC [dbo].[usp_my_approval_workflow_comments]


	EXEC [dbo].[usp_my_approval_details] @userId, 'close'
	EXEC [dbo].[usp_my_approval_texts] @userId, 'close'
	EXEC [dbo].[usp_my_approval_comments] @userId, 'close'
	EXEC [dbo].[usp_my_approval_workflow_details] @userId, 'close'
	EXEC [dbo].[usp_my_approval_workflow_comments] 'close'


END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_tab]    Script Date: 10/07/2010 10:25:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_access_team_tab]
	@userId nvarchar(128)
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	EXEC [dbo].[usp_access_team_details]
	EXEC [dbo].[usp_access_team_texts]
	EXEC [dbo].[usp_access_team_comments]
	EXEC [dbo].[usp_access_team_workflow_details]
	EXEC [dbo].[usp_access_team_workflow_comments]


	EXEC [dbo].[usp_access_team_details] 'close'
	EXEC [dbo].[usp_access_team_texts] 'close'
	EXEC [dbo].[usp_access_team_comments] 'close'
	EXEC [dbo].[usp_access_team_workflow_details] 'close'
	EXEC [dbo].[usp_access_team_workflow_comments] 'close'


END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_requests]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_requests]
	@userId nvarchar(128),
	@role nvarchar(10)
AS
/*
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if @role = 'approval' 
		begin
			EXEC [dbo].[usp_my_approval_tab] @userId
		end
	else if @role = 'my'
		begin
			EXEC [dbo].[usp_my_request_tab] @userId
		end
	else if @role = 'accessteam'
		begin
			EXEC [dbo].[usp_access_team_tab] @userId
		end

	
END
*/
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_workflow_details]    Script Date: 10/07/2010 10:25:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_open_my_request_workflow_details]
	@userId nvarchar(128)
AS
/*
BEGIN
	SET NOCOUNT ON;
	
	select f.requestId, s.pkId as workflowStateId, s.workflowId, s.workflowStatusEnum, s.notifyDate, s.dueDate, s.completedDate, a.pkId as actorId, a.actor_groupId, a.userId, a.displayName, a.emailAddress, a.isGroup, a.isDefault, a.isActive, (select ag.actorGroupType from dbo.SNAP_Actor_Group ag where ag.pkId = a.actor_groupId) as actorGroupType
	from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
	where (r.userId = @userId or  r.submittedBy = @userId) 
		and (r.statusEnum IN(0,1,2)) 
		and r.pkid = f.requestId and f.pkid = s.workflowId
		and f.actorid = a.pkid
	order by r.pkid, s.workflowId, s.pkId

END
*/
