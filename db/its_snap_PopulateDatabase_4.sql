USE [its_snap]
declare @group_id int

print 'Reseed SNAP_Requests'
DBCC CHECKIDENT ("dbo.SNAP_Requests", RESEED, 1000);
GO

--// Insert Form Labels and Descriptions
print 'Insert labels and descriptions'
INSERT INTO [its_snap].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(0,'Access Details','<p>Please complete one or more of the following sections.</p>',1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(1,'Windows Servers/Role Membership','<p>For Windows Servers, please provide a list of the servers you require access to, including domain name, and specify access type: Remote Desktop (RDP) or Administrator. If you require Role Membership, please list the roles on separate lines.&nbsp;&nbsp;<em>NOTE: Administrative Access includes RDP. All access will be provisioned via a domain-specific "a.account" which will be created for you as required.</em></p><p><a href="#" class="tooltip" rel="images/example_windows_1.png">Example 1</a>&nbsp;&nbsp;<a href="#" class="tooltip" rel="images/example_windows_2.png">Example 2</a></p>',1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(1,'Linux/Unix Servers','<p>Please provide a list of the servers you require access to, including domain name, and specify access type: Read-Only, Sudo (to a specific group or user), or Root privilege.&nbsp;&nbsp;<em>NOTE: All access will be provisioned via a domain-specific "a.account" which will be created for you as required.</em></p><p><a href="#" class="tooltip" rel="images/example_linuxunix_1.png">Example 1</a></p>',1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(1,'Network Shares','<p>Please provide the name of the server, including domain name, and the name of the shared drive, along with required permissions.</p><p><a href="#" class="tooltip" rel="images/example_shareddrive_1.png">Example 1</a></p>',1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(1,'SQL Databases','<p>Please follow the SQL Database format below to ensure your request is processed as quickly as possible.&nbsp;&nbsp;<em>NOTE: SQL Server Database access requires an "a.account".</em></p><p style="margin-top:10px;line-height:1.5em;">&nbsp;&nbsp;&nbsp;Server Name&nbsp;\&nbsp;Instance Name (if clustered)<br/>&nbsp;&nbsp;&nbsp;Table Name - Please use the format [database].[schema].[table name]<br/>&nbsp;&nbsp;&nbsp;Access Type Per Table - Select | Delete | Update | Insert | Execute<br/>&nbsp;&nbsp;&nbsp;Server-Level Access Required - Yes (specify RDP) | No</p><p style="margin-top:10px;"><a href="#" class="tooltip" rel="images/example_SQL.png">Example 1</a></p>',1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(1,'Oracle Databases','<p>Please follow the Oracle Database format below to ensure your request is processed as quickly as possible.&nbsp;&nbsp;<em>NOTE: For Oracle Database access, your regular username is used to log in (instead of your "a.account"). Server-level access will not be provided with an Oracle Database access request; however, we need the server name to validate your request.</em></p><p style="margin-top:10px;line-height:1.5em;">&nbsp;&nbsp;&nbsp;Username<br />&nbsp;&nbsp;&nbsp;Server/Database Name<br />&nbsp;&nbsp;&nbsp;Environment (PROD, DEV, QA)<br />&nbsp;&nbsp;&nbsp;Role <em>(NOTE: You may reference a teammate''s user name to clone that role)</em></p><p style="margin-top:10px;"><a href="#" class="tooltip" rel="images/example_oracle.png">Example 1</a></p>',1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Access_Details_Form]([parentId],[label],[description],[isActive],[isRequired])
VALUES(0,'Justification','<p>Specifically state why you require the requested access. "I need this to do my job" or "My manager told me to get this access" are NOT valid justifications.  Please be specific to aid in faster approval time.</p>',1,1)

print 'Insert groups and actors'
print ''
print 'Access & Identity Management'
--//Insert Access & Identity Management Group
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isLargeGroup],[isActive])
VALUES('Access & Identity Mangement',NULL,3,0,1)
select @group_id = scope_identity();

--//Insert Access & Identity Management Members
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'','Access & Identity Management','AccessManagementAccessAdmin@apollogrp.edu',0,1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'hjdeiter','Heather Deitermann','hjdeiter@apollogrp.edu',0,1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'mbrunell','Martin Brunelle','mbrunell@apollogrp.com',0,0,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'mxhaney','Milo Haney','mxhaney@apollogrp.com',0,0,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'ablang','Toni Keller','ablang@apollogrp.com',0,0,1)
--//Remove below users after testing
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'jdsteele','Jonathan Steele','jdsteele@apollogrp.com',0,0,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'pxlee','Pong Lee','pxlee@apollogrp.com',0,0,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'sxfinkel','Susan Finkel','sxfinkel@apollogrp.com',0,0,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'cemccorm','Caitlin McCormick','cemccorm@apollogrp.com',0,0,1)
print ''
print 'Software'
--//Insert Software Group
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isLargeGroup],[isActive])
VALUES('Software Group',NULL,0,0,1)
select @group_id = scope_identity();
--//Insert Software Group Members
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'cxambadi','Chandra Ambadipudi ','cxambadi@apollogrp.com',0,1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'jdnatoli','Joe Natoli','jdnatoli@apollogrp.com',0,0,1)

print ''
print 'Access Owner'
--//Insert Access Owner Group
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isLargeGroup],[isActive])
VALUES('Access Owner',NULL,1,0,1)
select @group_id = scope_identity();
--//Insert Access Owner Members
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'gjbelang','Greg Belanger','gjbelang@apollogrp.com',0,1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'nnlorest','Nereo Loresto','nnlorest@apollogrp.com',0,0,1)

print ''
print 'Managers'  --// ATTENTION MUST BE 4th!!!
--//Insert Manager Group
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isLargeGroup],[isActive])
VALUES('Manager',NULL,2,0,1)
select @group_id = scope_identity();
--//Insert Manager Members
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'gjbelang','Greg Belanger','gjbelang@apollogrp.edu',0,0,1)

print ''
print 'Active Directory'
--//Insert Active Directory Group
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isLargeGroup],[isActive])
VALUES('Active Directory',NULL,1,0,1)
select @group_id = scope_identity();
--//Insert Active Directory Members
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'jjwelsh','Josh Welsh','jjwelsh@apollogrp.com',0,1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'galdrich','Greg Aldrich','galdrich@apollogrp.com',0,0,1)

print ''
print 'Owners'
--//Insert Owners Group
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isLargeGroup],[isActive])
VALUES('Owners','This section allows you to add service and application owners as ''shotgun'' actors.',1,1,1)
select @group_id = scope_identity();
--//Insert Owners Members
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'pahardin','Aaron Hardin','pahardin@apollogrp.com',0,0,1)

print ''
print 'ITOC'
--//Insert IT Operations Center Group
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isLargeGroup],[isActive])
VALUES('IT Operations Center',NULL,1,0,1)
select @group_id = scope_identity();
--//Insert IT Operations Center Members
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'DG C-APO-Corporate-ITOC Managers','IT Managers','DGC-APO-Corporate-ITOCManagers@apollogrp.edu',1,1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'mgsampso','Michael Sampson','mgsampso@apollogrp.edu',0,0,1)

print ''
print 'MS SQL'
--//Insert MS SQL Group
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isLargeGroup],[isActive])
VALUES('MS SQL',NULL,1,0,1)
select @group_id = scope_identity();
--//Insert MS SQL Members
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'smstegma','Steve Stegman','smstegma@apollogrp.edu',0,1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'vkommine','Venkat Kommineni','vkommine@apollogrp.edu',0,0,1)

print ''
print 'Oracle BSD/CSD'
--//Insert Oracle BSD/CSD Group
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isLargeGroup],[isActive])
VALUES('Oracle BSD/CSD',NULL,1,0,1)
select @group_id = scope_identity();
--//Insert Oracle BSD/CSD Members
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'clewis1','Christian Lewis','clewis1@apollogrp.edu',0,1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'vkommine','Venkat Kommineni','vkommine@apollogrp.edu',0,0,1)

print ''
print 'Oracle FSG'
--//Insert Oracle FSG Group
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isLargeGroup],[isActive])
VALUES('Oracle FSG',NULL,1,0,1)
select @group_id = scope_identity();
--//Insert Oracle FSG Members
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'rwharkin','Ray Harkins','rwharkin@apollogrp.edu',0,1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'vkommine','Venkat Kommineni','vkommine@apollogrp.edu',0,0,1)

print ''
print 'IT Director'
--//Insert IT Director Group
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group]([groupName],[description],[actorGroupType],[isLargeGroup],[isActive])
VALUES('IT Director',NULL,1,0,1)
select @group_id = scope_identity();
--//Insert IT Director Members
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'bxpatel','Bhavesh Patel','bxpatel@apollogrp.com',0,1,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'jdnatoli','Joe Natoli','jdnatoli@apollogrp.com',0,0,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'vkommine','Venkat Kommineni','vkommine@apollogrp.edu',0,0,1)
INSERT INTO [its_snap].[dbo].[SNAP_Actors]([actor_groupId],[userId],[displayName],[emailAddress],[isGroup],[isDefault],[isActive])
VALUES(@group_id,'obharris','Niel Harris','obharris@apollogrp.com',0,0,1)

--// Insert Actor Group Types
print ''
print 'Insert actor group types'
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group_Type]([pkId],[typeName])
VALUES(0,'Team Approver')
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group_Type]([pkId],[typeName])
VALUES(1,'Technical Approver')
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group_Type]([pkId],[typeName])
VALUES(2,'Manager')
INSERT INTO [its_snap].[dbo].[SNAP_Actor_Group_Type]([pkId],[typeName])
VALUES(3,'Workflow Admin')

--// Insert Request State Types
print ''
print 'Insert request state types'
INSERT INTO [its_snap].[dbo].[SNAP_Request_State_Type]([pkId],[typeName])
VALUES(0,'open')
INSERT INTO [its_snap].[dbo].[SNAP_Request_State_Type]([pkId],[typeName])
VALUES(1,'change requested')
INSERT INTO [its_snap].[dbo].[SNAP_Request_State_Type]([pkId],[typeName])
VALUES(2,'pending')
INSERT INTO [its_snap].[dbo].[SNAP_Request_State_Type]([pkId],[typeName])
VALUES(3,'closed')

--// Insert Comments Types
print ''
print 'Insert comment types'
INSERT INTO [its_snap].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(0,'deny','everyone')
INSERT INTO [its_snap].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(1,'cancel','everyone')
INSERT INTO [its_snap].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(2,'request change','everyone')
INSERT INTO [its_snap].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(3,'email nag','everyone')
INSERT INTO [its_snap].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(4,'access notes','aim')
INSERT INTO [its_snap].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(5,'access notes','approvers')
INSERT INTO [its_snap].[dbo].[SNAP_Comments_Type]([pkId],[typeName],[audience])
VALUES(6,'access notes','everyone')

--// Insert Workflow State Types
print ''
print 'Insert workflow state types'
INSERT INTO [its_snap].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(0,'approved')
INSERT INTO [its_snap].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(1,'change requested')
INSERT INTO [its_snap].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(2,'closed: cancelled')
INSERT INTO [its_snap].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(3,'closed: completed')
INSERT INTO [its_snap].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(4,'closed: denied')
INSERT INTO [its_snap].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(5,'not active')
INSERT INTO [its_snap].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(6,'pending: acknowledgement')
INSERT INTO [its_snap].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(7,'pending: approval')
INSERT INTO [its_snap].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(8,'pending: provisioning')
INSERT INTO [its_snap].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(9,'pending: workflow')
INSERT INTO [its_snap].[dbo].[SNAP_Workflow_State_Type]([pkId],[typeName])
VALUES(10,'workflow created')

--// Insert 2 years of Holidays and Weekends
print ''
print 'Insert holidays and weekends'
DECLARE @FirstSat datetime, @x int
SELECT @FirstSat = '1/2/2010', @x = 1 
print ''
PRINT 'Adding 2010 Weekends'
WHILE @x < 104
	BEGIN
		INSERT INTO [dbo].[SNAP_Weekends_and_Holidays](dayOfWeekDate, dayName)
		SELECT DATEADD(ww,@x,@FirstSat),   'SAT' UNION ALL
		SELECT DATEADD(ww,@x,@FirstSat+1), 'SUN'
		
		IF @x = 52
			BEGIN
				print ''
				print 'Adding 2011 Weekends'
				SELECT @FirstSat = DATEADD(yy,1,@FirstSat)
			END
		
		SELECT @x = @x + 1
	END
print ''
print 'Adding 2010 and 2011 Apollo Holidays'
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