USE [BPS_SupplementalAccess]

declare @requestId int
declare @workflowStateId int
declare @workflowId int
declare @dueDate smalldatetime
declare @today smalldatetime

set @today = getdate()
set @dueDate = dbo.udf_get_next_business_day(@today, 1)

DECLARE db_cursor CURSOR FOR
select r.pkId, w.pkid, ws.pkId from dbo.SNAP_Requests r
join SNAP_Workflow w on r.pkId = w.requestId
join SNAP_Actors a on w.actorId = a.pkId
join SNAP_Workflow_State ws on w.pkId = ws.workflowId
where r.pkId IN(1013,1012,1011,1010,1009,1031,1034,1042,1041,1091,1094,1111,1110,1106,1114,1113,1146,1145,1119,1155,1154,1184,1138,1137,1132,1131,1130,1238,1265,1263,1040,1038,1290,1287,1182,1347,1345,1358,1357,1353,1391,1385,1152,1406,1410,1407)
and r.statusEnum = 1
and a.actor_groupId = 1
and ws.completedDate is null
group by r.pkId, r.statusEnum, w.pkid, ws.pkId

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @requestId, @workflowId, @workflowStateId

WHILE @@FETCH_STATUS = 0   
	BEGIN

		update dbo.SNAP_Requests set statusEnum = 3
		where pkId = @requestId

		update dbo.SNAP_Workflow_State set completedDate = @today
		where pkId = @workflowStateId

		insert into dbo.SNAP_Workflow_State(workflowId,workflowStatusEnum,notifyDate,dueDate,completedDate)
		select @workflowId as workflowId,
		12 as workflowStatusEnum,
		@today as notifyDate,
		@dueDate as dueDate,
		@today as completedDate

		insert into dbo.SNAP_Workflow_Comments(workflowId,commentTypeEnum,commentText,createdDate)
		select @workflowId as workflowId,
		10 as commentTypeEnum,
		'This request has not been updated for over 14 days.' as commentText,
		@today as createdDate
		FETCH NEXT FROM db_cursor INTO @requestId, @workflowId, @workflowStateId
	END   

CLOSE db_cursor   
DEALLOCATE db_cursor