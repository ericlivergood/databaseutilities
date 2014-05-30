create view BlockingChain_vw
as

	with blocked as
	(
			select
					session_id
			,       blocking_session_id
			,       chain = convert(varchar(100), convert(varchar(10), session_id) + ' -> ' + convert(varchar(10), blocking_session_id))
			,       1 as level
			from sys.dm_exec_requests
			where blocking_session_id <> 0

			union all
	        
			select
					b.session_id
			,       r.blocking_session_id
			,       chain = convert(varchar(100), b.chain + ' -> ' + convert(varchar(10), r.blocking_session_id))
			,       b.level + 1 as level
			from blocked b
			 inner join sys.dm_exec_requests r
					on b.blocking_session_id = r.session_id
					and r.blocking_session_id <> 0
	)
	select top(32000) b.chain --want to force order by 
	from blocked b
	where not exists(
		select 1 from blocked
		where session_id = b.session_id
		and level > b.level
	)
	order by b.level, b.session_id;
