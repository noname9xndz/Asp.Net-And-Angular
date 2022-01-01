SELECT f.Id,
       f.Name,
	   f.ParentId,
	   sum(case when sa.Id = 'CREATE' then 1 else 0 end) as HasCreate,
	   sum(case when sa.Id = 'UPDATE' then 1 else 0 end) as HasUpdate,
	   sum(case when sa.Id = 'DELETE' then 1 else 0 end) as HasDelete,
	   sum(case when sa.Id = 'VIEW' then 1 else 0 end) as HasView,
	   sum(case when sa.Id = 'APPROVE' then 1 else 0 end) as HasApprove
from Functions f join CommandInFunctions cif on f.Id = cif.FunctionId
left join Commands sa on cif.CommandId = sa.Id
GROUP BY f.Id,f.Name, f.ParentId
order BY f.ParentId