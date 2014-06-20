CREATE PROCEDURE [dbo].[StartJob_prc]
(
	@job_id uniqueidentifier = null
,	@job_name sysname = null
)
AS
BEGIN
	if(@job_id is null)
	begin
		select @job_id = j.job_id
		from msdb.dbo.sysjobs j 
		where j.name = @job_name;
	end

	declare @running int;
	exec @running = dbo.IsJobRunning_prc @job_id = @job_id	

	if(@running = 0)
	begin
		exec msdb.dbo.sp_start_job @job_id = @job_id;
	end

END