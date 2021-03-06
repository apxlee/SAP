USE [BPS_SupplementalAccess]
GO
/****** Object:  StoredProcedure [dbo].[usp_create_weekends_and_holidays_table]    Script Date: 06/03/2010 16:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_create_weekends_and_holidays_table] 

AS
	SET NOCOUNT ON;
	
	IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[SNAP_Weekends_and_Holidays]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
		BEGIN
			PRINT 'Dropping [dbo].[SNAP_Weekends_and_Holidays]'
			DROP TABLE [dbo].[SNAP_Weekends_and_Holidays]
		END
	
	PRINT 'Creating [dbo].[SNAP_Weekends_and_Holidays]'
	CREATE TABLE [dbo].[SNAP_Weekends_and_Holidays](
	[dayOfWeekDate] [datetime] NOT NULL,
	[dayName] [char](3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
	) ON [PRIMARY]
	
	DECLARE @FirstSat datetime, @x int
	SELECT @FirstSat = '1/2/2010', @x = 1 

	PRINT 'Adding 2010 Weekends'
	WHILE @x < 104
		BEGIN
			INSERT INTO [dbo].[SNAP_Weekends_and_Holidays](dayOfWeekDate, dayName)
			SELECT DATEADD(ww,@x,@FirstSat),   'SAT' UNION ALL
			SELECT DATEADD(ww,@x,@FirstSat+1), 'SUN'
			
			IF @x = 52
				BEGIN
					PRINT 'Adding 2011 Weekends'
					SELECT @FirstSat = DATEADD(yy,1,@FirstSat)
				END
			
			SELECT @x = @x + 1
		END

	PRINT 'Adding 2010 and 2011 Apollo Holidays'
	INSERT INTO [dbo].[SNAP_Weekends_and_Holidays](dayOfWeekDate, dayName)
	SELECT '1/1/2010',   'FRI' UNION ALL
	SELECT '1/18/2010',  'MON' UNION ALL
	SELECT '2/15/2010',  'MON' UNION ALL
	SELECT '4/2/2010',   'FRI' UNION ALL
	SELECT '5/31/2010',   'MON' UNION ALL
	SELECT '7/5/2010', 'MON' UNION ALL
	SELECT '9/6/2010', 'MON' UNION ALL
	SELECT '11/25/2010', 'THU' UNION ALL
	SELECT '11/26/2010',   'FRI' UNION ALL
	SELECT '12/24/2010',  'FRI' UNION ALL
	SELECT '12/27/2010',  'MON' UNION ALL
	SELECT '12/31/2010',   'FRI' UNION ALL
	--2011
	SELECT '1/17/2011',  'MON' UNION ALL
	SELECT '2/21/2011',  'MON' UNION ALL
	SELECT '4/22/2011',   'FRI' UNION ALL
	SELECT '5/30/2011',   'MON' UNION ALL
	SELECT '7/4/2011', 'MON' UNION ALL
	SELECT '9/5/2011', 'MON' UNION ALL
	SELECT '11/24/2011', 'THU' UNION ALL
	SELECT '11/25/2011',   'FRI' UNION ALL
	SELECT '12/23/2011',  'FRI' UNION ALL
	SELECT '12/26/2011',  'MON'
	
	SET NOCOUNT OFF;
GO
/****** Object:  Table [dbo].[SNAP_Actor_Group]    Script Date: 06/03/2010 16:25:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Actor_Group](
	[pkId] [int] IDENTITY(1,1) NOT NULL,
	[groupName] [nvarchar](50) NULL,
	[description] [nvarchar](max) NULL,
	[actorGroupType] [tinyint] NULL,
	[isLargeGroup] [bit] NULL CONSTRAINT [DF_SNAP_Actor_Group_isLargeGroup]  DEFAULT ((0)),
	[isActive] [bit] NULL,
 CONSTRAINT [PK_Actor_Group] PRIMARY KEY CLUSTERED 
(
	[pkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNAP_Weekends_and_Holidays]    Script Date: 06/03/2010 16:25:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SNAP_Weekends_and_Holidays](
	[dayOfWeekDate] [datetime] NOT NULL,
	[dayName] [char](3) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SNAP_Comments_Type]    Script Date: 06/03/2010 16:25:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Comments_Type](
	[pkId] [int] NOT NULL,
	[typeName] [nvarchar](50) NOT NULL,
	[audience] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Comments_Type] PRIMARY KEY CLUSTERED 
(
	[pkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNAP_Request_State_Type]    Script Date: 06/03/2010 16:25:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Request_State_Type](
	[pkId] [int] NOT NULL,
	[typeName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Request_State_Type] PRIMARY KEY CLUSTERED 
(
	[pkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNAP_Actor_Group_Type]    Script Date: 06/03/2010 16:25:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Actor_Group_Type](
	[pkId] [int] NULL,
	[typeName] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNAP_Workflow_State_Type]    Script Date: 06/03/2010 16:25:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Workflow_State_Type](
	[pkId] [int] NOT NULL,
	[typeName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Workflow_State_Type] PRIMARY KEY CLUSTERED 
(
	[pkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_access_user_text]    Script Date: 06/03/2010 16:25:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_insert_access_user_text]
(
	@requestId int,
	@access_details_formId int,
	@userText nvarchar(MAX)
)

AS
BEGIN
SET NOCOUNT ON;

	INSERT INTO SNAP_Access_User_Text 
	VALUES(@requestId,@access_details_formId,@userText,GetDate()) 

END
GO
/****** Object:  Table [dbo].[SNAP_Requests]    Script Date: 06/03/2010 16:25:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Requests](
	[pkId] [int] IDENTITY(1,1) NOT NULL,
	[submittedBy] [nvarchar](10) NOT NULL,
	[userId] [nvarchar](10) NOT NULL,
	[userDisplayName] [nvarchar](100) NOT NULL,
	[userTitle] [nvarchar](100) NOT NULL,
	[managerUserId] [nvarchar](10) NOT NULL,
	[managerDisplayName] [nvarchar](100) NOT NULL,
	[ticketNumber] [nvarchar](20) NULL,
	[isChanged] [bit] NOT NULL,
	[statusEnum] [tinyint] NOT NULL,
	[createdDate] [smalldatetime] NOT NULL,
	[lastModifiedDate] [smalldatetime] NOT NULL CONSTRAINT [DF_SNAP_Requests_lastModifiedDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_Request] PRIMARY KEY CLUSTERED 
(
	[pkId] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNAP_Access_Details_Form]    Script Date: 06/03/2010 16:25:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Access_Details_Form](
	[pkId] [int] IDENTITY(1,1) NOT NULL,
	[parentId] [int] NULL,
	[label] [nvarchar](50) NULL,
	[description] [nvarchar](max) NULL,
	[isActive] [bit] NULL,
	[isRequired] [bit] NULL,
 CONSTRAINT [PK_Access_Details_Form] PRIMARY KEY CLUSTERED 
(
	[pkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNAP_Actors]    Script Date: 06/03/2010 16:25:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Actors](
	[pkId] [int] IDENTITY(1,1) NOT NULL,
	[actor_groupId] [int] NOT NULL,
	[userId] [nvarchar](128) NULL,
	[displayName] [nvarchar](128) NOT NULL,
	[emailAddress] [nvarchar](128) NOT NULL,
	[isGroup] [bit] NOT NULL CONSTRAINT [DF_SNAP_Actors_isGroup]  DEFAULT ((0)),
	[isDefault] [bit] NOT NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_Actors] PRIMARY KEY CLUSTERED 
(
	[pkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNAP_Workflow_Comments]    Script Date: 06/03/2010 16:25:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Workflow_Comments](
	[pkId] [int] IDENTITY(1,1) NOT NULL,
	[workflowId] [int] NOT NULL,
	[commentTypeEnum] [tinyint] NOT NULL,
	[commentText] [nvarchar](max) NOT NULL,
	[createdDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED 
(
	[pkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNAP_Workflow_State]    Script Date: 06/03/2010 16:25:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Workflow_State](
	[pkId] [int] IDENTITY(1,1) NOT NULL,
	[workflowId] [int] NOT NULL,
	[workflowStatusEnum] [tinyint] NOT NULL,
	[notifyDate] [smalldatetime] NULL,
	[dueDate] [smalldatetime] NULL,
	[completedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_Workflow_State] PRIMARY KEY CLUSTERED 
(
	[pkId] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNAP_Workflow]    Script Date: 06/03/2010 16:25:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Workflow](
	[pkId] [int] IDENTITY(1,1) NOT NULL,
	[requestId] [int] NOT NULL,
	[actorId] [int] NOT NULL,
 CONSTRAINT [PK_Workflow] PRIMARY KEY CLUSTERED 
(
	[pkId] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNAP_Access_User_Text]    Script Date: 06/03/2010 16:25:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Access_User_Text](
	[pkId] [int] IDENTITY(1,1) NOT NULL,
	[requestId] [int] NOT NULL,
	[access_details_formId] [int] NOT NULL,
	[userText] [nvarchar](max) NULL,
	[modifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_SNAP_Access_User_Text] PRIMARY KEY CLUSTERED 
(
	[pkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNAP_Request_Comments]    Script Date: 06/03/2010 16:25:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNAP_Request_Comments](
	[pkId] [int] IDENTITY(1,1) NOT NULL,
	[requestId] [int] NOT NULL,
	[commentTypeEnum] [tinyint] NOT NULL,
	[commentText] [nvarchar](max) NOT NULL,
	[createdDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_SNAP_Request_Comments] PRIMARY KEY CLUSTERED 
(
	[pkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_workflow_details]    Script Date: 06/03/2010 16:25:32 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_workflow_details]    Script Date: 06/03/2010 16:25:31 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_workflow_details]    Script Date: 06/03/2010 16:25:30 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_workflow_details]    Script Date: 06/03/2010 16:25:29 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_workflow_details]    Script Date: 06/03/2010 16:25:25 ******/
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
		select f.requestId, s.pkId as workflowStateId, s.workflowId, s.workflowStatusEnum, s.notifyDate, s.dueDate, s.completedDate, a.pkId as actorId, a.actor_groupId, a.userId, a.displayName, a.emailAddress, a.isGroup, a.isDefault, a.isActive, (select ag.actorGroupType from dbo.SNAP_Actor_Group ag where ag.pkId = a.actor_groupId) as actorGroupType
		from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
		where (r.statusEnum Not IN(0,1,2)) 
			and r.pkid = f.requestId and f.pkid = s.workflowId
			and f.actorid = a.pkid
			and r.lastModifiedDate > getdate()-7
		order by r.pkid, s.workflowId, s.pkId

	end

END
GO
/****** Object:  StoredProcedure [dbo].[usp_active_manager_check]    Script Date: 06/03/2010 16:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_active_manager_check]
@userId nvarchar(128)

AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @isManager bit;

	IF EXISTS(
	SELECT a.userId
	FROM SNAP_Actors a
	JOIN SNAP_Actor_Group ag
	ON a.actor_groupId = ag.pkId
	WHERE actorGroupType = 2
	AND a.isActive = 1
	AND ag.isActive = 1
	AND a.userId = @userID)
		BEGIN
			SET @isManager = 1
		END
	ELSE 
		BEGIN
			SET @isManager = 0
		END
	
	SELECT @isManager
	
END
GO
/****** Object:  UserDefinedFunction [dbo].[udf_get_next_business_day]    Script Date: 06/03/2010 16:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[udf_get_next_business_day]
( @startDate smalldatetime,
	@numDays int )
RETURNS smalldatetime
AS
BEGIN

DECLARE @returnDate smalldatetime
DECLARE @endDate smalldatetime
DECLARE @dayCount int

SET @dayCount = 0

SET @endDate = DATEADD(dd,1,@startDate)
		WHILE @dayCount < @numDays
			BEGIN
				SET @returnDate = NULL
				WHILE @returnDate IS NULL
					BEGIN
						IF EXISTS (SELECT wh.dayOfWeekDate 
							FROM dbo.SNAP_Weekends_and_Holidays wh
							WHERE DATEDIFF(day, wh.dayOfWeekDate, @endDate) = 0)
							SET @endDate = DATEADD(dd,1,@endDate)
						ELSE
							SET @returnDate = @endDate
					END
				SET @endDate = DATEADD(dd,1,@endDate)
				SET @dayCount = @dayCount + 1 
			END
	RETURN (@returnDate)
END
GO
/****** Object:  StoredProcedure [dbo].[usp_build_status_tracking]    Script Date: 06/03/2010 16:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_build_status_tracking]
@requestId int

AS
BEGIN
	SET NOCOUNT ON;
	
	select w.requestId, ws.workflowId, 
	a.userId, a.displayName, wst.typeName, 
	ws.notifyDate, ws.dueDate, ws.completedDate,
	wc.workflowId, wc.commentText
	from SNAP_workflow w
	join SNAP_workflow_state ws
	on w.pkId = ws.workflowId
	join SNAP_actors a
	on w.actorId = a.pkId
	join SNAP_workflow_state_type wst
	on ws.workflowStatusEnum = wst.pkId
	left outer join SNAP_Workflow_Comments wc
	on w.pkId = wc.workflowId
	where w.requestId = @requestId

END
GO
/****** Object:  StoredProcedure [dbo].[usp_open_access_team_details_old]    Script Date: 06/03/2010 16:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_open_access_team_details_old]

AS
BEGIN
SET NOCOUNT ON;
	
SELECT r.pkId as requestId,
r.submittedBy, 
r.userId, 
r.userDisplayName,
r.userTitle,
r.managerUserId,
r.managerDisplayName,
adf.pkId as fieldId,
adf.label as fieldLabel,
aut.userText as fieldText, 
r.ticketNumber, 
r.isChanged, 
r.statusEnum, 
r.createdDate,
rc.commentTypeEnum,
rc.commentText
FROM SNAP_Requests r
JOIN SNAP_Access_User_Text aut
ON r.pkId = aut.requestId
JOIN SNAP_Access_Details_Form adf
ON aut.access_details_formId = adf.pkId
LEFT OUTER JOIN SNAP_Request_Comments rc
ON r.pkId = rc.requestId
WHERE r.statusEnum IN(0,1,2)
AND aut.modifiedDate = 
(select MAX(modifiedDate) 
from dbo.SNAP_Access_User_Text aut2 
where aut.access_details_formId = aut2.access_details_formId 
and requestId = r.pkId)
ORDER BY r.pkId Asc

END
GO
/****** Object:  StoredProcedure [dbo].[usp_open_request_details]    Script Date: 06/03/2010 16:25:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_open_request_details]
(
	@userId nvarchar(128),
	@requestId int
)

AS
BEGIN
SET NOCOUNT ON;
	
SELECT r.pkId as requestId,
r.submittedBy, 
r.userId, 
r.userDisplayName,
r.userTitle,
r.managerUserId,
r.managerDisplayName,
adf.pkId as fieldId,
adf.label as fieldLabel,
aut.userText as fieldText,
aut.modifiedDate, 
r.ticketNumber, 
r.isChanged, 
r.statusEnum, 
r.createdDate
FROM SNAP_Requests r
JOIN SNAP_Access_User_Text aut
ON r.pkId = aut.requestId
JOIN SNAP_Access_Details_Form adf
ON aut.access_details_formId = adf.pkId
WHERE r.pkId = @requestId
AND (r.statusEnum = 1)
AND (adf.isActive = 1)
AND aut.modifiedDate = 
(select MAX(modifiedDate) 
from dbo.SNAP_Access_User_Text aut2 
where aut.access_details_formId = aut2.access_details_formId 
and requestId = @requestId)
AND (r.submittedBy = @userId
OR r.userId = @userId)
ORDER BY r.pkId Asc

END
GO
/****** Object:  StoredProcedure [dbo].[usp_request_details]    Script Date: 06/03/2010 16:25:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_request_details]
(
	@requestId int
)

AS
BEGIN
SET NOCOUNT ON;
	
SELECT r.pkId as requestId,
r.submittedBy, 
r.userId, 
r.userDisplayName,
r.userTitle,
r.managerUserId,
r.managerDisplayName,
adf.pkId as fieldId,
adf.label as fieldLabel,
aut.userText as fieldText,
aut.modifiedDate, 
r.ticketNumber, 
r.isChanged, 
r.statusEnum, 
r.createdDate
FROM SNAP_Requests r
JOIN SNAP_Access_User_Text aut
ON r.pkId = aut.requestId
JOIN SNAP_Access_Details_Form adf
ON aut.access_details_formId = adf.pkId
WHERE r.pkId = @requestId
AND (adf.isActive = 1)
AND aut.modifiedDate = 
(select MAX(modifiedDate) 
from dbo.SNAP_Access_User_Text aut2 
where aut.access_details_formId = aut2.access_details_formId 
and requestId = @requestId)
ORDER BY adf.pkId asc

END
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_texts]    Script Date: 06/03/2010 16:25:32 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_texts]    Script Date: 06/03/2010 16:25:28 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_texts]    Script Date: 06/03/2010 16:25:24 ******/
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
			and r.lastModifiedDate > getdate()-7
		group by access_details_formId, requestID

		select a.* 
		from snap_access_user_text a, #access_temp2 t
		where a.requestId = t.requestId and a.access_details_formId = t.access_details_formId and a.modifiedDate = t.maxDate
		order by a.requestId, a.access_details_formId

		drop table #access_temp2
	end
	

END
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_texts]    Script Date: 06/03/2010 16:25:29 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_comments]    Script Date: 06/03/2010 16:25:24 ******/
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
		and r.lastModifiedDate > getdate()-7

	end

END
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_comments]    Script Date: 06/03/2010 16:25:29 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_comments]    Script Date: 06/03/2010 16:25:31 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_comments]    Script Date: 06/03/2010 16:25:30 ******/
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
BEGIN
	SET NOCOUNT ON;
	
	select c.* from dbo.SNAP_Requests r 
	JOIN  dbo.SNAP_Request_Comments c on
	r.pkid = c.requestId
	where (r.userId = @userId or  r.submittedBy = @userId) 
	and (r.statusEnum IN(0,1,2)) 

END
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_comments]    Script Date: 06/03/2010 16:25:28 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_delete_workflow]    Script Date: 06/03/2010 16:25:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_delete_workflow]
(
	@workflowId int
)

AS
BEGIN
SET NOCOUNT ON;

	--Delete Workflow Approver
	DELETE FROM SNAP_Workflow
	WHERE pkId = @workflowId
	
	--Delete Approver States
	DELETE FROM SNAP_Workflow_State
	WHERE workflowId = @workflowId

	--Delete Approver Comments
	DELETE FROM SNAP_Workflow_Comments
	WHERE workflowId = @workflowId

END
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_workflow_comments]    Script Date: 06/03/2010 16:25:30 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_workflow_comments]    Script Date: 06/03/2010 16:25:32 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_approval_status]    Script Date: 06/03/2010 16:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_open_my_approval_status]
(
	@userId nvarchar(128)
)

AS
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
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_workflow_comments]    Script Date: 06/03/2010 16:25:30 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_workflow_comments]    Script Date: 06/03/2010 16:25:25 ******/
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
			and r.lastModifiedDate > getdate()-7
		order by r.pkid, c.workflowId

	end
	

END
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_workflow_comments]    Script Date: 06/03/2010 16:25:29 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_details]    Script Date: 06/03/2010 16:25:28 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_workflow_comments]    Script Date: 06/03/2010 16:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_insert_workflow_comments]
(
	@workflowId int,
	@commentTypeEnum tinyint,
	@commentText nvarchar(MAX)
)

AS
BEGIN
SET NOCOUNT ON;

	INSERT INTO SNAP_Workflow_Comments(workflowId,commentTypeEnum,commentText,createdDate)
	VALUES(@workflowId,@commentTypeEnum,@commentText,GETDATE())

END
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_workflow_state]    Script Date: 06/03/2010 16:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_insert_workflow_state]
(
	@workflowId int,
	@workflowStatusEnum tinyint,
	@notifyDate smalldatetime = NULL
)

AS
BEGIN
SET NOCOUNT ON;
	
	DECLARE @dueDate smalldatetime
	--set dueDate value
	if (@notifyDate IS NOT NULL)
		begin
			select @dueDate =  dateadd(day,1,@notifyDate);
			--TODO use HolidayWeekend function to get dueDate 
		end
	else
		begin
			set @dueDate = NULL
		end
	
	INSERT INTO SNAP_Workflow_State(workflowId,workflowStatusEnum,notifyDate,dueDate)
	VALUES(@workflowId,@workflowStatusEnum,@notifyDate,@dueDate)

END
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_actor]    Script Date: 06/03/2010 16:25:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_insert_actor]
(
	@actor_groupId int,
	@userId nvarchar(128),
	@displayName nvarchar(128),
	@isDefault bit	
)

AS
BEGIN
SET NOCOUNT ON;
	
	IF NOT EXISTS(select * from SNAP_Actors where actor_groupId = @actor_groupId and userId = @userId)
		BEGIN	
			INSERT INTO SNAP_Actors(actor_groupId,userId,displayName,emailAddress,isDefault,isActive)
			VALUES(@actor_groupId,@userId,@displayName,@userId + '@apollogrp.edu',@isDefault,1)
		END 

END
GO
/****** Object:  StoredProcedure [dbo].[usp_open_my_request_details]    Script Date: 06/03/2010 16:25:30 ******/
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
BEGIN
	SET NOCOUNT ON;
	
	select * from dbo.SNAP_Requests r 
	where (r.userId = @userId or r.submittedBy = @userId) 
		and (r.statusEnum IN(0,1,2)) 
	order by r.pkid

END
GO
/****** Object:  StoredProcedure [dbo].[usp_search_request_details]    Script Date: 06/03/2010 16:25:32 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_details]    Script Date: 06/03/2010 16:25:24 ******/
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
		and r.lastModifiedDate > getdate()-7
		order by r.lastModifiedDate desc

	end

END
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_details]    Script Date: 06/03/2010 16:25:29 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_build_request_form]    Script Date: 06/03/2010 16:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_build_request_form]
AS
BEGIN
	SET NOCOUNT ON;
	-- building the request form requires two selects...

	-- outer repeater
	select * from SNAP_Access_Details_Form
	where parentId = 0 and isActive = 1

	-- inner repeater
	select * from SNAP_Access_Details_Form
	where parentId = 1 and isActive = 1
	order by pkId

END
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_request_xml]    Script Date: 06/03/2010 16:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_insert_request_xml] 
--//'<ROOT><request_data access_details_formId="1" userText="this is the input text"></request_data></ROOT>'
@request_data as xml,
@submittedBy nvarchar(10),
@userId nvarchar(10),
@userDisplayName nvarchar(100),
@userTitle nvarchar(100),
@managerUserId nvarchar(10),
@managerDisplayName nvarchar(100)


AS
BEGIN
	SET NOCOUNT ON;
	
	declare @requestId int
	
	insert into SNAP_Requests(submittedBy,userId,userDisplayName,userTitle,managerUserId,managerDisplayName,isChanged,statusEnum,createdDate)
	values (@submittedBy,@userId,@userDisplayName,@userTitle,@managerUserId,@managerDisplayName,0,0,GetDate());
	select @requestId = scope_identity();
	
	declare @idoc int
	exec sp_xml_preparedocument @idoc OUTPUT, @request_data

	select access_details_formId,userText into #tRequest 
	from openxml(@idoc, '/ROOT/request_data',1)
	with
	(
		access_details_formId int,
		userText nvarchar(max)
	)
	
	--cursor to read data from temp table
	declare cu_xml cursor for select access_details_formId,userText from #tRequest    
    declare @access_details_formId int
	declare	@userText nvarchar(max)    

	--Open cursor
    open cu_xml

	--fetch the temp table data row by row
    fetch cu_xml into @access_details_formId,@userText                

    --while loop is used to check the end of cursor is reached or not
    while (@@fetch_status = 0)
		begin
            --insert the values into access_user_text table.
			EXEC [dbo].[usp_insert_access_user_text] @requestId,@access_details_formId,@userText     
            fetch cu_xml into @access_details_formId,@userText
        end
    
	--close the cursor and deallocate.
    close cu_xml
    deallocate cu_xml

	--insert default access team workflow record and return workflow id
	declare @workflowId int
	declare @currentDate smalldatetime

	set @currentDate = GETDATE()

	insert into SNAP_Workflow values(@requestId,1)
	select @workflowId = scope_identity();
	
	--insert default access team workflow_state
	EXEC [dbo].[usp_insert_workflow_state] @workflowId,6,@currentDate

	--insert manager into actors table
	EXEC [dbo].[usp_insert_actor] 4,@managerUserId,@managerDisplayName,1
	
	--return pkID
	RETURN @requestId
	
END
GO
/****** Object:  StoredProcedure [dbo].[usp_insert_workflow_xml]    Script Date: 06/03/2010 16:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_insert_workflow_xml] 
--//'<ROOT><workflow_data actorId="1" workflowStatusEnum="0" notifyDate="02/24/2010 11:30:20"></workflow_data></ROOT>'
@workflow_data as xml,
@requestId int

AS
BEGIN
	SET NOCOUNT ON;

	declare @idoc int
	exec sp_xml_preparedocument @idoc OUTPUT, @workflow_data

	select actorId,workflowStatusEnum,notifyDate into #tWorkflow
	from openxml(@idoc, '/ROOT/workflow_data',1)
	with
	(
		actorId int,
		workflowStatusEnum tinyint,
		notifyDate smalldatetime
	)
--	
	--cursor to read data from temp table
	declare cu_xml cursor for select actorId,workflowStatusEnum,notifyDate from #tWorkflow
    declare @actorId int
	declare @workflowStatusEnum tinyint
	declare	@notifyDate smalldatetime
	declare @workflowId int

	--Open cursor
    open cu_xml

	--fetch the temp table data row by row
    fetch cu_xml into @actorId,@workflowStatusEnum,@notifyDate                

    --while loop is used to check the end of cursor is reached or not
    while (@@fetch_status = 0)
		begin
			
            --insert the values into workflow table and return pkId.
			insert into SNAP_Workflow(requestId,actorId)
			values(@requestId,@actorId)
			select @workflowId = scope_identity();

			EXEC [dbo].[usp_insert_workflow_state] @workflowId,@workflowStatusEnum,@notifyDate

            fetch cu_xml into @actorId,@workflowStatusEnum,@notifyDate     
        end
    
	--close the cursor and deallocate.
    close cu_xml
    deallocate cu_xml
	
END
GO
/****** Object:  StoredProcedure [dbo].[usp_search_requests]    Script Date: 06/03/2010 16:25:32 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_my_request_tab]    Script Date: 06/03/2010 16:25:29 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_my_approval_tab]    Script Date: 06/03/2010 16:25:28 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_access_team_tab]    Script Date: 06/03/2010 16:25:24 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[usp_open_request_tab]    Script Date: 06/03/2010 16:25:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_open_request_tab]
(
	@userId nvarchar(128),
	@requestId int
)

AS
BEGIN
SET NOCOUNT ON;
	
EXEC [dbo].[usp_open_request_details] @userId, @requestId 

END
GO
/****** Object:  StoredProcedure [dbo].[usp_requests]    Script Date: 06/03/2010 16:25:31 ******/
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
GO
/****** Object:  ForeignKey [FK_Access_User_Text_Access_Details_Form]    Script Date: 06/03/2010 16:25:35 ******/
ALTER TABLE [dbo].[SNAP_Access_User_Text]  WITH NOCHECK ADD  CONSTRAINT [FK_Access_User_Text_Access_Details_Form] FOREIGN KEY([access_details_formId])
REFERENCES [dbo].[SNAP_Access_Details_Form] ([pkId])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[SNAP_Access_User_Text] NOCHECK CONSTRAINT [FK_Access_User_Text_Access_Details_Form]
GO
/****** Object:  ForeignKey [FK_Access_User_Text_Request]    Script Date: 06/03/2010 16:25:36 ******/
ALTER TABLE [dbo].[SNAP_Access_User_Text]  WITH NOCHECK ADD  CONSTRAINT [FK_Access_User_Text_Request] FOREIGN KEY([requestId])
REFERENCES [dbo].[SNAP_Requests] ([pkId])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[SNAP_Access_User_Text] NOCHECK CONSTRAINT [FK_Access_User_Text_Request]
GO
/****** Object:  ForeignKey [FK_Actors_Actor_Group]    Script Date: 06/03/2010 16:25:40 ******/
ALTER TABLE [dbo].[SNAP_Actors]  WITH NOCHECK ADD  CONSTRAINT [FK_Actors_Actor_Group] FOREIGN KEY([actor_groupId])
REFERENCES [dbo].[SNAP_Actor_Group] ([pkId])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[SNAP_Actors] NOCHECK CONSTRAINT [FK_Actors_Actor_Group]
GO
/****** Object:  ForeignKey [FK_SNAP_Request_Comments_SNAP_Requests]    Script Date: 06/03/2010 16:25:42 ******/
ALTER TABLE [dbo].[SNAP_Request_Comments]  WITH NOCHECK ADD  CONSTRAINT [FK_SNAP_Request_Comments_SNAP_Requests] FOREIGN KEY([requestId])
REFERENCES [dbo].[SNAP_Requests] ([pkId])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[SNAP_Request_Comments] NOCHECK CONSTRAINT [FK_SNAP_Request_Comments_SNAP_Requests]
GO
/****** Object:  ForeignKey [FK_Workflow_Actors]    Script Date: 06/03/2010 16:25:48 ******/
ALTER TABLE [dbo].[SNAP_Workflow]  WITH NOCHECK ADD  CONSTRAINT [FK_Workflow_Actors] FOREIGN KEY([actorId])
REFERENCES [dbo].[SNAP_Actors] ([pkId])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[SNAP_Workflow] NOCHECK CONSTRAINT [FK_Workflow_Actors]
GO
/****** Object:  ForeignKey [FK_Workflow_Request]    Script Date: 06/03/2010 16:25:48 ******/
ALTER TABLE [dbo].[SNAP_Workflow]  WITH NOCHECK ADD  CONSTRAINT [FK_Workflow_Request] FOREIGN KEY([requestId])
REFERENCES [dbo].[SNAP_Requests] ([pkId])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[SNAP_Workflow] NOCHECK CONSTRAINT [FK_Workflow_Request]
GO
/****** Object:  ForeignKey [FK_Comments_Workflow]    Script Date: 06/03/2010 16:25:49 ******/
ALTER TABLE [dbo].[SNAP_Workflow_Comments]  WITH NOCHECK ADD  CONSTRAINT [FK_Comments_Workflow] FOREIGN KEY([workflowId])
REFERENCES [dbo].[SNAP_Workflow] ([pkId])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[SNAP_Workflow_Comments] NOCHECK CONSTRAINT [FK_Comments_Workflow]
GO
/****** Object:  ForeignKey [FK_Workflow_State_Workflow]    Script Date: 06/03/2010 16:25:51 ******/
ALTER TABLE [dbo].[SNAP_Workflow_State]  WITH NOCHECK ADD  CONSTRAINT [FK_Workflow_State_Workflow] FOREIGN KEY([workflowId])
REFERENCES [dbo].[SNAP_Workflow] ([pkId])
NOT FOR REPLICATION
GO
ALTER TABLE [dbo].[SNAP_Workflow_State] NOCHECK CONSTRAINT [FK_Workflow_State_Workflow]
GO