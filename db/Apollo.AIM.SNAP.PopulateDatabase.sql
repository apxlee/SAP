USE [Apollo.AIM.SNAP]

--// Insert Form Labels and Descriptions
print 'Insert labels and descriptions'
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(0,'Access Details','<p>Please complete one or more of the following sections.</p>',1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(1,'Windows Servers','<p>Please provide a list of the servers you require access to, including domain name, and specify access type: Remote Desktop (RDP) or Administrator.</p><p>NOTE: Administrative Access includes RDP.</p><p><a href="#" class="tooltip" rel="images/example_windows_1.png">Example 1</a>&nbsp;&nbsp;<a href="#" class="tooltip" rel="images/example_windows_2.png">Example 2</a></p>',1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(1,'Linux/Unix Servers','<p>Please provide a list of the servers you require access to, including domain name, and specify access type: Read-Only, Sudo (to a specific group or user), or Root privilege.</p><p><a href="#" class="tooltip" rel="images/example_linuxunix_1.png">Example 1</a></p>',1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(1,'Network Shares','<p>Please provide the name of the server, including domain name, and the name of the shared drive.</p><p><a href="#" class="tooltip" rel="images/example_shareddrive_1.png">Example 1</a></p>',1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(1,'SQL Databases','<p>Please follow the database format below, for SQL, to ensure your request is processed as quickly as possible.</p><br /><p><b>SQL Database Access Format</b></p><p>Server Name\Instance Name (if clustered):</p><p>Table Name, following the format (database.schema.table name):</p><p>Access Type Per Table (Select, Delete, Update, Insert, Execute):</p><p>Server-Level Access Required (Yes/No-if Yes, specify RDP ): </p><br /><p>NOTE: If you are requesting SQL Server Database access and you do not already have one, you must request an a.account and specify which domain(s).</p><p><a href="#" class="tooltip" rel="images/example_SQL.png">Example 1</a></p>',1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(1,'Oracle Databases','<p>Please follow the database format below, for Oracle, to ensure your request is processed as quickly as possible.</p><br /><p><b>Oracle Database Access Format</b></p><p>Username:</p><p>Server/Database Name:</p><p>Environment (Prod, Dev, QA):</p><p>Role: (you may reference a teammate''s user name to clone that role)</p><br /><p>NOTE: For Oracle Database access, your regular user name is used to log in.</p><p><a href="#" class="tooltip" rel="images/example_oracle.png">Example 1</a></p>',1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(0,'Justification','<p>Specifically state why you require the requested access. "I need this to do my job" or "My manager told me to get this access" are NOT valid justifications.  Please be specific to aid in faster approval time.</p>',1,1)

--// Insert Actor Group Types
print 'Insert actor group types'
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actor_Group_Type]([pkId],[typeName])
VALUES(0,'Team Approver')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actor_Group_Type]([pkId],[typeName])
VALUES(1,'Technical Approver')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actor_Group_Type]([pkId],[typeName])
VALUES(2,'Manager')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actor_Group_Type]([pkId],[typeName])
VALUES(3,'Workflow Admin')

--// Insert Actor Groups
print 'Insert actor groups'
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isActive])
VALUES('Access & Identity Mangement',NULL,3,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isActive])
VALUES('Software Group',NULL,0,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isActive])
VALUES('Windows',NULL,1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isActive])
VALUES('Manager',NULL,2,1)

--// Insert Actors
print 'Insert actors'
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(1,'','Access & Identity Management','snap@snap.com',1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(1,'clschwim','Chris Schwimmer','clschwim@apollogrp.edu',1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(1,'jdsteele','Jonathan Steele','jdsteele@apollogrp.com',0,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(1,'pxlee','Pong Lee','pxlee@apollogrp.com',0,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(1,'sxfinkel','Susan Finkel','sxfinkel@apollogrp.com',0,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(1,'cemccorm','Caitlin McCormick','cemccorm@apollogrp.com',0,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(2,'magarrig','Mark Garrigian','magarrig@apollogrp.com',1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(2,'pxlee','Pong Lee','pxlee@apollogrp.com',0,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(2,'clschwim','Chris Schwimmer','clschwim@apollogrp.edu',0,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(3,'pxlee','Pong Lee','pxlee@apollogrp.com',1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(3,'jdsteele','Jonathan Steele','jdsteele@apollogrp.com',0,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(3,'clschwim','Chris Schwimmer','clschwim@apollogrp.edu',0,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(4,'gjbelang','Greg Belanger','gjbelang@apollogrp.edu',1,1)
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isDefault],[isActive])
VALUES(4,'pxlee','Pong Lee','pxlee@apollogrp.edu',0,1)

--// Insert Request State Types
print 'Insert request state types'
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Request_State_Type]([pkId],[typeName])
VALUES(0,'open')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Request_State_Type]([pkId],[typeName])
VALUES(1,'change requested')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Request_State_Type]([pkId],[typeName])
VALUES(2,'pending')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Request_State_Type]([pkId],[typeName])
VALUES(3,'closed')

--// Insert Comments Types
print 'Insert comment types'
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(0,'deny','everyone')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(1,'cancel','everyone')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(2,'request change','everyone')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(3,'email nag','everyone')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(4,'access notes','aim')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(5,'access notes','approvers')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(6,'access notes','everyone')

--// Insert Workflow State Types
print 'Insert workflow state types'
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(0,'approved')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(1,'change requested')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(2,'closed: cancelled')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(3,'closed: completed')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(4,'closed: denied')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(5,'not active')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(6,'pending: acknowledgement')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(7,'pending: approval')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(8,'pending: provisioning')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(9,'pending: workflow')
INSERT INTO [Apollo.AIM.SNAP].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(10,'workflow created')

--// Insert 2 years of Holidays and Weekends
print 'Insert holidays and weekends'
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