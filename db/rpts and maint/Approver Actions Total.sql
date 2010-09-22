CREATE TABLE #approvalTotals
(
	pkId INT,
	displayName nvarchar(128),
	daysLate decimal(18,2)
)
insert into #approvalTotals(pkId,displayName,daysLate)
select r.pkId as pkId, a.displayName as displayName,
(DateDiff(d,ws.dueDate, ws.completedDate) - (select count(dayOfWeekDate) from dbo.SNAP_Weekends_and_Holidays
where dayOfWeekDate between ws.dueDate and ws.completedDate) + (select count(dayOfWeekDate) from dbo.SNAP_Weekends_and_Holidays
where dayOfWeekDate between ws.completedDate and ws.dueDate)) as daysLate
from dbo.SNAP_Requests r
join dbo.SNAP_Workflow w on r.pkId = w.requestId
join dbo.SNAP_Workflow_State ws on w.pkId = ws.workflowId
join dbo.SNAP_Actors a on w.actorId = a.pkId
where a.pkId <> 1 
and ws.pkId IN(select max(subWs.pkId) from dbo.SNAP_Workflow subW
join dbo.SNAP_Workflow_State subWs on subW.pkId = subWs.workflowId
join dbo.SNAP_Actors subA on subW.actorId = subA.pkId
where subW.requestId = w.requestId
and subWs.workflowStatusEnum NOT IN(5,7)
and subA.userId = a.userId)
group by r.pkId,a.displayName,ws.dueDate, ws.completedDate

DECLARE @totalAverage as decimal(18,2)

select @totalAverage = (select AVG(daysLate) from #approvalTotals)

select displayName as [Approver Name],
count(pkId) as [Total Requests],
CAST(AVG(daysLate) as decimal(18,2)) as [Average Days],
CAST((@totalAverage - AVG(daysLate)) as decimal(18,2)) as [Approver Rating]
from #approvalTotals
group by displayName
order by displayName

DROP TABLE #approvalTotals

select r.pkId as pkId, a.displayName as displayName,
(DateDiff(d,ws.dueDate, ws.completedDate) - (select count(dayOfWeekDate) from dbo.SNAP_Weekends_and_Holidays
where dayOfWeekDate between ws.dueDate and ws.completedDate) + (select count(dayOfWeekDate) from dbo.SNAP_Weekends_and_Holidays
where dayOfWeekDate between ws.completedDate and ws.dueDate)) as daysLate
from dbo.SNAP_Requests r
join dbo.SNAP_Workflow w on r.pkId = w.requestId
join dbo.SNAP_Workflow_State ws on w.pkId = ws.workflowId
join dbo.SNAP_Actors a on w.actorId = a.pkId
where a.pkId <> 1 
and ws.pkId IN(select max(subWs.pkId) from dbo.SNAP_Workflow subW
join dbo.SNAP_Workflow_State subWs on subW.pkId = subWs.workflowId
join dbo.SNAP_Actors subA on subW.actorId = subA.pkId
where subW.requestId = w.requestId
and subWs.workflowStatusEnum NOT IN(5,7)
and subA.userId = a.userId)
group by r.pkId,a.displayName,ws.dueDate, ws.completedDate
order by a.displayName