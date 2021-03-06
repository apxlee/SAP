--//Outstanding Approval Duration

USE [BPS_SupplementalAccess]
DECLARE @today as smalldatetime

DECLARE @monthStart smalldatetime
DECLARE @monthEnd smalldatetime

SET @today = getdate() 
SET @monthStart = convert(smalldatetime, '11/01/2010', 101)
SET @monthEnd = convert(smalldatetime, '11/30/2010', 101)

select f.requestId as [RequestId], a.displayName as [Approver Name], 
(DateDiff(d,s.dueDate, @monthEnd) - (select count(dayOfWeekDate) from dbo.SNAP_Weekends_and_Holidays
where dayOfWeekDate between s.dueDate and @monthEnd) + (select count(dayOfWeekDate) from dbo.SNAP_Weekends_and_Holidays
where dayOfWeekDate between @monthEnd and s.dueDate)) as [Days Late],
r.userDisplayName as [Affected End User]
from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
where (r.statusEnum = 2)
and r.pkid = f.requestId 
and f.pkid = s.workflowId
and f.actorid = a.pkid
and s.workflowStatusEnum = 7
and s.dueDate between @monthStart and @monthEnd
and completedDate is null
and (DateDiff(d,s.dueDate, @today) - (select count(dayOfWeekDate) from dbo.SNAP_Weekends_and_Holidays
where dayOfWeekDate between s.dueDate and @today)) > 0
order by [Days Late] desc