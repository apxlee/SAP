declare @expirationDate as smalldatetime
set @expirationDate = dateadd(d,-15,getdate())
select pkId, userId, (select 'https://access-prod.apollogrp.edu/snap/RequestForm.aspx?requestId=' + cast(pkId as nvarchar(20))) as link,
datediff(d,getdate(),lastModifiedDate) as daysOld 
from dbo.SNAP_Requests
where statusEnum = 1 and lastModifiedDate < @expirationDate
order by daysOld asc
