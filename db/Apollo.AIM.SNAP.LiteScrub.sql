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
