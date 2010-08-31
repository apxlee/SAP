declare @requestId int
declare @manager nvarchar(10)
declare @workflowStateId int
declare @workflowId int
declare @dueDate smalldatetime
declare @today smalldatetime

set @today = getdate()
set @dueDate = dbo.udf_get_next_business_day(@today, 1)
set @requestId = 1264
set @manager = 'ksmonday'

--//BEGIN Manager state update
select @workflowStateId = ws.pkId, @workflowId = ws.workflowId from dbo.SNAP_Requests r
join SNAP_Workflow w on r.pkId = w.requestId
join SNAP_Actors a on w.actorId = a.pkId
join SNAP_Workflow_State ws on w.pkId = ws.workflowId
where r.pkId = @requestId 
and a.actor_groupId = 4 
and a.userId = @manager
and ws.workflowStatusEnum = 7

--//UPDATE pending approval state
update SNAP_Workflow_State set completedDate = @today
where pkId = @workflowStateId

--//INSERT approved state
insert into SNAP_Workflow_State(workflowId,workflowStatusEnum,notifyDate,dueDate,completedDate)
select @workflowId as workflowId,
0 as workflowStatusEnum,
@today as notifyDate,
@dueDate as dueDate,
@today as completedDate
--//END Manager Fix

--//BEGIN Move to next in queue

--//UPDATE not active workflow states
update SNAP_Workflow_State 
set completedDate = @today
where pkId in(select ws.pkId from dbo.SNAP_Requests r
join SNAP_Workflow w on r.pkId = w.requestId
join SNAP_Actors a on w.actorId = a.pkId
join SNAP_Actor_Group ag on a.actor_groupId = ag.pkId
join SNAP_Workflow_State ws on w.pkId = ws.workflowId
where r.pkId = @requestId
and ag.actorGroupType = 1
and ws.workflowStatusEnum = 5)

--//INSERT pending approval states
DECLARE db_cursor CURSOR FOR
select ws.workflowId from dbo.SNAP_Requests r
join SNAP_Workflow w on r.pkId = w.requestId
join SNAP_Actors a on w.actorId = a.pkId
join SNAP_Actor_Group ag on a.actor_groupId = ag.pkId
join SNAP_Workflow_State ws on w.pkId = ws.workflowId
where r.pkId = @requestId
and ag.actorGroupType = 1
and ws.workflowStatusEnum = 5

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @workflowId

WHILE @@FETCH_STATUS = 0   
BEGIN   
		insert into SNAP_Workflow_State(workflowId,workflowStatusEnum,notifyDate,dueDate,completedDate)
		select @workflowId as workflowId,
		7 as workflowStatusEnum,
		@today as notifyDate,
		@dueDate as dueDate,
		null as completedDate
       FETCH NEXT FROM db_cursor INTO @workflowId   
END   

CLOSE db_cursor   
DEALLOCATE db_cursor

set @requestId = 1268
set @manager = 'ksmonday'

--//BEGIN Manager state update
select @workflowStateId = ws.pkId, @workflowId = ws.workflowId from dbo.SNAP_Requests r
join SNAP_Workflow w on r.pkId = w.requestId
join SNAP_Actors a on w.actorId = a.pkId
join SNAP_Workflow_State ws on w.pkId = ws.workflowId
where r.pkId = @requestId 
and a.actor_groupId = 4 
and a.userId = @manager
and ws.workflowStatusEnum = 7

--//UPDATE pending approval state
update SNAP_Workflow_State set completedDate = @today
where pkId = @workflowStateId

--//INSERT approved state
insert into SNAP_Workflow_State(workflowId,workflowStatusEnum,notifyDate,dueDate,completedDate)
select @workflowId as workflowId,
0 as workflowStatusEnum,
@today as notifyDate,
@dueDate as dueDate,
@today as completedDate
--//END Manager Fix

--//BEGIN Move to next in queue

--//UPDATE not active workflow states
update SNAP_Workflow_State 
set completedDate = @today
where pkId in(select ws.pkId from dbo.SNAP_Requests r
join SNAP_Workflow w on r.pkId = w.requestId
join SNAP_Actors a on w.actorId = a.pkId
join SNAP_Actor_Group ag on a.actor_groupId = ag.pkId
join SNAP_Workflow_State ws on w.pkId = ws.workflowId
where r.pkId = @requestId
and ag.actorGroupType = 1
and ws.workflowStatusEnum = 5)

--//INSERT pending approval states
DECLARE db_cursor CURSOR FOR
select ws.workflowId from dbo.SNAP_Requests r
join SNAP_Workflow w on r.pkId = w.requestId
join SNAP_Actors a on w.actorId = a.pkId
join SNAP_Actor_Group ag on a.actor_groupId = ag.pkId
join SNAP_Workflow_State ws on w.pkId = ws.workflowId
where r.pkId = @requestId
and ag.actorGroupType = 1
and ws.workflowStatusEnum = 5

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @workflowId

WHILE @@FETCH_STATUS = 0   
BEGIN   
		insert into SNAP_Workflow_State(workflowId,workflowStatusEnum,notifyDate,dueDate,completedDate)
		select @workflowId as workflowId,
		7 as workflowStatusEnum,
		@today as notifyDate,
		@dueDate as dueDate,
		null as completedDate
       FETCH NEXT FROM db_cursor INTO @workflowId   
END   

CLOSE db_cursor   
DEALLOCATE db_cursor

set @requestId = 1340
set @manager = 'ksmonday'

--//BEGIN Manager state update
select @workflowStateId = ws.pkId, @workflowId = ws.workflowId from dbo.SNAP_Requests r
join SNAP_Workflow w on r.pkId = w.requestId
join SNAP_Actors a on w.actorId = a.pkId
join SNAP_Workflow_State ws on w.pkId = ws.workflowId
where r.pkId = @requestId 
and a.actor_groupId = 4 
and a.userId = @manager
and ws.workflowStatusEnum = 7

--//UPDATE pending approval state
update SNAP_Workflow_State set completedDate = @today
where pkId = @workflowStateId

--//INSERT approved state
insert into SNAP_Workflow_State(workflowId,workflowStatusEnum,notifyDate,dueDate,completedDate)
select @workflowId as workflowId,
0 as workflowStatusEnum,
@today as notifyDate,
@dueDate as dueDate,
@today as completedDate
--//END Manager Fix

--//BEGIN Move to next in queue

--//UPDATE not active workflow states
update SNAP_Workflow_State 
set completedDate = @today
where pkId in(select ws.pkId from dbo.SNAP_Requests r
join SNAP_Workflow w on r.pkId = w.requestId
join SNAP_Actors a on w.actorId = a.pkId
join SNAP_Actor_Group ag on a.actor_groupId = ag.pkId
join SNAP_Workflow_State ws on w.pkId = ws.workflowId
where r.pkId = @requestId
and ag.actorGroupType = 1
and ws.workflowStatusEnum = 5)

--//INSERT pending approval states
DECLARE db_cursor CURSOR FOR
select ws.workflowId from dbo.SNAP_Requests r
join SNAP_Workflow w on r.pkId = w.requestId
join SNAP_Actors a on w.actorId = a.pkId
join SNAP_Actor_Group ag on a.actor_groupId = ag.pkId
join SNAP_Workflow_State ws on w.pkId = ws.workflowId
where r.pkId = @requestId
and ag.actorGroupType = 1
and ws.workflowStatusEnum = 5

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @workflowId

WHILE @@FETCH_STATUS = 0   
BEGIN   
		insert into SNAP_Workflow_State(workflowId,workflowStatusEnum,notifyDate,dueDate,completedDate)
		select @workflowId as workflowId,
		7 as workflowStatusEnum,
		@today as notifyDate,
		@dueDate as dueDate,
		null as completedDate
       FETCH NEXT FROM db_cursor INTO @workflowId   
END   

CLOSE db_cursor   
DEALLOCATE db_cursor

