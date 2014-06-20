CREATE PROCEDURE [dbo].[IsJobRunning_prc]
(
	@job_name sysname = null
,	@job_id uniqueidentifier = null
)
AS
BEGIN
	set nocount on

	declare @return bit
	set @return = 0

	declare @xp_results table(
		job_id					UNIQUEIDENTIFIER NOT NULL,
		last_run_date			INT              NOT NULL,
		last_run_time			INT              NOT NULL,
		next_run_date			INT              NOT NULL,
		next_run_time			INT              NOT NULL,
		next_run_schedule_id	INT              NOT NULL,
		requested_to_run		INT              NOT NULL,
		request_source			INT              NOT NULL,
		request_source_id		sysname          COLLATE database_default NULL,
		running					INT              NOT NULL,
		current_step			INT              NOT NULL,
		current_retry_attempt	INT              NOT NULL,
		job_state				INT              NOT NULL
	)

	if(@job_id is null)
	begin
		select @job_id = j.job_id
		from msdb.dbo.sysjobs j 
		where j.name = @job_name
	end

	insert into @xp_results  
	exec master.dbo.xp_sqlagent_enum_jobs 1, 'irrelevant'

	if exists (
		select 1 
		from @xp_results  r 
		where r.running = 1
		and r.job_id = @job_id
	)
	begin 
	  set @return = 1
	end

	--This is not proper use of a return, but it beats breaking on nested insert/exec errors :/
	return @return;
end