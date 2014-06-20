CREATE PROCEDURE [dbo].[StopJob_prc]
(
	@job_id uniqueidentifier = null
,	@job_name sysname = null
,	@waitForSeconds int = 60
)
AS
BEGIN
	if(@job_id is null)
	begin
		select @job_id = j.job_id
		from msdb.dbo.sysjobs j 
		where j.name = @job_name;
	end

	declare @waitUntil datetime, @running int;
		
	
	exec @running = dbo.IsJobRunning_prc @job_id = @job_id	

	set @waitUntil = DATEADD(second, @waitForSeconds, getdate());

	while(@running = 1 and GETDATE() < @waitForSeconds)
	begin
		exec msdb.dbo.sp_stop_job @job_id = @job_id;

		waitfor delay '00:00:01';

		exec @running = dbo.IsJobRunning_prc @job_id = @job_id	
	end
END