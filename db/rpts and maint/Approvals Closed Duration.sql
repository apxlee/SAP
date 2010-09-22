--//Approvals Closed Duration

USE [BPS_SupplementalAccess]

select f.requestId, r.userDisplayName, a.displayName as [Approver Name], s.notifyDate, s.dueDate, s.completedDate,
(DateDiff(d,s.dueDate, s.completedDate) - (select count(dayOfWeekDate) from dbo.SNAP_Weekends_and_Holidays
where dayOfWeekDate between s.dueDate and s.completedDate)) as [Days Late]
            from dbo.SNAP_Requests r, SNAP_Workflow f, SNAP_Workflow_State s, dbo.SNAP_Actors a
            where (r.statusEnum = 3)
                  and r.pkid = f.requestId 
                  and f.pkid = s.workflowId
                  and f.actorid = a.pkid
				  and f.actorid <> 1
                  and s.workflowStatusEnum = 0
                  and completedDate is not null
            order by a.displayName, r.pkId