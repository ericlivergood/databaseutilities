CREATE view dbo.GetSQL_vw
as    

select   
	req.session_id
,	req.request_id
,	j.name as job_name
,	req.blocking_session_id
,	substring(
		st.text
	,	(req.statement_start_offset / 2)  
	,	((case req.statement_end_offset
			when -1 then datalength(st.text)
            else req.statement_end_offset
			end - req.statement_start_offset
		) / 2)
	) as statement_text
,	object_name(st.objectid, st.dbid) as object_name
,	req.cpu_time
,	req.writes
,	req.reads
,	req.logical_reads
,	req.wait_type
,	req.wait_time
,	sess.host_name
,	sess.login_name
,	sess.program_name
,	req.command
,	req.total_elapsed_time
,	req.granted_query_memory
,	req.last_wait_type
,	req.wait_resource
,	db_name(req.database_id) as database_name
,	user_name(req.user_id) as [user_name]
,	req.statement_start_offset
,	req.statement_end_offset
,	datalength(st.text) as statementLength
,	req.percent_complete
,	ph.query_plan
,	req.open_transaction_count
,	req.row_count
,	req.executing_managed_code
,	conn.net_transport
,	conn.client_net_address
from     sys.dm_exec_requests req (nolock)
	left outer join sys.dm_exec_sessions sess (nolock)
		on req.session_id = sess.session_id
	left outer join sys.dm_exec_connections conn (nolock)
		on req.session_id = conn.session_id
	left outer join msdb.dbo.sysjobs j with (nolock)
		on sys.fn_varbintohexstr(j.job_id) = substring(
			rtrim(replace(sess.program_name , 'SQLAgent - TSQL JobStep (Job ' , '')) , 1 , 
						charindex(' : Step' , rtrim(replace(sess.program_name , 'SQLAgent - TSQL JobStep (Job ' , '')))
						)
	outer apply sys.dm_exec_sql_text(req.sql_handle) as st
	outer apply sys.dm_exec_query_plan(req.plan_handle) as ph
where 1=1
and req.session_id <> @@spid
