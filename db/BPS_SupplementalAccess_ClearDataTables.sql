USE [BPS_SupplementalAccess]

print 'scrubbing data tables and reseeding primary kees'

delete from dbo.SNAP_Actors
print 'scrub dbo.SNAP_Actors'

DBCC CHECKIDENT ("dbo.SNAP_Actors", RESEED, 0);
print 'reseed dbo.SNAP_Actors'

delete from dbo.SNAP_Actor_Group
print 'scrub dbo.SNAP_Actor_Group'

DBCC CHECKIDENT ("dbo.SNAP_Actor_Group", RESEED, 0);
print 'reseed dbo.SNAP_Actor_Group'

delete from dbo.SNAP_Access_Details_Form
print 'scrub dbo.SNAP_Access_Details_Form'

DBCC CHECKIDENT ("dbo.SNAP_Access_Details_Form", RESEED, 0);
print 'reseed dbo.SNAP_Access_Details_Form'

delete from dbo.SNAP_Access_User_Text
print 'scrub dbo.SNAP_Access_User_Text'

DBCC CHECKIDENT ("dbo.SNAP_Access_User_Text", RESEED, 0);
print 'reseed dbo.SNAP_Access_User_Text'

delete from dbo.SNAP_Request_Comments
print 'scrub dbo.SNAP_Request_Comments'

DBCC CHECKIDENT ("dbo.SNAP_Request_Comments", RESEED, 0);
print 'reseed dbo.SNAP_Request_Comments'

delete from dbo.SNAP_Requests
print 'scrub dbo.SNAP_Requests'

DBCC CHECKIDENT ("dbo.SNAP_Requests", RESEED, 1000);
print 'reseed dbo.SNAP_Requests'

delete from dbo.SNAP_Workflow
print 'scrub dbo.SNAP_Workflow'

DBCC CHECKIDENT ("dbo.SNAP_Workflow", RESEED, 0);
print 'reseed dbo.SNAP_Workflow'

delete from dbo.SNAP_Workflow_Comments
print 'scrub dbo.SNAP_Workflow_Comments'

DBCC CHECKIDENT ("dbo.SNAP_Workflow_Comments", RESEED, 0);
print 'reseed dbo.SNAP_Workflow_Comments'

delete from dbo.SNAP_Workflow_State
print 'scrub dbo.SNAP_Workflow_State'

DBCC CHECKIDENT ("dbo.SNAP_Workflow_State", RESEED, 0);
print 'reseed dbo.SNAP_Workflow_State'

delete from dbo.SNAP_Actor_Group_Type
print 'scrub dbo.SNAP_Actor_Group_Type'

delete from dbo.SNAP_Request_State_Type
print 'scrub dbo.SNAP_Request_State_Type'

delete from dbo.SNAP_Comments_Type
print 'scrub dbo.SNAP_Comments_Type'

delete from dbo.SNAP_Workflow_State_Type
print 'scrub dbo.SNAP_Workflow_State_Type'

delete from dbo.SNAP_Weekends_and_Holidays
print 'scrub dbo.SNAP_Weekends_and_Holidays'