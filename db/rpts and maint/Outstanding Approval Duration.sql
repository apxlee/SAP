--//Outstanding Approval Duration

USE [BPS_SupplementalAccess]
DECLARE @today as smalldatetime
SET @today = getdate() 

select f.requestId as [RequestId], a.displayName as [Approver Name], (DateDiff(d,s.dueDate, @today) - (select count(dayOfWeekDate) from dbo.SNAP_Weekends_and_Holidays
where dayOfWeekDate between s.dueDate and @today)) as [Days Late],
r.userDisplayName as [Affected End User]
from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
where (r.statusEnum = 2)
and r.pkid = f.requestId 
and f.pkid = s.workflowId
and f.actorid = a.pkid
and s.workflowStatusEnum = 7
and completedDate is null
and (DateDiff(d,s.dueDate, @today) - (select count(dayOfWeekDate) from dbo.SNAP_Weekends_and_Holidays
where dayOfWeekDate between s.dueDate and @today)) > 0
            order by a.displayName, r.pkId