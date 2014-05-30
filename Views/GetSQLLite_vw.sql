CREATE view dbo.GetSQLLite_vw
as    

select   
	req.session_id
,	req.request_id
,	req.blocking_session_id
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
,	req.percent_complete
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
where 1=1
and req.session_id <> @@spid
